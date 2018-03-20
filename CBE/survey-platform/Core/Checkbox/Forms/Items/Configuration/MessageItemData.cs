//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information for message items.
    /// </summary>
    [Serializable]
    public class MessageItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Get message item data table name
        /// </summary>
        public override string ItemDataTableName { get { return "MessageItemData"; } }

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetMessage"; } }

        public override string ObjectTypeName { get { return "MessageItemData"; } }
        /// <summary>
        /// Get/set the textId for the message contained in this item.
        /// </summary>
        public virtual string TextID
        {
            get { return GetTextID("text"); }
        }


        /// <summary>
        /// Get/set the textId for the message contained in this item.
        /// </summary>
        /// <value>
        /// <c>true</c> if [reportable section break]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ReportableSectionBreak { get; set; }


        /// <summary>
        /// Create an instance of configuration data for a message item in the data store.
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Create()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMessage");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("ReportableSectionBreak", DbType.String, ReportableSectionBreak);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an existing instance of message item configuration in the data store.
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Update()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMessage");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextId", DbType.String, TextID);
            command.AddInParameter("ReportableSectionBreak", DbType.Boolean, ReportableSectionBreak);

            db.ExecuteNonQuery(command, t);
        }


        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            PopulateMessageFields(data.Tables[ParentDataTableName]);
        }


        private void PopulateMessageFields(DataTable table)
        {
            if (table != null)
            {
                foreach (DataRow dataRow in table.Rows)
                {
                    this.ReportableSectionBreak = DbUtility.GetValueFromDataRow(dataRow, "ReportableSectionBreak", default(bool));
                }
            }
        }


        /// <summary>
        /// Create an instance of a data item based on this configuration information.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new Message();
        }

        /// <summary>
        /// Create a text decorator for the message item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new MessageItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {"text"}.ToArray();
        }

        /// <summary>
        /// Add text data to metadata object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ItemMetaData)
            {
                var textDataList = ((ItemMetaData)itemDto).TextData;

                foreach (var textData in textDataList)
                {
                    textData.TextValues["Text"] = Utilities.StripHtmlAndEncode(TextManager.GetText(TextID, textData.LanguageCode));

                    textData.TextValues["StrippedText"] = Utilities.CustomDecode(Utilities.StripHtml(textData.TextValues["Text"], null)) ;

                    textData.TextValues["NavText"] = Utilities.TruncateText(textData.TextValues["StrippedText"], 50);
                }
            }
        }

        /// <summary>
        /// Represents custom fields that are included in setialziation process
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="externalDataCallback"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("ReportableSectionBreak", this.ReportableSectionBreak.ToString());
        
        }

        /// <summary>
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"></param>
        /// <param name="creator"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);
          
            this.ReportableSectionBreak = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ReportableSectionBreak"));
        }

    }
}
