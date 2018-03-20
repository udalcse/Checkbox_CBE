
// Advanced Settings Elements
var $pmAddToggle;
var $pmAddCancel;
var $pmAdd;
var $pmCurrent;
var $pmAddSubmit;
var $pmPermMsg;
var $pmPermList;

// Toggle setting save actions 
function toggleSettingSaveOpts(settingInput, show) {
    var $item = $(settingInput).closest('.setting-item');
    var $saveOpts = $item.find('.save-options');
    if ($saveOpts.length > 0) {
        if (show) {
            if (!$saveOpts.is(':visible')) { // only animate if the buttons are NOT visible
                $saveOpts.css({ opacity: '0', display: 'inline-block', width: 'auto' }).animate({ opacity: '1' });
            }
        }
        else {
            $saveOpts.animate({ opacity: '0', width: '0' }).hide();
        }
    }
}

// Toggle advanced settings
function toggleAdvancedSettings(toggleLink) {
    var $settings = $(toggleLink).closest('.advanced-settings').find('.setting-container');
    if ($settings.is(':visible')) {
        $(toggleLink).animate({ opacity: '0' }, function () {
            $(this).text($(this).text().replace('- Hide', '+ Show')).animate({ opacity: '1' });
        });
        $settings.animate({ opacity: '0' }, function () {
            $(this).css({ opacity: '1' }).hide();
        });
    }
    else { // show
        $(toggleLink).animate({ opacity: '0' }, function () {
            $(this).text($(this).text().replace('+ Show', '- Hide')).animate({ opacity: '1' });
        });
        $settings.css({ opacity: '0' }).show().animate({ opacity: '1' });
    }
}

// Autosave a setting
function autosaveSetting(inputItem) {
    var $item = $(inputItem).closest('.setting-item');
    var $status = $item.children('.status');
    var $saveOpts = $item.children('.save-options');
    if ($saveOpts.length > 0) {
        $saveOpts.hide();
    }
    if ($status.length > 0 && !$status.hasClass('saving')) {
        $status.css('opacity', '1').attr('class', 'status saving');
        var statusCompleteTO = setTimeout(function () {
            if (!$status.hasClass('error')) {
                $status.css('opacity', '0').text('Saved.').attr('class', 'status saved').animate({ opacity: '1' });
            } else {
                $status.css('opacity', '0').text('').attr('class', 'status');
            }
            var statusClearTO = setTimeout(function () {
                $status.animate({ opacity: '0' }, function () {
                    $(this).text('').attr('class', 'status');
                });
            }, 4000);
        }, 1200);
    }
}

// Toggle permission "add" view
function togglePermissionAdd(show) {
    if (show) {
        $pmCurrent.animate({ opacity: '0' }, function () {
            $(this).hide();
            $pmAdd.css('opacity', '0').show();
            $pmAdd.animate({ opacity: '1' });
        });
    }
    else {
        $pmAdd.animate({ opacity: '0' }, function () {
            $(this).hide();
            $pmCurrent.css('opacity', '0').show();
            $pmCurrent.animate({ opacity: '1' });
        });
    }
}

// Toggle "add user/group" button active
function togglePermissionAddButton() {
    if ($pmAdd.find('input[type="checkbox"]:checked').length > 0) {
        $pmAddSubmit.removeClass('disabled');
    }
    else {
        $pmAddSubmit.addClass('disabled');
    }
}

$(document).ready(function () {

    /**
    * Survey Settings
    */

    var $settings = $('#surveySettings');
    var $settingTabs = $('#setting_tabs').find('li');
    var $settingSections = $('#setting_sections').children();
    $settingTabs.children('a').click(function (e) {
        var tabIdx = $settingTabs.index($(this).parent());
        $settingTabs.removeClass('active');
        $(this).parent().addClass('active');
        $settingSections.hide();

        //update mobile preview
        if (tabIdx == 1)
            initializeMobilePreview();

        $settingSections.eq(tabIdx).show();
    });
    $settingTabs.children('a:eq(0)').click();

    // Activate toggle switch inputs
    $settings.find(".switch").addClass('icons').slickswitch();

    //ignore uniform 
    $settings.find('[type=checkbox], select').attr('uniformIgnore', 'true');

    // Activate settings save option toggling
    $(document).on('focus', '#surveySettings [type="text"]:not(.datetimepicker)', function (e) {
        toggleSettingSaveOpts(this, true);
    });
    $(document).on('blur', '#surveySettings [type="text"]:not(.datetimepicker)', function (e) {
        var parent = $(this).parents('[data-oldvalue]')[0];
        if (typeof (parent) != 'undefined' && $(this).val() == $(parent).attr('data-oldvalue')) {
            toggleSettingSaveOpts(this, false);
        }
    });

    $('#surveySettings').on('click', ' .actions .cancel', function (e) {
        toggleSettingSaveOpts(this, false);
    });

    // Activate advanced settings
    $('#surveySettings').on('click', '.advanced-settings .toggle-settings > a', function (e) {
        toggleAdvancedSettings(this);
        return false;
    });

    // Alter tooltip when designated dropdowns are changed
    var $alterTooltipSettingSelects = $settings.find('select.alter-tooltip');
    $alterTooltipSettingSelects.change(function (e) {
        var $selOpt = $(this).find('option:selected');
        var $tt = $(this).closest('.setting-item').find(' > label .tooltip');
        if ($tt.length > 0 && typeof $selOpt.attr('data-message') !== 'undefined') {
            $tt.attr('data-message', $selOpt.attr('data-message'));
        }
    });
    $alterTooltipSettingSelects.change();

    // Show additional options when designated dropdowns are changed
    $settings.find('select.has-additional-options').change(function (e) {
        var $allAddl = $(this).closest('.setting-item').find('.additional-options');
        var $thisAddl = $allAddl.filter('.additional-options-' + $(this).find('option:selected').attr('data-type'));
        $allAddl.hide();
        if ($thisAddl.length > 0) {
            $thisAddl.css('opacity', '0').show().animate({ opacity: '1' });
        }
    });
    $settings.find('select.has-additional-options').change();

    /**
    * Permissions - Advanced
    */

    $pmAddToggle = $("#permissions_acl_add_toggle");
    $pmAddCancel = $("#permissions_acl_add_cancel");
    $pmAdd = $("#permissions_acl_add");
    $pmCurrent = $("#permissions_acl_current");
    $pmAddSubmit = $("#permissions_acl_add_submit");
    $pmPermMsg = $("#permissions_acl_perm_message");
    $pmPermList = $("#permissions_acl_perm_list");

    // Toggling the "add user/group" panel
    $pmAddToggle.click(function (e) {
        e.preventDefault();

        togglePermissionAddButton();
        togglePermissionAdd(true);
    });
    $pmAddCancel.click(function (e) {
        e.preventDefault();

        togglePermissionAdd(false);
    });
    $(document).on('click', '#' + $pmAdd.attr('id') + ' input[type="checkbox"]', function (e) {
        togglePermissionAddButton();
    });
    $pmAddSubmit.click(function (e) {
        e.preventDefault();
        if ($(this).hasClass('disabled')) {
            return false;
        }

        $pmAdd.find('input[type="checkbox"]:checked').each(function () {
            var $currentGrid = $pmCurrent.find('.ckbxAclGrid');
            var $thisItem = $(this).closest('.gridContent');
            $(this).parent().remove();
            var $newItem = $thisItem.clone();
            $currentGrid.prepend($newItem);
            $thisItem.remove();
        });

        $pmAdd.animate({ opacity: '0' }, function () {
            $(this).hide();
            $pmCurrent.css('opacity', '0').show();
            $pmCurrent.animate({ opacity: '1' });
        });
    });

    // When a user/group is selected for permission editing...
    $(document).on('click', '#' + $pmCurrent.attr('id') + ' .gridContent', function (e) {
        $pmCurrent.find('.gridContent').removeClass('gridActive');
        $(this).addClass('gridActive');
        $pmPermMsg.animate({ opacity: '0' }, function () {
            $(this).hide();
            $pmPermList.css('opacity', '0').show();
            $pmPermList.animate({ opacity: '1' });
        });
    });

    // Autosave for toggle switches, submit buttons, & autosave dropdowns
    $(document).on('click', '#surveySettings > .switch, .actions .submit', function (e) {
        autosaveSetting(this);
    });
    $settings.find('select.autosave').change(function (e) {
        autosaveSetting(this);
    });

});