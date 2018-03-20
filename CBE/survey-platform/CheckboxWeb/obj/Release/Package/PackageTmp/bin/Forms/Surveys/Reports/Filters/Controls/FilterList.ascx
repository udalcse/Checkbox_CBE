<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FilterList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Filters.Controls.FilterList" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" TagName="Sorter" Src="~/Controls/Sorter.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" assembly="CheckboxWeb"%>

<script type="text/javascript">
    $(document).ready(function(){
        //implementation all/none selection
        $(document).on('click', '.deleteFilter', function(event) {
            toggleGridActionButtons('filter', this);
            event.stopPropagation();
        });

        $(document).on('click', '#_selectAllFilters', function(){
            if ($(this).prop('checked'))
            {                
                $('.deleteFilter').prop('checked', true);
                $('.deleteFilter').attr('checked', 'checked');
            }
            else
            {
                $('.deleteFilter').prop('checked', false);
                $('.deleteFilter').attr('checked', null);
            }
            $.uniform.update('.deleteFilter');
            toggleGridActionButtons('filter', this);
        });

        //Bind delete selected users click
        $('#_deleteSelectedFiltersLink').click(function(){
            if($('.deleteFilter:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/deleteSelectedFilterConfirmText") %>', 
                    onDeleteSelectedFiltersConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/deleteSelectedFilterConfirmTitle") %>'
                );
            }
        });
    });

    //Confirm handler for deleting users
    function onDeleteSelectedFiltersConfirm(args){
        if(args.success){
            var idArray = new Array();

            $('.deleteFilter:checked').each(function(index){
                idArray.push($(this).attr('value'));
            });

            if (idArray.length > 0){
                svcReportManagement.deleteFilters(
                     _at,
                     <%=ResponseTemplateId%>,
                     idArray,
                     onFiltersDeleted,
                     idArray.length                     
                 );
            }
        }
    }

    //User deleted handler
    function onFiltersDeleted(resultData, count){
        reloadFilterList();
        $('#introTxt').html('');
        <%if (!String.IsNullOrEmpty(OnFilterDeleted))
          {%>
            <%=OnFilterDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/deleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), true);
        <%
          }%> 
    }

    <%-- Load filter list --%>
    function loadItemFilterList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcReportManagement.listReportFiltersForSurvey(
            _at, 
            <%=ResponseTemplateId%>,
            '',
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

    <%-- Update Existing Item Name --%>
    function updateFilterRow(filterData){
        if(filterData == null){
            return;
        }

        $('#filterName_' + filterData.FilterId).html(filterData.Name);
        $('#filterDesc_' + filterData.FilterId).html(filterData.Description);
        $('#filterItemCount_' + filterData.FilterId).html(filterData.ItemIds.length);

        //Update a data binded to the element
        $.tmplItem($('#filterName_' + filterData.FilterId).parent('ul')).data = filterData;
    }

    //Reload filter list handler
    function reloadFilterList(){
        <%=_filterGrid.ReloadGridHandler %>();
    }
</script>

<div class="gridMenu">
    <div class="left gridSorter">
        <sort:Sorter ID="_sorter" runat="server">
            <sort:SortOption SortField="ID" TextId="/pageText/forms/surveys/reports/filters/manage.aspx/id" />
            <sort:SortOption SortField="Type" TextId="/pageText/forms/surveys/reports/filters/manage.aspx/type" />
            <sort:SortOption SortField="Filter" TextId="/pageText/forms/surveys/reports/filters/manage.aspx/filter" />
        </sort:Sorter>
    </div>
    <div class="right gridButtons">
        <ul class="itemActionMenu allMenu">
            <li><a class="ckbxButton roundedCorners shadow999 border999 redButton" href="#" id="_deleteSelectedFiltersLink"><%=WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/deleteSelected")%></a></li>
        </ul>
    </div>
    <br class="clear" />
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_filterGrid" runat="server" GridCssClass="ckbxGrid" />

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
    public string OnFilterDeleted { get; set; }

    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _filterGrid.InitialSortField = "ID";
        _filterGrid.ItemClickCallback = FilterSelectedClientCallback;
        _filterGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Reports/Filters/jqtmpl/filterListTemplate.html");
        _filterGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Reports/Filters/jqtmpl/filterListItemTemplate.html");
        _filterGrid.LoadDataCallback = "loadItemFilterList";
        _filterGrid.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/noFiltersFound");
        
        _sorter.Initialize(_filterGrid);
        
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
           "svcReportManagement.js",
           ResolveUrl("~/Services/js/svcReportManagement.js"));
        
        RegisterClientScriptInclude(
            GetType(),
            "UFrame.js",
            ResolveUrl("~/Resources/UFrame.js"));
            
        RegisterClientScriptInclude(
            GetType(),
            "htmlParser.js",
            ResolveUrl("~/Resources/htmlParser.js"));

        RegisterClientScriptInclude(
            GetType(),
            "StatusControl.js",
            ResolveUrl("~/Resources/statusControl.js"));
    }
</script>
