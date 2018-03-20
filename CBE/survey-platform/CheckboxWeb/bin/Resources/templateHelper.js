/***************************************************************************
 * Helper class to encapsulate common functionality required when using    *
 *   jQuery templates.  Relies on jQuery template functionality which must *
 *   be included in document making use of this helper.                    *
 ***************************************************************************/

//Create instance of helper object
var templateHelper = new templateHelperObj();

//Definition of helper object
function templateHelperObj() {

    //Storage for names of loaded templates
    this.templateArray = new Array();

    ///////////////////////////////////////////////////////////////////////
    // DEFERRED - Methods in this section use jQuery deferred and should //
    //            be the preferred method of using templates. "Callback" //
    //            methods found below are kept for legacy purposes and   //
    //            use should be phased out.                              //
    ///////////////////////////////////////////////////////////////////////
    
    /**********************************************************************
    * Load template and compile using jquery Deferred                     *
    **********************************************************************/
    this.loadAndCompileTemplateD = function(templatePath, templateName, callbackArgs) {
        return $.Deferred(function(dfd) {
            if (templateHelper.templateArray[templateName] != null) {
                dfd.resolve(callbackArgs);
                return;
            }

            $.when($.ajax({
                        type: 'GET',
                        url: templatePath,
                        cache: false
                    })
                )
                .then(
                    function(result, code, xhr) {
                        if (templateHelper.handleTemplateGetSuccess(templateName, result, code, xhr)) {
                            dfd.resolve(callbackArgs);
                        } else {
                            dfd.reject();
                        }
                    },
                    function(result, code, xhr) {
                        templateHelper.onAjaxError(result, code, xhr);
                        dfd.reject();
                    }
                );
        }).promise();
    }

   /**********************************************************************
    * Load template if necessary, apply to data, and append result to    *
    *  result container.  Container should contain element ID and        *
    *  ovewrite flag indicates if result replaces existing data or is    *
    *  appended to existing data.                                        *
    **********************************************************************/
    this.loadAndApplyTemplateD = function (templateName, templatePath, data, options, resultContainer, overwrite, callbackArgs) {
        return $.Deferred(function (dfd) {

            //If template already loaded, use it and apply
            if (templateHelper.templateArray[templateName] != null) {
                templateHelper.applyTemplate(templateName, data, options, resultContainer, overwrite);
                dfd.resolve(callbackArgs);
                return;
            }

            templateHelper.loadAndCompileTemplateD(templatePath, templateName, callbackArgs)
                .then(function () { templateHelper.applyTemplate(templateName, data, options, resultContainer, overwrite); })
                .then(function () { dfd.resolve(callbackArgs) });
        }).promise();
    }

    /**********************************************************************
     * Handle successful call to get template.                            *
     **********************************************************************/
    this.handleTemplateGetSuccess = function(templateName, result, code, xhr) {
        if (result == null) {
            templateHelper.onAjaxError(null, 'Template load call succeeded, but returned null data.  Checkbox error log may contain more details. [' + templateName + ']', null);
            return false;
        }

        //Compile the template
        $.template(templateName, result);

        //Add set flag on template array so we know template loaded/compiled
        templateHelper.templateArray[templateName] = true;

        return true;
    }

    /**********************************************************************
    * Apply template (in string form) to data, and append result to       *
    *  result container.  Container should contain element ID and         *
    *  ovewrite flag indicates if result replaces existing data or is     *
    *  appended to existing data. -- DEFERRED OBJECT                      *
    **********************************************************************/
    this.applyTemplateD = function (templateName, data, options, resultContainer, overwrite) {
        $.Deferred(function (dfd) {
            if (overwrite) {
                if (typeof (resultContainer) == 'string')
                    $('#' + resultContainer).empty();//.html('');
                else
                    resultContainer.empty();//.html('');
            }

            if (typeof (resultContainer) == 'string')
                $.tmpl(templateName, data, options).appendTo('#' + resultContainer);
            else
                $.tmpl(templateName, data, options).appendTo(resultContainer);

            dfd.resolve();
        }).promise();
    }

    /**********************************************************************
    * Apply template (in string form) to data, and append result to      *
    *  result container.  Container should contain element ID and        *
    *  ovewrite flag indicates if result replaces existing data or is    *
    *  appended to existing data.                                        *
    **********************************************************************/
    this.applyTemplate = function (templateName, data, options, resultContainer, overwrite) {
        if (overwrite) {
            if (typeof (resultContainer) == 'string')
                $('#' + resultContainer).empty(); //.html('');
            else
                resultContainer.empty(); //.html('');
        }

        var alternateRowMode = false;
        if (data.length > 1 && data.length % 2 == 1 && options != null && options.pageNum % 2 == 0)
            alternateRowMode = true;

        if (typeof (resultContainer) == 'string') {
            $container = $('#' + resultContainer);
            if (data.length > 1) {
                $.each(data, function (ind, obj) {
                    if (alternateRowMode) ind++;
                    $container.append($.tmpl(templateName, obj, { index: ind })); //.appendTo('#' + resultContainer);
                });
            } else {
                $container.append($.tmpl(templateName, data, options));
            }
        } else {
            if (data.length > 1) {
                $.each(data, function (ind, obj) {
                    if (alternateRowMode) ind++;
                    resultContainer.append($.tmpl(templateName, obj, { index: ind })); //.appendTo('#' + resultContainer);
                });
            } else {
                $.tmpl(templateName, data, options).appendTo(resultContainer);
            }
        }
    }

    /**********************************************************************
     *Show ajax error                                                     *
     **********************************************************************/
    //Error message
    this.onAjaxError = function (xhr, errorMsg, errorObj) {
        return null;
        if (errorMsg == 'error') {
            alert('An ajax error occurred.  Server error log may contain more details. [' + xhr.status + '] [' + xhr.statusText + '] [' + xhr.responseText + ']');
        }
        else {
            alert(errorMsg);
        }
    }

    ///////////////////////////////////////////////////////////////////////
    // LEGACY -                                                          //
    ///////////////////////////////////////////////////////////////////////

    /**********************************************************************
    * Load template and compile.                                          *
    **********************************************************************/
    this.loadAndCompileTemplate = function(templatePath, templateName, callback, callbackArgs) {
        //Load and compile temlpate if not already loaded
        if (templateHelper.templateArray[templateName] != null) {
            if (callback != null) {
                callback(callbackArgs);
            }
            return;
        }

        //when running as root of website we have a leading '//' which must be replaced before the $.get()
        templatePath = templatePath.replace('//', '/');
        $.ajax({
                type: 'GET',
                url: templatePath,
                cache: false,
                error: templateHelper.onAjaxError,
                success: function(result, code, xhr) {
                    if (result == null) {
                        templateHelper.onAjaxError(null, 'Template load call succeeded, but returned null data.  Checkbox error log may contain more details. [' + templatePath + ']', null);
                        return;
                    }

                    //Compile the template
                    $.template(templateName, result);

                    //Add set flag on template array so we know template loaded/compiled
                    templateHelper.templateArray[templateName] = true;

                    //Run callback
                    if (callback != null) {
                        callback(callbackArgs);
                    }
                }
            });
    }

    /**********************************************************************
    * Load template if necessary, apply to data, and append result to    *
    *  result container.  Container should contain element ID and        *
    *  ovewrite flag indicates if result replaces existing data or is    *
    *  appended to existing data.                                        *
    **********************************************************************/
    this.loadAndApplyTemplate = function(templateName, templatePath, data, options, resultContainer, overwrite, callback, callbackArgs) {
        //If template already loaded, use it and apply
        if (templateHelper.templateArray[templateName] != null) {
            templateHelper.applyTemplate(templateName, data, options, resultContainer, overwrite);

            if (callback != null) {
                callback(callbackArgs);
            }
            return;
        }

        //Load template and apply
        templateHelper.loadAndCompileTemplate(
            templatePath, //Path to template
            templateName, //Name of template
            function() {
                templateHelper.applyTemplate(templateName, data, options, resultContainer, overwrite);

                if (callback != null) {
                    callback(callbackArgs);
                }
            }
        );
    }
}