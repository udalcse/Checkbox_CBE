<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyStyleList.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.SurveyStyleList" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript">
    <%-- Load user list --%>
    function loadSurveyStyleList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcSurveyManagement.listSurveyStyleTemplates(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: '',
                filterValue: '',
                sortField: sortField,
                sortAscending: sortAscending
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }
</script>


<%-- Container for Results --%>
<ckbx:Grid ID="_styleGrid" runat="server" />
    

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _styleGrid.InitialSortField = "Name";
        _styleGrid.ItemClickCallback = StyleSelectedClientCallback;
        _styleGrid.ListTemplatePath = ResolveUrl("~/Styles/jqtmpl/styleListTemplate.html");
        _styleGrid.ListItemTemplatePath = ResolveUrl("~/Styles/jqtmpl/styleListItemTemplate.html");
        _styleGrid.LoadDataCallback = "loadSurveyStyleList";
        _styleGrid.EmptyGridText = WebTextManager.GetText("/pageText/styles_default.aspx/noStylesFound");
    }
    
    
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
    }
</script>
