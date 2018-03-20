/****************************************************************************
* svcSecurityManagement.js                                                   *
*    Helper class and methods for accessing security management service.     *
****************************************************************************/

//Instance of helper object
var svcSecurityManagement = new securityManagementServiceObj();

function securityManagementServiceObj() {
    this.RESOURCETYPE_SURVEY = 'Survey';
    this.RESOURCETYPE_FOLDER = 'Folder';
    this.RESOURCETYPE_REPORT = 'Report';
    this.RESOURCETYPE_USERGROUP = 'UserGroup';
    this.RESOURCETYPE_LIBRARY = 'Library';
    this.RESOURCETYPE_EMAILLIST = 'EmailList';
    this.RESOURCETYPE_USER = 'User';

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('SecurityManagementService.svc', 'json', operationName);
    };

    //Get acl entries for resource
    this.getAclEntries = function (authTicket, resourceType, resourceId, pageNumber, pageSize, sortField, sortAscending, filterValue, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetAclEntries'),
            {
                authTicket: authTicket,
                resourceType: resourceType,
                resourceId: resourceId,
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortField: sortField,
                sortAscending: sortAscending,
                filterValue: filterValue
            },
            callback,
            callbackArgs);
    };

    //Get acl entries for resource -- DEFERRED
    this.getAclEntriesD = function (authTicket, resourceType, resourceId, pageNumber, pageSize, sortField, sortAscending, filterValue, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetAclEntries'),
            {
                authTicket: authTicket,
                resourceType: resourceType,
                resourceId: resourceId,
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortField: sortField,
                sortAscending: sortAscending,
                filterValue: filterValue
            },
            callbackArgs);
        };

        //Get acl entries permissible entities that can be added to ACL
        this.getAvailableAclEntries = function (authTicket, provider, resourceType, resourceId, permissionToGrant, pageNumber, pageSize, sortField, sortAscending, filterValue, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetAvailableEntries'),
            {
                authTicket: authTicket,
                provider : provider,
                resourceType: resourceType,
                resourceId: resourceId,
                permissionToGrant: permissionToGrant,
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortField: sortField,
                sortAscending: sortAscending,
                filterValue: filterValue
            },
            callback,
            callbackArgs);
        };


        //Get acl entries permissible entities that can be added to ACL
        this.getCurrentAclEntries = function (authTicket, resourceType, resourceId, permissionToCheck, pageNumber, pageSize, sortField, sortAscending, filterValue, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetCurrentEntries'),
            {
                authTicket: authTicket,
                resourceType: resourceType,
                resourceId: resourceId,
                permissionToCheck: permissionToCheck,
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortField: sortField,
                sortAscending: sortAscending,
                filterValue: filterValue
            },
            callback,
            callbackArgs);
        };

        //Get acl entries permissible entities that can be added to ACL
        this.addEntryToAcl = function (authTicket, resourceType, resourceId, aclEntryType, aclEntryIdentifier, permissionToGrant, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
                svcSecurityManagement.getServiceUrl('AddEntryToAcl'),
                {
                    authTicket: authTicket,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    aclEntryType: aclEntryType,
                    aclEntryIdentifier: aclEntryIdentifier,
                    permissionToGrant: permissionToGrant
                },
                callback,
                callbackArgs);
            };

            //Get acl entries permissible entities that can be added to ACL
            this.addEntryToAclD = function (authTicket, resourceType, resourceId, aclEntryType, aclEntryIdentifier, permissionToGrant, callbackArgs) {
                serviceHelper.makeServicePostCallD(
                authTicket,
                svcSecurityManagement.getServiceUrl('AddEntryToAcl'),
                {
                    authTicket: authTicket,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    aclEntryType: aclEntryType,
                    aclEntryIdentifier: aclEntryIdentifier,
                    permissionToGrant: permissionToGrant
                },
                callbackArgs);
            };


            //Get acl entries permissible entities that can be added to ACL
            this.removeEntryFromAcl = function (authTicket, resourceType, resourceId, aclEntryType, aclEntryIdentifier, callback, callbackArgs) {
                serviceHelper.makeServicePostCall(
                authTicket,
                svcSecurityManagement.getServiceUrl('RemoveEntryFromAcl'),
                {
                    authTicket: authTicket,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    aclEntryType: aclEntryType,
                    aclEntryIdentifier: aclEntryIdentifier
                },
                callback,
                callbackArgs);
            };

        //Get permissions for policy
        this.getPolicyPermissions = function (authTicket, resourceType, resourceId, policyId, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetPolicyPermissions'),
            {
                authTicket: authTicket,
                resourceType: resourceType,
                resourceId: resourceId,
                policyId: policyId
            },
            callback,
            callbackArgs);
        };

        //Get masked permissions for policy
        this.getMaskedPolicyPermissions = function (authTicket, resourceType, resourceId, policyId, callback, callbackArgs) {
            serviceHelper.makeServiceCall(
            authTicket,
            svcSecurityManagement.getServiceUrl('GetMaskedPolicyPermissions'),
            {
                authTicket: authTicket,
                resourceType: resourceType,
                resourceId: resourceId,
                policyId: policyId
            },
            callback,
            callbackArgs);
        };


        //Get masked permissions for policy
        this.updatePolicyPermissions = function (authTicket, resourceType, resourceId, policyId, permissions, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
                svcSecurityManagement.getServiceUrl('UpdatePolicyPermissions'),
                {
                    authTicket: authTicket,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    policyId: policyId,
                    permissions: permissions
                },
                callback,
                callbackArgs);
        };


        //Get masked permissions for policy
        this.updatePolicyMaskedPermissions = function (authTicket, resourceType, resourceId, policyId, permissionMasks, callback, callbackArgs) {
            serviceHelper.makeServicePostCall(
                authTicket,
                svcSecurityManagement.getServiceUrl('UpdatePolicyMaskedPermissions'),
                {
                    authTicket: authTicket,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    policyId: policyId,
                    permissionMasks: permissionMasks
                },
                callback,
                callbackArgs);
        };
}
