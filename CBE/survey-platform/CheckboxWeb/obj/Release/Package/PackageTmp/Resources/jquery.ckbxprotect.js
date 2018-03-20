(function($) {
    var methods = {

        //Init tabs
        protect: function(options) {

            //try {

            //Supported setting default values
            var settings = {
                resourceType: '',
                resourceId: '',
                userUniqueIdentifier: '',
                permission: '',
                onAuthorizeComplete: null
            };

            //Merge options with settings
            if (options) {
                $.extend(settings, options);
            }

            //Require auth service
            if (typeof(svcAuthorization) == 'undefined') {
                alert('Auth service required for ckbxProtect');
                return;
            }

            //Validate settings. Empty username is ok
            if (settings.resourceType == '' || settings.resourceId == '') {
                return;
            }

            var callbackCount = 0;
            var numElements = this.length;

            //If no elements, simply run callback
            if (numElements == 0) {
                if (settings.onAuthorizeComplete != null) {
                    settings.onAuthorizeComplete(null, false);
                }

                return;
            }

            //Following code is attempt to ensure only 1 auth call per permission to authorize.

            //Build permission array, if necessary.  Initialize values to "false"
            var permissionsArray = new Array();
            var permissionsLookup = new Object();

            //If permission specified in arg, use that, otherwise build from protectPermission attributes
            // on protected element
            if (settings.permission != null && settings.permission != '') {
                permissionsArray.push(settings.permission);
            }
            else {
                this.each(function() {
                    var permission = $(this).attr('protectPermission');

                    if (permission != null && permission != '' && !(permission in permissionsLookup)) {
                        permissionsLookup[permission] = '';
                        permissionsArray.push(permission);
                    }
                });
            }

            //If no permissions found or specified, simply do callback
            if (permissionsArray.length == 0) {
                if (settings.onAuthorizeComplete != null) {
                    settings.onAuthorizeComplete(null, false);
                }

                return;
            }

            svcAuthorization.batchAuthorizeAccessD(
                settings.userUniqueIdentifier,
                settings.resourceType,
                settings.resourceId,
                permissionsArray,
                this
                )
                .then(
                function(authResults, jqObjects) {
                    var resultsArray = new Object();

                    for (var i = 0; i < authResults.NameValueList.length; i++) {
                        resultsArray[authResults.NameValueList[i].Name] = authResults.NameValueList[i].Value;
                    }

                    jqObjects.each(
                        function() {
                            var $this = $(this);

                            var permissionToCheck = settings.permission;

                            if (permissionToCheck == null || permissionToCheck == '') {
                                permissionToCheck = $this.attr('protectPermission');
                            }

                            if (permissionToCheck == null
                                || permissionToCheck == ''
                                    || !(permissionToCheck in resultsArray)) {
                                return true;
                            }
                            
                            if (resultsArray[permissionToCheck].toLowerCase() == 'true') {
                                $this.show();
                            }
                            else {
                                var tagName = $this.prop('tagName').toLowerCase();

                                if (tagName == 'select') {
                                    $this.attr('disabled', 'disabled');
                                }
                                else {
                                    $this.hide();
                                    $this.empty();
                                }
                            }
                        }
                    );

                    if (settings.onAuthorizeComplete != null) {
                        settings.onAuthorizeComplete(this, true);
                    }
                }
                );
            //}
            // catch (err) {
            //    alert('jquery.ckbxprotect.js error.');
            //}

            return this.each(function() { });
        }
    };

    $.fn.ckbxProtect = function(method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.protect.apply(this, arguments);
        }
    };
})(jQuery);