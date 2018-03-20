//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration of <see cref="AddressVerifierItemData"/> items.
    /// </summary>
    [Serializable]
    public class AddressVerifierItemData : TextItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "AddressVerifierItemData"; } }

        /// <summary>
        /// Get load item sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetAddress"; } }

        /// <summary>
        /// Get/set the region where address should be selected from
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Get/set the search type how address should be shown
        /// </summary>
        public string SearchType { get; set; }

        /// <summary>
        /// Get/set the search rule how address should be searched
        /// </summary>
        public string Rule { get; set; }

        /// <summary>
        /// Get/set the type of the address: rural, urban or both
        /// </summary>
        public string Rural { get; set; }
        
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertAddress");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("Region", DbType.String, Region);
            command.AddInParameter("SearchType", DbType.String, SearchType);
            command.AddInParameter("Rule", DbType.String, Rule);
            command.AddInParameter("Rural", DbType.String, Rural);

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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateAddress");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired ? 1 : 0);
            command.AddInParameter("DefaultTextID", DbType.String, DefaultTextID);
            command.AddInParameter("Region", DbType.String, Region);
            command.AddInParameter("SearchType", DbType.String, SearchType);
            command.AddInParameter("Rule", DbType.String, Rule);
            command.AddInParameter("Rural", DbType.String, Rural);

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

            Region = DbUtility.GetValueFromDataRow(data, "Region", string.Empty);
            SearchType = DbUtility.GetValueFromDataRow(data, "SearchType", string.Empty);
            Rule = DbUtility.GetValueFromDataRow(data, "Rule", string.Empty);
            Rural = DbUtility.GetValueFromDataRow(data, "Rural", string.Empty);
        }

        /// <summary>
        /// Create an instance of a single-line text item based on this data
        /// </summary>
        /// <returns>the Item</returns>
        protected override Item CreateItem()
        {
            return new AddressVerifierItem();
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
            var theCopy = (AddressVerifierItemData)base.Copy();

            if (theCopy != null)
            {
                theCopy.Region = Region;
            }

            return theCopy;
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
            
            Region =  XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Region"));
            SearchType =  XmlUtility.GetNodeText(xmlNode.SelectSingleNode("SearchType"));
            Rule =  XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Rule"));
            Rural =  XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Rural"));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("Region", Region);
            writer.WriteElementString("SearchType", SearchType);
            writer.WriteElementString("Rule", Rule);
            writer.WriteElementString("Rural", Rural);
		}
    }
}
