//===============================================================================
// Checkbox Application Source Code
// Copyright © Prezza Technologies, Inc.  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Common;
using Checkbox.Forms.Piping.PipeHandlers;
using Checkbox.Users;
using Prezza.Framework.Data;

using Checkbox.Panels;
using Checkbox.Management;

namespace Checkbox.Invitations
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class Recipient
    {
        private string _emailToAddress;
        private bool _success;
        private readonly Panelist _panelist;
        private string _uniqueIdentifier;

        private Dictionary<string, string> _propertyCache;

        /// <summary>
        /// Constructor
        /// </summary>
        public Recipient()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panelist"></param>
        public Recipient(Panelist panelist)
        {
            _panelist = panelist;
        }

        /// <summary>
        /// Initialize a recipient with properties from a data reader
        /// </summary>
        /// <param name="recipientDataReader"></param>
        public Recipient(IDataReader recipientDataReader)
        {
            LoadFromDataRow(recipientDataReader);
        }

        /// <summary>
        /// Load recipient data from the data reader
        /// </summary>
        /// <param name="recipientDataReader"></param>
        protected virtual void LoadFromDataRow(IDataReader recipientDataReader)
        {
            ID = DbUtility.GetValueFromDataReader(recipientDataReader, "RecipientId", (long)0);
            InvitationID = DbUtility.GetValueFromDataReader(recipientDataReader, "InvitationId", 0);
            PanelID = DbUtility.GetValueFromDataReader(recipientDataReader, "PanelId", 0);
            EmailToAddress = DbUtility.GetValueFromDataReader(recipientDataReader, "EmailAddress", string.Empty);
            UniqueIdentifier = DbUtility.GetValueFromDataReader(recipientDataReader, "UniqueIdentifier", string.Empty);
            GUID = DbUtility.GetValueFromDataReader(recipientDataReader, "Guid", Guid.Empty);
            HasResponded = DbUtility.GetValueFromDataReader(recipientDataReader, "HasResponded", false);
            NumberOfMessagesSent = DbUtility.GetValueFromDataReader(recipientDataReader, "TotalSent", 0);
            SuccessfullySent = DbUtility.GetValueFromDataReader(recipientDataReader, "SuccessfullySent", false);
            LastSent = DbUtility.GetValueFromDataReader<DateTime?>(recipientDataReader, "LastSent", null);
            Error = DbUtility.GetValueFromDataReader(recipientDataReader, "ErrorMessage", string.Empty);
            Deleted = DbUtility.GetValueFromDataReader(recipientDataReader, "Deleted", false);
            OptedOut = DbUtility.GetValueFromDataReader(recipientDataReader, "OptOut", false);
            BatchMessageId = DbUtility.GetValueFromDataReader<long?>(recipientDataReader, "LastBatchMessageId", null);
            PanelTypeId = DbUtility.GetValueFromDataReader(recipientDataReader, "PanelTypeId", 0);

            //cast opt out type to enum
            int optOutType = DbUtility.GetValueFromDataReader(recipientDataReader, "OptOutType", -1);
            if (optOutType > -1)
                OptedOutType = (InvitationOptOutType) optOutType;

            if (PanelTypeId > 0)
                PanelTypeName = PanelManager.GetPanelTypeNameFromId(PanelTypeId);
        }

        /// <summary>
        /// Get property value cache for recipients without associated panelists 
        /// </summary>
        protected Dictionary<string, string> PropertyCache
        {
            get { return _propertyCache ?? (_propertyCache = new Dictionary<string, string>()); }
        }

        /// <summary>
        /// Get profile property for invitation recipient. Unless override, assumes
        /// identifier refers to a user unique identifier.
        /// </summary>
        /// <param name="recipientIdentifier"></param>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        protected virtual string GetRecipientProfileProperty(string recipientIdentifier, string propertyKey)
        {
            if (PropertyCache.ContainsKey(propertyKey))
            {
                return PropertyCache[propertyKey];
            }

            string propStringValue = ProfilePipeHandler.GetPipeValue(
                UserManager.GetUserPrincipal(recipientIdentifier),
                propertyKey);

            //Cache value, including empty strings to minimize redundant lookups for properties
            // with no values.
            PropertyCache[propertyKey] = propStringValue;

            return propStringValue;
        }

        /// <summary>
        /// Accessor for getting panelist information
        /// </summary>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (_panelist != null)
                {
                    return _panelist.GetProperty(key);
                }

                if (Utilities.IsNotNullOrEmpty(UniqueIdentifier))
                {
                    return GetRecipientProfileProperty(UniqueIdentifier, key);
                }

                //fix for email/email-list recipients
                if (key.ToLower() == "email" && !string.IsNullOrEmpty(EmailToAddress))
                    return EmailToAddress;

                return null;
            }
        }


        /// <summary>
        /// Panelist
        /// </summary>
        protected Panelist Panelist
        {
            get { return _panelist; }
        }

        public int? GroupID { set; get; }

        /// <summary>
        /// Unique identifier of user (if any) associated with recipient
        /// </summary>
        public string UniqueIdentifier
        {
            get
            {
                if (Utilities.IsNotNullOrEmpty(_uniqueIdentifier))
                {
                    return _uniqueIdentifier;
                }

                if (Panelist != null)
                {
                    return Panelist.GetProperty("UniqueIdentifier");
                }

                return string.Empty;
            }
            set
            {
                _uniqueIdentifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier 
        /// </summary>
        public long? ID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Invitation
        /// </summary>
        public int InvitationID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Panel
        /// </summary>
        public int PanelID { get; set; }

        /// <summary>
        /// Gets or sets a unique Guid
        /// </summary>
        public Guid GUID { get; set; }

        /// <summary>
        /// Gets or sets a unique Guid
        /// </summary>
        public InvitationOptOutType? OptedOutType { get; set; }

        /// <summary>
        /// Gets or sets an email address
        /// </summary>
        public string EmailToAddress
        {
            get
            {
                if (Utilities.IsNullOrEmpty(_emailToAddress)
                    && _panelist != null)
                {
                    _emailToAddress = _panelist.Email;
                }

                return _emailToAddress;
            }

            set
            {
                _emailToAddress = value;
            }
        }

        /// <summary>
        /// All emails for one recipient
        /// </summary>
        public List<string> EmailList { get; set; }

        /// <summary>
        /// Gets or sets whether this Recipient has responded to an email invitation
        /// </summary>
        public bool HasResponded { get; set; }

        /// <summary>
        /// Gets or sets the date this Recipient was last sent an invitation
        /// </summary>
        public DateTime? LastSent { get; set; }

        /// <summary>
        /// Get/set number of messages sent to recipient
        /// </summary>
        public int NumberOfMessagesSent { get; set; }

        /// <summary>
        /// Get/set id of last batch message sent for this recipient.
        /// </summary>
        public long? BatchMessageId { get; set; }

        /// <summary>
        /// Get/set id of batch which is processing this recipient.
        /// </summary>
        public long? ProcessingBatchId { get; set; }

        /// <summary>
        /// Type of panel recipient is associated with
        /// </summary>
        public int PanelTypeId { get; set; }

        /// <summary>
        /// Type name of panel recipient is associated with
        /// </summary>
        public string PanelTypeName { get; set; }

        /// <summary>
        /// Get/set whether recipient has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Get/set whether recipient opted out
        /// </summary>
        public bool OptedOut { get; set; }

        /// <summary>
        /// Get/set whether email have been bounced
        /// </summary>
        public bool Bounced { get; set; }

        /// <summary>
        /// Internal flag used by invitation to mark recipients as modified.
        /// </summary>
        public bool Modified { get; set; }

        /// <summary>
        /// Gets or sets whether the last Invitation was successfully sent to this Recipient
        /// </summary>
        public bool SuccessfullySent
        {
            get { return _success; }
            set
            {
                _success = value;

                //Clear the error on success
                if (_success)
                {
                    Error = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets any error message associated with a failed send
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Persists a record of this Recipient
        /// </summary>
        public void Save()
        {
            if (ID == null)
                Create();
            else
                Update();
        }

        /// <summary>
        /// Creates a record in the ckbx_InvitationRecipients table for the Recipient
        /// </summary>
        protected virtual void Create()
        {
            InsertRecipient();
        }

        /// <summary>
        /// Updates the database record for an existing Recipient
        /// </summary>
        protected virtual void Update()
        {
            RecordSend();
        }

        /// <summary>
        /// Add an invitation recipient to the database.
        /// </summary>
        protected virtual void InsertRecipient()
        {
            if (GUID == default(Guid))
                GUID = Guid.NewGuid();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_InsertRecipient");
            command.AddInParameter("InvitationID", DbType.Int32, InvitationID);
            command.AddInParameter("PanelID", DbType.Int32, PanelID);
            command.AddInParameter("EmailToAddress", DbType.String, EmailToAddress);
            command.AddInParameter("UniqueIdentifier", DbType.String, UniqueIdentifier);
            command.AddInParameter("GUID", DbType.Guid, GUID);
            command.AddInParameter("LastSent", DbType.DateTime, LastSent);
            command.AddInParameter("Success", DbType.Boolean, SuccessfullySent);
            command.AddInParameter("OptOut", DbType.Boolean, OptedOut);
            command.AddInParameter("Error", DbType.String, Error);
            command.AddInParameter("LastBatchMessageId", DbType.Int64, BatchMessageId);
            command.AddInParameter("ProcessingBatchId", DbType.Int64, ProcessingBatchId);
            command.AddOutParameter("RecipientID", DbType.Int64, 8);

            db.ExecuteNonQuery(command);

            object idValue = command.GetParameterValue("RecipientID");

            if (idValue != DBNull.Value)
            {
                ID = Convert.ToInt32(idValue);
            }
            else
            {
                throw new Exception("Unable to insert recipient into database.");
            }
        }

        private void RecordSend()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Send2Recipient");
            command.AddInParameter("RecipientID", DbType.Int64, ID);
            command.AddInParameter("InvitationId", DbType.Int32, InvitationID);
            command.AddInParameter("LastSent", DbType.DateTime, LastSent);
            command.AddInParameter("Success", DbType.Boolean, SuccessfullySent);
            command.AddInParameter("OptOut", DbType.Boolean, OptedOut);
            command.AddInParameter("Error", DbType.String, Error);
            command.AddInParameter("LastBatchMessageId", DbType.Int64, BatchMessageId);
            command.AddInParameter("ProcessingBatchId", DbType.Int64, ProcessingBatchId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Personalizes the text content for an <see cref="InvitationTemplate"/>, parsing any tokens
        /// </summary>
        /// <param name="template">the InvitationTemplate to personalize</param>
        /// <param name="invitation">Invitation</param>
        /// <param name="customUserFieldNames"></param>
        /// <param name="baseSurveyUrl"></param>
        public virtual void PersonalizeTemplate(Invitation invitation, InvitationTemplate template, List<string> customUserFieldNames, string baseSurveyUrl, Guid? surveyGuid)
        {
            //Create the recipient so the recipient ID is available foro piping
            if (ID == null || this.EmailList.Count > 1)
            {
                Create();
            }

            InvitationPipeMediator mediator = GetPipeMeditor();

            if (mediator != null)
            {
                mediator.Initialize(invitation, this, customUserFieldNames, baseSurveyUrl, surveyGuid);
                mediator.RegisterText("subject", template.Subject);
                mediator.RegisterText("remindersubject", template.ReminderSubject);

                if (((template.IncludeOptOutLink && !ApplicationManager.AppSettings.EnableMultiDatabase) ||
                    ApplicationManager.AppSettings.EnableMultiDatabase) && !ApplicationManager.AppSettings.FooterEnabled)
                {
                    if (template.Format == Messaging.Email.MailFormat.Html)
                    {
                        mediator.RegisterText("body", template.Body + "<br /><br />" + template.OptOutText);
                        mediator.RegisterText("reminderbody", template.ReminderBody + "<br /><br />" + template.OptOutText);
                    }
                    else
                    {
                        mediator.RegisterText("body", template.Body + Environment.NewLine + Environment.NewLine + template.OptOutText);
                        mediator.RegisterText("reminderbody", template.ReminderBody + Environment.NewLine + Environment.NewLine + template.OptOutText);
                    }
                }
                else
                {
                    mediator.RegisterText("body", template.Body);
                    mediator.RegisterText("reminderbody", template.ReminderBody);
                }

                mediator.RegisterText("subject", template.Subject);

                template.Body = mediator.GetText("body");
                template.Subject = mediator.GetText("subject");
                template.ReminderSubject = mediator.GetText("remindersubject");
                template.ReminderBody = mediator.GetText("reminderbody");
            }
        }

        /// <summary>
        /// Compute a hash key for internal use by invitation code to compare pending recipients with sent (persisted)
        /// recipients.
        /// </summary>
        /// <returns></returns>
        public string ComputeHashKey()
        {
            string hashCode = PanelID.ToString();

            if (Utilities.IsNotNullOrEmpty(UniqueIdentifier))
            {
                hashCode += "__" + UniqueIdentifier;
            }
            else
            {
                hashCode += "__" + EmailToAddress;
            }

            return hashCode;
        }

        /// <summary>
        /// Get a pipe meditor
        /// </summary>
        /// <returns></returns>
        protected virtual InvitationPipeMediator GetPipeMeditor()
        {
            return new InvitationPipeMediator();
        }
    }
}