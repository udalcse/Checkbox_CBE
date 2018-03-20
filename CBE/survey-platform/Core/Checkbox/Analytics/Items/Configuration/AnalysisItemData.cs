using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Checkbox.Analytics.Filters;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Analytics.Items.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public abstract class AnalysisItemData : ItemData
    {
        /// <summary>
        /// The <see cref="AnalysisItemFilterCollection"/> to be applied.
        /// </summary>
        private AnalysisItemFilterCollection _filters;

        private List<int> _sourceItems;
        private List<int> _sourceResponseTemplates;

        #region Constructor/Init

        /// <summary>
        /// Get name of analysis item data table
        /// </summary>
        public override string ItemDataTableName { get { return "AnalysisItemData"; } }

        /// <summary>
        /// Do not allow export of analysis item data
        /// </summary>
        public override bool IsExportable { get { return false; } }

        /// <summary>
        /// List source item ids
        /// </summary>
        public List<int> SourceItemIds
        {
            get { return _sourceItems ?? (_sourceItems = new List<int>()); }
        }

        /// <summary>
        /// List source response templates
        /// </summary>
        public List<int> ResponseTemplateIds
        {
            get { return _sourceResponseTemplates ?? (_sourceResponseTemplates = new List<int>()); }
        }

        #endregion
      
        #region Properties

        /// <summary>
        /// Override initialize to initialize analysis items with filters
        /// </summary>
        /// <param name="item"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        protected override void InitializeItem(Item item, string languageCode, int? templateId)
        {
            EnsureFilterCollection();

            var analysisItem = item as AnalysisItem;
            if (analysisItem != null && templateId.HasValue)
            {
                analysisItem.SourceResponseTemplateId = templateId.Value;
                analysisItem.Configure(this, languageCode, _filters);
            }
        }
        /// <summary>
        /// Override initialize to initialize analysis items with filters
        /// </summary>
        protected override void InitializeItem(Item item, string languageCode, int? responseTemplateId, int? analysisTemplateId)
        {
            EnsureFilterCollection();

            var analysisItem = item as AnalysisItem;
            if (analysisItem != null)
            {
                if (analysisTemplateId.HasValue)
                    analysisItem.SourceAnalysisTemplateId = analysisTemplateId.Value;
                
                if (responseTemplateId.HasValue)
                    analysisItem.SourceResponseTemplateId = responseTemplateId.Value;
                else if (analysisTemplateId.HasValue)
                    analysisItem.SourceResponseTemplateId = AnalysisTemplateManager.GetAnalysisTemplate(analysisTemplateId.Value).ResponseTemplateID;

                analysisItem.Configure(this, languageCode, _filters);
            }
        }

        /// <summary>
        /// Import filters from the data set
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="oldItemID"></param>
        public void ImportFilters(DataSet ds, Int32 oldItemID)
        {
            EnsureFilterCollection();

            _filters.Import(ds, oldItemID);
        }

        /// <summary>
        /// Ensure filter collection is not null
        /// </summary>
        private void EnsureFilterCollection()
        {
            if (_filters == null)
            {
                _filters = new AnalysisItemFilterCollection();
                
                if (ID.HasValue)
                {
                    _filters.ParentID = ID.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseAliases { get; set; }

        /// <summary>
        /// Add a filter to the item's filter collection
        /// </summary>
        /// <param name="filterID"></param>
        public virtual void AddFilter(Int32 filterID)
        {
            EnsureFilterCollection();

            _filters.AddFilter(filterID);
        }

        /// <summary>
        /// Add a filter to the item's filter collection
        /// </summary>
        /// <param name="filter"></param>
        public virtual void AddFilter(FilterData filter)
        {
            EnsureFilterCollection();

            _filters.AddFilter(filter);
        }

        /// <summary>
        /// Remove a filter from the item's filter collection
        /// </summary>
        /// <param name="filterID"></param>
        public virtual void DeleteFilter(Int32 filterID)
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
            
            var filterIdsToDelete = (from filterData in filterList where filterData.ID.HasValue select filterData.ID.Value).ToList();

            foreach (int filterId in filterIdsToDelete)
            {
                DeleteFilter(filterId);
            }
        }

        /// <summary>
        /// Remove a filter from the item's filter collection
        /// </summary>
        public virtual void DeleteFilter(FilterData filter)
        {
            if (filter != null)
            {
                DeleteFilter(filter.ID.Value);
            }
        }

        /// <summary>
        /// Create analysis item configuration data set.
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new AnalysisItemDataSet(ObjectTypeName, "AnalysisItemData");
        }

        /// <summary>
        /// Save the item filters
        /// </summary>
        public virtual void SaveFilters()
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
                        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

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

        /// <summary>
        /// Add a source item association
        /// </summary>
        /// <param name="sourceItemID"></param>
        public virtual void AddSourceItem(Int32 sourceItemID)
        {
            if (sourceItemID > 0
                && !SourceItemIds.Contains(sourceItemID))
            {
                SourceItemIds.Add(sourceItemID);
            }
        }

        /// <summary>
        /// Remove a source item association
        /// </summary>
        /// <param name="sourceItemID"></param>
        public virtual void RemoveSourceItem(Int32 sourceItemID)
        {
            if (SourceItemIds.Contains(sourceItemID))
            {
                SourceItemIds.Remove(sourceItemID);
            }
        }

        /// <summary>
        /// Add a response template association
        /// </summary>
        /// <param name="responseTemplateID"></param>
        public virtual void AddResponseTemplate(Int32 responseTemplateID)
        {
            if (responseTemplateID > 0
                && !ResponseTemplateIds.Contains(responseTemplateID))
            {
                ResponseTemplateIds.Add(responseTemplateID);
            }
        }

        /// <summary>
        /// Remove a source item association
        /// </summary>
        /// <param name="responseTemplateID"></param>
        public virtual void RemoveResponseTemplate(Int32 responseTemplateID)
        {
            if (ResponseTemplateIds.Contains(responseTemplateID))
            {
                ResponseTemplateIds.Remove(responseTemplateID);
            }
        }

        /// <summary>
        /// Get the filters for this item
        /// </summary>
        public List<FilterData> Filters
        {
            get
            {
                EnsureFilterCollection();

                return _filters.GetFilterDataObjects();
            }
        }

        #endregion

        #region Load

        /// <summary>
        /// Load from datarow
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            if (data != null)
            {
                UseAliases = DbUtility.GetValueFromDataRow(data, "UseAliases", false);
            }
        }

        /// <summary>
        /// Load the item from the database
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            if (data == null)
            {
                throw new Exception("Unable to load analysis item from NULL data.");
            }

            base.LoadAdditionalData(data);

            SourceItemIds.Clear();
            ResponseTemplateIds.Clear();

            //Load items and response templates
            DataRow[] sourceItemRows = ((AnalysisItemDataSet)data).GetSourceItems();

            foreach (DataRow sourceItemRow in sourceItemRows)
            {
                var itemId = DbUtility.GetValueFromDataRow<int?>(sourceItemRow, "SourceItemId", null);

                if (itemId.HasValue)
                {
                    SourceItemIds.Add(itemId.Value);
                }
            }

            DataRow[] sourceTemplateRows = ((AnalysisItemDataSet)data).GetResponseTemplates();

            foreach (DataRow sourceTemplateRow in sourceTemplateRows)
            {
                var itemId = DbUtility.GetValueFromDataRow<int?>(sourceTemplateRow, "ResponseTemplateId", null);

                if (itemId.HasValue)
                {
                    ResponseTemplateIds.Add(itemId.Value);
                }
            }

            EnsureFilterCollection();

            //Load filters
            _filters.Load(ID.Value);
        }

        #endregion

        #region Save

        /// <summary>
        /// Update source item tables
        /// </summary>
        /// <param name="t"></param>
        protected virtual void UpdateSourceItemTables(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            EnsureFilterCollection();

            //Save the filters
            _filters.ParentID = ID.Value;
            _filters.Save(t, db, false);

            //Delete source item mappings
            DBCommandWrapper deleteSourceItemCommand = GetDeleteSourceItemCommand(db);
            db.ExecuteNonQuery(deleteSourceItemCommand, t);

            //Delete response temlate mappings
            DBCommandWrapper deleteSourceRtCommand = GetDeleteRtCommand(db);
            db.ExecuteNonQuery(deleteSourceRtCommand, t);

            //TODO: Add ordering to stored procedure
            //Add mappings
            foreach(int sourceItemId in SourceItemIds)
            {
                DBCommandWrapper insertSourceItemCommand = GetInsertSourceItemCommand(db);
                insertSourceItemCommand.AddInParameter("SourceItemId", DbType.Int32, sourceItemId);
                db.ExecuteNonQuery(insertSourceItemCommand, t);
            }

            //Add mappings
            foreach(int responseTemplateId in ResponseTemplateIds)
            {
                DBCommandWrapper insertSourceRtCommand = GetInsertRtCommand(db);
                insertSourceRtCommand.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

                db.ExecuteNonQuery(insertSourceRtCommand, t);
            }
        }


        /// <summary>
        /// Get a command to insert a source item association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertSourceItemCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertAISource");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        /// Get a command to delete a source item association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeleteSourceItemCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeleteAISource");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        /// Get a command to insert a response template association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertRtCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertAIRT");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        ///  Get a command to delete a response template association
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeleteRtCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeleteAIRT");
            command.AddInParameter("AnalysisItemID", DbType.Int32, ID);

            return command;
        }

        #endregion

        #region DataTransfer
        
        /// <summary>
        /// Create data transfer object for viewing analysis item configuration remotely.
        /// </summary>
        /// <returns></returns>
        public override IItemMetadata CreateDataTransferObject()
        {
            return new ItemMetaData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ItemMetaData)
            {
                ((ItemMetaData)itemDto).TextData =
                    ResponseTemplateManager.ActiveSurveyLanguages.Select(
                        language => new TextData {LanguageCode = language, TextValues = new SimpleNameValueCollection()})
                        .ToArray();

                //Pick first source item to use for text
                var textDataList = ((ItemMetaData) itemDto).TextData;

                //
                foreach (var textData in textDataList)
                {
                    if (SourceItemIds.Count > 0)
                    {
                        textData.TextValues["NavText"] =
                            Utilities.DecodeAndStripHtml(
                                ItemConfigurationManager.GetItemText(
                                    SourceItemIds[0],
                                    textData.LanguageCode, 
                                    null,
                                    UseAliases, 
                                    false),
                            50);
                    }
                }
            }
        }
        #endregion

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("ResponseTemplateIds", String.Join(",", ResponseTemplateIds));
            writer.WriteElementString("SourceItemIds", String.Join(",", SourceItemIds));
            writer.WriteElementString("UseAliases", UseAliases.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);
            
            var templates = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ResponseTemplateIds"));
            
            if (!String.IsNullOrEmpty(templates))
                _sourceResponseTemplates = templates.Split(',').Select(int.Parse).ToList();

            var items = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("SourceItemIds"));

            if (!String.IsNullOrEmpty(items))
                _sourceItems = items.Split(',').Select(int.Parse).ToList();
            
            UseAliases = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("UseAliases"));
        }
        #endregion

        /// <summary>
        /// Do nothing for Analysis
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(UpdatePipesCallback callback)
        {            
        }
    }
}
