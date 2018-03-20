//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Security.Principal;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Abstract class representing item configuration data.
    /// </summary>
    [Serializable]
    public abstract class ItemData : PersistedDomainObject, IEquatable<ItemData>
    {
        #region Properties

        /// <summary>
        /// Get default identity column name, which is item id
        /// </summary>
        public override string IdentityColumnName { get { return "ItemId"; } }

        /// <summary>
        /// Parent data table name
        /// </summary>
        public virtual string ParentDataTableName
        {
            get { return "Items"; }
        }

        /// <summary>
        /// Specifies the profile property the item is connected to
        /// </summary>
        public int? ProfilePropertyId { get; set; }

        /// <summary>
        /// Add constructor to ensure items are active by default
        /// </summary>
        protected ItemData()
        {
            IsActive = true;
			ItemTypeID = -1;
        }

        /// <summary>
        /// Get whether the item data can be exported
        /// </summary>
        public virtual bool IsExportable
        {
            get { return true; }
        }

        ///<summary>
        /// Get/set the id of parent template.
        ///</summary>
        public int? ParentTemplateId { get; set; }

        /// <summary>
        /// Get/set the alias of an item.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Get/set the typeid for this item
        /// </summary>
        public int ItemTypeID { get; set; }

        /// <summary>
        /// Get the typename for this item
        /// </summary>
        public string ItemTypeName { get; set; }

        /// <summary>
        /// Matrix id for children
        /// </summary>
        [XmlIgnore]
        public int? ParentItemId { get; set; }

        /// <summary>
        /// Get whether the item has been deleted
        /// </summary>
        [XmlIgnore]
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Who created the item
        /// </summary>
        [XmlIgnore]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Who modified the item
        /// </summary>
        [XmlIgnore]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Get/set whether the item is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get item data
        /// </summary>
        public override string ObjectTypeName { get { return "ItemData"; } }

        /// <summary>
        /// Get name of an item's data table when loading data from a data set.
        /// </summary>
        public abstract string ItemDataTableName { get; }

        /// <summary>
        /// Data table name for  configuration data.
        /// </summary>
        public override string DataTableName { get { return ItemDataTableName; } }

        /// <summary>
        /// Create configuration data for items
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ItemDataSet(ObjectTypeName, ItemDataTableName);
        }

        /// <summary>
        /// Unless overriden, return parameter named item id
        /// </summary>
        /// <returns></returns>
        protected override List<DbParameter> GetLoadSprocParameters(PersistedDomainObjectDataSet ds)
        {
            return new List<DbParameter> { new GenericDbParameter(ds.IdentityColumnName, DbType.Int32, ID) };
        }

        /// <summary>
        /// Get an indication of whether the item created by CreateItem(...) supports
        /// the IAnswerable interface.
        /// </summary>
        public virtual bool ItemIsIAnswerable
        {
            get { return false; }
        }

        /// <summary>
        /// Get text id prefix.  No prefix indicates item does not have localized text
        /// </summary>
        public string TextIdPrefix
        {
            get
            {
                ItemTypeInfo info = null;

                if (ItemTypeID != -1)
                    info = ItemConfigurationManager.GetTypeInfoById(ItemTypeID);
                else if (!string.IsNullOrEmpty(ItemTypeName))
                    info = ItemConfigurationManager.GetTypeInfoByName(ItemTypeName);

                return info != null ? info.TextIdPrefix : string.Empty;
            }
        }

        /// <summary>
        /// Get an indication of whether the item created by CreateItem(...) supports
        /// the IScored interface
        /// </summary>
        public virtual bool ItemIsIScored
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ExportElementName
        {
            get { return "Item"; }
        }

        #endregion

        #region Abstract Members

        /// <summary>
        /// Create an item based on this configuration.
        /// </summary>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int? templateId)
        {
            Item item = CreateItem();

            if (item != null)
            {
                InitializeItem(item, languageCode, templateId);
                return item;
            }

            return null;
        }

        /// <summary>
        /// Create an item based on this configuration.
        /// </summary>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int? responseTemplateId, int? analysisTemplateId)
        {
            Item item = CreateItem();

            if (item != null)
            {
                InitializeItem(item, languageCode, responseTemplateId, analysisTemplateId);
                return item;
            }

            return null;
        }

        /// <summary>
        /// Create an item based on this configuration.
        /// </summary>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int? templateId, ExportMode exportMode)
        {
            Item item = CreateItem();

            if (item != null)
            {
                item.ExportMode = exportMode;
                InitializeItem(item, languageCode, templateId);
                return item;
            }

            return null;
        }

        /// <summary>
        /// Create an item based on this configuration.
        /// </summary>
        /// <returns></returns>
        public Item CreateItem(string languageCode, int? templateId, Response response)
        {
            Item item = CreateItem();

            if (response != null)
            {
                var responseItem = item as ResponseItem;
                if (responseItem != null && responseItem.Response == null)
                    responseItem.Response = response;
            }

            if (item != null)
            {
                InitializeItem(item, languageCode, templateId);
                return item;
            }

            return null;
        }

        /// <summary>
        /// Create an instance of an item to be initialized with its configuration
        /// </summary>
        /// <returns></returns>
        protected abstract Item CreateItem();

        /// <summary>
        /// Initialize the item with it's configuration
        /// </summary>
        protected virtual void InitializeItem(Item item, string languageCode, int? templateId)
        {
            item.Configure(this, languageCode, templateId);
        }

        /// <summary>
        /// Initialize the item with it's configuration
        /// </summary>
        protected virtual void InitializeItem(Item item, string languageCode, int? responseTemplateId, int? analysisTemplateId)
        {
            item.Configure(this, languageCode, responseTemplateId);
        }

        /// <summary>
        /// Creates an <see cref="ItemTextDecorator"/> for the typed ItemData
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public virtual ItemTextDecorator CreateTextDecorator(string languageCode) { return new ItemTextDecorator(this, languageCode); }


        #endregion

        #region CRUD

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            int newItemId = ItemConfigurationManager.InsertAbstractItem(
                ItemTypeName,
                Alias,
                true,
                CreatedBy,
                t);

            if (newItemId <= 0)
            {
                throw new Exception("Unable to save item data.");
            }


            ID = newItemId;
        }

        /// <summary>
        /// Update the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            ItemConfigurationManager.UpdateAbstractItem(ID.Value, Alias, IsActive, ModifiedBy, t);
        }

        /// <summary>
        /// Delete the specified configuration for the item.
        /// </summary>
        public override void Delete(IDbTransaction t)
        {
            ItemConfigurationManager.DeleteAbstractItem(ID.Value, t);
        }

        #endregion

        #region Load

        /// <summary>
        /// Load "additional" data, which would include the abstract item data.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            if (data is ItemDataSet)
            {
                DataRow parentDataRow = ((ItemDataSet)data).GetParentDataRow();

                Alias = DbUtility.GetValueFromDataRow(parentDataRow, "Alias", string.Empty);
                ItemTypeID = DbUtility.GetValueFromDataRow(parentDataRow, "ItemTypeID", -1);
                ItemTypeName = DbUtility.GetValueFromDataRow(parentDataRow, "ItemName", string.Empty);
                CreatedDate = DbUtility.GetValueFromDataRow<DateTime?>(parentDataRow, "CreatedDate", null);
                LastModified = DbUtility.GetValueFromDataRow<DateTime?>(parentDataRow, "ModifiedDate", null);
                IsActive = DbUtility.GetValueFromDataRow(parentDataRow, "IsActive", true);
                ParentItemId = DbUtility.GetValueFromDataRow(parentDataRow, "ParentItemID", default(int?));
            }

            base.LoadAdditionalData(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadAdditionalData(string alias, int itemTypeId, string itemName,
            DateTime? createdDate, DateTime? lastModified, bool isActive, int? parentItemId)
        {
            Alias = alias;
            ItemTypeID = itemTypeId;
            ItemTypeName = itemName;
            CreatedDate = createdDate;
            LastModified = lastModified;
            IsActive = isActive;
            ParentItemId = parentItemId;
        }

        #endregion

        #region IEquatable<ItemData> Members

        /// <summary>
        /// Equate item data based on id.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ItemData other)
        {
            if (ID == other.ID)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"></param>
        public override void Import(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            Import(xmlNode, null, callback, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="itemReader"></param>
        /// <param name="callback"></param>
        /// <param name="creator"></param>
        public void Import(XmlNode xmlNode, ItemImportReader itemReader, ReadExternalDataCallback callback = null, object creator = null)
        {
            //Read common xml
            ReadCommonXml(xmlNode);

            if (creator != null)
            {
                CheckboxPrincipal principal = (CheckboxPrincipal)creator;
                CreatedBy = principal.Identity.Name;
                ModifiedBy = principal.Identity.Name;
            }

            //Save before reading type-specific stuff which may require a valid
            // item id.
            Save();

            //read item mapping data 
            ReadItemImportData(itemReader);

            //Read type specific Xml
            ReadItemTypeSpecificXml(xmlNode, callback, creator);

            //Read appearance
            var appearanceData = ReadAppearanceXml(xmlNode, callback);

            if (ID.HasValue && appearanceData != null)
                appearanceData.Save(ID.Value);

            if (callback != null)
            {
                callback(this, xmlNode, creator);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetDefaults(Template template) { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetDefaultTexts() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected void ReadCommonXml(XmlNode xmlNode)
		{
            Alias = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Alias"));
            ItemTypeName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ItemTypeName"));
            CreatedDate = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("CreatedDate"));
            LastModified = XmlUtility.GetNodeDate(xmlNode.SelectSingleNode("LastModified"));
            IsActive = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsActive"));
            ParentItemId = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("ParentItemId"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"></param>
        protected AppearanceData ReadAppearanceXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            var appearanceDataNode = xmlNode.SelectSingleNode("AppearanceData");

            AppearanceData data = AppearanceDataManager.GetDefaultAppearanceDataForType(ItemTypeID);

            if (data != null && appearanceDataNode != null)
            {
                data.Import(appearanceDataNode, callback, creator);
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadItemImportData(ItemImportReader reader)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlkNode"></param>
        protected virtual void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
              //TODO : timeline feature which broke the survey import
//            if (callback != null)
//                callback(this, xmlNode, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteExportAttributes(XmlWriter writer)
        {
            base.WriteExportAttributes(writer);

            writer.WriteAttributeString("TypeName", GetType().Name);
            writer.WriteAttributeString("Id", ID.ToString());
        }

        /// <summary>
        /// Serialize this object to XML
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteConfigurationToXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            if (!ID.HasValue)
                return; // item is not in the DB

            //Write common Xml
            WriteCommonXml(writer);

            //Write type specific Xml
            WriteItemTypeSpecificXml(writer, externalDataCallback);

            //Write appearance
            WriteAppearanceXml(writer);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteCommonXml(XmlWriter writer)
        {
            writer.WriteElementString("Alias", Alias);
            writer.WriteElementString("ItemTypeName", ItemTypeName);
            writer.WriteElementValue("CreatedDate", CreatedDate);
            writer.WriteElementValue("LastModified", LastModified);
            writer.WriteElementString("IsActive", IsActive.ToString());
            writer.WriteElementValue("ParentItemId", ParentItemId);
            writer.WriteElementString("TextIdPrefix", TextIdPrefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteAppearanceXml(XmlWriter writer)
        {
            AppearanceData appearance = AppearanceDataManager.GetAppearanceDataForItem(ID.Value);

            if (appearance != null)
                appearance.WriteXml(writer);
        }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemIdMap"></param>
		/// <param name="pageIdMap"></param>
        protected internal virtual void UpdateImportId(Dictionary<int, ItemData> itemIdMap, Dictionary<int, TemplatePage> pageIdMap) { }

        #endregion

        /// <summary>
        /// Create a copy of the ItemData.
        /// </summary>
        /// <param name="idOfCopy">ID to assign to the copy</param>
        /// <returns></returns>
        /// <remarks>
        /// In general, this method should only be used in cases where using the ItemConfigurationManager to
        /// make the copy would not be appropriate, such as when there is an open DB transaction.
        /// </remarks>
        public ItemData Copy(int? idOfCopy)
        {
            ItemData theCopy = Copy();

            if (theCopy != null)
            {
                theCopy.Alias = Alias;
                theCopy.ItemTypeID = ItemTypeID;
                theCopy.ItemTypeName = ItemTypeName;
                theCopy.ID = idOfCopy;
                theCopy.IsActive = IsActive;
            }

            return theCopy;
        }

        /// <summary>
        /// Peform the work of copying the item.  This should be overridden by appropriate child objects.  For the 4.0 release, 
        /// the Copy(...) method is only used by MatrixItemData so the only classes that extend this method will be
        /// those that can appear in a matrix.
        /// </summary>
        /// <returns></returns>
        protected virtual ItemData Copy()
        {
            var theClone = ItemConfigurationManager.CreateConfigurationData(ItemTypeID);
            theClone.Alias = Alias;
            theClone.CreatedDate = CreatedDate;
            theClone.ID = ID;
            theClone.IsActive = IsActive;
            theClone.ItemTypeID = ItemTypeID;
            theClone.ItemTypeName = ItemTypeName;
            theClone.LastModified = LastModified;
            theClone.ParentItemId = ParentItemId;
            return theClone;
        }

        #region Data Transfer Object For Remote Editing

        /// <summary>
        /// Create the data transfer object for the item
        /// </summary>
        /// <returns></returns>
        public abstract IItemMetadata CreateDataTransferObject();

        /// <summary>
        /// Update item state based on input DTO
        /// </summary>
        /// <param name="dto"></param>
        public virtual void UpdateFromDataTransferObject(IItemMetadata dto)
        {
        }

        /// <summary>
        /// Get data transfer object for item. This object is suitable for binding to item editors
        /// and/or to external data processes and removes the need for references to core Checkbox dlls.
        /// </summary>
        /// <returns></returns>
        public IItemMetadata GetDataTransferObject(Template template)
        {
            //Create the object
            var itemDto = CreateDataTransferObject();

            //Build object
            if (itemDto != null)
            {
                BuildDataTransferObject(itemDto);

                if (itemDto is ItemMetaData && template != null)
                {
                    ((ItemMetaData)itemDto).ItemPosition = template.GetItemPositionWithinPage(ID.Value) ?? 0;
                    ((ItemMetaData)itemDto).PagePosition = template.GetPagePositionForItem(ID.Value) ?? 0;
                }
            }

            return itemDto;
        }

        /// <summary>
        /// Build up data transfer object for survey item.
        /// </summary>
        /// <param name="itemDto"></param>
        protected virtual void BuildDataTransferObject(IItemMetadata itemDto)
        {
            if (itemDto is ItemMetaData)
            {
                ((ItemMetaData)itemDto).ItemId = ID ?? -1;
                ((ItemMetaData)itemDto).TypeName = ItemTypeName;

                ((ItemMetaData)itemDto).TextData =
                    ResponseTemplateManager.ActiveSurveyLanguages.Select(
                        language => new TextData { LanguageCode = language, TextValues = new SimpleNameValueCollection() }).ToArray();

                ((ItemMetaData)itemDto)["IsAnswerable"] = ItemIsIAnswerable.ToString();
                ((ItemMetaData)itemDto)["IsActive"] = IsActive.ToString();
            }
        }

        #endregion

        #region Pipes Updating
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public delegate bool UpdatePipesCallback(ref string text);

        /// <summary>
        /// Updates all texts for the item that can contain pipes
        /// </summary>
        public abstract void UpdatePipes(UpdatePipesCallback callback);

        /// <summary>
        /// Resolves dependencies 
        /// </summary>
        public virtual void ResolveIdDependencies(Dictionary<int, int> itemIdMap) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="textID"></param>
        protected virtual void updatePipes(ItemData.UpdatePipesCallback callback, string textID)
        {
            Dictionary<string, string> texts = TextManager.GetAllTexts(textID);
            foreach (string language in texts.Keys)
            {
                string text = TextManager.GetText(textID, language);
                if (callback(ref text))
                {
                    TextManager.SetText(textID, language, text);
                }
            }
        }
        #endregion Pipes Updating
    }
}
