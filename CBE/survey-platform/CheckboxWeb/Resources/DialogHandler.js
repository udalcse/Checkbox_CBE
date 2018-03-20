//Basic data for window sizes

var windowSizes = new Object();
windowSizes['confirm'] = { width: 300, height: 200 };
windowSizes['wizard'] = { width: 900, height: 650 };
windowSizes['addNewSurveyItemWizard'] = { width: 900, height: 680 };
windowSizes['smallproperties'] = { width: 400, height: 200 };
windowSizes['smallwideproperties'] = { width: 550, height: 250 };
windowSizes['properties'] = { width: 600, height: 400 };
windowSizes['mediumProperties'] = { width: 600, height: 450 };
windowSizes['largeProperties'] = { width: 600, height: 600 };
windowSizes['longlargeProperties'] = {width: 600, height: 650}
windowSizes['xlargeProperties'] = { width: 600, height: 700 };
windowSizes['security'] = { width: 800, height: 650 };
windowSizes['largeDialog'] = { width: 800, height: 600 };
windowSizes['xlargeDialog'] = { width: 900, height: 600 };
windowSizes['screenSize'] = { width: window.screen.availWidth * 0.8, height: window.screen.availHeight * 0.8 };

//Name of button clicked and will be used for asynchrous dialogs, such as confirm.
var _elementName;

//Slight hack to allow scrolling to be enabled for some dialogs w/out re-writing show dialog to take
// parameter object
var _scrolling = 'no';

/////////////////////////////////////////////////////////////////////////////
// showConfirmDialogWithCallback                                        //
// Show rad confirm dialog and initiate callback on OK click.              //
/////////////////////////////////////////////////////////////////////////////
function showMessage(message, title, width, height, onclose) {
    var defaultWidth = windowSizes['confirm'].width;
    var defaultHeight = windowSizes['confirm'].height;

    if (width == null || width == '') {
        width = defaultWidth;
    }

    if (height == null || height == '') {
        height = defaultHeight;
    }

    if (title == null) {
        title = '';
    }

    $('#messageDialog').modal({
        minHeight: height,
        maxHeight: height,
        minWidth: width,
        maxWidth: width,
        appendTo: 'form',
        closeHTML: '<div class="ckbx_dialogTitleBar"><div class="ckbx_dialogTitle">' + title + '</div><a href="javascript:void(0);" class="simplemodal-close roundedCorners ckbx-closedialog">CLOSE</a><br class="clear" /></div>',
        closeClass: 'ckbx-closedialog',
        onOpen: function (dialog) {
            dialog.overlay.fadeIn('300');
            dialog.container.fadeIn('300');
            dialog.data.fadeIn('300');
        },
        onClose: function (dialog) {
            dialog.overlay.fadeOut('300');
            dialog.container.fadeOut('300');
            dialog.data.fadeOut('300', function () {
                if ($.isFunction(onclose)) {
                    onclose();
                }

                $.modal.close();
            });
        },
        onShow: function (dialog) {
            $('.confirmText', dialog.data[0]).append(message);
            // if the user clicks "yes"
            $('#_okDialogBtn', dialog.data[0]).click(function () {
                // close the dialog
                $.modal.close();
            });
        }
    });
}

/////////////////////////////////////////////////////////////////////////////
// showConfirmDialogWithCallback                                        //
// Show rad confirm dialog and initiate callback on OK click.              //
/////////////////////////////////////////////////////////////////////////////
function showConfirmDialogWithCallback(confirmMessage, callbackFn, width, height, title, args, closeCallbackFn) {
    var defaultWidth = windowSizes['confirm'].width;
    var defaultHeight = windowSizes['confirm'].height;

    if (width == null || width == '') {
        width = defaultWidth;
    }

    if (height == null || height == '') {
        height = defaultHeight;
    }

    if (title == null) {
        title = '';
    }

    $('#confirmationDialog').modal({
        minHeight: height,
        maxHeight: height,
        minWidth: width,
        maxWidth: width,
        appendTo: 'form',
        closeHTML: '<div class="ckbx_dialogTitleBar"><div class="ckbx_dialogTitle">' + title + '</div><a href="javascript:void(0);" class="simplemodal-close roundedCorners ckbx-closedialog">CLOSE</a><br class="clear" /></div>',
        closeClass: 'ckbx-closedialog',
        onOpen: function (dialog) {
            dialog.overlay.fadeIn('300');
            dialog.container.fadeIn('300');
            dialog.data.fadeIn('300');
        },
        onClose: function (dialog) {
            dialog.overlay.fadeOut('300');
            dialog.container.fadeOut('300');
            dialog.data.fadeOut('300', function () {
                $.modal.close();
                if (!(dialog.success == true)) {
                    if ($.isFunction(closeCallbackFn)) {
                        closeCallbackFn();
                    }
                }
            });
        },
        onShow: function (dialog) {
            $('.confirmText', dialog.data[0]).append(confirmMessage);

            // if the user clicks "yes"
            $('#_yesBtn', dialog.data[0]).click(function () {
                // call the callback
                if ($.isFunction(callbackFn)) {
                    if (args == null)
                        args = {};
                    args['success'] = true;
                    dialog.success = true;
                    callbackFn(args);
                }
                // close the dialog
                $.modal.close();
            });
        }
    });
}

/////////////////////////////////////////////////////////////////////////////
// showConfirmDialog                                                       //
// Show  confirm dialog based on the click of any element that             //
// supports a postback event via doPostback.                               //
// Second argument specifies whether call is triggered by a link button    //
//  or not.
/////////////////////////////////////////////////////////////////////////////
function showConfirmDialog(theElement, isLinkButton, confirmMessage, width, height) {
    var defaultWidth = windowSizes['confirm'].width;
    var defaultHeight = windowSizes['confirm'].height;

    if (width == null || width == '') {
        width = defaultWidth;
    }

    if (height == null || height == '') {
        height = defaultHeight;
    }
    
    _elementName = theElement.name;

    if (!isLinkButton) {
        _elementName = _elementName + '$ctl00';
    }

    $('#confirmationDialog').modal({
        minHeight: height,
        maxHeight: height,
        minWidth: width,
        maxWidth: width,
        appendTo: 'form',
        closeHTML: '<div class="ckbx_dialogTitleBar"><a href="javascript:void(0);" class="simplemodal-close roundedCorners ckbx-closedialog">CLOSE</a><br class="clear" /></div>',
        closeClass: 'ckbx-closedialog',
        onOpen: function (dialog) {
            dialog.overlay.fadeIn('300');
            dialog.container.fadeIn('300');
            dialog.data.fadeIn('300');
        },
        onClose: function (dialog) {
            dialog.overlay.fadeOut('300');
            dialog.container.fadeOut('300');
            dialog.data.fadeOut('300', function () {
                $.modal.close();
            });
        },
        onShow: function (dialog) {
            $('.confirmText', dialog.data[0]).append(confirmMessage);

            // if the user clicks "yes"
            $('#_yesBtn', dialog.data[0]).click(function () {
                // call the callback
                if ($.isFunction(confirmCallBackFn)) {
                    confirmCallBackFn.apply();
                }
                // close the dialog
                $.modal.close();
            });
        }
    });
}

/////////////////////////////////////////////////////////////////////////////
// confirmCallBackFn                                                       //
// Call __doPostback for element.
// Should be executed only if the action is confirmed.
/////////////////////////////////////////////////////////////////////////////
function confirmCallBackFn() {
    __doPostBack(_elementName, '');
}

/////////////////////////////////////////////////////////////////////////////
// isWindowConfirmDialog                                                   //
// Determine if window is a confirmation dialog                            //
/////////////////////////////////////////////////////////////////////////////
function isWindowConfirmDialog(window) {
    var re = new RegExp("^confirm");
    return re.test(window.get_name());
}


/////////////////////////////////////////////////////////////////////////////
// closeWindow                                                             //
// Close current dialog and call callback if supplied.                     //
/////////////////////////////////////////////////////////////////////////////
function closeWindow(callback, arg) {
    if (top === self) {
        $.modal.close();
    }
    else {
        window.parent.$.modal.close();
    }

    if (callback != null) {
        if (typeof(callback) === 'function') {
            callback(arg);
        }
        else if (typeof (callback) == 'string' && callback != '') {
            executeStringCallback(callback, arg);
        }
    }
}

/////
function executeStringCallback(callback, arg) {
    //Try local callback
    if (!executeStringCallbackSource(window, callback, arg)) {
        //Try in dialog opener
        executeStringCallbackSource(window.parent, callback, arg);
    }
}

//////////////////
function executeStringCallbackSource(src, callback, arg) {
    var dotIndex = callback.indexOf('.');

    if (dotIndex > 0) {
        var firstPart = callback.substring(0, dotIndex);
        var secondPart = '';
        if (dotIndex < callback.length) {
            secondPart = callback.substring(dotIndex + 1);
        }

        if (secondPart == '' && src[callback]) {
            src[callback](arg);
            return true;
        }
        else if(src[firstPart] && src[firstPart][secondPart]) {
            src[firstPart][secondPart](arg);
            return true;
        }
    }
    else if(src[callback]) {
        src[callback](arg);
        return true;
    }

    return false;
}



/////////////////////////////////////////////////////////////////////////////
// refreshParentPage                                                       //
// Refresh parent window.                                                  //
/////////////////////////////////////////////////////////////////////////////
function refreshParentPage() {
    if (top === self)
        document.location.reload();
    else
        window.parent.location.reload();
}


/////////////////////////////////////////////////////////////////////////////
// redirectParentPage                                                      //
// Redirect parent window to new location.                                 //
/////////////////////////////////////////////////////////////////////////////
function redirectParentPage(newHref) {
    if (top === self)
        document.location.href = newHref;
    else
        window.parent.location.href = newHref;
}

/////////////////////////////////////////////////////////////////////////////
// closeWindowAndRefreshParentPage                                         //
// Close current dialog and refresh parent.                                //
/////////////////////////////////////////////////////////////////////////////
function closeWindowAndRefreshParentPage(callback, arg) {
    closeWindow(callback, arg);
    refreshParentPage();
}

/////////////////////////////////////////////////////////////////////////////
// closeWindowAndRedirectParentPage                                        //
// Close current dialog and redirect parent to new location.               //
/////////////////////////////////////////////////////////////////////////////
function closeWindowAndRedirectParentPage(callback, arg, newHref) {
    closeWindow(callback, arg);
    redirectParentPage(newHref);
}

/////////////////////////////////////////////////////////////////////////////
// Close current dialog and redirect parent to new location.               //
/////////////////////////////////////////////////////////////////////////////
function closeWindowModal(callback, arg) {
    closeWindow(callback, arg);
}


/////////////////////////////////////////////////////////////////////////////
// showDialog                                                              //
// Show a dialog.  dialogData can be a url or an element id                //
/////////////////////////////////////////////////////////////////////////////
function showDialog(dialogData, height, width, callback, appendTo) {
    if (height != null && typeof (height) == 'string') {
        if (windowSizes[height] != null) {
            width = windowSizes[height].width;
            height = windowSizes[height].height;
        }
        else {
            width = 600;
            height = 400;
        }
    }
    if (!height && !width) {
        width = 600;
        height = 400;
    }

    var options = {
        modal: true,
        minHeight: height,
        minWidth: width,
        maxHeight: height,
        maxWidth: width,
        appendTo: appendTo ? appendTo : '#aspnetForm',
        containerId: 'ckbx_dialogContainer',
        closeHTML: '<div class="ckbx_dialogTitleBar"><div class="ckbx_dialogTitle"></div><a href="javascript:void(0);" class="simplemodal-close roundedCorners ckbx-closedialog">CLOSE</a><br class="clear" /></div>',
        closeClass: 'ckbx-closedialog',
        onOpen: function (dialog) {
            dialog.overlay.fadeIn('300');
            dialog.container.fadeIn('300');
            dialog.data.fadeIn('300');
        },
        onClose: function (dialog) {
            if (callback != null) {
                if (typeof (callback) === 'function') {
                    callback();
                }
            }
            dialog.overlay.fadeOut('300');
            dialog.container.fadeOut('300');
            dialog.data.fadeOut('300', function () {
                $.modal.close();
            });
        },
        onShow: function () {
            //To prevent close click on modal header
            //since it is a part of header content it should be moved to modal template html markup or disable closing on header click
            $('.ckbx_dialogTitleBar.ckbx-closedialog').off();
            //setting event on close button to return prev value on dropdown menu
            $('.simplemodal-close.roundedCorners.ckbx-closedialog').on('click', function() {
                if (window.selectedField !== undefined && window.prevFieldType !== undefined) {
                    $('select[id$="' + selectedField + '"]').val(prevFieldType).click();
                }
            });
            //To avoid IE exceptions we should set iframe source after the modal show
            $('#modalIframe').attr("src", dialogData);
        }
    };

    var element = document.getElementById(dialogData);

    if (element) {
        $(element).modal(options);
    }
    else {
        var ticks = new Date().getTime();
        
        if (dialogData.indexOf('?') > 0) {
            dialogData = dialogData + '&modalId=' + ticks;
        }
        else {
            dialogData = dialogData + '?modalId=' + ticks;
        }
        
        //height and width must be adjusted for the dialog header and content padding
        $.modal('<iframe id="modalIframe" src="" height="' + (height - 30) + '" width="' + width + '" style="border:0;" scrolling="' + _scrolling + '">', options);
    }
}

