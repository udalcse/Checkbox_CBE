/****************************************************************************
* svcUserManagement.js                                                      *
*    Helper class and methods for accessing user management service.        *
****************************************************************************/

//Instance of helper object
var svcUserManagement = new userManagementObj();

//Survey management JS helper
function userManagementObj() {

    //Get service url
    this.getServiceUrl = function (operationName) {
        return serviceHelper.getServiceUrl('UserManagementService.svc', 'json', operationName);
    }

    //Unlock the user
    this.getUserData = function (authTicket, uniqueIdentifier, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
           svcUserManagement.getServiceUrl('GetUserData'),
            {
                authToken: authTicket,
                uniqueIdentifier: uniqueIdentifier
            },
            callback,
            callbackArgs);
    }

    //GetPageItemUsersData
    this.getPageItemUsersData = function (authTicket, provider, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
           svcUserManagement.getServiceUrl('GetPageItemUsersData'),
            {
                authToken: authTicket,
                provider: provider,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue
            },
            callback,
            callbackArgs);
    }

    //List users
    this.getUsers = function (authTicket, provider, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,            
           svcUserManagement.getServiceUrl('GetUsersByPeriod'),
            {
                authToken: authTicket,
                provider: provider,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                period: pagingArgs.period,
                dateFieldName:  pagingArgs.dateFieldName
            },
            callback,
            callbackArgs);
        }

    this.getTenantUsers = function (authTicket, provider, pagingArgs, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
           svcUserManagement.getServiceUrl('GetUsersTenantByPeriod'),
            {
                authToken: authTicket,
                provider: provider,
                pageNumber: pagingArgs.pageNumber,
                pageSize: pagingArgs.pageSize,
                sortField: pagingArgs.sortField,
                sortAscending: pagingArgs.sortAscending,
                filterField: pagingArgs.filterField,
                filterValue: pagingArgs.filterValue,
                period: pagingArgs.period,
                dateFieldName: pagingArgs.dateFieldName
            },
            callback,
            callbackArgs);
    }

        //List surveys & folders contained in specified folder
        this.listUserGroups = function (authTicket, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
               svcUserManagement.getServiceUrl('ListUserGroupsByPeriod'),
                {
                    authToken: authTicket,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue,
                    period: pagingArgs.period,
                    dateFieldName:  pagingArgs.dateFieldName
                },
                callback,
                callbackArgs
            );
        }

        //Get user group data by id
        this.getUserGroupById = function (authTicket, groupId, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
               svcUserManagement.getServiceUrl('GetUserGroupById'),
                {authToken: authTicket, groupId: groupId},
                callback,
                callbackArgs
            );
        }

        //Get user group data by id - DEFERRED
        this.getUserGroupByIdD = function (authTicket, groupId, callbackArgs) {
            return serviceHelper.makeServiceCallD(
                authTicket,
               svcUserManagement.getServiceUrl('GetUserGroupById'),
                { authToken: authTicket, groupId: groupId },
                callbackArgs
            );
        }

        //Copy group
        this.copyGroup = function (authTicket, groupId, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
               svcUserManagement.getServiceUrl('CopyGroup'),
                { authToken: authTicket, groupId: groupId },
                callback,
                callbackArgs
            );
        }

        //List surveys & folders contained in specified folder
        this.listUserGroupMembers = function (authTicket, groupId, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
               svcUserManagement.getServiceUrl('ListUserGroupMembers'),
                {
                    authToken: authTicket,
                    groupId: groupId,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue
                },
                callback,
                callbackArgs
            );
            }

            //List surveys & folders contained in specified folder - DEFERRED
            this.listUserGroupMembersD = function (authTicket, groupId, pagingArgs, callbackArgs) {
                return serviceHelper.makeServiceCallD(
                    authTicket,
                   svcUserManagement.getServiceUrl('ListUserGroupMembers'),
                    {
                        authToken: authTicket,
                        groupId: groupId,
                        pageNumber: pagingArgs.pageNumber,
                        pageSize: pagingArgs.pageSize,
                        sortField: pagingArgs.sortField,
                        sortAscending: pagingArgs.sortAscending,
                        filterField: pagingArgs.filterField,
                        filterValue: pagingArgs.filterValue
                    },
                    callbackArgs
                );
            }
            
        //List surveys & folders contained in specified folder
        this.listUsersNotInGroup = function (authTicket, groupId, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListUsersNotInGroup'),
                {
                    authToken: authTicket,
                    groupId: groupId,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue
                },
                callback,
                callbackArgs
            );
        }

        //List surveys & folders contained in specified folder
        this.listPageItemUserDataForGroup = function (authTicket, provider, groupId, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListPageItemUserDataForGroup'),
                {
                    authToken: authTicket,
                    groupId: groupId,
                    provider: provider,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue
                },
                callback,
                callbackArgs
            );
        }

        this.getPotentialUsersForNewGroup = function (authTicket, provider, pagingArgs, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListPotentialUsersForNewGroupByProvider'),
                {
                    authToken: authTicket,
                    provider: provider,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue
                },
                callback,
                callbackArgs
            );
        }

        this.getCurrentUsersForNewGroup = function (authTicket, pagingArgs, callback, callbackArgs) {
            
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListCurrentUsersForNewGroup'),
                {
                    authToken: authTicket,
                    pageNumber: pagingArgs.pageNumber,
                    pageSize: pagingArgs.pageSize,
                    sortField: pagingArgs.sortField,
                    sortAscending: pagingArgs.sortAscending,
                    filterField: pagingArgs.filterField,
                    filterValue: pagingArgs.filterValue
                },
                callback,
                callbackArgs
            );
        }

        this.AddToUsersForNewGroup = function (authTicket, userId, callback, callbackArgs) {
            
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListCurrentUsersForNewGroupAddUser'),
                {
                    authToken: authTicket,
                    userId: userId
                },
                callback,
                callbackArgs
            );
        }

        this.RemoveFromUsersForNewGroup = function (authTicket, userId, callback, callbackArgs) {
            
            serviceHelper.makeServiceCall(
                authTicket,
                svcUserManagement.getServiceUrl('ListCurrentUsersForNewGroupRemoveUser'),
                {
                    authToken: authTicket,
                    userId: userId
                },
                callback,
                callbackArgs
            );
        }

        //Unlock a user
        this.unlockUser = function (authTicket, uniqueIdentifier, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
               authTicket,
               svcUserManagement.getServiceUrl('UnlockUser'),
                {
                    authToken: authTicket,
                    uniqueIdentifier: uniqueIdentifier
                },
                callback,
                callbackArgs);
        }

        //Lock a user
        this.lockUser = function (authTicket, uniqueIdentifier, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
               authTicket,
               svcUserManagement.getServiceUrl('LockUser'),
                {
                    authToken: authTicket,
                    uniqueIdentifier: uniqueIdentifier
                },
                callback,
                callbackArgs);
        }

        //Delete the certain user
        this.deleteUser = function (authTicket, uniqueIdentifier, deleteResponses, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteUser'),
                {
                    authToken: authTicket,
                    uniqueIdentifier: uniqueIdentifier,
                    deleteResponses: deleteResponses
                },
                callback,
                callbackArgs
            );
        }

        //Delete the certain group
        this.deleteUserGroup = function (authTicket, groupID, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteUserGroup'),
                {
                    authToken: authTicket,
                    databaseID: groupID                   
                },
                callback,
                callbackArgs
            );
        }

        //Delete the list of users
        this.deleteUsers = function (authTicket, uniqueIdentifierList, deleteResponses, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteUsers'),
                {
                    authToken: authTicket,
                    uniqueIdentifierList: uniqueIdentifierList,
                    deleteResponses: deleteResponses
                },
                callback,
                callbackArgs
            );
        }

        //Delete responses of the selected users
        this.deleteResponsesOfUsers = function (authTicket, uniqueIdentifierList, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteResponsesOfUsers'),
                {
                    authToken: authTicket,
                    uniqueIdentifierList: uniqueIdentifierList
                },
                callback,
                callbackArgs
            );
        }

        //Copy the specified userGroup
        this.copyUserGroup = function (authTicket, groupId, languageCode, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('CopyUserGroup'),
                {
                    authToken: authTicket,
                    groupId: groupId,
                    languageCode: languageCode
                },
                callback,
                callbackArgs
            );
        }

        //Remove a list of users from the group
        this.removeUsersFromGroup = function (authTicket, userUniqueIdentifiers, groupDatabaseID, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('RemoveUsersFromGroup'),
                {
                    authToken: authTicket,
                    groupDatabaseID: groupDatabaseID,
                    userUniqueIdentifiers: userUniqueIdentifiers
                },
                callback,
                callbackArgs
            );
        }

            //Remove all users from the group
            this.removeAllUsersFromGroup = function (authTicket, groupDatabaseID, callback, callbackArgs) {
                serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('RemoveAllUsersFromGroup'),
                {
                    authToken: authTicket,
                    groupDatabaseID: groupDatabaseID                    
                },
                callback,
                callbackArgs
            );
            }

            //delete all group members of the group from checkbox
            this.deleteAllGroupMembersFromCheckBox = function (authTicket, groupDatabaseID, callback, callbackArgs) {
                serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteAllGroupMembersFromCheckBox'),
                {
                    authToken: authTicket,
                    groupDatabaseID: groupDatabaseID,                   
                },
                callback,
                callbackArgs
            );
            }


        //Delete the certain groups
        this.deleteUserGroups = function (authTicket, groupIdList, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('DeleteUserGroups'),
                {
                    authToken: authTicket,
                    databaseIdList: groupIdList
                },
                callback,
                callbackArgs
            );
        }

        //Copy the specified userGroups
            this.copyUserGroups = function (authTicket, groupIdList, languageCode, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
               svcUserManagement.getServiceUrl('CopyUserGroups'),
                {
                    authToken: authTicket,
                    groupIdList: groupIdList,
                    languageCode: languageCode
                },
                callback,
                callbackArgs
            );
            }

            //Search users
            this.searchUsers = function (authTicket, searchTerm, callback, callbackArgs) {
                serviceHelper.makeServiceCall(
                    authTicket,
                    svcUserManagement.getServiceUrl('SearchUsers'),
                    { authToken: authTicket, searchTerm: searchTerm },
                    callback,
                    callbackArgs
                );
            }

            //Search groups
            this.searchGroups = function (authTicket, searchTerm, callback, callbackArgs) {
                serviceHelper.makeServiceCall(
                    authTicket,
                    svcUserManagement.getServiceUrl('SearchGroups'),
                    { authToken: authTicket, searchTerm: searchTerm },
                    callback,
                    callbackArgs
                );
            }

            //Add users to group
            this.addUsersToGroup = function (authTicket, groupId, userList, callback, callbackArgs){
                serviceHelper.makeServicePostCall(
                    authTicket,
                    svcUserManagement.getServiceUrl('AddUsersToGroup'),
                    {authToken: authTicket, userUniqueIdentifiers: userList, groupDatabaseID: groupId},
                    callback,
                    callbackArgs);
            }
}


