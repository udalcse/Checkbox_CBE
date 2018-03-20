/****************************************************************************
 * svcAuthorization.js                                                      *
 *    Helper class and methods for authorizing access to Checkbox secured   * 
 *    resources.                                                            *
 ****************************************************************************/

//Instance of helper object
var svcAuthorization = new authorizationServiceObj();

//Invitation management JS helper
function authorizationServiceObj() {

    this.RESOURCETYPE_SURVEY = 'Survey';
    this.RESOURCETYPE_FOLDER = 'Folder';
    this.RESOURCETYPE_REPORT = 'Report';
    this.RESOURCETYPE_USERGROUP = 'UserGroup';
    this.RESOURCETYPE_LIBRARY = 'Library';
    this.RESOURCETYPE_EMAILLIST = 'EmailList';
    this.RESOURCETYPE_USER = 'User';

    //Get service url
    this.getServiceUrl = function(operationName) {
        return serviceHelper.getServiceUrl('AuthorizationService.svc', 'json', operationName);
    };

    //Get authorization
    this.authorizeAccess = function(uniqueIdentifier, resourceType, resourceId, permission, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            '',
            svcAuthorization.getServiceUrl('IsAuthorized'),
            { userUniqueIdentifier: uniqueIdentifier, securedResourceType: resourceType, resourceId: resourceId, permission: permission },
            callback,
            callbackArgs);
    };

    //Get authorization
    this.authorizeAccessD = function(uniqueIdentifier, resourceType, resourceId, permission, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            '',
            svcAuthorization.getServiceUrl('IsAuthorized'),
            { userUniqueIdentifier: uniqueIdentifier, securedResourceType: resourceType, resourceId: resourceId, permission: permission },
            callbackArgs);
    };

    //Get authorization for multiple permissions
    this.batchAuthorizeAccess = function(uniqueIdentifier, resourceType, resourceId, permissionArray, callback, callbackArgs) {
        serviceHelper.makeServiceCall(
            '',
            svcAuthorization.getServiceUrl('BatchIsAuthorized'),
            { userUniqueIdentifier: uniqueIdentifier, securedResourceType: resourceType, resourceId: resourceId, permissionCsv: permissionArray.join(',') },
            callback,
            callbackArgs);
    };

    //Get authorization
    this.batchAuthorizeAccessD = function (uniqueIdentifier, resourceType, resourceId, permissionArray, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            '',
            svcAuthorization.getServiceUrl('BatchIsAuthorized'),
            { userUniqueIdentifier: uniqueIdentifier, securedResourceType: resourceType, resourceId: resourceId, permissionCsv: permissionArray.join(',') },
            callbackArgs);
    };

    //Get authorization
    this.userHasRolePermissionD = function(uniqueIdentifier, permission, callbackArgs) {
        return serviceHelper.makeServiceCallD(
            '',
            svcAuthorization.getServiceUrl('HasRolePermission'),
            { userUniqueIdentifier: uniqueIdentifier, permission: permission },
            callbackArgs);
    };
}