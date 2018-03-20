using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using System.Data;
using Checkbox.Security.Principal;
using System.Text.RegularExpressions;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Management;

namespace Checkbox.Timeline
{
    /// <summary>
    /// Processes requests for timeline
    /// </summary>
    public static class TimelineManager
    {
        /// <summary>
        /// Get Request Status
        /// </summary>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public static TimelineRequestStatus GetRequestStatus(CheckboxPrincipal callingPrincipal, long RequestID, out string message)
        {
            message = null;
            if (RequestID == 0)
                return TimelineRequestStatus.None;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_GetStatus");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name);
            command.AddInParameter("RequestID", DbType.Int64, RequestID);

            string status = "None";

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (!(reader["RequestStatus"] is DBNull))
                            status = (string)reader["RequestStatus"];
                        if (!(reader["Message"] is DBNull))
                            message = (string)reader["Message"];
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

            return (TimelineRequestStatus)Enum.Parse(typeof(TimelineRequestStatus), status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void GetPeriodDates(int period, out DateTime? start, out DateTime? end)
        {            
            DateTime now = DateTime.Now;
            start = GetStartFilterDate(period);
            end = period > 0 ? (DateTime?)now : null;
        }

        /// <summary>
        /// Get request ID
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="manager"></param>
        /// <param name="timelineRequestExpiration"></param>
        /// <returns></returns>
        public static long GetRequest(CheckboxPrincipal callingPrincipal, string manager, int timelineRequestExpiration)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Get");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name.Replace("'", "&#39;"));
            command.AddInParameter("Manager", DbType.String, manager);
            command.AddInParameter("ExpirationPeriodSeconds", DbType.Int32, timelineRequestExpiration);

            long RequestID = 0;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (!(reader["RequestID"] is DBNull))
                            RequestID = (long)reader["RequestID"];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (RequestID != 0)
            {
                runRequest(RequestID);
            }

            return RequestID;
        }

        /// <summary>
        /// Returns aggregated results for request
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public static TimelineAggregatedResult[] GetResults(CheckboxPrincipal callingPrincipal, long RequestID, long parentObjectID, string parentObjectType)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Aggregate");
            command.AddInParameter("UserID", DbType.String, callingPrincipal.Identity.Name);
            command.AddInParameter("RequestID", DbType.Int32, RequestID);
            command.AddInParameter("ParentObjectID", DbType.Int64, parentObjectID);
            command.AddInParameter("ParentObjectType", DbType.String, parentObjectType);

            List<TimelineAggregatedResult> results = new List<TimelineAggregatedResult>();
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        TimelineAggregatedResult tar = new TimelineAggregatedResult();
                        
                        tar.EventID = (int)reader["EventID"];
                        tar.UserID = reader["UserID"] is DBNull ? null : (string)reader["UserID"];
                        tar.ObjectID = reader["ObjectID"] is DBNull ? null : (string)reader["ObjectID"];
                        tar.ObjectGUID = reader["ObjectGUID"] is DBNull ? null : (Guid?)reader["ObjectGUID"];
                        tar.Date = reader["Date"] is DBNull ? null : (DateTime?)reader["Date"];
                        tar.Count = (long)reader["Cnt"];
                        tar.Period = (int)reader["Period"];
                        tar.EventName = reader["EventName"] is DBNull ? null : (string)reader["EventName"];
                        tar.Image = reader["Image"] is DBNull ? null : (string)reader["Image"];
                        tar.Url = reader["URL"] is DBNull ? null : (string)reader["URL"];
                        tar.ObjectParentID = reader["ObjectParentID"] is DBNull ? null : (long?)reader["ObjectParentID"];
                        if (tar.ObjectParentID.HasValue)
                        {
                            try
                            {
                                tar.ObjectParentName = reader["ObjectParentName"] is DBNull ? null : (string)reader["ObjectParentName"];
                            }
                            catch (Exception)
                            {
                                tar.ObjectParentName = "#" + tar.ObjectParentID.ToString();
                            }
                        }
                        results.Add(tar);
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

            return results.ToArray();
        }

        /// <summary>
        /// Get visible events of the specific Manager
        /// </summary>
        /// <param name="managerName"></param>
        /// <returns></returns>
        public static List<String> GetVisibleEvents(string managerName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_GetVisibleEvents");
            command.AddInParameter("Manager", DbType.String, managerName);

            var visibleEvents = new List<String>();
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string eventName = DbUtility.GetValueFromDataReader(reader, "EventName", string.Empty);
                        visibleEvents.Add(eventName);
                    }
                }
                catch
                {
                    return new List<string>();
                }
                finally
                {
                    reader.Close();
                }
            }

            return visibleEvents;
        }

        #region Threading

        class ThreadingParams
        {
            public string ApplicationContext
            {
                get;
                set;
            }
            public long RequestID
            {
                get;
                set;
            }
        }

        /// <summary>
        /// The method being runned in a separate thread. 
        /// It calls Timeline_run procedure that collects events from the Checkbox database to the timeline results table.
        /// </summary>
        /// <param name="?"></param>
        private static void timelineWorkerRoutine(object param)
        {
            ThreadingParams ps = param as ThreadingParams;
            if (ps == null)
                return;

            Thread.SetData(
                Thread.GetNamedDataSlot(DataContextProvider.APPLICATION_CONTEXT_KEY),
                ps.ApplicationContext);

            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Run");
                command.AddInParameter("RequestID", DbType.Int64, ps.RequestID);

                db.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                try
                {
                    ExceptionPolicy.HandleException(ex, "BusinessInternal");
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Runs request in a separate thread
        /// </summary>
        /// <param name="RequestID"></param>
        private static void runRequest(long RequestID)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(timelineWorkerRoutine), new ThreadingParams() { RequestID = RequestID, ApplicationContext = ApplicationManager.ApplicationDataContext });
        }

        #endregion
        
        #region Utility Methods
        /// <summary>
        /// Create a date to filter events by period.
        /// </summary>
        /// <param name="period">
        /// Period: 
        ///     1 - immediate (latest single event)
        ///     2 - daily
        ///     3 - weekly
        ///     4 - monthly
        /// </param>
        /// <returns>Current date minus period</returns>
        public static DateTime? GetStartFilterDate(int period)
        {
            var dtStart = DateTime.Now;
            switch (period)
            {  
                case 0:
                    return null;
                case 2:
                    dtStart = dtStart.AddDays(-1);
                    break;
                case 3:
                    dtStart = dtStart.AddDays(-7);
                    break;
                case 4:
                    dtStart = dtStart.AddMonths(-1);
                    break;
            }
            return dtStart;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string ProtectFieldNameFromSQLInjections(string src)
        {
            if (!string.IsNullOrEmpty(src))
                return System.Text.RegularExpressions.Regex.Replace(src, "[^a-zA-Z]+", "", RegexOptions.Compiled);
            return src;
        }

        /// <summary>
        /// Clear timeline cache by user name
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static void ClearByACLEntry(string entryType, string entry)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_ClearByEntry");
            command.AddInParameter("EntryType", DbType.String, entryType);
            command.AddInParameter("EntryName", DbType.String, entry);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Clear timeline cache by policy that was granted or revoked to / from user
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static void ClearByPolicy(int policyID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_ClearByPolicy");
            command.AddInParameter("PolicyID", DbType.String, policyID);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Clear timeline for the given user
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void ClearByPrincipal(string uniqueIdentifier)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_ClearByPrincipal");
            command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);
            db.ExecuteNonQuery(command);
        }
        #endregion

        
        #region Timeline settings
        /// <summary>
        /// Get timeline settings for all events of the specific manager
        /// </summary>
        /// <param name="callingPrincipal"> </param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static List<TimelineSettings> GetTimelineSettings(CheckboxPrincipal callingPrincipal, string manager)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
            {
                return new List<TimelineSettings>();
            }

            var settingsList = new List<TimelineSettings>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Settings_Get");
            command.AddInParameter("Manager", DbType.String, manager);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        bool doesTheFirstTableExist = reader.GetName(0) == "EventName";
                        if (doesTheFirstTableExist)
                        {
                            
                            int eventOrder = DbUtility.GetValueFromDataReader(reader, "EventOrder", -1);
                            string eventName = DbUtility.GetValueFromDataReader(reader, "EventName", string.Empty);
                            string managerName = DbUtility.GetValueFromDataReader(reader, "Manager", string.Empty);
                            bool single = DbUtility.GetValueFromDataReader(reader, "Single", false);
                            bool daily = DbUtility.GetValueFromDataReader(reader, "Daily", false);
                            bool weekly = DbUtility.GetValueFromDataReader(reader, "Weekly", false);
                            bool monthly = DbUtility.GetValueFromDataReader(reader, "Monthly", false);
                            string fullEventName =
                                TextManager.GetText(
                                    String.Format("/timeline/eventsettings/caption/{0}",
                                                                         eventName.ToLower()));

                            settingsList.Add(new TimelineSettings()
                            {
                                Manager = managerName,
                                EventName = eventName,
                                Single = single,
                                Weekly = weekly,
                                Daily = daily,
                                Monthly = monthly,
                                EventOrder = eventOrder,
                                FullEventName = fullEventName
                            });
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //make shure in correct order of events
            var orderedSettingsList = settingsList.OrderBy(x => x.EventOrder).ToList();
            for (int i = 0; i < orderedSettingsList.Count; i++)
            {
                UpdateTimelineEventOrder(callingPrincipal, orderedSettingsList[i].Manager,
                                         orderedSettingsList[i].EventName, i + 1);
                orderedSettingsList[i].EventOrder = i + 1;
            }

            return orderedSettingsList;
        }

        /// <summary>
        /// Update period value for specified manager and event
        /// </summary>
        /// <param name="callingPrincipal"> </param>
        /// <param name="manager"></param>
        /// <param name="eventName"></param>
        /// <param name="periodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateTimelineEventPeriod(CheckboxPrincipal callingPrincipal, string manager, string eventName, string periodName, bool value)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
            {
                return false;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Settings_UpdateEventPeriod");
            command.AddInParameter("Manager", DbType.String, manager);
            command.AddInParameter("EventName", DbType.String, eventName);
            command.AddInParameter("PeriodName", DbType.String, periodName);
            command.AddInParameter("Value", DbType.Boolean, value);

            try
            {
                db.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Update value of event order of the specific event
        /// </summary>
        /// <param name="callingPrincipal"> </param>
        /// <param name="manager"></param>
        /// <param name="eventName"></param>
        /// <param name="eventOrder"></param>
        /// <returns></returns>
        public static bool UpdateTimelineEventOrder(CheckboxPrincipal callingPrincipal, string manager, string eventName, int eventOrder)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
            {
                return false;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_Timeline_Settings_UpdateEventOrder");
            command.AddInParameter("Manager", DbType.String, manager);
            command.AddInParameter("EventName", DbType.String, eventName);
            command.AddInParameter("Value", DbType.Int32, eventOrder);

            try
            {
                db.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}
