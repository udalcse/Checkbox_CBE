
//Useful object for handling some tasks of status showing
var statusControl = new statusControlObj();

//Enumeration which describes status message type
var StatusMessageType = { 'error': 0, 'success': 1, 'warning' : 2 };


/////////////////////////////////////////////////////////////////////////////
// Container for status control javascript routines. Requires jQuery       //
/////////////////////////////////////////////////////////////////////////////
function statusControlObj() {

    //Id of a panel, which will contain information about status
    this.statusPanelId = null;
    //Delay (in milliseconds) after which the message will be hidden
    this.delay = 1500;
    this.timeout_id = null;

    ///////////////////////////////////////////////////////////
    // Initialize with status panel id                       //
    ///////////////////////////////////////////////////////////
    this.initialize = function (statusPanelId) {
        statusControl.statusPanelId = statusPanelId;
    }

    //////////////////////////////////////////////////////////////
    // Show status message with a specific messageType.         //
    // After 'delay' milliseconds the message will be hidden.   //
    //////////////////////////////////////////////////////////////
    this.showStatusMessage = function (message, messageType) {
        var classForMessage = '';
        switch (messageType) {
            case StatusMessageType.error:
                classForMessage += " error";
                break;
            case StatusMessageType.success:
                classForMessage += " success";
                break;
            default:
                classForMessage += " warning";
        };
        $('#' + statusControl.statusPanelId).html("<span class='" + classForMessage + "'>" + message + "</span>");
        $('#' + statusControl.statusPanelId).slideDown('300');

        if (statusControl.delay <= 0) {
            alert('Status control error! Delay must be bigger than 0');
            return;
        }

        if (statusControl.timeout_id != null)
            clearTimeout(statusControl.timeout_id);

        statusControl.timeout_id = setTimeout(function () { $('#' + statusControl.statusPanelId).slideUp('300') }, statusControl.delay);
    }
}