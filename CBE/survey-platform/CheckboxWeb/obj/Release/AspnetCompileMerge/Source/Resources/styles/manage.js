
$(document).ready(function () {

    /* Left Column - "+ Style" dropdown menu */
    var managerAddStyleButtonId = 'stylemanager_addstyle';
    var managerAddStyleMenuId = 'stylemanager_addstyle_menu';

    $('#' + managerAddStyleButtonId).hover(function () {
        $('#' + managerAddStyleMenuId).show();
    }, function () {
        $('#' + managerAddStyleMenuId).hide();
    });

    $('#' + managerAddStyleMenuId).hover(function () {
        $(this).show();
    }, function () {
        $(this).hide();
    })

    // "Style Actions" menu
    var styleHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var styleActionsButtonId = 'style_actions_button';
    var styleActionsMenuId = 'style_actions_menu';
    $(document).on('click', '#' + styleActionsButtonId, function (e) {
        e.preventDefault();
        $(styleHeaderMenuSel + ':not(#' + styleActionsMenuId + ')').hide();
        var $menu = $('#' + styleActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
    });
});