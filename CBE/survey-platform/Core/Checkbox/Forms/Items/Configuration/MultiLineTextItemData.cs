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
    /// Container for multi-line text item data
    /// </summary>
    [Serializable]
    public class MultiLineTextItemData : TextItemData
    {
        /// <summary>
        /// Gets or sets the bindined property identifier.
        /// </summary>
        /// <value>
        /// The bindined property identifier.
        /// </value>
        public int? BindinedPropertyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the binded field.
        /// </summary>
        /// <value>
        /// The name of the binded field.
        /// </value>
        public string BindedFieldName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MultiLineTextItemData()
        {
            IsHtmlFormattedData = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "MultiLineTextItemData"; } }

        /// <summary>
        /// Get procedure used to load data
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetMLText"; } }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new MultiLineTextBoxItem();
        }

        /// <summary>
        /// Create an instance of multi line text item configuration in the persistent data store.
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMLText");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("IsHtmlFormattedData", DbType.Boolean, IsHtmlFormattedData);
            command.AddInParameter("MaxLength", DbType.Int32, MaxLength);
            command.AddInParameter("MinLength", DbType.Int32, MinLength);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an existing multi line text item configuration in the database.
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMLText");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("IsHtmlFormattedData", DbType.Boolean, IsHtmlFormattedData);
            command.AddInParameter("MaxLength", DbType.Int32, MaxLength);
            command.AddInParameter("MinLength", DbType.Int32, MinLength);

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
            IsHtmlFormattedData = DbUtility.GetValueFromDataRow(data, "IsHtmlFormattedData", false);
            MaxLength = DbUtility.GetValueFromDataRow(data, "MaxLength", default(int?));
            MinLength = DbUtility.GetValueFromDataRow(data, "MinLength", default(int?));

            BindinedPropertyId = DbUtility.GetValueFromDataRow<int?>(data, "BindedPropertyId", null);
        }

        /// <summary>
        /// Get the text decorator for this item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new TextItemDecorator(this, languageCode);
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
            IsHtmlFormattedData = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsHtmlFormattedData"));
            MaxLength = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MaxLength"));
            MinLength = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("MinLength"));

            BindedFieldName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("BindedPropertyName"));

            if (!string.IsNullOrWhiteSpace(BindedFieldName) && ID.HasValue)
            {
                var property = ProfileManager.GetPropertiesList().FirstOrDefault(item => item.Name.Equals(BindedFieldName));
                if (property != null)
                {
                    PropertyBindingManager.AddItemMapping(ID.Value, property.FieldId, CustomFieldType.MultiLine);
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
            writer.WriteElementString("IsHtmlFormattedData", IsHtmlFormattedData.ToString());
            writer.WriteElementValue("MaxLength", MaxLength);
            writer.WriteElementValue("MinLength", MinLength);

            if (BindinedPropertyId.HasValue && this.ID.HasValue)
            {
                var bindedFieldName = ProfileManager.GetConnectedProfileFieldName(this.ID.Value);

                writer.WriteElementString("BindedPropertyName", bindedFieldName);

            }

        }
    }
}
