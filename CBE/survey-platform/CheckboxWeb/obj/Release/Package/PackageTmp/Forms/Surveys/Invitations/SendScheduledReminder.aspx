<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="SendScheduledReminder.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.SendScheduledReminder" ValidateRequest="false" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" TagPrefix="ckbx" TagName="EditMessage" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/jquery-ui-timepicker-addon.js" />

    <ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />

    <script type="text/javascript">

        $(function () {
            initScheduleStep();
            checkDateVisibility();
        });
        
        function checkDateVisibility() {
            if ($('#<%=_schedule.ClientID%>').prop("checked"))
                $('#<%=_scheduledDate.ClientID%>').show();
            else
                $('#<%=_scheduledDate.ClientID%>').hide();
        }

        function initScheduleStep() {
            /*
            $('#<%=_scheduledDate.ClientID%>').datetimepicker({
                numberOfMonths: 2
            });

            $('#<%=_scheduledDate.ClientID%>').datetimepicker("setDate", new Date().addHours(2));*/

            $('#<%=_immediate.ClientID%>').change(function () { checkDateVisibility(); });
            $('#<%=_schedule.ClientID%>').change(function () { checkDateVisibility(); });
        }

        function closeAndRefreshInvitations() {
            closeWindow(window.top.onDialogClosed, { op: 'refreshInvitations' });
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_invitationWizard" runat="server" 
        DisplaySideBar="false" 
        DisplayCancelButton="true" 
        OnFinishButtonClick="InvitationWizard_FinishButtonClick"
        OnCancelButtonClick="InvitationWizard_CancelButtonClick"
        OnNextButtonClick = "InvitationWizard_NextButtonClick" >
        <WizardSteps>
            <asp:WizardStep ID="TextStep" runat="server" StepType="Start">
                <div class="padding10 overflowPanel_468">
                    <ckbx:EditMessage ID="_editMessage" runat="server" HideFromFields="true" />
                </div>
            </asp:WizardStep>            
            <asp:WizardStep ID="ReviewStep" runat="server" StepType="Step">
                <div class="padding10">
                    <asp:Panel ID="_reviewPanel" CssClass="overflowPanel_490" runat="server">
                        <div class="dialogSubTitle">
                            <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/reviewTitle" />
                        </div>
                        <div class="dialogInstructions">
                            <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/reviewInstructions" Text="Please review the information below to ensure it is correct"/>
                        </div>
                        <asp:Panel ID="_invitationWarningPanel" runat="server" Visible="false" CssClass="StatusPanel warning">
                            <ckbx:MultiLanguageLabel ID="_invitationWarning" runat="server" />
                        </asp:Panel>

                        <div class="centerContent fixed_600 dashStatsContentLarge">
                            <div class="left fixed_75 label"><ckbx:MultiLanguageLabel ID="_subjectReviewLabel" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/subjectReviewLabel" CssClass="invitationMessageReviewLabel" /></div>
                            <div class="left fixed_500 border999 padding5" style="background-color:#FFF;"><asp:Label ID="_subjectReview" runat="server" /></div>
                            <br class="clear" />
                            <div>&nbsp;</div>
                            <div class="left fixed_75 label"><ckbx:MultiLanguageLabel ID="_messageReviewLabel" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/messageReviewLabel" CssClass="invitationMessageReviewLabel" /></div>
                            <div class="left fixed_500 border999 padding5" style="height:205px;overflow:auto;background-color:#FFF;"><asp:Label ID="_messageReview" runat="server" CssClass="invitationBody"/></div>
                            <br class="clear" />
                        </div>

                        <asp:Panel id="_emailsNotEnoughWarningPanel" runat="server" CssClass="warning message" Visible="false">
                            <asp:Label ID="_emailsNotEnoughWarning" runat="server" />
                        </asp:Panel>

                        <div class="dashStatsContentHeader">
                            <ckbx:MultiLanguageLabel id="_recipientReviewTitle" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/recipientReviewTitle">Recipients</ckbx:MultiLanguageLabel>
                        </div>
                        <div class="dashStatsContentLarge">
                            <asp:Label ID="_recipientCountReview" runat="server"  />&nbsp;&nbsp;<ckbx:MultiLanguageHyperLink ID="_recipientReviewLink" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/recipientReviewLink" />
                        </div>
                    </asp:Panel>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="ScheduleStep" runat="server" StepType="Finish">                
                <div class="dialogSubTitle">
                    <ckbx:MultiLanguageLabel ID="_scheduleTitle" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/scheduleTitle" />
                </div>
                <asp:Panel ID="_invitationNotSentWarning" runat="server" Visible="false" CssClass="StatusPanel warning" style="font-size:14px;">
                    <ckbx:MultiLanguageLabel ID="_activationWarningLbl" runat="server" />
                </asp:Panel>
                <div class="padding10">
                    <div>
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_immediate" runat="server" Checked="true" GroupName="mode" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/immediateOption"/>
                        </div>
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_schedule" runat="server" GroupName="mode" TextID="/pageText/forms/surveys/invitations/sendReminder.aspx/scheduleOption" /> <ckbx:DateTimePicker ID="_scheduledDate" runat="server" style="display:none;" NumberOfMonths="2"/><asp:RequiredFieldValidator ControlToValidate="_scheduledDate" runat="server" Display="Dynamic" CssClass="error message" runat="server">*</asp:RequiredFieldValidator>
                        </div>                        
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_postponed" runat="server" Checked="false" GroupName="mode" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/postponedOption"/>
                        </div>
                    </div>
                </div>
            </asp:WizardStep>   

        </WizardSteps>
        <StartNavigationTemplate>
            <btn:WizardButtons ID="_startNavigationButtons" runat="server" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons ID="_finishNavigationButtons" runat="server" />
        </FinishNavigationTemplate>
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" />
        </HeaderTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
    </asp:Wizard>                
</asp:Content>
