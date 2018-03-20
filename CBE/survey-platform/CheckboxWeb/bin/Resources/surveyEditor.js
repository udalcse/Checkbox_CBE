/****************************************************************************
 * SurveyEditor.js                                                          *
 *   Client UI controller for editing surveys.  Relies on other components  *
 *   such as templateEditor.js, templateHelper.js, svcSurveyManagement.js   *
 ****************************************************************************/

var surveyEditor = new surveyEditorObj();

/////////////////////////////////////////////////////////////////////////////
// Survey Editor Object Definition
function surveyEditorObj() {
    this.surveyId = '';
    this.isReadOnly = false;
    this.countAlreadyLoadedTemplates = 0;
    this.countTemplates = 6;

    ///////////////////////////////////////////////////////////////////////////
    // Initialize with application root url of child action window.          //                                                    
    ///////////////////////////////////////////////////////////////////////////
    this.initialize = function (authToken, appRoot, surveyId, languageCode, mode, isReadOnly, baseOpenWindowArgs, itemFrameId, itemTitleElemId, itemTypeElemId, itemIdElemId, detailElemId, welcomeElemId, progressElemId, detailMenuContainerId, confirmation, settingsElemId, mobileThemesElemId) {

        //Set 0-based index of first content page
        templateEditor.firstContentPageIndex = 1;

        //
        templateEditor.currentLanguage = languageCode;
        templateEditor.isReadOnly = isReadOnly;
        templateEditor.confirmation = confirmation;

        //Initialize template editor
        templateEditor.initialize(authToken, appRoot, appRoot + '/Forms/Surveys', mode, baseOpenWindowArgs, itemFrameId, itemTitleElemId, itemTypeElemId, itemIdElemId, detailElemId, welcomeElemId, progressElemId, detailMenuContainerId, settingsElemId, mobileThemesElemId);

        surveyEditor.surveyId = surveyId;
        surveyEditor.isReadOnly = isReadOnly;

        //Register callbacks
        templateEditor.itemEditedHandler = templateEditor.reloadItem;

        //Service callbacks
        templateEditor.registerCallback('getTemplateMetaData', surveyEditor.loadSurveyMetaData);
        templateEditor.registerCallback('getTemplatePageMetaData', surveyEditor.loadPageMetaData);
        templateEditor.registerCallback('getItemMetaData', surveyEditor.loadItemMetaData);
        templateEditor.registerCallback('listPageItemsMetaData', surveyEditor.listPageItemsMetaData);
        templateEditor.registerCallback('deletePage', surveyEditor.deletePage);
        templateEditor.registerCallback('deleteItem', surveyEditor.deleteItem);
        templateEditor.registerCallback('openPage', surveyEditor.openPage);
        templateEditor.registerCallback('dialogCallback_editLanguages', surveyEditor.onLanguagesUpdated);
        templateEditor.registerCallback('moveItem', surveyEditor.moveItem);
        templateEditor.registerCallback('movePage', surveyEditor.movePage);
        templateEditor.registerCallback('addPage', surveyEditor.addPage);

        //Load and compile templates
        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageListTemplate.html',
            'surveyPageListTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('pageListTemplate', 'surveyPageListTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageListTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageTemplate.html',
            'surveyPageTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('pageTemplate', 'surveyPageTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyItemTemplate.html',
            'surveyItemTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('itemTemplate', 'surveyItemTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyItemTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageTitleTemplate.html',
            'surveyPageTitleTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('pageTitleTemplate', 'surveyPageTitleTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageTitleTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageMenuTemplate.html',
            'surveyPageMenuTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('pageMenuTemplate', 'surveyPageMenuTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageMenuTemplate.html');

        templateHelper.loadAndCompileTemplate(
            templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageLogicTemplate.html',
            'surveyPageLogicTemplate.html', surveyEditor.templateLoadedHandler);

        templateEditor.registerTemplate('pageLogicTemplate', 'surveyPageLogicTemplate.html', templateEditor.appRoot + '/Forms/Surveys/jqtmpl/surveyPageLogicTemplate.html');

        //        //Bind click events for items
        //        $('#editItemLink').click(function () {
        //            templateEditor.loadItemEditor(templateEditor.selectedItemId);
        //        });
        //        //Bind click events
        //        $('#previewItemLink').click(function () {
        //            templateEditor.loadItemPreview(templateEditor.selectedItemId);
        //        });

        //
        $('#deleteItemLink').click(function () {
            templateEditor.onDeleteItemClick(templateEditor.selectedItemId);
        });

        //
        $(document).on('click', '.deleteSurveyPageLink', function () {
            templateEditor.onDeletePageClick($(this), true);
        });

        //
        $('#moveCopyItemLink').click(function () {
            templateEditor.openMoveItemDialog(
                templateEditor.selectedItemId,
                null,
                null,
                'largeDialog'
                );
        });

        $(document).on('click', '.groupMenuToggle', (function (event) { pageOptionsClick(event); }));
    };

    function pageOptionsClick(event) {
        event.stopPropagation();

        //declaration area
        var $target = $(event.target);
        var parent = ($target.parent().find(".page-options-menu"));
        var optionsMenu = $('.page-options-menu');

        //Checking if theres already opened menu
        var visibleCount = 0;
        $.each(optionsMenu, function() {
            if ($(this).css('display') === 'block') {
                    visibleCount++
                }
        });

        //if another item is visible - hide all and display parent 
        if (parent.css("display") === "none" && visibleCount > 0) {
            optionsMenu.hide();
            parent.show();
        } else if (parent.css("display") === "none" && visibleCount < 1) {
            parent.show();
        } else  {
            optionsMenu.hide();
        }
    }

    //Hide  page Options menu if click on another DOM element detected
    $('html').click(function (event) {
       
        var $target = $(event.target);
       
        if(!$target.hasClass('groupMenuToggle')) {
            $('.page-options-menu').hide();
        }
    });

  

    /////////////////////////////////////////////////////////////////////////
    // Open activate/deactivate item dialog                                //
    /////////////////////////////////////////////////////////////////////////
    this.openActivateItemDialog = function (itemId, surveyId) {
        //Popup activate/deactivate dialog        

        templateEditor.openChildWindow(itemId, null, 'ItemActivation.aspx', null, '_dialog');
    }

    /////////////////////////////////////////////////////////////////////////
    // Check if all templates are loaded or not                            //
    /////////////////////////////////////////////////////////////////////////
    this.checkIfTemplatesLoaded = function () {
        return surveyEditor.countAlreadyLoadedTemplates == surveyEditor.countTemplates;
    }

    /////////////////////////////////////////////////////////////////////////
    // Handler of template loaded event                                    //
    /////////////////////////////////////////////////////////////////////////
    this.templateLoadedHandler = function () {
        surveyEditor.countAlreadyLoadedTemplates++;
    }

    /////////////////////////////////////////////////////////////////////////
    // Return selected item id                                             //
    /////////////////////////////////////////////////////////////////////////
    this.getSelectedItemId = function() {
        return templateEditor.selectedItemId;
    };

    /////////////////////////////////////////////////////////////////////////
    // Load survey metadata                                                //
    /////////////////////////////////////////////////////////////////////////
    this.loadSurveyMetaData = function(authTicket, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.getSurveyMetaData(authTicket, surveyEditor.surveyId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Move item position                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.moveItem = function (authTicket, itemId, newPageId, position, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.moveSurveyItem(authTicket, surveyEditor.surveyId, itemId, newPageId, position, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Move page position                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.movePage = function (authTicket, pageId, position, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.moveSurveyPage(authTicket, surveyEditor.surveyId, pageId, position, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load survey page metadata                                           //
    /////////////////////////////////////////////////////////////////////////
    this.loadPageMetaData = function(authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.getSurveyPageMetaData(authTicket, surveyEditor.surveyId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Load survey item metadata                                           //
    /////////////////////////////////////////////////////////////////////////
    this.loadItemMetaData = function(authTicket, itemId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.getSurveyItemMetaData(authTicket, surveyEditor.surveyId, itemId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // List page items metadata                                            //
    /////////////////////////////////////////////////////////////////////////
    this.listPageItemsMetaData = function (authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.listPageItemsData(authTicket, surveyEditor.surveyId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Add survey page.                                                    //
    /////////////////////////////////////////////////////////////////////////
    this.addPage = function (authTicket, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.addSurveyPage(authTicket, surveyEditor.surveyId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Delete survey page.                                                 //
    /////////////////////////////////////////////////////////////////////////
    this.deletePage = function(authTicket, pageId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.deleteSurveyPage(authTicket, surveyEditor.surveyId, pageId, callback, callbackArgs);
    };

    /////////////////////////////////////////////////////////////////////////
    // Delete survey item.                                                 //
    /////////////////////////////////////////////////////////////////////////
    this.deleteItem = function(authTicket, itemId, callback, callbackArgs) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.deleteSurveyItem(authTicket, surveyEditor.surveyId, itemId, callback, callbackArgs);
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
        templateEditor.loadNavigation(navContainer, surveyEditor.surveyId);
    };
    
    ////////////////////////////////////////////////////////////////////////
    // Show the MovePage dialog.                                          //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showMovePageDialog = function(pageId, popupWindowId) {
        templateEditor.openMovePageDialog(pageId, '', popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the AddItem dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showAddItemDialog = function(pageId, popupWindowId, callback) {
        templateEditor.openChildWindow('', pageId, 'AddItem.aspx', null, popupWindowId, function () {
            //var args = { op: 'addItem', result: 'ok', pageId: pageId };
            //closeWindow('templateEditor.onItemAdded', args);
        }, callback);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the AddPage dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showAddPageDialog = function(popupWindowId) {
        templateEditor.openChildWindow('', '', 'AddPage.aspx', null, popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the CopyPage dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showCopyPageDialog = function(pageId, popupWindowId) {
        templateEditor.openChildWindow('', pageId, 'CopyPage.aspx', null, popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the languages dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showLanguagesDialog = function(popupWindowId) {
        templateEditor.openChildWindow('', '', 'Languages.aspx', null, popupWindowId);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the Edit Branches dialog.                                     //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showEditPageBranchesDialog = function(pageId, position) {

        if (surveyEditor.isReadOnly)
            return;

        $('#' + templateEditor.welcomeElemId).hide();
        $('#' + templateEditor.mobileThemesElemId).hide();
        $('#' + templateEditor.detailElemId).show();
        $('#' + templateEditor.settingsElemId).hide();
        
        var pageText = '';

        if (position > 0) {
            pageText = (position - 1).toString();
        }

        if (position == 0) {
            pageText = textHelper.getTextValue('/pageText/pageBranching.aspx/hiddenItems', 'Hidden')
        }

        //Set title
        $('#' + templateEditor.itemTitleElemId).html(textHelper.getTextValue('/pageText/pageBranching.aspx/branchingForPage', 'Branching: ') + ' ' + pageText);

        //Hide item menu
        $('#' + templateEditor.detailMenuContainerId).hide();

        //Load frame
        var editUrl = 'PageBranches.aspx?s=' + surveyEditor.surveyId + '&p=' + pageId + '&l=' + templateEditor.currentLanguage;

        templateEditor.loadDetailFrameDialog(editUrl);
    };

    ////////////////////////////////////////////////////////////////////////
    // Show the Edit Conditions dialog.                                           //
    //  Default page to add item to.                                      //
    ////////////////////////////////////////////////////////////////////////
    this.showEditPageConditionsDialog = function(pageId, position, totalConditionsCount) {

        if (surveyEditor.isReadOnly)
            return;

        $('#' + templateEditor.welcomeElemId).hide();
        $('#' + templateEditor.mobileThemesElemId).hide();
        $('#' + templateEditor.detailElemId).show();
        $('#' + templateEditor.settingsElemId).hide();

        //Set title
        $('#' + templateEditor.itemTitleElemId).html(textHelper.getTextValue('/pageText/pageConditions.aspx/conditionsForPage', 'Conditions: ') + ' ' + totalConditionsCount);

        //Hide item menu
        $('#' + templateEditor.detailMenuContainerId).hide();

        //Load frame
        var editUrl = 'PageConditions.aspx?s=' + surveyEditor.surveyId + '&p=' + pageId + '&l=' + templateEditor.currentLanguage;

        templateEditor.loadDetailFrameDialog(editUrl);
    };

    /////////////////////////////////////////////////////////////////////////
    // Add conditions/branching to page.                                   //
    /////////////////////////////////////////////////////////////////////////
    this.openPage = function(pageData) {
        if (pageData.PageType.toLowerCase() != 'completion' || pageData.PageType.toLowerCase() == 'contentpage') {
            $('#pageLogic_' + pageData.Id).append('<div class="groupHeader conditionWrapper" id="pageLogicPlace_' + pageData.Id + '"></div>');
            serviceHelper.initialize(templateEditor.appRoot + '/Services/');
            svcSurveyManagement.getPageLogic(templateEditor.authToken, surveyEditor.surveyId, pageData.Id, surveyEditor.onPageNavLogicLoaded, pageData);
        }

        //Add "No Items" message when page has no items.
        if (pageData.ItemIds.length == 0) {
            $('#pageItems_' + pageData.Id).append(
                '<div class="groupContentNoHover itemContainer"><div class="left fixed_25">&nbsp;</div><div class="left groupContentAttributes">'
                    + textHelper.getTextValue('/pageText/formEditor.aspx/noItems', 'No Items')
                        + '</div></div><br class="clear" />'
            );
        }
    };

    /////////////////////////////////////////////////////////////////////////
    // onBranchesEdited                                                    //
    /////////////////////////////////////////////////////////////////////////
    this.onBranchesEdited = function(pageId) {
        $('#pageItemsToggle_' + pageId).each(function () {
            svcSurveyManagement.getPageLogic(templateEditor.authToken, surveyEditor.surveyId, pageId, surveyEditor.onPageNavLogicLoaded, $.tmplItem(this).data);
        });
    };

    /////////////////////////////////////////////////////////////////////////
    // onConditionsEdited                                                  //
    /////////////////////////////////////////////////////////////////////////
    this.onConditionsEdited = function(pageId) {
        $('#pageItemsToggle_' + pageId).each(function () {
            svcSurveyManagement.getPageLogic(templateEditor.authToken, surveyEditor.surveyId, pageId, surveyEditor.onPageNavLogicLoaded, $.tmplItem(this).data);
        });
    };

    /////////////////////////////////////////////////////////////////////////
    // Apply logic template to data.                                      //
    /////////////////////////////////////////////////////////////////////////
    this.onPageNavLogicLoaded = function (pageLogic, pageData) {

        templateHelper.loadAndApplyTemplate(
            templateEditor.jqTemplateNames['pageLogicTemplate'],
            templateEditor.jqTemplatePaths['pageLogicTemplate'],
            pageLogic,
            { pageId: pageData.Id, position: pageData.Position, pageType: pageData.PageType.toLowerCase() },
            'pageLogicPlace_' + pageData.Id,
            true
        );
    };

    /////////////////////////////////////////////////////////////////////////
    // Languages changed                                                   //
    /////////////////////////////////////////////////////////////////////////
    this.onLanguagesUpdated = function (arg) {
        if (arg != null && arg.refresh == 'yes') {
            templateEditor.refreshDisplay();
        }
    };

    this.addPageBreak = function (pageId, shouldPageBreak, templateId) {
        serviceHelper.initialize(templateEditor.appRoot + '/Services/');
        svcSurveyManagement.addPageBreak(pageId, shouldPageBreak, templateId);
    };
}
