////////////////////////////////////////////////////////////////////////////
// Helper for streamlining hiding/showing elements based on security      //
//   permissions. Helper looks for specific tags corresponding to         //
//   security roles/permissions.                                          //
////////////////////////////////////////////////////////////////////////////

var securityHelper = new securityHelperObj();

//
function securityHelperObj() {

    //Show/hide page elements. Returns a Deferred promise object, so this method can be called using
    // jQuery deferred framework, such as:
    // securityHelper.protect(...).then(doSomething());
    this.protect = function(userUniqueIdentifier, resourceType, resourceId, scope) {
        return $.Deferred(function(dfd) {
            var selector = '[protectPermission]';

            if (scope != null && scope != '') {
                selector = scope + ' ' + selector;
            }

            $(selector).ckbxProtect({
                    userUniqueIdentifier: userUniqueIdentifier,
                    resourceType: resourceType,
                    resourceId: resourceId,
                    onAuthorizeComplete: dfd.resolve
                });
        }).promise();
    };
}
