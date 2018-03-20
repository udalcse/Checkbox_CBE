//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Security;
using Checkbox.Users;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration of <see cref="SingleLineTextItemData"/> items.
    /// </summary>
    [Serializable]
    public class SingleLineTextItemData : TextItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "SingleLineTextItemData"; } }

        /// <summary>
        /// Get load item sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetSLText"; } }

        /// <summary>
        /// Get/set the minimum value for data in the text box when it contains numeric data.
        /// </summary>
        public double? MinValue { get; set; }

        /// <summary>
        /// Get/set the maximum value for data in the text box when it contains numeric data.
        /// </summary>
        public double? MaxValue { get; set; }

        /// <summary>
        /// Autocomplete list Id
        /// </summary>
        public int? AutocompleteListId { get; set; }

        /// <summary>
        /// Autocomplete remote source url
        /// </summary>
        public string AutocompleteRemote { get; set; }

        /// <summary>
        /// Gets or sets the bindined property identifier.
        /// </summary>
        /// <value>
        /// The bindined property identifier.
        /// </value>
        public int? BindinedPropertyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the binded property.
        /// </summary>
        /// <value>
        /// The name of the binded property.
        /// </value>
        public string BindedPropertyName { get; set; }

        /// <summary>
        /// Create an instance of single line text item configuration in the persistent data store.
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertSLText");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("TextFormat", DbType.Int32, (int)Format);
            command.AddInParameter("MaxLength", DbType.Int32, MaxLength);
            command.AddInParameter("MaxValue", DbType.Double, MaxValue);
            command.AddInParameter("MinValue", DbType.Double, MinValue);
            command.AddInParameter("CustomFormatId", DbType.String, CustomFormatId);
            command.AddInParameter("AutocompleteListId", DbType.Int32, AutocompleteListId);
            command.AddInParameter("AutocompleteRemote", DbType.String, AutocompleteRemote);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an existing single line text item configuration in the database.
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateSLText");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("TextFormat", DbType.Int32, (int)Format);
            command.AddInParameter("MaxLength", DbType.Int32, MaxLength);
            command.AddInParameter("MaxValue", DbType.Double, MaxValue);
            command.AddInParameter("MinValue", DbType.Double, MinValue);
            command.AddInParameter("CustomFormatId", DbType.String, CustomFormatId);
            command.AddInParameter("AutocompleteListId", DbType.Int32, AutocompleteListId);
            command.AddInParameter("AutocompleteRemote", DbType.String, AutocompleteRemote);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Load the configuration data for this item from the specified <see cref="DataSet"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", 0) == 1;
            Format = (AnswerFormat)Convert.ToInt32(DbUtility.GetValueFromDataRow(data, "TextFormat", 0));

            CustomFormatId = string.Empty; // the ckbx_sp_ItemData_GetSLText doesn't have the column "CustomFormatId" DbUtility.GetValueFromDataRow(data, "CustomFormatId", string.Empty);


            MinValue = DbUtility.GetValueFromDataRow<double?>(data, "MinValue", null);
            MaxValue = DbUtility.GetValueFromDataRow<double?>(data, "MaxValue", null);

            MaxLength = DbUtility.GetValueFromDataRow<int?>(data, "MaxLength", null);

            AutocompleteListId = DbUtility.GetValueFromDataRow<int?>(data, "AutocompleteListId", null);
            AutocompleteRemote = DbUtility.GetValueFromDataRow<string>(data, "AutocompleteRemote", null);

            BindinedPropertyId = DbUtility.GetValueFromDataRow<int?>(data, "BindedPropertyId", null);
        }

        /// <summary>
        /// Create an instance of a single-line text item based on this data
        /// </summary>
        /// <returns>the Item</returns>
        protected override Item CreateItem()
        {
            return new SingleLineTextBoxItem();
        }

        /// <summary>
        /// Create a text item text decorator
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new TextItemDecorator(this, languageCode);
        }

        /// <summary>
        /// Copy the item data
        /// </summary>
        /// <returns></returns>
        protected override ItemData Copy()
        {
            var theCopy = (SingleLineTextItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.MaxValue = MaxValue;
                theCopy.MinValue = MinValue;

                theCopy.AutocompleteListId = AutocompleteListId;
                theCopy.AutocompleteRemote = AutocompleteRemote;
            }

            return theCopy;
        }

        /// <summary>
        /// Get minimum value as a datetime
        /// </summary>
        /// <returns></returns>
        public DateTime? GetMinValueAsDateTime()
        {
            return MinValue.HasValue
                ? (MinValue == 0 ? null : (DateTime?)(new DateTime((long)MinValue))) 
                : null;

        }

        /// <summary>
        /// Get maximum value as a date time
        /// </summary>
        /// <returns></returns>
        public DateTime? GetMaxValueAsDateTime()
        {
             return MaxValue.HasValue
                ? (MaxValue == 0 ? null : (DateTime?)(new DateTime((long)MaxValue, DateTimeKind.Utc))) 
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Format"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = AnswerFormat.None.ToString();
            }

            Format =
                (AnswerFormat)
                Enum.Parse(typeof(AnswerFormat), enumString);

			CustomFormatId = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("CustomFormatId"));
			IsRequired = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsRequired"));
			MaxLength = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxLength"));

            if (Format == AnswerFormat.Date || Format == AnswerFormat.Date_ROTW || Format == AnswerFormat.Date_USA)
            {
                MaxValue = XmlUtility.GetNodeDouble(xmlNode.SelectSingleNode("MaxValue"));
                MinValue =XmlUtility.GetNodeDouble(xmlNode.SelectSingleNode("MinValue"));
            }
            else
            {
                MaxValue = XmlUtility.GetNodeDouble(xmlNode.SelectSingleNode("MaxValue"));
                MinValue = XmlUtility.GetNodeDouble(xmlNode.SelectSingleNode("MinValue"));
            }

       //     AutocompleteListId = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("AutocompleteListId"));
            AutocompleteRemote = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AutocompleteRemote"));

            BindedPropertyName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("BindedPropertyName"));

            // bind singleline field if it has binding id value 
            if (!string.IsNullOrWhiteSpace(BindedPropertyName) && ID.HasValue)
            {
                var property = ProfileManager.GetPropertiesList().FirstOrDefault(item => item.Name.Equals(BindedPropertyName));
                if (property != null)
                {
                    PropertyBindingManager.AddItemMapping(ID.Value, property.FieldId, CustomFieldType.SingleLine);
                }
            }
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("Format", Format.ToString());
			writer.WriteElementString("CustomFormatId", CustomFormatId);
			writer.WriteElementString("IsRequired", IsRequired.ToString());
			writer.WriteElementValue("MaxLength", MaxLength);
			writer.WriteElementValue("MaxValue", MaxValue);
			writer.WriteElementValue("MinValue", MinValue);
         //   writer.WriteElementValue("AutocompleteListId", AutocompleteListId);
            writer.WriteElementString("AutocompleteRemote", AutocompleteRemote);

            if (BindinedPropertyId.HasValue && this.ID.HasValue)
            {
                var bindedFieldName = ProfileManager.GetConnectedProfileFieldName(this.ID.Value);
                writer.WriteElementString("BindedPropertyName", bindedFieldName);
            }
            

        }
    }
}
