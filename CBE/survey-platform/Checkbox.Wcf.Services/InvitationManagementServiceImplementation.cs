using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Security.Cryptography;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Messaging.Email;
using Checkbox.Pagination;
using Checkbox.Panels;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Checkbox.Management;
using System.Text;
using Checkbox.SystemMode;
using Checkbox.Timeline;
using Checkbox.Web;
using Newtonsoft.Json;

namespace Checkbox.Wcf.Services
{
    public static class InvitationManagementServiceImplementation
    {
        #region Invitations & Recipients

        /// <summary>
        /// Authorize access to a response template for invitation purposes.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to authorize user for.</param>
        private static void AuthorizeResponseTemplate(CheckboxPrincipal userPrincipal, int responseTemplateId)
        {
            //Authorize role
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            //Authorize response template
            LightweightAccessControllable rt = ResponseTemplateManager.GetLightweightResponseTemplate(responseTemplateId);

            if (rt != null && rt.ACL != null)
            {
                Security.AuthorizeUserContext(userPrincipal, rt, "Form.Administer");
            }
            else
            {
                throw new Exception("Unable to load survey with id: " + responseTemplateId);
            }
        }

        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static int GetInvitationCountForSurvey(CheckboxPrincipal userPrincipal, int responseTemplateId)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);
            return InvitationManager.GetInvitationCountForSurvey(userPrincipal, responseTemplateId);
        }


        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="isSent">Count sent invitations or scheduled</param>
        /// <returns></returns>
        public static int GetInvitationCountForSurveyByType(CheckboxPrincipal userPrincipal, int responseTemplateId, bool isSent)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);
            return InvitationManager.GetInvitationCountForSurveyByType(userPrincipal, responseTemplateId, isSent);
        }

        /// <summary>
        /// List invitations for a survey
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns>List of invitation info objects.</returns>
        public static PagedListResult<InvitationData[]> ListInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = CreatePaginationContext(filterField, filterValue, 
                sortField, sortAscending, pageNumber, pageSize);

            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            return new PagedListResult<InvitationData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = (from invitationId in invitationIds
                                select InvitationManager.GetInvitationData(invitationId) into invitation where invitation != null
                                  select ConvertDatesToClientTime(invitation)).ToArray()
            };
        }

        private static PaginationContext CreatePaginationContext(string filterField = "", string filterValue = "",
            string sortField = "", bool sortAscending = true, int pageNumber = -1, int pageSize = -1)
        {
            //check params
            switch (sortField)
            {
                case "Name":
                case "InvitationId":
                case "CreatedBy":
                case "LastSentOn":
                case "Scheduled":
                    break;
                default:
                    sortField = "InvitationId";
                    break;
            }

            return new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static PagedListResult<InvitationData[]> ListFilteredInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            //search without any specific survey
            if (responseTemplateId == 0)
            {
                int totalCount;

                var paginationContext = CreatePaginationContext(filterField, filterValue,
                    "InvitationId", sortAscending, pageNumber, pageSize);

                var res = InvitationManager.ListInvitations(userPrincipal, paginationContext, out totalCount);

                return new PagedListResult<InvitationData[]>
                {
                    TotalItemCount = totalCount,
                    ResultPage = res
                };
            }

            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);
            if (filterKey == null || filterKey == "all" || filterKey == String.Empty)
            {
                //with pagination
                return ListInvitations(userPrincipal, responseTemplateId, pageNumber, pageSize, sortField, sortAscending,
                                       filterField, filterValue);
            }
            switch (filterKey)
            {
                case "sent":
                    return ListSentInvitations(userPrincipal, responseTemplateId, pageNumber, pageSize, sortField, sortAscending);
                case "scheduled":
                    return ListScheduledSortedInvitations(userPrincipal, responseTemplateId, pageNumber, pageSize, sortField, sortAscending);
                default:
                    return ListDraftInvitations(userPrincipal, responseTemplateId, sortField, pageNumber, pageSize, sortAscending);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static PagedListResult<InvitationData[]> ListFilteredUsersInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            var invitationIds = InvitationManager.ListUsersInvitationForSurvey(userPrincipal, responseTemplateId, paginationContext);

            return new PagedListResult<InvitationData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = invitationIds.ToArray()
            };
        }
        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        ///  <param name="sortField"></param>
        ///  <param name="sortAscending"></param>
        /// <returns>List of invitation info objects.</returns>
        public static PagedListResult<InvitationData[]> ListSentInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize, string sortField=null, bool sortAscending=false)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            sortField = sortField ?? "LastSentOn";

            var paginationContext = CreatePaginationContext("", "", sortField, sortAscending, 1);

            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            var invitationsOnly = (from invitationId in invitationIds
                                   select InvitationManager.GetInvitationData(invitationId));

            InvitationData[] invitations;

            if (ApplicationManager.AppSettings.MSSMode.Equals("SES"))
            {
                var unordered = (from invitation in invitationsOnly
                                 let scheduleList = InvitationManager.GetScheduleByInvitationId(invitation.DatabaseId)
                                 from schedule in scheduleList
                                 where schedule.ProcessingStarted.HasValue && schedule.InvitationActivityType != InvitationActivityType.Test
                                 select  ConvertDatesToClientTime(invitation)).
                                Union((from invitation in invitationsOnly where InvitationManager.GetScheduleByInvitationId(invitation.DatabaseId).Count == 0 && invitation.LastSent.HasValue select ConvertDatesToClientTime(invitation)));
                invitations = unordered.ToArray();
            }
            else 
            {
                invitations = (from invitation in invitationsOnly where invitation.LastSent.HasValue select ConvertDatesToClientTime(invitation)).ToArray();
            }

            return new PagedListResult<InvitationData[]>
            {
                TotalItemCount = invitations.Count(),
                ResultPage =
                    invitations.Select((invitation, index) => new { Invitation = invitation, Index = index })
                .Where(i => i.Index >= pageSize * (pageNumber - 1) && i.Index < pageSize * pageNumber).Select(
                    i => i.Invitation).ToArray()
            };
        }

        /// <summary>
        /// List recently sent invitations for a survey
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="count"></param>
        /// <returns>List of invitation info objects.</returns>
        public static InvitationData[] ListRecentlySentInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int count)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = CreatePaginationContext("", "", "LastSentOn", false, 1);

            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            var invitationsOnly = (from invitationId in invitationIds
                                   select InvitationManager.GetInvitation(invitationId));

            if (ApplicationManager.AppSettings.MSSMode.Equals("SES"))
            {
                var unordered = (from invitation in invitationsOnly
                                 from schedule in invitation.Schedule
                                 where schedule.ProcessingStarted.HasValue
                                 select CreateInvitationInfo(invitation, schedule)).
                                Union((from invitation in invitationsOnly where invitation.Schedule.Count == 0 && invitation.LastSent.HasValue select CreateInvitationInfo(invitation)));
                return unordered.OrderByDescending(i => i.LastSent).
                                Take(count).ToArray();
            }
            
            return (from invitation in invitationsOnly select CreateInvitationInfo(invitation)).Take(count).ToArray();
        }

        /// <summary>
        /// List scheduled invitations for a survey
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of invitation info objects.</returns>
        public static PagedListResult<InvitationData[]> ListScheduledInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = CreatePaginationContext("", "", "LastSentOn", false, 1);
            
            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            var invitationsOnly = (from invitationId in invitationIds
                                   select InvitationManager.GetInvitationData(invitationId));
            var filtered = (from invitation in invitationsOnly
                            let scheduleList = InvitationManager.GetScheduleByInvitationId(invitation.DatabaseId)
                            from schedule in scheduleList
                    where !schedule.ProcessingStarted.HasValue && schedule.InvitationActivityType != InvitationActivityType.Test
                            select ConvertDatesToClientTime(invitation)).
                    Union((from invitation in invitationsOnly where !invitation.Scheduled.HasValue && !invitation.LastSent.HasValue select ConvertDatesToClientTime(invitation)));

            return new PagedListResult<InvitationData[]>
                       {
                           TotalItemCount = filtered.Count(),
                           ResultPage =
                               filtered.Select((invitation, index) => new {Invitation = invitation, Index = index})
                               .Where(i => i.Index >= pageSize*(pageNumber - 1) && i.Index < pageSize*pageNumber).Select
                               (i => i.Invitation).ToArray()
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="email"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static OptedOutInvitationData GetEmailOptOutDetails(CheckboxPrincipal userPrincipal, string email, int responseTemplateId, int invitationId)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            return InvitationManager.GetOptedOutEmailsDataBySurveyId(responseTemplateId, email, invitationId);
        }

        /// <summary>
        /// List of draft invitations (not scheduled, not sent or without recipients) for a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="sortField"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedListResult<InvitationData[]> ListDraftInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, string sortField, int pageNumber, int pageSize, bool sortAscending)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = CreatePaginationContext("", "", sortField, sortAscending, 1);

            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            var invitationsOnly = (from invitationId in invitationIds
                                   select InvitationManager.GetInvitationData(invitationId));
            var filtered = (from invitation in invitationsOnly
                            let scheduleList = InvitationManager.GetScheduleByInvitationId(invitation.DatabaseId)
                            where invitation.Scheduled == null && scheduleList.Count == 0 && InvitationManager.GetRecipientsCount(invitation.DatabaseId) == 0
                            select ConvertDatesToClientTime(invitation));

            return new PagedListResult<InvitationData[]>
            {
                TotalItemCount = filtered.Count(),
                ResultPage =
                    filtered.Select((invitation, index) => new { Invitation = invitation, Index = index })
                    .Where(i => i.Index >= pageSize * (pageNumber - 1) && i.Index < pageSize * pageNumber).Select
                    (i => i.Invitation).ToArray()
            };
        }

        /// <summary>
        /// List scheduled sorted invitations for a survey
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="responseTemplateId">ID of the survey to list invitations for.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns>List of invitation info objects.</returns>
        public static PagedListResult<InvitationData[]> ListScheduledSortedInvitations(CheckboxPrincipal userPrincipal, int responseTemplateId, int pageNumber, int pageSize, string sortField, bool sortAscending)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateId);

            var paginationContext = CreatePaginationContext("", "", sortField, sortAscending, 1);
            
            var invitationIds = InvitationManager.ListInvitationIDsForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext);

            var invitationsOnly = (from invitationId in invitationIds
                                   select InvitationManager.GetInvitationData(invitationId));
            var filtered = (from invitation in invitationsOnly
                            let scheduleList = InvitationManager.GetScheduleByInvitationId(invitation.DatabaseId)
                            from schedule in scheduleList
                            where
                                !schedule.ProcessingStarted.HasValue &&
                                schedule.InvitationActivityType != InvitationActivityType.Test
                            select ConvertDatesToClientTime(invitation));

            return new PagedListResult<InvitationData[]>
            {
                TotalItemCount = filtered.Count(),
                ResultPage =
                    filtered.Select((invitation, index) => new { Invitation = invitation, Index = index })
                    .Where(i => i.Index >= pageSize * (pageNumber - 1) && i.Index < pageSize * pageNumber).Select
                    (i => i.Invitation).ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationID"></param>
        /// <param name="sortAscending"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedListResult<InvitationScheduleData[]> ListInvitationSchedule(CheckboxPrincipal userPrincipal, int invitationID, bool sortAscending, int pageNumber, int pageSize)
        {
            var invitation = InvitationManager.GetInvitationData(invitationID);
            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID = {0} has not been found.", invitationID));

            AuthorizeResponseTemplate(userPrincipal, invitation.ResponseTemplateId);
            var res = new List<InvitationSchedule>(InvitationManager.GetScheduleByInvitationId(invitationID));

            var invitations = from s in res
                              where s.InvitationActivityType != InvitationActivityType.Test
                              select s;

            var count = invitations.Count();

            if (!invitations.Any(i => i.InvitationActivityType == InvitationActivityType.Invitation))
            {
                res.Insert(0, new InvitationSchedule
                                { 
                                    InvitationID = invitation.DatabaseId,
                                    InvitationActivityType = InvitationActivityType.Invitation,
                                    InvitationScheduleID = -1
                                });
                count++;
            }
            else if (!sortAscending)
                res.Reverse();

            if (pageNumber > 0 && pageSize > 0)
                res = res.Skip((pageNumber - 1)* pageSize).Take(pageSize).ToList();

            var resArray = (from schedule in res
                              where schedule.InvitationActivityType != InvitationActivityType.Test
                              select CreateInvitationScheduleInfo(schedule)).OrderBy(i => i.Scheduled).ToArray();

            //don't delete the first invitation at the schedule
            /*
            var notSentInvitations = (from s in resArray where s.CanBeDeleted && s.InvitationActivityType == "Invitation" select s).ToArray();
            var sentInvitations = (from s in resArray where !s.CanBeDeleted && s.InvitationActivityType == "Invitation" select s).ToArray();
            if (sentInvitations.Length == 0 && notSentInvitations.Length > 0)
                notSentInvitations[0].CanBeDeleted = false;                
            */

            return new PagedListResult<InvitationScheduleData[]>
            {
                ResultPage = resArray,
                TotalItemCount = count
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitationID"></param>
        /// <param name="scheduleItemList"></param>
        /// <param name="userPrincipal"></param>
        public static bool DeleteScheduleItems(CheckboxPrincipal userPrincipal, int invitationID, int[] scheduleItemList)
        {
            if (scheduleItemList == null || scheduleItemList.Length == 0)
            {
                throw new Exception("Nothing to delete.");
            }

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");
            var invitation = InvitationManager.GetInvitation(invitationID);
            
            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID = {0} has not been found.", invitationID));
            
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            var deleteInvitation = false;
            foreach (int t in scheduleItemList)
            {
                if (t == -1)
                    deleteInvitation = true;
                else
                {
                    var scheduleItem =
                        (from s in invitation.Schedule where s.InvitationScheduleID == t select s).FirstOrDefault();
                    //if (scheduleItem != null && scheduleItem.InvitationActivityType == InvitationActivityType.Invitation)
                    //    deleteInvitation = true;

                    invitation.DeleteScheduleItem(t);
                }
            }
            
            if (deleteInvitation)
                DeleteInvitation(userPrincipal, invitationID);
            else
                invitation.Save(userPrincipal);

            return deleteInvitation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheduleId"></param>
        /// <param name="authTicket"></param>
        public static void ResetProcessingBatchForRecipients(string authTicket, int sheduleId)
        {
            AuthenticateMessagingService(authTicket);

            InvitationManager.ResetProcessingBatchForRecipients(sheduleId);
        }

        /// <summary>
        /// Sets a required date for the invitation sending
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationID"></param>
        /// <param name="scheduleID"></param>
        /// <param name="scheduledDate"></param>
        public static int SetScheduledDate(CheckboxPrincipal userPrincipal, int invitationID, int? scheduleID, string scheduledDate)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");
            
            var invitation = InvitationManager.GetInvitation(invitationID);
            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID = {0} has not been found.", invitationID));
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            var dtScheduled = DateTime.Now;

            int res = 0;

            try
            {
                dtScheduled = DateTime.Parse(scheduledDate);
                dtScheduled = WebUtilities.ConvertFromClientToServerTimeZone(dtScheduled);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Cannot parse the string '{0}' as a date", scheduledDate), ex);
            }

            if (dtScheduled < DateTime.Now.AddMinutes(-1))
                dtScheduled = DateTime.Now.AddMinutes(1);

            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            if (scheduleID == null || scheduleID == -1)
            {
                //create a new schedule item
                var scheduleItem = new InvitationSchedule {Scheduled = dtScheduled};
                scheduleItem.InvitationID = invitation.ID;
                scheduleItem.InvitationActivityType = InvitationActivityType.Invitation;
                scheduleItem.Save(userPrincipal);

                res = scheduleItem.InvitationScheduleID.Value;

                invitation.AddScheduleItem(scheduleItem);
                InvitationSender.Send(invitation, scheduleItem, null, ApplicationManager.ApplicationDataContext);
            }
            else
            {
                var item = invitation.Schedule.FirstOrDefault(ss => ss.InvitationScheduleID == scheduleID);
                if (item.Scheduled < DateTime.Now.AddMinutes(1))
                    throw new Exception("Unable to change the schedule date of the invitation being sent.");
                item.Scheduled = dtScheduled;
                item.Save(userPrincipal);

                res = item.InvitationScheduleID.Value;

                if (item.BatchID.HasValue)
                {
                    EmailGateway.SetMessageBatchDate(item.BatchID.Value, item.Scheduled.Value);
                }
                else
                {
                    InvitationSender.Send(invitation, item, null, ApplicationManager.ApplicationDataContext); 
                }
            }

            return res;
        }

        /// <summary>
        /// Returns a schedule status
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="scheduleID"></param>
        public static string GetScheduleStatus(CheckboxPrincipal userPrincipal, int scheduleID)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");
            var scheduleItem = new InvitationSchedule {InvitationScheduleID = scheduleID};
            scheduleItem.Load();
            return scheduleItem.ErrorMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        /// <param name="batchSize"> </param>
        public static int RequestBatchMessages(string authTicket, int scheduleID, int? batchSize = null)
        {
            AuthenticateMessagingService(authTicket);

            return InvitationSender.RequestBatchMessages(scheduleID, batchSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="scheduleID"></param>
        public static void UpdateBatchStatus(string authTicket, int scheduleID, string Status, string ErrorText)
        {
            AuthenticateMessagingService(authTicket);

            InvitationSender.UpdateBatchStatus(scheduleID, Status, ErrorText);
        }

        /// <summary>
        /// Custom authentication for the Messaging Service
        /// </summary>
        public static string LoginAsCheckboxMessagingServiceAccount(string userName, string password)
        {
            InvitationSender.EnsureMessagingServiceCredentialsInitialized();
            if (userName.Equals(ApplicationManager.AppSettings.MessagingServiceUserName)
                &&
                password.Equals(ApplicationManager.AppSettings.MessagingServicePassword))
            {
                return Hash(string.Format("{0}-{1}-{2}", ApplicationManager.ApplicationDataContext, ApplicationManager.AppSettings.MessagingServiceUserName, ApplicationManager.AppSettings.MessagingServicePassword));
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        private static void AuthenticateMessagingService(string authTicket)
        {
            var correctTicket = Hash(string.Format("{0}-{1}-{2}", ApplicationManager.ApplicationDataContext, ApplicationManager.AppSettings.MessagingServiceUserName, ApplicationManager.AppSettings.MessagingServicePassword));
            if (!correctTicket.Equals(authTicket))
                throw new Exception("Bad authentication ticket");
        }
        
        /// <summary>
        /// Hashes the string with SHA1
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        private static string Hash(string res)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            var enc = new UTF8Encoding();
            var hashed = sha.ComputeHash(enc.GetBytes(res));
            var sb = new StringBuilder();
            foreach (byte b in hashed)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Create an invitation object based on the passed-in invitation.
        /// </summary>
        /// <returns><see cref="InvitationData"/> object.</returns>
        private static InvitationScheduleData CreateInvitationScheduleInfo(InvitationSchedule schedule)
        {
            var data = new InvitationScheduleData();

            if (schedule != null)
            {
                data.Scheduled = Web.WebUtilities.ConvertToClientTimeZone(schedule.Scheduled);
                data.InvitationActivityType = schedule.InvitationActivityType.ToString();
                data.ProcessingStarted = Web.WebUtilities.ConvertToClientTimeZone(schedule.ProcessingStarted);
                data.ProcessingFinished = Web.WebUtilities.ConvertToClientTimeZone(schedule.ProcessingFinished);
                data.CanBeDeleted = data.ProcessingStarted == null;
                data.InvitationScheduleID = schedule.InvitationScheduleID;
                data.InvitationID = schedule.InvitationID;
                data.ErrorMessage = schedule.ErrorMessage;
                data.BouncedEmails = schedule.BouncedEmails.ToArray();
                if (data.BouncedEmails.Length > 0)
                {
                    data.ErrorMessage = schedule.RecentBatchErrorText;
                }
            }

            return data;
        }

        /// <summary>
        /// Create a new invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the the calling user.</param>
        /// <param name="responseTemplateID">ID of the survey to create an invitation for.</param>
        /// <param name="name">Name of the creating invitation</param>
        /// <returns>Invitation info object representing the new invitation.</returns>
        public static InvitationData CreateInvitation(CheckboxPrincipal userPrincipal, int responseTemplateID, string name)
        {
            AuthorizeResponseTemplate(userPrincipal, responseTemplateID);

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = new Invitation
                                        {
                                            Name = name,
                                            ParentID = responseTemplateID,
                                        };

            invitation.Save(userPrincipal);
            invitation.Load();

            return CreateInvitationInfo(invitation);
        }

        /// <summary>
        /// Create an invitation object based on the passed-in invitation.
        /// </summary>
        /// <param name="invitation">Invitation to get information form.</param>
        /// <returns><see cref="InvitationData"/> object.</returns>
        private static InvitationData CreateInvitationInfo(Invitation invitation)
        {
            var data = new InvitationData();

            if (invitation != null)
            {
                data.Body = invitation.Template.Body;
                data.CreatedDate = Web.WebUtilities.ConvertToClientTimeZone(invitation.Created);
                data.CreatedBy = invitation.CreatedBy;
                data.DatabaseId = invitation.ID.Value;
                data.FromAddress = invitation.Template.FromAddress;
                data.FromName = invitation.Template.FromName;
                data.Guid = invitation.GUID;
                data.IncludeOptOut = invitation.Template.IncludeOptOutLink;
                data.LinkText = invitation.Template.LinkText;
                data.LoginOption = invitation.Template.LoginOption.ToString();
                data.MailFormat = invitation.Template.Format.ToString();
                data.OptOutText = invitation.Template.OptOutText;
                data.ResponseTemplateId = invitation.ParentID;
                data.Subject = invitation.Template.Subject;
                data.LastSent = Web.WebUtilities.ConvertToClientTimeZone(invitation.LastSent);
                data.Scheduled = Web.WebUtilities.ConvertToClientTimeZone(invitation.InvitationScheduled);
                data.InvitationActivityType = InvitationActivityType.Invitation.ToString();
                data.Name = invitation.Name;
                data.ErrorMessage = (from s in invitation.Schedule where s.ProcessingFinished.HasValue orderby s.ProcessingFinished descending select s.ErrorMessage).FirstOrDefault();
                data.SuccessfullySent = invitation.SuccessfullySent;
            }

            return data;
        }

        /// <summary>
        /// Create an invitation object based on the passed-in invitation.
        /// </summary>
        /// <param name="invitation">Invitation to get information form.</param>
        /// <returns><see cref="InvitationData"/> object.</returns>
        private static InvitationData CreateInvitationInfo(Invitation invitation, InvitationSchedule schedule)
        {
            var data = new InvitationData();

            if (invitation != null)
            {
                data.Body = invitation.Template.Body;
                data.CreatedDate = Web.WebUtilities.ConvertToClientTimeZone(invitation.Created);
                data.CreatedBy = invitation.CreatedBy;
                data.DatabaseId = invitation.ID.Value;
                data.FromAddress = invitation.Template.FromAddress;
                data.FromName = invitation.Template.FromName;
                data.Guid = invitation.GUID;
                data.IncludeOptOut = invitation.Template.IncludeOptOutLink;
                data.LinkText = invitation.Template.LinkText;
                data.LoginOption = invitation.Template.LoginOption.ToString();
                data.MailFormat = invitation.Template.Format.ToString();
                data.OptOutText = invitation.Template.OptOutText;
                data.ResponseTemplateId = invitation.ParentID;
                data.Subject = invitation.Template.Subject;
                data.ErrorMessage = invitation.ErrorMessage;
                data.SuccessfullySent = invitation.SuccessfullySent;
                if (schedule != null)
                {
                    data.LastSent = Web.WebUtilities.ConvertToClientTimeZone(schedule.ProcessingFinished);
                    data.Scheduled = Web.WebUtilities.ConvertToClientTimeZone(schedule.Scheduled);
                    data.ScheduleID = schedule.InvitationScheduleID;
                    data.InvitationActivityType = schedule.InvitationActivityType.ToString();
                    data.StatusDescription = schedule.ErrorMessage;
                    if (schedule.ProcessingStarted.HasValue)
                    {
                        if (schedule.ProcessingFinished.HasValue)
                            data.Status = "InvitationStatusCompleted";
                        else
                        {
                            data.StatusDescription = "Sending...";
                            data.Status = "InvitationStatusSending";
                        }
                    }

                }
                data.Name = invitation.Name;
            }

            return data;
        }

        /// <summary>
        /// Converts datetime properties of object with type T to client zone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static T ConvertDatesToClientTime<T> (T data)
        {
            foreach (var property in data.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(DateTime?) || property.PropertyType == typeof(DateTime))
                {
                    var newValue =  Web.WebUtilities.ConvertToClientTimeZone((DateTime?)data.GetType().GetProperty(property.Name).GetValue(data, null));
                    property.SetValue(data, newValue, null);
                }
            }
            return data;
        }

        /// <summary>
        /// Get an information object for the specified information.
        /// </summary>
        /// <param name="userPrincipal">User context of the the calling user.</param>
        /// <param name="invitationID">ID of the invitation to get information for.</param>
        /// <returns>Lightweight object containing invitation information.</returns>
        public static InvitationData GetInvitation(CheckboxPrincipal userPrincipal, int invitationID)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation != null)
            {
                AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);
                return CreateInvitationInfo(invitation);
            }

            return null;
        }

        /// <summary>
        /// Get an information object for the specified information.
        /// </summary>
        /// <param name="userPrincipal">User context of the the calling user.</param>
        /// <param name="invitationID">ID of the invitation to get information for.</param>
        /// <returns>Lightweight object containing invitation information.</returns>
        public static InvitationData GetLightWeightInvitation(CheckboxPrincipal userPrincipal, int invitationID)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitationData(invitationID);

            if (invitation != null)
            {
                AuthorizeResponseTemplate(userPrincipal, invitation.ResponseTemplateId);
                return invitation;
            }

            return null;
        }

        /// <summary>
        /// Returns batch id for the original invitation
        /// </summary>
        /// <param name="userPrincipal">User context of the the calling user.</param>
        /// <param name="scheduleID"> </param>
        /// <returns>returns -1 if the current shedule alredy is the original invitation</returns>
        public static int GetRelatedInvitationBatchId(string authTicket, int scheduleID)
        {
            AuthenticateMessagingService(authTicket);

            return InvitationManager.GetRelatedInvitationBatchId(scheduleID);
        }

        /// <summary>
        /// Get the count for the specified invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the the calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <returns>Lightweight object containing invitation information.</returns>
        public static int GetInvitationSentCount(CheckboxPrincipal userPrincipal, int responseTemplateId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var ids = InvitationManager.ListInvitationIDsForSurvey(userPrincipal, responseTemplateId, new PaginationContext());

            return (from id in ids select InvitationManager.GetInvitation(id) into inv where inv != null select inv.GetRecipients(RecipientFilter.All) into recipients select recipients.Sum(recipient => recipient.NumberOfMessagesSent)).Sum();
        }

        /// <summary>
        /// Persist changes to the specified invitation, excluding recipient changes.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationData"></param>
        public static void UpdateInvitation(CheckboxPrincipal userPrincipal, InvitationData invitationData)
        {
            AuthorizeResponseTemplate(userPrincipal, invitationData.ResponseTemplateId);

            var invitation = InvitationManager.GetInvitation(invitationData.DatabaseId);

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            //Update message properties
            invitation.Template.Body = invitationData.Body;
            invitation.Template.FromAddress = invitationData.FromAddress;
            invitation.Template.FromName = invitationData.FromName;
            invitation.Template.IncludeOptOutLink = invitationData.IncludeOptOut;
            invitation.Template.LinkText = invitationData.LinkText;
            invitation.Template.LoginOption = (LoginOption)Enum.Parse(typeof(LoginOption), invitationData.LoginOption);
            invitation.Template.Format = (MailFormat)Enum.Parse(typeof(MailFormat), invitationData.MailFormat);
            invitation.Template.OptOutText = invitationData.OptOutText;
            invitation.Template.Subject = invitationData.Subject;

            //Update name
            invitation.Name = invitationData.Name;

            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Delete the specified invitations
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationIds">Ids of the invitations to delete.</param>
        public static void DeleteInvitations(CheckboxPrincipal userPrincipal, int[] invitationIds)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            foreach (var id in invitationIds)
            {
                var invitation = InvitationManager.GetInvitation(id);

                if (invitation == null) continue;
                AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);
                InvitationManager.DeleteInvitation(id);
            }
        }

        /// <summary>
        /// Delete the specified invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">ID of the invitation to delete.</param>
        public static void DeleteInvitation(CheckboxPrincipal userPrincipal, int invitationID)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);
            InvitationManager.DeleteInvitation(invitationID);
        }

        /// <summary>
        /// Add the specified email addresses to the invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailAddresses">List of email addresses to add to the invitation.</param>
        public static void AddEmailAddressesToInvitation(CheckboxPrincipal userPrincipal, int invitationID, string[] emailAddresses)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID=[{0}] can not be found.", invitationID));
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            //trim all the addresses
            for (int i = 0; i < emailAddresses.Length; i++)
            {
                emailAddresses[i] = emailAddresses[i].Trim();
            }

            foreach (var emailAddress in from emailAddress in emailAddresses let validator = new Forms.Validation.EmailValidator() where !validator.Validate(emailAddress) select emailAddress)
            {
                throw new InvalidEmailAddressException(emailAddress);
            }

            invitation.AddPanel(new List<string>(emailAddresses));
            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Add the specified users to the invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        public static void AddUsersToInvitation(CheckboxPrincipal userPrincipal, int invitationID, string[] invitations)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID=[{0}] can not be found.", invitationID));
            
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);


            var userInvitations = invitations.Select(user => JsonConvert.DeserializeObject<UserInvitation>(user)).ToList();

            invitation.AddPanel(userInvitations);
            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Generate links for specified users.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="userNames">List of users to add to the invitation.</param>
        public static void GenerateUsersLinks(CheckboxPrincipal userPrincipal, int surveyId, string[] userNames)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var userEmails = userNames.Select(user => JsonConvert.DeserializeObject<UserInvitation>(user)).ToList();

            InvitationManager.GenerateLinksForUsers(userPrincipal, surveyId, userEmails);
            //invitation.AddPanel(userInvitations);
            //invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Add the specified email list panels to the invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">Id of the invitation.</param>
        /// <param name="emailListPanelIDs">Ids of email list panels to add to the invitation.</param>
        public static void AddEmailListPanelsToInvitation(CheckboxPrincipal userPrincipal, int invitationID, int[] emailListPanelIDs)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            foreach (var panel in emailListPanelIDs.Select(panelID => PanelManager.GetPanel(panelID)).OfType<EmailListPanel>())
            {
                try
                {
                    Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.View");
                    invitation.AddPanel(panel);
                }
                catch
                {
                }
            }

            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Add the specified user groups to the invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">ID of the invitation to add a user group to.</param>
        /// <param name="userGroupIDs">IDs of the user groups to add to the invitation.</param>
        public static void AddUserGroupsToInvitation(CheckboxPrincipal userPrincipal, int invitationID, int[] userGroupIDs)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID=[{0}] can not be found.", invitationID));

            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            foreach (var groupID in userGroupIDs)
            {
                var g = GroupManager.GetGroup(groupID);

                if (g == null)
                    throw new Exception(string.Format("User Group with ID=[{0}] can not be found.", groupID));

                Security.AuthorizeUserContext(userPrincipal, g, "Group.View");

                var usersWithNoAccess = new List<string>();
                var usersWithNoEmail = new List<string>();

                foreach (var userName in g.GetUserIdentifiers())
                {
                    var principal = UserManager.GetUserPrincipal(userName);
                    if (principal == null)
                        throw new UserDoesNotExistException(userName);

                    if (string.IsNullOrWhiteSpace(principal.Email))
                        usersWithNoEmail.Add(userName);

                    try { Security.AuthorizeUserContext(principal, "Form.Fill"); }
                    catch (ServiceAuthorizationException)
                    {
                        usersWithNoAccess.Add(userName);
                    }
                }

                if (usersWithNoAccess.Any())
                    throw new UserGroupMembersDoNotHavePermissionToTakeSurveyException(g.Name, usersWithNoAccess);
                if (usersWithNoEmail.Any())
                    throw new UserGroupMembersDoNotHaveEmailException(g.Name, usersWithNoEmail);

                invitation.AddPanel(g);
            }

            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Get a list of recipients for a given invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        /// <param name="recipientIdFilter"></param>
        /// <returns></returns>
        public static RecipientData[] ListInvitationRecipients(CheckboxPrincipal userPrincipal, int invitationID, string recipientFilter, string recipientIdFilter)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            var recipients = new List<RecipientData>();

            if (invitation != null)
            {
                AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

                var invitationRecipients = invitation.GetRecipients((RecipientFilter)Enum.Parse(typeof(RecipientFilter), recipientFilter), true, true).OrderBy(item => item.UniqueIdentifier);

                string formatText = TextManager.GetText("/pageText/forms/surveys/invitations/recipientReview.aspx/plusMoreRecipients");
                const int panelMembers = 10; 

                foreach (var recipient in invitationRecipients.Where(recipient => string.IsNullOrEmpty(recipientIdFilter) ||
                                                                                  ((!string.IsNullOrEmpty(recipient.UniqueIdentifier) && recipient.UniqueIdentifier.IndexOf(recipientIdFilter, StringComparison.InvariantCultureIgnoreCase) >= 0) ||
                                                                                   (!string.IsNullOrEmpty(recipient.EmailToAddress) && recipient.EmailToAddress.IndexOf(recipientIdFilter, StringComparison.InvariantCultureIgnoreCase) >= 0))))
                {

                    var info = CreateRecipientInfo(recipient, invitation.ParentID);
                    recipients.Add(info);

                    if ("Pending".Equals(recipientFilter)) //grouping by panel makes sense only! for create invitation mode
                    {
                        if (recipient.GroupID != null)
                        {
                            var panel = (GroupPanel)PanelManager.GetPanel(recipient.PanelID);
                            var panelists = panel.Panelists.Select(
                                    panelist => CreateRecipientInfo(new Recipient(panelist), invitation.ParentID)).Take(panelMembers);
                            var totalCount = panel.Panelists.Count;

                            foreach (var recipientInfo in panelists)
                            {
                                recipientInfo.PanelMember = true;
                                recipientInfo.GroupId = panel.GroupId;
                                recipients.Add(recipientInfo);
                            }

                            if (totalCount > panelMembers)
                            {
                                RecipientData recipientInfo = new RecipientData();
                                recipientInfo.EmailToAddress = string.Format(formatText, totalCount - 10);
                                recipientInfo.PanelMember = true;
                                recipientInfo.GroupId = panel.GroupId;
                                recipients.Add(recipientInfo);
                            }
                        }
                        if (recipient.PanelTypeId == 3)
                        {
                            var panel = (EmailListPanel)PanelManager.GetPanel(recipient.PanelID);
                            var panelists = panel.Panelists.Select(
                                    panelist => CreateRecipientInfo(new Recipient(panelist), invitation.ParentID)).Take(panelMembers);
                            var totalCount = panel.Panelists.Count;

                            foreach (var recipientInfo in panelists)
                            {
                                recipientInfo.PanelMember = true;
                                recipientInfo.EmailListId = panel.ID;
                                recipients.Add(recipientInfo);
                            }

                            if (totalCount > panelMembers)
                            {
                                RecipientData recipientInfo = new RecipientData();
                                recipientInfo.EmailToAddress = string.Format(formatText, totalCount - 10);
                                recipientInfo.PanelMember = true;
                                recipientInfo.EmailListId = panel.ID;
                                recipients.Add(recipientInfo);
                            }
                        }
                    }
                    else
                    {
                        //cleanup fields that force grouping
                        info.EmailListId = null;                            
                        info.GroupId = null;
                    }
                }

            }

            return recipients.ToArray();
        }

        /// <summary>
        /// Get a list of recent recipients for a given invitation.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">ID of the invitation to get recipients for.</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static RecipientData[] ListRecentInvitationRecipients(CheckboxPrincipal userPrincipal, int invitationID, int count)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            var recipients = new List<RecipientData>();

            if (invitation != null)
            {
                AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

                var invitationRecipients = invitation.GetRecipients(RecipientFilter.All, true, true)
                    .OrderByDescending(recipient => recipient.LastSent)
                    .Take(count)
                    .ToList();

                recipients.AddRange(invitationRecipients.Select(recipient => CreateRecipientInfo(recipient, invitation.ParentID)));
            }

            return recipients.ToArray();
        }

        /// <summary>
        /// Create a recipient info object based on the recipient.
        /// </summary>
        /// <param name="recipient">Recipient to return info object for.</param>
        /// <param name="surveyId"> </param>
        /// <returns>Recipient information object.</returns>
        private static RecipientData CreateRecipientInfo(Recipient recipient, int surveyId)
        {
            var info = new RecipientData();

            if (recipient != null)
            {
                info.DatabaseId = recipient.ID.HasValue ? recipient.ID.Value : -1;
                info.EmailToAddress = recipient.EmailToAddress;
                info.Error = recipient.Error;
                info.Guid = recipient.GUID;
                info.HasResponded = recipient.HasResponded;
                info.LastSent = Web.WebUtilities.ConvertToClientTimeZone(recipient.LastSent);
                info.Sent = recipient.LastSent.HasValue;
                info.SuccessfullySent = recipient.SuccessfullySent;
                info.UserName = recipient.UniqueIdentifier;
                info.GroupId = recipient.GroupID;
                info.MessageCount = recipient.NumberOfMessagesSent;
                info.OptedOut = recipient.OptedOut;
                info.Bounced = recipient.Bounced;
                info.ResponseTemplateId = surveyId;
                info.EmailListId = recipient.PanelTypeId == 3 && recipient.PanelID > 0 ? recipient.PanelID : default(int?);
            }

            return info;
        }

        /// <summary>
        /// Delete the specified recipients from an invitation.
        /// </summary>
        /// <param name="authToken">An encrypted forms auth token identifying the requesting user.</param>
        /// <param name="invitationId">ID of the invitation to delete recipients from.</param>
        /// <param name="recipientList">IDs of the recipients to delete</param>
        public static void RemoveRecipients(CheckboxPrincipal userPrincipal, int invitationID, long[] recipientList)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null)
                throw new Exception(string.Format("Invitation with ID=[{0}] can not be found.", invitationID));
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);
            invitation.RemoveRecipients(recipientList);
            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// Send the invitation to the recipients matching the specified filter.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="invitationID">ID of the invitation to send.</param>
        /// <param name="recipientFilter">Recipient filter.  Valid values are "Pending", "Deleted", "Current", "OptOut", "Responded", "NotResponded"</param>
        public static void SendInvitation(CheckboxPrincipal userPrincipal, int invitationID, string recipientFilter)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            var invitationRecipients = invitation.GetRecipients((RecipientFilter)Enum.Parse(typeof(RecipientFilter), recipientFilter));

            SendInvitationEmails(invitation, invitationRecipients, "Invitation");
        }


        /// <summary>
        /// Send and invitation or a reminder to the recipients
        /// </summary>
        /// <param name="checkboxPrincipal"></param>
        /// <param name="invitationId">ID of the invitation to send.</param>
        /// <param name="recipientEmails">Recipient emails</param>
        /// <param name="InvitationType">Type of the invitation: Invitation or a Reminder</param>
        public static Dictionary<string, string> SendInvitation(CheckboxPrincipal userPrincipal, int invitationID, string[] recipientEmails, string InvitationType)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationID);

            if (invitation == null) 
                return null;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            List<Recipient> invitationRecipients = null;
            invitationRecipients = invitation.GetRecipients(RecipientFilter.All).Where(r => recipientEmails.Contains(r.EmailToAddress)).ToList();
            invitationRecipients.AddRange(invitation.GetRecipients(RecipientFilter.PendingUngrouped).Where(r => recipientEmails.Contains(r.EmailToAddress)).ToList());

            return SendInvitationEmails(invitation, invitationRecipients, InvitationType);
        }


        /// <summary>
        /// Send invitation emails to the specified list of recipients.
        /// </summary>
        /// <param name="invitation">Invitation to send.</param>
        /// <param name="recipients">List of recipients to send invitation to.</param>
        private static Dictionary<string, string> SendInvitationEmails(Invitation invitation, IEnumerable<Recipient> recipients, string InvitationType)
        {
            var fieldNames = ProfileManager.ListPropertyNames();
            var results = new Dictionary<string, string>();

            Guid? surveyGuid = ResponseTemplateManager.GetResponseTemplateGUID(invitation.ParentID);
            var surveyBaseUrl = InvitationPipeMediator.GetBaseSurveyUrl(surveyGuid);

            var processedEmails = new Dictionary<string, Recipient>();

            //Now let's get to work sending the invitation.
            foreach (var recipient in recipients)
            {
                try
                {
                    if ("Invitation".Equals(InvitationType) && recipient.SuccessfullySent)
                    {
                        results[recipient.EmailToAddress] = "The invitation has been sent already";
                        continue;
                    }

                    if (processedEmails.ContainsKey(recipient.EmailToAddress))
                    {
                        //Store the date/time the attempt was made.
                        recipient.LastSent = processedEmails[recipient.EmailToAddress].LastSent;
                        recipient.SuccessfullySent = processedEmails[recipient.EmailToAddress].SuccessfullySent;

                        //Save the recipient so the success status, error message, and
                        // sending time are saved.
                        recipient.Save();
                        continue;
                    }

                    //Copy the template, which will be "personalized" by the recipient
                    var templateCopy = invitation.Template.Copy();                    

                    //Personalize the template, this includes adding the invitation id, any
                    // user profile properties, etc.  Fields in the invitation that can be
                    // customized are the body and the subject.  If the recipient is 
                    // a registered user, you can make the body or subject contain the value
                    // of a profile field by adding a @@[FIELD_NAME] token to the message or subject.
                    // For example, to add the user's name and password, you could put something like
                    // this in the message:
                    // 
                    // User Name: @@UserName
                    // Password: @@Password
                    //
                    recipient.PersonalizeTemplate(invitation, templateCopy, fieldNames, surveyBaseUrl, surveyGuid);

                    //Create and send a mail message.
                    var msg = new EmailMessage
                    {
                        To = recipient.EmailToAddress,
                        From =
                            string.IsNullOrEmpty(templateCopy.FromName)
                                ? templateCopy.FromAddress
                                : templateCopy.FromName + "<" + templateCopy.FromAddress + ">",
                        Body = "Reminder".Equals(InvitationType) ? templateCopy.ReminderBody : templateCopy.Body,
                        Subject = "Reminder".Equals(InvitationType) ? templateCopy.ReminderSubject : templateCopy.Subject,
                        Format = templateCopy.Format
                    };

                    //Send the message, which exception catching so the errors can be reported.
                    try
                    {
                        //If successful, mark the invitation as successfully sent to this
                        // recipient.
                        EmailGateway.Send(msg);
                        recipient.SuccessfullySent = true;
                        results[recipient.EmailToAddress] = "Success";
                    }
                    catch (Exception ex)
                    {
                        //If successful, mark the invitation as not successfully sent to this
                        // recipient and record the error, which will appear in the invitation
                        // management screens in the application.
                        recipient.SuccessfullySent = false;
                        recipient.Error = ex.Message;
                        results[recipient.EmailToAddress] = ex.Message + ex.InnerException == null ? "" : ("Inner Exception " + ex.InnerException.Message);
                    }

                    //Store the date/time the attempt was made.
                    recipient.LastSent = DateTime.Now;

                    //Save the recipient so the success status, error message, and
                    // sending time are saved.
                    recipient.Save();

                    processedEmails.Add(recipient.EmailToAddress, recipient);
                }
                catch (Exception ex)
                {
                    results[recipient.EmailToAddress] = ex.Message + ex.InnerException == null ? "" : ("Inner Exception " + ex.InnerException.Message);
                }
            }

            InvitationManager.UpdateInvitationSentDate(invitation.ID.Value);

            return results;
        }

        public static PagedListResult<PageItemUserData[]> ListAvailablePageItemUserDataForInvitation(
            CheckboxPrincipal userPrincipal, string provider, int invitationId, int pageNumber, int pageSize,
            string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Load invitation
            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null)
            {
                return null;
            }

            //Load securable template object
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(invitation.ParentID);

            if (lightweightRt == null)
            {
                return null;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Form.Administer");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            // AD optimization
            // TODO : rewrite pagination logic for other providersgit 
            if (UserManager.IsActiveDirectoryMembershipProvider(provider))
            {
                var userItemPageList = invitation.ListAvailablePageItemUserData(userPrincipal, paginationContext, provider);

                //List users
                return new PagedListResult<PageItemUserData[]>
                {
                    TotalItemCount = paginationContext.ItemCount,
                    ResultPage = userItemPageList
                };
            }

            var userNameList = invitation.ListAvailableUsers(userPrincipal, paginationContext, provider);

            var resultPage =
                userNameList.Select(
                    user =>
                        UserManagementServiceImplementation.GetUserData(userPrincipal, user.Key, user.Value)
                            .GetPageItemUserData()).ToList();

            //remove all users without emails 
            resultPage.RemoveAll(item => !item.AllEmails.Any());

            //List users
            return new PagedListResult<PageItemUserData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = resultPage.ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="provider"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        public static PagedListResult<UserData[]> ListAvailableUsersForInvitation(CheckboxPrincipal userPrincipal, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Load invitation
            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null)
            {
                return null;
            }

            //Load securable template object
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(invitation.ParentID);

            if (lightweightRt == null)
            {
                return null;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Form.Administer");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            var userNameList = invitation.ListAvailableUsers(userPrincipal, paginationContext, provider);

            //List users
            return new PagedListResult<UserData[]>
                       {
                           TotalItemCount = paginationContext.ItemCount,
                           ResultPage = userNameList.Select(user => UserManagementServiceImplementation.GetUserData(userPrincipal, user.Key, user.Value)).ToArray()
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        public static PagedListResult<UserGroupData[]> ListAvailableGroupsForInvitation(CheckboxPrincipal userPrincipal, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Load invitation
            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null)
            {
                return null;
            }

            //Load securable template object
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(invitation.ParentID);

            if (lightweightRt == null)
            {
                return null;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Form.Administer");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue,
                Permissions = new List<string> { "Group.View", "Group.Edit" },
                PermissionJoin = PermissionJoin.Any
            };

            IEnumerable<int> groupIdList = invitation.ListAvailableGroups(userPrincipal, paginationContext);

            //List users
            return new PagedListResult<UserGroupData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = groupIdList.Select(groupId => UserManagementServiceImplementation.GetUserGroupById(userPrincipal, groupId)).ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        public static PagedListResult<EmailListPanelData[]> ListAvailableEmailListsForInvitation(CheckboxPrincipal userPrincipal, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Load invitation
            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null)
            {
                return null;
            }

            //Load securable template object
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(invitation.ParentID);

            if (lightweightRt == null)
            {
                return null;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Form.Administer");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            IEnumerable<int> emailListPanelIdList = invitation.ListAvailableEmailLists(userPrincipal, paginationContext);

            //List users
            return new PagedListResult<EmailListPanelData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = emailListPanelIdList.Select(emailListPanelId => GetEmailListPanelInfo(userPrincipal, emailListPanelId)).ToArray()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<InvitationData>[] SearchInvitations(CheckboxPrincipal userPrincipal, string searchTerm)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");
            //name
            //createdby
            //recipent id
            //recipient guid

            var paginationContext = new PaginationContext { FilterValue = searchTerm, FilterField = "Name" };

            var resultsByName = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            paginationContext.FilterField = "CreatedBy";
            var resultsByOwner = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            paginationContext.FilterField = "InvitationId";
            var resultsById = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            paginationContext.FilterField = "RecipientId";
            var resultsByRecipientId = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            paginationContext.FilterField = "RecipientGuid";
            var resultsByRecipientGuid = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            paginationContext.FilterField = "RecipientEmail";
            var resultsByRecipientEmail = InvitationManager.ListAccessibleInvitationIds(userPrincipal, paginationContext);

            var results = new List<GroupedResult<InvitationData>>
                              {
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingName",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsByName
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      },
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingOwner",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsByOwner
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      },
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingId",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsById
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      },
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingRecipientId",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsByRecipientId
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      },
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingRecipientGuid",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsByRecipientGuid
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      },
                                  new GroupedResult<InvitationData>
                                      {
                                          GroupKey = "matchingRecipientEmail",
                                          GroupResults =
                                              (
                                                  from
                                                      invitationId in resultsByRecipientEmail
                                                  select
                                                      InvitationManager.GetInvitation(invitationId)
                                                  into
                                                      invitation
                                                  where
                                                      invitation != null
                                                  select CreateInvitationInfo(invitation)
                                              ).ToArray()
                                      }
                              };

            //By invitation name

            //By owner

            //By invitation id

            //By recipient id

            //By recipient guid

            //By recipient email

            //Return
            return results.ToArray();
        }


        #endregion

        #region Email List Panels

        /// <summary>
        /// Get a list of email
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <returns>List of email list panel info objects for the panels.</returns>
        public static PagedListResult<EmailListPanelData[]> ListEmailPanels(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.View");
            if (period == 0)
                dateFieldName = string.Empty;

            switch (filterField.ToLower())
            {
                case "name":
                case "description":
                case "datecreated":
                case "createdby":
                case "modifieddate":
                case "modifiedby":
                    break;
                default:
                    filterField = "Name";
                    break;
            }

            switch (sortField.ToLower())
            {
                case "name":
                case "description":
                case "datecreated":
                case "createdby":
                case "modifieddate":
                case "modifiedby":
                case "panelid":
                    break;
                default:
                    sortField = "Name";
                    break;
            }

            switch (dateFieldName.ToLower())
            {
                case "datecreated":
                case "modifieddate":
                    break;
                default:
                    dateFieldName = string.Empty;
                    break;
            }

            var paginationContext = new PaginationContext
            {
                Permissions = new List<string> { "EmailList.View" },
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue,
                DateFieldName = TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName),
                StartDate = TimelineManager.GetStartFilterDate(period)
            };

            var emailListPanelIds = EmailListManager.ListAvailableEmailLists(userPrincipal, paginationContext);

            return new PagedListResult<EmailListPanelData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = emailListPanelIds
                    .Select(PanelManager.GetPanel)
                    .OfType<EmailListPanel>()
                    .Select(CreateEmailListPanelInfo)
                    .ToArray()
            };
        }

        /// <summary>
        /// Create a panel info object for the specified email list panel.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        private static EmailListPanelData CreateEmailListPanelInfo(EmailListPanel panel)
        {
            var info = new EmailListPanelData();

            if (panel != null)
            {
                info.DatabaseId = panel.ID.Value;
                info.Description = panel.Description;
                info.Name = panel.Name;
                info.AddressCount = panel.Count;
            }

            return info;
        }

        /// <summary>
        /// Create an email panel
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="name">Name of the panel.</param>
        /// <param name="description">Description of the panel.</param>
        /// <returns>Email list panel information object.</returns>
        public static EmailListPanelData CreateEmailListPanel(CheckboxPrincipal userPrincipal, string name, string description)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Create");

            var panel = EmailListManager.CreateEmailList(name, description, userPrincipal);

            return CreateEmailListPanelInfo(panel);
        }

        /// <summary>
        /// Delete and email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to delete.</param>
        public static void DeleteEmailListPanel(CheckboxPrincipal userPrincipal, int emailListPanelID)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Delete");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Delete");

            panel.Delete();
        }

        /// <summary>
        /// Delete the specified email list panels
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelIdList">ID list of the email list panels to delete.</param>
        public static void DeleteEmailListPanels(CheckboxPrincipal userPrincipal, int[] emailListPanelIdList)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Delete");

            foreach (var emailListPanelId in emailListPanelIdList)
            {
                var panel = PanelManager.GetPanel(emailListPanelId) as EmailListPanel;

                if (panel == null)
                    throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelId));
                Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Delete");
                panel.Delete();
            }
        }

        /// <summary>
        /// Get a lightweight information object for the specified email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get.</param>
        /// <returns>Panel information object.</returns>
        public static EmailListPanelData GetEmailListPanelInfo(CheckboxPrincipal userPrincipal, int emailListPanelID)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.View");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
          
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.View");
            return CreateEmailListPanelInfo(panel);
        }


        /// <summary>
        /// Create a copy of emailListPanel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">Id of emailListPanel which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID of the newly created emailListPanel.  If the value is negative, a it was not successfully created.</returns>
        public static int CopyEmailListPanel(CheckboxPrincipal userPrincipal, int emailListPanelID, string languageCode)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Create");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));

            try
            {
                var newPanel = EmailListManager.CopyEmailList(panel, userPrincipal, languageCode);

                return newPanel.ID ?? -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>
        /// Create a copies of emailListPanels.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelIdList">Id list of emailListPanels which should be copied.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <returns>ID list of the newly created emailListPanels.</returns>
        public static int[] CopyEmailListPanels(CheckboxPrincipal userPrincipal, int[] emailListPanelIdList, string languageCode)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Create");
            var newIdList = new List<int>();
            try
            {
                newIdList.AddRange(from emailListPanelId in emailListPanelIdList select PanelManager.GetPanel(emailListPanelId) as EmailListPanel into panel select EmailListManager.CopyEmailList(panel, userPrincipal, languageCode) into newPanel where newPanel.ID.HasValue select newPanel.ID.Value);

                return newIdList.ToArray();
            }
            catch (Exception)
            {
                return newIdList.ToArray();
            }
        }


        /// <summary>
        /// Persist changes to the email list panel info to the database.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="panelInfo">Information for the panel to update.</param>
        public static void UpdateEmailListPanel(CheckboxPrincipal userPrincipal, EmailListPanelData panelInfo)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(panelInfo.DatabaseId) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", panelInfo.DatabaseId));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

            panel.Name = panelInfo.Name;
            panel.Description = panelInfo.Description;

            panel.Save(userPrincipal);
        }

        /// <summary>
        /// Add the specified email addresses to an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to add addresses to</param>
        /// <param name="emailAddresses">Addresses to add to the panel.</param>
        public static void AddEmailAddressesToEmailListPanel(CheckboxPrincipal userPrincipal, int emailListPanelID, string[] emailAddresses)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

            foreach (var emailAddress in emailAddresses)
            {
                panel.AddPanelist(emailAddress, string.Empty, string.Empty);
            }

            panel.Save(userPrincipal);
        }

        /// <summary>
        /// Remove the specified email addresses from an email list panel
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the panel to remove email addresses from.</param>
        /// <param name="emailAddresses">Email addresses to remove from the panel.</param>
        public static void RemoveEmailAddressesFromEmailListPanel(CheckboxPrincipal userPrincipal, int emailListPanelID, string[] emailAddresses)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

            panel.RemovePanelists(emailAddresses);

            panel.Save(userPrincipal);
        }

        /// <summary>
        /// List the email addresses contained in an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>List of email addresses in the panel.</returns>
        public static PagedListResult<string[]> ListEmailListPanelAddresses(CheckboxPrincipal userPrincipal, int emailListPanelID, int pageNumber, int pageSize)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.View");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            var emailAddresses = new List<string>();
            var totalItemCount = 0;

            if (panel != null)
            {
                Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.View");
                totalItemCount = panel.Panelists.Count;
                emailAddresses.AddRange(pageNumber <= 0
                                            ? panel.Panelists.Select(p => p.Email)
                                            : panel.Panelists.Skip((pageNumber - 1)*pageSize)
                                                   .Take(pageSize)
                                                   .Select(p => p.Email));
            }
            else
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));

            return new PagedListResult<string[]>
            {
                TotalItemCount = totalItemCount,
                ResultPage = emailAddresses.ToArray()
            };
        }

        /// <summary>
        /// Get a list of permissions set on the default policy of an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to get the default policy permissions for.</param>
        /// <returns>List of permissions on the email list panel's default policy.</returns>
        public static string[] ListEmailListPanelDefaultPolicyPermissions(CheckboxPrincipal userPrincipal, int emailListPanelID)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel != null)
            {
                Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

                if (panel.DefaultPolicy != null)
                {
                    return panel.DefaultPolicy.Permissions.ToArray();
                }
            }
            else
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));

            return new string[] { };

        }

        /// <summary>
        /// Set the default policy permissions for an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel to set default policy permissions for.</param>
        /// <param name="permissions">Permissions to set on the default policy.</param>
        public static void SetEmailListPanelDefaultPolicyPermissions(CheckboxPrincipal userPrincipal, int emailListPanelID, string[] permissions)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

            var editor = panel.GetEditor();

            if (editor == null) return;
            editor.Initialize(userPrincipal);
            editor.SetDefaultPolicy(panel.CreatePolicy(permissions));
        }

        /// <summary>
        /// List the ACL permissions that a given user has on an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user get the permissions list for.</param>
        public static string[] ListEmailListPanelAccessListPermissions(CheckboxPrincipal userPrincipal, int emailListPanelID, string uniqueIdentifier)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel != null)
            {
                Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

                if (UserManager.UserExists(uniqueIdentifier))
                {
                    
                    var principal = UserManager.GetUserPrincipal(uniqueIdentifier);

                    //if (principal == null) *** A principal is always return per the comment above
                    //    throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
                    //                                      uniqueIdentifier));

                    if (panel.ACL != null)
                    {
                        var p = panel.ACL.GetPolicy(principal);
                        if (p != null)
                            return p.Permissions.ToArray();
                    }
                }
                else
                    throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
                                                      uniqueIdentifier));
            }
            else
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));

            return new string[] { };
        }

        /// <summary>
        /// List the ACL permissions that a given user group has on an email list panel.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to list acl permissions for.</param>
        public static string[] ListEmailListPanelAccessListPermissions(CheckboxPrincipal userPrincipal, int emailListPanelID, int userGroupID)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel != null)
            {
                Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

                var g = GroupManager.GetGroup(userGroupID);

                if (g == null)
                    throw new Exception(string.Format("User Group with ID=[{0}] can not be found.", userGroupID));

                if (panel.ACL != null)
                {
                    var p = panel.ACL.GetPolicy(g);

                    if (p != null)
                    {
                        return p.Permissions.ToArray();
                    }
                }
            }
            else
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));

            return new string[] { };
        }

        /// <summary>
        /// Remove a user from an email list panel's access list.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to remove from the access list.</param>
        public static void RemoveFromEmailListPanelAccessList(CheckboxPrincipal userPrincipal, int emailListPanelID, string uniqueIdentifier)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            if (!UserManager.UserExists(uniqueIdentifier))
                throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
                                                  uniqueIdentifier));
            
            var principal = UserManager.GetUserPrincipal(uniqueIdentifier);

            //if (principal == null) *** A principal is always return per the comment above
            //    throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
            //                                      uniqueIdentifier));

            var editor = panel.GetEditor();
            editor.Initialize(userPrincipal);
            editor.RemoveAccess(principal);

            editor.SaveAcl();
        }

        /// <summary>
        /// Remove a user group from an email list panel's access list.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to remove a user from.</param>
        public static void RemoveFromEmailListPanelAccessList(CheckboxPrincipal userPrincipal, int emailListPanelID, int userGroupID)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            var g = GroupManager.GetGroup(userGroupID);

            if (g == null)
                throw new Exception(string.Format("User Group with ID=[{0}] can not be found.", userGroupID));

            var editor = panel.GetEditor();
            editor.Initialize(userPrincipal);
            editor.RemoveAccess(g);

            editor.SaveAcl();
        }

        /// <summary>
        /// Add a user to an email list panel's access list with the specified permissions.  If the user is already
        /// on the access list, the user's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="uniqueIdentifier">Unique identifier of the user to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user.</param>
        public static void AddToEmailListPanelAccessList(CheckboxPrincipal userPrincipal, int emailListPanelID, string uniqueIdentifier, string[] permissions)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            if (!UserManager.UserExists(uniqueIdentifier))
                throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
                                                  uniqueIdentifier));
          
            var principal = UserManager.GetUserPrincipal(uniqueIdentifier);

            //if (principal == null) *** A principal is always return per the comment above
            //    throw new Exception(string.Format("User with unique identifier '{0}' can not be found.",
            //                                      uniqueIdentifier));

            var editor = panel.GetEditor();
            editor.Initialize(userPrincipal);
            editor.ReplaceAccess(principal, permissions);

            editor.SaveAcl();
        }

        /// <summary>
        /// Add a user group to an email list panel's access list with the specified permissions.  If the user group is already
        /// on the access list, the user group's permissions will be updated to match the passed-in permissions.
        /// </summary>
        /// <param name="userPrincipal">User context of the calling user.</param>
        /// <param name="emailListPanelID">ID of the email list panel.</param>
        /// <param name="userGroupID">ID of the user group to add to the access list.</param>
        /// <param name="permissions">Permissions to set on the access list for the user group.</param>
        public static void AddToEmailListPanelAccessList(CheckboxPrincipal userPrincipal, int emailListPanelID, int userGroupID, string[] permissions)
        {
            Security.AuthorizeUserContext(userPrincipal, "EmailList.Edit");

            var panel = PanelManager.GetPanel(emailListPanelID) as EmailListPanel;

            if (panel == null)
                throw new Exception(string.Format("Email List with ID=[{0}] can not be found.", emailListPanelID));
            Security.AuthorizeUserContext(userPrincipal, panel, "EmailList.Edit");

            var g = GroupManager.GetGroup(userGroupID);

            if (g == null)
                throw new Exception(string.Format("User Group with ID=[{0}] can not be found.", userGroupID));

            var editor = panel.GetEditor();
            editor.Initialize(userPrincipal);
            editor.ReplaceAccess(g, permissions);

            editor.SaveAcl();
        }

        /// <summary>
        /// Get summary of recipients for invitation
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <returns></returns>
        public static InvitationRecipientSummary GetRecipientSummary(CheckboxPrincipal userPrincipal, int invitationId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null)
            {
                return null;
            }

            return invitation.GetRecipientSummary();

            /*
            return new InvitationRecipientSummary
            {
                PendingCount = invitation.GetPendingRecipientsCount(),
                CurrentCount = invitation.GetRecipients(RecipientFilter.Current, false).Count,
                NotRespondedCount = invitation.GetRecipients(RecipientFilter.NotResponded, false).Count,
                RespondedCount = invitation.GetRecipients(RecipientFilter.Responded, false).Count,
                OptedOutCount = invitation.GetRecipients(RecipientFilter.OptOut, false).Count,
                BouncedCount = invitation.GetRecipients(RecipientFilter.Bounced, false).Count
            };*/
        }

        /// <summary>
        /// Get recent responses to invitation
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ResponseData[] ListRecentResponsesForInvitation(CheckboxPrincipal userPrincipal, int invitationId, int count)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Responses.View");

            var responses = ResponseManager.GetResponseListForInvitation(
                invitationId,
                true,
                new PaginationContext
                    {
                        CurrentPage = 1,
                        PageSize = count,
                        SortField = "Started",
                        SortAscending = false
                    }
            );

            //Convert dateTimes to the client's time zone.
            foreach (var responseData in responses)
            {
                responseData.Started = Web.WebUtilities.ConvertToClientTimeZone(responseData.Started);
                responseData.LastEditDate = Web.WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
                responseData.CompletionDate = Web.WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);
            }

            return responses.ToArray();
        }

        /// <summary>
        /// Get recent responses to invitation
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="count"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static PagedListResult<ResponseData[]> ListResponsesForInvitation(CheckboxPrincipal userPrincipal, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Responses.View");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };


            var responseList = ResponseManager.GetResponseListForInvitation(
                invitationId,
                true,
               paginationContext
            );

            //Convert dateTimes to the client's time zone.
            foreach (var responseData in responseList)
            {
                responseData.Started = Web.WebUtilities.ConvertToClientTimeZone(responseData.Started);
                responseData.LastEditDate = Web.WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
                responseData.CompletionDate = Web.WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);
            }

            return new PagedListResult<ResponseData[]>
            {
                ResultPage = responseList.ToArray(),
                TotalItemCount = paginationContext.ItemCount
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientUserNames"></param>
        /// <param name="recipientEmailAddresses"></param>
        /// <param name="recipientGroupIds"></param>
        public static void RemovePendingRecipientsFromInvitation(CheckboxPrincipal userPrincipal, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds)
        {
            if (recipientUserNames.Length != recipientEmailAddresses.Length)
            {
                throw new Exception("Recipient User Name and Recipient Email Address arrays must have same length.");
            }

            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            foreach (var t in recipientUserNames.Where(t => !string.IsNullOrEmpty(t)))
            {
                invitation.RemoveUserPanelists(new[] { t });
            }

            for (var i = 0; i < recipientEmailAddresses.Length; i++)
            {
                if (!string.IsNullOrEmpty(recipientEmailAddresses[i]))
                    invitation.RemoveEmailPanelists(new[] { recipientEmailAddresses[i] });
            }

            foreach (var t in recipientGroupIds)
            {
                int id;
                if (int.TryParse(t, out id))
                    invitation.RemoveGroupPanel(id);
            }

            foreach (var t in recipientEmailListIds)
            {
                int id;
                if (int.TryParse(t, out id))
                    invitation.RemoveEmailListPanel(id);
            }
                
            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        public static void MarkRecipientsOptedOut(CheckboxPrincipal userPrincipal, int invitationId, long[] recipientList)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            var recipients = invitation.GetRecipients(recipientList);

            foreach (var recipient in recipients)
            {
                InvitationManager.OptOutRecipient(recipient, invitation.ParentID, InvitationOptOutType.BlockedByAdmin, string.Empty);
                invitation.OptOutRecipient(recipient.ID.Value);
            }

            invitation.Save(userPrincipal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="invitationId"></param>
        /// <param name="recipientList"></param>
        public static void MarkRecipientsResponded(CheckboxPrincipal userPrincipal, int invitationId, long[] recipientList)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Administer");

            var invitation = InvitationManager.GetInvitation(invitationId);

            if (invitation == null) return;
            AuthorizeResponseTemplate(userPrincipal, invitation.ParentID);

            Invitation.MarkRecipientsResponded(recipientList);

            invitation.Save(userPrincipal);
        }


        #endregion

    }
}
