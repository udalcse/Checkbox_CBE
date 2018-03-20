<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LanguageSelect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls.LanguageSelect" %>

<div class="wrapperMaster center borderRadius surveyContentFrame surveyDialogFrame">
    <div style="padding:5px;margin:5px;">
        <div style="margin-bottom:10px;" class="Question">
            <asp:Label AssociatedControlID="_surveyLanguageList" ID="_selectLanguageLbl" runat="server" />
        </div>
        <div>
            <asp:DropDownList ID="_surveyLanguageList" runat="server" CssClass="workflowAjaxPostAction" AutoPostBack="false" />
        </div>
        <div style="margin-top:5px;">
            <asp:Button ID="_selectLanguageBtn" runat="server" CssClass="workflowAjaxPostAction" Style="color:black;font-size:12px;"/>                       
        </div>
        <div style="clear:both;"></div>
    </div>
</div>

