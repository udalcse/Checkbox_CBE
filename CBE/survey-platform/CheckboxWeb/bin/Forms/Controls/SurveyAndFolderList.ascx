<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyAndFolderList.ascx.cs"  Inherits="CheckboxWeb.Forms.Controls.SurveyAndFolderList" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>

<script type="text/javascript">
    //Elem, which is going to be deleted. It's needed for radConfirm dialog.
    var _deletedElem;
    var _initialSurveyId = -1;
    var _initialFolderId = -1;
    var _lastPage = 1;
    var _loadInProgress = false;
    var _lastSearchTerm = '<%=SearchTerm%>';
    var _filterField = 'Name';
    var _period = 0;
    var _eventName = '';

    <%-- Ensure service initialized --%>
    $(document).ready(function() {
        $('#listPlace').parents('.overflow-y').scroll(function () { 
            if($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight && !_loadInProgress) {
                loadNextPage(); 
            }           
        });

        $('.surveys-filter-buttons').appendTo('.leftPanel').show();
        $(document).on('change', '.surveys-filter-buttons input', function() {
            $("label[for='"+$(this).attr('id')+"']").toggleClass("checked");
        });

        $(this).on('searchExecuted', function(e, searchTerm, filterItemType, period, eventName) {
            if(filterItemType == 'survey') {
                if (typeof(eventName) != 'undefined' && typeof(period) != 'undefined') {
                    _lastSearchTerm = escapeInjections(searchTerm);
                    _period = period;
                    _eventName = eventName;
                    clearAndReload();
                } else if (_lastSearchTerm != searchTerm) {
                    _period = 0;
                    _eventName = '';
                    _lastSearchTerm = escapeInjections(searchTerm);
                    clearAndReload();
                } 
            }
        });

        $('#active-survey-filter, #inactive-survey-filter').on('change', clearAndReload);

        $(window).resize(function () {
            loadNextPageOnLeftPanelExtraSize();
        });

        clearAndReload();
    });

    //Show loader
    function showLoader() {
        $('#listPlace').append(
            '<div id="loader" style="text-align:center;">' +
                '<p><%=WebTextManager.GetText("/common/loading")%></p>' +
                '<p><asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" /></p>' +
            '</div>'
        );
    }

    //Stop loader
    function removeLoader() {
        $('#listPlace #loader').remove();
    }

    //Reload the list
    function clearAndReload() {
        $('#listPlace').empty();
        var $favorites = $("<div class='favorites-container' id='favoriteSurveys' />");
        $('#listPlace').prepend($favorites);
        _lastPage = 1;
        
        //Precompile survey list item templates
        templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Forms/jqtmpl/surveyFavoriteListTemplate.html") %>', 'surveyFavoriteListTemplate.html');
        templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Forms/jqtmpl/surveyListSurveyTemplate.html") %>', 'surveyListSurveyTemplate.html')
            .then(function() {
                templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/surveyListFolderTemplate.html") %>', 'surveyListFolderTemplate.html'); })
            .then(function() { 
                getFromServer();
                getFavoriteSurveysFromServer();
        });
    }

    //Load next page
    function loadNextPage() {
        _loadInProgress = true;
        _lastPage++;
        getFromServer();
    }

    //Reload data in survey and folder list
    function getFromServer(){
        showLoader();
        loadSurveyAndFolderList(_at, <%=FolderId %>, { pageAjaxRequest : true, resultContainer: 'listPlace' });
    }
    
    //Reload data in favotite survey list
    function getFavoriteSurveysFromServer(){
        //showLoader();
        loadFavoriteSurveyList(_at, 0, { pageAjaxRequest : false, resultContainer: 'favoriteSurveys' });
    }
    
    //Make ajax call to load favorite list and then call selected callback
    function loadFavoriteSurveyList(at, folderId, callbackArgs) {
        //Prune folder menu
        securityHelper.protect('<%=WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_FOLDER, folderId, '#folderMenu_' + folderId)
            .then(function(){ 
                //Show folder menu
                $('#folderMenu_' + folderId).show();
            });

        svcSurveyManagement.listFavoriteSurveys(at, folderId, -1, -1, false, _lastSearchTerm, callbackArgs)
            .then(onFavoriteSurveyListLoaded);
    }

    //Handle list load completed
    function onFavoriteSurveyListLoaded(resultData, args) {
        if (resultData == null || args.resultContainer == null) {
			return;
		}

        //we are at the end of the page
        if(resultData.length == 0 && _lastPage > 1) {
            removeLoader();
            return;
        }

        //Add empty message, if necessary
        if(resultData.length == 0){
            return;
        }

        //Apply indexes to the results
        var curIndex = 0;
        for(var i = 0; i < resultData.length; i++){
            if(resultData[i].Type.toLowerCase() != 'folder'){
                resultData[i].index = curIndex;
                curIndex++;
            }
        }

        $('#' + args.resultContainer).empty();

        templateHelper.loadAndApplyTemplateD(
            'surveyFavoriteListTemplate.html',
            '<%=ResolveUrl("~/Forms/jqtmpl/surveyFavoriteListTemplate.html") %>',
            resultData,
            null,
            args.resultContainer,
            false,
            args)
            .then(onListTemplateApplied)
            .then(function() {
                var $favorites = $("#favoriteSurveys");
                $favorites.append("<div class='divider' />");

                //protect permissions
                for (var i = 0; i < resultData.length; i++) {
                    securityHelper.protect('<%=WebUtilities.GetCurrentUserEncodedName() %>',
                        svcAuthorization.RESOURCETYPE_SURVEY, resultData[i].ID, '#favoriteSurvey_' + resultData[i].ID);
                }
            });
        //Perform subsequent work in callback, since ajax may be used to
        // load template
	}

    <%-- calculates the field name date for the given period --%>
    function getDateFieldName(eventName)
    {
        if (typeof(eventName) != 'undefined')
        {
            if (eventName.indexOf('CREATE') > 0)
                return 'CreatedDate';
            if (eventName.indexOf('EDIT') > 0)
                return 'ModifiedDate';
        }
        return '';
    }

    //Make ajax call to load list and then call selected callback
    function loadSurveyAndFolderList(at, folderId, callbackArgs) {
        //Prune folder menu
        securityHelper.protect('<%=WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_FOLDER, folderId, '#folderMenu_' + folderId)
            .then(function(){ 
                //Show folder menu
                $('#folderMenu_' + folderId).show();
            });

        var page = _lastPage;
        var pageSize = <%=ApplicationManager.AppSettings.PagingResultsPerPage %>;
        if (parseInt(folderId) > 1) {
            page = 1;
            pageSize = -1;
        }

        var includeActive = $('#active-survey-filter').prop('checked');
        var includeinactive = $('#inactive-survey-filter').prop('checked');

        svcSurveyManagement.listSurveysAndFoldersByActiveStatusD(at, folderId, page, pageSize, false, _filterField, _lastSearchTerm, _period, getDateFieldName(_eventName), includeActive, includeinactive, callbackArgs)
            .then(onSurveyAndFolderListLoaded);
    }

    //Handle list load completed
    function onSurveyAndFolderListLoaded(resultData, args) {
        if (resultData == null || args.resultContainer == null) {
			return;
		}

        //we are at the end of the page
        if(resultData.length == 0 && _lastPage > 1 && args.pageAjaxRequest) {
            removeLoader();
            return;
        }

        //Add empty message, if necessary
        if(resultData.length == 0){
            removeLoader();
            $('#' + args.resultContainer).append('<ul class="no-results groupContentNoHover allMenu"><li class="groupContentName" style="padding-left:20px;"><%=WebTextManager.GetText("/pageText/forms/manage.aspx/noSurveys") %></li><div class="clear"></div></ul><div class="clear"></div>');
            $('#' + args.resultContainer).show();
            return;
        }

        //Apply indexes to the results
        var curIndex = 0;
        for(var i = 0; i < resultData.length; i++){
            if (resultData[i].Type.toLowerCase() != 'folder') {
                resultData[i].index = curIndex;
                curIndex++;
            } else {
                resultData[i].showCreatedBy =  <%= (ApplicationManager.AppSettings.ShowCreatedBy.ToString().ToLower()) %>;
            }
        }

        templateHelper.loadAndApplyTemplateD(
            'surveyListTemplate.html',
            '<%=ResolveUrl("~/Forms/jqtmpl/surveyListTemplate.html") %>',
            resultData,
            null,
            args.resultContainer,
            args.folderId >= 1000,
            args)
                .then(onListTemplateApplied)
                .then(function(){
                    //protect permissions
                    for(var i = 0; i < resultData.length; i++){
                        if(resultData[i].Type.toLowerCase() != 'folder'){
                            securityHelper.protect('<%=WebUtilities.GetCurrentUserEncodedName() %>',
                                svcAuthorization.RESOURCETYPE_SURVEY, resultData[i].ID, '#survey_' + resultData[i].ID);
                        } else {
                            securityHelper.protect('<%=WebUtilities.GetCurrentUserEncodedName() %>',
                                svcAuthorization.RESOURCETYPE_FOLDER, resultData[i].ID, '#folderMenu_' + resultData[i].ID);
                        }
                    }
                    loadNextPageOnLeftPanelExtraSize();
                });
        //Perform subsequent work in callback, since ajax may be used to
        // load template
	}

    //
    function loadNextPageOnLeftPanelExtraSize() {
        if(!_loadInProgress && $('.leftPanel .viewport').height() >= $('#listPlace').height()) {
            loadNextPage();
        }
    }

    //Handle survey template applied
    function onListTemplateApplied(args){
        if (typeof(onListTemplateAppliedExternal) != 'undefined')
            onListTemplateAppliedExternal();
                    
        if(args == null)
            return;

        //Flag container as loaded
		$('#' + args.resultContainer).attr('loaded', 'true');

        //Ensure results shown
		if(!args.pageAjaxRequest) {
            $('#' + args.resultContainer).show('blind', null, 'fast', function() { resizePanels(); });
        }

        //Bind click events loading folder
        $('#' + args.resultContainer).find('.groupHeader').unbind('click').click(function () {toggleFolder($(this)); });
        $('#' + args.resultContainer).find('.deleteFolderLink').unbind('click').click(function(e) {
                e.stopPropagation();
                onDeleteFolderClick($(this));
        });

        //Bind folder "Options" menu click event
        $('#' + args.resultContainer).find('.groupMenuToggle').unbind('click').click(folderOptionsClick);

        //Bind survey click event, if any specified
        <% if(!string.IsNullOrEmpty(OnSurveyClickHandler)) { %>
            $('#' + args.resultContainer).find('.groupContent').unbind('click').click(function () { 
                <%=OnSurveyClickHandler %>($(this)); 
            });
        <%} %>

        //Bind folder link click events
        $('#' + args.resultContainer).find('a[folderLink]').unbind('click').click(function(event){
            showDialog($(this).attr('folderLink'), $(this).attr('windowName'));
            event.stopPropagation();
        });
        
        //Run onSurveyClick defined in Manage.aspx
        if(_initialSurveyId > 0) {
            var initialSurveyId = _initialSurveyId;
            var initialFolderId = _initialFolderId;

            _initialFolderId = -1;
            _initialSurveyId = -1;
            
            var surveySel = '#survey_' + initialSurveyId;
            var folderSel = '#folderHeader_' + initialFolderId;
            var totalTime = 0;
            //wait for element creation
            var timer = setInterval(function() {
                totalTime += 50;
                if (($(surveySel).length > 0 && initialFolderId <= 0) || $(folderSel).length > 0) {
                    clearInterval(timer);  
                    if (initialFolderId > 0) {
                        toggleFolder(
                            $(folderSel),
                            function() {
                                onSurveyClick($(surveySel));
                            });
                    } else {
                        onSurveyClick($(surveySel));
                    }
                }
                //stop to wait if it's too long
                if(totalTime > 1000) {
                    clearInterval(timer);  
                    loadSurveyData(initialSurveyId);
                }
            }, 50);  
        }
                        
        //Run callback, if any
        if(args.callback && typeof (args.callback) == 'function'){
            args.callback();
        }

        removeLoader();
        _loadInProgress = false;
    }

    //
    function showOnLoad(folderId, surveyId) {
        _initialFolderId = folderId;
        _initialSurveyId = surveyId;
    }

    //Open close folder by showing children and properties information
	function toggleFolder(folderElem, callback) {
        if (folderElem == null) {
		    return;
		}
	    var childrenCount = folderElem.attr('data-childrencount');
	    if (childrenCount != "0") {
	        var folderId = folderElem.attr('folderId');
	        var childElemId = 'folder_' + folderId + '_children';
	        var childElem = $('#' + childElemId);
	        var menuElem = $('#folderMenu_' + folderId);

	        //If children are visible, toggle to hidden
	        if (childElem.is(':visible')) {
	            $(folderElem).removeClass('active');
	            menuElem.hide();
	            childElem.hide('blind', null, 'fast', function() { resizePanels(); });
	            return;
	        }

	        $(folderElem).addClass('active');

	        //If children are showing, determine if load needed and/or show
	        if (childElem.attr('loaded') == 'false') {
	            menuElem.show();
	            loadSurveyAndFolderList(_at, folderId, { resultContainer: childElemId, folderId: folderId, callback: callback });
	        } else {
	            menuElem.show();
	            childElem.show('blind', null, 'fast', function() { resizePanels(); });
	        }
	    }
	}

    //Show confirm dialog about deleting a folder
    function onDeleteFolderClick(folderElem){
        var folderId = folderElem.attr('folderId');
        _deletedElem = folderElem;
        
        if(folderId == null || folderId == ''){
            return null;
        }
        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/deleteFolderConfirm") %>', 
            deleteFolderConfirmCallback,
            null,
            null,
            '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/confirmDeleteFolder") %>'
        );
    }

    //Callback for confirm dialog about deleting a folder
    function deleteFolderConfirmCallback(args){
        if(args.success){
            var folderId = _deletedElem.attr('folderId');
            svcSurveyManagement.deleteFolder(_at, folderId, folderDeletedCallback, null);
        }
    }

    //Handle folder deleted
    function folderDeletedCallback(resultData){
        var message;
        if (resultData)
        {
            clearAndReload();           
            message = '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/folderDeleted") %>';
        }
        else
        {
            message = '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/folderCouldNotDelete") %>';
        }
        <% if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
           {%>
           <%=ShowStatusMessageHandler%>(message, resultData);
        <%
           }%>
    }

    //Handle folder Options menu click
    function folderOptionsClick(event)
    {
        event.stopPropagation();
        var $target = $(event.target);
        $('.folder-options-menu').hide();
        if (($target.hasClass('groupMenuToggle') && $target.parent().hasClass('folderHeader'))) {
            $('.folder-options-menu').hide();
            $(this).parent().find('.groupMenu').show();
            event.stopPropagation();
        }
    }
    //Hide  folder Options menu if click on another DOM element detected
    $('html').click(function(event) {
            $('.folder-options-menu').hide();
    });

</script>
<%-- Container for Results --%>
<div id="listPlace">
</div>

<div class="surveys-filter-buttons">
    <div class="left-header">
        <input id="active-survey-filter" type="checkbox" checked="checked" class="hidden" />
        <input id="inactive-survey-filter" type="checkbox" checked="checked" class="hidden" />
        <label for="active-survey-filter" class="header-button ckbxButton ltGreyButton checked" >Active</label>
        <label for="inactive-survey-filter" class="header-button ckbxButton ltGreyButton checked" >Inactive</label>
    </div>
</div>
<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set folder id to display.  FolderId == 1 indicates root
    /// </summary>
    public string FolderId { get; set; }

    /// <summary>
    /// Get/set callback for handling survey click event
    /// </summary>
    public string OnSurveyClickHandler { get; set; }

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
            "serviceHelper.js",
            ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "svcSurveyManagement.js",
            ResolveUrl("~/Services/js/svcSurveyManagement.js"));

        RegisterClientScriptInclude(
           "svcAuthorization.js",
           ResolveUrl("~/Services/js/svcAuthorization.js"));

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
            "jquery.hoverintent.js",
            ResolveUrl("~/Resources/jquery.hoverintent.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
          "jquery.ckbxProtect.js",
          ResolveUrl("~/Resources/jquery.ckbxProtect.js"));

        RegisterClientScriptInclude(
          "securityHelper.js",
          ResolveUrl("~/Resources/securityHelper.js"));
    }
</script>
