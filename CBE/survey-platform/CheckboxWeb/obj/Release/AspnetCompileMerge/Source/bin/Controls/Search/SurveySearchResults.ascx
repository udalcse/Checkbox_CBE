<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveySearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.SurveySearchResults" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Security" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();

    $(document).ready(function(){
        $(document).on('click', '.surveySearchResult', onSurveyResultSelect);

        templateHelper.loadAndCompileTemplate(
            '<%=ResolveUrl("~/Controls/Search/jqtmpl/folderDashSurveyListTemplate.html") %>',
            'folderDashSurveyListTemplate.html');

        templateHelper.loadAndCompileTemplate(
            '<%=ResolveUrl("~/Controls/Search/jqtmpl/folderDashSurveyListItemTemplate.html") %>',
            'folderDashSurveyListItemTemplate.html');

        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingNameContainer', '<%=ID %>matchingOwnerContainer', '<%=ID %>matchingIdContainer', '<%=ID %>matchingGuidContainer'],
            [<%=_surveyByNameResultsGrid.ReloadGridHandler %>, <%=_surveyByOwnerResultsGrid.ReloadGridHandler %>, <%=_surveyByIdResultsGrid.ReloadGridHandler %>, <%=_surveyByGuidResultsGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcSurveyManagement.search,
            '<%=ID %>'
        );
    });

    //
    function <%=ID%>_startSearch(searchTerm){
        $('#resultCount_<%=ID %>').hide();
        $('#<%=ID %>resultsContainer').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onResultsLoaded);
    }

    //
    function  <%=ID %>onResultsLoaded(resultCount){
        <% if(!RoleManager.UserHasRoleWithPermission(HttpContext.Current.User.Identity.Name, "FormFolder.Read")) { %>
            $('.surveySearchResultHeader').text('<%= WebTextManager.GetText("/controlText/surveySearchResults.ascx/onlySurveysMatching")%>');
        <% } %>               

        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
    }

    //
    function onSurveyResultSelect() {
        <% if(RoleManager.UserHasRoleWithPermission(HttpContext.Current.User.Identity.Name, "FormFolder.Read")) { %>               
            //Clear current dash
            $('#<%=DashContainer %>').empty();//.html('');

            //Figure out what we're doing
            var itemType = $(this).attr('itemType');

            if(itemType == null || itemType == ''){
                return;
            }

            if(itemType == 'folder'){
                loadFolderDashboard($(this).attr('folderId'));
            }
            else {
               loadSurveyDashboard($(this).attr('surveyId'));
            }
        <% } else { %>
            window.open('<%=ResolveUrl("~/Survey.aspx?s=") %>' + $(this).attr('guid') + '<%=UserIDSection%>', '_blank');        
        <% } %>
    }

    //
    function loadSurveyDashboard(surveyId){
        if(surveyId == null || surveyId == ''){
            return;
        }

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcSurveyManagement.getSurveyMetaData(
            _at, 
            surveyId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                resultData.searchTerm = searchResults_<%=ID %>.term;

                templateHelper.loadAndApplyTemplate(
                    'surveySearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/surveySearchDashTemplate.html") %>',
                    resultData,
                    null,
                    '<%=DashContainer %>',
                    true);
            }
        );
    }

    //
    function loadFolderDashboard(folderId){
        if(folderId == null || folderId == ''){
            return;
        }

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcSurveyManagement.getFolderListItem(
            _at, 
            folderId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                templateHelper.loadAndApplyTemplate(
                    'folderSearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/folderSearchDashTemplate.html") %>',
                    resultData,
                    null,
                    '<%=DashContainer %>',
                    true);
            }
        );
    }

    //
    function <%=ID%>_loadResultsByName(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingName', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onSurveyResultsByNameGridRendered(){
        $('.surveyName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByOwner(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingOwner', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onSurveyResultsByOwnerGridRendered(){
        $('.surveyOwner').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsById(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingId', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onSurveyResultsByIdGridRendered(){
        $('.surveyId').highlight(searchResults_<%=ID %>.term);
    }

    function <%=ID%>_loadResultsByGuid(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingGuid', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onSurveyResultsByGuidGridRendered(){
        $('.surveyGuid').highlight(searchResults_<%=ID %>.term);
    }

</script>

<div id="<%=ID%>resultHeader" class="dashStatsHeader">
    <span class="left mainStats">
        <div id="detailProgressContainer_<%=ID %>" class="left">
            <div id="detailProgress_<%=ID %>" class="ProgressSpinner">
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" Style="height:24px;" />
            </div>
        </div>
        <span id="resultCount_<%=ID %>" style="display:none;" class="left"></span>
        <span class="left">&nbsp;&nbsp;<span class="surveySearchResultHeader"><%=WebTextManager.GetText("/controlText/surveySearchResults.ascx/surveysMatching")%></span>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">
    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/surveySearchResults.ascx/noSurveys")%></div>

    <div class="padding10" id="<%=ID %>matchingNameContainer">
        <div class="label"><span class="surveySearchResultHeader">Surveys and Folders matching</span> NAME:</div>
        <ckbx:Grid ID="_surveyByNameResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingOwnerContainer">
        <div class="label">Surveys and Folders matching CREATED BY:</div>
        <ckbx:Grid ID="_surveyByOwnerResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingIdContainer">
        <div class="label">Surveys and folders matching ID:</div>
        <ckbx:Grid ID="_surveyByIdResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingGuidContainer">
        <div class="label">Surveys and Folders matching GUID:</div>
        <ckbx:Grid ID="_surveyByGuidResultsGrid" runat="server" />
    </div>
</div>

<script type="text/C#" runat="server">

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _surveyByNameResultsGrid.InitialSortField = "Name";
        _surveyByNameResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _surveyByNameResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyResultListTemplate.html");
        _surveyByNameResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyResultListItemTemplate.html");
        _surveyByNameResultsGrid.LoadDataCallback = ID + "_loadResultsByName";
        _surveyByNameResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noSurveys");
        _surveyByNameResultsGrid.DelayLoad = true;
        _surveyByNameResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingSurveys";
        _surveyByNameResultsGrid.RenderCompleteCallback = "onSurveyResultsByNameGridRendered";

        _surveyByOwnerResultsGrid.InitialSortField = "Name";
        _surveyByOwnerResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _surveyByOwnerResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyResultListTemplate.html");
        _surveyByOwnerResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyResultListItemTemplate.html");
        _surveyByOwnerResultsGrid.LoadDataCallback = ID + "_loadResultsByOwner";
        _surveyByOwnerResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noSurveys");
        _surveyByOwnerResultsGrid.DelayLoad = true;
        _surveyByOwnerResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingSurveys";
        _surveyByOwnerResultsGrid.RenderCompleteCallback = "onSurveyResultsByOwnerGridRendered";

        _surveyByIdResultsGrid.InitialSortField = "Name";
        _surveyByIdResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _surveyByIdResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyByIdResultListTemplate.html");
        _surveyByIdResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyByIdResultListItemTemplate.html");
        _surveyByIdResultsGrid.LoadDataCallback = ID + "_loadResultsById";
        _surveyByIdResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noSurveys");
        _surveyByIdResultsGrid.DelayLoad = true;
        _surveyByIdResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingSurveys";
        _surveyByIdResultsGrid.RenderCompleteCallback = "onSurveyResultsByIdGridRendered";

        _surveyByGuidResultsGrid.InitialSortField = "Name";
        _surveyByGuidResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _surveyByGuidResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyByGuidResultListTemplate.html");
        _surveyByGuidResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/surveyByGuidResultListItemTemplate.html");
        _surveyByGuidResultsGrid.LoadDataCallback = ID + "_loadResultsByGuid";
        _surveyByGuidResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noSurveys");
        _surveyByGuidResultsGrid.DelayLoad = true;
        _surveyByGuidResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingSurveys";
        _surveyByGuidResultsGrid.RenderCompleteCallback = "onSurveyResultsByGuidGridRendered";
    }

    /// <summary>
    /// Gets the "u=.." part of the survey URL if it is needed
    /// </summary>
    protected string UserIDSection
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            //Add user guid, if a user is logged in
            if (UserManager.GetCurrentPrincipal() != null)
            {
                string guidString = UserManager.GetCurrentPrincipal().UserGuid.ToString();

                if (Utilities.IsNotNullOrEmpty(guidString))
                {
                    sb.Append("&u=");
                    sb.Append(guidString.Replace("-", string.Empty));
                }
            }
            return sb.ToString();

        }
    }

    /// <summary>
    /// Ensure necessary scripts are loaded
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        //Service helper
        RegisterClientScriptInclude(
            "serviceHelper.js",
            ResolveUrl("~/Services/js/serviceHelper.js"));

        //Survey management
        RegisterClientScriptInclude(
            "svcSurveyManagement.js",
            ResolveUrl("~/Services/js/svcSurveyManagement.js"));

        //Search results client script
        RegisterClientScriptInclude(
            "searchResults.js",
            ResolveUrl("~/Controls/Search/searchResults.js"));
        
        //Highlight
        RegisterClientScriptInclude(
            "jquery.highlight-3.yui.js",
            ResolveUrl("~/Resources/jquery.highlight-3.yui.js"));
    }
</script>
