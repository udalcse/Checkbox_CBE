
$(document).ready(function () {

    // Alter the page display to show a full-width grid
    activateFullPageGrid();

    // "Report Actions" menu
    var reportHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var reportActionsButtonId = 'report_actions_button';
    var reportActionsMenuId = 'report_actions_menu';
    $(document).on('click', '#' + reportActionsButtonId, function (e) {
        e.preventDefault();
        $(reportHeaderMenuSel + ':not(#' + reportActionsMenuId + ')').hide();
        var $menu = $('#' + reportActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
    });

});