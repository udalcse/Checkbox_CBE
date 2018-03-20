
$(document).ready(function () {
    /* Left Column - "+ Library" dropdown menu */
    var managerAddLibraryButtonId = 'librarymanager_addlibrary';
    var managerAddLibraryMenuId = 'librarymanager_addlibrary_menu';

    $('#' + managerAddLibraryButtonId).hover(function () {
        $('#' + managerAddLibraryMenuId).show();
    }, function () {
        $('#' + managerAddLibraryMenuId).hide();
    });

    $('#' + managerAddLibraryMenuId).hover(function () {
        $(this).show();
    }, function () {
        $(this).hide();
    });

    // "Library Actions" menu
    var libraryHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';
    var libraryActionsButtonId = 'library_actions_button';
    var libraryActionsMenuId = 'library_actions_menu';
    $(document).on('click', '#' + libraryActionsButtonId, function (e) {
        e.preventDefault();
        $(libraryHeaderMenuSel + ':not(#' + libraryActionsMenuId + ')').hide();
        var $menu = $('#' + libraryActionsMenuId);
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