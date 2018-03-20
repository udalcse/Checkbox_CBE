
/* Viewing overflowed survey names */
var hoveredSurveyId = null;
var hoveredFolderId = null;
var surveyNameWidth = 220;
var folderNameWidth = 180;
var cloneHideInterval = null;
var cloneHideIntervalCount = 0;
var enableNameClones = false;

function hideNameClones(allImmediate) {
    allImmediate = (typeof allImmediate != 'undefined') ? allImmediate : false;
    if (allImmediate) {
        $('body > .name-clone').removeClass('hovered').hide();
        $('#listPlace .groupContent').removeClass('hovered');
        overNameClone = false;
    }
    else {
        clearInterval(cloneHideInterval);
        cloneHideIntervalCount = 0;
        cloneHideInterval = setInterval(function () {
            cloneHideIntervalCount++;
            if (cloneHideIntervalCount > 20) {
                cloneHideIntervalCount = 0;
                clearInterval(cloneHideIntervalCount);
            }
            else {
                var cloneSel = 'body > .name-clone';
                $(cloneSel + ':not(.hovered)').each(function () {
                    var itemType = $(this).attr('data-item-type');
                    var itemId = $(this).attr('data-item-id');
                    $(this).hide();
                    if (itemType == 'survey') {
                        $('#survey_' + itemId).removeClass('hovered');
                    }
                });
                if ($(cloneSel + '.hovered').length == 0) {
                    overNameClone = false;
                    resizePanels();
                }
            }
        }, 50);
    }
}
function showNameClone(hoveredItem) {
    if (!enableNameClones) {
        return false;
    }

    hideNameClones(true);
    var $folderName = $(hoveredItem).find('.folderName');
    var $surveyName = $(hoveredItem).find('.listTemplateName');

    var itemId = null;
    var nameType = null;
    var $nameToClone = null;
    if ($folderName.length > 0) {
        nameType = 'folder';
        $nameToClone = $folderName;
    }
    else if ($surveyName.length > 0) {
        nameType = 'survey';
        $nameToClone = $surveyName;
    }

    if ($nameToClone != null) {
        var nameOffset = $(hoveredItem).offset();
        var nameCloneId = '';
        if (nameType == 'survey') {
            hoveredSurveyId = $(hoveredItem).closest('.groupContent').attr(nameType + 'id');
            nameCloneId = 'survey_' + hoveredSurveyId + '_' + nameType + '_name_clone';
            itemId = hoveredSurveyId;
        }
        else if (nameType == 'folder') {
            hoveredFolderId = $(hoveredItem).attr(nameType + 'id');
            nameCloneId = 'folder_' + hoveredFolderId + '_' + nameType + '_name_clone';
            itemId = hoveredFolderId;
        }
        var $nameClone = $('#' + nameCloneId);
        if ($nameClone.length == 0) {
            $nameClone = $('<div />').html((nameType == 'folder' ? $folderName.html() : (nameType == 'survey' ? $surveyName.html() : '')));
            if ($nameClone.find('.inactive-label').length > 0) {
                $nameClone.addClass('inactive');
                $nameClone.find('.inactive-label').remove();
            }
            if (nameType == 'survey' && $(hoveredItem).closest('.folderContainer').length > 0) {
                $nameClone.addClass('in-folder');
            }
            $nameClone.attr('id', nameCloneId).addClass('listTemplateName').addClass('name-clone').addClass(nameType + '-name');
            $nameClone.hide().appendTo('body');
        }
        if ((nameType == 'survey' && mouseX <= surveyNameWidth) || (nameType == 'folder' && mouseX <= folderNameWidth)) {
            if (nameType == 'survey' && $(hoveredItem).hasClass('activeContent')) {
                $nameClone.addClass('active');
            }
            else {
                $nameClone.removeClass('active');
            }
            $nameClone.css({
                left: nameOffset.left + 'px',
                "line-height": "2em",
                position: 'absolute',
                top: nameOffset.top + 'px',
                "z-index": 1000
            }).attr('data-item-type', nameType).attr('data-item-id', itemId).removeClass('disabled').addClass('hovered').show();
            if (nameType == 'survey') {
                $('#survey_' + itemId).addClass('hovered');
            }

            overNameClone = true;
        }
    }
}

/* Fixed survey header */
function enableFixedSurveyHeader() {
    var fixedHeaderTimeout = setTimeout(function () {
        $(window).resize();
        var $glimpseInfo = $("#infoPlace");
        if ($glimpseInfo.length > 0) {
            var $nameContainer = $glimpseInfo.find(".survey-header-container");
            $nameContainer.css({ position: 'static', top: '0' });
            $glimpseInfo.css('padding-top', 0);
            var infoWidth = $glimpseInfo.width();
            if ($nameContainer.length > 0) {
                var nameHeight = $nameContainer.outerHeight();
                var nameOffset = $nameContainer.offset();
                $nameContainer.css({ top: (nameOffset.top - 10) + 'px', left: nameOffset.left + 'px', position: 'fixed', width: infoWidth + 'px' });
                $glimpseInfo.css('padding-top', (nameHeight - 10) + 'px');
            }
        }
    }, 500);
}


/**
 * Init.
 */

$(document).ready(function () {

    /**
    * Survey/Folder Overflow Names 
    */

    $(document).on('mouseover', '.name-clone', function (e) {
        $(this).addClass('hovered');
    });
    $(document).on('mouseout', '.name-clone', function (e) {
        $(this).removeClass('hovered');
        hideNameClones();
    });
    $(document).on('mousemove', '.name-clone', function (e) {
        var isFolderName = $(this).hasClass('folder-name');
        if (isFolderName && mouseX > folderNameWidth) {
            $(this).removeClass('hovered').addClass('disabled').hide();
        }
        if (!isFolderName && mouseX > surveyNameWidth) {
            $(this).removeClass('hovered').addClass('disabled').hide();
        }
    });
    $(document).on('click', '.name-clone', function (e) {
        e.preventDefault();
        var nameType = $(this).attr('data-item-type');
        var objectId = $(this).attr('data-item-id');
        var $item = nameType == 'folder' ? $('#folderHeader_' + objectId) : (nameType == 'survey' ? $('#survey_' + objectId) : '');
        if ($item.length > 0) {
            $item.click();
        }
        $(this).removeClass('hovered');
    });
    var namesSelector = '#listPlace .groupContent, #listPlace .groupWrapper .folderHeader';
    $(document).on('mouseover', namesSelector, function (e) {
        showNameClone(this);
    });
    $(document).on('mouseout', namesSelector, function (e) {
        hideNameClones();
    });
    $(document).on('mousemove', namesSelector, function (e) {
        if ((mouseX < folderNameWidth) || (mouseX < surveyNameWidth)) {
            showNameClone(this);
        }
    });


    /**
    * Survey Header - "Options", "Share" & "Info" Buttons/Menus 
    */

    var surveyActionsButtonId = 'survey_actions_button';
    var surveyActionsMenuId = 'survey_actions_menu';
    var surveyShareButtonId = 'survey_share_button';
    var surveyShareMenuId = 'survey_share_menu';
    var surveyInfoButtonId = 'survey_info_button';
    var surveyInfoMenuId = 'survey_info_menu';
    $(document).on('click', '#' + surveyActionsButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyActionsMenuId + ')').hide();
        var $menu = $('#' + surveyActionsMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 8);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });
    $(document).on('click', '#' + surveyShareButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyShareMenuId + ')').hide();
        var $menu = $('#' + surveyShareMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        }
        else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 8);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });
    $(document).on('click', '#' + surveyInfoButtonId, function (e) {
        e.preventDefault();
        $(surveyHeaderMenuSel + ':not(#' + surveyInfoMenuId + ')').hide();
        var $menu = $('#' + surveyInfoMenuId);
        if ($menu.is(':visible')) {
            $menu.hide();
        } else {
            var menuRight = $(this).parent().width() - $(this).position().left - $(this).outerWidth() - 8;
            var menuTop = ($(this).height() + 8);
            $menu.css({ right: menuRight + 'px', top: menuTop + 'px' }).show();
        }
    });

    $(document).on('click', ".toggle-arrow", function (e, data) {
        e.stopPropagation();
        $(this).parent().click();
    });

    /* Left Column - "+ Survey" dropdown menu */
    var managerAddSurveyButtonId = 'surveymanager_addsurvey';
    var managerAddSurveyMenuId = 'surveymanager_addsurvey_menu';

    $('#' + managerAddSurveyButtonId).hover(function () {
        $('#' + managerAddSurveyMenuId).show();
    }, function() {
        $('#' + managerAddSurveyMenuId).hide();
    });
    $('#' + managerAddSurveyMenuId).hover(function () {
        $(this).show();
    }, function () {
        $(this).hide();
    });
});