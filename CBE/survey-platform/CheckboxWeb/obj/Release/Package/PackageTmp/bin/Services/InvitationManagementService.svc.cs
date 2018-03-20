using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;

using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// CheckboxWeb Invitation Management Web Service
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class InvitationManagementService : IInvitationManagementService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> AuthenticateUser(string userName, string password)
        {
            string ticket = InvitationManagementServiceImplementation.LoginAsCheckboxMessagingServiceAccount(userName, password);
            if (!string.IsNullOrEmpty(ticket))
                return new ServiceOperationResult<string>() { CallSuccess = true, ResultData = ticket };

            var service = new AuthenticationService();            

            return service.Login(userName, password);
        }

        #region Invitations

        /// <summary>
        /// Create a new invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateID">ID of the response template to create an invitation for.</param>
        /// <param name="name">Name of the creating invitation</param>
        /// <returns>Invitation info object representing the new invitation.</returns>
        public ServiceOperationResult<InvitationData> CreateInvitation(string authTicket, int responseTemplateID, string name)
        {
            try
            {
                return new ServiceOperationResult<InvitationData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CreateInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateID, name)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get information for.</param>
        /// <returns>Lightweight object containing invitation information.</returns>
        public ServiceOperationResult<InvitationData> GetInvitation(string authTicket, int invitationID)
        {
            try
            {
                return new ServiceOperationResult<InvitationData>
              {
                  CallSuccess = true,
                  ResultData = InvitationManagementServiceImplementation.GetLightWeightInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitation"></param>
        public ServiceOperationResult<object> UpdateInvitation(string authTicket, InvitationData invitation)
        {
            try
            {
                InvitationManagementServiceImplementation.UpdateInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitation);

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
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteInvitations(string authTicket, int[] invitationIds)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteInvitations(AuthenticationService.GetCurrentPrincipal(authTicket),
                                                                            invitationIds);

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
        /// Delete the specified invitation.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationId">ID of the invitation to delete.</param>
        public ServiceOperationResult<object> DeleteInvitation(string authTicket, int invitationId)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailAddresses">List of email addresses to add to the invitation.</param>
        public ServiceOperationResult<object> AddEmailAddressesToInvitation(string authTicket, int invitationID, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailAddressesToInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, emailAddresses);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        public ServiceOperationResult<object> AddUsersToInvitation(string authTicket, int invitationID, string[] userNames)
        {
            try
            {
                InvitationManagementServiceImplementation.AddUsersToInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, userNames);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        public ServiceOperationResult<object> GenerateUsersLinks(string authTicket, int surveyId, string[] userNames)
        {
            try
            {
                InvitationManagementServiceImplementation.GenerateUsersLinks(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, userNames);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailListPanelIDs">Ids of email list panels to add to the invitation.</param>
        public ServiceOperationResult<object> AddEmailListPanelsToInvitation(string authTicket, int invitationID, int[] emailListPanelIDs)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailListPanelsToInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, emailListPanelIDs);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to add a user group to.</param>
        /// <param name="userGroupIDs">IDs of the user groups to add to the invitation.</param>
        public ServiceOperationResult<object> AddUserGroupsToInvitation(string authTicket, int invitationID, int[] userGroupIDs)
        {
            try
            {
                InvitationManagementServiceImplementation.AddUserGroupsToInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, userGroupIDs);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="recipientStatusFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <param name="recipientIdFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of recipients.</returns>
        public ServiceOperationResult<PagedListResult<RecipientData[]>> ListInvitationRecipients(string authTicket, int invitationID, string recipientStatusFilter, string recipientIdFilter, int pageNumber, int pageSize)
        {
            try
            {
                var recipientList = InvitationManagementServiceImplementation.ListInvitationRecipients(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, recipientStatusFilter, recipientIdFilter);

                List<RecipientData> page = recipientList.ToList();

                if (pageNumber > 0 && pageSize > 0)
                {
                    //And here comes the tricky paging                    
                    page = (from r in recipientList where r.GroupId == null || !r.PanelMember select r).Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize).ToList();

                    //Now the page contains groups or standalone recipients. Let's insert group members after each group
                    for (int i = 0; i < page.Count; i++)
                    {
                        if (page[i].GroupId != null && !page[i].PanelMember)
                        {
                            page.InsertRange(i + 1, (from r in recipientList where r.GroupId == page[i].GroupId && r.PanelMember select r));
                        }
                    }
                }

                return new ServiceOperationResult<PagedListResult<RecipientData[]>>
                {
                    CallSuccess = true,
                    ResultData = new PagedListResult<RecipientData[]> { ResultPage = page.ToArray(), 
                        //now we take into account 
                        TotalItemCount = (from r in recipientList where r.GroupId == null || !r.PanelMember select r).Count() }
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
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ServiceOperationResult<RecipientData[]> ListRecentInvitationRecipients(string authTicket, int invitationID, int count)
        {
            try
            {
                return new ServiceOperationResult<RecipientData[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListRecentInvitationRecipients(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, count)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<RecipientData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<RecipientData[]>
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to send.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        public ServiceOperationResult<object> SendInvitationToFilteredRecipientList(string authTicket, int invitationID, string recipientFilter)
        {
            try
            {
                InvitationManagementServiceImplementation.SendInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, recipientFilter);

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
                    InvitationManagementServiceImplementation.SendInvitation(AuthenticationService.GetCurrentPrincipal(authToken), invitationId, recipientEmails.Split(new char[] { ',' }), InvitationType))
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
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
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListFilteredInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey)
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
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredUsersInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListFilteredUsersInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey</param>
        /// <returns>Count of invitations</returns>
        public ServiceOperationResult<int> GetInvitationCountForSurvey(string authTicket, int responseTemplateId)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetInvitationCountForSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey</param>
        /// <returns>Count of invitations</returns>
        public ServiceOperationResult<int> GetInvitationCountForSurveyByType(string authTicket, int responseTemplateId, bool isSent)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetInvitationCountForSurveyByType(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, isSent)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListSentInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListSentInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, pageSize)
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
        /// List scheduled invitations for a survey
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListScheduledInvitations(string authTicket, int responseTemplateId, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListScheduledInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, pageSize)
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
        /// Get opted out invitation details
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="email"> </param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="responseTemplateId">ID of the invitation.</param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<OptedOutInvitationData> GetEmailOptOutDetails(string authTicket, string email, int responseTemplateId, int invitationId)
        {
            try
            {
                return new ServiceOperationResult<OptedOutInvitationData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetEmailOptOutDetails(AuthenticationService.GetCurrentPrincipal(authTicket),email, responseTemplateId, invitationId)
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
        /// Returns a list of scheduled mailing activities for the invitation
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID">ID of the invitation to list schedule from.</param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of scheduled mailing activities for the invitation</returns>
        public ServiceOperationResult<PagedListResult<InvitationScheduleData[]>> ListInvitationSchedule(string authTicket, int invitationID, bool sortAscending, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<InvitationScheduleData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListInvitationSchedule(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, sortAscending, pageNumber, pageSize)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<InvitationScheduleData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<InvitationScheduleData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Deletes all selected schedule items by IDs
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="invitationID"></param>
        /// <param name="scheduleItemList">List of schedule item IDs to delete</param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteScheduleItems(string authTicket, int invitationID, int[] scheduleItemList)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.
                        DeleteScheduleItems(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, scheduleItemList)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Sets a required date for the invitation sending
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationID"></param>
        /// <param name="scheduledDate"></param>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> SetScheduledDate(string authTicket, int invitationID, int? scheduleID, string scheduledDate)
        {
            try
            {
                var scheduleItemID = InvitationManagementServiceImplementation.SetScheduledDate(AuthenticationService.GetCurrentPrincipal(authTicket), invitationID, scheduleID, scheduledDate);
                return new ServiceOperationResult<object>
                {
                    ResultData = scheduleItemID,
                    CallSuccess = true
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
        /// Returns a schedule status
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="invitationID"></param>
        /// <param name="scheduledDate"></param>
        /// <param name="scheduleID"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetScheduleStatus(string authTicket, int scheduleID)
        {
            try
            {                
                return new ServiceOperationResult<string>
                {
                    ResultData = InvitationManagementServiceImplementation.GetScheduleStatus(AuthenticationService.GetCurrentPrincipal(authTicket), scheduleID),
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Mail sending service requests mesages for the scheduled batch
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        public ServiceOperationResult<object> RequestBatchMessages(string authTicket, int scheduleID)
        {
            try
            {
                InvitationManagementServiceImplementation.RequestBatchMessages(authTicket, scheduleID);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
        /// Mail sending service requests mesages for the scheduled batch
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <param name="batchSize"> </param>
        public ServiceOperationResult<int> RequestBatchMessagesPartially(string authTicket, int scheduleID, int batchSize)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.RequestBatchMessages(authTicket, scheduleID, batchSize)
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
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <param name="Status"></param>
        /// <param name="ErrorText"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateBatchStatus(string authTicket, int scheduleID, string Status, string ErrorText)
        {
            try
            {
                InvitationManagementServiceImplementation.UpdateBatchStatus(authTicket, scheduleID, Status, ErrorText);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="count">Number of invitations to list.</param>
        /// <returns>List of invitation info objects.</returns>
        public ServiceOperationResult<InvitationData[]> ListRecentlySentInvitations(string authTicket, int responseTemplateId, int count)
        {
            try
            {
                return new ServiceOperationResult<InvitationData[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListRecentlySentInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, count)
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

        /// <summary>
        /// Reset 'ProcessingBatchId' column to NULL in ckbx_InvitationRecipients for requested invitation
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="scheduleID"> </param>
        /// <returns>.</returns>
        public ServiceOperationResult<object> ResetProcessingBatchForRecipients(string authTicket, int scheduleID)
        {
            try
            {
                InvitationManagementServiceImplementation.ResetProcessingBatchForRecipients(authTicket, scheduleID);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
        /// Returns batch id for the original invitation
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="scheduleID"> </param>
        /// <returns>returns -1 if the current shedule alredy is the original invitation</returns>
        public ServiceOperationResult<int> GetRelatedInvitationBatchId(string authTicket, int scheduleID)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.
                        GetRelatedInvitationBatchId(authTicket, scheduleID)
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

        #endregion

        #region Email List Panels

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
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanels(string authTicket, string permission, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailPanels(AuthenticationService.GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, 0, null)
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
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanelsByPeriod(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailPanels(AuthenticationService.GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="name">The name of the new email list.</param>
        /// <param name="description">The description of the new email list.</param>
        /// <returns></returns>
        public ServiceOperationResult<EmailListPanelData> CreateEmailListPanel(string authTicket, string name, string description)
        {
            try
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CreateEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), name, description)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get.</param>
        public ServiceOperationResult<EmailListPanelData> GetEmailListPanelInfo(string authTicket, int emailListPanelID)
        {
            try
            {
                return new ServiceOperationResult<EmailListPanelData>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetEmailListPanelInfo(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">Id of emailListPanel which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID of the newly created emailListPanel.  If the value is negative, a it was not successfully created.</returns>
        public ServiceOperationResult<int> CopyEmailListPanel(string authTicket, int emailListPanelID, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CopyEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, languageCode)
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
        /// Create a copies of emailListPanels.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelIdList">Id list of emailListPanels which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID list of the newly created emailListPanels.</returns>
        public ServiceOperationResult<int[]> CopyEmailListPanels(string authTicket, int[] emailListPanelIdList, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.CopyEmailListPanels(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelIdList, languageCode)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<int[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int[]>
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="panelInfo">Information for the panel to update.</param>
        public ServiceOperationResult<object> UpdateEmailListPanel(string authTicket, EmailListPanelData panelInfo)
        {
            try
            {
                InvitationManagementServiceImplementation.UpdateEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), panelInfo);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to add addresses to</param>
        /// <param name="emailAddresses">Addresses to add to the panel.</param>
        public ServiceOperationResult<object> AddEmailAddressesToEmailListPanel(string authTicket, int emailListPanelID, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.AddEmailAddressesToEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, emailAddresses);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to remove email addresses from.</param>
        /// <param name="emailAddresses">Email addresses to remove from the panel.</param>
        public ServiceOperationResult<object> RemoveEmailAddressesFromEmailListPanel(string authTicket, int emailListPanelID, string[] emailAddresses)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveEmailAddressesFromEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, emailAddresses);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of email addresses in the panel.</returns>
        public ServiceOperationResult<PagedListResult<string[]>> ListEmailListPanelAddresses(string authTicket, int emailListPanelID, int pageNumber, int pageSize)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<string[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAddresses(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, pageNumber, pageSize)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get the default policy permissions for.</param>
        /// <returns>List of permissions on the email list panel's default policy.</returns>
        public ServiceOperationResult<string[]> ListEmailListPanelDefaultPolicyPermissions(string authTicket, int emailListPanelID)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to set default policy permissions for.</param>
        /// <param name="permissions">Permissions to set on the default policy.</param>
        public ServiceOperationResult<object> SetEmailListPanelDefaultPolicyPermissions(string authTicket, int emailListPanelID, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.SetEmailListPanelDefaultPolicyPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, permissions);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user get the permissions list for.</param>
        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForUser(string authTicket, int emailListPanelID, string uniqueIdentifier)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, uniqueIdentifier)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to list acl permissions for.</param>
        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForGroup(string authTicket, int emailListPanelID, int userGroupID)
        {
            try
            {
                return new ServiceOperationResult<string[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListEmailListPanelAccessListPermissions(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, userGroupID)
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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from the access list.</param>
        public ServiceOperationResult<object> RemoveUserFromEmailListPanelAccessList(string authTicket, int emailListPanelID, string uniqueIdentifier)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveFromEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, uniqueIdentifier);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to remove a user from.</param>

        public ServiceOperationResult<object> RemoveGroupFromEmailListPanelAccessList(string authTicket, int emailListPanelID, int userGroupID)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveFromEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, userGroupID);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user.</param>
        public ServiceOperationResult<object> AddUserToEmailListPanelAccessList(string authTicket, int emailListPanelID, string uniqueIdentifier, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.AddToEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, uniqueIdentifier, permissions);

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
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user group.</param>
        public ServiceOperationResult<object> AddGroupToEmailListPanelAccessList(string authTicket, int emailListPanelID, int userGroupID, string[] permissions)
        {
            try
            {
                InvitationManagementServiceImplementation.AddToEmailListPanelAccessList(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID, userGroupID, permissions);

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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <returns></returns>
        public ServiceOperationResult<InvitationRecipientSummary> GetRecipientSummary(string authTicket, int invitationId)
        {
            try
            {
                return new ServiceOperationResult<InvitationRecipientSummary>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.GetRecipientSummary(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId)
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ServiceOperationResult<ResponseData[]> ListRecentResponses(string authTicket, int invitationId, int count)
        {
            try
            {
                return new ServiceOperationResult<ResponseData[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListRecentResponsesForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, count)
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListResponses(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ResponseData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListResponsesForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
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
        /// <param name="authTicket"></param>
        /// <param name="emailListPanelID"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteEmailListPanel(string authTicket, int emailListPanelID)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteEmailListPanel(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelID);

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
        /// <param name="authTicket"></param>
        /// <param name="emailListPanelIdList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteEmailListPanels(string authTicket, int[] emailListPanelIdList)
        {
            try
            {
                InvitationManagementServiceImplementation.DeleteEmailListPanels(AuthenticationService.GetCurrentPrincipal(authTicket), emailListPanelIdList);

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
        public ServiceOperationResult<PagedListResult<UserData[]>> ListAvailableUsersForInvitation(string authTicket, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableUsersForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), provider, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

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
        public ServiceOperationResult<PagedListResult<PageItemUserData[]>> ListAvailablePageItemUserDataForInvitation(string authTicket, string provider, int invitationId,
            int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailablePageItemUserDataForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), provider, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<PageItemUserData[]>>
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<UserGroupData[]>> ListAvailableUserGroupsForInvitation(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<UserGroupData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableGroupsForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListAvailableEmailListsForInvitation(string authTicket, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<EmailListPanelData[]>>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.ListAvailableEmailListsForInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)

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
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<InvitationData>[]> SearchInvitations(string authTicket, string searchTerm)
        {

            try
            {
                return new ServiceOperationResult<GroupedResult<InvitationData>[]>
                {
                    CallSuccess = true,
                    ResultData = InvitationManagementServiceImplementation.SearchInvitations(AuthenticationService.GetCurrentPrincipal(authTicket), searchTerm)

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
        /// Delete the specified recipients from an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to delete recipients from.</param>
        /// <param name="recipientList">IDs of the recipients to delete</param>
        public ServiceOperationResult<object> RemoveRecipients(string authTicket, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.RemoveRecipients(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, recipientList);
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientUserNames"></param>
        /// <param name="recipientEmailAddresses"></param>
        /// <param name="recipientGroupIds"></param>
        /// <param name="recipientEmailListIds"> </param>
        /// <returns></returns>
        public ServiceOperationResult<object> RemovePendingRecipients(string authTicket, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds)
        {
            try
            {
                InvitationManagementServiceImplementation.RemovePendingRecipientsFromInvitation(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, recipientUserNames, recipientEmailAddresses, recipientGroupIds, recipientEmailListIds);
                
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MarkRecipientsOptedOut(string authTicket, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.MarkRecipientsOptedOut(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, recipientList);
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
        /// <param name="authTicket"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MarkRecipientsResponded(string authTicket, int invitationId, long[] recipientList)
        {
            try
            {
                InvitationManagementServiceImplementation.MarkRecipientsResponded(AuthenticationService.GetCurrentPrincipal(authTicket), invitationId, recipientList);
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

        #endregion
       
    }

}
