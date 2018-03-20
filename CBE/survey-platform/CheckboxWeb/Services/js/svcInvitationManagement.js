/****************************************************************************
 * svcInvitationManagement.js                                               *
 *    Helper class and methods for accessing invitation management service. *
 ****************************************************************************/

//Instance of helper object
var svcInvitationManagement = new invitationManagementObj();

//Invitation management JS helper
function invitationManagementObj() {

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('InvitationManagementService.svc', 'json', operationName);
    };

    //Get invitation data
    this.getInvitation = function(authTicket, invitationId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetInvitation'),
            { authTicket: authTicket, invitationId: invitationId },
            callback,
            callbackArgs);
    };

    //Get recent invitations for survey
    this.listSentInvitationsForSurvey = function (authTicket, surveyId, pageNumber, pageSize, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListSentInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callback,
            callbackArgs);
    };

    //Get recent invitations for survey
    this.listSentInvitationsForSurveyD = function (authTicket, surveyId, pageNumber, pageSize, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('ListSentInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callbackArgs);
    };

    //Get recent invitations for survey
    this.listRecentlySentInvitationsForSurvey = function (authTicket, surveyId, count, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
        authTicket,
        this.getServiceUrl('ListRecentlySentInvitations'),
        { responseTemplateId: surveyId, authTicket: authTicket, count: count },
        callback,
        callbackArgs);
    };

    //Get recent invitations for survey
    this.listRecentlySentInvitationsForSurveyD = function (authTicket, surveyId, count, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('ListRecentlySentInvitations'),
            { responseTemplateId: surveyId, authTicket: authTicket, count: count },
            callbackArgs);
    };

    //Get invitations count for survey
    this.getInvitationSentCount = function (authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetInvitationSentCount'),
            { responseTemplateId: surveyId, authTicket: authTicket },
            callback,
            callbackArgs);
    };

    //Get invitations count for survey
    this.getInvitationSentCountD = function (authTicket, surveyId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('GetInvitationSentCount'),
            { responseTemplateId: surveyId, authTicket: authTicket },
            callbackArgs);
    };

    //Get scheduled invitations for survey
    this.listScheduledInvitationsForSurvey = function (authTicket, surveyId, pageNumber, pageSize, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListScheduledInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callback,
            callbackArgs);
    };

    //Get scheduled invitations for survey
    this.listScheduledInvitationsForSurveyD = function (authTicket, surveyId, pageNumber, pageSize, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('ListScheduledInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callbackArgs);
    };
    
    //Get opted out invitation details
    this.getEmailOptOutDetails = function (authTicket, email, surveyId, invitationId, callback, callbackArgs) {
        return serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetEmailOptOutDetails'),
            {
                authTicket: authTicket,
                email: email,
                responseTemplateId: surveyId,
                invitationId: invitationId
            },
            callback,
            callbackArgs);
    };

    this.listUsersInvitationsForSurvey = function (authTicket, surveyId, pagingArgs, callback, callbackArgs, filterKey) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListFilteredUsersInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending
            },
            callback,
            callbackArgs);
    };

    //Get page of invitations for survey
    this.listInvitationsForSurvey = function(authTicket, surveyId, pagingArgs, callback, callbackArgs, filterKey) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListFilteredInvitations'),
            {
                responseTemplateId: surveyId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                filterKey: filterKey
            },
            callback,
            callbackArgs);
    };


    //List recipients for invitation
    this.listInvitationRecipients = function(authTicket, invitationId, recipientStatusFilter, recipientIdFilter, pageNumber, pageSize, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListInvitationRecipients'),
            {
                authTicket: authTicket,
                invitationId: invitationId,
                recipientStatusFilter: recipientStatusFilter,
                recipientIdFilter: recipientIdFilter,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callback,
            callbackArgs);
    };

    //Get recipient summary data
    this.getRecipientSummary = function(authTicket, invitationId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetRecipientSummary'),
            {
                authTicket: authTicket,
                invitationId: invitationId
            },
            callback,
            callbackArgs);
    };

    //Get recent recipients
    this.listRecentInvitationRecipients = function(authTicket, invitationId, count, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListRecentInvitationRecipients'),
            {
                authTicket: authTicket,
                invitationId: invitationId,
                count: count
            },
            callback,
            callbackArgs);
    };

    //Get page of invitations for survey
    this.listInvitationResponses = function(authTicket, invitationId, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListResponses'),
            {
                invitationId: invitationId,
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue
            },
            callback,
            callbackArgs);
    };

    //Get invitation count for the specified survey
    this.getInvitationCountForSurvey = function(authTicket, surveyId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetInvitationCountForSurvey'),
            {
                authTicket: authTicket,
                responseTemplateId: surveyId
            },
            callback,
            callbackArgs);
    };

    //Get invitation count for the specified survey
    this.getInvitationCountForSurveyByType = function (authTicket, surveyId, isSent, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
        authTicket,
        this.getServiceUrl('GetInvitationCountForSurveyByType'),
        {
            authTicket: authTicket,
            responseTemplateId: surveyId,
            isSent: isSent
        },
        callback,
        callbackArgs);
    };


    //Get page of invitations for survey
    this.listRecentInvitationResponses = function(authTicket, invitationId, count, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListRecentResponses'),
            {
                invitationId: invitationId,
                authTicket: authTicket,
                count: count
            },
            callback,
            callbackArgs);
    };

    //List panels accessible to the user
    this.listViewableEmailPanels = function(authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListEmailPanelsByPeriod'),
            {
                authTicket: authTicket,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'EmailList.View',
                period: pagingArgs.period,
                dateFieldName: pagingArgs.dateFieldName
            },
            callback,
            callbackArgs
        );
    };

    //Get email list panel info
    this.getEmailListPanelInfo = function(authTicket, emailListPanelId, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('GetEmailListPanelInfo'),
            {
                authTicket: authTicket,
                emailListPanelId: emailListPanelId
            },
            callback,
            callbackArgs
        );
    };

    //Get email list panel info - DEFERRED
    this.getEmailListPanelInfoD = function(authTicket, emailListPanelId, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('GetEmailListPanelInfo'),
            {
                authTicket: authTicket,
                emailListPanelId: emailListPanelId
            },
            callbackArgs
        );
    };

    //Get email addresses list for the certain emailListPanel
    this.listEmailListPanelAddresses = function(authTicket, emailListPanelId, pageNumber, pageSize, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListEmailListPanelAddresses'),
            {
                authTicket: authTicket,
                emailListPanelId: emailListPanelId,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callback,
            callbackArgs
        );
    };

    //Get email addresses list for the certain emailListPanel
        this.listEmailListPanelAddressesD = function (authTicket, emailListPanelId, pageNumber, pageSize, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            this.getServiceUrl('ListEmailListPanelAddresses'),
            {
                authTicket: authTicket,
                emailListPanelId: emailListPanelId,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callbackArgs
        );
    };

    //Delete the specified emailListPanel
    this.deleteEmailListPanel = function(authTicket, emailListPanelId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteEmailListPanel'),
            {
                authTicket: authTicket,
                emailListPanelID: emailListPanelId
            },
            callback,
            callbackArgs
        );
    };

    //Copy the specified emailListPanel
    this.copyEmailListPanel = function(authTicket, emailListPanelId, languageCode, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('CopyEmailListPanel'),
            {
                authTicket: authTicket,
                emailListPanelID: emailListPanelId,
                languageCode: languageCode
            },
            callback,
            callbackArgs
        );
    };

    //Remove the specified email addresses from an email list panel
    this.removeEmailAddressesFromEmailListPanel = function(authTicket, emailListPanelID, emailAddresses, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl("RemoveEmailAddressesFromEmailListPanel"),
            {
                authTicket: authTicket,
                emailListPanelID: emailListPanelID,
                emailAddresses: emailAddresses
            },
            callback,
            callbackArgs
        );
    };

    //Delete the specified emailListPanels
    this.deleteEmailListPanels = function(authTicket, emailListPanelIdList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteEmailListPanels'),
            {
                authTicket: authTicket,
                emailListPanelIdList: emailListPanelIdList
            },
            callback,
            callbackArgs
        );
    };


    //Copy the specified emailListPanels
    this.copyEmailListPanels = function(authTicket, emailListPanelIdList, languageCode, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('CopyEmailListPanels'),
            {
                authTicket: authTicket,
                emailListPanelIdList: emailListPanelIdList,
                languageCode: languageCode
            },
            callback,
            callbackArgs
        );
    };

    //Delete array of invitations
    this.deleteInvitations = function(authTicket, invitationIds, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('DeleteInvitations'),
            {
                authTicket: authTicket,
                invitationIds: invitationIds
            },
            callback,
            callbackArgs
        );
    };

    //Delete specified invitation
    this.deleteInvitation = function(authTicket, invitationId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('DeleteInvitation'),
            {
                authTicket: authTicket,
                invitationId: invitationId
            },
            callback,
            callbackArgs
        );
    };

    //List available users to add to invitation
    this.listAvailablePageItemUserDataForInvitation = function (authTicket, provider, invitationId, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListAvailablePageItemUserDataForInvitation'),
            {
                authTicket: authTicket,
                provider: provider,
                invitationId: invitationId,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'Group.View'
            },
            callback,
            callbackArgs
        );
    };

    //List available users to add to invitation
    this.listAvailableUsersForInvitation = function(authTicket, provider, invitationId, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListAvailableUsersForInvitation'),
            {
                authTicket: authTicket,
                provider: provider,
                invitationId: invitationId,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'Group.View'
            },
            callback,
            callbackArgs
        );
    };


        //List available users to add to invitation
    this.listAvailableUserGroupsForInvitation = function(authTicket, invitationId, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListAvailableUserGroupsForInvitation'),
            {
                authTicket: authTicket,
                invitationId: invitationId,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'Group.View'
            },
            callback,
            callbackArgs
        );
    };

        //List available users to add to invitation
    this.listAvailableEmailListsForInvitation = function(authTicket, invitationId, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListAvailableEmailListsForInvitation'),
            {
                authTicket: authTicket,
                invitationId: invitationId,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'Group.View'
            },
            callback,
            callbackArgs
        );
    };

    this.listAvailableEmailListsForInvitation = function (authTicket, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListAvailableEmailListsForInvitation'),
            {
                authTicket: authTicket,
                invitationId: invitationId,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                permission: 'Group.View'
            },
            callback,
            callbackArgs
        );
    };


    //Add users to invitation
    this.addUsersToInvitation = function(authTicket, invitationId, userNamesArray, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('AddUsersToInvitation'),
            {
                authTicket: authTicket,
                invitationID: invitationId,
                userNames: userNamesArray
            },
            callback,
            callbackArgs);
    };

    //Add users to invitation
    this.generateUsersLinks = function (authTicket, userNamesArray, surveyId, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('GenerateUsersLinks'),
            {
                authTicket: authTicket,
                surveyId : surveyId,
                userNames: userNamesArray
            },
            callback,
            callbackArgs);
    };


        //Add users to invitation
    this.addGroupsToInvitation = function(authTicket, invitationId, userGroupsArray, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('AddUserGroupsToInvitation'),
            {
                authTicket: authTicket,
                invitationID: invitationId,
                userGroupIDs: userGroupsArray
            },
            callback,
            callbackArgs);
    };

        //Add users to invitation
    this.addEmailListsToInvitation = function(authTicket, invitationId, emailListsArray, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('AddEmailListPanelsToInvitation'),
            {
                authTicket: authTicket,
                invitationID: invitationId,
                emailListPanelIDs: emailListsArray
            },
            callback,
            callbackArgs);
    };

        //Add email addresses to invitation
    this.addEmailAddressesToInvitation = function(authTicket, invitationId, emailAddressArray, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            this.getServiceUrl('AddEmailAddressesToInvitation'),
            {
                authTicket: authTicket,
                invitationID: invitationId,
                emailAddresses: emailAddressArray
            },
            callback,
            callbackArgs);
    };

            //Search invitations
    this.searchInvitations = function(authTicket, searchTerm, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('SearchInvitations'),
            { authTicket: authTicket, searchTerm: searchTerm },
            callback,
            callbackArgs);
    };

        //Remove recipients from invitation
    this.removeRecipients = function(authTicket, invitationId, recipientList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('RemoveRecipients'),
            { authTicket: authTicket, invitationId: invitationId, recipientList: recipientList },
            callback,
            callbackArgs
            );
    };

        //Remove recipients from invitation
    this.removePendingRecipients = function (authTicket, invitationId, recipientUserNameList, recipientEmailList, groupsIdList, emailPanelList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('RemovePendingRecipients'),
            { authTicket: authTicket, invitationId: invitationId, recipientUserNames: recipientUserNameList, recipientEmailAddresses: recipientEmailList, recipientGroupIds: groupsIdList, recipientEmailListIds: emailPanelList },
            callback,
            callbackArgs
            );
    };

        //Mark recipients as "opted out"
    this.markRecipientsOptedOut = function(authTicket, invitationId, recipientList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('MarkRecipientsOptedOut'),
            { authTicket: authTicket, invitationId: invitationId, recipientList: recipientList },
            callback,
            callbackArgs
            );
    };


        //Mark recipients as "responded"
    this.markRecipientsResponded = function(authTicket, invitationId, recipientList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('MarkRecipientsResponded'),
            { authTicket: authTicket, invitationId: invitationId, recipientList: recipientList },
            callback,
            callbackArgs
            );
    };

    //List recipients for invitation
    this.listInvitationSchedule = function (authTicket, invitationId, sortAscending, pageNumber, pageSize, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            this.getServiceUrl('ListInvitationSchedule'),
            {
                authTicket: authTicket,
                invitationID: invitationId,
                sortAscending: sortAscending,
                pageNumber: pageNumber,
                pageSize: pageSize
            },
            callback,
            callbackArgs);
    };

    //Remove scheduled items from the invitation
    this.deleteScheduleItems = function (authTicket, invitationId, scheduleItemList, callback, callbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('DeleteScheduleItems'),
            { authTicket: authTicket, invitationID: invitationId, scheduleItemList: scheduleItemList },
            callback,
            callbackArgs
            );
    };

    //Sets scheduled date for the invitation
    this.setScheduledDate = function (authTicket, invitationID, scheduleID, scheduledDate, callback, callbackArgs, serviceErrorCallback, errorCallbackArgs) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('SetScheduledDate'),
            { authTicket: authTicket, invitationID: invitationID, scheduleID: scheduleID, scheduledDate: scheduledDate },
            callback,
            callbackArgs,
            serviceErrorCallback,
            errorCallbackArgs);
    };

    //Sets scheduled date for the invitation
    this.getScheduleStatus = function (authTicket, scheduleID, callback) {
        serviceHelper.makeServicePostCall(
            authTicket,
            svcInvitationManagement.getServiceUrl('GetScheduleStatus'),
            { authTicket: authTicket, scheduleID: scheduleID},
            callback);
    };
}

