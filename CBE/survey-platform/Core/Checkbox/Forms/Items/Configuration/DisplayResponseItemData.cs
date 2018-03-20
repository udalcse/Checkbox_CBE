using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Item that displays the survey response
    /// </summary>
    [Serializable]
    public class DisplayResponseItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Default constructor to initialize DisplayInlineResponse to be true
        /// </summary>
        public DisplayResponseItemData()
        {
            DisplayInlineResponse = true;
            IncludeResponseDetails = true;
            ShowPageNumbers = true;
            IncludeMessageItems = false;
            ShowHiddenItems = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "DisplayResponseItemData"; } }

        /// <summary>
        /// Get "load" procedure name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetDR"; } }

        /// <summary>
        /// Show the response inline
        /// </summary>
        public bool DisplayInlineResponse { get; set; }

        /// <summary>
        /// Get/set whether to include response details or not
        /// </summary>
        public bool IncludeResponseDetails { get; set; }

        /// <summary>
        /// Get/set whether to show page numbers or not
        /// </summary>
        public bool ShowPageNumbers { get; set; }

        /// <summary>
        /// Get/set whether to show item numbers or not
        /// </summary>
        public bool ShowQuestionNumbers { get; set; }

        /// <summary>
        /// Get/set whether include message items or not
        /// </summary>
        public bool IncludeMessageItems { get; set; }

        /// <summary>
        /// Get/set whether show hidden items or not
        /// </summary>
        public bool ShowHiddenItems { get; set; }

        /// <summary>
        /// Get the text id for the view response link
        /// </summary>
        public string LinkTextID
        {
            get { return GetTextID("linkText"); }
        }

        /// <summary>
        /// Do nothing for now
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertDR");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("DisplayInline", DbType.Boolean, DisplayInlineResponse);
            command.AddInParameter("IncludeResponseDetails", DbType.Boolean, IncludeResponseDetails);
            command.AddInParameter("ShowPageNumbers", DbType.Boolean, ShowPageNumbers);
            command.AddInParameter("ShowQuestionNumbers", DbType.Boolean, ShowQuestionNumbers);
            command.AddInParameter("IncludeMessageItems", DbType.Boolean, IncludeMessageItems);
            command.AddInParameter("ShowHiddenItems", DbType.Boolean, ShowHiddenItems);

            db.ExecuteNonQuery(command, t);

        }

        /// <summary>
        /// Do nothing for now
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateDR");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("DisplayInline", DbType.Boolean, DisplayInlineResponse);
            command.AddInParameter("IncludeResponseDetails", DbType.Boolean, IncludeResponseDetails);
            command.AddInParameter("ShowPageNumbers", DbType.Boolean, ShowPageNumbers);
            command.AddInParameter("ShowQuestionNumbers", DbType.Boolean, ShowPageNumbers);
            command.AddInParameter("IncludeMessageItems", DbType.Boolean, IncludeMessageItems);
            command.AddInParameter("ShowHiddenItems", DbType.Boolean, ShowHiddenItems);

            db.ExecuteNonQuery(command, t);
        }

     
        /// <summary>
        /// Load the item from data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            DisplayInlineResponse = DbUtility.GetValueFromDataRow(data, "DisplayInline", false);
            IncludeResponseDetails = DbUtility.GetValueFromDataRow(data, "IncludeResponseDetails", false);
            ShowPageNumbers = DbUtility.GetValueFromDataRow(data, "ShowPageNumbers", false);
            ShowQuestionNumbers = DbUtility.GetValueFromDataRow(data, "ShowQuestionNumbers", false);
            IncludeMessageItems = DbUtility.GetValueFromDataRow(data, "IncludeMessageItems", false);
            ShowHiddenItems = DbUtility.GetValueFromDataRow(data, "ShowHiddenItems", false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) {"linkText"}.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("DisplayInline", DisplayInlineResponse.ToString());
            writer.WriteElementString("IncludeResponseDetails", IncludeResponseDetails.ToString());
            writer.WriteElementString("ShowPageNumbers", ShowPageNumbers.ToString());
            writer.WriteElementString("ShowQuestionNumbers", ShowQuestionNumbers.ToString());
            writer.WriteElementString("IncludeMessageItems", IncludeMessageItems.ToString());
            writer.WriteElementString("ShowHiddenItems", ShowHiddenItems.ToString());
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

			DisplayInlineResponse = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("DisplayInline"));
            IncludeResponseDetails = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IncludeResponseDetails"));
            ShowPageNumbers = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowPageNumbers"));
            ShowQuestionNumbers = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowQuestionNumbers"));
            IncludeMessageItems = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IncludeMessageItems"));
            ShowHiddenItems = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowHiddenItems"));
		}


        /// <summary>
        /// Create a text decoroator for the display response item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new DisplayResponseItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// Create an instance of a data item based on this configuration information.
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new DisplayResponseItem();
        }
    }
}
