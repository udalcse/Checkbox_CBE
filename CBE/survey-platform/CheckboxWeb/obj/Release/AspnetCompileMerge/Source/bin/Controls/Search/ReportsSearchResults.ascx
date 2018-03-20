<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportsSearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.ReportsSearchResults" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();

    $(document).ready(function(){
        $(document).on('click', '.reportSearchResult', onReportResultSelect);
                
        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingNameContainer', '<%=ID %>matchingOwnerContainer'],
            [<%=_reportsByNameResultsGrid.ReloadGridHandler %>, <%=_reportsByOwnerResultsGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcReportManagement.search,
            '<%=ID %>'
        );
    });

   
    function <%=ID%>_startSearch(searchTerm){
        $('#resultCount_<%=ID %>').hide();
        $('#<%=ID %>resultsContainer').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onResultsLoaded);
    }

    //
    function  <%=ID %>onResultsLoaded(resultCount){
        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
    }

     function onReportResultSelect(){
        var reportId = $(this).attr('reportId');

        if(reportId == null || reportId == ''){
            return;
        }

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcReportManagement.getReportMetaData(
            _at, 
            reportId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                templateHelper.loadAndApplyTemplate(
                    'reportSearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/reportSearchDashTemplate.html") %>',
                    resultData,
                    {
                        reportRunUrl: '<%=ApplicationManager.ApplicationPath + "/RunAnalysis.aspx?ag=" %>' + resultData.ReportGuid,
                        reportEditUrl: '<%=ApplicationManager.ApplicationPath + "/Forms/Surveys/Reports/Edit.aspx?r=" %>' + resultData.ReportId
                    },
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
    function onReportResultsByNameGridRendered(){
        $('.reportResultsReportName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByOwner(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingOwner', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onReportResultsByOwnerGridRendered(){
        $('.reportResultsOwnerName').highlight(searchResults_<%=ID %>.term);
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
        <span class="left">&nbsp;&nbsp;<%=WebTextManager.GetText("/controlText/reportsSearchResults.ascx/reportsMatching")%>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">
    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/reportsSearchResults.ascx/noReports")%></div>

    <div class="padding10" id="<%=ID %>matchingNameContainer">
        <div class="label">Matching NAME:</div>
        <ckbx:Grid ID="_reportsByNameResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingOwnerContainer">
        <div class="label">Matching CREATED BY:</div>
        <ckbx:Grid ID="_reportsByOwnerResultsGrid" runat="server" />
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

        _reportsByNameResultsGrid.InitialSortField = "Name";
        _reportsByNameResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _reportsByNameResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/reportsByReportNameResultListTemplate.html");
        _reportsByNameResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/reportsByReportNameResultListItemTemplate.html");
        _reportsByNameResultsGrid.LoadDataCallback = ID + "_loadResultsByName";
        _reportsByNameResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/reportsSearchResults.ascx/noReports");
        _reportsByNameResultsGrid.DelayLoad = true;
        _reportsByNameResultsGrid.LoadingTextId = "/controlText/reportsSearchResults.ascx/findingReports";
        _reportsByNameResultsGrid.RenderCompleteCallback = "onReportResultsByNameGridRendered";

        _reportsByOwnerResultsGrid.InitialSortField = "Name";
        _reportsByOwnerResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _reportsByOwnerResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/reportsByOwnerResultListTemplate.html");
        _reportsByOwnerResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/reportsByOwnerResultListItemTemplate.html");
        _reportsByOwnerResultsGrid.LoadDataCallback = ID + "_loadResultsByOwner";
        _reportsByOwnerResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/reportsSearchResults.ascx/noReports");
        _reportsByOwnerResultsGrid.DelayLoad = true;
        _reportsByOwnerResultsGrid.LoadingTextId = "/controlText/reportsSearchResults.ascx/findingReports";
        _reportsByOwnerResultsGrid.RenderCompleteCallback = "onReportResultsByOwnerGridRendered";
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
            "svcReportManagement.js",
            ResolveUrl("~/Services/js/svcReportManagement.js"));

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
    }
</script>
