<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseSelect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls.ResponseSelect" %>

<div class="wrapperMaster">
    <div class="center borderRadius surveyContentFrame surveyDialogFrame">
        <div class="innerSurveyContentFrame">
            <asp:PlaceHolder ID="_startNewTopPlaceholder" runat="server"></asp:PlaceHolder>

            <asp:Panel ID="_resumePanel" runat="server" />
            <asp:Panel ID="_editPanel" runat="server" />

            <asp:PlaceHolder ID="_startNewBottomPlaceholder" runat="server"></asp:PlaceHolder>
        </div>
    </div>
</div>
