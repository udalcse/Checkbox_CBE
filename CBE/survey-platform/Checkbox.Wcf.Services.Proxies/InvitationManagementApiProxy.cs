﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace Checkbox.Wcf.Services.Proxies
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public class InvitationManagementApiProxy : System.ServiceModel.ClientBase<IInvitationManagementApi>, IInvitationManagementApi
    {

        public InvitationManagementApiProxy()
        {
        }

        public InvitationManagementApiProxy(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public InvitationManagementApiProxy(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public InvitationManagementApiProxy(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public InvitationManagementApiProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public ServiceOperationResult<string> AuthenticateUser(string username, string password)
        {
            return Channel.AuthenticateUser(username, password);
        }

        public ServiceOperationResult<InvitationData> CreateInvitation(string authToken, int surveyId, string name)
        {
            return Channel.CreateInvitation(authToken, surveyId, name);
        }

        public ServiceOperationResult<InvitationData> GetInvitation(string authToken, int invitationID)
        {
            return Channel.GetInvitation(authToken, invitationID);
        }

        public ServiceOperationResult<object> UpdateInvitation(string authToken, InvitationData invitationData)
        {
            return Channel.UpdateInvitation(authToken, invitationData);
        }

        public ServiceOperationResult<object> DeleteInvitation(string authToken, int invitationId)
        {
            return Channel.DeleteInvitation(authToken, invitationId);
        }

        public ServiceOperationResult<object> AddEmailAddressesToInvitation(string authToken, int invitationId, string[] emailAddresses)
        {
            return Channel.AddEmailAddressesToInvitation(authToken, invitationId, emailAddresses);
        }

        public ServiceOperationResult<object> AddUsersToInvitation(string authToken, int invitationId, string[] usernames)
        {
            return Channel.AddUsersToInvitation(authToken, invitationId, usernames);
        }

        public ServiceOperationResult<object> GenerateUsersLinks(string authToken, int surveyId, string[] usernames)
        {
            return Channel.GenerateUsersLinks(authToken, surveyId, usernames);
        }

        public ServiceOperationResult<object> AddEmailListsToInvitation(string authToken, int invitationId, int[] emailListIds)
        {
            return Channel.AddEmailListsToInvitation(authToken, invitationId, emailListIds);
        }

        public ServiceOperationResult<object> AddUserGroupsToInvitation(string authToken, int invitationId, int[] userGroupIDs)
        {
            return Channel.AddUserGroupsToInvitation(authToken, invitationId, userGroupIDs);
        }

        public ServiceOperationResult<PagedListResult<RecipientData[]>> ListInvitationRecipients(string authToken, int invitationId, string recipientStatusFilter, string recipientNameFilter, int pageNumber, int pageSize)
        {
            return Channel.ListInvitationRecipients(authToken, invitationId, recipientStatusFilter, recipientNameFilter, pageNumber, pageSize);
        }

        public ServiceOperationResult<object> SendInvitationToFilteredRecipientList(string authToken, int invitationId, string recipientFilter)
        {
            return Channel.SendInvitationToFilteredRecipientList(authToken, invitationId, recipientFilter);
        }

        public ServiceOperationResult<SimpleNameValueCollection> SendInvitationToRecipientList(string authToken, int invitationId, string recipientEmails, string InvitationType)
        {
            return Channel.SendInvitationToRecipientList(authToken, invitationId, recipientEmails, InvitationType);
        }

        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListInvitations(authToken, surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            return Channel.ListFilteredInvitations(authToken, surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey);
        }

        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListFilteredUsersInvitations(string authToken, int surveyId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, string filterKey = null)
        {
            return Channel.ListFilteredUsersInvitations(authToken, surveyId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, filterKey);
        }

        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListSentInvitations(string authToken, int surveyId, int pageNumber, int pageSize)
        {
            return Channel.ListSentInvitations(authToken, surveyId, pageNumber, pageSize);
        }

        public ServiceOperationResult<PagedListResult<InvitationData[]>> ListScheduledInvitations(string authToken, int surveyId, int pageNumber, int pageSize)
        {
            return Channel.ListScheduledInvitations(authToken, surveyId, pageNumber, pageSize);
        }

        public ServiceOperationResult<OptedOutInvitationData> GetEmailOptOutDetails(string authTicket, string email, int responseTemplateId, int invitationId)
        {
            return Channel.GetEmailOptOutDetails(authTicket, email, responseTemplateId, invitationId);
        }

        public ServiceOperationResult<int> GetInvitationCountForSurvey(string authToken, int surveyId)
        {
            return Channel.GetInvitationCountForSurvey(authToken, surveyId);
        }

        public ServiceOperationResult<int> GetInvitationCountForSurveyByType(string authToken, int surveyId, bool isSent)
        {
            return Channel.GetInvitationCountForSurveyByType(authToken, surveyId, isSent);
        }

        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanels(string authToken, string permission, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListEmailPanels(authToken, permission, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListEmailPanelsByPeriod(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue, int period, string dateFieldName)
        {
            return Channel.ListEmailPanelsByPeriod(authToken, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue, period, dateFieldName);
        }

        public ServiceOperationResult<EmailListPanelData> CreateEmailListPanel(string authToken, string name, string description)
        {
            return Channel.CreateEmailListPanel(authToken, name, description);
        }

        public ServiceOperationResult<EmailListPanelData> GetEmailListPanel(string authToken, int emailListPanelId)
        {
            return Channel.GetEmailListPanel(authToken, emailListPanelId);
        }

        public ServiceOperationResult<int> CopyEmailListPanel(string authToken, int emailListPanelId, string languageCode)
        {
            return Channel.CopyEmailListPanel(authToken, emailListPanelId, languageCode);
        }

        public ServiceOperationResult<object> UpdateEmailListPanel(string authToken, EmailListPanelData panelInfo)
        {
            return Channel.UpdateEmailListPanel(authToken, panelInfo);
        }

        public ServiceOperationResult<object> AddEmailAddressesToEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses)
        {
            return Channel.AddEmailAddressesToEmailListPanel(authToken, emailListPanelId, emailAddresses);
        }

        public ServiceOperationResult<object> RemoveEmailAddressesFromEmailListPanel(string authToken, int emailListPanelId, string[] emailAddresses)
        {
            return Channel.RemoveEmailAddressesFromEmailListPanel(authToken, emailListPanelId, emailAddresses);
        }

        public ServiceOperationResult<PagedListResult<string[]>> ListEmailListPanelAddresses(string authToken, int emailListPanelId, int pageNumber, int pageSize)
        {
            return Channel.ListEmailListPanelAddresses(authToken, emailListPanelId, pageNumber, pageSize);
        }

        public ServiceOperationResult<string[]> ListEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId)
        {
            return Channel.ListEmailListPanelDefaultPolicyPermissions(authToken, emailListPanelId);
        }

        public ServiceOperationResult<object> SetEmailListPanelDefaultPolicyPermissions(string authToken, int emailListPanelId, string[] permissions)
        {
            return Channel.SetEmailListPanelDefaultPolicyPermissions(authToken, emailListPanelId, permissions);
        }

        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForUser(string authToken, int emailListPanelId, string uniqueIdentifier)
        {
            return Channel.ListEmailListPanelAccessListPermissionsForUser(authToken, emailListPanelId, uniqueIdentifier);
        }

        public ServiceOperationResult<string[]> ListEmailListPanelAccessListPermissionsForGroup(string authToken, int emailListPanelId, int userGroupId)
        {
            return Channel.ListEmailListPanelAccessListPermissionsForGroup(authToken, emailListPanelId, userGroupId);
        }

        public ServiceOperationResult<object> RemoveUserFromEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier)
        {
            return Channel.RemoveUserFromEmailListPanelAccessList(authToken, emailListPanelId, uniqueIdentifier);
        }

        public ServiceOperationResult<object> RemoveGroupFromEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId)
        {
            return Channel.RemoveGroupFromEmailListPanelAccessList(authToken, emailListPanelId, userGroupId);
        }

        public ServiceOperationResult<object> AddUserToEmailListPanelAccessList(string authToken, int emailListPanelId, string uniqueIdentifier, string[] permissions)
        {
            return Channel.AddUserToEmailListPanelAccessList(authToken, emailListPanelId, uniqueIdentifier, permissions);
        }

        public ServiceOperationResult<object> AddGroupToEmailListPanelAccessList(string authToken, int emailListPanelId, int userGroupId, string[] permissions)
        {
            return Channel.AddGroupToEmailListPanelAccessList(authToken, emailListPanelId, userGroupId, permissions);
        }

        public ServiceOperationResult<InvitationRecipientSummary> GetRecipientSummary(string authToken, int invitationId)
        {
            return Channel.GetRecipientSummary(authToken, invitationId);
        }

        public ServiceOperationResult<ResponseData[]> ListRecentResponses(string authToken, int invitationId, int count)
        {
            return Channel.ListRecentResponses(authToken, invitationId, count);
        }

        public ServiceOperationResult<PagedListResult<ResponseData[]>> ListResponses(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListResponses(authToken, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<object> DeleteEmailListPanel(string authToken, int emailListPanelId)
        {
            return Channel.DeleteEmailListPanel(authToken, emailListPanelId);
        }

        public ServiceOperationResult<PagedListResult<UserData[]>> ListAvailableUsersForInvitation(string authToken, string provider, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListAvailableUsersForInvitation(authToken, provider, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<PagedListResult<UserGroupData[]>> ListAvailableUserGroupsForInvitation(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListAvailableUserGroupsForInvitation(authToken, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<PagedListResult<EmailListPanelData[]>> ListAvailableEmailListsForInvitation(string authToken, int invitationId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            return Channel.ListAvailableEmailListsForInvitation(authToken, invitationId, pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);
        }

        public ServiceOperationResult<GroupedResult<InvitationData>[]> SearchInvitations(string authToken, string searchTerm)
        {
            return Channel.SearchInvitations(authToken, searchTerm);
        }

        public ServiceOperationResult<object> RemoveRecipients(string authToken, int invitationId, long[] recipientList)
        {
            return Channel.RemoveRecipients(authToken, invitationId, recipientList);
        }

        public ServiceOperationResult<object> RemovePendingRecipients(string authToken, int invitationId, string[] recipientUserNames, string[] recipientEmailAddresses, string[] recipientGroupIds, string[] recipientEmailListIds)
        {
            return Channel.RemovePendingRecipients(authToken, invitationId, recipientUserNames, recipientEmailAddresses, recipientGroupIds, recipientEmailListIds);
        }

        public ServiceOperationResult<object> MarkRecipientsOptedOut(string authToken, int invitationId, long[] recipientList)
        {
            return Channel.MarkRecipientsOptedOut(authToken, invitationId, recipientList);
        }

        public ServiceOperationResult<object> MarkRecipientsResponded(string authToken, int invitationId, long[] recipientList)
        {
            return Channel.MarkRecipientsResponded(authToken, invitationId, recipientList);
        }

        public ServiceOperationResult<int> GetInvitationSentCount(string authTicket, int responseTemplateId)
        {
            return Channel.GetInvitationSentCount(authTicket, responseTemplateId);
        }

        public ServiceOperationResult<InvitationData[]> ListRecentlySentInvitations(string authToken, int surveyId, int count)
        {
            return Channel.ListRecentlySentInvitations(authToken, surveyId, count);
        }
    }
}