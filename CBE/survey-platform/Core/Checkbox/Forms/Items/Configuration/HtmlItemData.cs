using System;
using System.Data;
using System.Xml;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

using Checkbox.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for HTML messages
    /// </summary>
    [Serializable]
    public class HtmlItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Get data table name for html item data
        /// </summary>
        public override string ItemDataTableName { get { return "HtmlItemData"; } }

		/// <summary>
		/// Get data table name for html item data
		/// </summary>
		public override string DataTableName { get { return "HtmlItemData"; } }

        /// <summary>
        /// Get load sproc for item
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetHtml"; } }

        /// <summary>
        /// Create an instance of the item.
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Create()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertHtml");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Html", DbType.String, HTML);
            command.AddInParameter("InlineCss", DbType.String, InlineCSS);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an existing item.
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified for Update()");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateHtml");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Html", DbType.String, HTML);
            command.AddInParameter("InlineCss", DbType.String, InlineCSS);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteStartElement("Html");
            writer.WriteCData(HTML);

			writer.WriteEndElement();

			writer.WriteStartElement("Css");
            writer.WriteCData(InlineCSS);
	
			writer.WriteEndElement();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            HTML = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Html"));
            InlineCSS = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Css"));
		}


        /// <summary>
        /// For this item, data can also include token data, which is stored separately
        /// </summary>
        /// <param name="itemDataRow"></param>
        protected override void LoadBaseObjectData(DataRow itemDataRow)
        {
            base.LoadBaseObjectData(itemDataRow);

            HTML = DbUtility.GetValueFromDataRow(itemDataRow, "Html", string.Empty);
            InlineCSS = DbUtility.GetValueFromDataRow(itemDataRow, "InlineCss", string.Empty);
        }

        /// <summary>
        /// Get/set the HTML for the item
        /// </summary>
        public string HTML { get; set; }

        /// <summary>
        /// Get/set the CSS for the item
        /// </summary>
        public string InlineCSS { get; set; }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new HtmlItem();
        }

        /// <summary>
        /// Create a text decorator for the HTML item.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new HtmlItemTextDecorator(this, languageCode);
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
                    textData.TextValues["Text"] = HTML;

                    textData.TextValues["StrippedText"] = Utilities.StripHtml(textData.TextValues["Text"], null);

                    textData.TextValues["NavText"] = Utilities.TruncateText(textData.TextValues["StrippedText"], 50);
                }
            }
        }

        /// <summary>
        /// Updates pipes
        /// </summary>
        /// <param name="callback"></param>
        public override void UpdatePipes(ItemData.UpdatePipesCallback callback)
        {
            base.UpdatePipes(callback);

            string tmp = HTML;
            if (callback(ref tmp))
            {
                HTML = tmp;
            }

            tmp = InlineCSS;
            if (callback(ref tmp))
            {
                InlineCSS = tmp;
            }
        }

    }
}
