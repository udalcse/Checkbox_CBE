/****************************************************************************
 * serviceHelper.js                                                         *
 *    Helper class and methods for accessing checkbox services.             *
 ****************************************************************************/

//Instance of helper object
var serviceHelper = new serviceHelperObj();


//Service JS helper
function serviceHelperObj() {
    this._noUserError = 'NoUser';
    this._baseServiceUrl = '/Services/';
    this._baseAppUrl = '';

    //Initialize with base service url
    this.initialize = function(baseServiceUrl, baseAppUrl) {
        serviceHelper._baseServiceUrl = baseServiceUrl;
        serviceHelper._baseAppUrl = baseAppUrl;
    };

    //
    this.getServiceUrl = function(serviceSpecificUrl, format, operation) {
        return serviceHelper._baseServiceUrl + serviceSpecificUrl + '/' + format + '/' + operation;
    };

    //
    this.urlEncode = function(c) {
        var o = ''; var x = 0; c = c.toString(); var r = /(^[a-zA-Z0-9_.]*)/ ;
        while (x < c.length) {
            var m = r.exec(c.substr(x));
            if (m != null && m.length > 1 && m[1] != '') {
                o += m[1]; x += m[1].length;
            } else {
                if (c[x] == ' ') o += '+'; else {
                    var d = c.charCodeAt(x); var h = d.toString(16);
                    o += '%' + (h.length < 2 ? '0' : '') + h.toUpperCase();
                } x++;
            }
        } return o;
    };

    //Call service. This function uses "Get" request.
    this.makeServiceCall = function (authTicket, serviceUrl, args, successCallback, successCallbackArgs, serviceErrorCallback) {
        //when running as root of website we have a leading '//' which must be replaced before the $.ajax()
        serviceUrl = serviceUrl.replace('//', '/');
        $.ajax({
            type: 'GET',
            url: serviceUrl,
            cache: false,
            data: args,
            dataType: 'json',
            error: serviceHelper.onAjaxError,
            success: function (result, code, xhr) {
                if (result == null || result.d == null) {
                    serviceHelper.onAjaxError(null, 'Service call succeeded, but returned null data.  Checkbox error log may contain more details.', null);
                }

                result = typeof result.d != "undefined" ? result.d : result;

                if ((typeof (args.skipAuthentication) == 'undefined' || !args.skipAuthentication) && !result.IsAuthenticated) {
                    window.location.href = serviceHelper._baseAppUrl + 'Login.aspx?returnUrl=' + serviceHelper.urlEncode(window.location.href);
                    return;
                }

                if (result.CallSuccess) {
                    if (successCallback != null) {
                        successCallback(result.ResultData, successCallbackArgs);
                    }

                    return;
                }

                //If no user error, go to login page and attempt return url
                if (result.FailureMessage != null
                        && result.FailureMessage.toLowerCase() == serviceHelper._noUserError.toLowerCase()) {

                    window.location.href = serviceHelper._baseAppUrl + 'Login.aspx?returnUrl=' + serviceHelper.urlEncode(window.location.href);
                    return;
                }

                if (serviceErrorCallback != null) {
                    serviceErrorCallback(result);
                }
                else {
                    serviceHelper.onServiceError(result);
                }
            }
        });
    };

    //DEFERRED OBJECT - Call service. This function uses "Get" request. -
    this.makeServiceCallD = function(authTicket, serviceUrl, args, successCallbackArgs) {
        return $.Deferred(function (dfd) {
            serviceUrl = serviceUrl.replace('//', '/');

            $.ajax({
                    type: 'GET',
                    url: serviceUrl,
                    cache: false,
                    data: args,
                    dataType: 'json'
                })
                .then(
                function(result, code, xhr) {
                    if ((typeof (args.skipAuthentication) == 'undefined' || !args.skipAuthentication) && !result.d.IsAuthenticated) {
                        window.location.href = serviceHelper._baseAppUrl + 'Login.aspx?returnUrl=' + serviceHelper.urlEncode(window.location.href);
                        return;
                    }

                    if (serviceHelper.handleServiceCallSuccess(result, code, xhr)) {
                        dfd.resolve(result.d.ResultData, successCallbackArgs);
                    }
                    else {
                        dfd.reject(result.d);
                    }
                },
                function(result, code, xhr) {
                    serviceHelper.onAjaxError(result, code, xhr);
                    dfd.reject();
                }
            );
        }).promise();
    };


    //Handle call success, inspect data, and return boolean indicating
    // if result data indicates success
    this.handleServiceCallSuccess = function(result, code, xhr) {
        if (result == null
            || result.d == null) {
            serviceHelper.onAjaxError(null, 'Service call succeeded, but returned null data.  Checkbox error log may contain more details.', null);
            return false;
        }

        if (result.d.CallSuccess) {
            return true;
        }

        return false;
    };


    //Call service. This function uses "Post" request.
    this.makeServicePostCall = function(authTicket, serviceUrl, args, successCallback, successCallbackArgs, serviceErrorCallback, errorCallbackArgs) {
        //when running as root of website we have a leading '//' which must be replaced before the $.ajax()
        serviceUrl = serviceUrl.replace('//', '/');

        $.ajax({
                type: 'POST',
                url: serviceUrl,
                cache: false,
                data: JSON.stringify(args),
                dataType: 'json',
                contentType: 'application/json',
                error: serviceHelper.onAjaxError,
                success: function(result, code, xhr) {
                    if (result == null
                        || result.d == null) {
                        serviceHelper.onAjaxError(null, 'Service call succeeded, but returned null data.  Checkbox error log may contain more details.', null);
                        return;
                    }

                    if ((typeof (args.skipAuthentication) == 'undefined' || !args.skipAuthentication) && !result.d.IsAuthenticated) {
                        window.location.href = serviceHelper._baseAppUrl + 'Login.aspx?returnUrl=' + serviceHelper.urlEncode(window.location.href);
                        return;
                    }

                    if (result.d.CallSuccess) {
                        if (successCallback != null) {
                            successCallback(result.d.ResultData, successCallbackArgs);
                        }

                        return;
                    }

                    if (serviceErrorCallback != null) {
                        serviceErrorCallback(result.d, errorCallbackArgs);
                    }
                    else {
                        serviceHelper.onServiceError(result.d);
                    }
                }
            });
        };

        //DEFERRED OBJECT - Post data to service
        this.makeServicePostCallD = function (authTicket, serviceUrl, args, successCallbackArgs) {
            return $.Deferred(function (dfd) {
                serviceUrl = serviceUrl.replace('//', '/');

                $.ajax({
                    type: 'POST',
                    url: serviceUrl,
                    cache: false,
                    data: JSON.stringify(args),
                    dataType: 'json',
                    contentType: 'application/json'
                })
                .then(
                function (result, code, xhr) {
                    if ((typeof (args.skipAuthentication) == 'undefined' || !args.skipAuthentication) && !result.d.IsAuthenticated) {
                        window.location.href = serviceHelper._baseAppUrl + 'Login.aspx?returnUrl=' + serviceHelper.urlEncode(window.location.href);
                        return;
                    }

                    if (serviceHelper.handleServiceCallSuccess(result, code, xhr)) {
                        dfd.resolve(result.d.ResultData, successCallbackArgs);
                    }
                    else {
                        dfd.reject(result.d);
                    }
                },
                function (result, code, xhr) {
                    serviceHelper.onAjaxError(result, code, xhr);
                    dfd.reject();
                }
            );
            }).promise();
        };

    //Error message
        this.onAjaxError = function (xhr, errorMsg, errorObj) {

            //Suppress these types of errors for now as they generally have no useful information and don't
            // represent a fatal error.
            return null;

            if (errorMsg == 'error') {
                alert('An ajax error occurred.  Server error log may contain more details. [' + xhr.status + '] [' + xhr.statusText + '] [' + xhr.responseText + ']');
            }
            else {
                if (errorMsg != null && errorMsg != '') {
                    alert(errorMsg);
                }
            }
        };

    //Error when ajax call successful, but back-end service reported error
    this.onServiceError = function(result) {
        if (result.FailureMessage == null) {
            alert('An unknown service error occurred.  Checkbox error log may contain more details.');
        } else {
            alert('An error occurred: ' + result.FailureMessage + '.  Checkbox error log may contain more details.');
        }
    };
}