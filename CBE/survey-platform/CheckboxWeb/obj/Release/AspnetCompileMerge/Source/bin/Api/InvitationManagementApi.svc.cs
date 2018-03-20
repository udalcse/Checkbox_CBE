using System;
using System.Linq;
using System.ServiceModel.Activation;
using Checkbox.Invitations;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;

using Prezza.Framework.ExceptionHandling;
using System.Collections.Generic;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// CheckboxWeb Invitation Management Web Service
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class InvitationManagementApi : IInvitationManagementApi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> AuthenticateUser(string username, string password)
        {
            var service = new AuthenticationService();

            return service.Login(username, password);
        }

        #region Invitations

        /// <summary>
        /// Create a new invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the response template to create an invitation for.</param>
        /// <param name="name">Name of the creating invitation</param>
        /// <returns>Invitation info object representing the new invitation.</returns>
        public ServiceOperationResult<InvitationData> CreateInvitation(string authToken, int surveyId, string name)
        {
            try
            {
                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CreateInvitation(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, name)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get an information object for the specified information.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationID">ID of the invitation to get information for.</param>
        /// <returns>Lightweight object containing invitation information.</returns>
        public ServiceOperationResult<InvitationData> GetInvitation(string authToken, int invitationID)
        {
            try
            {
                return new ServiceOperationResult<InvitationData>
              {
                  CallSuccess = true,
                  ResultData = InvitationManagementServiceImplementation.GetInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationID)
              };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Persist changes to the specified invitation, excluding recipient changes.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationData"></param>
        public ServiceOperationResult<object> UpdateInvitation(string authToken, InvitationData invitationData)
        {
            try
            {
                InvitationManagementServiceImplementation.UpdateInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationData);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Delete the specified invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to delete.</param>
        public ServiceOperationResult<object> DeleteInvitation(string authToken, int invitationId)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add the specified email addresses to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">Id of the invitation.</param>
        /// <param name="emailAddresses">List of email addresses to add to the invitation.</param>
        public ServiceOperationResult<object> AddEmailAddressesToInvitation(string authToken, int invitationId, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailAddressesToInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, emailAddresses);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add the specified users to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">Id of the invitation.</param>
        /// <param name="usernames">List of users to add to the invitation.</param>
        public ServiceOperationResult<object> AddUsersToInvitation(string authToken, int invitationId, string[] usernames)
        {
            try
            {
                InvitationManagementServiceImplementation.AddUsersToInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, usernames);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Generate links for specified users.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="usernames">List of users to add to the invitation.</param>
        public ServiceOperationResult<object> GenerateUsersLinks(string authToken, int surveyId, string[] usernames)
        {
            try
            {
                InvitationManagementServiceImplementation.GenerateUsersLinks(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, usernames);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add the specified email list panels to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">Id of the invitation.</param>
        /// <param name="emailListIds">Ids of email list panels to add to the invitation.</param>
        public ServiceOperationResult<object> AddEmailListsToInvitation(string authToken, int invitationId, int[] emailListIds)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailListPanelsToInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, emailListIds);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add the specified user groups to the invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to add a user group to.</param>
        /// <param name="userGroupIDs">IDs of the user groups to add to the invitation.</param>
        public ServiceOperationResult<object> AddUserGroupsToInvitation(string authToken, int invitationId, int[] userGroupIDs)
        {
            try
            {
                InvitationManagementServiceImplementation.AddUserGroupsToInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, userGroupIDs);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get a list of recipients for a given invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to get recipients for.</param>
        /// <param name="recipientStatusFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <param name="recipientIdFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of recipients.</returns>
        public ServiceOperationResult<PagedListResult<RecipientData[]>> ListInvitationRecipients(string authToken, int invitationId, string recipientStatusFilter, string recipientIdFilter, int pageNumber, int pageSize)
        {
            try
            {
                var recipientList = InvitationManagementServiceImplementation.ListInvitationRecipients(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientStatusFilter, recipientIdFilter);

                var returnList = (pageNumber > 0 && pageSize > 0)
                    ? recipientList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                    : recipientList;

                return new ServiceOperationResult<PagedListResult<RecipientData[]>>
                {
                    CallSuccess = true,
                    ResultData = new PagedListResult<RecipientData[]> { ResultPage = returnList.ToArray(), TotalItemCount = recipientList.Count()}
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<RecipientData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<RecipientData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

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
        public ServiceOperationResult<object> RemoveRecipients(string authToken, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveRecipients(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientList);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Send the invitation to the recipients matching the specified filter.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to send.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        public ServiceOperationResult<object> SendInvitationToFilteredRecipientList(string authToken, int invitationId, string recipientFilter)
        {
            try
            {
                InvitationManagementServiceImplementation.SendInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId,  recipientFilter);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Send and invitation or a reminder to the recipients
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to send.</param>
        /// <param name="recipientEmails">Recipient emails</param>
        /// <param name="InvitationType">Type of the invitation: Invitation or a Reminder</param>
        /// <returns>Distionary with pairs: {Email, Result}</returns>
        public ServiceOperationResult<SimpleNameValueCollection> SendInvitationToRecipientList(string authToken, int invitationId, string recipientEmails, string InvitationType)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = new SimpleNameValueCollection(
                    InvitationManagementServiceImplementation.SendInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientEmails.Split(new char[]{','}), InvitationType))
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// List invitations for a survey
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

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
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListFilteredInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

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
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredUsersInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListFilteredUsersInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey</param>
        /// <returns>Count of invitations</returns>
        public ServiceOperationResult<int> GetInvitationCountForSurvey(string authToken, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetInvitationCountForSurvey(AuthenticationService.GetCurrentPrincipal(authToken), surveyId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey</param>
        /// <returns>Count of invitations</returns>
        public ServiceOperationResult<int> GetInvitationCountForSurveyByType(string authToken, int surveyId, bool isSent)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetInvitationCountForSurveyByType(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, isSent)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListSentInvitations(string authToken, int surveyId, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListSentInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageNumber, pageSize)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get opted out invitation details
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="email"> </param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="invitationId">ID of the invitation.</param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<OptedOutInvitationData> GetEmailOptOutDetails(string authTicket, string email, int responseTemplateId, int invitationId)
        {
            try
            {
                return new ServiceOperationResult<OptedOutInvitationData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetEmailOptOutDetails(AuthenticationService.GetCurrentPrincipal(authTicket), email, responseTemplateId, invitationId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<OptedOutInvitationData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<OptedOutInvitationData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List scheduled invitations for a survey
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListScheduledInvitations(string authToken, int surveyId, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListScheduledInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageNumber, pageSize)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion

        #region Email List Panels

        /// <summary>
        /// Get a list of email
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="permission">Permission to check for on the email lists.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>List of email list panel info objects for the panels.</returns>
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanels(string authToken, string permission, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailPanels(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, 0, null)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get a list of email
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>List of email list panel info objects for the panels.</returns>
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanelsByPeriod(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailPanels(AuthenticationService.GetCurrentPrincipal(authToken), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Create a new email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="name">The name of the new email list.</param>
        /// <param name="description">The description of the new email list.</param>
        public ServiceOperationResult<EmailListPanelData> CreateEmailListPanel(string authToken, string name, string description)
        {
            try
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CreateEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), name, description)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get a lightweight information object for the specified email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel to get.</param>
        public ServiceOperationResult<EmailListPanelData> GetEmailListPanel(string authToken, int emailListPanelId)
        {
            try
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetEmailListPanelInfo(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Create a copy of emailListPanel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">Id of emailListPanel which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID of the newly created emailListPanel.  If the value is negative, a it was not successfully created.</returns>
        public ServiceOperationResult<int> CopyEmailListPanel(string authToken, int emailListPanelId, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CopyEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, languageCode)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Persist changes to the email list panel info to the database.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="panelInfo">Information for the panel to update.</param>
        public ServiceOperationResult<object> UpdateEmailListPanel(string authToken, EmailListPanelData panelInfo)
        {
            try
            {
                InvitationManagementServiceImplementation.UpdateEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), panelInfo);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add the specified email addresses to an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the panel to add addresses to</param>
        /// <param name="emailAddresses">Addresses to add to the panel.</param>
        public ServiceOperationResult<object> AddEmailAddressesToEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailAddressesToEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, emailAddresses);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Remove the specified email addresses from an email list panel
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the panel to remove email addresses from.</param>
        /// <param name="emailAddresses">Email addresses to remove from the panel.</param>
        public ServiceOperationResult<object> RemoveEmailAddressesFromEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveEmailAddressesFromEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, emailAddresses);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List the email addresses contained in an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of email addresses in the panel.</returns>
        public ServiceOperationResult<PagedListResult<string[]>> ListEmailListPanelAddresses(string authToken, int emailListPanelId, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAddresses(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, pageNumber, pageSize)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get a list of permissions set on the default policy of an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel to get the default policy permissions for.</param>
        /// <returns>List of permissions on the email list panel's default policy.</returns>
        public ServiceOperationResult<string[]> ListEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Set the default policy permissions for an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel to set default policy permissions for.</param>
        /// <param name="permissions">Permissions to set on the default policy.</param>
        public ServiceOperationResult<object> SetEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.SetEmailListPanelDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List the ACL permissions that a given user has on an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user get the permissions list for.</param>
        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForUser(string authToken, int emailListPanelId, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, uniqueIdentifier)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List the ACL permissions that a given user group has on an email list panel.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="userGroupId">ID of the user group to list acl permissions for.</param>
        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForGroup(string authToken, int emailListPanelId, int userGroupId)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, userGroupId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Remove a user from an email list panel's access list.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from the access list.</param>
        public ServiceOperationResult<object> RemoveUserFromEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveFromEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, uniqueIdentifier);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Remove a user group from an email list panel's access list.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="userGroupId">ID of the user group to remove a user from.</param>

        public ServiceOperationResult<object> RemoveGroupFromEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveFromEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, userGroupId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add a user to an email list panel's access list with the specified permissions.  If the user is already
        /// on the access list, the user's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user.</param>
        public ServiceOperationResult<object> AddUserToEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.AddToEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, uniqueIdentifier, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add a user group to an email list panel's access list with the specified permissions.  If the user group is already
        /// on the access list, the user group's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="emailListPanelId">ID of the email list panel.</param>
        /// <param name="userGroupId">ID of the user group to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user group.</param>
        public ServiceOperationResult<object> AddGroupToEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.AddToEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId, userGroupId, permissions);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <returns></returns>
        public ServiceOperationResult<InvitationRecipientSummary> GetRecipientSummary(string authToken, int invitationId)
        {
            try
            {
                return new ServiceOperationResult<InvitationRecipientSummary>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetRecipientSummary(AuthenticationService.GetCurrentPrincipal(authToken), invitationId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<InvitationRecipientSummary>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<InvitationRecipientSummary>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseData[]> ListRecentResponses(string authToken, int invitationId, int count)
        {
            try
            {
                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListRecentResponsesForInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, count)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListResponses(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListResponsesForInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="emailListPanelId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteEmailListPanel(string authToken, int emailListPanelId)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteEmailListPanel(AuthenticationService.GetCurrentPrincipal(authToken), emailListPanelId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="provider"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserData[]>> ListAvailableUsersForInvitation(string authToken, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableUsersForInvitation(AuthenticationService.GetCurrentPrincipal(authToken), provider, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserGroupData[]>> ListAvailableUserGroupsForInvitation(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableGroupsForInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListAvailableEmailListsForInvitation(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableEmailListsForInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<InvitationData>[]> SearchInvitations(string authToken, string searchTerm)
        {

            try
            {
                return new ServiceOperationResult<GroupedResult<InvitationData>[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.SearchInvitations(AuthenticationService.GetCurrentPrincipal(authToken), searchTerm)

                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<GroupedResult<InvitationData>[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<InvitationData>[]>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientUserNames"></param>
        /// <param name="recipientEmailAddresses"></param>
        /// <param name="recipientGroupIds"></param>
        /// <param name="recipientEmailListIds"> </param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemovePendingRecipients(string authToken, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds)
        {
            try
            {
                InvitationManagementServiceImplementation.RemovePendingRecipientsFromInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientUserNames, recipientEmailAddresses, recipientGroupIds, recipientEmailListIds);
                
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MarkRecipientsOptedOut(string authToken, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.MarkRecipientsOptedOut(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientList);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MarkRecipientsResponded(string authToken, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.MarkRecipientsResponded(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientList);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false, 
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> GetInvitationSentCount(string authToken, int responseTemplateId)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.
                        GetInvitationSentCount(AuthenticationService.GetCurrentPrincipal(authToken), responseTemplateId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="surveyId">ID of the survey to list invitations for.</param>
        /// <param name="count">Number of invitations to list.</param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<InvitationData[]> ListRecentlySentInvitations(string authToken, int surveyId, int count)
        {
            try
            {
                return new ServiceOperationResult<InvitationData[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListRecentlySentInvitations(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, count)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<InvitationData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<InvitationData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion
    }

}
