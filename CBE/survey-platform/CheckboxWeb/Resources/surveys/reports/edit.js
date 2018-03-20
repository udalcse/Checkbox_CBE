
$(document).ready(function () {

    // "Iem Actions" menu
    var itemHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var itemActionsButtonId = 'reportitem_actions_button';
    var itemActionsMenuId = 'reportitem_actions_menu';
    $(document).on('click', '#' + itemActionsButtonId, function (e) {
        e.preventDefault();
        $(itemHeaderMenuSel + ':not(#' + itemActionsMenuId + ')').hide();
        var $menu = $('#' + itemActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
    });
    
    var reportActionsButtonId = 'report_editor_actions_button';
    var reportActionsMenuId = 'report_actions_menu';

    $(document).on('click', '#' + reportActionsButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + reportActionsMenuId + ')').hide();
        var $menu = $('#' + reportActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    var reportSharingButtonId = 'report_sharing_button';
    var reportSharingMenuId = 'report_sharing_menu';

    $(document).on('click', '#' + reportSharingButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + reportSharingMenuId + ')').hide();
        var $menu = $('#' + reportSharingMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    var reportFiltersButtonId = 'report_filters_button';
    var reportFiltersMenuId = 'report_filters_menu';

    $(document).on('click', '#' + reportFiltersButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + reportFiltersMenuId + ')').hide();
        var $menu = $('#' + reportFiltersMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    var reportSettingsButtonId = 'report_settings_button';
    var reportSettingsMenuId = 'report_settings_menu';

    $(document).on('click', '#' + reportSettingsButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + reportSettingsMenuId + ')').hide();
        var $menu = $('#' + reportSettingsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

});