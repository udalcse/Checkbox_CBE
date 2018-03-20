<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StyleList.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.StyleList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" assembly="CheckboxWeb"%>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Styles" %>

<script type="text/javascript">
    //Elem, which is going to be deleted. It's needed for radConfirm dialog.
    var _deletedElem;

    <%-- Ensure service initialized --%>
    $(document).ready(function() {
        //implementation all/none selection
        $(document).on('click', '.deleteStyle', function(){
           toggleGridActionButtons('style', this);
        });
        $(document).on('click', '#_selectAllStyles', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteStyle').prop('checked', true);
            } else {
                $('.deleteStyle').prop('checked', false);
            }
            $.uniform.update('.deleteStyle');
            toggleGridActionButtons('style', this, actionsAvailable);
        });

        //Bind delete selected styles click
        $('#_deleteSelectedStylesLink').click(function(){
            if($('.deleteStyle:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/styles/Manage.aspx/deleteSelectedStyleConfirm") %>', 
                    onDeleteSelectedStylesConfirm,
                    350,
                    200,
                    '<%=WebTextManager.GetText("/pageText/styles/Manage.aspx/deleteSelectedStyle") %>'
                );
            }
        });
    });
    
    //Make ajax call to load list and then call selected callback
    function loadFormStyleList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs) {
        //TODO: Paging?
        svcStyleManagement.listFormStyles(
            _at,
            {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                <% if (StyleType != StyleTemplateType.Any) { %>
                filterField: 'Type',
                filterValue: '<%=StyleType%>',
                <%} %>
                sortField: sortField,
                sortAscending: sortAscending
            },
            loadCompleteCallback,
            loadCompleteArgs
        );
    }

    //Make ajax call to load list
    function loadFormStyleListAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        svcStyleManagement.listFormStyles(
            _at,
            {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: filterField,
                filterValue: filterValue,
                sortField: sortField,
                sortAscending: sortAscending
            },
            loadCompleteCallback,
            loadCompleteArgs
        );
    }

    //Confirm handler for deleting users
    function onDeleteSelectedStylesConfirm(args){
        if(args.success){
            var idArray = new Array();

            $('.deleteStyle:checked').each(function(index){
                idArray.push($(this).attr('value'));
            });

            if (idArray.length > 0){
                svcStyleManagement.deleteFormStyles(
                     _at,
                     idArray,
                     onStylesDeleted,
                     idArray.length
                 );
            }
        }
    }

    //styles deleted handler
    function onStylesDeleted(resultData, count){
        <%if (!String.IsNullOrEmpty(OnStyleDeleted))
          {%>
            <%=OnStyleDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/styles/Manage.aspx/stylesDeleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), true);
        <%
          }%> 
    }

    //Reload data in style and style list
    function reloadStyleList(resetPaging){
        <%=_styleGrid.ReloadGridHandler %>(resetPaging);
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_styleGrid.ShowSorter %>();
    }
</script>

<div class="gridMenu">
    <div class="right gridButtons">
        <div class="itemActionMenu">
            <a class="ckbxButton border999 shadow999 redButton roundedCorners" href="javascript:void(0);" id="_deleteSelectedStylesLink"><%=WebTextManager.GetText("/pageText/Styles/Manage.aspx/deleteSelectedStyle")%></a>
        </div>
    </div>
    <br class="clear" />
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_styleGrid" runat="server" GridCssClass="ckbxGrid" />

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set style id to display.  StyleId == 1 indicates root
    /// </summary>
    public string StyleId { get; set; }

    /// <summary>
    /// Get/set callback for handling user delete event
    /// </summary>
    public string OnStyleDeleted { get; set; }

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected StyleTemplateType StyleType
    {
        get
        {
            switch (Request.Params["t"])
            {
                case "t":
                    return StyleTemplateType.Tablet;
                case "s":
                    return StyleTemplateType.SmartPhone;
                case "p":
                    return StyleTemplateType.PC;
                default:
                    return StyleTemplateType.Any;                    
            }
        }
    }

    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _styleGrid.InitialSortField = "Name";
        _styleGrid.ItemClickCallback = StyleSelectedClientCallback;
        _styleGrid.ListTemplatePath = ResolveUrl("~/Styles/jqtmpl/formStyleListTemplate.html");
        _styleGrid.ListItemTemplatePath = ResolveUrl("~/Styles/jqtmpl/formStyleListItemTemplate.html");
        _styleGrid.LoadDataCallback = "loadFormStyleListAjax";
        _styleGrid.EmptyGridText = WebTextManager.GetText("/pageText/Users/Manage.aspx/noStyles");
        _styleGrid.IsAjaxScrollModeEnabled = true;
        _styleGrid.InitialFilterField = "Name";
        _styleGrid.FilterItemType = "style";
        _styleGrid.RenderCompleteCallback = "gridRenderComplete";
    }

    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));
        
        RegisterClientScriptInclude(
            "serviceHelper.js",
            ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "svcStyleManagement.js",
            ResolveUrl("~/Services/js/svcStyleManagement.js"));
    }
</script>
