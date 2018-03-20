<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Schedule.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Schedule" ValidateRequest="false" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/Schedule.ascx" tagname="Schedule" tagprefix="ckbx" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/dateUtils.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/moment.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/jquery.localize.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/statusControl.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/jquery-ui-timepicker-addon.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Services/js/serviceHelper.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Services/js/svcInvitationManagement.js" />

    <ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />
    
    <script language="javascript" type="text/javascript">
        function hidePanels() {
            $('#<%=_errorPanel.ClientID%>').hide('slow');
            $('#<%=_successPanel.ClientID%>').hide('slow');
        }

        $(document).ready(function () {
            /*
            $('#<%=_scheduledDate.ClientID%>').datetimepicker({
                numberOfMonths: 2
            });

            $('#<%=_scheduledDate.ClientID%>').datetimepicker("setDate", new Date().addHours(1));
            */

            setTimeout(hidePanels, 5000);
        });        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel ID="_errorPanel" runat="server" Visible="false">
        <div class="error message" runat="server" id="_msgDivError"></div>
    </asp:Panel>

    <asp:Panel ID="_successPanel" runat="server" Visible="false">
        <div class="success message" runat="server" id="_msgDivSuccess"></div>
    </asp:Panel>

    <div class="padding15">
        <ckbx:Schedule id="_scheduleView" runat="server" />    
    </div>
    <div class="padding15 left">
        <label>
            <%=WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/reminder")%>
            <ckbx:DateTimePicker ID="_scheduledDate" runat="server" NumberOfMonths="2"/>
        </label>
        <a class="ckbxButton roundedCorners border999 shadow999 silverButton statistics_InvitationSend" id="_addToSchedule" runat="server"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/schedule.aspx/addToSchedule")%></a>
    </div>
    <br class="clear"/>
 </asp:Content>
