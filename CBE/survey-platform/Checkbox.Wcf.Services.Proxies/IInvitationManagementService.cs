using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Definition of service interface for managing invitations.
    /// </summary>
    [ServiceContract]
    public interface IInvitationManagementService
    {
        /// <summary>
        /// Authenticate the provided login credentials.
        /// </summary>
        /// <param name="userName">The username to authenticate.</param>
        /// <param name="password">The associated password.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{String}"/> object where T is of type <see cref="System.String"/>. </para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an encrypted forms auth token.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string> AuthenticateUser(string userName, string password);

        /// <summary>
        /// Create a new invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateID">ID of the response template to create an invitation for.</param>
        /// <param name="name">Name of the creating invitation</param>
        /// <returns>Invitation info object representing the new invitation.</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<InvitationData> CreateInvitation(string authTicket, int responseTemplateID, string name);

        /// <summary>
        /// Get an information object for the specified information.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get information for.</param>
        /// <returns>Lightweight object containing invitation information.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationData> GetInvitation(string authTicket, int invitationID);

        /// <summary>
        /// Persist changes to the specified invitation, excluding recipient changes.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitation"></param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateInvitation(string authTicket, InvitationData invitation);

        /// <summary>
        /// Delete the specified invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationId">ID of the invitation to delete.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteInvitation(string authTicket, int invitationId);

        /// <summary>
        /// Delete the specified invitations.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationIds">ID of the invitation to delete.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteInvitations(string authTicket, int[] invitationIds);
        
        /// <summary>
        /// Add the specified email addresses to the invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailAddresses">List of email addresses to add to the invitation.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailAddressesToInvitation(string authTicket, int invitationID, string[] emailAddresses);

        /// <summary>
        /// Add the specified users to the invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUsersToInvitation(string authTicket, int invitationID, string[] userNames);

        /// <summary>
        /// Generate links for specified users.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> GenerateUsersLinks(string authTicket, int surveyId, string[] userNames);

        /// <summary>
        /// Add the specified email list panels to the invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailListPanelIDs">Ids of email list panels to add to the invitation.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailListPanelsToInvitation(string authTicket, int invitationID, int[] emailListPanelIDs);

        /// <summary>
        /// Add the specified user groups to the invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to add a user group to.</param>
        /// <param name="userGroupIDs">IDs of the user groups to add to the invitation.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserGroupsToInvitation(string authTicket, int invitationID, int[] userGroupIDs);

        /// <summary>
        /// Get a list of recipients for a given invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="recipientStatusFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <param name="recipientIdFilter">Value to use to filter on recipient name or email address.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of recipients.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<RecipientData[]>> ListInvitationRecipients(string authTicket, int invitationID, string recipientStatusFilter, string recipientIdFilter, int pageNumber, int pageSize);


        /// <summary>
        /// Get a list of recent recipients for a given invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="count">Number of recent recipients to list.</param>
        /// <returns>List of recipients.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<RecipientData[]> ListRecentInvitationRecipients(string authTicket, int invitationID,  int count);


        /// <summary>
        /// Send the invitation to the recipients matching the specified filter.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to send.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SendInvitationToFilteredRecipientList(string authTicket, int invitationID, string recipientFilter);


        /// <summary>
        /// Send and invitation or a reminder to the recipients
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to send.</param>
        /// <param name="recipientEmails">Recipient emails</param>
        /// <param name="InvitationType">Type of the invitation: Invitation or a Reminder</param>
        /// <returns>Distionary with pairs: {Email, Result}</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> SendInvitationToRecipientList(string authToken, int invitationId, string recipientEmails, string InvitationType);

        /// <summary>
        /// List invitations for a survey
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey=null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredUsersInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null);


        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageSize></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListSentInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize);

        /// <summary>
        /// List scheduled invitations for a survey
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListScheduledInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize);

        /// <summary>
        /// Get opted out invitation details
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="email"> </param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<OptedOutInvitationData> GetEmailOptOutDetails(string authTicket, string email, int responseTemplateId, int invitationId);

        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey</param>
        /// <returns>Count of invitations</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationCountForSurvey(string authTicket, int responseTemplateId);

        /// <summary>
        /// Retrieve the total number of invitation associated with a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="responseTemplateId">The id of the survey.</param>
        /// <param name="isSent">Select Sent or Scheduled invitations count</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the number of invitations.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationCountForSurveyByType(string authToken, int responseTemplateId, bool isSent);

        /// <summary>
        /// Get a list of email
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="permission">Permission to check for on the email lists.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>List of email list panel info objects for the panels.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanels(string authTicket, string permission, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get a list of email
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>List of email list panel info objects for the panels.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanelsByPeriod(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName);

        /// <summary>
        /// Create a new email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="name">The name of the new email list.</param>
        /// <param name="description">The description of the new email list.</param>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<EmailListPanelData> CreateEmailListPanel(string authTicket, string name, string description);

        /// <summary>
        /// Get a lightweight information object for the specified email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get.</param>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<EmailListPanelData> GetEmailListPanelInfo(string authTicket, int emailListPanelID);

        /// <summary>
        /// Create a copy of emailListPanel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">Id of emailListPanel which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID of the newly created emailListPanel.  If the value is negative, a it was not successfully created.</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> CopyEmailListPanel(string authTicket, int emailListPanelID, string languageCode);

        /// <summary>
        /// Create a copies of emailListPanels.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelIdList">Id list of emailListPanels which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID list of the newly created emailListPanels.</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int[]> CopyEmailListPanels(string authTicket, int[] emailListPanelIdList, string languageCode);

        /// <summary>
        /// Persist changes to the email list panel info to the database.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="panelInfo">Information for the panel to update.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateEmailListPanel(string authTicket, EmailListPanelData panelInfo);

        /// <summary>
        /// Add the specified email addresses to an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to add addresses to</param>
        /// <param name="emailAddresses">Addresses to add to the panel.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailAddressesToEmailListPanel(string authTicket, int emailListPanelID, string[] emailAddresses);

        /// <summary>
        /// Remove the specified email addresses from an email list panel
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to remove email addresses from.</param>
        /// <param name="emailAddresses">Email addresses to remove from the panel.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveEmailAddressesFromEmailListPanel(string authTicket, int emailListPanelID, string[] emailAddresses);

        /// <summary>
        /// List the email addresses contained in an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of email addresses in the panel.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<string[]>> ListEmailListPanelAddresses(string authTicket, int emailListPanelID, int pageNumber, int pageSize);

        /// <summary>
        /// Get a list of permissions set on the default policy of an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get the default policy permissions for.</param>
        /// <returns>List of permissions on the email list panel's default policy.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelDefaultPolicyPermissions(string authTicket, int emailListPanelID);

        /// <summary>
        /// Set the default policy permissions for an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to set default policy permissions for.</param>
        /// <param name="permissions">Permissions to set on the default policy.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SetEmailListPanelDefaultPolicyPermissions(string authTicket, int emailListPanelID, string[] permissions);

        /// <summary>
        /// List the ACL permissions that a given user has on an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user get the permissions list for.</param>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForUser(string authTicket, int emailListPanelID, string uniqueIdentifier);

        /// <summary>
        /// List the ACL permissions that a given user group has on an email list panel.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to list acl permissions for.</param>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForGroup(string authTicket, int emailListPanelID, int userGroupID);

        /// <summary>
        /// Remove a user from an email list panel's access list.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from the access list.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUserFromEmailListPanelAccessList(string authTicket, int emailListPanelID, string uniqueIdentifier);

        /// <summary>
        /// Remove a user group from an email list panel's access list.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to remove a user from.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveGroupFromEmailListPanelAccessList(string authTicket, int emailListPanelID, int userGroupID);

        /// <summary>
        /// Add a user to an email list panel's access list with the specified permissions.  If the user is already
        /// on the access list, the user's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserToEmailListPanelAccessList(string authTicket, int emailListPanelID, string uniqueIdentifier, string[] permissions);

        /// <summary>
        /// Add a user group to an email list panel's access list with the specified permissions.  If the user group is already
        /// on the access list, the user group's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user group.</param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddGroupToEmailListPanelAccessList(string authTicket, int emailListPanelID, int userGroupID, string[] permissions);

        /// <summary>
        /// Get basic summary information about number of invitation recipients.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationId">Database Id of invitation to get summary information for.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationRecipientSummary> GetRecipientSummary(string authTicket, int invitationId);

        /// <summary>
        /// Get basic summary information about number of invitation recipients.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationId">Database Id of invitation to get summary information for.</param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseData[]> ListRecentResponses(string authTicket, int invitationId, int count);

        /// <summary>
        /// Get basic summary information about number of invitation recipients.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationId">Database Id of invitation to get summary information for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListResponses(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Delete the specified email list panel
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the emailListPanel to delete.</param>
        /// <returns></returns>         
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteEmailListPanel(string authTicket, int emailListPanelID);


        /// <summary>
        /// Delete the specified email list panels
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelIdList">ID list of the email list panels to delete.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteEmailListPanels(string authTicket, int[] emailListPanelIdList);

        /// <summary>
        /// List users that are able to be added to the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="provider"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<UserData[]>> ListAvailableUsersForInvitation(string authTicket, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// List users that are able to be added to the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="provider"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<PageItemUserData[]>> ListAvailablePageItemUserDataForInvitation(string authTicket, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// List user groups that can be added to an invitation
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<UserGroupData[]>> ListAvailableUserGroupsForInvitation(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// List email lists that can be added to an invitation
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListAvailableEmailListsForInvitation(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Search for invitations.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<InvitationData>[]> SearchInvitations(string authTicket, string searchTerm);

        /// <summary>
        /// Mark the provided lists of recipients as "Deleted" for the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveRecipients(string authTicket, int invitationId, long[] recipientList);

        /// <summary>
        /// Mark the provided lists of recipients as "Deleted" for the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientUserNames"></param>
        /// <param name="recipientEmailAddresses"></param>
        /// <param name="recipientGroupIds"></param>
        /// <param name="recipientEmailListIds"> </param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemovePendingRecipients(string authTicket, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds);
        
        /// <summary>
        /// Mark the provided lists of recipients as "Opted Out" for the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MarkRecipientsOptedOut(string authTicket, int invitationId, long[] recipientList);

        /// <summary>
        /// Mark the provided lists of recipients as "Responded" for the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MarkRecipientsResponded(string authTicket, int invitationId, long[] recipientList);

        /// <summary>
        /// Get a list of recipients for a given invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="recipientStatusFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <param name="recipientIdFilter">Value to use to filter on recipient name or email address.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of recipients.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationScheduleData[]>> ListInvitationSchedule(string authTicket, int invitationID, bool sortAscending, int pageNumber, int pageSize);

        /// <summary>
        /// Mark the provided lists of recipients as "Deleted" for the invitation.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationID"></param>
        /// <param name="scheduleItemList"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteScheduleItems(string authTicket, int invitationID, int[] scheduleItemList);

        /// <summary>
        /// Sets a required date for the invitation sending
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationID"></param>
        /// <param name="scheduledDate"></param>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SetScheduledDate(string authTicket, int invitationID, int? scheduleID, string scheduledDate);

        /// <summary>
        /// Returns a schedule status
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> GetScheduleStatus(string authTicket, int scheduleID);

        /// <summary>
        /// Requests actual recipients before sending the batch
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <param name="batchSize"> </param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> RequestBatchMessagesPartially(string authTicket, int scheduleID, int batchSize);

        /// <summary>
        /// Requests actual recipients before sending the batch
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RequestBatchMessages(string authTicket, int scheduleID);

        /// <summary>
        /// Updates the status of the batch
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <param name="Status"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateBatchStatus(string authTicket, int scheduleID, string Status, string ErrorText);

        ///<summary>
        /// Get the number of sent successfully sent invitations.
        ///</summary>
        ///<param name="authToken"></param>
        ///<param name="responseTemplateId"></param>
        ///<returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationSentCount(string authToken, int responseTemplateId);

        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="count">Number of inivitations to include in the list.</param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationData[]> ListRecentlySentInvitations(string authTicket, int responseTemplateId, int count);

        /// <summary>
        /// Reset 'ProcessingBatchId' column to NULL in ckbx_InvitationRecipients for requested invitation
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="scheduleID"> </param>
        /// <returns>.</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> ResetProcessingBatchForRecipients(string authTicket, int scheduleID);

        /// <summary>
        /// Returns batch id for the original invitation
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="scheduleID"> </param>
        /// <returns>returns -1 if the current shedule alredy is the original invitation</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetRelatedInvitationBatchId(string authTicket, int scheduleID);
    }
}
