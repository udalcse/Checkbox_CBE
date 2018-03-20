using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Configuration for survey item to generate and send email messages
    /// </summary>
    [Serializable]
    public class EmailResponseItemData : EmailItemData
    {
        /// <summary>
        /// Get/set whether to include response details or not
        /// </summary>
        public bool IncludeResponseDetails { get; set; }

        /// <summary>
        /// Get/set whether to show page numbers or not
        /// </summary>
        public bool ShowPageNumbers { get; set; }

        /// <summary>
        /// Get/set whether to show question numbers or not
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
        /// Get name of table containing email response item configuration
        /// </summary>
        public override string ItemDataTableName { get { return "EmailResponseItemData"; } }

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetEmailResponse"; } }

        /// <summary>
        /// Create an instance of an email response item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new EmailResponseItem();
        }


        /// <summary>
        /// Create an instance of a text decorator for the item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new EmailResponseItemTextDecorator(this, languageCode);
        }

        /// <summary>
        /// Create an instance of the item in the database
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertEmailResponse");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MessageFormat", DbType.String, MessageFormat ?? "Html");
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("FromAddress", DbType.String, From ?? string.Empty);
            command.AddInParameter("ToAddress", DbType.String, To ?? string.Empty);
            command.AddInParameter("Bcc", DbType.String, BCC ?? string.Empty);
            command.AddInParameter("SubjectTextID", DbType.String, SubjectTextID);
            command.AddInParameter("BodyTextID", DbType.String, BodyTextID);
            command.AddInParameter("SendOnce", DbType.Boolean, SendOnce);
            command.AddInParameter("IncludeResponseDetails", DbType.Boolean, IncludeResponseDetails);
            command.AddInParameter("ShowPageNumbers", DbType.Boolean, ShowPageNumbers);
            command.AddInParameter("ShowQuestionNumbers", DbType.Boolean, ShowQuestionNumbers);
            command.AddInParameter("IncludeMessageItems", DbType.Boolean, IncludeMessageItems);
            command.AddInParameter("ShowHiddenItems", DbType.Boolean, ShowHiddenItems);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an instance of the item in the database
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateEmailResponse");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MessageFormat", DbType.String, MessageFormat ?? "Html");
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("FromAddress", DbType.String, From ?? string.Empty);
            command.AddInParameter("ToAddress", DbType.String, To ?? string.Empty);
            command.AddInParameter("Bcc", DbType.String, BCC);
            command.AddInParameter("SubjectTextID", DbType.String, SubjectTextID);
            command.AddInParameter("BodyTextID", DbType.String, BodyTextID);
            command.AddInParameter("SendOnce", DbType.Boolean, SendOnce);
            command.AddInParameter("IncludeResponseDetails", DbType.Boolean, IncludeResponseDetails);
            command.AddInParameter("ShowPageNumbers", DbType.Boolean, ShowPageNumbers);
            command.AddInParameter("ShowQuestionNumbers", DbType.Boolean, ShowQuestionNumbers);
            command.AddInParameter("IncludeMessageItems", DbType.Boolean, IncludeMessageItems);
            command.AddInParameter("ShowHiddenItems", DbType.Boolean, ShowHiddenItems);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Load the item from the specified data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            if (data == null)
            {
                throw new Exception("Data Row cannot be null.");
            }

            base.LoadBaseObjectData(data);

            try
            {
                IncludeResponseDetails = DbUtility.GetValueFromDataRow(data, "IncludeResponseDetails", false);
                ShowPageNumbers = DbUtility.GetValueFromDataRow(data, "ShowPageNumbers", false);
                ShowQuestionNumbers = DbUtility.GetValueFromDataRow(data, "ShowQuestionNumbers", false);
                IncludeMessageItems = DbUtility.GetValueFromDataRow(data, "IncludeMessageItems", false);
                ShowHiddenItems = DbUtility.GetValueFromDataRow(data, "ShowHiddenItems", false);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
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

            IncludeResponseDetails = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IncludeResponseDetails"));
            ShowPageNumbers = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowPageNumbers"));
            ShowQuestionNumbers = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowQuestionNumbers"));
            IncludeMessageItems = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IncludeMessageItems"));
            ShowHiddenItems = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("ShowHiddenItems"));
        }

    }
}
