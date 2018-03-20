//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Checkbox.Common;
using Checkbox.Configuration;
using Checkbox.Pagination;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Data;
using Checkbox.Panels;
using Checkbox.Messaging.Email;
using Checkbox.Users;
using Prezza.Framework.Security.Principal;
using Checkbox.Security;
using Checkbox.Management;
using Checkbox.Forms;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Represents an invitation to take a survey.
    /// </summary>
    [Serializable]
    public class Invitation
    {
        private Dictionary<Int32, Panel> _panels;
        private UserPanel _userPanel;
        private AdHocEmailListPanel _adHocPanel;
        private InvitationTemplate _template;

        private Dictionary<long, Recipient> _recipientDictionary;
        private int _nextPanelTempId;

        private List<int> _addedPanels;
        private List<int> _removedPanels;
        private List<InvitationSchedule> _schedule;
        private bool? _successfullySent;

        /// <summary>
        /// Overloaded. Creates a new instance of an Invitation
        /// </summary>
        public Invitation()
        {
            _nextPanelTempId = -1;
        }

        /// <summary>
        /// Overloaded.  Constructor.
        /// </summary>
        /// <param name="id"></param>
        public Invitation(int id)
            : this()
        {
            ID = id;
        }

        /// <summary>
        /// Get the user name of the creator
        /// </summary>
        public string CreatedBy { get; private set; }

        /// <summary>
        /// Get the created date
        /// </summary>
        public DateTime Created { get; private set; }

        /// <summary>
        /// Get the last sent date
        /// </summary>
        public DateTime? LastSent { get; private set; }

        /// <summary>
        /// Get the nearest date of the scheduled delivery
        /// </summary>
        public DateTime? InvitationScheduled 
        { 
            get
            {
                var dates = (from s in Schedule where s.InvitationActivityType == InvitationActivityType.Invitation select s.Scheduled);
                return dates.FirstOrDefault();
            }
        }

        /// <summary>
        /// Get the nearest date of the scheduled delivery
        /// </summary>
        public DateTime? ReminderScheduled
        {
            get
            {
                var dates = (from s in Schedule where s.InvitationActivityType == InvitationActivityType.Reminder & !s.ProcessingFinished.HasValue select s.Scheduled);
                return dates.FirstOrDefault();
            }
        }
       
        /// <summary>
        /// Count of invitations successfully sent
        /// </summary>
        public Int32 SentCount { get; private set; }

        /// <summary>
        /// Count of invitations that failed to be sent
        /// </summary>
        public Int32 FailedCount { get; private set; }

        /// <summary>
        /// Status of the last sent invitation
        /// </summary>
        public bool? SuccessfullySent
        {
            get
            {
                if (!EmailGateway.ProviderSupportsBatches)
                    return _successfullySent;

                return SuccessfullySentScheduled;
            }
        }

        /// <summary>
        /// Error message of the last sent invitation
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Count of surveys responses resulting from this invitation
        /// </summary>
        public Int32 ResponseCount { get; private set; }

        /// <summary>
        /// Unique name used to identify the invitation in the admin app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Company profile id
        /// </summary>
        public int? CompanyProfileId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CompanyProfile GetCompanyProfile()
        {
            return CompanyProfileId.HasValue ? new CompanyProfile(CompanyProfileId.Value) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<int> AddedPanels
        {
            get { return _addedPanels ?? (_addedPanels = new List<int>()); }
        }

        /// <summary>
        /// List of the schedule records, sorted by date of sending
        /// </summary>
        public List<InvitationSchedule> Schedule
        {
            get
            {
                if (_schedule == null)
                    LoadSchedule();

                return _schedule;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<int> RemovedPanels
        {
            get { return _removedPanels ?? (_removedPanels = new List<int>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool SuccessfullySentScheduled
        {
            get
            {
                if (Schedule == null)
                    return false;

                var invitation = Schedule.FirstOrDefault(s => s.InvitationActivityType == InvitationActivityType.Invitation);
                return invitation != null && invitation.ProcessingFinished.HasValue;
            }
        }

        /// <summary>
        /// Get a reference to the recipient dictionary
        /// </summary>
        protected Dictionary<long, Recipient> RecipientDictionary
        {
            get
            {
                if (_recipientDictionary == null)
                    LoadRecipientData(ref _recipientDictionary);

                return _recipientDictionary;
            }
        }

        /// <summary>
        /// Get number of recipients
        /// </summary>
        public int RecipientsCount
        {
            get
            {

                return (_recipientDictionary == null)?0: _recipientDictionary.Count;
            }
        }

        /// <summary>
        /// Get the panels
        /// </summary>
        public Dictionary<int, Panel> PanelDictionary
        {
            get
            {
                if (_panels == null)
                    LoadPanels();

                return _panels;
            }
        }

        /// <summary>
        /// Get the users panel for the invitation
        /// </summary>
        protected UserPanel UsersPanel
        {
            get
            {
                if (_userPanel == null)
                {
                    _userPanel = (UserPanel)PanelManager.CreatePanel("Checkbox.Panels.UserPanel");
                    _userPanel.Name = string.Format("Invitation[{0}] UserPanel", ID);
                    _userPanel.Description = "This Panel was autogenerated for this Invitation";
                    AddPanel(_userPanel);
                }

                return _userPanel;
            }

            set
            {
                _userPanel = value;

                if (_userPanel.ID.HasValue && !PanelDictionary.ContainsKey(_userPanel.ID.Value))
                {
                    AddPanel(_userPanel);
                }
            }
        }

        /// <summary>
        /// Get panel for ad hoc email panelits
        /// </summary>
        protected AdHocEmailListPanel AdHocEmailPanel
        {
            get
            {
                if (_adHocPanel == null)
                {
                    _adHocPanel = (AdHocEmailListPanel)PanelManager.CreatePanel("Checkbox.Panels.AdHocEmailListPanel");
                    _adHocPanel.Name = string.Format("Invitation[{0}] AdHocEmailPanel", ID);
                    _adHocPanel.Description = "This Panel was autogenerated for this Invitation";
                    AddPanel(_adHocPanel);
                }

                return _adHocPanel;
            }
            set
            {
                _adHocPanel = value;

                if (_adHocPanel.ID.HasValue && !PanelDictionary.ContainsKey(_adHocPanel.ID.Value))
                {
                    AddPanel(_adHocPanel);
                }
            }
        }

        /// <summary>
        /// Populate the recipient data table with information
        /// </summary>
        private void LoadRecipientData(ref Dictionary<long, Recipient> recipients, long? processingBatchId = null, int? batchSize = null)
        {
            recipients = new Dictionary<long, Recipient>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipients");
            command.AddInParameter("InvitationID", DbType.Int32, ID);
            command.AddInParameter("Filter", DbType.String, "All");
            command.AddInParameter("ProcessingBatchId", DbType.Int64, processingBatchId);
            command.AddInParameter("BatchSize", DbType.Int32, batchSize);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var recipient = new Recipient(reader);

                        if (recipient.ID.HasValue)
                        {
                            recipients[recipient.ID.Value] = recipient;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processingBatchId"> </param>
        /// <param name="recipientsToHandleCount"></param>
        public Dictionary<long, Recipient> LoadRecipientData(long? processingBatchId, int? recipientsToHandleCount)
        {
            if (recipientsToHandleCount.HasValue && processingBatchId.HasValue)
            {
                Dictionary<long, Recipient> recipientDictionary = new Dictionary<long, Recipient>();
                LoadRecipientData(ref recipientDictionary, processingBatchId, recipientsToHandleCount);
                return recipientDictionary;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the unique database id of the Invitation
        /// </summary>
        public int? ID { get; set; }

        /// <summary>
        /// Gets a Globally Unique Identifier for this Invitation
        /// </summary>
        public Guid GUID { get; private set; }

        /// <summary>
        /// Gets the ID of the Parent ResponseTemplate
        /// </summary>
        public Int32 ParentID { get; set; }

        /// <summary>
        /// Gets the <see cref="InvitationTemplate"/> for this Invitation
        /// </summary>
        public InvitationTemplate Template
        {
            get { return _template ?? (_template = new InvitationTemplate()); }
        }

        /// <summary>
        /// Overloaded. Adds a <see cref="Panel"/> to the Invitation's Panels collection
        /// </summary>
        /// <param name="panel">the Panel to add</param>
        public void AddPanel(Panel panel)
        {
            if (!panel.ID.HasValue)
            {
                panel.ID = _nextPanelTempId;
                _nextPanelTempId--;
            }

            //If this panel isn't already on the invitation, be
            // sure to mark it as added, so the invitation to panel 
            // mapping in the db gets updated on save.
            if (!PanelDictionary.ContainsKey(panel.ID.Value))
            {
                AddedPanels.Add(panel.ID.Value);
            }

            PanelDictionary[panel.ID.Value] = panel;


        }

        /// <summary>
        /// Overloaded. Adds a <see cref="GroupPanel"/> to the Invitation's Panels collection
        /// </summary>
        /// <param name="group">a <see cref="Group"/> from which the GroupPanel is constructed</param>
        public void AddPanel(Group group)
        {
            // Creates a GroupPanel w/ group
            var panel = (GroupPanel)PanelManager.CreatePanel("Checkbox.Panels.GroupPanel");
            panel.Name = group.Name;
            panel.Description = group.Description;
            panel.GroupId = group.ID;
            AddPanel(panel);
        }

        /// <summary>
        /// Overloaded. Adds a <see cref="UserPanel"/> to the Invitation's Panels collection or appends the 
        /// argument to the existing UserPanel
        /// </summary>
        /// <param name="identities">a list of IIdentities from which the UserPanel is constructed</param>
        public void AddPanel(List<IIdentity> identities)
        {
            UsersPanel.AddIdentities(identities);
        }

        /// <summary>
        /// Adds the panel.
        /// </summary>
        /// <param name="invitations">The invitations.</param>
        public void AddPanel(List<UserInvitation> invitations)
        {
            foreach (var invitation in invitations)
            {
                foreach (var email in invitation.Emails)
                    UsersPanel.AddRecipient(invitation.UserName, email);
            }
        }

        /// <summary>
        /// Overloaded. Adds an <see cref="AdHocEmailListPanel"/> to the Invitation's Panels collection or appends the 
        /// argument to the existing AdHocEmailListPanel
        /// </summary>
        /// <param name="addresses">a list of string addresses</param>
        public void AddPanel(List<string> addresses)
        {
            // if no AdHocEmailPanel exists, creates one
            // adds emails and other data to panel
            AdHocEmailPanel.AddEmailAddresses(addresses);
        }

        /// <summary>
        /// Removes a <see cref="Panel"/> from the Invitation's Panels collection
        /// </summary>
        /// <param name="panelId"></param>
        public void RemovePanel(int panelId)
        {
            if (!RemovedPanels.Contains(panelId))
            {
                RemovedPanels.Add(panelId);
            }

            if (PanelDictionary.ContainsKey(panelId))
            {
                PanelDictionary.Remove(panelId);
            }

            foreach (Recipient recipient in RecipientDictionary.Values)
            {
                if (recipient.PanelID == panelId)
                {
                    recipient.Deleted = true;
                    recipient.Modified = true;
                }
            }
        }

        /// <summary>
        /// Remove panelists from the invitation
        /// </summary>
        /// <param name="userPanelists"></param>
        public void RemoveUserPanelists(IEnumerable<string> userPanelists)
        {
            if (_userPanel != null)            
                _userPanel.RemoveIdentities(userPanelists);
        }

        /// <summary>
        /// Removes group panel from the Invitation's Panels collection
        /// </summary>
        /// <param name="groupId"></param>
        public void RemoveGroupPanel(int groupId)
        {
            var panel = PanelDictionary
                .FirstOrDefault(p => (p.Value is GroupPanel && ((GroupPanel) p.Value).GroupId == groupId));
            
            RemovePanel(panel.Key);
        }

        /// <summary>
        /// Removes group panel from the Invitation's Panels collection
        /// </summary>
        /// <param name="emailListId"> </param>
        public void RemoveEmailListPanel(int emailListId)
        {
            var panel = PanelDictionary
                .FirstOrDefault(p => (p.Value is EmailListPanel && p.Value.ID == emailListId));

            RemovePanel(panel.Key);
        }

        /// <summary>
        /// Remove panelists from the invitation
        /// </summary>
        /// <param name="emailPanelists"></param>
        public void RemoveEmailPanelists(IEnumerable<string> emailPanelists)
        {
            if (_adHocPanel != null)
                _adHocPanel.RemoveEmailAddresses(emailPanelists);
        }

        /// <summary>
        /// Provide a way to remove users from an invitation's recipients
        /// </summary>
        /// <param name="recipients"></param>
        public void RemoveRecipients(IEnumerable<long> recipients)
        {
            foreach (long recipientID in recipients)
            {
                if (RecipientDictionary.ContainsKey(recipientID))
                {
                    RecipientDictionary[recipientID].Deleted = true;
                    RecipientDictionary[recipientID].Modified = true;
                }
                else
                    throw new Exception(string.Format("recipient with ID={0} hasn't been found.", recipientID));
            }
        }

        /// <summary>
        /// Provide a way to remove users from an invitation's recipients
        /// </summary>
        /// <param name="recipients"></param>
        public void RemoveRecipients(IEnumerable<IIdentity> recipients)
        {
            if (_userPanel == null)
            {
                return;
            }

            foreach (IIdentity identity in recipients)
            {
                foreach (Recipient recipient in RecipientDictionary.Values)
                {
                    if (recipient.PanelID == UsersPanel.ID && identity.Name.Equals(recipient.UniqueIdentifier, StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipient.Deleted = true;
                        recipient.Modified = true;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the specified recipient
        /// </summary>
        /// <param name="recipientID"></param>
        public void RemoveRecipient(long recipientID)
        {
            if (RecipientDictionary.ContainsKey(recipientID))
            {
                RecipientDictionary[recipientID].Deleted = true;
                RecipientDictionary[recipientID].Modified = true;
            }
        }

        /// <summary>
        /// Opt a recipient out of invitations
        /// </summary>
        /// <param name="recipientID"></param>
        public void OptOutRecipient(long recipientID)
        {
            if (RecipientDictionary.ContainsKey(recipientID))
            {
                RecipientDictionary[recipientID].OptedOut = true;
                RecipientDictionary[recipientID].Modified = true;
            }
        }

        /// <summary>
        /// Provide a way to remove email addresses from an invitation's recipients
        /// </summary>
        /// <param name="emailRecipients"></param>
        public void RemoveRecipients(List<string> emailRecipients)
        {
            if (_adHocPanel == null)
            {
                return;
            }

            foreach (string emailAddress in emailRecipients)
            {
                foreach (Recipient recipient in RecipientDictionary.Values)
                {
                    if (recipient.PanelID == AdHocEmailPanel.ID
                        && emailAddress.Equals(recipient.EmailToAddress, StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipient.Deleted = true;
                        recipient.Modified = true;
                    }
                }
            }
        }

        /// <summary>
        /// Get invitation recipients
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="groupRecipientsByPanel"></param>
        /// <param name="markOptedOut"> </param>
        /// <param name="batchSize"> </param>
        /// <param name="preparedRecipientsData"> </param>
        /// <returns></returns>
        public List<Recipient> GetRecipients(RecipientFilter filter, bool groupRecipientsByPanel = true,
            bool markOptedOut = false, int? batchSize = null, Dictionary<long, Recipient> preparedRecipientsData = null)
        {
            return GetRecipients(filter, "EmailAddress", true, groupRecipientsByPanel, markOptedOut, batchSize, preparedRecipientsData);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InvitationRecipientSummary GetRecipientSummary()
        {
            InvitationRecipientSummary summary = new InvitationRecipientSummary();
            summary.PendingCount = GetPendingRecipientsCount();

            var bouncedEmails = GetBouncedEmails();

            foreach (Recipient recipient in RecipientDictionary.Values)
            {
                if (!recipient.Deleted)
                {
                    if (!recipient.OptedOut)
                    {
                        summary.CurrentCount++;

                        if (recipient.HasResponded)
                            summary.RespondedCount++;
                        else if (bouncedEmails.Contains(recipient.EmailToAddress.Trim().ToLower()))
                            summary.BouncedCount++;
                        else
                            summary.NotRespondedCount++;
                    }
                    else
                        summary.OptedOutCount++;
                }
            }

            return summary;
        }

        /// <summary>
        /// Gets a DataTable of Recipient data
        /// </summary>
        /// <returns></returns>
        public List<Recipient> GetRecipients(RecipientFilter filter, string sortField, bool sortAscending, bool groupRecipientsByPanel = true,
            bool markOptedOut = false, int? batchSize = null, Dictionary<long, Recipient> preparedRecipientsData = null)
        {
            List<Recipient> recipientList;

            //Now list recipients
            if (filter == RecipientFilter.Pending)
            {
                //Specially handle pending recipients, which have not yet been
                // persisted to recipients table
                recipientList = GetPendingRecipients(groupRecipientsByPanel, batchSize);
            }
            else if (filter == RecipientFilter.PendingUngrouped)
            {
                recipientList = GetPendingRecipients(false, batchSize);
            }
            else
            {
                recipientList = new List<Recipient>();

                string[] bouncedEmails = null;
                if (filter == RecipientFilter.Bounced || filter == RecipientFilter.NotResponded)
                    bouncedEmails = GetBouncedEmails();

                //if there is no data passed, 
                if (preparedRecipientsData == null)
                    preparedRecipientsData = RecipientDictionary;

                foreach (Recipient recipient in preparedRecipientsData.Values)
                {
                    //email already exists in the list - skip it
                    if (!groupRecipientsByPanel &&
                        recipientList.Any(r => r.EmailToAddress.Equals(recipient.EmailToAddress)))
                        continue;

                    bool includeRecipient = false;

                    //Not responded
                    if (filter == RecipientFilter.NotResponded)
                    {
                        includeRecipient = !recipient.Deleted && !recipient.HasResponded && !recipient.OptedOut && 
                            !(recipient.Bounced = bouncedEmails.Contains(recipient.EmailToAddress.Trim().ToLower()));
                    }

                    if (filter == RecipientFilter.Responded)
                    {
                        includeRecipient = !recipient.Deleted && !recipient.OptedOut && recipient.HasResponded;
                    }

                    if (filter == RecipientFilter.Current)
                    {
                        includeRecipient = !recipient.Deleted && !recipient.OptedOut;
                    }

                    if (filter == RecipientFilter.Deleted)
                    {
                        includeRecipient = recipient.Deleted;
                    }

                    if (filter == RecipientFilter.OptOut)
                    {
                        includeRecipient = recipient.OptedOut;
                    }

                    if (filter == RecipientFilter.All)
                    {
                        includeRecipient = true;
                    }

                    if (filter == RecipientFilter.Bounced)
                    {
                        includeRecipient = recipient.Bounced =  bouncedEmails.Contains(recipient.EmailToAddress.Trim().ToLower());
                    }
                    
                    if (includeRecipient)
                    {
                        recipientList.Add(recipient);
                    }

                    
                    /*
                //this is email list item - we don't want to send it like a group item
                if (groupRecipientsByPanel && !string.IsNullOrEmpty(recipient.EmailToAddress) && recipient.PanelTypeId == 3)
                    recipient.PanelID = 0;*/
                }
            }

            if (markOptedOut && filter != RecipientFilter.NotResponded)
                MarkOptedOutRecipients(recipientList);

            if (filter == RecipientFilter.Current || filter == RecipientFilter.All)
                MarkBouncedEmails(recipientList);

            return recipientList;
        }

        /// <summary>
        /// Marks recipients as bounced
        /// </summary>
        /// <param name="recipientList"></param>
        void MarkBouncedEmails(List<Recipient> recipientList)
        {
            if (Schedule.Count == 0 || recipientList.Count == 0)
                return;

            var bouncedEmails = GetBouncedEmails();

            foreach (var r in recipientList)
            {
                if (bouncedEmails.Contains(r.EmailToAddress.Trim().ToLower()))
                {
                    r.Bounced = true;
                }
            }
        }

        /// <summary>
        /// Returns bounced emails
        /// </summary>
        /// <returns></returns>
        private string[] GetBouncedEmails()
        {
            return EmailGateway.GetBouncedEmails().Select(e => e.Trim().ToLower()).ToArray();
        }

        /// <summary>
        /// Gets a paged DataTable of Recipient data
        /// </summary>
        /// <returns></returns>
        public List<Recipient> GetPagedRecipientData(RecipientFilter filter,
                                               string sortField,
                                               bool sortAscending,
                                               int pageNumber,
                                               int resultsPerPage,
                                               out int count)
        {
            List<Recipient> recipients = GetRecipients(filter, sortField, sortAscending);
            count = recipients.Count;

            return Utilities.GetListDataPage(recipients, pageNumber, resultsPerPage);
        }

        /// <summary>
        /// Get recipients with the specified recipient ids
        /// </summary>
        /// <param name="recipientIDs"></param>
        /// <param name="markOptedOut"> </param>
        /// <returns></returns>
        public List<Recipient> GetRecipients(IEnumerable<long> recipientIDs, bool markOptedOut = false)
        {
            var allRecipients = (from recipientId in recipientIDs
                    where RecipientDictionary.ContainsKey(recipientId)
                    select RecipientDictionary[recipientId]).ToList();

            if (markOptedOut)
                MarkOptedOutRecipients(allRecipients);

            return allRecipients;
        }

        private void MarkOptedOutRecipients(IEnumerable<Recipient> recipients)
        {
            var optedOutEmailAdresses = InvitationManager.GetOptedOutEmailsBySurveyId(ParentID);

            foreach (var r in recipients)
            {
                try
                {
                    if (!r.OptedOut && optedOutEmailAdresses.Any(e => e.Equals(r.EmailToAddress.ToLower().Trim(),
                                                                               StringComparison
                                                                                   .InvariantCultureIgnoreCase)))
                        r.OptedOut = true;
                }
                catch (Exception exc)
                {
                    
                }
             }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPendingRecipientsCount()
        {
            return GetPendingRecipients(false).Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public List<Recipient> ListPendingRecipients(int? batchSize = null)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetPendingRecipients");
            invitationCommand.AddInParameter("InvitationID", DbType.Int32, ID.Value);
            invitationCommand.AddInParameter("BatchSize", DbType.Int32, batchSize);

            List<Recipient> pendingRecipients = new List<Recipient>();

            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        int panelId = DbUtility.GetValueFromDataReader(reader, "PanelID", 0);
                        int panelTypeId = DbUtility.GetValueFromDataReader(reader, "PanelTypeID", 0);
                        string emailAddress = DbUtility.GetValueFromDataReader(reader, "EmailAddress", string.Empty).Replace("''", "'");
                        string uniqueIdentifier = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);

                        string panelTypeName = PanelManager.GetPanelTypeNameFromId(panelTypeId);

                        Recipient r = CreateRecipient(panelId, panelTypeId, panelTypeName);
                        r.EmailToAddress = emailAddress;
                        r.UniqueIdentifier = uniqueIdentifier;
                        pendingRecipients.Add(r);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return pendingRecipients;
        }

        /// <summary>
        /// Get pending recipients
        /// </summary>
        /// <param name="groupRecipientsByPanel">For recipients from user group/email list panels, don't list all
        /// recipients, just add a placeholder for user/group.</param>
        /// <param name="batchSize"> </param>
        /// <returns></returns>
        public virtual List<Recipient> GetPendingRecipients(bool groupRecipientsByPanel, int? batchSize = null)
        {
            List<Recipient> pendingRecipients = null;
            var result = new List<Recipient>();

            if (!groupRecipientsByPanel)
            {
                result = ListPendingRecipients(batchSize);
                
                if (!StaticConfiguration.DisableForeighMembershipProviders)
                {
                    foreach (var recipient in ListPendingActiveDirectoryRecipients(batchSize.HasValue ? batchSize.Value - result.Count : default(int?)))
                    {
                        if (!result.Any(r => r.EmailToAddress.Equals(recipient.EmailToAddress, StringComparison.InvariantCultureIgnoreCase)))
                            result.Add(recipient);
                    }
                }

                return result;
            }

            foreach (var panelData in ListPanelIds())
            {
                int panelId = panelData.Key;
                int panelTypeId = panelData.Value;
                Type panelType = Type.GetType(PanelManager.GetPanelTypeNameFromId(panelTypeId));

                var panel = Activator.CreateInstance(panelType) as Panel;
                panel.Load(panelId);

                if ((panel is EmailListPanel || panel is GroupPanel))
                {
                    result.Add(new Recipient
                    {
                        PanelTypeId = panelTypeId,
                        UniqueIdentifier = panel.Name,
                        PanelID = panelId,
                        PanelTypeName = panelType.Name,
                        GroupID = panel is GroupPanel ? ((GroupPanel)panel).GroupId : null
                    });
                }
                else
                {
                    if (pendingRecipients == null)
                    {
                        pendingRecipients = ListPendingRecipients();

                        if (!StaticConfiguration.DisableForeighMembershipProviders)
                        {
                            foreach (var recipient in ListPendingActiveDirectoryRecipients())
                            {
                                if (!pendingRecipients.Any(r => r.EmailToAddress.Equals(recipient.EmailToAddress, StringComparison.InvariantCultureIgnoreCase)))
                                    pendingRecipients.Add(recipient);
                            }
                        }
                    }

                    var pendingByPanel = pendingRecipients.Where(r => r.PanelID == panelId);
                    foreach (var recipient in pendingByPanel)
                    {
                        //If no unique identifier, use the email address
                        if (string.IsNullOrWhiteSpace(recipient.UniqueIdentifier))
                        {
                            recipient.UniqueIdentifier = recipient.EmailToAddress.Replace("'", string.Empty);
                        }
                        result.Add(recipient);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        private List<Recipient> ListPendingActiveDirectoryRecipients(int? batchSize = null)
        {
            //get already existing recipients
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetInvitedDomainRecipients");
            invitationCommand.AddInParameter("InvitationId", DbType.Int32, ID.Value);

            List<string> invitedDomainRecipients = new List<string>();

            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        string name = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);
                        invitedDomainRecipients.Add(name);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //
            List<Recipient> result = new List<Recipient>();
            
            //check panels for domain users
            foreach (var panel in PanelDictionary.Values)
            {
                if (batchSize.HasValue && result.Count >= batchSize.Value)
                    break;

                if (panel.PanelTypeName == typeof(UserPanel).FullName)
                {
                    foreach (UserPanelist panelist in panel.Panelists)
                    {
                        string uniqueIdentifier = panelist.UniqueIdentifier;

                        if (UserManager.IsDomainUser(uniqueIdentifier) &&
                            !invitedDomainRecipients.Contains(uniqueIdentifier) &&
                            !result.Any(r => r.EmailToAddress.Equals(panelist.Email, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            Recipient recipient = new Recipient(panelist);
                            recipient.PanelID = panel.ID.Value;
                            recipient.InvitationID = ID.Value;
                            
                            result.Add(recipient);
                        }
                    }
                }

                if (panel.PanelTypeName == typeof(GroupPanel).FullName)
                {
                    int groupId = (panel as GroupPanel).GroupId.Value;
                    var group = GroupManager.GetGroup(groupId);

                    foreach (var uniqueIdentifier in group.GetUserIdentifiers())
                    {
                        if (UserManager.IsDomainUser(uniqueIdentifier) &&
                            !invitedDomainRecipients.Contains(uniqueIdentifier))
                        {
                            string email = UserManager.GetUserEmail(uniqueIdentifier);
                            if (result.Any(r => r.EmailToAddress.Equals(email, StringComparison.InvariantCultureIgnoreCase)))
                                continue;

                            Recipient recipient = CreateRecipient(panel.ID.Value, panel.PanelTypeId, panel.PanelTypeName);
                            recipient.EmailToAddress = email;
                            recipient.InvitationID = ID.Value;
                            recipient.UniqueIdentifier = uniqueIdentifier;

                            result.Add(recipient);
                        }
                    }
                }
            }

            return result;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
       /* public virtual List<Recipient> GetPendingWizardRecipients()
        {
            var pendingRecipients = new List<Recipient>();

            //Now list all panelists and remove sent recipients from list of "to send" panelists
            foreach (Panel panel in PanelDictionary.Values)
            {
                //Do nothing if no panel id.  It shouldn't happen, but check just to be safe.
                if (!panel.ID.HasValue)
                {
                    continue;
                }

                if (panel is EmailListPanel || panel is GroupPanel)
                {
                    pendingRecipients.Add(new Recipient
                    {
                        PanelTypeId = panel.PanelTypeId,
                        UniqueIdentifier = panel.Name,
                        PanelID = panel.ID.Value,
                        PanelTypeName = panel.PanelTypeName
                    });
                }
                else
                {
                    List<Panelist> panelists = panel.Panelists;

                    foreach (Panelist panelist in panelists)
                    {
                        //skip if the email already exists
                        if (pendingRecipients.Any(recipient => recipient.EmailToAddress.Equals(panelist.Email)))
                            continue;

                        //Add pending recipient
                        Recipient r = CreateRecipient(panelist, panel.ID.Value, panel.PanelTypeId, panel.GetType().ToString());

                        //If no unique identifier, use the email address
                        if (Utilities.IsNullOrEmpty(r.UniqueIdentifier))
                        {
                            r.UniqueIdentifier = r.EmailToAddress;
                        }

                        pendingRecipients.Add(r);
                    }
                }
            }

            return pendingRecipients;
        }*/

        /// <summary>
        /// Factory method to create the appropriate Recipient from a given Panelist
        /// </summary>
        /// <remarks>
        /// When plugin is supported, this method will need to make the associations
        /// </remarks>
        /// <param name="panelist"></param>
        /// <param name="panelID"></param>
        /// <param name="panelTypeId"></param>
        /// <param name="panelTypeName"></param>
        /// <returns></returns>
        protected virtual Recipient CreateRecipient(Panelist panelist, Int32 panelID, int panelTypeId, string panelTypeName)
        {
            Recipient r = (panelist.GetType() == typeof(UserPanelist))
                ? new UserPanelistRecipient(panelist)
                : new Recipient(panelist);

            SetRecipientProperties(r, panelID, panelTypeId, panelTypeName);

            return r;
        }

        /// <summary>
        /// Factory method to create the appropriate Recipient from a given Panelist
        /// </summary>
        /// <remarks>
        /// When plugin is supported, this method will need to make the associations
        /// </remarks>
        /// <param name="panelID"></param>
        /// <param name="panelTypeId"></param>
        /// <param name="panelTypeName"></param>
        /// <returns></returns>
        protected virtual Recipient CreateRecipient(Int32 panelID, int panelTypeId, string panelTypeName)
        {
            Recipient r = (panelTypeName == typeof(UserPanelist).ToString())
                ? new UserPanelistRecipient()
                : new Recipient();

            SetRecipientProperties(r, panelID, panelTypeId, panelTypeName);

            return r;
        }

        /// <summary>
        /// Set the properties of the recipient
        /// </summary>
        /// <param name="r"></param>
        /// <param name="panelID"></param>
        /// <param name="panelTypeId"></param>
        protected virtual void SetRecipientProperties(Recipient r, Int32 panelID, int panelTypeId, string panelTypeName)
        {
            r.InvitationID = ID.Value;
            r.GUID = Guid.NewGuid();
            r.PanelID = panelID;
            r.PanelTypeId = panelTypeId;
            r.PanelTypeName = panelTypeName;
        }

        /// <summary>
        /// Loads the data for an Invitation
        /// </summary>
        public bool Load(Database db = null)
        {
            bool loaded = false;

            _panels = null;
            _recipientDictionary = null;
            _schedule = null;
            
            RemovedPanels.Clear();
            AddedPanels.Clear();

            //Now load
            if (db == null)
                db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetInvitation");
            invitationCommand.AddInParameter("InvitationID", DbType.Int32, ID.Value);

            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    if (reader.Read())
                    {
                        GUID = (Guid)reader["GUID"];
                        Template.Format = (MailFormat)Enum.Parse(typeof(MailFormat), DbUtility.GetValueFromDataReader(reader, "EmailFormat", "Text"));

                        Template.FromAddress = DbUtility.GetValueFromDataReader(reader, "FromAddress", string.Empty);
                        Template.FromName = DbUtility.GetValueFromDataReader(reader, "FromName", string.Empty);
                        Template.Subject = DbUtility.GetValueFromDataReader(reader, "Subject", string.Empty);
                        Template.Body = DbUtility.GetValueFromDataReader(reader, "Body", string.Empty);
                        Template.OptOutText = DbUtility.GetValueFromDataReader(reader, "OptOutText", string.Empty);
                        Template.IncludeOptOutLink = DbUtility.GetValueFromDataReader(reader, "IncludeOptOutLink", false);
                        Template.LinkText = DbUtility.GetValueFromDataReader(reader, "LinkText", string.Empty);
                        Template.ReminderBody = DbUtility.GetValueFromDataReader(reader, "ReminderBody", Template.Body);
                        Template.ReminderSubject = DbUtility.GetValueFromDataReader(reader, "ReminderSubject", Template.Subject);

                        if (reader["LoginOption"] != DBNull.Value && ((string)reader["LoginOption"]).ToLower() == "auto")
                        {
                            Template.LoginOption = LoginOption.Auto;
                        }
                        else
                        {
                            Template.LoginOption = LoginOption.None;
                        }

                        Created = DbUtility.GetValueFromDataReader(reader, "DateCreated", DateTime.Now);
                        CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty);
                        Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);
                        _successfullySent = DbUtility.GetValueFromDataReader(reader, "SuccessfullySent", default(bool?));
                        ErrorMessage = DbUtility.GetValueFromDataReader(reader, "ErrorMessage", string.Empty);
                        CompanyProfileId = DbUtility.GetValueFromDataReader(reader, "CompanyProfileId", default(int?));

                        DateTime lastSent;
                        if (DateTime.TryParse(reader["LastSentOn"].ToString(), out lastSent))
                        {
                            LastSent = lastSent;
                        }
                        else
                        {
                            LastSent = null;
                        }

                        //Parent ID is required
                        ParentID = (int)reader["ResponseTemplateID"];

                        loaded = true;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Get the counts
            DBCommandWrapper countCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetInvitationResponseCounts");
            countCommand.AddInParameter("InvitationID", DbType.Int32, ID.Value);

            using (IDataReader reader = db.ExecuteReader(countCommand))
            {
                try
                {
                    if (reader.Read())
                    {
                        SentCount = DbUtility.GetValueFromDataReader(reader, "SentCount", 0);
                        FailedCount = DbUtility.GetValueFromDataReader(reader, "FailedCount", 0);
                        ResponseCount = DbUtility.GetValueFromDataReader(reader, "ResponseCount", 0);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            LoadPanels();

            return loaded;
        }

        private void LoadPanels()
        {
            _panels = new Dictionary<int, Panel>();

            foreach (var panelID in ListPanelIds())
            {
                Panel p = PanelManager.GetPanel(panelID.Key);
                // account for these two special panels
                if (p is UserPanel)
                {
                    UsersPanel = (UserPanel)p;
                }

                if (p is AdHocEmailListPanel)
                {
                    AdHocEmailPanel = (AdHocEmailListPanel)p;
                }

                PanelDictionary[p.ID.Value] = p;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns>panel id, panel type id dictionary</returns>
        private Dictionary<int, int> ListPanelIds()
        {
            Dictionary<int, int> panels = new Dictionary<int, int>();

            if (!ID.HasValue)
                return panels;
            
            Database db = DatabaseFactory.CreateDatabase();

            var invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetPanels");
            invitationCommand.AddInParameter("InvitationID", DbType.Int32, ID.Value);

            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        var panelID = (Int32)reader["PanelID"];
                        var panelTypeID = (Int32)reader["PanelTypeID"];

                        panels.Add(panelID, panelTypeID);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return panels;
        }

        #region Scheduled Invitations
        /// <summary>
        /// Loads schedule data
        /// </summary>
        private void LoadSchedule()
        {
            _schedule = new List<InvitationSchedule>();

            if (!ID.HasValue)
                return;

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Get_Schedule");
            invitationCommand.AddInParameter("InvitationID", DbType.Int32, ID.Value);

            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        var scheduleItem = new InvitationSchedule();
                        scheduleItem.LoadFromReader(reader);
                        _schedule.Add(scheduleItem);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            _schedule.Sort((i1, i2) => i1.Scheduled.HasValue && i2.Scheduled.HasValue ? (int)Math.Round(i1.Scheduled.Value.Subtract(i2.Scheduled.Value).TotalSeconds) :
                (i1.Scheduled.HasValue ? int.MinValue : int.MaxValue));
        }

        /// <summary>
        /// Remove schedule record from the invitation and deleted it from the database
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteScheduleItem(int ID)
        {
            var scheduleItem = Schedule.Find(delegate(InvitationSchedule s) { return s.InvitationScheduleID == ID; });
            if (scheduleItem == null)
            {
                throw new Exception(string.Format("Invitation Schedule Record with ID = {0} has notbeen found.", ID));
            }

            scheduleItem.Delete();
            Schedule.Remove(scheduleItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleItem"></param>
        public void AddScheduleItem(InvitationSchedule scheduleItem)
        {
            Schedule.Add(scheduleItem);
            Schedule.Sort((i1, i2) => i1.Scheduled.HasValue && i2.Scheduled.HasValue ? (int)Math.Round(i1.Scheduled.Value.Subtract(i2.Scheduled.Value).TotalSeconds) : 
                (i1.Scheduled.HasValue? int.MinValue : int.MaxValue));
        }
        #endregion Scheduled Invitations


        /// <summary>
        /// Persists an Invitation
        /// </summary>
        public void Save(CheckboxPrincipal principal)
        {
            //Ensure panels have created by
            foreach (var panel in PanelDictionary.Values)
            {
                if (string.IsNullOrEmpty(panel.CreatedBy))
                {
                    panel.CreatedBy = principal.Identity.Name;
                }
            }

            if (ID == null || ID.Value <= 0)
            {
                Create(principal);
            }
            else
            {
                Update(principal);
            }
        }

        #region Save Methods
        /// <summary>
        /// Inserts a new invitation record into the database
        /// </summary>
        protected virtual void Create(CheckboxPrincipal principal)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Create");
            command.AddOutParameter("InvitationID", DbType.Int32, 4);
            command.AddInParameter("GUID", DbType.Guid, Guid.NewGuid());
            command.AddInParameter("ResponseTemplateID", DbType.Int32, ParentID);
            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("EmailFormat", DbType.String, Template.Format.ToString());

            command.AddInParameter(
                "CreatedBy",
                DbType.String,
                principal != null
                    ? principal.Identity.Name
                    : string.Empty);

            command.AddInParameter("IsPublic", DbType.Int32, 0);
            command.AddInParameter("DateCreated", DbType.DateTime, DateTime.Now);
            command.AddInParameter("Subject", DbType.String, Template.Subject);
            command.AddInParameter("Body", DbType.String, Template.Body);
            command.AddInParameter("FromAddress", DbType.String, Template.FromAddress);
            command.AddInParameter("FromName", DbType.String, Template.FromName);
            command.AddInParameter("LinkText", DbType.String, Template.LinkText);
            command.AddInParameter("LoginOption", DbType.String, Template.LoginOption);
            command.AddInParameter("IncludeOptOutLink", DbType.Boolean, Template.IncludeOptOutLink);
            command.AddInParameter("OptOutText", DbType.String, Template.OptOutText);
            command.AddInParameter("CompanyProfileId", DbType.Int32, CompanyProfileId);

            try
            {
                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        db.ExecuteNonQuery(command, transaction);
                        ID = (Int32)command.GetParameterValue("InvitationID");

                        SavePanels(principal, db, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new ApplicationException("Unable to save Invitation", ex);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }

            }
            catch (Exception e)
            {
                bool rethrow = ExceptionPolicy.HandleException(e, "BusinessPublic");
                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Updates and invitation record in the database
        /// </summary>
        protected virtual void Update(CheckboxPrincipal principal)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Update");
            command.AddInParameter("InvitationID", DbType.Int32, ID);
            command.AddInParameter("Name", DbType.String, Name);
            command.AddInParameter("EmailFormat", DbType.String, Template.Format.ToString());
            command.AddInParameter("Subject", DbType.String, Template.Subject);
            command.AddInParameter("Body", DbType.String, Template.Body);
            command.AddInParameter("ReminderSubject", DbType.String, Template.ReminderSubject);
            command.AddInParameter("ReminderBody", DbType.String, Template.ReminderBody);
            command.AddInParameter("FromAddress", DbType.String, Template.FromAddress);
            command.AddInParameter("FromName", DbType.String, Template.FromName);
            command.AddInParameter("LinkText", DbType.String, Template.LinkText);
            command.AddInParameter("LoginOption", DbType.String, Template.LoginOption);
            command.AddInParameter("IncludeOptOutLink", DbType.Boolean, Template.IncludeOptOutLink);
            command.AddInParameter("OptOutText", DbType.String, Template.OptOutText);
            command.AddInParameter("CompanyProfileId", DbType.Int32, CompanyProfileId);

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    db.ExecuteNonQuery(command);
                    SavePanels(principal, db, transaction);
                    transaction.Commit();

                }
                catch (Exception Ex)
                {
                    transaction.Rollback();
                    throw Ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Save panel and recipient data
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        private void SavePanels(CheckboxPrincipal principal, Database db, IDbTransaction transaction)
        {
            var panelsToUpdate = new List<Panel>(PanelDictionary.Values);
            List<Recipient> recipients = GetRecipients(RecipientFilter.All);

            foreach (Panel p in panelsToUpdate)
            {
                Int32 oldPanelId = p.ID.Value;

                PanelManager.SavePanel(p, principal);

                Int32 newPanelID = p.ID.Value;

                //Update panel ids for recipients if panel id gets changed
                if (newPanelID != oldPanelId)
                {
                    //Update panel ids for recipients & while we are here, update the recipients
                    foreach (Recipient recipient in recipients)
                    {
                        if (recipient.PanelID == oldPanelId)
                        {
                            recipient.PanelID = newPanelID;
                            recipient.Modified = true;
                        }

                        if (recipient.Modified)
                        {
                            SaveRecipient(recipient, db, transaction);
                        }
                    }

                    //Update panel mappings for added/removed in case
                    // panel id chnaged.
                    if (AddedPanels.Contains(oldPanelId))
                    {
                        AddedPanels.Remove(oldPanelId);
                        AddedPanels.Add(newPanelID);
                    }

                    if (RemovedPanels.Contains(oldPanelId))
                    {
                        RemovedPanels.Remove(oldPanelId);
                        RemovedPanels.Add(newPanelID);
                    }

                    //Update panel dictionary
                    if (PanelDictionary.ContainsKey(oldPanelId))
                    {
                        PanelDictionary.Remove(oldPanelId);
                    }

                    PanelDictionary[newPanelID] = p;
                }
            }

            //Save panel mappings
            foreach (int panelId in AddedPanels)
            {
                AddPanelMapping(panelId, db, transaction);
            }

            foreach (int panelId in RemovedPanels)
            {
                RemovePanelMapping(panelId, db, transaction);
            }

            //Now save any other modified recipients that weren't saved during a panel update above
            foreach (Recipient recipient in recipients)
            {
                if (recipient.Modified)
                {
                    SaveRecipient(recipient, db, transaction);
                }
            }

            //Finally, any newly deleted recipients should be saved.  Deleted recipients
            // are not retrieved when calling GetRecipients with the all parameter...this should
            // be fixed or All should be renamed, but it's too late in current release to make
            // such a large change, so for now, work around by adding special handling
            recipients = GetRecipients(RecipientFilter.Deleted);

            foreach (Recipient recipient in recipients)
            {
                //Check modified flag to see if newly deleted
                if (recipient.Modified)
                {
                    SaveRecipient(recipient, db, transaction);
                }
            }
        }

        /// <summary>
        /// Persist updated to recipients Deleted or OptOut status to the database
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        private static void SaveRecipient(Recipient recipient, Database db, IDbTransaction transaction)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_UpdateRecipient");
            command.AddInParameter("RecipientID", DbType.Int64, recipient.ID);
            command.AddInParameter("Deleted", DbType.Boolean, recipient.Deleted);
            command.AddInParameter("OptOut", DbType.Boolean, recipient.OptedOut);

            db.ExecuteNonQuery(command, transaction);

            //If necessary, remove any batch-related information
            //
            /* this code led to the database lock when we was deleting 2 or more recipients at one transaction
             * Now recipient messages are being deleted in the ckbx_sp_Invitation_UpdateRecipient
            if (recipient.BatchMessageId.HasValue && EmailGateway.ProviderSupportsBatches && recipient.ID.HasValue)
            {
                InvitationManager.RemoveRecipientMessagesFromEmailQueue(recipient.ID.Value, transaction);
            }
             */

            //Reset modified flag
            recipient.Modified = false;
        }

        /// <summary>
        /// Add a mapping between the invitation and a panel
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        private void AddPanelMapping(int panelId, Database db, IDbTransaction transaction)
        {
            DBCommandWrapper addPanel = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_AddPanel");
            addPanel.AddInParameter("InvitationID", DbType.Int32, ID);
            addPanel.AddInParameter("PanelID", DbType.Int32, panelId);

            db.ExecuteNonQuery(addPanel, transaction);
        }

        /// <summary>
        /// Remove a mapping between the invitation and a panel
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="db"></param>
        /// <param name="transaction"></param>
        private void RemovePanelMapping(int panelId, Database db, IDbTransaction transaction)
        {
            DBCommandWrapper removePanel = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RemovePanel");
            removePanel.AddInParameter("InvitationID", DbType.Int32, ID);
            removePanel.AddInParameter("PanelID", DbType.Int32, panelId);

            db.ExecuteNonQuery(removePanel, transaction);
        }

        #endregion


        #region Panel Helper Methods

        /// <summary>
        /// Get a list of uniqueidentifiers for all users that can be added to the invitation.  This is a list of users with access to the survey and
        /// not already on invitation.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public Dictionary<string, bool> ListAvailableUsers(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext, string provider)
        {
            //List current users directly on the invitation
            var panelistIds = UsersPanel.Panelists.OfType<UserPanelist>().Select(panelist => (panelist).UniqueIdentifier);

            var userList = UserManager.ListUsers(callingPrincipal, paginationContext, provider);
            Dictionary<string, bool> result = userList.ToDictionary(user => user, user => panelistIds.Contains(user));

            return result;
        }

        /// <summary>
        /// Get a list of uniqueidentifiers for all users that can be added to the invitation.  This is a list of users with access to the survey and
        /// not already on invitation.
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public PageItemUserData[] ListAvailablePageItemUserData(ExtendedPrincipal callingPrincipal, PaginationContext paginationContext, string provider)
        {
            //List current users directly on the invitation
            var panelistIds = UsersPanel.Panelists.OfType<UserPanelist>().Select(panelist => (panelist).UniqueIdentifier);

            var userList = UserManager.GetPageItemUsersData(callingPrincipal, paginationContext, provider);
            foreach (var userData in userList)
            {
                userData.IsInList = panelistIds.Contains(userData.UniqueIdentifier);
            }

            return userList;
        }

        ///<summary>
        ///</summary>
        ///<param name="principal"></param>
        ///<param name="paginationContext"></param>
        ///<returns></returns>
        public List<int> ListAvailableGroups(ExtendedPrincipal principal, PaginationContext paginationContext)
        {
            //Step 1: Store pagination values since we need to page after manipulating the list
            var originalPageNumber = paginationContext.CurrentPage;
            var originalPageSize = paginationContext.PageSize;

            paginationContext.CurrentPage = -1;
            paginationContext.PageSize = -1;

            //Step 2: List groups
            var groupIdList = GroupManager.ListAccessibleGroups(principal, paginationContext, true);

            //Step 3: Check group panels
            var existingGroupPanels = PanelDictionary.Values
                .OfType<GroupPanel>()
                .Where(panel => panel.GroupId.HasValue)
                .Select(panel => panel.GroupId.Value);

            //Step 4: Remove groups already on invite & ensure "EVERYONE" group not available
            var availableList = groupIdList
                .Except(existingGroupPanels)
                .Except(new[] { 1 });

            //Step 5: Restore paging values
            paginationContext.PageSize = originalPageSize;
            paginationContext.CurrentPage = originalPageNumber;
            paginationContext.ItemCount = availableList.Count();


            //Step 6: Return
            return availableList
                .Skip((originalPageNumber - 1) * originalPageSize)
                .Take(originalPageSize)
                .ToList();
        }

        ///<summary>
        ///</summary>
        ///<param name="principal"></param>
        ///<param name="paginationContext"></param>
        ///<returns></returns>
        public List<int> ListAvailableEmailLists(ExtendedPrincipal principal, PaginationContext paginationContext)
        {
            //Step 1: Store pagination values since we need to page after manipulating the list
            var originalPageNumber = paginationContext.CurrentPage;
            var originalPageSize = paginationContext.PageSize;

            paginationContext.CurrentPage = -1;
            paginationContext.PageSize = -1;

            paginationContext.Permissions = new List<string> { "EmailList.View" };

            //Step 2: List groups
            var emailListIds = EmailListManager.ListAvailableEmailLists(principal, paginationContext);

            //Step 3: Check email list panels
            var existingEmailListPanels = PanelDictionary.Values
                .OfType<EmailListPanel>()
                .Select(panel => panel.PanelId);

            //Step 4: Remove email lists already on invite
            var availableList = emailListIds
                .Except(existingEmailListPanels);

            //Step 5: Restore paging values
            paginationContext.PageSize = originalPageSize;
            paginationContext.CurrentPage = originalPageNumber;
            paginationContext.ItemCount = availableList.Count();


            //Step 6: Return
            return availableList
                .Skip((originalPageNumber - 1) * originalPageSize)
                .Take(originalPageSize)
                .ToList();
        }

        #endregion

        /// <summary>
        /// Record a response to an invitation
        /// </summary>
        /// <param name="recipientID"></param>
        /// <param name="responseID"></param>
        public static void RecordResponse(Int64 recipientID, Int64 responseID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_InsertResponse");
            command.AddInParameter("RecipientID", DbType.Int64, recipientID);
            command.AddInParameter("ResponseID", DbType.Int64, responseID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Gets number of processed recipients count
        /// </summary>
        /// <param name="processingBatchId"> </param>
        public int GetProcessedRecipientsCount(long processingBatchId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetProcessedRecipientsCount");
            command.AddInParameter("ProcessingBatchId", DbType.Int64, processingBatchId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    reader.Read();
                    return DbUtility.GetValueFromDataReader(reader, "RecipientCount", 0);
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipientIds"></param>
        public static void MarkRecipientsResponded(long[] recipientIds)
        {
            //[ckbx_sp_Invitation_MarkRecipientResponded]
            var db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {

                    foreach (var recipientId in recipientIds)
                    {
                        DBCommandWrapper command =
                            db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_MarkRecipientResponded");
                        command.AddInParameter("RecipientId", DbType.Int64, recipientId);

                        db.ExecuteNonQuery(command, transaction);

                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="creator"></param>
        public InvitationSchedule Test(string email, CheckboxPrincipal creator)
        {
            InvitationSchedule scheduleItem = null;

            //create a batch, if the provider supports batches
            long? messageBatchId = null;
            if (EmailGateway.ProviderSupportsBatches)
            {
                //initialize schedule item
                scheduleItem = new InvitationSchedule();
                scheduleItem.InvitationID = ID.Value;
                scheduleItem.InvitationActivityType = InvitationActivityType.Test;
                scheduleItem.BatchID = messageBatchId;
                scheduleItem.Scheduled = DateTime.Now.AddSeconds(1);
                this.Schedule.Add(scheduleItem);
                scheduleItem.Save(creator);

                messageBatchId = InvitationManager.CreateInvitationEmailBatch(
                    ID.Value,
                    InvitationActivityType.Test,
                    1,
                    creator.Identity.Name,
                    DateTime.Now.AddSeconds(1),
                    scheduleItem.InvitationScheduleID.Value);
            }

            //prepare invitation template
            InvitationTemplate template = Template.Copy();
            var customUserFieldNames = ProfileManager.ListPropertyNames();
            Guid? surveyGuid = ResponseTemplateManager.GetResponseTemplateGUID(ParentID);
            var baseSurveyUrl = InvitationPipeMediator.GetBaseSurveyUrl(surveyGuid);

            // create a test panel. 
            // that's necessary because adding a test recipient to the adHocPanel for this invitation will
            // lead to displaying this recipient in the invitation and so on.
            AdHocEmailListPanel p = new AdHocEmailListPanel();
            p.Name = "Test invitation panel for invitation " + ID.Value;
            p.CreatedBy = creator.Identity.Name;            
            p.Save();

            //prepare recipient info
            Recipient recipient = recipient = new Recipient();
            recipient.EmailToAddress = email;
            recipient.InvitationID = ID.Value;
            recipient.BatchMessageId = messageBatchId;
            recipient.PanelID = p.ID.Value;
            recipient.GUID = Guid.NewGuid();



            //that's a marker that this recipient is used for testing the invitation only
            recipient.UniqueIdentifier = "Test recipient " + email;

            recipient.PersonalizeTemplate(this, template, customUserFieldNames, baseSurveyUrl + (baseSurveyUrl.Contains("?") ? "&" : "?") + "test=true", surveyGuid);

            //load this recipient
            LoadRecipientData(ref _recipientDictionary);
            //mark it as deleted
            RemoveRecipient(recipient.ID.Value);
            //save to database
            Save(creator);


            //create a message
            EmailMessage message = InvitationSender.CreateEmailMessage(recipient, template, InvitationActivityType.Test);

            if (EmailGateway.ProviderSupportsBatches)
            {
                //add message to the batch
                EmailGateway.AddEmailMessageToBatch(messageBatchId.Value, message);
            }
            else
            {
                //send message
                EmailGateway.Send(message);
            }

            //close the test batch
            if (EmailGateway.ProviderSupportsBatches)
            {
                InvitationManager.CloseInvitationEmailBatch(messageBatchId.Value);
            }

            return scheduleItem;
        }

        /// <summary>
        /// Lock invitation properties or not. 
        /// 
        /// Depends on the current date and nearest scheduled date
        /// </summary>
        public bool InvitationLocked 
        { 
            get
            {
                if (!InvitationScheduled.HasValue)
                    return false;

                return NextInitationDispatchInMinutes < ApplicationManager.AppSettings.InvitationLockMinutes;
            }
        }

        /// <summary>
        /// Lock invitation properties or not. 
        /// 
        /// Depends on the current date and nearest scheduled date
        /// </summary>
        public bool ReminderLocked
        {
            get
            {
                if (!ReminderScheduled.HasValue)
                    return false;

                return NextReminderDispatchInMinutes < ApplicationManager.AppSettings.InvitationLockMinutes;
            }
        }

        /// <summary>
        /// Shows next dispatch time in minutes
        /// </summary>
        public int NextInitationDispatchInMinutes
        {
            get { return InvitationScheduled.HasValue ? (int)InvitationScheduled.Value.Subtract(DateTime.Now).TotalMinutes : 0; }
        }

        /// <summary>
        /// Shows next dispatch time in minutes
        /// </summary>
        public int NextReminderDispatchInMinutes
        {
            get { return ReminderScheduled.HasValue ? (int)ReminderScheduled.Value.Subtract(DateTime.Now).TotalMinutes : 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckForEmptyReminder()
        {
            if(Template.UpdateReminder())
                Save(UserManager.GetCurrentPrincipal());
        }

    }

    ///<summary>
    ///</summary>
    [Serializable]
    public enum LoginOption
    {
        ///<summary>
        ///</summary>
        None,
        ///<summary>
        ///</summary>
        Auto
    }

    ///<summary>
    ///</summary>
    [Serializable]
    public enum RecipientFilter
    {
        /// <summary>
        /// All Recipients
        /// </summary>
        All = 1,
        /// <summary>
        /// All current recipients
        /// </summary>
        Current,
        /// <summary>
        /// All Recipients with HasResponded = true
        /// </summary>
        Responded,
        /// <summary>
        /// All Recipients with HasResponded = false
        /// </summary>
        NotResponded,
        /// <summary>
        /// All Recipients passed as argument
        /// </summary>
        Selected,
        /// <summary>
        /// All recipients pending invitations
        /// </summary>
        Pending,
        /// <summary>
        /// Recipients that have opted out
        /// </summary>
        OptOut,
        /// <summary>
        /// Recipients that have been deleted
        /// </summary>
        Deleted,
        /// <summary>
        /// Recipients which emals have been bounced
        /// </summary>
        Bounced,
        /// <summary>
        /// All recipients pending invitations always ungrouped
        /// </summary>
        PendingUngrouped
    }
}
