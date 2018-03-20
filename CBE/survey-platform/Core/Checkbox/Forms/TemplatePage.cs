using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// Encapsulates access and operations on <see cref="ItemData"/> within the context of a 
    /// <see cref="ResponseTemplate"/>
    /// </summary>
    [Serializable]
    public class TemplatePage : PersistedDomainObject, IEquatable<TemplatePage>
    {
        private List<int> _itemIds;
        private List<int> _analysisItemIds;
        private List<int> _branchRuleIds;

        /// <summary>
        /// Get object type name
        /// </summary>
        public override string ObjectTypeName { get { return "TemplatePage"; } }

        /// <summary>
        /// Get name of load procedure
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_TemplatePage_Get"; } }

        /// <summary>
        /// Get/set parent template id
        /// </summary>
        protected int ParentTemplateId { get; set; }

        /// <summary>
        /// Get data container for the page
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new PersistedDomainObjectDataSet(ObjectTypeName, "TemplatePage", "PageId");
        }

        /// <summary>
        /// Get/set type information for page.
        /// </summary>
        public TemplatePageType PageType { get; set; }

        /// <summary>
        /// Get the page position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Get/set page title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get/set the PageId of the layout template associated with this page
        /// </summary>
        public int? LayoutTemplateId { get; set; }

        /// <summary>
        /// Forces break after this page in PDF
        /// </summary>
        public bool? ShouldForceBreak { get; set; }

        /// <summary>
        /// Create an empty template page
        /// </summary>
        public TemplatePage()
        {
        }

        /// <summary>
        /// Create a new template page for the specified position
        /// </summary>
        /// <param name="parentTemplateId"></param>
        /// <param name="position"></param>
        /// <param name="pageType"></param>
        /// <param name="title"></param>
        public TemplatePage(int parentTemplateId, int position, TemplatePageType pageType, string title)
        {
            ParentTemplateId = parentTemplateId;
            Title = title;
            Position = position;
            PageType = pageType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentTemplateId"></param>
        /// <param name="itemIds"></param>
        /// <param name="branchRuleIds"></param>
        /// <param name="conditionRuleId"></param>
        public void Initialize(int parentTemplateId, List<int> itemIds, List<int> branchRuleIds, int conditionRuleId)
        {
            ParentTemplateId = parentTemplateId;
            _itemIds = itemIds;
            _branchRuleIds = branchRuleIds;
            ConditionRuleId = conditionRuleId;
        }

        /// <summary>
        /// Get data transfer object for item. This object is suitable for binding to item renderers
        /// and/or to external data processes and removes the need for references to core Checkbox dlls.
        /// </summary>
        /// <returns></returns>
        public SurveyResponsePage GetDataTransferObject()
        {
            var pageDto = new SurveyResponsePage
            {
                PageId = ID.Value,
                LayoutTemplateId = LayoutTemplateId,
                ItemIds = ItemIds.ToArray(),
                PageType = PageType.ToString(),
                Position = Position
            };

            return pageDto;
        }


        /// <summary>
        /// Gets a List of ItemDatas associated with this TemplatePage
        /// </summary>
        public int[] ListItemIds()
        {
            return ItemIds.ToArray();
        }

		internal bool ContainsItem(int itemId)
		{
			return ItemIds.Contains(itemId);
		}

        /// <summary>
        /// Get a List of branch rule ids.
        /// </summary>
        /// <returns></returns>
        public int[] ListBranchRuleIds()
        {
            return BranchRuleIds.ToArray();
        }

        /// <summary>
        /// Get the branching rules associated with this page.
        /// </summary>
        private List<int> BranchRuleIds
        {
            get { return _branchRuleIds ?? (_branchRuleIds = new List<int>()); }
        }

        /// <summary>
        /// Get list of item ids
        /// </summary>
        private List<int> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new List<int>()); }
        }
        /// <summary>
        /// Get the rule (condition) associated with this page.
        /// </summary>
        public int ConditionRuleId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="includeOpenEnded"></param>
        /// <returns></returns>
        public bool IsAnyAnswerableItemOnPage(ResponseTemplate rt, bool includeOpenEnded)
        {
            return ListItemIds().Select(rt.GetItem).Any(i => IsItemAnswerable(i, includeOpenEnded));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="includeOpenEnded"></param>
        /// <returns></returns>
        public bool IsItemAnswerable(ItemData itemData, bool includeOpenEnded)
        {
            return itemData is ICompositeItemData || itemData is SelectItemData || (includeOpenEnded
                && (itemData is TextItemData || itemData is HiddenItemData || itemData is UploadItemData));
        }

        #region Persisted Domain Object Members

        /// <summary>
        /// Insert page data into the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_InsertPage");
            
            command.AddInParameter("TemplateId", DbType.Int32, ParentTemplateId);
            command.AddInParameter("PagePosition", DbType.Int32, Position);
            command.AddInParameter("LayoutTemplateId", DbType.Int32, LayoutTemplateId);
            command.AddInParameter("Title", DbType.String, Title);
            command.AddInParameter("PageType", DbType.String, PageType.ToString());
            command.AddInParameter("ShouldForceBreak", DbType.Boolean, ShouldForceBreak);
            command.AddOutParameter("PageId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object pageIdObject = command.GetParameterValue("PageId");

            if (pageIdObject != null && pageIdObject != DBNull.Value)
            {
                ID = (int)pageIdObject;
            }
        }

        /// <summary>
        /// Update page data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_UpdatePage");

            command.AddInParameter("PageId", DbType.Int32, ID);
            command.AddInParameter("TemplateId", DbType.Int32, ParentTemplateId);
            command.AddInParameter("PagePosition", DbType.Int32, Position);
            command.AddInParameter("LayoutTemplateId", DbType.Int32, LayoutTemplateId);
            command.AddInParameter("Title", DbType.String, Title);
            command.AddInParameter("PageType", DbType.String, PageType.ToString());

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Load page data from the specified data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ParentTemplateId = DbUtility.GetValueFromDataRow(data, "TemplateId", -1);
            Position = DbUtility.GetValueFromDataRow(data, "PagePosition", -1);
            LayoutTemplateId = DbUtility.GetValueFromDataRow<int?>(data, "LayoutTemplateId", null);
            Title = DbUtility.GetValueFromDataRow(data, "Title", string.Empty);
            ShouldForceBreak = DbUtility.GetValueFromDataRow<bool?>(data, "ShouldForceBreak", null);

            string pageTypeString = DbUtility.GetValueFromDataRow(data, "PageType", "ContentPage");
            PageType = (TemplatePageType)Enum.Parse(typeof(TemplatePageType), pageTypeString);
        }

        #endregion

        #region IEquatable<TemplatePage> Members

        /// <summary>
        /// Return a boolean indicating if this page is the same as another page.  The comparison
        /// is based on page ids.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TemplatePage other)
        {
            return ID == other.ID;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override string ExportElementName
        {
            get { return "Page"; }
        }

		#region Xml Serializing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteExportAttributes(XmlWriter writer)
        {
            base.WriteExportAttributes(writer);

            writer.WriteAttributeString("Type", PageType.ToString());
            writer.WriteAttributeString("Id", ID.ToString());
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
        protected override void WriteConfigurationToXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
		{
			writer.WriteElementString("Title", Title);
			writer.WriteElementString("Position", Position.ToString());
			writer.WriteElementValue("CreatedDate", CreatedDate);
			writer.WriteElementValue("LastModified", LastModified);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"></param>
        public override void Import(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
		{
			Title = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Title"));
			Position = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("Position")) ?? -1;
			CreatedDate = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("CreatedDate"));
			LastModified = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("LastModified"));

            if(callback != null)
            {
                callback(this, xmlNode, creator);
            }
		}

		#endregion

	}
}
