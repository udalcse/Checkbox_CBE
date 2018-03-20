using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Base class for items that send emails
    /// </summary>
    [Serializable]
    public class EmailItemData : LocalizableResponseItemData
    {
        /// <summary>
        /// Data table name.
        /// </summary>
        public override string ItemDataTableName { get { return "EmailItemData"; } }

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetEmail"; } }

        #region Properties

        /// <summary>
        /// Get/set whether to send the email only once
        /// </summary>
        public virtual bool SendOnce { get; set; }

        /// <summary>
        /// Get/set the from field
        /// </summary>
        public virtual string From { get; set; }

        /// <summary>
        /// Format of the message.
        /// </summary>
        public virtual string MessageFormat { get; set; }

        /// <summary>
        /// Gets or sets the Style Template ID associated with this email
        /// </summary>
        public virtual int? StyleTemplateID { get; set; }

        /// <summary>
        /// To address for the email.  This field may contain survey tokens and semi-colon separated email addresses.
        /// </summary>
        public virtual string To { get; set; }

        /// <summary>
        /// Blind carbon-copy recipients address for the email.  This field may contain survey tokens and semi-colon separated email addresses.
        /// A separate email message will be created for each recipient so that they are unaware of each other.
        /// </summary>
        public virtual string BCC { get; set; }

        /// <summary>
        /// TextID for the subject of the email
        /// </summary>
        public virtual string SubjectTextID
        {
            get { return GetTextID("subject"); }
        }

        /// <summary>
        /// TextID for the body of the email
        /// </summary>
        public virtual string BodyTextID
        {
            get { return GetTextID("body"); }
        }

        #endregion

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new EmailItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new EmailItemTextDecorator(this, languageCode);
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertEmail");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MessageFormat", DbType.String, MessageFormat ?? "Html");
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("FromAddress", DbType.String, From ?? string.Empty);
            command.AddInParameter("ToAddress", DbType.String, To ?? string.Empty);
            command.AddInParameter("Bcc", DbType.String, BCC ?? string.Empty);
            command.AddInParameter("SubjectTextID", DbType.String, SubjectTextID);
            command.AddInParameter("BodyTextID", DbType.String, BodyTextID);
            command.AddInParameter("SendOnce", DbType.Boolean, SendOnce);

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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateEmail");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("MessageFormat", DbType.String, MessageFormat ?? "Html");
            command.AddInParameter("StyleTemplateID", DbType.Int32, StyleTemplateID);
            command.AddInParameter("FromAddress", DbType.String, From ?? string.Empty);
			command.AddInParameter("ToAddress", DbType.String, To ?? string.Empty);
            command.AddInParameter("Bcc", DbType.String, BCC);
            command.AddInParameter("SubjectTextID", DbType.String, SubjectTextID);
            command.AddInParameter("BodyTextID", DbType.String, BodyTextID);
            command.AddInParameter("SendOnce", DbType.Boolean, SendOnce);

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

            try
            {
                MessageFormat = DbUtility.GetValueFromDataRow(data, "MessageFormat", string.Empty);
                StyleTemplateID = DbUtility.GetValueFromDataRow<int?>(data, "StyleTemplateID", null);
                From = DbUtility.GetValueFromDataRow(data, "FromAddress", string.Empty);
                To = DbUtility.GetValueFromDataRow(data, "ToAddress", string.Empty);
                BCC = DbUtility.GetValueFromDataRow(data, "BCC", string.Empty);
                SendOnce = DbUtility.GetValueFromDataRow(data, "SendOnce", false);
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
        /// <returns></returns>
        protected override string[] GetTextIdSuffixes()
        {
            return new List<string>(base.GetTextIdSuffixes()) { "subject", "body" }.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteElementString("MessageFormat", MessageFormat);

            //Style template id not portable between installations, so should not be part of export
            //writer.WriteElementValue("StyleTemplateID", StyleTemplateID);  
			writer.WriteElementString("From", From);
			writer.WriteElementString("To", To);
			writer.WriteElementString("Bcc", BCC);
			writer.WriteElementString("SendOnce", SendOnce.ToString());
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            MessageFormat = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("MessageFormat")) ?? "Html";

            //Style template id not portable between installations, so should not be part of export
            StyleTemplateID = null;
            //StyleTemplateID = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("StyleTemplateID"));

            From = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("From"));
            To = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("To"));
            BCC = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Bcc"));
            SendOnce = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("SendOnce"));
        }

        /// <summary>
        /// 
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
                    textData.TextValues["NavText"] = Utilities.StripHtml(TextManager.GetText(SubjectTextID, textData.LanguageCode), 50);
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

            updatePipes(callback, SubjectTextID);
            updatePipes(callback, BodyTextID);

            string tmp = From;
            if (callback(ref tmp))
            {
                From = tmp;
            }

            tmp = To;
            if (callback(ref tmp))
            {
                To = tmp;
            }

            tmp = BCC;
            if (callback(ref tmp))
            {
                BCC = tmp;
            }
        }

    }
}
