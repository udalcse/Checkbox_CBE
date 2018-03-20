
$(document).ready(function () {

    /**
    * Survey Header - "Options", "Share" & "Info" Buttons/Menus 
    */

    var surveyHeaderMenuSel = '.rightPanel .survey-header-container-no-dash .groupMenu';
    $(document).click(function (e) {
        var $target = $(e.target);

        if ($target.parent().hasClass('popup'))
            return;

        if (!($target.hasClass('action-menu-toggle') && $target.parent().hasClass('header-content'))) {
            $(surveyHeaderMenuSel).hide();
        }
    });

    // Survey Inline Preview
    $(window).resize(function () {
        var $preview = $("#survey_preview.preview-pc");
        var $rightViewport = $('.rightPanel .viewport');
        $preview.height(($rightViewport.height() - 100));
    });
    $(window).resize();

    // Survey Settings
    var $surveySettings = $('#surveySettings');
    var $surveyWelcome = $('#detailWelcome');
    var $mobileThemes = $('#mobile-themes');
    var $surveyContent = $('#detailContent');
    $('#survey_settings_button').on('click', function (e) {
        e.preventDefault();
        $surveyWelcome.hide();
        $mobileThemes.hide();
        $surveyContent.hide();
        $surveySettings.show();
    });

    var surveyActionsButtonId = 'survey_actions_button';
    var surveyActionsMenuId = 'survey_actions_menu';

    $(document).on('click', '#' + surveyActionsButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyActionsMenuId + ')').hide();
        var $menu = $('#' + surveyActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px'}).show();
        }
    });

    var surveyPreviewButtonId = 'survey_preview_button';
    var surveyPreviewMenuId = 'survey_preview_menu';

    $(document).on('click', '#' + surveyPreviewButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyPreviewMenuId + ')').hide();
        var $menu = $('#' + surveyPreviewMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    var surveySharingButtonId = 'survey_sharing_button';
    var surveySharingMenuId = 'survey_sharing_menu';
    
    $(document).on('click','#' + surveySharingButtonId, function (e)
    {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveySharingMenuId + ')').hide();
        var $menu = $('#' + surveySharingMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });
    
    var surveyResultsButtonId = 'survey_results_button';
    var surveyResultsMenuId = 'survey_results_menu';

    $(document).on('click', '#' + surveyResultsButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyResultsMenuId + ')').hide();
        var $menu = $('#' + surveyResultsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 10);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    /* Item "Options" Menu */
    $(document).click(function (e) {
        var $target = $(e.target);
        if (!($target.hasClass('action-menu-toggle') && $target.parent().attr('id') == 'detailMenuContainer')) {
            $('#detailMenuContainer .groupMenu').hide();
        }
    });
    var itemActionsButtonId = 'item_actions_button';
    var userActionsMenuId = 'item_actions_menu';
    var userHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    $(document).on('click', '#' + itemActionsButtonId, function (e) {
        e.preventDefault();
        var $menu = $('#' + userActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            $menu.show();
        }
        e.stopPropagation();
    });

    $(document).on('click', ".toggle-arrow", function (e, data) {
        e.stopPropagation();
        $(this).parent().click();
    });

});