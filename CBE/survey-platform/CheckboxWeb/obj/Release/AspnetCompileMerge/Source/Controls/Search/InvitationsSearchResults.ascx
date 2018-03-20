<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="InvitationsSearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.InvitationsSearchResults" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();
    var _dashInvitationId = -1;

    $(document).ready(function(){
        $(document).on('click', '.invitationSearchResult', onInvitationResultSelect);
        
        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingNameContainer', '<%=ID %>matchingIdContainer', '<%=ID %>matchingRecipientEmailContainer', '<%=ID %>matchingRecipientIdContainer', '<%=ID %>matchingRecipientGuidContainer', '<%=ID %>matchingOwnerContainer'],
            [<%=_invitationResultsByNameGrid.ReloadGridHandler %>, <%=_invitationResultsByIdGrid.ReloadGridHandler %>, <%=_invitationResultsByRecipientEmailGrid.ReloadGridHandler %>, <%=_invitationResultsByRecipientIdGrid.ReloadGridHandler %>, <%=_invitationResultsByRecipientGuidGrid.ReloadGridHandler %>, <%=_invitationResultsByOwnerGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcInvitationManagement.searchInvitations,
            '<%=ID %>'
        );
    });

 
    function <%=ID%>_startSearch(searchTerm){
        $('#<%=ID %>resultsContainer').hide();
        $('#resultCount_<%=ID %>').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onResultsLoaded);
    }

    //
    function  <%=ID %>onResultsLoaded(resultCount){
        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
    }
    
    //
    function onInvitationResultSelect(){
        loadInvitationData($(this).attr('invitationId'));
    }

    //
    function loadInvitationData(invitationId){
     if(invitationId == null || invitationId == ''){
            return;
        }
        
        _dashInvitationId = invitationId;

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcInvitationManagement.getInvitation(
            _at, 
            invitationId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                templateHelper.loadAndApplyTemplate(
                    'invitationSearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/invitationSearchDashTemplate.html") %>',
                    resultData,
                    null,
                    '<%=DashContainer %>',
                    true,
                    onInvitationDashTemplateApplied,
                    {invitationId: invitationId}
                );
            }
        );
    }

    //
    function onInvitationDashTemplateApplied(){
    }

    //
    function <%=ID%>_loadResultsByName(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingName', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onInvitationResultsByNameGridRendered(){
        $('.invitationResultsInvitationName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsById(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingId', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onInvitationResultsByIdGridRendered(){
        $('.invitationResultsInvitationId').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByRecipientEmail(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingRecipientEmail', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onInvitationResultsByRecipientEmailGridRendered(){
        $('.invitationResultsRecipientEmail').highlight(searchResults_<%=ID %>.term);
    }

    function <%=ID%>_loadResultsByRecipientId(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingRecipientId', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onInvitationResultsByRecipientIdGridRendered(){
        $('.invitationResultsRecipientId').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByRecipientGuid(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingRecipientGuid', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onInvitationResultsByRecipientGuidGridRendered(){
        $('.invitationResultsRecipientGuid').highlight(searchResults_<%=ID %>.term);
    }

     //
    function <%=ID%>_loadResultsByOwner(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingOwner', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onInvitationResultsByOwnerGridRendered(){
        $('.invitationResultsOwner').highlight(searchResults_<%=ID %>.term);
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
        <span class="left">&nbsp;&nbsp;<%=WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/invitationsMatching")%>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">
    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations")%></div>

    <div class="padding10" id="<%=ID %>matchingNameContainer">
        <div class="label">Invitations matching NAME:</div>
        <ckbx:Grid ID="_invitationResultsByNameGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingOwnerContainer">
        <div class="label">Invitations matching CREATED BY:</div>
        <ckbx:Grid ID="_invitationResultsByOwnerGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingIdContainer">
        <div class="label">Invitations matching INVITATION ID:</div>
        <ckbx:Grid ID="_invitationResultsByIdGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingRecipientEmailContainer">
        <div class="label">Invitations matching RECIPIENT EMAIL:</div>
        <ckbx:Grid ID="_invitationResultsByRecipientEmailGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingRecipientIdContainer">
        <div class="label">Invitations matching RECIPIENT ID:</div>
        <ckbx:Grid ID="_invitationResultsByRecipientIdGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingRecipientGuidContainer">
        <div class="label">Invitations matching RECIPIENT GUID:</div>
        <ckbx:Grid ID="_invitationResultsByRecipientGuidGrid" runat="server" />
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

        _invitationResultsByNameGrid.InitialSortField = "Name";
        _invitationResultsByNameGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByNameGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByNameResultListTemplate.html");
        _invitationResultsByNameGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByNameResultListItemTemplate.html");
        _invitationResultsByNameGrid.LoadDataCallback = ID + "_loadResultsByName";
        _invitationResultsByNameGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByNameGrid.DelayLoad = true;
        _invitationResultsByNameGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByNameGrid.RenderCompleteCallback = "onInvitationResultsByNameGridRendered";

        _invitationResultsByIdGrid.InitialSortField = "Name";
        _invitationResultsByIdGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByIdGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByIdResultListTemplate.html");
        _invitationResultsByIdGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByIdResultListItemTemplate.html");
        _invitationResultsByIdGrid.LoadDataCallback = ID + "_loadResultsById";
        _invitationResultsByIdGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByIdGrid.DelayLoad = true;
        _invitationResultsByIdGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByIdGrid.RenderCompleteCallback = "onInvitationResultsByIdGridRendered";

        _invitationResultsByRecipientEmailGrid.InitialSortField = "Name";
        _invitationResultsByRecipientEmailGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByRecipientEmailGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientEmailResultListTemplate.html");
        _invitationResultsByRecipientEmailGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientEmailResultListItemTemplate.html");
        _invitationResultsByRecipientEmailGrid.LoadDataCallback = ID + "_loadResultsByRecipientEmail";
        _invitationResultsByRecipientEmailGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByRecipientEmailGrid.DelayLoad = true;
        _invitationResultsByRecipientEmailGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByRecipientEmailGrid.RenderCompleteCallback = "onInvitationResultsByRecipientEmailGridRendered";

        _invitationResultsByRecipientIdGrid.InitialSortField = "Name";
        _invitationResultsByRecipientIdGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByRecipientIdGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientIdResultListTemplate.html");
        _invitationResultsByRecipientIdGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientIdResultListItemTemplate.html");
        _invitationResultsByRecipientIdGrid.LoadDataCallback = ID + "_loadResultsByRecipientId";
        _invitationResultsByRecipientIdGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByRecipientIdGrid.DelayLoad = true;
        _invitationResultsByRecipientIdGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByRecipientIdGrid.RenderCompleteCallback = "onInvitationResultsByRecipientIdGridRendered";

        _invitationResultsByRecipientGuidGrid.InitialSortField = "Name";
        _invitationResultsByRecipientGuidGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByRecipientGuidGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientGuidResultListTemplate.html");
        _invitationResultsByRecipientGuidGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByRecipientGuidResultListItemTemplate.html");
        _invitationResultsByRecipientGuidGrid.LoadDataCallback = ID + "_loadResultsByRecipientGuid";
        _invitationResultsByRecipientGuidGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByRecipientGuidGrid.DelayLoad = true;
        _invitationResultsByRecipientGuidGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByRecipientGuidGrid.RenderCompleteCallback = "onInvitationResultsByRecipientGuidGridRendered";

        _invitationResultsByOwnerGrid.InitialSortField = "Name";
        _invitationResultsByOwnerGrid.ItemClickCallback = ClientResultSelectedHandler;
        _invitationResultsByOwnerGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByOwnerResultListTemplate.html");
        _invitationResultsByOwnerGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/invitationByOwnerResultListItemTemplate.html");
        _invitationResultsByOwnerGrid.LoadDataCallback = ID + "_loadResultsByOwner";
        _invitationResultsByOwnerGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationsSearchResults.ascx/noInvitations");
        _invitationResultsByOwnerGrid.DelayLoad = true;
        _invitationResultsByOwnerGrid.LoadingTextId = "/controlText/invitationsSearchResults.ascx/findingInvitations";
        _invitationResultsByOwnerGrid.RenderCompleteCallback = "onInvitationResultsByOwnerGridRendered";
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
            "svcInvitationManagement.js",
            ResolveUrl("~/Services/js/svcInvitationManagement.js"));

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
