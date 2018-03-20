using System;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class JavascriptItemData : ResponseItemData
    {
        /// <summary>
        /// Get name of load data stored procedure
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetJavascriptItem"; } }

        /// <summary>
        /// Get name of an item's data table when loading data from a data set.
        /// </summary>
        public override string ItemDataTableName { get { return "JavascriptItemData"; } }

        /// <summary>
        /// Create an instance of an item to be initialized with its configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new JavascriptItem();
        }

        /// <summary>
        /// Get/Set script text
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            Script = DbUtility.GetValueFromDataRow(data, "Script", string.Empty);
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertJavascript");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Script", DbType.String, Script);

            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Update an instance of a RankOrder configuration in the persistent store
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateJavascript");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Script", DbType.String, Script);

            db.ExecuteNonQuery(command, t);
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
                    textData.TextValues["Text"] = "Javascript Item";

                    textData.TextValues["StrippedText"] = Utilities.StripHtml(textData.TextValues["Text"], null);

                    textData.TextValues["NavText"] = Utilities.TruncateText(textData.TextValues["StrippedText"], 50);
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

            writer.WriteElementString("Script", Script);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            Script = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Script"));

            Save();
        }

    }
}
