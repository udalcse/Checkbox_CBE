using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Definition of service interface for managing invitations.
    /// </summary>
    [ServiceContract]
    public interface IInvitationManagementApi
    {
        /// <summary>
        /// Authenticate the provided login credentials.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The associated password.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{String}"/> object where T is of type <see cref="System.String"/>. </para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains an encrypted forms auth token.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [WebGet]
        [OperationContract]
        ServiceOperationResult<string> AuthenticateUser(string username, string password);

        /// <summary>
        /// Create a new invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey to create an invitation for.</param>
        /// <param name="name">Name of the creating invitation</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="InvitationData"/> object.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<InvitationData> CreateInvitation(string authToken, int surveyId, string name);

        /// <summary>
        /// Retrieve an invitation by id.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="InvitationData"/> object.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationData> GetInvitation(string authToken, int invitationId);

        /// <summary>
        /// Persist changes to the specified invitation, excluding recipient changes.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationData">The new invitation data.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateInvitation(string authToken, InvitationData invitationData);

        /// <summary>
        /// Delete the specified invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteInvitation(string authToken, int invitationId);

        /// <summary>
        /// Add one or more email address to an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to update.</param>
        /// <param name="emailAddresses">The list of email addresses to add to the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailAddressesToInvitation(string authToken, int invitationId, string[] emailAddresses);

        /// <summary>
        /// Add one or more users to an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to update.</param>
        /// <param name="usernames">The list of users to add to the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUsersToInvitation(string authToken, int invitationId, string[] usernames);

        /// <summary>
        /// Generate links for specified users.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="usernames">The list of users to add to the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> GenerateUsersLinks(string authToken, int surveyId, string[] usernames);

        /// <summary>
        /// Add one or more email list to an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to update.</param>
        /// <param name="emailListIds">The email list panels to add to the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailListsToInvitation(string authToken, int invitationId, int[] emailListIds);

        /// <summary>
        /// Add one or more user groups to an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to update.</param>
        /// <param name="userGroupIds">The list of user groups to add to the invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserGroupsToInvitation(string authToken, int invitationId, int[] userGroupIds);

        /// <summary>
        /// Retrieve a paged, sorted and filtered list of invitation recipients.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation.</param>
        /// <param name="recipientStatusFilter">The value used to filter the results by recipient status.</param>
        /// <param name="recipientIdFilter">The value to to filter recipients by name or email address.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="RecipientData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of recipients that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        /// <remarks>
        /// Available recipient filters:
        /// <list type="bullet">
        ///     <item>
        ///         <description>Pending</description> 
        ///     </item>
        ///     <item>
        ///         <description>Deleted</description> 
        ///     </item>
        ///     <item>
        ///         <description>Current</description> 
        ///     </item>
        ///     <item>
        ///         <description>OptOut</description> 
        ///     </item>
        ///     <item>
        ///         <description>Responded</description>  
        ///     </item>
        ///     <item>
        ///         <description>NotResponded</description>  
        ///     </item>
        /// </list>
        /// </remarks>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<RecipientData[]>> ListInvitationRecipients(string authToken,
                                                                                            int invitationId,
                                                                                            string recipientStatusFilter,
                                                                                            string recipientIdFilter,
                                                                                            int pageNumber,
                                                                                            int pageSize);

        /// <summary>
        /// Remove the specified recipients from an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to remove the recipients from.</param>
        /// <param name="recipientList">The ids of the recipients to be removed.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveRecipients(string authToken, int invitationId, long[] recipientList);

        /// <summary>
        /// Send an invitation to the recipients matching the specified filter.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of the invitation to be sent.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SendInvitationToFilteredRecipientList(string authToken, int invitationId,
                                                                             string recipientFilter);

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
        /// Retrieve a paged, sorted and filtered list of the invitations associated with a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="InvitationData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of invitation that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListInvitations(string authToken,
                                                                                    int surveyId,
                                                                                    int pageNumber,
                                                                                    int pageSize,
                                                                                    string sortField,
                                                                                    bool sortAscending,
                                                                                    string filterField,
                                                                                    string filterValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
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
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredInvitations(string authToken,
                                                                                    int surveyId,
                                                                                    int pageNumber,
                                                                                    int pageSize,
                                                                                    string sortField,
                                                                                    bool sortAscending,
                                                                                    string filterField,
                                                                                    string filterValue,
                                                                                    string filterKey = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
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
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredUsersInvitations(string authToken,
                                                                                    int surveyId,
                                                                                    int pageNumber,
                                                                                    int pageSize,
                                                                                    string sortField,
                                                                                    bool sortAscending,
                                                                                    string filterField,
                                                                                    string filterValue,
                                                                                    string filterKey = null);



        /// <summary>
        /// Retrieve a list of recently sent invitations by survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="InvitationData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListSentInvitations(string authToken, int surveyId, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieve a list of scheduled invitations by survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="InvitationData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<InvitationData[]>> ListScheduledInvitations(string authToken, int surveyId, int pageNumber, int pageSize);

        /// <summary>
        /// Get opted out invitation details
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="email"> </param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="invitationId">ID of the invitation.</param>
        /// <returns>List of invitation info objects.</returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<OptedOutInvitationData> GetEmailOptOutDetails(string authTicket, string email, int responseTemplateId, int invitationId);

        /// <summary>
        /// Retrieve the total number of invitation associated with a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the number of invitations.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationCountForSurvey(string authToken, int surveyId);


        /// <summary>
        /// Retrieve the total number of invitation associated with a survey.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">The id of the survey.</param>
        /// <param name="isSent">Select Sent or Scheduled invitations count</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the number of invitations.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationCountForSurveyByType(string authToken, int surveyId, bool isSent);

        /// <summary>
        /// Retrieve a paged, sorted and filtered list of email list panels.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="permission">Permission to check for on the email lists.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="EmailListPanelData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of email list panels that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanels(string authToken,
                                                                                        string permission,
                                                                                        int pageNumber,
                                                                                        int pageSize,
                                                                                        string sortField,
                                                                                        bool sortAscending,
                                                                                        string filterField,
                                                                                        string filterValue);

        /// <summary>
        /// Retrieve a paged, sorted and filtered list of email list panels.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="EmailListPanelData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of email list panels that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanelsByPeriod(string authToken,
                                                                                        int pageNumber,
                                                                                        int pageSize,
                                                                                        string sortField,
                                                                                        bool sortAscending,
                                                                                        string filterField,
                                                                                        string filterValue,
                                                                                        int period,
                                                                                        string dateFieldName);
        /// <summary>
        /// Create a new email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="name">The name of the new email list.</param>
        /// <param name="description">The description of the new email list.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="EmailListPanelData"/> object.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<EmailListPanelData> CreateEmailListPanel(string authToken, string name, string description);

        /// <summary>
        /// Retrieve an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel to retrieve.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="EmailListPanelData"/> object.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<EmailListPanelData> GetEmailListPanel(string authToken, int emailListPanelId);

        /// <summary>
        /// Create a copy of an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the source email list panel.</param>
        /// <param name="languageCode">The language code to use when storing the name and description of the copy.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="int"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains the id of the new email list panel. If the value is negative, an error has occurred.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> CopyEmailListPanel(string authToken, int emailListPanelId, string languageCode);

        /// <summary>
        /// Update an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="panelInfo">The modified email list panel details.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdateEmailListPanel(string authToken, EmailListPanelData panelInfo);

        /// <summary>
        /// Add one or more email address to an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list to update.</param>
        /// <param name="emailAddresses">The list of email addresses to add to the panel.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEmailAddressesToEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses);

        /// <summary>
        /// Remove one or more email address from an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the panel to update.</param>
        /// <param name="emailAddresses">The email addresses to remove from the panel.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveEmailAddressesFromEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses);

        /// <summary>
        /// Retrieve the list the email addresses contained in an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="PagedListResult{String}"/> of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of email addresses.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<string[]>> ListEmailListPanelAddresses(string authToken, int emailListPanelId, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieve an email list panel's default permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The email list panel's id.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId);

        /// <summary>
        /// Set the default policy permissions for an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="permissions">The list of permissions.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> SetEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId, string[] permissions);

        /// <summary>
        /// Retrieve the list of permissions that a user has been granted to the specified email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>
        /// The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.
        /// </para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForUser(string authToken, int emailListPanelId, string uniqueIdentifier);

        /// <summary>
        /// Retrieve the list of permissions that a user group has been granted to the specified email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="userGroupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an array of <see cref="string"/>s.</para>
        /// <para>
        /// The <see cref="ServiceOperationResult{T}.ResultData"/> property contains a list of permission names.
        /// </para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForGroup(string authToken, int emailListPanelId, int userGroupId);

        /// <summary>
        /// Remove a user from an email list panel's access list.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveUserFromEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier);

        /// <summary>
        /// Remove a user group from an email list panel's access list.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="userGroupId">The id of the user group.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveGroupFromEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId);

        /// <summary>
        /// Add a user to an email list panel's ACL (access control list) with the specified permissions. If the user is already
        /// on the access list, the user's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="uniqueIdentifier">The unique identifier of the user.</param>
        /// <param name="permissions">The list of permissions.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddUserToEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier, string[] permissions);

        /// <summary>
        /// Add a user group to an email list panel's ACL (access control list) with the specified permissions. If the user group is already
        /// on the access list, the user group's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel.</param>
        /// <param name="userGroupId">The id of the user group.</param>
        /// <param name="permissions">The list of permissions.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddGroupToEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId, string[] permissions);

        /// <summary>
        /// Retrieve summary information about recipient responses to an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The Id of invitation.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is an <see cref="InvitationRecipientSummary"/> object.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationRecipientSummary> GetRecipientSummary(string authToken, int invitationId);

        /// <summary>
        /// Retrieve a list of recent survey responses by invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The invitation id.</param>
        /// <param name="count">The number of responses to retrieve.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="ResponseData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ResponseData[]> ListRecentResponses(string authToken, int invitationId, int count);

        /// <summary>
        /// Retrieve a paged, sorted, and filtered list of survey responses by invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation to get summary information for.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="ResponseData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of responses that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ResponseData[]>> ListResponses(string authToken,
                                                                                int invitationId,
                                                                                int pageNumber,
                                                                                int pageSize,
                                                                                string sortField,
                                                                                bool sortAscending,
                                                                                string filterField,
                                                                                string filterValue);

        /// <summary>
        /// Delete the specified email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">The id of the email list panel to delete.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteEmailListPanel(string authToken, int emailListPanelId);

        /// <summary>
        /// Retrieve a paged, sorted, and filtered list of users that are eligible to be added to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="provider"></param>
        /// <param name="invitationId">The id of invitation to get summary information for.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="UserData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of available users that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<UserData[]>> ListAvailableUsersForInvitation(string authToken,
                                                                                            string provider,
                                                                                            int invitationId,
                                                                                            int pageNumber,
                                                                                            int pageSize,
                                                                                            string sortField,
                                                                                            bool sortAscending,
                                                                                            string filterField,
                                                                                            string filterValue);

        /// <summary>
        /// Retrieve a paged, sorted, and filtered list of users groups that are eligible to be added to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation to get summary information for.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="UserGroupData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of available user groups that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<UserGroupData[]>> ListAvailableUserGroupsForInvitation(string authToken,
                                                                                                        int invitationId,
                                                                                                        int pageNumber,
                                                                                                        int pageSize,
                                                                                                        string sortField,
                                                                                                        bool sortAscending,
                                                                                                        string filterField,
                                                                                                        string filterValue);

        /// <summary>
        /// Retrieve a paged, sorted, and filtered list of email list panels that are eligible to be added to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation to get summary information for.</param>
        /// <param name="pageNumber">The index of the first page to begin retrieving results from. Specifying 0 disables pagination.</param>
        /// <param name="pageSize">The number of results to display on a page. Specifying 0 disables pagination.</param>
        /// <param name="sortField">The field used when sorting results. Specifying null disables sorting.</param>
        /// <param name="sortAscending">The sort direction.</param>
        /// <param name="filterField">The field to use when filtering results. Specifying null disables filtering.</param>
        /// <param name="filterValue">Filter criteria. Specifying null disables filtering.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="PagedListResult{T}"/> of <see cref="EmailListPanelData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of available email list panels that match the filter criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListAvailableEmailListsForInvitation(string authToken,
                                                                                                            int invitationId,
                                                                                                            int pageNumber,
                                                                                                            int pageSize,
                                                                                                            string sortField,
                                                                                                            bool sortAscending,
                                                                                                            string filterField,
                                                                                                            string filterValue);

        /// <summary>
        /// Search for invitations.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="searchTerm">The value to search for.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is a <see cref="GroupedResult{T}"/> of <see cref="InvitationData"/> objects.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> contains the list of invitations that match the search criteria.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<InvitationData>[]> SearchInvitations(string authToken, string searchTerm);


        /// <summary>
        /// Mark the provided lists of recipients as "Deleted" for the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation.</param>
        /// <param name="recipientUserNames">The list of recipient usernames to remove from the invitation.</param>
        /// <param name="recipientEmailAddresses">The list of recipient email addresses to remove from the invitation.</param>
        /// <param name="recipientGroupIds">The list of receipent group ids to remove from the invitation </param>
        /// <param name="recipientEmailListIds"> </param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemovePendingRecipients(string authToken, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds);

        /// <summary>
        /// Mark the provided lists of recipients as "Opted Out" for the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation.</param>
        /// <param name="recipientList">The list of recipient ids to opt out.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MarkRecipientsOptedOut(string authToken, int invitationId, long[] recipientList);

        /// <summary>
        /// Mark the provided lists of recipients as "Responded" for the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">The id of invitation.</param>
        /// <param name="recipientList">The list of recipient ids to mark as responded.</param>
        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{T}"/> object where T is of type <see cref="object"/>.</para>
        /// <para>The <see cref="ServiceOperationResult{T}.ResultData"/> property contains no data.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MarkRecipientsResponded(string authToken, int invitationId, long[] recipientList);

        ///<summary>
        /// Get the number of sent successfully sent invitations.
        ///</summary>
        ///<param name="authToken"></param>
        ///<param name="responseTemplateId"></param>
        ///<returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetInvitationSentCount(string authToken, int responseTemplateId);

        /// <returns>
        /// <para>A <see cref="ServiceOperationResult{UserData}"/> object where T is an array of <see cref="InvitationData"/> objects.</para>
        /// All results are wrapped in a ServiceOperationResult object. The <see cref="ServiceOperationResult{T}.CallSuccess"/> property indicates whether or not the action completed successfully.
        /// </returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<InvitationData[]> ListRecentlySentInvitations(string authToken, int surveyId, int count);
    }
}
