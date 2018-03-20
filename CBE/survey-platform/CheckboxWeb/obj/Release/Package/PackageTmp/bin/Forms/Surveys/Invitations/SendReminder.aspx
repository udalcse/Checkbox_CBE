<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="SendReminder.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.SendReminder" ValidateRequest="false" EnableEventValidation="false" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="Send" Src="~/Forms/Surveys/Invitations/Controls/Send.ascx" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" TagPrefix="ckbx" TagName="EditMessage" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingCssElement runat="server" Source="../../../ControlSkins/ACLEditor/Grid.ACLEditor.css" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/WizardHighlight.js" />
    
    <% if(IsReviewStep) { %>
    <script type="text/javascript">
        //change Finish button text
        $(function () {
            $('.WizardNavContainer [id$="_nextButton"]').text('<%= TextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/sendNowButton") %>');            
        });
    </script>
    <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_invitationWizard" runat="server" 
        DisplaySideBar="false" 
        DisplayCancelButton="true" 
        OnNextButtonClick="InvitationWizard_NextButtonClick"
        OnCancelButtonClick="InvitationWizard_CancelButtonClick"
        OnPreviousButtonClick="ChangeWizardStepToPrevious"
        OnFinishButtonClick="SendButton_Click">
        <WizardSteps>
            <asp:WizardStep ID="TextStep" runat="server" StepType="Start">
                <div class="padding10 overflowPanel_468">
                    <ckbx:EditMessage ID="_editMessage" runat="server" HideFromFields="true" />
                </div>
            </asp:WizardStep>
                
            <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                <div class="padding10">
                    <asp:Panel ID="_reviewPanel" runat="server" CssClass="overflowPanel_490">
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
                            <div class="left fixed_500 border999 padding5" style="height:165px;overflow:auto;background-color:#FFF;"><asp:Label ID="_messageReview" runat="server" CssClass="invitationBody"/></div>
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

                    <asp:Panel ID="_sendPanel" runat="server" Visible="false">
                        <div>&nbsp;</div>
                        <ckbx:Send ID="_invitationSender" runat="server" SendMode="remind" />
                    </asp:Panel>
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
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
    </asp:Wizard>                
</asp:Content>
