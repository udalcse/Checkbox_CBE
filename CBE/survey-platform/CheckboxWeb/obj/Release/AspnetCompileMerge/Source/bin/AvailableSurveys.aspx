<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="AvailableSurveys.aspx.cs" Inherits="CheckboxWeb.AvailableSurveys" %>
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
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/availableSurveyListTemplate.html") %>', 'availableSurveyListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/availableSurveyListSurveyTemplate.html") %>', 'availableSurveyListSurveyTemplate.html');

        //resizePanels();
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

    <%-- Load survey list --%>
    function loadSurveyList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcSurveyManagement.listAvailableSurveys(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: 'Name',
                filterValue: escapeInjections($("#<%=_filter.ClientID%>").val()),
                sortField: sortField,
                sortAscending: sortAscending,
                skipAuthentication : true
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );      
    }
    
    function surveySelected(survey)
    {
        window.open('<%=ResolveUrl("~/Survey.aspx?s=") %>' + survey.SurveyGuid + '<%=UserIDSection%>', '_blank');        
    }

    function reloadGrid()
    {
        <%=_grid.ReloadGridHandler %>(true);
    }
</script>
<div class="padding15 avaliable-items-container">
    <div class="gridMenu">
        <div class="left gridSorter" style="float:left;padding-right:15px;">
            <asp:TextBox ID="_filter" runat="server" class="roundedCorners"/>
            <a href="#" onclick="reloadGrid();" class="ckbxButton orangeButton roundedCorners"><%=WebTextManager.GetText("/pageText/availableSurveys.aspx/filter")%></a>
        </div>
        <div class="left gridSorter" style="float:left">
            <sort:Sorter ID="_sorter" runat="server">
                <sort:SortOption SortField="TemplateName" TextId="/pageText/libraries/availableSurveys.aspx/surveyName" />
            </sort:Sorter>
        </div>
        <div class="clear" />
    </div>


    <%-- Container for Results --%>
    <div style="margin-top:25px; margin-bottom:150px">
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

        _grid.InitialSortField = "TemplateName";
        _grid.ItemClickCallback = "surveySelected";
        _grid.ListTemplatePath = ResolveUrl("~/Forms/jqtmpl/availableSurveyListTemplate.html");
        _grid.ListItemTemplatePath = ResolveUrl("~/Forms/jqtmpl/availableSurveyListSurveyTemplate.html");
        _grid.LoadDataCallback = "loadSurveyList";
        _grid.EmptyGridText = WebTextManager.GetText("/pageText/availableSurveys.aspx/noForms");

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
        
        if (currentPrincipal == null && !ApplicationManager.AppSettings.DisplayAvailableSurveyList)
        {
            Response.Redirect(ResolveUrl("~/Login.aspx"), false);                      
            return;
        }

        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "svcSurveyManagement.js",
            ResolveUrl("~/Services/js/svcSurveyManagement.js"));
    }
</script>
