<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="SendToPending.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.SendToPending" %>
<%@ Register TagPrefix="ckbx" TagName="Send" Src="~/Forms/Surveys/Invitations/Controls/Send.ascx" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <asp:Panel ID="_reviewPanel" runat="server">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/reviewTitle" />
            </div>
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" CssClass="" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/reviewInstructions"/>
            </div>

            <div class="dashStatsContentLarge centerContent fixed_600">
                <div class="left fixed_75 label"><ckbx:MultiLanguageLabel ID="_subjectReviewLabel" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/subjectReviewLabel" CssClass="invitationMessageReviewLabel" /></div>
                <div class="left fixed_500 border999 padding5" style="background-color:#FFF;"><asp:Label ID="_subjectReview" runat="server" /></div>
                <br class="clear" />
                <div>&nbsp;</div>
                <div class="left fixed_75 label"><ckbx:MultiLanguageLabel ID="_messageReviewLabel" runat="server" TextId="/pageText/forms/surveys/invitations/sendReminder.aspx/messageReviewLabel" CssClass="invitationMessageReviewLabel" /></div>
                <div class="left fixed_500 border999 padding5" style="height:190px;overflow:auto;background-color:#FFF;"><asp:Label ID="_messageReview" runat="server" CssClass="invitationBody"/></div>
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

        <asp:Panel ID="_sendPanel" runat="server" Visible="false" style="margin-top:15px;">
            <div>&nbsp;</div>
            <ckbx:Send ID="_invitationSender" runat="server" SendMode="invite" />
        </asp:Panel>
    </div>
    <div class="WizardNavContainer">
        <hr />
        <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" OnClientClick="closeWindow();" TextID="/pageText/forms/surveys/invitations/add.aspx/closeButton" />
        <btn:CheckboxButton ID="_sendNowButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right statistics_InvitationSend" OnClick="SendButton_Click" TextID="/pageText/forms/surveys/invitations/add.aspx/sendNowButton" /> 
    </div>
</asp:Content>
