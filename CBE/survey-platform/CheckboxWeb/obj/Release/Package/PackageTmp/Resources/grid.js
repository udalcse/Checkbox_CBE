/****************************************************************************
 * Helper method/object for grid rendering.  Requires jQuery template       *
 *   helper class. Expects grid to be renderered as sequence of <ul>        *
 *   elements.                                                              *
 ****************************************************************************/
var gridHelper = new gridHelperObj();


//Helper class
function gridHelperObj() {
    this.currentPage = 1;
    this.sortField = '';
    this.sortAscending = true;
    this.filterField = '';
    this.filterValue = '';
    this.isAjaxScrollingMode = false;
    this.filterKey = '';
    
    //Render the grid
    this.renderGrid = function(data, containerId, listTemplateName, listTemplatePath, args, renderCompleteCallback) {
        if (templateHelper == null) {
            alert('Template helper js include is required by gridHelper.');
        }

        if (data == null) {
            data = new Array();
        }

        var onTemplateAppliedArgs = { baseArgs: args, containerId: containerId, renderCompleteCallback: renderCompleteCallback };

        //Note that any child templates used by list template should be preloaded
        // by calling code.
        templateHelper.loadAndApplyTemplate(
            listTemplateName,
            listTemplatePath,
            { containerId: containerId, items: data },
            args,
            containerId,
            this.isAjaxScrollingMode == false,
            this.onTemplateApplied,
            onTemplateAppliedArgs);
    };

    //Bind click events
    this.onTemplateApplied = function (args) {
        if (args == null || args.baseArgs == null) {
            return;
        }

        if (args.baseArgs.sortClick != null) {
            $('#' + args.containerId + ' > ul:first-child > li[sortField]').click(function () {
                args.baseArgs.sortClick($(this).attr('sortField'));
            });
        }

        if (typeof (args.baseArgs.arrowStyle) == 'undefined' && args.baseArgs.arrowStyle == null) {
            args.baseArgs.arrowStyle = 'none';
        }

        //Remove all event handler attached below
        $(document).off("**", '#' + args.containerId + ' > div.gridContent');

        if (args.baseArgs.itemClick != null) {

            var hoverClass = '';

            if (args.baseArgs.arrowStyle.toLowerCase() == 'none') {
                hoverClass = 'gridHoverNoArrow';
                $(document).on('mouseenter', '#' + args.containerId + ' div.gridContent', function () {
                    $(this).addClass(hoverClass);
                });
                $(document).on('mouseleave', '#' + args.containerId + ' div.gridContent', function () {
                    $(this).removeClass(hoverClass);
                });

                /*
                NWC 2011-07-20 -- Restored prior way of selecting elements.  Selecting based only on div.gridContent causes hover action to
                --  to be applied to ALL grids on a page, regardless of whether individual grids have itemClick events 
                --  defined. That's the reason for the somewhat obtuse selector, to ensure only this particular grid
                --  gets events bound.                      

                */
            }



            /*
            NWC 2011-07-20  -- Updated selector for click event. Binding click to div.gridContent causes a click event to be bound
            --  for each grid on a page.  This causes multiple events to fire per click on pages such as survey 
            --  dashboard that contain multiple grids.
            */


            //Bind click event -- $('#' + args.containerId + ' > div:first-child ~ div, )   ('#' + args.containerId + ' > )

            //Ensure event unbound first
            $(document).off('click', '#' + args.containerId + ' div.gridContent');

            //Bind
            $(document).on('click', '#' + args.containerId + ' div.gridContent', function (e) {
                if (e == null || typeof (e) == 'undefined')
                    e = event;
                if ((typeof(event) != 'undefined') && $(event.target).length > 0 && ($(event.target).children(':checkbox').length > 0 || $(event.target).is(':checkbox')))
                    return true;
                if ($(event.target).hasClass('orangeButton'))
                    return;
                //Remove active style from all
                $('div.gridContent').removeClass('gridActive');
                $('div.gridContent').removeClass('gridActiveLeft');
                $('div.gridContent').removeClass('gridNoArrow');

                if (args.baseArgs.arrowStyle.toLowerCase() == 'right') {
                    //Add active style to selected item 
                    $(this).addClass('gridActive');
                }
                else if (args.baseArgs.arrowStyle.toLowerCase() == 'left') {
                    //Add active style to selected item 
                    $(this).addClass('gridActiveLeft');
                }
                else {
                    $(this).addClass('gridNoArrow');
                }

                //Fire click
                args.baseArgs.itemClick($.tmplItem($(this)).data);
            });
        }

        //Call resize 
        if (typeof (resizePanels) == 'function') {
            resizePanels();
        }

        if (args.renderCompleteCallback) {
            args.renderCompleteCallback();
        }
    };

    //Update sort parameter and page
    this.updateSort = function(newSortField) {
        this.currentPage = 1;

        if (newSortField == this.sortField) {
            this.sortAscending = !this.sortAscending;
        }
        else {
            this.sortField = newSortField;
            this.sortAscending = true;
        }
    };
}