using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Checkbox.Analytics.Configuration;
using Checkbox.Analytics.Export;
using Checkbox.Analytics.Items;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Users;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Provides create, load, delete and copy management tasks for <see cref="AnalysisTemplate"/> objects.
    /// The AnalysisTemplateManager can also create a temporary <see cref="AnalysisTemplate"/> with the 
    /// sole purpose of exporting form responses in CSV or SPSS-Compatible CSV format.
    /// </summary>
    public static class AnalysisTemplateManager
    {
        private static CacheManager _templateCache;

        /// <summary>
        /// Template cache manager
        /// </summary>
        private static CacheManager TemplateCache
        {
            get { return _templateCache ?? (_templateCache = CacheFactory.GetCacheManager("analysisTemplateCacheManager")); }
        }

        /// <summary>
        /// Add a source response template item to the source items list of an Export item for CSV
        /// or SPSS-compatiable CSV export.
        /// </summary>
        /// <param name="sourceItemData">Response template item to add to CSV export.</param>
        /// <param name="exportItemData">Export item to add source item to.</param>
        /// <remarks>CSV exports are actually reports that contain a single <see cref="ExportItemData"/> 
        /// that handles generating the list of export columns and getting the data for rows of the 
        /// export.  Loading the response data from the database is handled by the <see cref="Analysis"/>
        /// that contains the <see cref="ExportItem"/>.</remarks>
        private static void AddSourceItemToExportItem(ItemData sourceItemData, ExportItemData exportItemData)
        {
            if (sourceItemData.ItemIsIAnswerable)
            {
                exportItemData.AddSourceItem(sourceItemData.ID.Value);
            }

            if (sourceItemData is ICompositeItemData)
            {
                foreach (var childItemId in ((ICompositeItemData)sourceItemData).GetChildItemDataIDs())
                {
                    var childItemData = ItemConfigurationManager.GetConfigurationData(childItemId, true);
                    AddSourceItemToExportItem(childItemData, exportItemData);
                }
            }
        }

        ///<summary>
        /// Show user input items for report
        ///</summary>
        ///<param name="responseTemplateId"></param>
        ///<returns>true when response value less that max in config</returns>
        public static bool ShowUserInputItemsForReport(int responseTemplateId)
        {
            int maxCount =
                ((ReportPerformanceConfiguration)
                    Prezza.Framework.Configuration.ConfigurationManager.
                        GetConfiguration("checkboxReportPerformanceConfiguration")).
                            MaxResponseCountForUserInputItemsVisibility;
            
            int respondedCount = ResponseManager.GetResponseCount(
                responseTemplateId, ApplicationManager.AppSettings.ReportIncompleteResponses, true);
            
            return respondedCount <= maxCount;
        }

        /// <summary>
        /// Generate a blank analysis template which is used to load response data for xml data export.
        /// </summary>
        /// <param name="responseTemplate"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AnalysisTemplate GenerateXMLExportTemplate(ResponseTemplate responseTemplate, ExportOptions options)
        {
            if (responseTemplate == null)
            {
                return null;
            }

            return new AnalysisTemplate
            {
                FilterStartDate = options.StartDate,
                FilterEndDate = options.EndDate
            };
        }

        /// <summary>
        /// Generate a template for exporting response data in CSV format.
        /// </summary>
        /// <param name="responseTemplate"><see cref="ResponseTemplate"/> to export response information for.</param>
        /// <param name="options">Additional export options</param>
        /// <returns>An <see cref="AnalysisTemplate"/> containing a CSV export item configured to export the results of
        /// all answerable items in the specified <see cref="ResponseTemplate"/>.</returns>
        /// <remarks>An analysis template containing a single <see cref="CSVExportItemData"/> object is created
        /// and configured to export the results of each item in the <see cref="ResponseTemplate"/> argument.  If a NULL 
        /// <see cref="ResponseTemplate"/> is passed as an argument, a NULL value is returned.</remarks>
        public static AnalysisTemplate GenerateCSVExportTemplate(ResponseTemplate responseTemplate, ExportOptions options)
        {
            if (responseTemplate == null)
            {
                return null;
            }

            var newTemplate = new AnalysisTemplate
            {
                FilterStartDate = options.StartDate,
                FilterEndDate = options.EndDate
            };

            var exportItemData = (CSVExportItemData)ItemConfigurationManager.CreateConfigurationData("CSVExport");
            exportItemData.ID = -1;
            newTemplate.SetItem(exportItemData);

            exportItemData.UseAliases = options.UseAliases;
            exportItemData.IncludeHidden = options.IncludeHidden;
            exportItemData.IncludeOpenEnded = options.IncludeOpenEnded;
            exportItemData.MergeSelectMany = options.MergeSelectMany;
            exportItemData.ExportRankOrderPoints = options.ExportRankOrderPoints;

            newTemplate.ResponseTemplateID = responseTemplate.ID.Value;
            exportItemData.AddResponseTemplate(responseTemplate.ID.Value);


            //Add all items in the response template
            foreach (var templatePageId in responseTemplate.ListTemplatePageIds())
            {
                var templatePage = responseTemplate.GetPage(templatePageId);

                foreach (var itemId in templatePage.ListItemIds())
                {
                    var itemData = ItemConfigurationManager.GetConfigurationData(itemId, true);

                    AddSourceItemToExportItem(itemData, exportItemData);
                }
            }

            return newTemplate;
        }

        /// <summary>
        /// Generate an <see cref="AnalysisTemplate"/> configured to export response data in an SPSS compatible CSV format.
        /// </summary>
        /// <param name="responseTemplate"><see cref="ResponseTemplate"/> to export response information for.</param>
        /// <param name="options">Specific export options.</param>
        /// <returns>An <see cref="AnalysisTemplate"/> containing a SPSS compatible CSV export item configured to export the results of
        /// all answerable items in the specified <see cref="ResponseTemplate"/>.</returns>
        /// <remarks>An analysis template containing a single <see cref="SPSSExportItemData"/> object is created
        /// and configured to export the results of each item in the <see cref="ResponseTemplate"/> argument.  If a NULL 
        /// <see cref="ResponseTemplate"/> is passed as an argument, a NULL value is returned.</remarks>
        public static AnalysisTemplate GenerateSPSSExportTemplate(ResponseTemplate responseTemplate, ExportOptions options)
        {
            if (responseTemplate == null)
            {
                return null;
            }

            var newTemplate = new AnalysisTemplate
                                  {
                                      FilterStartDate = options.StartDate,
                                      FilterEndDate = options.EndDate
                                  };

            var exportItemData = (SPSSExportItemData)ItemConfigurationManager.CreateConfigurationData("SPSSExport");
            exportItemData.ID = -1;
            newTemplate.SetItem(exportItemData);

            exportItemData.UseAliases = options.UseAliases;
            exportItemData.IncludeHidden = options.IncludeHidden;
            exportItemData.IncludeOpenEnded = options.IncludeOpenEnded;
            exportItemData.MergeSelectMany = options.MergeSelectMany;

            newTemplate.ResponseTemplateID = responseTemplate.ID.Value;
            exportItemData.AddResponseTemplate(responseTemplate.ID.Value);


            //Add all items in the response template
            foreach (var templatePageId in responseTemplate.ListTemplatePageIds())
            {
                var templatePage = responseTemplate.GetPage(templatePageId);

                foreach (var itemId in templatePage.ListItemIds())
                {
                    var itemData = ItemConfigurationManager.GetConfigurationData(itemId);
                    AddSourceItemToExportItem(itemData, exportItemData);
                }
            }

            return newTemplate;
        }

        /// <summary>
        /// Get id of analysis template based on its GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int GetAnalysisTemplateId(Guid guid)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetID");
                command.AddInParameter("GUID", DbType.Guid, guid);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["AnalysisTemplateID"]);
                        }
                    }
                    catch { }

                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return 0;
        }


        /// <summary>
        /// Get the <see cref="AnalysisTemplate"/> with the specified Guid.
        /// </summary>
        /// <param name="guid">GUID associated with the <see cref="AnalysisTemplate"/>.</param>
        /// <returns><see cref="AnalysisTemplate"/> associated with the provided guid.</returns>
        /// <remarks>Returns NULL if an <see cref="AnalysisTemplate"/> with the specified GUID
        /// can't be located.</remarks>
        public static AnalysisTemplate GetAnalysisTemplate(Guid guid)
        {
            int analysisTemplateId = GetAnalysisTemplateId(guid);

            return analysisTemplateId > 0
                       ? GetAnalysisTemplate(analysisTemplateId)
                       : null;
        }

        private static string GetAnalysisTemplateIdHash(int analysisTemplateID)
        {
            return analysisTemplateID.ToString();
        }

        /// <summary>
        /// Get the <see cref="AnalysisTemplate"/> with the specified database id.
        /// </summary>
        /// <param name="analysisTemplateID">Database ID associated with the <see cref="AnalysisTemplate"/>.</param>
        /// <param name="useCache"></param>
        /// <returns><see cref="AnalysisTemplate"/> associated with the provided database id.</returns>
        /// <remarks>Returns NULL if an <see cref="AnalysisTemplate"/> with the specified ID
        /// can't be located.</remarks>
        public static AnalysisTemplate GetAnalysisTemplate(int analysisTemplateID, bool useCache = false)
        {
            AnalysisTemplate analysisTemplate = useCache ?
                TemplateCache.GetData(GetAnalysisTemplateIdHash(analysisTemplateID)) as AnalysisTemplate : null;
            
            if (analysisTemplate != null)
                return analysisTemplate;

            try
            {
                analysisTemplate = new AnalysisTemplate();
                analysisTemplate.Load(analysisTemplateID);
                TemplateCache.Add(GetAnalysisTemplateIdHash(analysisTemplateID), analysisTemplate);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return analysisTemplate;
        }

        /// <summary>
        /// Get the ID of a response template given it's name.
        /// </summary>
        /// <param name="responseTemplateId">ID of response template analysis template is associated with.</param>
        /// <param name="analysisTemplateName">Name of analysis template to get id for.</param>
        /// <returns>ID of response template.</returns>
        /// <remarks>Returns null if no analysis template with name found.  If multiple templates with name exists, returns 
        /// first matching name.</remarks>
        public static int? GetAnalysisTemplateId(int responseTemplateId, string analysisTemplateName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetIdFromName");

            command.AddInParameter("TemplateName", DbType.String, analysisTemplateName);
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddOutParameter("TemplateId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("TemplateId");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (int)outVal;
            }

            return null;
        }

        /// <summary>
        /// Get a lightweight <see cref="IAccessControllable"/> object that can be used as a proxy
        /// for performing security checks against the <see cref="AnalysisTemplate"/> with the specified ID.
        /// </summary>
        /// <param name="analysisTemplateID">ID of the analysis template to get a  <see cref="IAccessControllable"/> proxy
        /// object for.</param>
        /// <returns><see cref="IAccessControllable"/> proxy for the <see cref="AnalysisTemplate"/> with the 
        /// specified ID.</returns>
        /// <remarks>
        /// Loading an <see cref="AnalysisTemplate"/> can be a computationally expensive process. This method provides a way
        /// to access Access Control List and Default Policy information for an <see cref="AnalysisTemplate"/> object without
        /// the expense of loading the full object, which is useful when authorization checks need to be performed iteratively
        /// on a number of <see cref="AnalysisTemplate"/> objects.
        /// </remarks>
        public static LightweightAnalysisTemplate GetLightweightAnalysisTemplate(int analysisTemplateID)
        {
            var lightweightTemplate = new LightweightAnalysisTemplate(analysisTemplateID);
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, lightweightTemplate);

            return lightweightTemplate;
        }


        /// <summary>
        /// Creates a new, empty analysis and associates it with a form
        /// </summary>
        /// <param name="analysisName">The desired name for the new analysis</param>
        /// <param name="responseTemplateID">The ID of the form to associate this analysis with</param>
        /// <param name="creator">User prinicipal tha</param>
        /// <returns>An empty analysis template.</returns>
        public static AnalysisTemplate CreateAnalysisTemplate(string analysisName, int responseTemplateID, CheckboxPrincipal creator)
        {
            //Check authorization
            LightweightResponseTemplate form = ResponseTemplateManager.GetLightweightResponseTemplate(responseTemplateID);

            if (form == null)
            {
                return null;
            }

            // authorize the creation
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(creator, form, "Analysis.Create"))
            {
                throw new AuthorizationException();
            }

            var analysisTemplate = new AnalysisTemplate { Name = analysisName };

            //Initialize Access
            //Create a default policy

            if (ApplicationManager.UseSimpleSecurity)
            {

                string[] defaultPolicyPermissions = { "Analysis.Administer", "Analysis.Edit", "Analysis.Delete", "Analysis.Run" };
                Policy defaultPolicy = analysisTemplate.CreatePolicy(defaultPolicyPermissions);

                var acl = new AccessControlList();
                acl.Save();

                if (creator != null)
                {
                    var acl2 = new AccessControlList();
                    var creatorPolicy = new Policy(analysisTemplate.SupportedPermissions);
                    acl2.Add(creator, creatorPolicy);
                    acl2.Save();

                    analysisTemplate.CreatedBy = creator.Identity.Name;
                    analysisTemplate.InitializeAccess(defaultPolicy, acl2);
                }
                else
                {
                    analysisTemplate.InitializeAccess(defaultPolicy, acl);
                }

            }
            else
            {

                //If no principal, make public, otherwise create an acl for the creator
                if (creator == null)
                {
                    string[] defaultPolicyPermissions = { "Analysis.Run", "Analysis.Edit" };
                    Policy defaultPolicy = analysisTemplate.CreatePolicy(defaultPolicyPermissions);

                    var acl = new AccessControlList();
                    acl.Save();

                    analysisTemplate.InitializeAccess(defaultPolicy, acl);
                }
                else
                {
                    string[] defaultPolicyPermissions = { };
                    Policy defaultPolicy = analysisTemplate.CreatePolicy(defaultPolicyPermissions);

                    var acl = new AccessControlList();
                    var creatorPolicy = new Policy(analysisTemplate.SupportedPermissions);
                    acl.Add(creator, creatorPolicy);
                    acl.Save();

                    analysisTemplate.CreatedBy = creator.Identity.Name;
                    analysisTemplate.InitializeAccess(defaultPolicy, acl);
                }
            }

            analysisTemplate.ResponseTemplateID = responseTemplateID;

            //Save the template
            analysisTemplate.Save();

            return analysisTemplate;
        }

        /// <summary>
        /// Generate a report according the values defined in the AnalysisWizardOptions
        /// </summary>
        /// <param name="analysisName"></param>
        /// <param name="responseTemplateID"></param>
        /// <param name="options"></param>
        /// <param name="styleTemplateId"></param>
        /// <param name="chartStyleId"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static AnalysisTemplate AutoGenerateAnalysisTemplate(string analysisName,
                                                                     int responseTemplateID,
                                                                     AnalysisWizardOptions options,
                                                                     int? styleTemplateId,
                                                                     int chartStyleId,
                                                                     CheckboxPrincipal creator)
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(responseTemplateID);
            AnalysisTemplate analysisTemplate = CreateAnalysisTemplate(analysisName, responseTemplate.ID.Value, creator);
            AppearanceData chartStyleAppearance = ChartStyleManager.GetChartStyleAppearance(chartStyleId);

            analysisTemplate.StyleTemplateID = styleTemplateId;
            analysisTemplate.ChartStyleID = chartStyleId;

            analysisTemplate.IncludeIncompleteResponses = options.IncludeIncompleteResponses;
            analysisTemplate.IncludeTestResponses = options.IncludeTestResponses;

            int? newPageId = null;
            int reportPagePosition = 1;
            int[] pageIds = responseTemplate.ListTemplatePageIds();

            //due to the way items are added to the report, if they are going
            //to be on one page then we must add the pages in the reverse order
            //otherwise, the last survey page items will be at the first positions
            //in the report
            if (options.IsSinglePageReport)
                pageIds = pageIds.Reverse().ToArray();

            foreach (int pageId in pageIds)
            {
                TemplatePage page = responseTemplate.GetPage(pageId);

                int itemPosition = 1;
                foreach (int itemId in page.ListItemIds())
                {
                    ItemData item = responseTemplate.GetItem(itemId);

                    if (item is MatrixItemData)
                    {
                        if (options.MatrixGraphType.Equals("MatrixSummary", StringComparison.InvariantCultureIgnoreCase) ||
                            options.MatrixGraphType.Equals("MatrixSummaryAndNested", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var oldMGT = options.MatrixGraphType;
                            options.MatrixGraphType = "MatrixSummary";
                            GenerateAnalysisItemData(options, responseTemplate, analysisTemplate, chartStyleAppearance, ref newPageId, reportPagePosition, ref itemPosition, item, creator);
                            options.MatrixGraphType = oldMGT;
                        }

                        if (options.MatrixGraphType.Equals("MatrixSummaryAndNested", StringComparison.InvariantCultureIgnoreCase) ||
                            options.MatrixGraphType.Equals("NestedItemsOnly", StringComparison.InvariantCultureIgnoreCase))
                        {
                            MatrixItemData matrix = item as MatrixItemData;
                            for (int row = 0; row < matrix.RowCount; row++)
                            {
                                for (int col = 0; col < matrix.ColumnCount; col++)
                                {
                                    item = matrix.GetItemAt(row + 1, col + 1);
                                    GenerateAnalysisItemData(options, responseTemplate, analysisTemplate, chartStyleAppearance, ref newPageId, reportPagePosition, ref itemPosition, item, creator);
                                }
                            }
                        }
                    }
                    else
                        GenerateAnalysisItemData(options, responseTemplate, analysisTemplate, chartStyleAppearance, ref newPageId, reportPagePosition, ref itemPosition, item, creator);
                }

                //Increment page position and cause new page to be created
                if (!options.IsSinglePageReport)
                {
                    reportPagePosition++;
                    newPageId = null;
                }
            }
            return analysisTemplate;
        }

        /// <summary>
        /// Generate an analysis item data by response item data
        /// </summary>
        /// <param name="options"></param>
        /// <param name="responseTemplate"></param>
        /// <param name="analysisTemplate"></param>
        /// <param name="chartStyleAppearance"></param>
        /// <param name="newPageId"></param>
        /// <param name="reportPagePosition"></param>
        /// <param name="itemPosition"></param>
        /// <param name="item"></param>
        private static void GenerateAnalysisItemData(AnalysisWizardOptions options, ResponseTemplate responseTemplate, AnalysisTemplate analysisTemplate, AppearanceData chartStyleAppearance, ref int? newPageId, int reportPagePosition, ref int itemPosition, ItemData item, CheckboxPrincipal principal)
        {

            AnalysisItemData newItemData = AnalysisItemFactory.GetItem(item, responseTemplate.ID.Value, options, chartStyleAppearance, principal);

            if (newItemData != null)
            {
                newItemData.CreatedBy = principal.Identity.Name;
                newItemData.Save();

                if (!newPageId.HasValue)
                {
                    newPageId = analysisTemplate.AddPageToTemplate(reportPagePosition, true);
                }

                analysisTemplate.AddItemToPage(newPageId.Value, newItemData.ID.Value, itemPosition);
                itemPosition++;
            }
        }

        /// <summary>
        /// Returns a count of analysis templates the specified principal can view/edit for the specified response template.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="permissions"></param>
        /// <param name="permissionJoin"></param>
        /// <returns></returns>
        public static int GetAnalysisCountForSurvey(CheckboxPrincipal currentPrincipal, int responseTemplateId, List<String> permissions, PermissionJoin permissionJoin)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_CountForRT");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_CountAccessibleAnalysesForSurvey");
                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
                command.AddInParameter("FirstPermissionName", DbType.String, permissions.Count > 0 ? permissions[0] : string.Empty);
                command.AddInParameter("SecondPermissionName", DbType.String, permissions.Count > 1 ? permissions[1] : string.Empty);
                command.AddInParameter("RequireBothPermissions", DbType.Boolean, permissionJoin == PermissionJoin.All);
                command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);
            }


            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddOutParameter("TotalCount", DbType.Int32, 4);

            try
            {
                db.ExecuteNonQuery(command);
                return (int)command.GetParameterValue("TotalCount");
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return 0;
        }

        /// <summary>
        /// Return a list of analysis templates the specified principal can view/edit for the specified response template.
        /// </summary>
        /// <param name="principal">User principal to get the list of analyses for.</param>
        /// <param name="responseTemplateID">The ID of the form to list analyses for.</param>
        /// <returns>A DataTable containing a list of analyses</returns>
        public static List<LightweightAnalysisTemplate> ListAnalysisTemplatesForSurvey(CheckboxPrincipal principal, int responseTemplateID)
        {
            return ListAnalysisTemplatesForSurvey(principal, responseTemplateID, new PaginationContext());
        }

        /// <summary>
        /// Deletes all analysis templates from the cache in the AnalysisTemplateManager
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateID"></param>
        public static void CleanupAnalysisTemplatesCacheForSurvey(CheckboxPrincipal principal, int responseTemplateID)
        {
            var reports = ListAnalysisTemplatesForSurvey(principal, responseTemplateID);

            reports.ForEach(r => CleanupAnalysisTemplatesCache(r.ID));
        }

        /// <summary>
        /// Deletes all analysis templates from the cache in the AnalysisTemplateManager
        /// </summary>
        /// <param name="analysisTemplateID"></param>
        public static void CleanupAnalysisTemplatesCache(int analysisTemplateID)
        {
            string key = GetAnalysisTemplateIdHash(analysisTemplateID);
            if (_templateCache.Contains(key))
                _templateCache.Remove(key);
        }

        /// <summary>
        /// Returns a list of analyses associated with the given form
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="responseTemplateId">The ID of the form</param>
        /// <param name="paginationContext"></param>
        /// <returns>A DataTable containing a list of analyses</returns>
        public static List<LightweightAnalysisTemplate> ListAnalysisTemplatesForSurvey(CheckboxPrincipal currentPrincipal, int responseTemplateId, PaginationContext paginationContext)
        {
            //Switch for system admininistrator
            DBCommandWrapper command;

            //Prepare command
            Database db = DatabaseFactory.CreateDatabase();

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_List");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);
            }
            else
            {
                //Ensure permissions are set
                if (paginationContext.Permissions.Count == 0)
                {
                    paginationContext.Permissions.Add("Analysis.Run");
                    paginationContext.Permissions.Add("Analysis.Edit");
                    paginationContext.PermissionJoin = PermissionJoin.Any;
                }

                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleAnalysesForSurvey",
                    currentPrincipal,
                    paginationContext);
            }

            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            var templateIdList = new List<int>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Get template data
                    while (reader.Read())
                    {
                        int analysisTemplateId = DbUtility.GetValueFromDataReader(reader, "AnalysisTemplateId", -1);

                        if (analysisTemplateId > 0
                            && !templateIdList.Contains(analysisTemplateId))
                        {
                            templateIdList.Add(analysisTemplateId);
                        }
                    }

                    //Get item count
                    if (reader.NextResult() && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Now build result list
            var resultList = templateIdList.Select(GetLightweightAnalysisTemplate).ToList();
            if (paginationContext.ItemCount == 0)
                paginationContext.ItemCount = resultList.Count;
            return resultList;
        }

        /// <summary>
        /// Get a list of reports that can be viewed by a user
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<LightweightAnalysisTemplate> ListViewableAnalysisTemplates(CheckboxPrincipal currentPrincipal, PaginationContext context)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = null;

            if (currentPrincipal != null)
            {
                if (currentPrincipal.IsInRole("System Administrator"))
                {
                    command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_List");
                    command.AddInParameter("ResponseTemplateId", DbType.Int32, DBNull.Value);
                    QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);
                }
                else
                {
                    command = QueryHelper.CreateListAccessibleCommand("ckbx_sp_Security_ListAccessibleAnalyses",
                                                                      currentPrincipal, context);
                }
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleAnalysesAnonymous");
                command.AddInParameter("PermissionName", DbType.String, context.Permissions[0]);

                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);
            }

            var analysisList = new List<LightweightAnalysisTemplate>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int analysisTemplateId = DbUtility.GetValueFromDataReader(reader, "AnalysisTemplateId", -1);

                        if (analysisTemplateId > 0)
                        {
                            LightweightAnalysisTemplate template = GetLightweightAnalysisTemplate(analysisTemplateId);

                            if (!analysisList.Contains(template))
                                analysisList.Add(template);
                        }
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            int totalItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", 0);
                            context.ItemCount = totalItemCount;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return analysisList;
        }

        /// <summary>
        /// Delete the specified analysis template
        /// </summary>
        /// <param name="analysisTemplateID">ID of analysis template to delete.</param>
        public static void DeleteAnalysisTemplate(CheckboxPrincipal currentPrincipal, Int32 analysisTemplateID)
        {
            // authorize the removing
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(currentPrincipal, "Analysis.Delete"))
            {
                throw new AuthorizationException();
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_Delete");
            command.AddInParameter("AnalysisTemplateID", DbType.Int32, analysisTemplateID);
            command.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Get ResponseTemplates and folders available to the logged-in user.
        /// </summary>
        /// <param name="currentPrincipal">Logged-in user.</param>
        /// <param name="parentFolderId"></param>
        /// <param name="context"></param>
        /// <param name="totalItemCount"></param>
        /// <returns><see cref="DataSet"/> containing result.</returns>
        public static DataSet GetAvailableTemplatesAndFolders(CheckboxPrincipal currentPrincipal, int? parentFolderId, PaginationContext context, out int totalItemCount)
        {
            //TODO: Get templates and folders for report screen.

            //SelectQuery query = Forms.Data.QueryFactory.GetAllTemplatesAndFoldersQuery(context, parentFolderId);
            //Database db = DatabaseFactory.CreateDatabase();
            //DBCommandWrapper command = db.GetSqlStringCommandWrapper(query.ToString());

            //return FilterTemplatesAndFolders(
            //    db.ExecuteDataSet(command),
            //    currentPrincipal,
            //    context,
            //    out totalItemCount);

            totalItemCount = 0;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateDataSet"></param>
        /// <param name="currentPrincipal"></param>
        /// <param name="context"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <remarks>Assumption is made that incoming data is already sorted.</remarks>
        private static DataSet FilterTemplatesAndFolders(DataSet templateDataSet,
                                                         CheckboxPrincipal currentPrincipal,
                                                         PaginationContext context,
                                                         out int count)
        {
            //count = 0;

            ////Add columns
            //if (templateDataSet != null && templateDataSet.Tables.Count > 0)
            //{
            //    //Create an output table
            //    DataSet outputDs = new DataSet();
            //    DataTable resultsTable = templateDataSet.Tables[0].Clone();
            //    outputDs.Tables.Add(resultsTable);

            //    //Add columns for auth check values
            //    resultsTable.Columns.Add("ViewEditReports", typeof(bool));
            //    resultsTable.Columns.Add("ViewResponses", typeof(bool));
            //    resultsTable.Columns.Add("ExportResponses", typeof(bool));

            //    //Get an authorization provider
            //    IAuthorizationProvider authProvider = AuthorizationFactory.GetAuthorizationProvider();

            //    int pageStart = context.GetStartIndex();
            //    int pageEnd = context.GetEndIndex(templateDataSet.Tables[0].Rows.Count);
            //    int totalIncluded = 0;

            //    DataRow[] rtRows = templateDataSet.Tables[0].Select(null, null, DataViewRowState.CurrentRows);

            //    foreach (DataRow rtRow in rtRows)
            //    {
            //        if (rtRow["ItemID"] != DBNull.Value && rtRow["ItemType"] != DBNull.Value)
            //        {
            //            string itemType = DbUtility.GetValueFromDataRow<string>(rtRow, "ItemType", null);
            //            int itemId = DbUtility.GetValueFromDataRow(rtRow, "ItemID", -1);

            //            bool createReports = false;
            //            bool viewResponses = false;
            //            bool exportResponses = false;
            //            bool viewRunReports = false;
            //            bool viewFolder = false;

            //            if (itemType.ToLower() == "folder")
            //            {
            //                FormFolder f = new FormFolder();
            //                f.Load(itemId);
            //                viewFolder = authProvider.Authorize(currentPrincipal, f, "FormFolder.Read");
            //            }
            //            else
            //            {
            //                LightweightAccessControllable lightWeightRT = ResponseTemplateManager.GetLightweightResponseTemplate(itemId);
            //                createReports = authProvider.Authorize(currentPrincipal, lightWeightRT, "Analysis.Create");
            //                viewResponses = authProvider.Authorize(currentPrincipal, lightWeightRT, "Analysis.Responses.View")
            //                                || authProvider.Authorize(currentPrincipal, lightWeightRT, "Analysis.Responses.Edit");
            //                exportResponses = authProvider.Authorize(currentPrincipal, lightWeightRT, "Analysis.Responses.Export");

            //                //User has access if a report can be created, responses can be viewed/exported.
            //                if (!createReports && !viewResponses && !exportResponses)
            //                {
            //                    //If there is no access, check for any reports that can be viewed or edited
            //                    int analysisCount;
            //                    ListAnalysisTemplatesForSurvey(currentPrincipal, itemId, out analysisCount);

            //                    viewRunReports = (analysisCount > 0);
            //                }
            //            }
            //            if (createReports || viewRunReports || viewResponses || exportResponses || viewFolder)
            //            {
            //                //If the user has access to this report, add it to the result set if it is on the current "page" of results.
            //                if (context.CurrentPage <= 0 || context.PageSize <= 0 || (totalIncluded >= pageStart && totalIncluded <= pageEnd))
            //                {
            //                    //Create an object array to use
            //                    List<object> valueList = new List<object>();
            //                    valueList.AddRange(rtRow.ItemArray);

            //                    valueList.Add(viewRunReports || createReports);
            //                    valueList.Add(viewResponses);
            //                    valueList.Add(exportResponses);

            //                    //Add the row
            //                    resultsTable.Rows.Add(valueList.ToArray());
            //                }

            //                totalIncluded++;
            //            }
            //        }
            //    }

            //    count = totalIncluded;
            //    return outputDs;
            //}

            count = 0;
            return null;
        }

        /// <summary>
        /// Return a boolean indicating if an analysis template with the specified name exists
        /// for the specified response template
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <param name="responseTemplateID">Survey associated with analysis template.</param>
        /// <returns></returns>
        public static bool AnalysisTemplateExists(string name, Int32 responseTemplateID)
        {
            return AnalysisTemplateExists(name, responseTemplateID, null);
        }

        /// <summary>
        /// Return a boolean indicating if an analysis template with the specified name exists
        /// for the specified response template, other than the known analysis template
        /// </summary>
        /// <param name="name">Name to check.</param>
        /// <param name="responseTemplateID">Survey associated with analysis template.</param>
        /// <param name="analysisTemplateID">ID of analysis template to ignore when searching for name matches.</param>
        /// <returns></returns>
        public static bool AnalysisTemplateExists(string name, Int32 responseTemplateID, int? analysisTemplateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_GetIDFromName");
            command.AddInParameter("AnalysisName", DbType.String, name);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);
            command.AddOutParameter("AnalysisID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object id = command.GetParameterValue("AnalysisID");

            if (id != null && id != DBNull.Value && (int)id > 0)
            {
                if (analysisTemplateID.HasValue)
                {
                    if (Convert.ToInt32(analysisTemplateID) != Convert.ToInt32(id))
                        return true;

                    return false;
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Determines if a survey name is already in use and returns the next available unique name.
        /// The format of the unique name is Survey_Name Copy #.
        /// Returns the provided survey name if it is unique.
        /// </summary>
        /// <param name="ResponseTemplateID"></param>
        /// <param name="reportName"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string GetUniqueName(int ResponseTemplateID, string reportName, string languageCode)
        {
            int copyCount = 0;
            string newReportName = reportName;

            if (string.IsNullOrEmpty(languageCode))
                languageCode = TextManager.DefaultLanguage;

            while (AnalysisTemplateExists(newReportName, ResponseTemplateID))
            {
                copyCount++;
                newReportName = string.Format("{0} {1} {2}", reportName, TextManager.GetText("/common/duplicate", languageCode), copyCount);
            }

            return newReportName;
        }


        /// <summary>
        /// Copy the specified analysis template
        /// </summary>
        /// <param name="templateID">ID of analysis template to copy.</param>
        /// <param name="creator">User principal copying the template.</param>
        /// <param name="languageCode">Language code to use for setting new template title.</param>
        /// <returns>New copy of the analyis template.</returns>
        public static AnalysisTemplate CopyTemplate(Int32 templateID, string newName, CheckboxPrincipal creator, string languageCode)
        {
            ////Use a non-cached copy
            AnalysisTemplate analysisTemplate = GetAnalysisTemplate(templateID);

            if (string.IsNullOrEmpty(newName))
                newName = analysisTemplate.Name;

            ////Copy the template using the same serialization / deserialization as the export
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);

            analysisTemplate.Export(writer);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            //Determine the new name
            string proposedName =
                AnalysisTemplateExists(newName, analysisTemplate.ResponseTemplateID) ? GetUniqueName(analysisTemplate.ResponseTemplateID, newName, languageCode) : newName;

            //Create it
            AnalysisTemplate newTemplate = CreateAnalysisTemplate(proposedName, analysisTemplate.ResponseTemplateID, creator);
            newTemplate.Import(creator, xmlDoc.DocumentElement, null, null, null, true);

            newTemplate.Name = proposedName;
            newTemplate.Save();

            TextManager.SetText(newTemplate.NameTextID, languageCode, proposedName);

            return newTemplate;
        }


        /// <summary>
        /// Determines if the report can be edited
        /// </summary>
        /// <param name="template"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool CanBeEdited(LightweightAnalysisTemplate template, CheckboxPrincipal p)
        {
            return AuthorizationFactory.GetAuthorizationProvider().Authorize(p, template, "Analysis.Edit");
        }

        /// <summary>
        /// Determines if the report can be deleted
        /// </summary>
        /// <param name="template"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool CanBeDeleted(LightweightAnalysisTemplate template, CheckboxPrincipal p)
        {
            return AuthorizationFactory.GetAuthorizationProvider().Authorize(p, template, "Analysis.Delete");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<LightweightAnalysisTemplate> ListReportsByPeriod(CheckboxPrincipal userPrincipal, PaginationContext paginationContext)
        {
            //Prepare command
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = QueryHelper.CreateListAccessibleCommand(
                "ckbx_sp_Security_ListAccessibleAnalysesByPeriod",
                userPrincipal,
                paginationContext);
            command.AddInParameter("StartDate", DbType.DateTime, paginationContext.StartDate ?? SqlDateTime.MinValue);
            command.AddInParameter("DateFieldName", DbType.String, !string.IsNullOrEmpty(paginationContext.DateFieldName) ? paginationContext.DateFieldName : "CreatedDate");

            var templateIdList = new List<int>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Get template data
                    while (reader.Read())
                    {
                        int analysisTemplateId = DbUtility.GetValueFromDataReader(reader, "AnalysisTemplateId", -1);

                        if (analysisTemplateId > 0
                            && !templateIdList.Contains(analysisTemplateId))
                        {
                            templateIdList.Add(analysisTemplateId);
                        }
                    }

                    //Get item count
                    if (reader.NextResult() && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Now build result list
            var resultList = templateIdList.Select(GetLightweightAnalysisTemplate).ToList();
            if (paginationContext.ItemCount == 0)
                paginationContext.ItemCount = resultList.Count;
            return resultList;
        }
    }
}
