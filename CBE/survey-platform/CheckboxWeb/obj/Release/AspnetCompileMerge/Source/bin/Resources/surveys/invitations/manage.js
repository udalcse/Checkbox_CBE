
$(document).ready(function () {

    // Alter the page display to show a full-width grid
    activateFullPageGrid();

    // "Invitation Actions" menu
    var invitationHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var invitationActionsButtonId = 'invitation_actions_button';
    var invitationActionsMenuId = 'invitation_actions_menu';
    $(document).on('click', '#' + invitationActionsButtonId, function (e) {
        e.preventDefault();
        $(invitationHeaderMenuSel + ':not(#' + invitationActionsMenuId + ')').hide();
        var $menu = $('#' + invitationActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
    });

});