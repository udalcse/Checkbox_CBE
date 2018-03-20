/****************************************************************************
 * ReportEditor.js                                                          *
 *   Client UI controller for editing reports.  Relies on other components  *
 *   such as templateEditor.js, templateHelper.js, svcReportManagement.js   *
 ****************************************************************************/

var reportEditor = new reportEditorObj();

/////////////////////////////////////////////////////////////////////////////
// Report Editor Object Definition
function reportEditorObj() {
    this.reportId = '';

    ///////////////////////////////////////////////////////////////////////////
    // Initialize with application root url of child action window.          //                                                    
    ///////////////////////////////////////////////////////////////////////////
    this.initialize = function (authToken, appRoot, reportId, languageCode, mode, baseOpenWindowArgs, itemFrameId, itemTitleElemId, itemTypeElemId, itemIdElemId, detailElemId, welcomeElemId, progressElemId, detailMenuContainerId, settingsElemId, filtersElemId) {
        //Language
        templateEditor.currentLanguage = languageCode;

        //Initialize template editor
        templateEditor.initialize(authToken, appRoot, appRoot + '/Forms/Surveys/Reports', mode, baseOpenWindowArgs, itemFrameId, itemTitleElemId, itemTypeElemId, itemIdElemId, detailElemId, welcomeElemId, progressElemId, detailMenuContainerId, settingsElemId, filtersElemId);

        reportEditor.reportId = reportId;

        //Register callbacks
        templateEditor.itemEditedHandler = templateEditor.reloadItem;

        //Service callbacks
        templateEditor.registerCallback('getTemplateMetaData', reportEditor.loadReportMetaData);
        templateEditor.registerCallback('getTemplatePageMetaData', reportEditor.loadPageMetaData);
        templateEditor.registerCallback('getItemMetaData', reportEditor.loadItemMetaData);
        templateEditor.registerCallback('listPageItemsMetaData', reportEditor.listPageItemsMetaData);
        templateEditor.registerCallback('deletePage', reportEditor.deletePage);
        templateEditor.registerCallback('deleteItem', reportEditor.deleteItem);
        templateEditor.registerCallback('openPage', reportEditor.openPage);
        templateEditor.registerCallback('moveItem', reportEditor.moveItem);
        templateEditor.registerCallback('movePage', reportEditor.movePage);
        templateEditor.registerCallback('addPage', reportEditor.addPage);

        //Load and compile templates
        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageListTemplate.html',
            'reportPageListTemplate.html');

        templateEditor.registerTemplate('pageListTemplate', 'reportPageListTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageListTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageTemplate.html',
            'reportPageTemplate.html');

        templateEditor.registerTemplate('pageTemplate', 'reportPageTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportItemTemplate.html',
            'reportItemTemplate.html');

        templateEditor.registerTemplate('itemTemplate', 'reportItemTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportItemTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageTitleTemplate.html',
            'reportPageTitleTemplate.html');

        templateEditor.registerTemplate('pageTitleTemplate', 'reportPageTitleTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageTitleTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageMenuTemplate.html',
            'reportPageMenuTemplate.html');

        templateEditor.registerTemplate('pageMenuTemplate', 'reportPageMenuTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageMenuTemplate.html');

        //TODO: Filter
        //        templateHelper.loadAndCompileTemplate(
        //            templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageConditionsTemplate.html',
        //            'surveyPageConditionsTemplate.html');

        //        templateEditor.registerTemplate('pageConditionsTemplate', 'surveyPageConditionsTemplate.html', templateEditor.appRoot + '/Forms/Surveys/Reports/jqtmpl/reportPageConditionsTemplate.html');

        //        templateHelper.loadAndCompileTemplate(
        //            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageBranchingTemplate.html',
        //            'surveyPageBranchingTemplate.html');

        //        templateEditor.registerTemplate('pageBranchingTemplate', 'surveyPageBranchingTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageBranchingTemplate.html');

        //Bind click events for items
        $('#editItemLink').click(function() {
            templateEditor.loadItemEditor(templateEditor.selectedItemId);
        });

        //Bind click events
        $('#previewItemLink').click(function() {
            templateEditor.loadItemPreview(templateEditor.selectedItemId);
        });

        //
        $('#deleteItemLink').click(function () {
            showConfirmDialogWithCallback(
                textHelper.getTextValue('/pageText/formEditor.aspx/deleteItemConfirm', 'Are you sure you want to delete this item?'),
                function () { templateEditor.onDeleteItemClick(templateEditor.selectedItemId); },
                337,
                200,
                textHelper.getTextValue('/pageText/formEditor.aspx/deleteItemConfirmTitle', 'Delete Item')
            );
        });

        //
        $('#exportItemLink').click(function() {
        });

        //
        $('#moveCopyItemLink').click(function() {
            templateEditor.openMoveItemDialog(
                templateEditor.selectedItemId,
                null,
                null,
                'largeDialog'
                );
        });

        //bind click for page options menu
        $(document).on('click', '.groupMenuToggle', (function (event) { pageOptionsClick(event); }));
    };

    //show options menu
    function pageOptionsClick(event) {
        event.stopPropagation();
        var $target = $(event.target);
        $('.page-options-menu').hide();
        if (($target.hasClass('groupMenuToggle'))) {
            $('.page-options-menu').hide();
            $target.parent().find('.page-options-menu').show();
            event.stopPropagation();
        }
    }

    //Hide  page Options menu if click on another DOM element detected
    $('html').click(function (event) {
        $('.page-options-menu').hide();
    });

    /////////////////////////////////////////////////////////////////////////
    // Load report metadata                                                //
    /////////////////////////////////////////////////////////////////////////
    this.loadReportMetaData = function(authTicket, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.getReportMetaData(authTicket, reportEditor.reportId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load report page metadata                                           //
    /////////////////////////////////////////////////////////////////////////
    this.loadPageMetaData = function(authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.getReportPageMetaData(authTicket, reportEditor.reportId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load report item metadata                                           //
    /////////////////////////////////////////////////////////////////////////
    this.loadItemMetaData = function(authTicket, itemId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.getReportItemMetaData(authTicket, reportEditor.reportId, itemId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // List page items metadata                                            //
    /////////////////////////////////////////////////////////////////////////
    this.listPageItemsMetaData = function (authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.listReportPageItemsData(authTicket, reportEditor.reportId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Delete report page.                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.deletePage = function(authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.deleteReportPage(authTicket, reportEditor.reportId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Delete report item.                                                 //
    /////////////////////////////////////////////////////////////////////////
    this.deleteItem = function(authTicket, itemId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.deleteReportItem(authTicket, reportEditor.reportId, itemId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Handle language change to new language.                             //
    /////////////////////////////////////////////////////////////////////////
    this.onLanguageChange = function(newLanguage) {
        templateEditor.onLanguageChange(newLanguage);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load editor navigation.                                             //
    /////////////////////////////////////////////////////////////////////////
    this.loadNavigation = function(navContainer) {
        templateEditor.loadNavigation(navContainer, reportEditor.reportId);
    };

    /////////////////////////////////////////////////////////////////////////
    // Move item position                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.moveItem = function (authTicket, itemId, newPageId, position, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.moveReportItem(authTicket, reportEditor.reportId, itemId, newPageId, position, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Move page position                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.movePage = function (authTicket, pageId, position, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.moveReportPage(authTicket, reportEditor.reportId, pageId, position, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Add new page.                                                       //
    /////////////////////////////////////////////////////////////////////////
    this.addPage = function (authTicket, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcReportManagement.addReportPage(authTicket, reportEditor.reportId, callback, callbackArgs);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the MovePage dialog.                                          //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showMovePageDialog = function(pageId, popupWindowId) {
        templateEditor.openMovePageDialog(pageId, '', popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the CopyPage dialog.                                          //
    //  Default page to copy item to.                                     //
    ////////////////////////////////////////////////////////////////////////
    this.showCopyPageDialog = function(pageId, popupWindowId) {
        templateEditor.openChildWindow('', pageId, 'CopyPage.aspx', null, popupWindowId);
        //templateEditor.openMovePageDialog(pageId, '', popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the AddItem dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showAddItemDialog = function(pageId, popupWindowId) {
        templateEditor.openChildWindow('', pageId, 'AddItem.aspx', null, popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the AddPage dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showAddPageDialog = function(popupWindowId) {
        templateEditor.openChildWindow('', '', 'AddPage.aspx', null, popupWindowId);
    };

    /////////////////////////////////////////////////////////////////////////
    // Add No Items Message to Page                                        //
    /////////////////////////////////////////////////////////////////////////
    this.openPage = function(pageData) {
        //Add "No Items" message when page has no items.
        if (pageData.ItemIds.length == 0) {
            $('#pageItems_' + pageData.Id).append(
                 '<div class="groupContentNoHover itemContainer"><div class="left fixed_25">&nbsp;</div><div class="left groupContentAttributes">'
                + textHelper.getTextValue('/pageText/formEditor.aspx/noItems', 'No Items')
                + '</div></div><br class="clear" />'
            );
        }
    };

//    /////////////////////////////////////////////////////////////////////////
//    // Apply condition template to data.                                   //
//    /////////////////////////////////////////////////////////////////////////
//    this.onPageNavConditionsLoaded = function (ruleData, pageData) {

//        templateHelper.loadAndApplyTemplate(
//            templateEditor.jqTemplateNames['pageConditionsTemplate'],
//  TODO:     <<TemplatePath MUST BE THERE>>
//            ruleData,
//            { pageId: pageData.Id, position: pageData.Position },
//            'pageConditionPlace_' + pageData.Id,
//            true
//        );
//    }


//    /////////////////////////////////////////////////////////////////////////
//    // Apply branch template to data.                                      //
//    /////////////////////////////////////////////////////////////////////////
//    this.onPageNavBranchesLoaded = function (rulesArray, pageData) {

//        templateHelper.loadAndApplyTemplate(
//            templateEditor.jqTemplateNames['pageBranchingTemplate'],
//   TODO:    <<TemplatePath MUST BE THERE>>
//            { rules: rulesArray },
//            { pageId: pageData.Id, position: pageData.Position },
//            'pageBranchesPlace_' + pageData.Id,
//            true
//        );
//    }
}
