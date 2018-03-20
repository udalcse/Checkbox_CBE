<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LibraryList.ascx.cs" Inherits="CheckboxWeb.Libraries.Controls.LibraryList" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" assembly="CheckboxWeb"%>

<script type="text/javascript">
    $(document).ready(function(){
        //implementation all/none selection
        $(document).on('click', '.deleteLibrary', function(){
           toggleGridActionButtons('library', this);
        });
        $(document).on('click', '#_selectAllLibraries', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteLibrary').prop('checked', true);
            } else {
                $('.deleteLibrary').prop('checked', false);
            }
            $.uniform.update('.deleteLibrary');
            toggleGridActionButtons('library', this, actionsAvailable);
        });

        //Bind delete selected Libraries click
        $('#_deleteSelectedLibrariesLink').click(function(){
            if($('.deleteLibrary:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteSelectedLibrariesConfirm") %>', 
                    onDeleteSelectedLibrariesConfirm,
                    375,
                    200,
                    '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteSelectedLibrariesConfirmation") %>'
                );
            }
        });
    });

    //Confirm handler for deleting users
    function onDeleteSelectedLibrariesConfirm(){
        var idArray = new Array();

        $('.deleteLibrary:checked').each(function(index){
            idArray.push($(this).attr('value'));
        });

        if (idArray.length > 0){
            svcSurveyManagement.deleteLibraries(
                    _at,
                    idArray,
                    onLibrariesDeleted,
                    idArray.length                     
                );
        }
    }

    //User deleted handler
    function onLibrariesDeleted(resultData, count){
        resetGridMenu();
        reloadLibraryList(); 
        <%if (!String.IsNullOrEmpty(OnLibraryDeleted))
          {%>
            <%=OnLibraryDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/libraries/manage.aspx/librariesDeleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), resultData);
        <%
          }%>
    }

    <%-- Load library list --%>
    function loadItemLibraryList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcSurveyManagement.listItemLibraries(
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

    <%-- Load library list --%>
    function loadItemLibraryListAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        svcSurveyManagement.listItemLibraries(
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

    function gridRenderComplete() {
        <%=_libraryGrid.ShowSorter %>();
    }

    <%-- Update Existing Item Name --%>
    function updateLibraryRow(libraryData){
        if(libraryData == null){
            return;
        }

        $('#libraryName_' + libraryData.DatabaseId).html(libraryData.Name);
        $('#libraryDesc_' + libraryData.DatabaseId).html(libraryData.Description);
        $('#libraryItemCount_' + libraryData.DatabaseId).html(libraryData.ItemIds.length);

        //Update a data binded to the element
        $.tmplItem($('#libraryName_' + libraryData.DatabaseId).parent('ul')).data = libraryData;
    }

    //Reload library list handler
    function reloadLibraryList(){
        //reload grid and specify resetPaging=true parameter
        <%=_libraryGrid.ReloadGridHandler %>(true);
    }
</script>

<div class="gridMenu">
    <div class="gridButtons right itemActionMenu">
        <a class="ckbxButton roundedCorners border999 shadow999 redButton" href="#" id="_deleteSelectedLibrariesLink"><%=WebTextManager.GetText("/pageText/libraries/manage.aspx/deleteSelectedLibraries")%></a>
    </div>
    <br class="clear" />
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_libraryGrid" runat="server" GridCssClass="ckbxGrid" />

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
    public string OnLibraryDeleted { get; set; }

    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _libraryGrid.InitialSortField = "Name";
        _libraryGrid.ItemClickCallback = LibrarySelectedClientCallback;
        _libraryGrid.ListTemplatePath = ResolveUrl("~/Libraries/jqtmpl/libraryListTemplate.html");
        _libraryGrid.ListItemTemplatePath = ResolveUrl("~/Libraries/jqtmpl/libraryListItemTemplate.html");
        _libraryGrid.LoadDataCallback = "loadItemLibraryListAjax";
        _libraryGrid.EmptyGridText = WebTextManager.GetText("/pageText/libraries/availableLibraries.aspx/noLibrariesFound");
        _libraryGrid.IsAjaxScrollModeEnabled = true;
        _libraryGrid.InitialFilterField = "Name";
        _libraryGrid.FilterItemType = "library";
        _libraryGrid.RenderCompleteCallback = "gridRenderComplete";
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

