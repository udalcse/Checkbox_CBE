<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="AvailableReports.aspx.cs" Inherits="CheckboxWeb.AvailableReports" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" TagName="Sorter" Src="~/Controls/Sorter.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" Assembly="CheckboxWeb"%>
<%@ Import Namespace="Checkbox.Security.Principal" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<script type="text/javascript">
	$(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/availableReportListTemplate.html") %>', 'availableReportListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/availableReportListReportTemplate.html") %>', 'availableReportListReportTemplate.html');

        resizePanels();
	});

    //resize panels
    function resizePanels() {
        $.Deferred(function (dfd) {
            //Start with default values
            var headerHeight = 45;          //45 is default height of header
            var trialBannerHeight = 0;      //0 is default height of hidden banner

            //Attempt to populate with real values
            if ($('.topWrapper').length > 0) {
                headerHeight = $('.topWrapper').height();
            }

            if ($('#trialBanner').length > 0) {
                trialBannerHeight = $('#trialBanner').height() + 12;  //Add 12 to account for 4px padding and 2px border on top/bottom
            }

            var panelHeight = $('.gridMenu').height() - headerHeight - trialBannerHeight - 35; //35 is height of visible footer
            if (panelHeight < 678) { panelHeight = 678; }

            $('.avaliable-items-container').css('min-height', panelHeight);

            dfd.resolve();
        });
    }

    <%-- Load Report list --%>
    function loadReportList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcReportManagement.listAvailableReports(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: 'AnalysisName',
                filterValue: $("#<%=_filter.ClientID%>").val(),
                sortField: sortField,
                sortAscending: sortAscending,
                skipAuthentication : true
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }

    function ReportSelected(Report)
    {
        window.open('<%=ResolveUrl("~/RunAnalysis.aspx?aid=") %>' + Report.ReportId, '_blank');        
    }

    function reloadGrid()
    {
        <%=_grid.ReloadGridHandler %>(true);
    }
</script>
<div class="padding15 avaliable-items-container">
    <div class="gridMenu">
        <div class="left gridSorter">
            <asp:TextBox ID="_filter" runat="server" class="roundedCorners"/>
            <a href="#" onclick="reloadGrid();" class="ckbxButton orangeButton roundedCorners"><%=WebTextManager.GetText("/pageText/availableReports.aspx/filter")%></a>
        </div>
        <div class="padding10 left" ></div>
        <div class="left gridSorter">
            <sort:Sorter ID="_sorter" runat="server">
                <sort:SortOption SortField="AnalysisName" TextId="/pageText/availableReports.aspx/reportName" />
            </sort:Sorter>
        </div>
        <div class="clear" />
    </div>

    <div style="margin-top:25px;">
        <%-- Container for Results --%>
        <ckbx:Grid ID="_grid" runat="server"/>
    </div>
</div>

</asp:Content>

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _grid.InitialSortField = "AnalysisName";
        _grid.ItemClickCallback = "ReportSelected";
        _grid.ListTemplatePath = ResolveUrl("~/Forms/jqtmpl/availableReportListTemplate.html");
        _grid.ListItemTemplatePath = ResolveUrl("~/Forms/jqtmpl/availableReportListReportTemplate.html");
        _grid.LoadDataCallback = "loadReportList";
        _grid.EmptyGridText = WebTextManager.GetText("/pageText/availableReports.aspx/noReports");

        _sorter.Initialize(_grid);
    }


    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CheckboxPrincipal currentPrincipal = UserManager.GetCurrentPrincipal();
        
        if (currentPrincipal == null && !ApplicationManager.AppSettings.DisplayAvailableReportList)
        {
            Response.Redirect(ResolveUrl("~/Login.aspx"), false);
            return;
        }
        
        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "svcReportManagement.js",
            ResolveUrl("~/Services/js/svcReportManagement.js"));
    }

</script>



