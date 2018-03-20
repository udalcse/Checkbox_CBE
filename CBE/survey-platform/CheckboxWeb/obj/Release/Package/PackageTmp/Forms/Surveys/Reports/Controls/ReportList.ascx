<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" TagName="Sorter" Src="~/Controls/Sorter.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" Assembly="CheckboxWeb" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
  <%-- Ensure service initialized --%>
    $(document).ready(function() {
        //implementation all/none selection
        $(document).on('click', '.deleteReport', function(event){
            toggleGridActionButtons('report', this);
            event.stopPropagation();
        });
        $(document).on('click', '#_selectAllReports', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteReport').prop('checked', true);
            } else {
                $('.deleteReport').prop('checked', false);
            }
            $.uniform.update('.deleteReport');
            toggleGridActionButtons('report', this, actionsAvailable);
        });


        //Bind delete link click
        $('#_deleteSelectedLink').click(function(){
            var count = $('.deleteReport:checked').length;
            if(count > 0){
                var warning = '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/deleteSelectedReportsConfirm") %>';
                showConfirmDialogWithCallback(
                    warning.replace("{0}", count), 
                    onDeleteSelectedConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/deleteSelected") %>'
                );
            }
        });

        statusControl.initialize('_statusPanel');
    });

    var selectedSurveyID = <%=SurveyId  %>;
    var searchTerm = "<%=SearchTerm %>";
    var _period = <%=Period%>;
    var _dateFieldName = "<%=DateFieldName%>";
     
    <%-- Load survey response list --%>
    function loadReportList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        if (selectedSurveyID > 0)
        {
            svcReportManagement.listReportsForSurvey(
                _at, 
                selectedSurveyID, 
                '',
                 {
                    pageNumber: currentPage,
                    resultsPerPage: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    filterField: '',
                    filterValue: '',
                    sortField: sortField,
                    sortAscending: sortAscending
                }, 
                loadCompleteCallback,
                loadCompleteArgs
            );
        }
        else if (searchTerm.length > 0)
        {
            svcReportManagement.listAvailableReports(
                _at,                 
                 {
                    pageNumber: currentPage,
                    pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    filterField: '',
                    filterValue: searchTerm,
                    sortField: sortField,
                    sortAscending: sortAscending,
                    period: _period, 
                    dateFieldName: _dateFieldName
                }, 
                loadCompleteCallback,
                loadCompleteArgs
            );
        }
        else
        {
            svcReportManagement.listReportsByPeriod(
                _at,                 
                 {
                    pageNumber: currentPage,
                    resultsPerPage: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    filterField: '',
                    filterValue: '',
                    sortField: sortField,
                    sortAscending: sortAscending,
                    period: _period, 
                    dateFieldName: _dateFieldName
                }, 
                loadCompleteCallback,
                loadCompleteArgs
            );
        }       
    }

    //
    function onDeleteSelectedConfirm(args){
        <% if (AllowDelete) {%>
        if(args.success){
            var idArray = new Array();

            $('.deleteReport:checked').each(function(index){
                idArray.push($(this).attr('value'));
            });

            if(idArray.length > 0)
            {
                for (var i=0;i<idArray.length; i++)
                {
                    var id = idArray[i];
                    svcReportManagement.deleteReport(_at, id , onReportDeleted, [id]);
                }
            }
        }
        <%} %>
    }

    function onReportDeleted() {
        reloadList();
        $('.simplemodal-close').click();
    }

    var reportIDToSelect = 0;
    
    //Handle dialog close and reload report dashboard
    function onDialogClosed(arg){
        if(arg == null){
            return;
        }
        
        if (arg.op=="addReport"){
            if (typeof(arg.url) != 'undefined' && arg.url != null){
                <%--Redirect to new page--%>
                location = arg.url;
                return;
            }
            else
            {
                reportIDToSelect = arg.reportId;
                <%--Reload dash--%>
                reloadList();            
            }
        }
        if (arg.op=="properties"){
            reportIDToSelect = arg.reportId;
            reloadList();            
        }
        else if (arg.op == "refresh")
        {
            <%--Reload dash--%>
            reloadList();            
        }
    }
    
    //
    function reloadList()
    {
        <%=_reportList.ReloadGridHandler %>();
    }

    <%-- Show status message --%>
    function ShowStatusMessage(message, isSucceeded){
        statusControl.showStatusMessage(message, isSucceeded ? StatusMessageType.success : StatusMessageType.error);
    }

    <%-- Report list was loaded --%>
    function onReportGridRendered()
    {
        //protect permissions
        if($('.deleteReport:visible').length == 0) {
            $('#_selectAllReports').hide();
        }

        <%if (Request.Params["reportID"] != null) {%>
        reportIDToSelect = <%=Request.Params["reportID"]%>;
        $('[reportId="' + reportIDToSelect + '"]').click();
        return;
        <% }%>
        if (reportIDToSelect > 0)
        {
            var report = $.tmplItem($('[reportId="' + reportIDToSelect + '"]')).data;
            onReportSelected(report);
        }
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_reportList.ShowSorter %>();
    }
</script>

<div class="gridMenu">

    <% if (AllowDelete) {  %>

    <div class="gridButtons itemActionMenu" style="float: right; margin: 0 5px 0 0;">
        
            <a class="cancelButton right" style="text-decoration:underline" href="#" id="_deleteSelectedLink">
                <%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/deleteSelected")%></a>
    
    </div>
    <%} %>
    <br class="clear" />
</div>

<ckbx:Grid ID="_reportList" runat="server" GridCssClass="ckbxGrid reportList" RenderCompleteCallback="onReportGridRendered"/>

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Override OnInit to set up grid and sorter
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _reportList.InitialSortField = "AnalysisName";
        _reportList.ItemClickCallback = ReportSelectedClientCallback;
        _reportList.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Reports/jqtmpl/reportListTemplate.html");
        _reportList.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Reports/jqtmpl/reportListItemTemplate.html");
        _reportList.LoadDataCallback = "loadReportList";
        _reportList.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/noReportsFound");
        _reportList.RenderCompleteCallback = "gridRenderComplete";
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
         "templateHelper.js",
         ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
           "svcReportManagement.js",
           ResolveUrl("~/Services/js/svcReportManagement.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
          "grid.js",
          ResolveUrl("~/Resources/grid.js"));

        RegisterClientScriptInclude(
         "jquery.pager.js",
         ResolveUrl("~/Resources/jquery.pager.js"));
    }
</script>