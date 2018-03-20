/******************************************************************
 * Build a pager out of a specified unordered list.               *
 ******************************************************************/

/******************************************************************
See Below for List of Settings
*******************************************************************/


(function ($) {
    var methods = {
        init: function(options) {

            //Supported setting default values
            var settings = {
                currentPage: 1,
                pageSize: 25,
                totalItems: 1,
                pageSelectorCount: 10,
                pageChanged: null
            };

            return this.each(function() {

                try {
                    //Merge options with settings
                    if (options) {
                        $.extend(settings, options);
                    }

                    var $this = $(this);

                    $this.empty();

                    //Calculate number of pages
                    // | 0 is bitwise OR with 0 which has effect of converting result to int by
                    // truncating decimal portion
                    var pageCount = (settings.totalItems / settings.pageSize) | 0;

                    if (pageCount == 0 || settings.totalItems % settings.pageSize != 0) {
                        pageCount++;
                    }

                    /*** we will allow this to be built as the page numbers are useful
                    for some grid pages, individual grids will be responsible for
                    hiding the paging element ***
                    if (pageCount == 1) {
                    return;
                    }*/

                    //Create next/previous elements
                    var previous;
                    var next;

                    if (settings.currentPage == 1
                        || pageCount == 1) {
                        previous = $('<li class="previous-off">&lt;&lt;</li>');
                    } else {
                        previous = $('<li class="previous"><a href="javascript:void(0);">&lt;&lt;</a></li>');
                    }

                    if (settings.currentPage == pageCount) {
                        next = $('<li class="next-off">&gt;&gt;</li>');
                    } else {
                        next = $('<li class="next"><a href="javascript:void(0);">&gt;&gt;</a></li>');
                    }

                    //Determine first/last page in individual selectors
                    var pageCountLeft = (settings.pageSelectorCount / 2) | 0;
                    var pageCountRight = settings.pageSelectorCount - pageCountLeft;


                    //Handle case where selected page is closer than selectorCount /2 from start
                    if (settings.currentPage - pageCountLeft <= 1) {
                        pageCountLeft = settings.currentPage - 1;
                        pageCountRight = settings.pageSelectorCount - pageCountLeft;
                    }

                    //Handle case where selected page is closer than selectorCount /2 from end
                    if (settings.currentPage + pageCountRight >= pageCount) {
                        pageCountRight = pageCount - settings.currentPage;
                        pageCountLeft = settings.pageSelectorCount - pageCountRight;
                    }


                    var firstPage = Math.max(1, settings.currentPage - pageCountLeft);
                    var lastPage = Math.min(pageCount, settings.currentPage + pageCountRight);

                    //Handle case where selected page is closer than selectorCount /2 from start/end
                    if (firstPage == 1
                        && pageCount >= settings.pageSelectorCount) {
                        lastPage = settings.pageSelectorCount;
                    }

                    //Figure out of we need First Page & Last Page Explicit Selectors
                    var firstPageSelectorRequired = (firstPage > 1);
                    var lastPageSelectorRequired = (lastPage < pageCount);

                    //Previous Page
                    $(this).append(previous);

                    //First page selector
                    if (firstPageSelectorRequired) {
                        $(this).append('<li class="inactive"><a href="javascript:void(0);">1</a></li>');
                        $(this).append('<li>...</li>');
                    }

                    //Individual pages
                    for (var pageNumber = firstPage; pageNumber <= lastPage; pageNumber++) {
                        if (pageNumber == settings.currentPage) {
                            $(this).append('<li class="active">' + pageNumber + '<li>');
                        } else {
                            $(this).append('<li class="inactive"><a href="javascript:void(0);">' + pageNumber + '</a><li>');
                        }
                    }

                    //Last page
                    if (lastPageSelectorRequired) {
                        $(this).append('<li>...</li>');
                        $(this).append('<li class="inactive"><a href="javascript:void(0);">' + pageCount + '</a></li>');
                    }

                    //Next button
                    $(this).append(next);
                    //Bind click events
                    if (settings.pageChanged) {
                        $this.children('.next').children('a').click(function() {
                            settings.pageChanged(settings.currentPage + 1);
                        });

                        $this.children('.previous').children('a').click(function() {
                            settings.pageChanged(settings.currentPage - 1);
                        });

                        $this.children('.inactive').children('a').click(function() {
                            settings.pageChanged(parseInt($(this).html()));
                        });
                    }
                } catch(err) {
                    alert(err.message);
                }
            });
        }
    };

    $.fn.pager = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        }
    };
})(jQuery);