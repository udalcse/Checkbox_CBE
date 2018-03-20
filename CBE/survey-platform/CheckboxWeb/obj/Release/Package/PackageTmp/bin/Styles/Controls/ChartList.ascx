<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChartList.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.ChartList" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" TagName="Sorter" Src="~/Controls/Sorter.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" assembly="CheckboxWeb"%>

<script type="text/javascript" language="javascript">
    function loadChartStyleList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs) {
        $('#formsLink').removeClass('selected');
        $('#chartLink').addClass('selected');

        svcStyleManagement.listChartStyles(
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
<div class="gridMenu">
    <div class="left gridSorter">
        <sort:Sorter ID="_sorter" runat="server">
            <sort:SortOption SortField="Name" TextId="/styles/chartStyleListTemplate/styleName" />        
        </sort:Sorter>
    </div>
    <div class="right gridButtons">
        <ul class="itemActionMenu allMenu">
            <li><a class="ckbxButton border999 shadow999 redButton roundedCorners" href="javascript:void(0);" id="_deleteSelectedUsersLink"><%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteSelectedStyle")%></a></li>
        </ul>
    </div>
    <br class="clear" />
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_chartGrid" runat="server" GridCssClass="ckbxGrid" />

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
    /// <summary>
    /// Get/set callback for handling user delete event
    /// </summary>
    public string OnStyleDeleted { get; set; }
    
    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _chartGrid.InitialSortField = "Name";
        _chartGrid.ItemClickCallback = StyleSelectedClientCallback;
        _chartGrid.ListTemplatePath = ResolveUrl("~/Styles/jqtmpl/chartStyleListTemplate.html");
        _chartGrid.ListItemTemplatePath = ResolveUrl("~/Styles/jqtmpl/chartStyleListItemTemplate.html");
        _chartGrid.LoadDataCallback = "loadChartStyleList";
        _chartGrid.EmptyGridText = WebTextManager.GetText("/pageText/Styles/Manage.aspx/noStyles");
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
           "svcUserManagement.js",
           ResolveUrl("~/Services/js/svcStyleManagement.js"));
    }
</script>
