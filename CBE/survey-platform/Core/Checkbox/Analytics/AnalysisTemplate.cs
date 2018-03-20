using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Analytics.Filters;
using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;
using Checkbox.Analytics.Security;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Management;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Security;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Configuration data for reports associated with a survey.  At runtime, an analysis template acts as a factory for creating
    /// <see cref="Analysis"/> objects that load and summarize response data for a survey.
    /// </summary>
    [Serializable]
    public class AnalysisTemplate : Template
    {
        #region Members

        /// <summary>
        /// A collection of <see cref="AnalysisFilterCollection"/> applied at the top-level of analysis
        /// </summary>
        private AnalysisFilterCollection _filters;

        /// <summary>
        /// Object type name
        /// </summary>
        public override string ObjectTypeName { get { return "AnalysisTemplate"; } }

        /// <summary>
        /// Load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_AnalysisTemplate_Get"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new AnalysisTemplateDataSet(ObjectTypeName);
        }

        #endregion

        #region Access Controllable
        /// <summary>
        /// Construct a new instance of analysis template.
        /// </summary>
        public AnalysisTemplate()
            : base(new[] { "Analysis.Administer", "Analysis.Edit", "Analysis.Run" },
                   new[] { "Analysis.Administer", "Analysis.Edit", "Analysis.Delete", "Analysis.Run" })
        {
        }

        /// <summary>
        /// Get the GUID associated with the analysis.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// Get the ID of the style template associated with this analysis
        /// </summary>
        public int? StyleTemplateID { get; set; }

        /// <summary>
        /// Get/set response template id this analysis reports on responses to.
        /// </summary>
        public Int32 ResponseTemplateID { get; set; }

        /// <summary>
        /// Get whether the analysis template displays survey title
        /// </summary>
        public bool DisplaySurveyTitle { get; set; }

        /// <summary>
        /// Get whether the analysis template displays pdf export button
        /// </summary>
        public bool DisplayPdfExportButton { get; set; }

        /// <summary>
        /// Get whether the analysis template includes incomplete responses
        /// </summary>
        public bool IncludeIncompleteResponses  { get; set; }

        /// <summary>
        /// Get whether the analysis template includes test responses
        /// </summary>
        public bool IncludeTestResponses { get; set; }

        /// <summary>
        /// Get whether the analysis template has been deleted
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Get the text id of the analysis name
        /// </summary>
        public string NameTextID
        {
            get
            {
                if (ID != null && ID.Value > 0)
                {
                    return "/analysisTemplate/" + ID + "/name";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Load analysis template data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ResponseTemplateID = DbUtility.GetValueFromDataRow(data, "ResponseTemplateId", -1);
            Name = DbUtility.GetValueFromDataRow(data, "AnalysisName", string.Empty);
            Guid = DbUtility.GetValueFromDataRow(data, "GUID", Guid.Empty);
            StyleTemplateID = DbUtility.GetValueFromDataRow(data, "StyleTemplateId", (int?)null);
            LastModified = DbUtility.GetValueFromDataRow(data, "ModifiedDate", (DateTime?)null);
            CreatedDate = DbUtility.GetValueFromDataRow(data, "CreatedDate", (DateTime?)null);
            ChartStyleID = DbUtility.GetValueFromDataRow(data, "CharStyleID", (int?)null);
            CreatedBy = DbUtility.GetValueFromDataRow(data, "CreatedBy", string.Empty);
            DisplaySurveyTitle = DbUtility.GetValueFromDataRow(data, "DisplaySurveyTitle", false);
            DisplayPdfExportButton = DbUtility.GetValueFromDataRow(data, "DisplayPdfExportButton", true);
            FilterStartDate = DbUtility.GetValueFromDataRow(data, "DateFilterStart", (DateTime?)null);
            FilterEndDate = DbUtility.GetValueFromDataRow(data, "DateFilterEnd", (DateTime?)null);
            IncludeIncompleteResponses = DbUtility.GetValueFromDataRow(data, "IncludeIncompleteResponses", ApplicationManager.AppSettings.ReportIncompleteResponses);
            IncludeTestResponses = DbUtility.GetValueFromDataRow(data, "IncludeTestResponses", ApplicationManager.AppSettings.ReportTestResponses);
            AclID = DbUtility.GetValueFromDataRow<int?>(data, "AclID", null);
            DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(data, "DefaultPolicy", null);

        }

        ///// <summary>
        ///// Get the name of the identity column in an analysis' configuration data set.
        ///// </summary>
        //public override string IdentityColumnName
        //{
        //    get { return "TemplateID"; }
        //}

        /// <summary>
        /// Create a new analysis template in the database
        /// </summary>
        /// <param name="t">Transaction to participate in when inserting data.</param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis template data.  DataID <= 0.");
            }

            Guid = Guid.NewGuid();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_Insert");
            command.AddInParameter("AnalysisTemplateID", DbType.Int32, ID);
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, ResponseTemplateID);
            command.AddInParameter("AnalysisName", DbType.String, Name);
            command.AddInParameter("NameTextID", DbType.String, NameTextID);
            command.AddInParameter("GUID", DbType.Guid, Guid);
            command.AddInParameter("DateFilterStart", DbType.DateTime, FilterStartDate);
            command.AddInParameter("DateFilterEnd", DbType.DateTime, FilterEndDate);
            command.AddInParameter("ChartStyleID", DbType.Int32, ChartStyleID);
            command.AddInParameter("DisplaySurveyTitle", DbType.Boolean, DisplaySurveyTitle);
            command.AddInParameter("DisplayPdfExportButton", DbType.Boolean, DisplayPdfExportButton);
            command.AddInParameter("IncludeIncompleteResponses", DbType.Boolean, IncludeIncompleteResponses);
            command.AddInParameter("IncludeTestResponses", DbType.Boolean, IncludeTestResponses);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update the analysis item in the database
        /// </summary>
        /// <param name="t">Transaction to participate in when updating data.</param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis template data.  DataID <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_Update");
            command.AddInParameter("AnalysisTemplateID", DbType.Int32, ID);
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, ResponseTemplateID);
            command.AddInParameter("AnalysisName", DbType.String, Name);
            command.AddInParameter("NameTextID", DbType.String, NameTextID);
            command.AddInParameter("DateFilterStart", DbType.DateTime, FilterStartDate);
            command.AddInParameter("DateFilterEnd", DbType.DateTime, FilterEndDate);
            command.AddInParameter("ChartStyleID", DbType.Int32, ChartStyleID);
            command.AddInParameter("DisplaySurveyTitle", DbType.Boolean, DisplaySurveyTitle);
            command.AddInParameter("DisplayPdfExportButton", DbType.Boolean, DisplayPdfExportButton);
            command.AddInParameter("IncludeIncompleteResponses", DbType.Boolean, IncludeIncompleteResponses);
            command.AddInParameter("IncludeTestResponses", DbType.Boolean, IncludeTestResponses);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Delete the analysis from the database.
        /// </summary>
        public override void Delete(IDbTransaction t)
        {
            if (ID <= 0)
            {
                throw new Exception("Unable to delete analysis template data.  DataID <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AnalysisTemplate_Delete");
            command.AddInParameter("AnalysisTemplateID", DbType.Int32, ID);

            db.ExecuteNonQuery(command, t);
        }

        ///// <summary>
        ///// Load base analysis template data and filter information from the specified dataset.
        ///// </summary>
        ///// <param name="data"><see cref="DataSet"/> containing analysis template configuration data.</param>
        //public override void Load(DataSet data)
        //{
        //    base.Load(data);

        //    _filters.ParentID = ID.Value;
        //    DataSet filtersDs = _filters.Load(ID.Value);

        //    data.Merge(filtersDs);

        //    LoadTemplateData(data, false);
        //}

        ///// <summary>
        ///// Load template-level data from the template data set
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="isImport"></param>
        //protected void LoadTemplateData(DataSet data, bool isImport)
        //{
        //    base.LoadTemplateData(data);

        //    if (data.Tables.Count > 0 && data.Tables.Contains(TemplateDataTableName) && data.Tables[TemplateDataTableName].Rows.Count > 0)
        //    {
        //        DataRow row = data.Tables[TemplateDataTableName].Rows[0];

        //        ResponseTemplateID = DbUtility.GetValueFromDataRow(row, "ResponseTemplateID", -1);

        //        if (!isImport)
        //        {
        //            AclID = DbUtility.GetValueFromDataRow<int?>(row, "AclID", null);
        //            DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(row, "DefaultPolicy", null);
        //            LastModified = DbUtility.GetValueFromDataRow<DateTime?>(row, "ModifiedDate", null);
        //            GUID = DbUtility.GetValueFromDataRow(row, "GUID", default(Guid));
        //            ChartStyleID = DbUtility.GetValueFromDataRow<int?>(row, "ChartStyleID", null);
        //        }

        //        StyleTemplateID = DbUtility.GetValueFromDataRow<int?>(row, "StyleTemplateID", null);
        //        Name = DbUtility.GetValueFromDataRow(row, "AnalysisName", string.Empty);
        //        FilterStartDate = DbUtility.GetValueFromDataRow<DateTime?>(row, "DateFilterStart", null);
        //        FilterEndDate = DbUtility.GetValueFromDataRow<DateTime?>(row, "DateFilterEnd", null);
        //        IsDeleted = DbUtility.GetValueFromDataRow(row, "Deleted", false);
        //    }
        //}

        ///// <summary>
        ///// Import the analysis template with the the specified ID from the specified data set.
        ///// </summary>
        ///// <param name="ds"><see cref="DataSet"/> containing configuration information for template to import.</param>
        ///// <param name="oldTemplateID">ID of analysis template to import.</param>
        ///// <remarks>The template id is needed to import filter information from the DataSet.</remarks>
        //public void Import(DataSet ds, Int32 oldTemplateID)
        //{
        //    base.Import(ds);

        //    //Set filter parent id
        //    _filters.ParentID = ID.Value;

        //    //Now import filters
        //    ImportFilters(ds, oldTemplateID);
        //}

        ///// <summary>
        ///// Import template-level data from the dataset
        ///// </summary>
        ///// <param name="ds"></param>
        //protected override void ImportTemplateData(DataSet ds)
        //{
        //    base.ImportTemplateData(ds);

        //    LoadTemplateData(ds, true);
        //}

        ///// <summary>
        ///// Import filters
        ///// </summary>
        ///// <param name="ds"></param>
        ///// <param name="oldTemplateID"></param>
        //protected virtual void ImportFilters(DataSet ds, int oldTemplateID)
        //{
        //    _filters.Import(ds, oldTemplateID);
        //}

        ///// <summary>
        ///// Import the specified item
        ///// </summary>
        ///// <param name="itemID">ID of the item before import</param>
        ///// <param name="ds"></param>
        ///// <returns></returns>
        //protected override ItemData ImportItem(int itemID, PersistedDomainObjectDataSet ds)
        //{
        //    ItemData itemData = base.ImportItem(itemID, ds);

        //    //If an analysis item, import any filters
        //    if (itemData is AnalysisItemData)
        //    {
        //        ((AnalysisItemData)itemData).ImportFilters(ds, Math.Abs(itemID));
        //    }

        //    return itemData;
        //}

        /// <summary>
        /// Get a security editor object that can be used to modify the ACL and Default Policy for the analysis template.
        /// </summary>
        /// <returns>Security editor object initialized with the analysis template as it's secured resource.</returns>
        public override SecurityEditor GetEditor()
        {
            return new AnalysisSecurityEditor(this);
        }

        /// <summary>
        /// Create a policy for storing permissions for the analysis template.
        /// </summary>
        /// <param name="permissions">Permissions to add to the new policy.</param>
        /// <returns><see cref="Policy"/> configured with the specified filter.</returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new AnalysisPolicy(permissions);
        }
        #endregion

        /// <summary>
        /// Minimum timestamp for completed responses to include in the analysis.
        /// </summary>
        public DateTime? FilterStartDate { get; set; }

        /// <summary>
        /// Maximum timestamp for completed responses to include in the analysis.
        /// </summary>
        public DateTime? FilterEndDate { get; set; }

        /// <summary>
        /// Get/set chart style id for chart style associated with the analysis tempate.
        /// </summary>
        public int? ChartStyleID { get; set; }

        /// <summary>
        /// Create an instance of an analysis configured from this analysis template.
        /// </summary>
        /// <param name="languageCode">Language code to use for getting analysis text.</param>
        /// <param name="includeIncompleteResponses">Specify whether incomplete responses should be included in results.</param>
        /// <param name="includeTestResponses">Specify whether test responses should be included in results.</param>
        /// <returns>Loaded and configured <see cref="Analysis"/> object.</returns>
        public Analysis CreateAnalysis(string languageCode, bool includeIncompleteResponses, bool includeTestResponses)
        {
            EnsureFilterCollection();

            var analysis = new Analysis { Name = TextManager.GetText(NameTextID, languageCode) };

            //Set ID of template.  In some cases, such as with on-the-fly created
            // templates for exporting, there will be no value.
            if (ID.HasValue)
            {
                analysis.SetId(ID.Value);
            }

            foreach (int itemId in ItemIds)
            {
                ItemData itemData = GetItem(itemId);
                itemData.ParentTemplateId = ID;
                Item item = itemData.CreateItem(languageCode, ResponseTemplateID, ID);

                if (item is AnalysisItem)
                {
                    ((AnalysisItem)item).RunMode = true;
                }

                analysis.AddItem(item);
            }

            foreach (int pageId in PageIds)
            {
                TemplatePage page = GetPage(pageId);
                var analysisPage = new AnalysisPage(page.ID.Value, page.Position, page.LayoutTemplateId) { Parent = analysis };

                foreach (int id in page.ListItemIds())
                {
                    analysisPage.AddItemID(id);
                }

                analysis.AddPage(analysisPage);
            }

            analysis.AddResponseTemplateID(ResponseTemplateID);

            analysis.Initialize(_filters, languageCode, FilterStartDate, FilterEndDate, includeIncompleteResponses, includeTestResponses, LastModified);

            return analysis;
        }

        /// <summary>
        /// Initialize the access for the object
        /// </summary>
        /// <param name="defaultPolicy"></param>
        /// <param name="acl"></param>
        internal void InitializeAccess(Policy defaultPolicy, AccessControlList acl)
        {
            if (ID != null && ID > 0)
            {
                throw new Exception("Access can only be initialized for a new analysis template.");
            }

            ArgumentValidation.CheckExpectedType(defaultPolicy, typeof(AnalysisPolicy));

            SetAccess(defaultPolicy, acl);
        }

        #region Filters for Analysis

        /// <summary>
        /// Ensure filter collection exists and is initialized with report id
        /// </summary>
        protected void EnsureFilterCollection()
        {
            if (_filters == null)
            {
                _filters = new AnalysisFilterCollection();

                if (ID.HasValue)
                {
                    _filters.ParentID = ID.Value;
                    _filters.Load(ID.Value);
                }
            }
        }

        /// <summary>
        /// Get a readonly collection containing analysis filters associated with this response template
        /// </summary>
        public List<FilterData> GetFilterDataObjects()
        {
            EnsureFilterCollection();

            return _filters.GetFilterDataObjects();
        }

        /// <summary>
        /// Add a repoort-level filter to the analysis template.
        /// </summary>
        /// <param name="filter">Filter to add to the collection.</param>
        public void AddFilter(FilterData filter)
        {
            EnsureFilterCollection();

            _filters.AddFilter(filter);
        }

        /// <summary>
        /// Associated an  existing filter with the analysis tempate.
        /// </summary>
        /// <param name="filterID">Database ID of filter to associated with the template.</param>
        public void AddFilter(Int32 filterID)
        {
            EnsureFilterCollection();

            _filters.AddFilter(filterID);
        }

        /// <summary>
        /// Remove the mapping between a filter and the analysis template.
        /// </summary>
        /// <param name="filter">Filter to remove mapping for.</param>
        public void DeleteFilter(FilterData filter)
        {
            EnsureFilterCollection();

            _filters.DeleteFilter(filter);
        }

        /// <summary>
        /// Remove the mapping between a filter and the analysis template.
        /// </summary>
        /// <param name="filterID">ID of filter to remove mapping for.</param>
        public void DeleteFilter(Int32 filterID)
        {
            EnsureFilterCollection();

            _filters.DeleteFilter(filterID);
        }

        /// <summary>
        /// Remove the mapping between all filter and the analysis template.
        /// </summary>
        public void ClearFilters()
        {
            EnsureFilterCollection();

            List<FilterData> filterList = _filters.GetFilterDataObjects();
            List<int> filterIdsToDelete = (from filterData in filterList where filterData.ID.HasValue select filterData.ID.Value).ToList();

            foreach (int filterId in filterIdsToDelete)
            {
                DeleteFilter(filterId);
            }
        }

        /// <summary>
        /// Save filter objects associated with this analysis template and save the filter to analysis template mapping information for
        /// this analysis template.
        /// </summary>
        public void SaveFilters()
        {
            if (_filters != null)
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    try
                    {
                        connection.Open();
                        IDbTransaction t = connection.BeginTransaction();
                        try
                        {
                            _filters.Save(t, db, true);
                            t.Commit();

                            //Force filters to be reloaded
                            _filters = null;
                        }
                        catch (Exception)
                        {
                            t.Rollback();
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPrivate");

                        if (rethrow)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Get the name of the data table for response template data
        /// </summary>
        public static new string DataTableName
        {
            get { return "AnalysisTemplateData"; }
        }

        #region Export
        /// <summary>
        /// Writes template data to the XML writer
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteTemplateData(XmlWriter writer)
        {
            writer.WriteElementString("AnalysisName", Name);
            writer.WriteElementString("Guid", Guid.ToString());

            writer.WriteElementValue<DateTime>("FilterStartDate", FilterStartDate);
            writer.WriteElementValue<DateTime>("FilterEndDate", FilterEndDate);
            writer.WriteElementValue<int>("ChartStyleID", ChartStyleID);
            writer.WriteElementValue<int>("StyleTemplateID", this.StyleTemplateID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteCustomTextData(XmlWriter writer)
        {
        }
        #endregion

        #region Import

        /// <summary>
        /// 
        /// </summary>
        public override string ExportElementName
        {
            get { return "AnalysisTemplate"; }
        }

        /// <summary>
        /// Reads template data from the XML
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplateData(XmlNode xmlNode)
        {
            var templateNode = xmlNode.SelectSingleNode("TemplateData") ?? xmlNode;

            if (templateNode == null)
            {
                throw new Exception("Unable to load AnalysisTemplate: TemplateData node not found.");
            }

            Name = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AnalysisName"));

            FilterStartDate = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("FilterStartDate"));
            FilterEndDate = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("FilterEndDate"));
            ChartStyleID = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("ChartStyleID"));
            StyleTemplateID = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("StyleTemplateID"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void LoadTemplateCustomTextData(XmlNode xmlNode)
        {
        }

        #endregion Import
    }
}
