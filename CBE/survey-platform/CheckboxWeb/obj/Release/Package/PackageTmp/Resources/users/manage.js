
$(document).ready(function () {

    /* Left Column - "+ User(s)" dropdown menu */
    var usersmanagerAddUsersButtonId = 'ctl00_ctl00__pageContent__titleLinks__buttonAddUser';
    var usersmanagerAddUsersMenuId = 'usermanager_adduser_menu';

    $('#' + usersmanagerAddUsersButtonId).hover(function () {
        $('#' + usersmanagerAddUsersMenuId).show();
    }, function () {
        $('#' + usersmanagerAddUsersMenuId).hide();
    });

    $('#' + usersmanagerAddUsersMenuId).hover(function () {
        $(this).show();
    }, function () {
        $(this).hide();
    });

    // "User/Group/List Actions" menu
    var userHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var userActionsButtonId = 'user_actions_button';
    var userActionsMenuId = 'user_actions_menu';
    $(document).on('click', '#' + userActionsButtonId, function (e) {
        e.preventDefault();
        $(userHeaderMenuSel + ':not(#' + userActionsMenuId + ')').hide();
        var $menu = $('#' + userActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
    });

    $(document).on('click', ".toggle-arrow", function (e, data) {
        e.stopPropagation();
        $(this).parent().click();
    });

});