/////////////////////////////////////////////////////////////////////////////
// ajaxWidget.js                                                           //
// Utility methods for handling ajaxification of control loading.          //
/////////////////////////////////////////////////////////////////////////////

//Name of button clicked and will be used for asynchrous dialogs, such as confirm.
var _elementName;

/////////////////////////////////////////////////////////////////////////////
// showConfirmDialogWithCallback                                        //
// Show rad confirm dialog and initiate callback on OK click.              //
/////////////////////////////////////////////////////////////////////////////
function showConfirmDialogWithCallback(confirmMessage, callbackFn, width, height, title) {
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
                if ($.isFunction(callbackFn)) {
                    callbackFn.apply();
                }
                // close the dialog
                $.modal.close();
            });
        }
    });
}

/////////////////////////////////////////////////////////////////////////////
// showConfirmDialog                                                    //
// Show rad confirm dialog based on the click of any element that          //
// supports a postback event via doPostback.                               //
// Call is not inherently "Ajaxy" but clicked element can be bound by      //
// rad ajax manager to make it ajaxy.                                      //
// Second argument specifies whether call is triggered by a link button    //
//  or not.
/////////////////////////////////////////////////////////////////////////////
function showConfirmDialog(theElement, isLinkButton, confirmMessage) {
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
// Call __doPostback for element when true is returned from radconfirm     //
// dialog.
/////////////////////////////////////////////////////////////////////////////
function confirmCallBackFn(arg) {
    if (arg == true) {
        __doPostBack(_elementName, '');
    }
}

/////////////////////////////////////////////////////////////////////////////
// bindAjaxRequestToLoad                                                   //
// Use rad ajax manager to fire an ajax request on page load.  Used to     //
// perform initial widget load in ajaxy manner and to support use of       //
// auto-wired ajax loading panels.                                         //
/////////////////////////////////////////////////////////////////////////////
//function bindLoad(radAjaxMgrClientId, eventName) {
//    $(document).ready(
//        function() {
//            setTimeout(
//                function() { window[radAjaxMgrClientId].ajaxRequest(eventName); },
//                200);
//        }
//    );
//}

/////////////////////////////////////////////////////////////////////////////
// doPostback                                                              //
// Simulate a normal postback with rad ajax manager                        //
/////////////////////////////////////////////////////////////////////////////
//function doPostback(radAjaxMgrClientId, eventTarget, eventArgument) {
//    window[radAjaxMgrClientId].ajaxRequestWithTarget(eventTarget, eventArgument);
//}

//Determine if the window is a confirm window
function isWindowConfirmDialog(window){
    var re = new RegExp("^confirm");
    return re.test(window.get_name());
}