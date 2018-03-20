<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Forms.Manage" MasterPageFile="~/DetailList.Master" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Security" %>
<%@ Register Src="~/Forms/Controls/SurveyAndFolderList.ascx" TagName="SurveyList" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Controls/SurveyDashboard.ascx" TagName="SurveyDashboard" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Controls/Timeline.ascx" TagName="Timeline" TagPrefix="ckbx" %>
<%@ MasterType VirtualPath="~/DetailList.master" %>

<asp:Content ID="_headContent" runat="server" ContentPlaceHolderID="_head">
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/surveys/manage.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/StatusControl.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/jquery.ckbxtab.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/highcharts/highcharts.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/feature-tour.js" />
    
    <ckbx:ResolvingScriptElement ID="_productTour" runat="server" />

    <script type="text/javascript">
        <%-- Ensure statusControl initialized--%>
        var featureTour;
        $(document).ready(function () {
            <% if(ShowCreateSurveyDialogOnLoad) {%>
            var cookieName = 'hide-create-survey-dialog';
            var hide = readCookie(cookieName) != null;
            if (!hide) {
                    showDialog('Surveys/CreateQuick.aspx', 'smallwideproperties');
                    createCookie(cookieName, '1');
                }
            <%}%>

            statusControl.initialize('_statusPanel');
            
            <% if (ApplicationManager.AppSettings.EnableMultiDatabase &&
                   CurrentPrincipal != null &&
                   CurrentPrincipal.IsInRole("System Administrator") &&
                   ApplicationManager.AppSettings.DisplayProductTour) 
                { %>
                   var hideTour = readCookie('hide-feature-tour') != null;
                   if (!hideTour) {
                       launchProductTour();
                   }
            <% } %>
        });

        <%-- Run Product Tour 
        function launchProductTour() {
            if (typeof featureTour == 'undefined') {
                featureTour = new FeatureTour();
                featureTour.initFeatureTour();
            }
            featureTour.launchTour();
        }
        --%>
        <%-- Redirect to created invitation --%>
        function openInvitation(params) {
            window.location.href = '<%= ResolveUrl("~/Forms/Surveys/Invitations/Manage.aspx") %>?s=' + params.surveyId + '&i=' + params.invitationId;
        }

        <%-- Handle Survey Click --%>
        function onSurveyClick(arg) {
            if (typeof (event) != 'undefined' && event !=null && $(event.target).attr("class") == "edit-link")
            {
                return;
            }
            
            var surveyId = arg.attr('surveyId');

            if (surveyId == null || surveyId == '') {
                return;
            }

            $('.groupContent').removeClass('activeContent');
            $(arg).addClass('activeContent');

            <%-- Call load js method exposed by Dashboard control --%>
            loadSurveyData(surveyId);

            <%-- Reload timeline, filter the events by the survey ID --%>
            if (typeof(timeline) != 'undefined')
                timeline.setParentObject(surveyId, 'SurveyManager', 'SURVEY');

        }

        <%-- Handle Survey Deleted --%>
        function onSurveyDeleted(surveyId){
            <%-- Call load js method exposed by SurveyList control --%>
            clearAndReload();
            <%-- Call load js method exposed by Dashboard control --%>
            CleanDashboard();
        }

        <%-- Handle Survey Moved/Copied --%>
        function onSurveyMoved(){
            <%-- Call load js method exposed by SurveyList control --%>
            clearAndReload();
        }

        <%-- Show status message --%>
        function ShowStatusMessage(message, isSucceeded){
            statusControl.showStatusMessage(message, isSucceeded? StatusMessageType.success : StatusMessageType.error);
        }

    </script>
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <div class="surveys-manage-buttons">
        <%if (RoleManager.UserHasRoleWithPermission(User.Identity.Name, "FormFolder.FullControl"))
          {
         %>
            <a class="header-button ckbxButton blueButton" href="javascript:showDialog('Folders/Create.aspx', 'smallproperties');"><%=WebTextManager.GetText("/pageText/forms/manage.aspx/createFolder")%></a>
        <%} %>
        <%if (RoleManager.UserHasRoleWithPermission(User.Identity.Name, "Form.Create"))
          {
         %>
            <a id="surveymanager_addsurvey" class="header-button ckbxButton blueButton statistics_AddSurvey" href="javascript:showDialog('Surveys/Create.aspx', 'largeProperties');"><%=WebTextManager.GetText("/pageText/forms/manage.aspx/createSurvey+")%></a>
            <div id="surveymanager_addsurvey_menu" class="groupMenu" style="display: none;">              
                <ul class="allMenu">
                    <li><a class="ckbxButton blueButton" href="javascript:showDialog('Surveys/Import.aspx', 'mediumProperties');"><%=WebTextManager.GetText("/pageText/forms/manage.aspx/xmlImport")%></a></li>
                </ul>
            </div>
        <%} %>
    </div>
</asp:Content>

<asp:Content ID="_leftContent" runat="server" ContentPlaceHolderID="_leftContent">
    <ckbx:SurveyList ID="_surveyList" runat="server" OnSurveyClickHandler="onSurveyClick" ShowStatusMessageHandler="ShowStatusMessage" FolderId="1" />
</asp:Content>

<asp:Content ID="_rightContent" runat="server" ContentPlaceHolderID="_rightContent">
    <ckbx:SurveyDashboard ID="_dashboard" runat="server" OnSurveyDeleted="onSurveyDeleted" ShowStatusMessageHandler="ShowStatusMessage" OnSurveyMoved="onSurveyMoved" />      
    <ckbx:Timeline ID="_timeline" runat="server" Manager="SurveyManager" ShowGraph="true"/>      
</asp:Content>