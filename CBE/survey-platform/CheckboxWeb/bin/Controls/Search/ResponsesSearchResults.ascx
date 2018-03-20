<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponsesSearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.ResponsesSearchResults" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();

    $(document).ready(function(){
        $(document).on('click', '.responseSearchResult', onResponseRowSelected);
        
        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingUserContainer', '<%=ID %>matchingInviteeContainer', '<%=ID %>matchingIdContainer', '<%=ID %>matchingGuidContainer'],
            [<%=_responsesByUserResultsGrid.ReloadGridHandler %>, <%=_responsesByInviteeResultsGrid.ReloadGridHandler %>, <%=_responsesByIdResultsGrid.ReloadGridHandler %>, <%=_responsesByGuidResultsGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcResponseData.search,
            '<%=ID %>'
        );
    });

    //
    function <%=ID%>_startSearch(searchTerm){
        $('#resultCount_<%=ID %>').hide();
        $('#<%=ID %>resultsContainer').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onReportResultsLoaded);
    }

     //
    function <%=ID %>onReportResultsLoaded(resultCount){
        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
    }

    //
    function onResponseRowSelected()  {
        onResponseSelected($(this).attr('responseGuid'));
    }

    //
    function onResponseSelected(responseGuid) {
        if(responseGuid == null
            || responseGuid == ''){
                return;
        }

        UFrameManager.init({
            id: '<%= DashContainer %>',
            loadFrom: '<%=ResolveUrl("~/") %>Forms/Surveys/Responses/View.aspx',
            params : {responseGUID: responseGuid},
            progressTemplate : $('#detailProgressContainer_<%=ID %>').html(),
            showProgress: true
        });
    }



    //
    function <%=ID%>_loadResultsByUserName(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingUser', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onResponseResultsByUserGridRendered(){
        $('.userName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByInvitee(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingInvitee', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onResponseResultsByInviteeGridRendered(){
        $('.invitee').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsById(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingId', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onResponseResultsByIdGridRendered(){
        $('.responseId').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByGuid(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingGuid', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onResponseResultsByGuidGridRendered(){
        $('.responseGuid').highlight(searchResults_<%=ID %>.term);
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
        <span class="left">&nbsp;&nbsp;<%=WebTextManager.GetText("/controlText/surveySearchResults.ascx/responsesMatching")%>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">
    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/surveySearchResults.ascx/noResponses")%></div>

    <div class="padding10" id="<%=ID %>matchingUserContainer">
        <div class="label">Matching RESPONDENT:</div>
        <ckbx:Grid ID="_responsesByUserResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingInviteeContainer">
        <div class="label">Matching INVITEE:</div>
        <ckbx:Grid ID="_responsesByInviteeResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingIdContainer">
        <div class="label">Matching ID:</div>
        <ckbx:Grid ID="_responsesByIdResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingGuidContainer">
        <div class="label">Matching GUID:</div>
        <ckbx:Grid ID="_responsesByGuidResultsGrid" runat="server" />
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

        _responsesByUserResultsGrid.InitialSortField = "UserIdentifier";
        _responsesByUserResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _responsesByUserResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByUserResultListTemplate.html");
        _responsesByUserResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByUserResultListItemTemplate.html");
        _responsesByUserResultsGrid.LoadDataCallback = ID + "_loadResultsByUserName";
        _responsesByUserResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noResponses");
        _responsesByUserResultsGrid.DelayLoad = true;
        _responsesByUserResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingResponses";
        _responsesByUserResultsGrid.RenderCompleteCallback = "onResponseResultsByUserGridRendered";

        _responsesByInviteeResultsGrid.InitialSortField = "Invitee";
        _responsesByInviteeResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _responsesByInviteeResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByInviteeResultListTemplate.html");
        _responsesByInviteeResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByInviteeResultListItemTemplate.html");
        _responsesByInviteeResultsGrid.LoadDataCallback = ID + "_loadResultsByInvitee";
        _responsesByInviteeResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noResponses");
        _responsesByInviteeResultsGrid.DelayLoad = true;
        _responsesByInviteeResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingResponses";
        _responsesByInviteeResultsGrid.RenderCompleteCallback = "onResponseResultsByInviteeGridRendered";

        _responsesByIdResultsGrid.InitialSortField = "Id";
        _responsesByIdResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _responsesByIdResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByIdResultListTemplate.html");
        _responsesByIdResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByIdResultListItemTemplate.html");
        _responsesByIdResultsGrid.LoadDataCallback = ID + "_loadResultsById";
        _responsesByIdResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noResponses");
        _responsesByIdResultsGrid.DelayLoad = true;
        _responsesByIdResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingResponses";
        _responsesByIdResultsGrid.RenderCompleteCallback = "onResponseResultsByIdGridRendered";

        _responsesByGuidResultsGrid.InitialSortField = "Guid";
        _responsesByGuidResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _responsesByGuidResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByGuidResultListTemplate.html");
        _responsesByGuidResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/responseByGuidResultListItemTemplate.html");
        _responsesByGuidResultsGrid.LoadDataCallback = ID + "_loadResultsByGuid";
        _responsesByGuidResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/surveySearchResults.ascx/noResponses");
        _responsesByGuidResultsGrid.DelayLoad = true;
        _responsesByGuidResultsGrid.LoadingTextId = "/controlText/surveySearchResults.ascx/findingResponses";
        _responsesByGuidResultsGrid.RenderCompleteCallback = "onResponseResultsByGuidGridRendered";
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
            "svcResponseData.js",
            ResolveUrl("~/Services/js/svcResponseData.js"));

        //Search results client script
        RegisterClientScriptInclude(
            "searchResults.js",
            ResolveUrl("~/Controls/Search/searchResults.js"));
        
        //Highlight
        RegisterClientScriptInclude(
            "jquery.highlight-3.yui.js",
            ResolveUrl("~/Resources/jquery.highlight-3.yui.js"));

        //Date Utils
        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        //Text helper
        RegisterClientScriptInclude(
            "textHelper.js",
            ResolveUrl("~/Resources/textHelper.js"));

        //Helper for uframe
        RegisterClientScriptInclude(
            "htmlparser.js",
            ResolveUrl("~/Resources/htmlparser.js"));

        //Helper for uframe
        RegisterClientScriptInclude(
            "UFrame.js",
            ResolveUrl("~/Resources/UFrame.js"));
    }
</script>
