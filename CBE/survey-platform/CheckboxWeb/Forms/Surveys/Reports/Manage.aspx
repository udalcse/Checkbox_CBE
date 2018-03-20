<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Manage" MasterPageFile="~/DetailList.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Register TagPrefix="ckbx" TagName="ReportList" Src="~/Forms/Surveys/Reports/Controls/ReportList.ascx" %> 

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_head">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/StatusControl.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/surveys/reports/manage.js" />

    <script type="text/javascript">
        var _selected = "<%=ReportID%>";
        //
        function onDashLinkClick(target, event) {
            if (target.attr('reportDashLink') == "delete") {
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/deletereportconfirmtext") %>',
                    function (res) {
                        if (res) {
                            deleteReportConfirmCallback(target.attr('reportId'));
                        }
                    },
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/deletereportconfirmcaption") %>'
                );
            }
            else
                showDialog(target.attr('reportDashLink'), target.attr('windowName'));
            event.stopPropagation();
        }

        function deleteReportConfirmCallback(id) {
            svcReportManagement.deleteReport(_at, id, onReportDeleted, [id]);
        }

        function onReportSelected(report) {
            $("#introTxt").hide();
            //first check for Analisys.Administer permission
            svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_REPORT, report.ReportId, 'Analysis.Administer')
                .then(function (hasAdministerPermission) {
                    report['allowAdminister'] = hasAdministerPermission;
                    //check for Analisys.Delete permission
                    svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_REPORT, report.ReportId, 'Analysis.Delete')
                        .then(function (hasDeletePermission) {
                            report['allowDelete'] = hasDeletePermission;
                        })
                        //then apply template
                        .then(function () {
                            templateHelper.loadAndApplyTemplate(
                                'reportInfoTemplate.html',
                                '<%=ResolveUrl("~/Forms/Surveys/Reports/jqtmpl/reportInfoTemplate.html") %>',
                                report,
                                { reportUrl: '<%=ApplicationManager.ApplicationPath + "/RunAnalysis.aspx?ag=" %>' + report.ReportGuid },
                                'reportContent',
                                true,
                                onReportLoaded);
                        })
                });
            return;
        }

        //Report loaded handler
        function onReportLoaded() {
            //Bind dash link clicks
            $('a[reportDashLink]').click(function (event) { onDashLinkClick($(this), event); });
        }
        
    </script>
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <asp:PlaceHolder ID="_titleButtonsPlace" runat="server">
        <% if(ResponseTemplateId >= 1000) { %>
            <a class="ckbxButton silverButton statistics_AddReport" href="javascript:showDialog('Add.aspx?s=<%=SurveyID%>&onClose=onDialogClosed', 'wizard');"><%=WebTextManager.GetText("/controlText/surveyDashboard/createNew")%></a>
            <a class="ckbxButton silverButton" href="Filters/Manage.aspx?s=<%=ResponseTemplateId%>&back=report"><%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/manageFilters")%></a>
        <% } %>
    </asp:PlaceHolder>
</asp:Content>

<asp:Content runat="server" ID="_left" ContentPlaceHolderID="_leftContent">
    <ckbx:ReportList Id="_reportList" runat="server" ReportSelectedClientCallback="onReportSelected"></ckbx:ReportList>
</asp:Content>

<asp:Content runat="server" ID="_right" ContentPlaceHolderID="_rightContent">
    
        <div class="padding10 dashboard" id="introTxt">
        <div class="introPage">
           
        </div>        
    </div>
    
    <div id="reportContent" class="padding10"></div>
</asp:Content>
