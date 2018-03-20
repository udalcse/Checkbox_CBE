using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using System.Data;
using Checkbox.Security.Principal;
using System.Text.RegularExpressions;
using Checkbox.Management;
using Checkbox.Common;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Search
{
    /// <summary>
    /// Implements multithreaded logic for Universal Search
    /// </summary>
    public static class SearchManager
    {
        /// <summary>
        /// Collect all available objects
        /// </summary>
        /// <returns></returns>
        public static Guid InitializeAvailableObjects(CheckboxPrincipal callingPrincipal, int expPeriodSeconds)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Collect_Available_Objects");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name);
            command.AddInParameter("ExpirationPeriodSeconds", DbType.Int32, ApplicationManager.AppSettings.SearchAccessibleObjectExpPeriodSeconds);
            command.AddOutParameter("SearchRequestID", DbType.Guid, 64);

            db.ExecuteNonQuery(command);
            Guid res = (Guid)command.GetParameterValue("SearchRequestID");

            return res;
        }

        /// <summary>
        /// Get request status or create a new request
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <param name="expPeriodSeconds"></param>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public static string GetStatus(CheckboxPrincipal callingPrincipal, string searchTerm, ref Guid RequestID)
        {
            //System.Diagnostics.Trace.WriteLine(string.Format("!!!!!!!{1} search status requested [{0}]", searchTerm, RequestID));
            //DateTime dtStart = DateTime.Now;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_GetRequest");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name);
            command.AddInParameter("SearchTerm", DbType.String, searchTerm);
            command.AddInParameter("ResultsExpirationPeriodSeconds", DbType.Int64, ApplicationManager.AppSettings.SearchResultsExpPeriodSeconds);
            command.AddInParameter("ObjectsExpirationPeriodSeconds", DbType.Int64, ApplicationManager.AppSettings.SearchResultsExpPeriodSeconds);
            command.AddInParameter("CollectObjectIfNone", DbType.Boolean, !callingPrincipal.IsInRole("System Administrator"));
            command.AddInParameter("RequestID", DbType.Guid, RequestID == null || RequestID.Equals(Guid.Empty) ? null : (Guid?)RequestID);

            string res = "Error";

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        RequestID = (Guid)reader["SearchRequestID"];
                        res = (string)reader["Status"];
                        //System.Diagnostics.Trace.WriteLine(string.Format("{1} search status is {2} for [{0}]. Elapsed: {3}", searchTerm, RequestID, res, 
                        //    (double)DateTime.Now.Subtract(dtStart).TotalMilliseconds / 1000.0));
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    reader.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// Collects all search results
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="RequestID"></param>
        /// <param name="Term"></param>
        /// <returns></returns>
        public static SearchAnswerData[] CollectResults(CheckboxPrincipal callingPrincipal, Guid RequestID, string Term)
        {
            //System.Diagnostics.Trace.WriteLine(string.Format("{1} search results collected for {0}", Term, RequestID));
            //DateTime dtStart = DateTime.Now;

            List<SearchAnswerData> results = new List<SearchAnswerData>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Collect_Results");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name);
            command.AddInParameter("RequestID", DbType.Guid, RequestID);
            command.AddInParameter("Term", DbType.String, Term);
            command.AddInParameter("PageSize", DbType.Int32, ApplicationManager.AppSettings.SearchPageSize);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        SearchAnswerData data = new SearchAnswerData();

                        data.ObjectType = (string)reader["ObjectType"];
                        data.ObjectID = reader["ObjectID"] is DBNull ? null : (long?)reader["ObjectID"];
                        data.ObjectGUID = reader["ObjectGUID"] is DBNull ? null : (Guid?)reader["ObjectGUID"];
                        data.ObjectIDString = reader["ObjectIDString"] is DBNull ? null : (string)reader["ObjectIDString"];
                        data.MatchedText = Utilities.StripHtml((string)reader["MatchedText"], 1000);
                        data.MatchedField = (string)reader["MatchedField"];
                        data.Title = Utilities.StripHtml((string)reader["Title"], 50);

                        results.Add(data);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return results.ToArray();
        }

        #region Threading
        /// <summary>
        /// Runs a search request
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="RequestID"></param>
        /// <param name="Term"></param>
        public static void RunSearch(CheckboxPrincipal callingPrincipal, Guid RequestID, string Term)
        {
            List<SearchAnswerData> results = new List<SearchAnswerData>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Run");
            command.AddInParameter("RequestID", DbType.Guid, RequestID);
            
            //get all available object types for this user
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string ObjectType = (string)reader["ObjectType"];
                        //run in the separate thread data collection routine
                        ThreadPool.QueueUserWorkItem(new WaitCallback(searchWorkerRoutine), new SearchParams() 
                            { 
                                 ApplicationContext = ApplicationManager.ApplicationDataContext,
                                 EntityType = ObjectType,
                                 UserName = callingPrincipal.Identity.Name,
                                 RequestID = RequestID,
                                 Term = Term,
                                 isAdmin = callingPrincipal.IsInRole("System Administrator")
                            });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //one time per hour run a routine that clears old cached searches
            if (LastCleanCacheDate == null || DateTime.Now.Subtract(LastCleanCacheDate.Value).TotalHours >= 1)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(clearOldRequests), null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DateTime? LastCleanCacheDate
        {
            get;
            set;
        }

        /// <summary>
        /// Clears caches
        /// </summary>
        /// <param name="param"></param>
        public static void clearOldRequests(object param)
        {
            try
            {
                LastCleanCacheDate = DateTime.Now;

                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_ClearOldRequests");
                command.AddInParameter("CachePeriodDays", DbType.Int32, ApplicationManager.AppSettings.SearchCachePeriodDays);

                db.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessInternal");
            }
        }

        /// <summary>
        /// Params for search routines
        /// </summary>
        struct SearchParams
        {
            public string EntityType { get; set; }
            public string UserName { get; set; }
            public Guid RequestID { get; set; }
            public string Term { get; set; }
            public bool isAdmin { get; set; }
            public string ApplicationContext { get; set; }
        }

        /// <summary>
        /// The method being runned in a separate thread. 
        /// It calls Timeline_run procedure that collects events from the Checkbox database to the timeline results table.
        /// </summary>
        /// <param name="?"></param>
        private static void searchWorkerRoutine(object param)
        {
            try
            {
                SearchParams sp = (SearchParams)param;
                Thread.SetData(
                    Thread.GetNamedDataSlot(DataContextProvider.APPLICATION_CONTEXT_KEY),
                    sp.ApplicationContext);
                //DateTime dtStart = DateTime.Now;
                //System.Diagnostics.Trace.WriteLine(string.Format("{0} search started", sp.EntityType));
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper(string.Format("ckbx_sp_Search_{0}", sp.EntityType) + (sp.isAdmin ? "_Admin" : ""));
                command.AddInParameter("RequestID", DbType.Guid, sp.RequestID);
                command.AddInParameter("MaxObjectsToCache", DbType.Int32, ApplicationManager.AppSettings.SearchMaxResultRecordsPerObjectType);

                db.ExecuteNonQuery(command);
                //System.Diagnostics.Trace.WriteLine(string.Format("{0} search finished for [{1}]. Elapsed: {2}", sp.EntityType, sp.RequestID,
                //    (double)DateTime.Now.Subtract(dtStart).TotalMilliseconds / 1000.0));
            }
            catch (Exception ex)
            {
                try
                {
                    ExceptionPolicy.HandleException(ex, "BusinessInternal");
                }
                catch
                {
                    
                }
            }
        }
        #endregion Threading


        /// <summary>
        /// Returns object types that can be used in the search
        /// </summary>
        /// <returns></returns>
        public static SearchSettingsInfo[] GetSearchSettings()
        {
            List<SearchSettingsInfo> results = new List<SearchSettingsInfo>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Settings_Get");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        SearchSettingsInfo data = new SearchSettingsInfo();

                        data.ObjectType = (string)reader["ObjectType"];
                        data.Included = (bool)reader["Included"];
                        data.Order = (int)reader["Order"];

                        results.Add(data);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            foreach (var r in results)
            {
                r.Roles = string.Join(",", getObjectTypeRoles(r.ObjectType));
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns roles which can user this object type in the search
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static string[] getObjectTypeRoles(string objectType)
        {
            List<string> results = new List<string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Get_ObjectType_Roles");
            command.AddInParameter("ObjectType", DbType.String, objectType);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        results.Add((string)reader["RoleName"]);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return results.ToArray();
        }


        /// <summary>
        /// Updates the order for the object type
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="order"></param>
        public static void UpdateSearchResultsOrder(string objectType, int order)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Settings_Set_Order");
            command.AddInParameter("ObjectType", DbType.String, objectType);
            command.AddInParameter("Order", DbType.Int32, order);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Updates the roles list for the object type
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="roles"></param>
        public static void UpdateObjectsRoles(string objectType, string roles)
        {
            string[] newRoles = roles.Split(',');
            string[] oldRoles = getObjectTypeRoles(objectType);

            foreach (var r in newRoles)
            {
                if (!oldRoles.Contains(r))
                    addRoleToObjectType(objectType, r);
            }

            foreach (var r in oldRoles)
            {
                if (!newRoles.Contains(r))
                    deleteRoleFromObjectType(objectType, r);
            }
        }

        /// <summary>
        /// Adds a role to the object type
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="role"></param>
        private static void addRoleToObjectType(string objectType, string role)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Settings_Add_ObjectType_Role");
            command.AddInParameter("ObjectType", DbType.String, objectType);
            command.AddInParameter("Role", DbType.String, role);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Removes a role from the object type
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="role"></param>
        private static void deleteRoleFromObjectType(string objectType, string role)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Settings_Delete_ObjectType_Role");
            command.AddInParameter("ObjectType", DbType.String, objectType);
            command.AddInParameter("Role", DbType.String, role);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Includes or excludes object type from the search
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="included"></param>
        public static void ToggleSearchObjectType(string objectType, bool included)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Search_Settings_Include_ObjectType");
            command.AddInParameter("ObjectType", DbType.String, objectType);
            command.AddInParameter("Include", DbType.Boolean, included);
            db.ExecuteNonQuery(command);
        }
    }
}
