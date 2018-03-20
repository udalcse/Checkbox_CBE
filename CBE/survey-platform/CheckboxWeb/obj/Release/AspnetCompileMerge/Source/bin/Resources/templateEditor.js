//Useful object for handling some tasks of template editing
var templateEditor = new templateEditorObj();

function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}
function guid() {
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

////////////////////////////////////////////////////////////////////////////////
// Container for template editor javascript routines.  Requires jQuery.       //
////////////////////////////////////////////////////////////////////////////////
function templateEditorObj() {

    this.appRoot = '/';
    this.selectedItemId = -1;
    this.currentLanguage = 'en-US';
    this.totalPageCount = 1;
    this.currentPageCount = 0;
    this.editRoot = '';

    this.itemTitleElemId = '';
    this.detailElemId = '';
    this.settingsElemId = '';
    this.itemFrameId = '';
    this.progressElemId = '';
    this.welcomeElemId = '';
    this.mode = '';
    this.defaultItemView = 'editor';
    this.confirmation = "False";
    this.authToken = '';
    this.navPanelElemId = '';
    this.detailMenuContainerId = '';
    this.filtersElemId = '';

    this.templateId = -1;
    this.isReadOnly = false;

    this.pageToDelete = null;
    this.itemToDelete = null;
    this.args = null;
    this.itemTypeElemId = '';
    this.itemIdElemId = '';

    this.baseOpenWindowArgs = null;

    this.callbacks = new Array();
    this.jqTemplateNames = new Array();
    this.jqTemplatePaths = new Array();

    //0-based index of first template "content page"  For surveys this will be one, for all
    // other templates, 0
    this.firstContentPageIndex = 0;

    this.lastRefreshedPage = -1;

    ///////////////////////////////////////////////////////////////////////////
    // Initialize with application root url of child action window.          //                                                    
    ///////////////////////////////////////////////////////////////////////////
    this.initialize = function (authToken, appRoot, editRoot, mode, baseOpenWindowArgs, itemFrameId, itemTitleElemId, itemTypeElemId, itemIdElemId, detailElemId, welcomeElemId, progressElemId, detailMenuContainerId, settingsElemId, filtersElemId) {
        templateEditor.appRoot = appRoot;
        templateEditor.editRoot = editRoot;
        templateEditor.itemTitleElemId = itemTitleElemId;
        templateEditor.detailElemId = detailElemId;
        templateEditor.settingsElemId = settingsElemId;
        templateEditor.welcomeElemId = welcomeElemId;
        templateEditor.mode = mode;
        templateEditor.baseOpenWindowArgs = baseOpenWindowArgs;
        templateEditor.itemFrameId = itemFrameId;
        templateEditor.progressElemId = progressElemId;
        templateEditor.authToken = authToken;
        templateEditor.detailMenuContainerId = detailMenuContainerId;
        templateEditor.itemTypeElemId = itemTypeElemId;
        templateEditor.itemIdElemId = itemIdElemId;
        templateEditor.filtersElemId = filtersElemId;
    };

    ///////////////////////////////////////////////////////////////////////////
    // Register callback for various template-related operations.            //
    ///////////////////////////////////////////////////////////////////////////
    this.registerTemplate = function(templateId, compiledTemplateName, compiledTemplatePath) {
        templateEditor.jqTemplateNames[templateId] = compiledTemplateName;
        templateEditor.jqTemplatePaths[templateId] = compiledTemplatePath;
    };

    ///////////////////////////////////////////////////////////////////////////
    // Register callback for various template-related operations.            //
    ///////////////////////////////////////////////////////////////////////////
    this.registerCallback = function(callbackId, callback) {
        templateEditor.callbacks[callbackId] = callback;
    };

    ///////////////////////////////////////////////////////////////////////////
    // Run callback if found.  Accept up to 10 argments to pass as well.     //
    ///////////////////////////////////////////////////////////////////////////
    this.runCallback = function(callbackId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) {
        if (templateEditor.callbacks[callbackId] != null) {
            templateEditor.callbacks[callbackId](arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }
    };

    ///////////////////////////////////////////////////////////////////////////
    // Merge arrays of objects. The source arrays aren't modified.           //
    ///////////////////////////////////////////////////////////////////////////
    this.mergeArrayOfObjects = function (arr1, arr2) {
        var result = $.extend([], arr1);
        for (var i in arr2) {
            var used = false;
            for (var j in result) {
                if (result[j].name == arr2[i].name) {
                    result[j].value = arr2[i].value;
                    used = true;
                }
            }

            if (!used) {
                result.push(arr2[i]);
            }
        }

        return result;
    }

    ///////////////////////////////////////////////////////////////////////////
    // Open child window with specified URL and params.  Params should be an //
    //  array of name/value elements.                                        //
    ///////////////////////////////////////////////////////////////////////////
    this.openChildWindow = function (itemId, pageId, baseUrl, additionalParams, windowId, canCloseCallback, closeBtnCallback) {
        if (additionalParams == null) {
            additionalParams = new Array();
        }

        additionalParams = templateEditor.mergeArrayOfObjects(new Array(
                                                { name: 'l', value: templateEditor.currentLanguage },
                                                { name: 'onClose', value: 'templateEditor.onDialogClosed' }),
                                                additionalParams);

        var urlParam;

        if (baseUrl.indexOf('?') > 0) {
            urlParam = '&';
        }
        else {
            urlParam = '?';
        }

        //Sanitize args
        if (itemId == null) {
            itemId = '';
        }

        if (pageId == null) {
            pageId = '';
        }

        //Set base url
        var url = baseUrl + urlParam + 'i=' + itemId + '&p=' + pageId;

        //Add base args
        url = templateEditor.addParamsToUrl(url, templateEditor.baseOpenWindowArgs);
        url = templateEditor.addParamsToUrl(url, additionalParams, true);

        //Set url and open
        var height = 600;
        var width = 800;
        if (windowId && windowSizes[windowId]) {
            height = windowSizes[windowId].height;
            width = windowSizes[windowId].width;
        }

        var options = {
            modal: true,
            minHeight: height,
            minWidth: width,
            maxHeight: height,
            maxWidth: width,
            appendTo: '#aspnetForm',
            containerId: 'ckbx_dialogContainer',
            closeHTML: '<div class="ckbx_dialogTitleBar"><a href="javascript:void(0);" class="simplemodal-close roundedCorners ckbx-closedialog">CLOSE</a><br class="clear" /></div>',
            closeClass: 'ckbx-closedialog',
            onOpen: function (dialog) {
                dialog.overlay.fadeIn('300');
                dialog.container.fadeIn('300');
                dialog.data.fadeIn('300');
            },
            onClose: function (dialog) {
                if (canCloseCallback != null) {
                    if (typeof (canCloseCallback) === 'function') {
                        if (!canCloseCallback()) {
                            return;
                        }
                    }
                }

                dialog.overlay.fadeOut('300');
                dialog.container.fadeOut('300');
                dialog.data.fadeOut('300', function () {
                    $.modal.close();
                });
            },
            onShow: function () {

                //To prevent close click on modal header
                //since it is a part of header content it should be moved to modal template html markup or disable closing on header click
                $('.ckbx_dialogTitleBar.ckbx-closedialog').off();
                //setting event on close button to return prev value on dropdown menu
                $('.simplemodal-close.roundedCorners.ckbx-closedialog').on('click', function() {
                    if (closeBtnCallback != null) {
                        if (typeof (closeBtnCallback) === 'function') {
                            closeBtnCallback();
                        }
                    }
                });

                //To avoid IE exceptions we should set iframe source after the modal show
                $('#templateEditorModal').attr("src", url);
            }
        };

        //height and width must be adjusted for the dialog header and content padding
        $.modal('<iframe id="templateEditorModal" src="" height="' + (height - 30) + '" width="' + width + '" style="border:0;" scrolling="no">', options);
    };

    /////////////////////////////////////////////////////////////////////////
    // Add elements from params array to URL                               //
    /////////////////////////////////////////////////////////////////////////
    this.addParamsToUrl = function (url, params, skipReplacement) {
        if (url == null) {
            url = '';
        }

        //TODO: URL Encode values

        if (params != null && params.length > 0) {
            for (var i = 0; i < params.length; i++) {
                var param = params[i];

                if (param.name != null
                    && param.name != ''
                        && param.value != null
                            && param.value != '') {
                    url += '&' + param.name + '=' + param.value;
                }
            }
        }

        if (!skipReplacement) {
            return url.replace('//', '/');
        }
        else {
            return url;
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Load navigation into specified container.                           //
    /////////////////////////////////////////////////////////////////////////
    this.loadNavigation = function (navContainer, templateId) {
        templateEditor.navPanelElemId = navContainer;
        templateEditor.templateId = templateId;

        templateEditor.loadTemplateNavigation(
            templateEditor.authToken,
            templateId,
            navContainer,
            templateEditor.itemFrameId,
            templateEditor.defaultItemView,
            templateEditor.progressElemId,
            function () {

                if (templateEditor.isReadOnly)
                    return;

                $('#pageNavList').sortable({
                    handle: '.upDownArrowDrag',
                    axis: 'y',
                    update: templateEditor.onPageDrop
                });
                $('.pageItems').sortable({
                    handle: '.itemContainer',
                    connectWith: '.pageItems',
                    axis: 'y',
                    update: templateEditor.onItemDrop,
                    cursor: 'url(../../App_Themes/CheckboxTheme/Images/dragHand.png) !important'
                });

                templateEditor.toggleAllPages(true);
            });

        //Bind click events
        $('.copyPageLink').bind('click', function () {
            templateEditor.onCopyPageClick($(this));
        });

        $('.deletePageLink').bind('click', function () {
            templateEditor.onDeletePageClick($(this));
        });
    };

    /////////////////////////////////////////////////////////////////////////
    // Load navigation information for temlate and put it in the specified //
    //  panel.  Requires that callbacks to access data have been           //
    //  registered and that any jq temlates have been loaded/compiled.     //
    /////////////////////////////////////////////////////////////////////////
    this.loadTemplateNavigation = function(authTicket, templateId, navPanelId, containerFrameId, defaultItemView, progressTemplateId, callback, callbackArgs) {
        templateEditor.currentPageCount = 0;
        templateEditor.totalPageCount = 1;

        var args =
        {
            authTicket: authTicket,
            navPanelId: navPanelId,
            containerFrameId: containerFrameId,
            defaultView: defaultItemView,
            progressTemplateId: progressTemplateId,
            callback: callback,
            callbackArgs: callbackArgs,
            templateId: templateId
        };

        templateEditor.args = args;

        //Load template data
        templateEditor.runCallback('getTemplateMetaData', authTicket, templateEditor.onTemplateNavDataLoaded, args);

       

        $(".statistics_ExpandPages").click(function () {
            $(this).toggleClass("expand-down-angle-arrow");

            if ($(this).text() === "Collapse All") {
                $(this).text("Expand All");
                templateEditor.toggleAllPages(false);
            } else {
                $(this).text("Collapse All");
                templateEditor.toggleAllPages(true);
            }
        });
       
    };

    this.toggleAllPages = function(expandAll) {
        $("[id*='pageHeader']").each(function () {
            var arguments = { pageId: $.tmplItem(this).data.Id, baseArgs: templateEditor.args }
            if (expandAll) {
                templateEditor.togglePage($.tmplItem(this).data, arguments, true, false);
            } else {
                templateEditor.togglePage($.tmplItem(this).data, arguments, false, true);
            }

        });
    }


    //////////////////////////////////////////////////////////////////////////
    // Apply template to have data and place output in nav panel specified  //
    //  in args.                                                            //
    //////////////////////////////////////////////////////////////////////////
    this.onTemplateNavDataLoaded = function(templateNavData, args) {
        templateEditor.totalPageCount = templateNavData.PageIds.length;
        templateHelper.loadAndApplyTemplate(
            templateEditor.jqTemplateNames['pageListTemplate'],
            templateEditor.jqTemplatePaths['pageListTemplate'],
            templateNavData,
            null,
            args.navPanelId,
            true,
            templateEditor.onTemplateNavTemplateApplied,
            args);
    };

    ///////////////////////////////////////////////////////////////////////////
    // Populate page information after application of template nav template. //
    ///////////////////////////////////////////////////////////////////////////
    this.onTemplateNavTemplateApplied = function(args) {
        //Load basic page data
        $('.templateNavPage').each(function() {
            var elemId = $(this).attr('id');

            if (elemId != null) {
                templateEditor.populateNavPageInfo(elemId, elemId.replace('pageContainer_', ''), args);
            }
        });

        resizePanels();
    };

    /////////////////////////////////////////////////////////////////////////
    // Populate basic page data for nav panel                              //
    /////////////////////////////////////////////////////////////////////////
    this.populateNavPageInfo = function(pageContainer, pageId, args) {
        templateEditor.runCallback(
            'getTemplatePageMetaData',
            args.authTicket,
            pageId,
            templateEditor.onPageNavDataLoaded,
            { pageContainer: pageContainer, baseArgs: args });
    };

    /////////////////////////////////////////////////////////////////////////
    // Apply template to loaded page data.                                 //
    /////////////////////////////////////////////////////////////////////////
    this.onPageNavDataLoaded = function(pageData, args) {
        //Apply page template
        templateHelper.loadAndApplyTemplate(
            templateEditor.jqTemplateNames['pageTemplate'],
            templateEditor.jqTemplatePaths['pageTemplate'],
            pageData,
            { templateId: args.baseArgs.templateId, isReadOnly: templateEditor.isReadOnly, pageCount: templateEditor.totalPageCount },
            args.pageContainer,
            true,
            templateEditor.onPageNavTemplateApplied,
            { pageId: pageData.Id, baseArgs: args.baseArgs });
    };

    /////////////////////////////////////////////////////////////////////////
    // Apply template to loaded page data.                                 //
    /////////////////////////////////////////////////////////////////////////
    this.onPageNavTemplateApplied = function(args) {
        $('#pageItemsToggle_' + args.pageId).click(function() {
            templateEditor.togglePage($.tmplItem(this).data, args);
        });
        $('#pageHeader_' + args.pageId).click(function() {
            templateEditor.togglePage($.tmplItem(this).data, args);
        });
        templateEditor.currentPageCount++;

        if (templateEditor.currentPageCount >= templateEditor.totalPageCount) {
            if (args.baseArgs.callback) {
                args.baseArgs.callback(args.baseArgs.callbackArgs);
            }
        }

        resizePanels();
    };

    
   
    /////////////////////////////////////////////////////////////////////////
    // Show/hide page nav data in response to user click                   //
    /////////////////////////////////////////////////////////////////////////
    this.togglePage = function (pageData, args , expandElement, collapseElement) {
        //Do nothing if no items
        if (pageData == null
            || pageData.ItemIds == null) {
            return;
        }

        var pageItemsElem = $('#pageItems_' + pageData.Id);
        var pageLogicElem = $('#pageLogic_' + pageData.Id);
        var pageToggleElem = $('#pageItemsToggle_' + args.pageId);

        //Figure out if items have been rendered or not
        var itemsRendered = pageItemsElem.data('itemsRendered');

        if (itemsRendered == null) {
            //Add containers for items
            for (var i = 0; i < pageData.ItemIds.length; i++) {
                $('#pageItems_' + args.pageId).append('<div itemIndex="' + (i + 1) + '" id="itemPlace_' + pageData.ItemIds[i] + '" style="display:none;"></div>');
            }

            templateEditor.runCallback('openPage', pageData);

            pageItemsElem.show();
            pageLogicElem.show();
            pageToggleElem.parent().addClass('active'); 
            pageToggleElem.removeClass('pageArrowDown');
            pageToggleElem.addClass('pageArrowUp');

            templateEditor.loadPageItemsData(args.pageId, { pageId: args.pageId, baseArgs: args.baseArgs });

            $(pageItemsElem).data('itemsRendered', 'true');
           
            return;
        }

        // if expand element variable or collapse element are difened , no need to use toogle logic  
        if (expandElement) {
            pageItemsElem.show(
               'blind',
               null,
               'fast',
               function () {
                   pageLogicElem.show('blind', null, 'fast', function () {

                       pageToggleElem.parent().addClass('active');;
                       pageToggleElem.removeClass('pageArrowDown');
                       pageToggleElem.addClass('pageArrowUp');

                       resizePanels();
                   });
               });

            return;

        }
        else if (collapseElement) {
            pageItemsElem.hide(
             'blind',
             null,
             'fast',
             function () {
                 pageLogicElem.hide('blind', null, 'fast', function () {
                     pageToggleElem.parent().removeClass('active');
                     pageToggleElem.removeClass('pageArrowUp');
                     pageToggleElem.addClass('pageArrowDown');

                     resizePanels();
                 });
             });

            return;
        }


        if (pageItemsElem.is(':visible')) {
            pageItemsElem.hide(
                'blind',
                null,
                'fast',
                function () {
                    pageLogicElem.hide('blind', null, 'fast', function () {
                        pageToggleElem.parent().removeClass('active');
                        pageToggleElem.removeClass('pageArrowUp');
                        pageToggleElem.addClass('pageArrowDown');

                        resizePanels();
                    });
                });

        }
        else {
            pageItemsElem.show(
                'blind',
                null,
                'fast',
                function () {
                    pageLogicElem.show('blind', null, 'fast', function () {

                        pageToggleElem.parent().addClass('active'); ;
                        pageToggleElem.removeClass('pageArrowDown');
                        pageToggleElem.addClass('pageArrowUp');

                        resizePanels();
                    });
                });
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Load item navigation data.                                          //
    /////////////////////////////////////////////////////////////////////////
    this.loadItemDataForList = function(itemId, args) {
        templateEditor.runCallback(
            'getItemMetaData',
            args.baseArgs.authTicket,
            itemId,
            templateEditor.onItemNavDataLoaded,
            args);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load item navigation data.                                          //
    /////////////////////////////////////////////////////////////////////////
    this.loadPageItemsData = function (pageId, args) {
        templateEditor.runCallback(
            'listPageItemsMetaData',
            args.baseArgs.authTicket,
            pageId,
            templateEditor.onItemListNavDataLoaded,
            args);
    };

    ///////////////////////////////////////////////////////////////////////////
    // Apply template to item.                                               //
    ///////////////////////////////////////////////////////////////////////////
    this.onItemNavDataLoaded = function (itemData, args) {
        //Apply item template.
        templateHelper.loadAndApplyTemplate(
            templateEditor.jqTemplateNames['itemTemplate'],
            templateEditor.jqTemplatePaths['itemTemplate'],
            itemData,
            { languageCode: templateEditor.currentLanguage, index: args.index },
            'itemPlace_' + itemData.ItemId,
            true,
            templateEditor.onItemNavTemplateApplied,
            { itemData: itemData, pageId: args.pageId, baseArgs: args.baseArgs, isItemReloaded: args.isItemReloaded });
    };

    ///////////////////////////////////////////////////////////////////////////
    // Apply template to item.                                               //
    ///////////////////////////////////////////////////////////////////////////
    this.onItemListNavDataLoaded = function(itemDataList, args) {
        $.each(itemDataList, function(index, itemData) {
            //Apply item template.
            templateHelper.loadAndApplyTemplate(
                templateEditor.jqTemplateNames['itemTemplate'],
                templateEditor.jqTemplatePaths['itemTemplate'],
                itemData,
                { languageCode: templateEditor.currentLanguage, index: index },
                'itemPlace_' + itemData.ItemId,
                true,
                templateEditor.onItemNavTemplateApplied,
                { itemData: itemData, pageId: args.pageId, baseArgs: args.baseArgs, isItemReloaded: args.isItemReloaded });
        });
    };

    /////////////////////////////////////////////////////////////////////////
    // Bind events after item template applied and attach ML data          //
    /////////////////////////////////////////////////////////////////////////
    this.onItemNavTemplateApplied = function(args) {
        var elemId = 'itemPlace_' + args.itemData.ItemId;

        //Associate text data with item text element
        $('#' + elemId + ' .itemText').data('textData', args.itemData.TextData);

        if ($('#' + elemId).data('additionalData') == null
            && args.pageId != null
                && args.pageId > 0) {
            $('#' + elemId).data('additionalData', { pageId: args.pageId });
        }

        var pagePositionAdjustment = 0;

        if (templateEditor.mode != null
            && templateEditor.mode.toLowerCase() == 'surveyeditor') {
            pagePositionAdjustment = -1;
        }

        //If item is reloaded, then update some fields
        if (args.isItemReloaded) {
            $('#' + templateEditor.itemTitleElemId).html((args.itemData.PagePosition + pagePositionAdjustment) + '.' + args.itemData.ItemPosition + ' - ' + templateEditor.getTextValue(args.itemData.TextData, 'navText', templateEditor.currentLanguage, args.itemData.TypeName));
            $('#' + templateEditor.itemTypeElemId).html(args.itemData.TypeName);
        }

        //Show item
        $('#' + elemId).show('blind', null, 'fast', function () {
            resizePanels();
        });

        $('#' + elemId).unbind('click');

        //Bind click event to load item editor
        $('#' + elemId).bind('click', function() {
            if (typeof (tinymce) != 'undefined')
                tinymce.remove('textarea');

            $('#' + templateEditor.itemTitleElemId).html((args.itemData.PagePosition + pagePositionAdjustment) + '.' + args.itemData.ItemPosition + ' - ' + templateEditor.getTextValue(args.itemData.TextData, 'navText', templateEditor.currentLanguage, args.itemData.TypeName));
            $('#' + templateEditor.itemTypeElemId).html(args.itemData.TypeName);
            $('#' + templateEditor.itemIdElemId).html(args.itemData.ItemId);

            $('#' + templateEditor.detailMenuContainerId).show();

            if (args.baseArgs.defaultView.toLowerCase() == 'editor') {
                templateEditor.loadItemEditor(args.itemData.ItemId, args.baseArgs.templateId, args.baseArgs.containerFrameId, args.baseArgs.progressTemplateId);
            }
            else {
                templateEditor.loadItemPreview(args.itemData.ItemId, args.baseArgs.templateId, args.baseArgs.containerFrameId, args.baseArgs.progressTemplateId);
            }

            $('#' + templateEditor.welcomeElemId).hide();
            $('#' + templateEditor.detailElemId).show();
            $('#' + templateEditor.settingsElemId).hide();
            $('#' + templateEditor.filtersElemId).hide();

            $('#exportItemLink').attr('href', 'Edit.aspx?doitemexport=' + templateEditor.selectedItemId);
            $('div[id^="itemPlace_"] >  .templateItemList.activeContent').removeClass('activeContent').find('.upDownArrow').hide('fast');
            $('#itemPlace_' + args.itemData.ItemId + ' > div.templateItemList').addClass('activeContent');

            return false;
        });

        /* Not sure this is necessary, since item in editor will be reloaded when a button is clicked
        due to server postback */
        //if (templateEditor.selectedItemId == args.itemData.ItemId)
        //$('#' + elemId).click();
    };

    //////////////////////////////////////////////////////////////////////////
    // Load dialog embedded in template editor right-side frame.            //
    //////////////////////////////////////////////////////////////////////////
    this.loadDetailFrameDialog = function(url) {
        UFrameManager.init({
                id: templateEditor.itemFrameId,
                loadFrom: url,
                progressTemplate: $('#' + templateEditor.progressElemId).html(),
                showProgress: true
            });
    };

    /////////////////////////////////////////////////////////////////////////
    // Load item preview in editor panel id field.                         //
    /////////////////////////////////////////////////////////////////////////
    this.loadItemPreview = function(itemId) {
        //Do nothing if no item id
        if (itemId == null) {
            return;
        }

        templateEditor.selectedItemId = itemId;

        //Determine URL
        var editUrl = templateEditor.appRoot + '/ItemHtml.aspx?i=' + itemId + '&p=1&m=' + templateEditor.mode + '&l=' + templateEditor.currentLanguage;
        editUrl = templateEditor.addParamsToUrl(editUrl, templateEditor.baseOpenWindowArgs);

        templateEditor.loadDetailFrameDialog(editUrl);
    };


    /////////////////////////////////////////////////////////////////////////
    // Load item editor in editor panel id field.                          //
    /////////////////////////////////////////////////////////////////////////
    this.loadItemEditor = function (itemId) {
        //Do nothing if no item id    
        if (itemId == null || itemId == 'null') {
            return;
        }

        //Check item data for page id
        var elemId = 'itemPlace_' + itemId;

        var pageId = -1;
        var additionalData = $('#' + elemId).data('additionalData');

        if (additionalData != null
            && additionalData.pageId != null) {
            pageId = additionalData.pageId;
        }

        templateEditor.selectedItemId = itemId;

        $('#' + templateEditor.settingsElemId).hide();
        $('#' + templateEditor.filtersElemId).hide();
        $('#' + templateEditor.welcomeElemId).show();

        var editUrl = templateEditor.editRoot + '/EditItem.aspx?i=' + itemId + '&p=' + pageId + '&l=' + templateEditor.currentLanguage + '&ro=' + templateEditor.isReadOnly + '&mode=' + templateEditor.mode + '&guid=' + guid();
        editUrl = templateEditor.addParamsToUrl(editUrl, templateEditor.baseOpenWindowArgs);

        templateEditor.loadDetailFrameDialog(editUrl);

        if (typeof (copyActionIsAvailable) != "undefined") {
            copyActionIsAvailable('', this.templateId, itemId);
        }
        if (typeof (updateItemIsActiveStatus) != "undefined") {
            updateItemIsActiveStatus('', this.templateId, itemId);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Show dialog for moving pages.                                       //
    /////////////////////////////////////////////////////////////////////////
    this.openMovePageDialog = function (pageId, newPageIndex, popupWindowId) {
        //Popup move dialog
        var additionalParams = new Array(
            { name: 'a', value: 'm' }
        );

        if (newPageIndex != null) {
            additionalParams.push({ name: 'np', value: newPageIndex });
        }

        templateEditor.openChildWindow('', pageId, 'MovePage.aspx', additionalParams, popupWindowId);
    };

        /////////////////////////////////////////////////////////////////////////
        // Show dialog for copy page.                                         //
        /////////////////////////////////////////////////////////////////////////
        this.openCopyPageDialog = function (pageId, newPageIndex, popupWindowId) {
        //Popup copy dialog
        var additionalParams = new Array(
            { name: 'a', value: 'm' }
        );

        if (newPageIndex != null) {
            additionalParams.push(
                { name: 'np', value: newPageIndex }
        );
        }

        templateEditor.openChildWindow('', pageId, 'CopyPage.aspx', additionalParams, popupWindowId);
    };

        /////////////////////////////////////////////////////////////////////////
        // Show dialog for moving items.                                       //
        /////////////////////////////////////////////////////////////////////////
        this.openMoveItemDialog = function(itemId, targetPageId, newItemPosition, popupWindowId) {
        if (itemId == null || itemId <= 0) {
            return;
        }

        //Popup move dialog
        var additionalParams = new Array(
        { name: 'a', value: 'c' }
        );

        if (newItemPosition != null) {
            additionalParams.push({ name: 'ip', value: newItemPosition });
        }

        templateEditor.openChildWindow(itemId, targetPageId, 'CopyItem.aspx', additionalParams, popupWindowId);
    };

    

        /////////////////////////////////////////////////////////////////////////
        // Handle item drag & drop.                                            //
        /////////////////////////////////////////////////////////////////////////
    this.onItemDrop = function (event, ui) {
        //itemPlace_1234 element added dynamically by template helper.js

        //Get Id of item dropped
        var elementId = $(ui.item).attr('id');

        if (elementId == null || elementId == '') {
            return;
        }

        var itemId = elementId.replace('itemPlace_', '');

        //Now figure out where item was dropped
        if ($(ui.item).parent('.pageItems').length == 0) {
            return;
        }

        var targetPageElementId = $(ui.item).parent('.pageItems').attr('id');

        if (targetPageElementId == null || targetPageElementId == '') {
            return;
        }

        var pageId = targetPageElementId.replace('pageItems_', '');

        //Figure out order of items
        var targetItemArray = $(ui.item).parent('.pageItems').sortable('toArray');
        var newItemIndex = jQuery.inArray('itemPlace_' + itemId, targetItemArray);

        //Increment index to account for fact items start at index 0
        newItemIndex++;

        //reset refresh flag
        templateEditor.lastRefreshedPage = -1;

        templateEditor.runCallback('moveItem', templateEditor.authToken, itemId, pageId, newItemIndex, templateEditor.onItemMoveSuccess, pageId);
    };

    //call after item move
    this.onItemMoveSuccess = function (param, pageId) {
        if (templateEditor.lastRefreshedPage != pageId) {
            templateEditor.lastRefreshedPage = pageId;
            templateEditor.onItemMoved({ result: 'ok' });
        }
    }

    //add new page
    this.addPage = function () {
        templateEditor.runCallback('addPage', templateEditor.authToken, this.onPageAdded, null);
    }

    

    /////////////////////////////////////////////////////////////////////////
    // Handle page drag & drop                                             //
    /////////////////////////////////////////////////////////////////////////
    this.onPageDrop = function(event, ui) {
        var pageIndex = $(ui.item).attr('pageIndex');
        var pageId = $(ui.item).attr('pageId');

        if (pageIndex == null || pageId == null
            || pageIndex == '' || pageId == '') {
            return;
        }

      
        //Get new page ordering
        var pageArray = $('#pageNavList').sortable('toArray').filter(function (s) { return s != ""; });
        var newIndex = jQuery.inArray('pageContainer_' + pageId, pageArray);

        if (newIndex < 0) {
            return;
        }

        /*
        Increment index to account for position of first content page.  For example, "Page 1" of the survey is actually
        the second page of the survey because hidden items is first page. Increment a second time to account
        for fact that item at index 0 of drop zone is survey Page 1 and not hidden items page. 
        */
        newIndex = newIndex + templateEditor.firstContentPageIndex;

        //
        //newIndex = newIndex + 1;

        templateEditor.runCallback('movePage', templateEditor.authToken, pageId, newIndex, templateEditor.onPageMoveSuccess, null);
    };

    //call after page move
    this.onPageMoveSuccess = function () {
        templateEditor.onPageMoved({ result: 'ok' });
    }

    /////////////////////////////////////////////////////////////////////////
    // Handle language change to new language.                             //
    /////////////////////////////////////////////////////////////////////////
    this.onLanguageChange = function(newLanguage) {
        templateEditor.currentLanguage = newLanguage;

        //Update text
        $('.itemText').each(function() {
            $(this).html(templateEditor.getTextValue($(this).data('textData'), 'navText', templateEditor.currentLanguage));
        });

        //trancate text of each item
        $('.templateItemList .itemText').truncate({
            width: 210,
            after: '…',
            side: 'center',
            multiline: false
        });

        //Reload item 
        if (templateEditor.selectedItemId > 0) {
            if (templateEditor.defaultItemView == 'editor') {
                templateEditor.loadItemEditor(templateEditor.selectedItemId);
            }
            else {
                templateEditor.loadItemPreview(templateEditor.selectedItemId);
            }
        }
    };


    /////////////////////////////////////////////////////////////////////////
    // Truncates the text by symbols count.                                //
    /////////////////////////////////////////////////////////////////////////
    this.trunc = function (text, maxLength) {
        if (typeof (maxLength) == 'undefined' || !maxLength || typeof (text) == 'undefined')
            return text;
        if (text.length < maxLength)
            return text;

        return text.substring(0, maxLength);
    }

    /////////////////////////////////////////////////////////////////////////
    // Get text value from metadata text data array.                       //
    /////////////////////////////////////////////////////////////////////////
    this.getTextValue = function (textDataArray, textName, language, defaultValue, maxLength) {
        if (textDataArray == null || language == null) {
            return '';
        }

        for (var i = 0; i < textDataArray.length; i++) {
            if (textDataArray[i].LanguageCode != null
                && textDataArray[i].LanguageCode.toLowerCase() == language.toLowerCase()
                    && textDataArray[i].TextValues != null) {

                for (var j = 0; j < textDataArray[i].TextValues.NameValueList.length; j++) {
                    if (textDataArray[i].TextValues.NameValueList[j].Name != null
                        && textDataArray[i].TextValues.NameValueList[j].Name.toLowerCase() == textName.toLowerCase()) {
                        return templateEditor.trunc(textDataArray[i].TextValues.NameValueList[j].Value, maxLength);
                    }
                }
            }
        }

        if (defaultValue != null) {
            return templateEditor.trunc(defaultValue, maxLength);
        }

        return '';
    };

    ///////////////////////////////////////////////////////////////
    // Reload item                                               //
    ///////////////////////////////////////////////////////////////
    this.reloadItem = function(itemId) {
        /*if (templateEditor.defaultItemView == 'editor') {
        templateEditor.loadItemEditor(templateEditor.selectedItemId);
        }
        else {
        templateEditor.loadItemPreview(templateEditor.selectedItemId);
        }*/

        //Reload navigation if item has already been loaded
        if ($('#itemPlace_' + itemId).length > 0) {
            var args =
            {
                pageId: -1,             //-1 value indicates value in 'data' associated with item place node should be used
                isItemReloaded: true,
                baseArgs:
                    {
                        authTicket: templateEditor.authToken,
                        navPanelId: templateEditor.navPanelElemId,
                        editorPanelId: templateEditor.detailElemId,
                        progressTemplateId: templateEditor.progressElemId,
                        defaultView: templateEditor.defaultItemView
                    }
            };

            templateEditor.loadItemDataForList(itemId, args);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Copy Page Click                                                     //
    /////////////////////////////////////////////////////////////////////////
    this.onCopyPageClick = function(pageElem) {
        alert('TODO: Open Copy Page Dialog');
        //.var pageId = pageElem.attr('pageId');
        //Do a copy. To reload navigation panel loadNavidation callback will be called.
        //svcSurveyManagement.copySurveyPage(templateEditor.authToken, <%=ResponseTemplateId %>, pageId, loadNavigation, null);
    };

    /////////////////////////////////////////////////////////////////////////
    // Delete Page Click                                                   //
    /////////////////////////////////////////////////////////////////////////
    this.onDeletePageClick = function (pageElem, fromSurveyList) {
        var pageId = pageElem.attr('PageId');

        if (pageId == null || pageId == '') {
            return;
        }

        templateEditor.pageToDelete = pageId;

        if (this.confirmation == "True") {

            var message = textHelper.getTextValue('/pageText/forms/surveys/edit.aspx/deletePageConfirm', '');
            var heigth = 200;

            if (fromSurveyList) {
                message += '<div class="warning message">' + textHelper.getTextValue('/pageText/forms/surveys/edit.aspx/deletingPageWillDeleteBranches', '') + '</div>';
                heigth = 250;
            }

            showConfirmDialogWithCallback(
                message,
                templateEditor.deletePageConfirmCallback,
                337,
                heigth,
                textHelper.getTextValue('/common/confirmDelete', 'Confirm')
            );
        }
        else {
            templateEditor.runCallback('deletePage', templateEditor.authToken, pageId, templateEditor.refreshDisplay, { itemId: null });
            templateEditor.pageToDelete = null;
        }
    };

    /////////////////////////////////////////////////////////////////////////
    //Callback for confirm dialog about deleting the page
    this.deletePageConfirmCallback = function(arg) {
        if (arg) {
            var pageId = templateEditor.pageToDelete;
            templateEditor.runCallback('deletePage', templateEditor.authToken, pageId, templateEditor.refreshDisplay, { itemId: null });
            templateEditor.pageToDelete = null;
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Show delete item confirmation.                                      //
    /////////////////////////////////////////////////////////////////////////
    this.onDeleteItemClick = function(itemId) {
        if (itemId == null || itemId == -1) {
            return; // if there is no selected item, then exit.
        }

        if (this.confirmation == "True") {

        templateEditor.itemToDelete = itemId;

        var message = textHelper.getTextValue('/pageText/forms/surveys/edit.aspx/deleteItemConfirm', '');
        message += '<div class="warning message">' + textHelper.getTextValue('/pageText/forms/surveys/edit.aspx/deletingItemWillDeleteConditions', '') + '</div>';

        showConfirmDialogWithCallback(
            message,
            templateEditor.deleteItemConfirmCallback,
            337,
            230,
            textHelper.getTextValue('/common/confirmDelete', 'Confirm'));
        }
         else {
             templateEditor.runCallback('deleteItem', templateEditor.authToken, itemId, templateEditor.refreshDisplay, { itemId: null });
             templateEditor.itemToDelete = null;
        }
    };

    

    /////////////////////////////////////////////////////////////////////////
    // Handle delete item confirmation                                     //
    /////////////////////////////////////////////////////////////////////////
    this.deleteItemConfirmCallback = function(arg) {
        if (arg) {
            var itemId = templateEditor.itemToDelete;
            //templateEditor.runCallback('deleteItem', templateEditor.authToken, itemId, templateEditor.refreshDisplay, { itemId: null });
            templateEditor.runCallback('deleteItem', templateEditor.authToken, itemId, templateEditor.refreshDisplayAfterDeleteItem);
            templateEditor.itemToDelete = null;
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Refresh display after delete item                                   //
    /////////////////////////////////////////////////////////////////////////
    this.refreshDisplayAfterDeleteItem = function () {

        //Reload and open pages, which were opened before the moving operation
        $('#' + templateEditor.welcomeElemId).show();
        $('#' + templateEditor.detailElemId).hide();
        $('#' + templateEditor.settingsElemId).hide();
        $('#' + templateEditor.filtersElemId).hide();

        templateEditor.selectedItemId = -1;
        $('#' + templateEditor.itemFrameId).attr('src', '');
        $('#' + templateEditor.itemTitleElemId).empty(); //.html('');
        $('#' + templateEditor.itemTypeElemId).empty(); //.html('');
        $('#' + templateEditor.itemIdElemId).empty(); //.html('');


        //Reload and open pages, which were opened before the moving operation
        $('.pageArrowUp').each(function (index) {
            templateEditor.refreshPage($(this).attr('pageId'), true);
        });

        //Reload all other pages
        $('.pageArrowDown').each(function (index) {
            templateEditor.refreshPage($(this).attr('pageId'), false);
        });

        refreshSurveyPreview();
        resizePanels();
    };


    /////////////////////////////////////////////////////////////////////////
    // Refresh display. If itemId isn't equal null, it will be loaded to   //
    //  the panel.                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.refreshDisplay = function (itemId) {
        templateEditor.loadNavigation(templateEditor.navPanelElemId, templateEditor.templateId);

        if (itemId != null) {
            reloadItem(itemId);
        }
        else {
            $('#' + templateEditor.welcomeElemId).show();
            $('#' + templateEditor.detailElemId).hide();
            $('#' + templateEditor.settingsElemId).hide();
            $('#' + templateEditor.filtersElemId).hide();

            templateEditor.selectedItemId = -1;
            $('#' + templateEditor.itemFrameId).attr('src', '');
            $('#' + templateEditor.itemTitleElemId).empty(); //.html('');
            $('#' + templateEditor.itemTypeElemId).empty(); //.html('');
            $('#' + templateEditor.itemIdElemId).empty(); //.html('');
        }
        
        refreshSurveyPreview();
        resizePanels();
    };

    function refreshSurveyPreview() {
        //refresh survey preview iframe
        if ($('#survey_preview')) {
            $('#survey_preview').attr('src', $('#survey_preview').attr('src'));
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // Refresh display. If itemId isn't equal null, it will be loaded to   //
    //  the panel.                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.refreshPage = function(pageId, openPage) {
        templateEditor.populateNavPageInfo(
            'pageContainer_' + pageId,
            pageId,
            {
                authTicket: _at,
                callback: function() {
                    if (openPage)
                        $('#pageItemsToggle_' + pageId).click();

                    $('.pageItems').sortable({
                            handle: '.itemContainer',
                            connectWith: '.pageItems',
                            axis: 'y',
                            update: templateEditor.onItemDrop,
                            cursor: 'url(../../App_Themes/CheckboxTheme/Images/dragHand.png) !important'
                        });
                },
                defaultView: templateEditor.defaultItemView
            }
        );
    };

    /////////////////////////////////////////////////////////////////////////
    // Handle closing of child dialogs.                                    //
    /////////////////////////////////////////////////////////////////////////
    this.onDialogClosed = function (arg) {
        if (arg == null || arg == 'cancel') {
            return;
        }

        //Handle move page
        if (arg.op != null && arg.op == 'movePage') {
            templateEditor.onPageMoved(arg);
        }

        //Handle copy page
        if (arg.op != null && arg.op == 'copyPage') {
            templateEditor.onPageCopy(arg);
        }

        //Handle move item
        if (arg.op != null && arg.op == 'moveItem') {
            templateEditor.onItemMoved(arg);
        }

        //Handle edit item
        if (arg.op != null && arg.op == 'editItem') {
            templateEditor.onItemEdited(arg);
        }

        //Handle add item
        if (arg.op != null && arg.op == 'addItem') {
            templateEditor.onItemAdded(arg);
        }

        //Handle activate item
        if (arg.op != null && arg.op == 'activateItem') {
            templateEditor.onItemActivated(arg);
        }

        //Add page
        if (arg.op != null && arg.op == 'newPage') {
            templateEditor.onPageAdded(arg);
        }

        if (arg.op != null) {
            //Run registered dialog callback
            templateEditor.runCallback('dialogCallback_' + arg.op, arg);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onPageMoved                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.onPageMoved = function(arg) {
        if (arg.result != null && arg.result == 'ok') {
            templateEditor.refreshDisplay(null);
        }
        else {
            $('#pageNavList').sortable('cancel');
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onPageCopy                                                          //
    /////////////////////////////////////////////////////////////////////////
    this.onPageCopy = function(arg) {
        if (arg.result != null && arg.result == 'ok') {
            templateEditor.refreshDisplay(null);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onItemMoved                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.onItemMoved = function (arg) {
        if (arg.result != null && arg.result == 'ok') {

            if (templateEditor.selectedItemId > 0)
                templateEditor.reloadItem(templateEditor.selectedItemId);

            //Reload and open pages, which were opened before the moving operation
            $('.pageArrowUp').each(function (index) {
                templateEditor.refreshPage($(this).attr('pageId'), true);
            });

            //Reload all other pages
            $('.pageArrowDown').each(function (index) {
                templateEditor.refreshPage($(this).attr('pageId'), false);
            });

            //Reload survey preview
            refreshSurveyPreview();
        }
        else {
            //Move item back to original positions
            $(".pageItems").sortable('cancel');
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onItemEdited                                                        //
    /////////////////////////////////////////////////////////////////////////
    this.onItemEdited = function (arg) {
        if (arg.result != null && arg.result == 'ok' && arg.itemId != null) {
            templateEditor.reloadItem(arg.itemId);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onItemActivated                                                     //
    /////////////////////////////////////////////////////////////////////////
    this.onItemActivated = function(arg) {
        if (arg.result != null && arg.result == 'ok' && arg.itemId != null) {
            templateEditor.loadItemEditor(arg.itemId);
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onItemAdded                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.onItemAdded = function (arg) {
        if (arg.result != null && arg.result == 'ok') {
            if ($('#detailContent').is(':visible')) {
                if (arg.itemId != null) {
                    templateEditor.selectedItemId = arg.itemId;
                    templateEditor.loadItemEditor(arg.itemId);

                }
            }
            if (arg.pageId != null)
                templateEditor.refreshPage(arg.pageId, true);

            if ($('#detailWelcome').is(':visible')) {
                refreshSurveyPreview();
                resizePanels();
                $('#detailContent').hide();
                $('#detailWelcome').show();
            }
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onPageAdded                                                         //
    /////////////////////////////////////////////////////////////////////////
    this.onPageAdded = function () {
        if (templateEditor.mode.toLowerCase() == 'surveyeditor') {
            statusControl.showStatusMessage("Page " + (templateEditor.currentPageCount - 1) + " added", StatusMessageType.success);
        } else {
            statusControl.showStatusMessage("Page " + (templateEditor.currentPageCount + 1) + " added", StatusMessageType.success);
        }
        templateEditor.refreshDisplay();
    };
}
