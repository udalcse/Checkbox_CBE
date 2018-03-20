
// Get response grid info
var $responsePagination = null;
var $activeResponsePage = null;
var numResponsePages = 0;
var $activeResponseRow = null;

function getResponsePaginationInfo() {
    // Response table pagination
    if ($responsePagination == null) {
        $responsePagination = $('.leftPanel .pagination:first');
        numResponsePages = $responsePagination.find('li.active, li.inactive').length;
    }
    $activeResponsePage = $responsePagination.find('li.active');
    var currPage = parseInt($.trim($activeResponsePage.text()));
    var $responseGrid = $('.leftPanel .ckbxGrid');
    var $responseRows = $responseGrid.find('.gridContent');
    var numRows = $responseRows.length;

    $activeResponseRow = $responseRows.filter('.gridActive');
    var selReponseIdx = $responseRows.index($activeResponseRow);
    var firstSelected = selReponseIdx == 0;
    var lastSelected = (selReponseIdx + 1) == numRows;

    return { currPage: currPage, selRowIdx: selReponseIdx, firstSelected: firstSelected, lastSelected: lastSelected };
}

// Select  a new response row
function selectResponseRow(idx) {
    var $responseGrid = $('.leftPanel .ckbxGrid');
    var $responseRows = $responseGrid.find('.gridContent');
    switch (idx) {
        case 'first':
            $responseRows.first().click();
            break;
        case 'last':
            $responseRows.last().click();
            break;
        default:
            $responseRows.eq(idx).click();
            break;
    }
}

// Switch the response currently being viewed
function switchResponseView(prevNext) {
    var pgInfo = getResponsePaginationInfo();
    var firstSelected = pgInfo.firstSelected;
    var lastSelected = pgInfo.lastSelected;

    switch (prevNext) {
        case 'prev':
            switch (firstSelected) {
                case true:
                    $activeResponsePage.parent().find('.previous > a').click();
                    // TODO: Ensure this runs as soon as the page finishes changing
                    setTimeout(function () {
                        selectResponseRow('last');
                        setViewResponsePagination();
                    }, 1000);
                    break;
                default: // false
                    $activeResponseRow.prev().click();
                    break;
            }
            break;
        case 'next':
            switch (lastSelected) {
                case true:
                    $activeResponsePage.parent().find('.next > a').click();
                    // TODO: Ensure this runs as soon as the page finishes changing
                    setTimeout(function () {
                        selectResponseRow('first');
                        setViewResponsePagination();
                    }, 1000);
                    break;
                default: // false
                    $activeResponseRow.next().click();
                    break;
            }
            break;
    }
}

// Set pagination info in a response view's header
function setViewResponsePagination() {
    var pgInfo = getResponsePaginationInfo();
    var currPage = parseInt(pgInfo.currPage);
    var selRowIdx = parseInt(pgInfo.selRowIdx);
    var currResponseNum = ((currPage - 1) * _pageSize) + selRowIdx + 1;

    $('#view_current_response_num').text(currResponseNum);
    $('#view_total_response_num').text(grid_totalItemCount); // this global value is defined in Grid.ascx

    // View first or last result, don't change
    $('div.response-navigation a').show();
    if (currResponseNum === 1) {
        $('div.response-navigation a.prev').hide();
        return;
    }
    if (currResponseNum === grid_totalItemCount) { // this global value is defined in Grid.ascx
        $('div.response-navigation a.next').hide();
        return;
    }
}

$(document).ready(function () {

    // Alter the page display to show a full-width grid
    activateFullPageGrid();

    // Single Response - Paging in Header
    var responseHeaderSel = "#view_response_header";
    $(document).on('click', responseHeaderSel + ' .navigate-link', function (e) {
        e.preventDefault();

        var navDirection = $(this).hasClass('prev') ? 'prev' : 'next';
        switchResponseView(navDirection);

        setViewResponsePagination();
    });

});