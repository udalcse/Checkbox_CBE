/**********************************************************************
 * LibraryTemplateManager.cs                                          *
 * Creation/deletion/retrieve libraries                               *
 **********************************************************************/
using System;
using System.Data;
using System.Security.Authentication;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.Data.Sprocs;
using System.Collections.Generic;
using Prezza.Framework.Caching;

namespace Checkbox.Forms
{
    /// <summary>
    /// Manager for library CRUD operations
    /// </summary>
    public class LibraryTemplateManager
    {
        private static readonly CacheManager _templateCache;
        private static object _lockObject = new object();

        static LibraryTemplateManager()
        {            
            lock (_lockObject)
            {
                _templateCache = CacheFactory.GetCacheManager("libraryTemplateCacheManager");
            }
        }

        /// <summary>
        /// Create a library template with the specified name and owner, who will be granted access to the library
        /// </summary>
        /// <param name="libraryName">Name of the library.</param>
        /// <param name="owner">Owner of the library.</param>
        /// <returns>Newly created library template.</returns>
        public static LibraryTemplate CreateLibraryTemplate(string libraryName, ExtendedPrincipal owner)
        {
            //Check authorization
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(owner, "Library.Create"))
            {
                throw new AuthorizationException();
            }

            var libraryTemplate = new LibraryTemplate();

            //Initialize Access
            //Create a default policy

            if (ApplicationManager.UseSimpleSecurity)
            {
                string[] defaultPolicyPermissions = { "Library.Create", "Library.Delete", "Library.Edit", "Library.View" };
                Policy defaultPolicy = libraryTemplate.CreatePolicy(defaultPolicyPermissions);

                var acl = new AccessControlList();
                acl.Save();

                libraryTemplate.InitializeAccess(defaultPolicy, acl);
            }
            else
            {
                //If no principal, make public, otherwise create an acl for the creator
                if (owner == null)
                {
                    string[] defaultPolicyPermissions = { "Library.View", "Library.Edit" };
                    Policy defaultPolicy = libraryTemplate.CreatePolicy(defaultPolicyPermissions);

                    var acl = new AccessControlList();
                    acl.Save();

                    libraryTemplate.InitializeAccess(defaultPolicy, acl);
                }
                else
                {
                    string[] defaultPolicyPermissions = { };
                    Policy defaultPolicy = libraryTemplate.CreatePolicy(defaultPolicyPermissions);

                    var acl = new AccessControlList();
                    var creatorPolicy = new Policy(libraryTemplate.SupportedPermissions);
                    acl.Add(owner, creatorPolicy);
                    acl.Save();

                    libraryTemplate.InitializeAccess(defaultPolicy, acl);
                }
            }

            //Save the template
            libraryTemplate.Save();

			libraryTemplate.Name = libraryName;

            //Load it so it's configuration data set is populated
            libraryTemplate.Load();

            return libraryTemplate;
        }

        /// <summary>
        /// Retrieve the library with the specified id
        /// </summary>
        /// <param name="libraryID"></param>
        /// <returns></returns>
        public static LibraryTemplate GetLibraryTemplate(int libraryID)
        {
            LibraryTemplate libraryTemplate = null;
            libraryTemplate = (LibraryTemplate)_templateCache[libraryID.ToString()];

            //Ensure template is "fresh".  If template has been updated since
            // the cached template's "Last Modified" date, ensure template
            // gets reloaded.
            if (libraryTemplate != null
                && ResponseTemplateManager.CheckTemplateUpdated(libraryID, libraryTemplate.LastModified))
            {
                libraryTemplate = null;
            }

            if (libraryTemplate != null)
                return libraryTemplate;
            
            try
            {
                libraryTemplate = new LibraryTemplate();
                libraryTemplate.Load(libraryID);

                _templateCache.Add(libraryID.ToString(), libraryTemplate);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return libraryTemplate;
        }

        /// <summary>
        /// Set options for library item
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="shouldShow">Shoud we show item in menu</param>
        /// <returns></returns>
        public static void UpdateItemLibraryOption(int itemID, bool shouldShow)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_UpdateItemSettings");
            command.AddInParameter("LibraryItemID", DbType.Int32, itemID);
            command.AddInParameter("ShowShow", DbType.Boolean, shouldShow);

            db.ExecuteNonQuery(command);
        }

        public static Dictionary<int, bool> GetItemLibraryOptions(int[] itemIDs)
        {
            var concatedIds = string.Join(",", itemIDs);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_GetItemsSettings");
            command.AddInParameter("LibraryItemIds", DbType.String, concatedIds);

            var result = new Dictionary<int, bool>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(DbUtility.GetValueFromDataReader(reader, "ItemId", -1), DbUtility.GetValueFromDataReader(reader, "ShowInMenu", false));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a <see cref="DataSet"/> of ResponseTemplates available to the logged-in user.
        /// </summary>
        /// <param name="currentPrincipal">Logged-in user</param>
        /// <param name="context"></param>
        /// <returns>A <see cref="DataSet"/> containing one table.  </returns>
        public static List<LightweightLibraryTemplate> GetAvailableLibraryTemplates(ExtendedPrincipal currentPrincipal, PaginationContext context)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command;
            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleItemLibrariesAdmin");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleItemLibraries",
                    currentPrincipal,
                    context);
            }

            var accessibleTemplates = new List<LightweightLibraryTemplate>();
            
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int templateId = DbUtility.GetValueFromDataReader(reader, "LibraryTemplateID", -1);

                        if (templateId > 0)
                        {
                            accessibleTemplates.Add(GetLightweightLibraryTemplate(templateId));
                        }
                    }

                    //Now attempt to get page count, which will be present in second result set
                    if (reader.NextResult() && reader.Read())
                    {
                        context.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return accessibleTemplates;
        }

        /// <summary>
        /// Get an <see cref="LightweightAccessControllable"/> object which has the same ACL and DefaultPolicy as a given template.
        /// </summary>
        /// <param name="templateID">ID of the library template to clone ACL and DefaultPolicy from.</param>
        /// <returns>A <see cref="LightweightAccessControllable"/> object initialized with a
        /// a given Template's ACL and DefaultPolicy.</returns>
        public static LightweightLibraryTemplate GetLightweightLibraryTemplate(int templateID)
        {
            var lightweightTemplate = new LightweightLibraryTemplate { ID = templateID };
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, lightweightTemplate);

            return lightweightTemplate;
        }

        /// <summary>
        /// Mark the specified library template as "deleted"
        /// </summary>
        /// <param name="libraryTemplateId">ID of the library template to delete</param>
        /// <param name="userPrincipal"></param>
        public static void DeleteLibraryTemplate(ExtendedPrincipal userPrincipal, int libraryTemplateId)
        {
            var lt = GetLightweightLibraryTemplate(libraryTemplateId);

            if (lt == null)
            {
                return;
            }

            if(!AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, lt, "Library.Edit"))
            {
                throw new AuthenticationException();
            }
            
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_Delete");
            command.AddInParameter("LibraryTemplateID", DbType.Int32, libraryTemplateId);
            command.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Return a boolean indicating whether a library template with the specified name exists. Don't count
        /// template name as a duplicate if the id of the existing template matches the passed-in ID.
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="idToIgnore"></param>
        /// <returns></returns>
        public static bool LibraryTemplateExists(string libraryName, int? idToIgnore)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_GetLibraryTemplateIDFromName");
            command.AddInParameter("LibraryName", DbType.String, libraryName);
            command.AddOutParameter("LibraryID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object libraryTemplateID = command.GetParameterValue("LibraryID");

            if (libraryTemplateID == DBNull.Value)
            {
                return false;
            }

            if (idToIgnore == null)
            {
                return true;
            }

            return idToIgnore.Value != (int)libraryTemplateID;
        }

        /// <summary>
        /// Return the number of items in the library
        /// </summary>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        public static int GetTemplateItemsCount(int libraryId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Library_GetItemsCount");
            command.AddInParameter("LibraryID", DbType.Int32, libraryId);

            return (int)db.ExecuteScalar(command);
        }

        /// <summary>
        /// Copy the specified library template
        /// </summary>
        /// <param name="libraryID">ID of the library template to copy.</param>
        /// <param name="owner">Owner of the new template.</param>
        /// <param name="languageCode">Language code for template name.</param>
        /// <returns></returns>
        public static LibraryTemplate CopyLibraryTemplate(Int32 libraryID, ExtendedPrincipal owner, string languageCode)
        {
            //TODO: Copy library template
            return null;

        //    DataSet data = new DataSet();

        //    //Use a non-cached copy
        //    LibraryTemplate library = GetLibraryTemplate(libraryID);

        //    //Copy the template using the same serialization / deserialization as the export
        //    MemoryStream stream = new MemoryStream();

        //    XmlSerializer serializer = new XmlSerializer(typeof(LibraryTemplate));
        //    serializer.Serialize(stream, library);

        //    stream.Seek(0, SeekOrigin.Begin);

        //    //Load the raw data into the empty dataset
        //    data.ReadXml(stream, XmlReadMode.ReadSchema);

        //    //Create a new survey
        //    LibraryTemplate newLibrary = CreateLibraryTemplate("Library Copy", owner);
        //    newLibrary.Import(data, null, null, null, true);
        //    int copyCount = 1;

        //    //Make sure the survey name is not duplicated
        //    string newLibraryName = string.Format(
        //        "{0} {1} {2} {3}",
        //        TextManager.GetText("/pageText/manageSurveys.aspx/copy", languageCode),
        //        copyCount,
        //        TextManager.GetText("/pageText/manageSurveys.aspx/of", languageCode),
        //        TextManager.GetText(library.NameTextID, languageCode));

        //    while (LibraryTemplateExists(newLibraryName, null))
        //    {
        //        copyCount++;
        //        newLibraryName = string.Format(
        //            "{0} {1} {2} {3}",
        //            TextManager.GetText("/pageText/manageSurveys.aspx/copy", languageCode),
        //            copyCount,
        //            TextManager.GetText("/pageText/manageSurveys.aspx/of", languageCode),
        //            TextManager.GetText(library.NameTextID, languageCode));
        //    }

        //    TextManager.SetText(newLibrary.NameTextID, languageCode, newLibraryName);
        //    newLibrary.Save();

        //    return newLibrary;
        }
    }
}
