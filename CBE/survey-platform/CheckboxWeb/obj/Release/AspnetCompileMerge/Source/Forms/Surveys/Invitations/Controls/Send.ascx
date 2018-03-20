<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Send.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.Send" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Messaging.Email" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery-ui-timepicker-addon.js" />
<ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />

<script type="text/javascript">
    $(document).ready(function () {
        initSchedule();
    });

    /************** PROGRESS *************************/
    var progressStatusSuccess = false;

    //Start progress monitoring
    function startProgress(progressKey, mode) {
        //Start send process
        $.ajax({
            type: "GET",
            url: '<%=ResolveUrl("~/Forms/Surveys/Invitations/Worker.aspx")%>',
            async: true,
            contentType: "application/json; charset=utf-8",
            data: { mode: mode, i: '<%=InvitationId %>', s: '<%=ScheduleId%>' },
            dataType: "json",
            timeout: 100
        });         //Set a short timeout to work around issues where load balancers and the like
        // may timeout after 60 seconds, etc.  This essentially makes the call fire and
        // forget instead of waiting for error/success error to return.

        //Start monitoring progress
        ajaxProgress.startProgress(
            progressKey,
            'progressDiv',
            '<%=ResolveUrl("~/") %>',
            onProgressUpdate,
            onProgressError,
            onProgressComplete);


    }

    //Show message on error
    function onProgressError(errorMessage) {
        $('#progressDiv').hide();
        $('#progressText').addClass('error');
        $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
    }

    //Update status
    function onProgressUpdate(progressData) {
        $('#progressDiv').show();
        $('#progressText').html(progressData.StatusMessage);
    }

    //Do something on progress complete
    function onProgressComplete(progressData) {
        $('#<%= _returnDiv.ClientID %>').show();
        $('#progressText').hide();
        $('#progressDiv').hide();
        $('#progressTitle').hide();
    }

    function checkDateVisibility() {
        if ($('#<%=_schedule.ClientID%>').attr("checked") == "checked")
            $('#<%=_scheduledDate.ClientID%>').show();
        else
            $('#<%=_scheduledDate.ClientID%>').hide();
    }

    function initSchedule() {
        $('#<%=_immediate.ClientID%>').attr("checked", "checked");
        
        /*
        $('#<%=_scheduledDate.ClientID%>').datetimepicker({
            numberOfMonths: 2
        });
        $('#<%=_scheduledDate.ClientID%>').datetimepicker("setDate", new Date().addHours(1));
        */

        $('#<%=_immediate.ClientID%>').change(function () { checkDateVisibility(); });
        $('#<%=_schedule.ClientID%>').change(function () { checkDateVisibility(); });
    }
 </script>

<asp:Panel ID="_noRecipientsPanel" runat="server">
    <div class="warning message"><%=WebTextManager.GetText("/controlText/send.ascx/noRecipients")%></div>
</asp:Panel>

<asp:Panel ID="_errorPanel" runat="server" Visible="false">
    <div class="error message" runat="server" id="_msgDivError"></div>
</asp:Panel>

<asp:Panel ID="_successPanel" runat="server" Visible="false">
    <div class="success message" runat="server" id="_msgDivSuccess"></div>
</asp:Panel>

<asp:Panel ID="_recipientsPanel" runat="server">
    <% if (EmailGateway.ProviderSupportsBatches && !AutoSend)
       {  %>
    <div class="clear"></div>
    <div>
        <div class="padding10">
            <ckbx:MultiLanguageRadioButton ID="_immediate" runat="server" GroupName="sendMode" TextId="/pageText/forms/surveys/invitations/add.aspx/immediateOption"/>
        </div>
        <div class="padding10">
            <ckbx:MultiLanguageRadioButton ID="_schedule" runat="server" GroupName="sendMode" TextID="/pageText/forms/surveys/invitations/add.aspx/scheduleOption" /> 
        </div>                        
        <div class="padding10">
            <ckbx:DateTimePicker ID="_scheduledDate" runat="server" style="display:none;" NumberOfMonths="2"/>
        </div>                        
    </div>
    <%} %>

    <div class="clear"></div>

    <div style="margin-top:25px;">
        <asp:Panel ID="_remindPanel" runat="server">
            <btn:CheckboxButton ID="_remindBtn" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" runat="server" TextID="/controlText/send.ascx/sendReminder" />
            <asp:Panel id="_emailsNotEnoughToRemindWarningPanel" runat="server" CssClass="warning message" Visible="false">
                <asp:Label ID="_emailsNotEnoughToRemindWarning" runat="server" /><br />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_invitePanel" runat="server">
            <btn:CheckboxButton ID="_inviteBtn" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" runat="server" TextID="/controlText/send.ascx/sendInvitation" />
			<asp:Panel id="_emailsNotEnoughToInviteWarningPanel" runat="server" CssClass="warning message" Visible="false">
                <asp:Label ID="_emailsNotEnoughToInviteWarning" runat="server" /><br />
            </asp:Panel>
        </asp:Panel>
    </div>

    <div class="clear"></div>
     
     <asp:Panel ID="_sendPanel" runat="server" Visible="false">
         <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader" style="padding:5px;">
                <span class="mainStats left"><%=WebTextManager.GetText("/pageText/invitations/batchsendsummary.aspx/title")%></span>
            </div>
            <div class="padding10">
                <div  id="progressTitle">
                                <ckbx:MultiLanguageLabel ID="_importingLbl" runat="server" CssClass="label" TextId="/pageText/batchSendSummary.aspx/queuingMessage" />                
                </div>                
                <br class="clear" />
                <div id="progressDiv"></div>
                <div id="progressText"></div>
                <asp:Panel runat="server" id="_returnDiv" style="margin-top:25px;display:none;">
                    <ckbx:MultiLanguageLabel ID="_toReturnLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/forms/surveys/invitations/progress.aspx/queueSuccess" />
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
   <asp:Panel runat="server" ID="_closeWizardContainer" CssClass="WizardNavContainer">
      <%--commented due to uselessness of this button--%>
      <%--<hr />
      <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" OnClientClick="closeWindow();" TextID="/pageText/forms/surveys/invitations/add.aspx/closeButton" />--%>
    </asp:Panel>
</asp:Panel>

