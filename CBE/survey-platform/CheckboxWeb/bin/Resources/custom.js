
/**
 * Globals
 */

/* General Info. */
var activePage = '';
var _appRoot = '';

/* Global Selectors */
var surveyHeaderMenuSel = '.rightPanel .survey-header-container .groupMenu';

/* Header Messages */
var $headerMessages;

/*Live Search*/
var $liveSearch;

/* Panel Sizing */
var overNameClone = false;
var panelsBeingResized = false;
var collapsedLeftWidth = 60;
var expandedLeftWidth = 320;
var leftColumnCollapseAtPageWidth = 1120;
var collapsedLeftClass = 'leftPanel-collapsed';
var forceCollapsedLeftClass = 'force-collapsed';
var userAlteredColumn = false;
var leftResizingClass = 'leftPanel-resizing';
var leftResizing = false;
var toggleLeftTimeout = null;

var mouseX = 0;
var mouseY = 0;
$(document).mousemove(function (e) {
    mouseX = e.pageX;
    mouseY = e.pageY;
});
$(document).mousemove();


/**
* JQuery Extensions
*/

// Case insensitive :contains
jQuery.expr[':'].Contains = function (a, i, m) {
    return jQuery(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
};



/********************************************************
*                   GENERAL
*********************************************************/

/* Full-width Grid Layout */

function activateFullPageGrid() {
    var $content = $('.pageContent');
    $content.addClass('full-width-grid');
    var $rightPanel = $content.find('.rightPanel');
    $rightPanel.prepend('<div class="ckbx_dialogTitleBar"><a id="rightPanel_close" class="simplemodal-close" href="javascript:void(0);">CLOSE</a><br class="clear"></div>');
    $('#rightPanel_close').click(function (e) {
        $rightPanel.hide();
    });

    // Move the left panel search into the grid header (if a grid page)
    var $gridMenu = $('.leftPanel .gridMenu');
    if ($gridMenu.length > 0) {
        $('#leftPanel_search').prependTo($gridMenu);
    }

    // Show right panel (as popup) when an item is selected from the grid
    $(document).on('click', '.leftPanel .gridContent', function (e) {
        if (!$(this).parent().hasClass('reportList'))
            $('.rightPanel').show();
    });

    var html = $('.crumb-panel').hide().html();
    $('#breadcrumbTopPanel').html(html).addClass('crumb-panel');
}

/* Left Column - Action Buttons Toggling */

// Used to determine whether or not to show action
// buttons above a left-column grid of items
function toggleGridActionButtons(itemType, eventItem, show, deleteResponsesOnly) {
    var itemTypeCap = itemType.substr(0, 1).toUpperCase() + itemType.substr(1);
    var $grid = $(eventItem).closest('.ckbxGrid');
    var $menu = $grid.parent().find('.gridMenu');
    if ($menu.length > 0) {
        show = (typeof show != 'undefined') ? show : ($grid.find('.delete' + itemTypeCap + ':checked').length > 0);
        if (show) {
            $menu.addClass('actions-available');
            if (!deleteResponsesOnly) {
                $('.gridMenu #_deleteSelectedUsersLink').show();
            } else {
                $('.gridMenu #_deleteSelectedUsersLink').hide();
            }
        }
        else {
            $menu.removeClass('actions-available');
        }
    }
}

function resetGridMenu() {
    $('.gridMenu').removeClass('actions-available');
}

// Get the active page being viewed
function setActivePage() {
    if (window.location.href.indexOf('Forms/Manage.aspx') >= 0) {
        activePage = 'survey-manager';
    }
    else if (window.location.href.indexOf('Surveys/Edit.aspx') >= 0) {
        activePage = 'survey-editor';
    }
    else if (window.location.href.indexOf('Users/Manage.aspx') >= 0) {
        activePage = 'user-manager';
    }
    else if (window.location.href.indexOf('Libraries/Manage.aspx') >= 0) {
        activePage = 'library-manager';
    }
    else if (window.location.href.indexOf('Styles/Manage.aspx') >= 0) {
        activePage = 'style-manager';
    }
    else if (window.location.href.indexOf('Responses/Manage.aspx') >= 0) {
        activePage = 'response-manager';
    }
    else if (window.location.href.indexOf('Install/Maintenance.aspx') >= 0) {
        activePage = 'install-maintenance';
    }
    else if (window.location.href.indexOf('Filters/Manage.aspx') >= 0) {
        activePage = 'report-filter-manager';
    }
    else if (window.location.href.indexOf('Invitations/Manage.aspx') >= 0) {
        activePage = 'invitation-manager';
    }
    else if (window.location.href.indexOf('Reports/Manage.aspx') >= 0) {
        activePage = 'report-manager';
    }
}

function getFilterItemType() {
    var itemType = '';

    if (activePage == '') {
        activePage = setActivePage();
    }
    switch (activePage) {
        case 'survey-manager':
            itemType = 'survey';
            break;
        case 'user-manager':
            itemType = 'user';
            break;
        case 'library-manager':
            itemType = 'library';
            break;
        case 'style-manager':
            itemType = 'style';
            break;
        case 'response-manager':
            itemType = 'response';
            break;
    }

    return itemType;
}

// Toggle left column's search
function toggleLeftColumnSearch() {
    switch (activePage) {
        case 'survey-manager':
        case 'user-manager':
        case 'library-manager':
        case 'style-manager':
            $('.leftPanel .viewport').addClass('has-search');
            $('#leftPanel_search').fadeIn('fast');
            break;
    }
}

function filterItemList(searchTerm) {
    var itemType = getFilterItemType();
    if (itemType == '') { // don't filter if no item type is found
        return false;
    }
    switch (itemType) {
        case 'survey':
            var $itemsToShow = $('#listPlace .groupContent .listTemplateName .survey-name');
            var $itemsToHide = $();
            if (searchTerm != '') { // filter based on query
                var containsSel = ":Contains('" + searchTerm + "')";
                $itemsToHide = $itemsToShow.filter(":not(" + containsSel + ")");
                $itemsToShow = $itemsToShow.filter(containsSel);
            }
            $itemsToHide.closest('.groupContent').hide();
            $itemsToShow.closest('.groupContent').show();
            break;
        case 'user':
        case 'library':
        case 'style':
            var $itemsToShow = $('.leftPanel .ckbxGrid .gridContent');
            var $itemsToHide = $();
            if (searchTerm != '') { // filter based on query
                var containsSel = ":Contains('" + searchTerm + "')";
                $itemsToHide = $itemsToShow.children().filter(":not(" + containsSel + ")").parent();
                $itemsToShow = $itemsToShow.children().filter(containsSel).parent();
            }
            $itemsToHide.hide();
            $itemsToShow.show();
            break;
    }
}

// Generate a universal message for display above the header
function generateUniversalMessage(id, content) {
    return '<div data-message-id="' + id + '" class="header-message"><a class="close-message" href="#"></a>' + content + '</div>';
}

/********************************************************
*                   INITIALIZATIONS
*********************************************************/

$(document).ready(function () {

    /********************************************************
    *                       GENERAL
    *********************************************************/

    setActivePage();
    toggleLeftColumnSearch();

    $liveSearch = new LiveSearchObj('left_column_search', getFilterItemType());

    /* Universal Header Messages */

    // Close messages
    $headerMessages = $('#header_messages');
    $(document).on('click', '#' + $headerMessages.attr('id') + ' .close-message', function (e) {
        e.preventDefault();
        var $msg = $(this).closest('.header-message');
        $msg.animate({ height: 0, opacity: 0 }, function () {
            // Set a cookie to prevent re-displaying this message in the future
            var postId = $(this).attr('data-message-id');
            window.localStorage.setItem('lastMessagePost', postId);
            $(this).remove();
        });
    });

    // Check for "customer-updates" to be shown as header messages
    //$.ajax({
    //    //get information about the user usejquery from twitter api
    //    url: 'https://www.checkbox.com/api/core/get_category_posts/?callback=?&slug=customer-updates&count=1',
    //    dataType: 'json',
    //    success: function (json) {
    //        if (typeof json.posts != 'undefined') {
    //            var lastPostId = window.localStorage.getItem('lastMessagePost');

    //            $.each(json.posts, function (idx, postInfo) {
    //                // If a user has designated to hide this message, don't display it
    //                if (postInfo.id <= parseInt(lastPostId)) {
    //                    return;
    //                }
    //                var $msg = $(generateUniversalMessage(postInfo.id, '<a target="_blank" href="' + postInfo.url + '">' + postInfo.title + '<span class="read-more">Read More &raquo;</span></a>'));
    //                $msg.css('height', '0');
    //                $headerMessages.append($msg);
    //                $msg.animate({
    //                    height: '2em'
    //                },
    //                {
    //                    complete: function () {
    //                        resizePanels();
    //                    }
    //                });
    //            });
    //        }
    //    },
    //    error: function (ex, status, error) {
    //        console.log(error);
    //    }
    //});
    /**
    * Panel Sizing
    */

    var $leftPanel = $('.leftPanel');
    var $rightPanel = $('.rightPanel');
    if ($leftPanel.length > 0 && $rightPanel.length > 0) {
        // Enable auto-resizing & scrollable content for left/right panels
        //    $leftPanel.tinyscrollbar({ axis: 'y' });
        //     $rightPanel.tinyscrollbar({ axis: 'y' });
        resizePanels();
        $leftPanel.mouseenter(function () {
            var content = $(this).closest('.pageContent');
            if (content.hasClass(collapsedLeftClass) && !content.hasClass('doNotRemoveCollapsedLeftClass')) {
                toggleLeftPanel(true, true);
            }
        }).mouseleave(function () {
            if (($(window).width() < leftColumnCollapseAtPageWidth) && !$(this).closest('.pageContent').hasClass('doNotRemoveCollapsedLeftClass')) {
                var leaveTOLength = leftResizing ? 500 : 0;
                var mouseleaveLeftTimeout = setTimeout(function () {
                    toggleLeftPanel(false);
                }, leaveTOLength);
            }
        });
        $(window).resize(function () {
            resizePanels();
        });
        // Manual collapse/expand button for left panel
        $('#leftColumn_toggle').click(function (e) {
            userAlteredColumn = true;
            toggleLeftPanel(null, false, true);
        });
    }

    /**
    * Header Functionality
    */

    $('#headerSearchText').keypress(function (e) {
        if (e.which == 13) {
            onHeaderSearchClick();
            return false;
        }
    });

    $('#loginName').click(function (e) {
        e.stopPropagation();
        if ($('#loginContainer').hasClass('open')) {
            $('#loginContainer').removeClass('open');
            $('.loginInfoContainer').hide();
        }
        else {
            $('#loginContainer').addClass('open');
            $('.loginInfoContainer').show();
        }
    });

    $('body').click(function () {
        $('#loginContainer').removeClass('open');
        $('.loginInfoContainer').hide();
    });


    /**
    * Right Column Headers
    */
    $(document).click(function (e) {
        var $target = $(e.target);

        if ($target.parent().hasClass('popup'))
            return;

        if (!($target.hasClass('action-menu-toggle') && $target.parent().hasClass('header-content'))) {
            $(surveyHeaderMenuSel).hide();
        }
        
    });

});

/**
 * Panel Sizing
 */

function toggleLeftPanel(show, forced, userTriggered) {
    var $content = $('.pageContent');
    var $right = $('.rightPanel');
    
    forced = (typeof forced != 'undefined') ? forced : false;
    userTriggered = (typeof userTriggered != 'undefined') ? userTriggered : false;
    show = userTriggered ? $content.hasClass(collapsedLeftClass) : show;

    if (overNameClone || panelsBeingResized || leftResizing || (!userTriggered && userAlteredColumn)) {
        return false;
    }

    if (forced) {
        $content.addClass(forceCollapsedLeftClass);
    }

    if (userTriggered && !$content.hasClass(collapsedLeftClass) &&  $content.hasClass(forceCollapsedLeftClass)) {
        $content.removeClass(forceCollapsedLeftClass);
        $('.left-header .toggle-column').css('background-image', '');
        return false;
    }

    var rightWidth = $right.width();
    var $surveyHeader = $('#infoPlace .survey-header-container');
    var headerLeft = $surveyHeader.length > 0 ? $surveyHeader.offset().left : null;
    if (show && $content.hasClass(collapsedLeftClass)) {
        leftResizing = true;
        $content.addClass(leftResizingClass);
        rightWidth -= (expandedLeftWidth - collapsedLeftWidth);
        $right.css('width', rightWidth + 'px');
        if (headerLeft != null) {
            headerLeft += (expandedLeftWidth - collapsedLeftWidth);
            $surveyHeader.css({ left: headerLeft + 'px', width: (rightWidth-20) + 'px' });
        }
        $content.removeClass(collapsedLeftClass);
        if (userTriggered) {
            $('.left-header .toggle-column').css('background-image', '');
        }

        var resizeCompleteTimeout = setTimeout(function () {
            $content.removeClass(leftResizingClass);
            leftResizing = false;
            //$content.removeClass(forceCollapsedLeftClass);
            $('.leftPanel .overview').show();
            if ($content.find('#styleEditorAccordion, #reportNavigatorPlace, #surveyNavigatorPlace').length <= 0) {
                $('#leftPanel_search').show();
            }
            $('.surveys-filter-buttons').show();
            $('.header-actions').show();
            $('.left-header').css('width', '');
            resizePanels();
        }, 500);
    }
    else if (!show && (!$content.hasClass(collapsedLeftClass) || $content.hasClass(forceCollapsedLeftClass))) {
        leftResizing = true;
        $content.addClass(leftResizingClass);
        rightWidth += (expandedLeftWidth - collapsedLeftWidth);
        $right.css('width', rightWidth + 'px');
        if (headerLeft != null) {
            headerLeft -= (expandedLeftWidth - collapsedLeftWidth);
            $surveyHeader.css({ left: headerLeft + 'px', width: (rightWidth - 20) + 'px' });
        }
        $content.addClass(collapsedLeftClass);
        $('.left-header .toggle-column').css('background-image', 'url(' + _appRoot + '/App_Themes/CheckboxTheme/Images/2arrows_right.png)');

        var resizeCompleteTimeout = setTimeout(function () {
            $content.removeClass(leftResizingClass);
            leftResizing = false;
            if (!$content.hasClass('doNotRemoveCollapsedLeftClass')) {
                $content.removeClass(forceCollapsedLeftClass);
                $('.leftPanel .overview').hide();
            }
            resizePanels();
            $('.surveys-filter-buttons').hide();
            $('#leftPanel_search').hide();
            $('.header-actions').hide();
            $('.left-header').css('width', '40px');
        }, 500);
    }
}

function resizePanels() {
    $.Deferred(function (dfd) {

        if (leftResizing) {
            return false;
        }

        //fix for AvailableSurveys.aspx and AvailableReports.aspx, wich do not need such resizing
        if ($('.avaliable-items-container').length > 0) {
            return false;
        }

        if (toggleLeftTimeout != null) {
            clearTimeout(toggleLeftTimeout);
        }
        panelsBeingResized = true;

        var $content = $('.pageContent');

        // Collapse the left column if the window width is < leftColumnCollapseAtPageWidth
        if (!$content.hasClass(forceCollapsedLeftClass)) {
            toggleLeftTimeout = setTimeout(function () {
                toggleLeftPanel(($(window).width() > leftColumnCollapseAtPageWidth));
            }, 500);
        }

        var leftWidth = $('.leftPanel').outerWidth() + 1; //.rightPanel(border-left of 1px)

        //Start with default values
        var headerHeight = 45;          //45 is default height of header
        var footerHeight = 0;

        //Attempt to populate with real values
        if ($('#header').length > 0) {
            headerHeight = $('#header').height();
        }
        if ($('#footer').length > 0) {
            footerHeight = $('#footer').height();
        }

        var panelHeight = $(window).height() - headerHeight - footerHeight; 

        //pageContent width/height should always be set
        //$('.pageContent').width($(window).width());
        $content.height(panelHeight);

        if ($('.leftPanel').is(':visible')) {
            //leftPanel widths
            //$('.leftPanel > .viewport').width($('.leftPanel').width());
            //leftPanel height
            $('.leftPanel').height(panelHeight - 15); // - $('.pageHeaderContainer').height() - 8);

            switch (activePage) {
                case 'survey-manager':
                case 'user-manager':
                case 'style-manager':
                case 'library-manager':
                case 'report-filter-manager':
                case 'response-manager':
                case 'report-manager':
                case 'invitation-manager':
                case 'install-maintenance':
                    $('.leftPanel > .viewport').height(panelHeight - $('#footer').height() - 10);
                    break;
                case 'survey-editor':
                    $('.leftPanel > .viewport').height(panelHeight - 38);
                    $('.leftPanelScrollFix').width($('.overview').outerWidth());
                    break;
            }
        }
        //rightPanel width/height should always be set
        // '-17' for the scroll place, otherwise right panel can fall
        var rightWidth = $(window).width() - 17 - leftWidth;

        //rightWidth = (rightWidth < 676)  ? 676 : rightWidth;
        $('.rightPanel').width(rightWidth);
        //$('.rightPanel > .viewport').width(rightWidth);
        //                if (panelHeight < 714) {
        //                    panelHeight -= 35;
        //                }
        $('.rightPanel').height(panelHeight);
        $('.rightPanel > .viewport').height(panelHeight);
        $('#detailContentContainer').height(panelHeight - 100);

        //$('.rightPanel > .viewport > .overview').width(rightWidth);

        //
        // TODO: Is this correct?
        //
        //fix to avoid setting negative width value 
        if ((rightWidth - 36) > 0) 
        {
            $('#infoPlace .survey-header-container').css('width', (rightWidth - 36) + 'px');
        }
        dfd.resolve();

        panelsBeingResized = false;

    }).promise().then(function () {
        //Petr: this line leads to exception in javascript in Chrome at the Survey Manager
        //$('.leftPanel').tinyscrollbar_update('relative');
    });}

function closeLeftPanel() {
    $('.leftPanel').animate({ 'width': 0 }, '300', function () {
        $('.leftPanel').hide();
        $('.rightPanel').css('height', $(window).height() - 52 - 35);
    });
    $('.rightPanel').animate({ 'width': $(window).width() - 1 }, '100', function () {
        $('.openLeftPanel').show();
        $('.openLeftPanel').animate({ opacity: 1 }, '300', function () {
            setTimeout(function () { $('.openLeftPanel').animate({ opacity: 0 }, '3000'); }, 3000);
        });
    });
}

function openLeftPanel() {
    $('.leftPanel').show();
    $('.leftPanel').animate({ 'width': 570 }, '300', function () {
        resizePanels();
        $('.openLeftPanel').hide();
    });
    $('.rightPanel').animate({ 'width': $(window).width() - 571 }, '100');
}
