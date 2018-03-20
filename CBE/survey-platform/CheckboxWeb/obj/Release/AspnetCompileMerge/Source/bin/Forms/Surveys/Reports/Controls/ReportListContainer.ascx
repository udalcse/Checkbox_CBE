<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportListContainer.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportListContainer" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ReportList.ascx" TagName="ReportList" TagPrefix="ckbx" %>
        <div id="detailContentContainer" class="padding10">
            <div id="detailWelcome" class="introPage" <%if (!string.IsNullOrEmpty(Request.Params["s"])) { %>style="display:none" <%} %>>
                <h3>Welcome to the Checkbox Report Manager!</h3>
                <p>
                    To add reports to your survey, select the survey in the survey navigator to your left. This panel will display all available reports for the survey and button for creating
                    a new report.
                </p>
                <p>
                    To edit a report please click on it in the report list.
                </p>
                <p>
                    To delete report(s) please mark the corresponding checkbox and push the Delete Report(s) button.
                </p>
                <p>
                    To add filters for the report just click on the Manage Filters button and create neccesary filters. Then apply filters to the report in the Report Editor.
                </p>
                <p>
                    For more help on getting started, check out our online <a target="_blank" href="http://www.checkbox.com/resources/training_videos.aspx">Training Videos</a>.
                </p>
            </div>
        </div>
        <div id="reportListButtons"  <%if (string.IsNullOrEmpty(Request.Params["s"])) { %>style="display:none" <%} %>>
            <div>
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:onAddReportClick();"><%=WebTextManager.GetText("/controlText/surveyDashboard/createNew")%></a>
                <a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:onManageFiltersClick();"><%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/manageFilters")%></a>
            </div>
            <div style="width: 650px;">
                <ckbx:ReportList ID="_reportList" runat="server"/>
            </div>
        </div>

