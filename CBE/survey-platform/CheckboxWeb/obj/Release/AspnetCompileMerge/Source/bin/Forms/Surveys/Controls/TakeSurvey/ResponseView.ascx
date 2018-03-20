<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseView.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.ResponseView" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/LanguageSelect.ascx" TagPrefix="ckbx" TagName="LanguageSelect" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/MobileControls/LanguageSelect.ascx" TagPrefix="ckbx" TagName="LanguageSelectMobile" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/ResponseSelect.ascx" TagPrefix="ckbx" TagName="ResponseSelect" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/MobileControls/ResponseSelect.ascx" TagPrefix="ckbx" TagName="ResponseSelectMobile" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/EnterPassword.ascx" TagPrefix="ckbx" TagName="EnterPassword" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/MobileControls/EnterPassword.ascx" TagPrefix="ckbx" TagName="EnterPasswordMobile" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/ProgressSaved.ascx" TagPrefix="ckbx" TagName="ProgressSaved" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/MobileControls/ProgressSaved.ascx" TagPrefix="ckbx" TagName="ProgressSavedMobile" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/PageView.ascx" TagPrefix="ckbx" TagName="PageView" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/Login.ascx" TagPrefix="ckbx" TagName="Login" %>
<%@ Register Src="~/Forms/Surveys/Controls/TakeSurvey/MobileControls/Login.ascx" TagPrefix="ckbx" TagName="LoginMobile" %>

<%-- Error Control --%>
<asp:Panel CssClass="errror" Visible="false" runat="server" ID="_errorPanel">
    <div style="margin:15px;">
        <div style="float:left;">
            <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/images/Stop.png") %>" alt="Error" title="An error occurred." />
        </div>
        <div style="float:left;margin-left:10px;">
            <div style="color:red;font-weight:bold;"><asp:Literal ID="_errorMessage" runat="server" /></div>
            <div><asp:Literal ID="_errorSubMessage" runat="server"/></div>
            <div style="font-size:10px;">
                <asp:Literal ID="_moreInfoMessage" runat="server" />
            </div>
        </div>
        <div style="clear:both;"></div>
    </div>
</asp:Panel>

<%-- Survey Language Select --%>
<ckbx:LanguageSelect ID="_languageSelect" Visible="False" runat="server" />

<%-- Survey Language Select --%>
<ckbx:LanguageSelectMobile ID="_languageSelectMobile" Visible="False" runat="server" />

<%-- Survey Response Select --%>
<ckbx:ResponseSelect ID="_responseSelect" Visible="False" runat="server" />

<%-- Survey Response Select --%>
<ckbx:ResponseSelectMobile ID="_responseSelectMobile" Visible="False" runat="server" />

<%-- Enter Password --%>
<ckbx:EnterPassword ID="_enterPassword" Visible="False" runat="server" />

<%-- Enter Password --%>
<ckbx:EnterPasswordMobile ID="_enterPasswordMobile" Visible="False" runat="server" />

<%-- Login to Checkbox  --%>
<ckbx:Login ID="_login" Visible="False" runat="server" />

<%-- Login to Checkbox  --%>
<ckbx:LoginMobile ID="_loginMobile" Visible="False" runat="server" />

<%-- Progress Saved --%>
<ckbx:ProgressSaved ID="_progressSaved" Visible="False" runat="server" />

<%-- Progress Saved --%>
<ckbx:ProgressSavedMobile ID="_progressSavedMobile" Visible="False" runat="server" />

<%-- Page View --%>
<ckbx:PageView ID="_pageView" runat="server" />

<%-- Survey Not Active --%>
<asp:Panel ID="_notActivePanel" runat="server">
    <asp:Label ID="_notActiveMsg"  runat="server" CssClass="Question" />
</asp:Panel>

<%-- Survey Deleted --%>
<asp:Panel ID="_surveyDeletedPanel" runat="server">
    <asp:Label ID="_surveyDeletedMsg"  runat="server" CssClass="Question" />
</asp:Panel>

<%-- Survey Not Active, Start Date In Future --%>
<asp:Panel ID="_beforeStartDatePanel" runat="server">
    <asp:Label ID="_beforeStartDateMsg" runat="server" CssClass="Question" />
</asp:Panel>

<%-- Survey Not Active, End Date In Past --%>
<asp:Panel ID="_afterEndDatePanel" runat="server">
    <asp:Label ID="_afterEndDateMsg" runat="server" CssClass="Question" />
</asp:Panel>

<%-- No More Reposnses Allowed --%>
<asp:Panel ID="_noResponsesPanel" runat="server">
    <asp:Label ID="_noResponsesMsg" runat="server" CssClass="Question" />
</asp:Panel>

<%-- Required Invitation Not Found --%>
<asp:Panel ID="_noInvitationPanel" runat="server">
    <asp:Label ID="_noInvitationMsg" runat="server" CssClass="Question" />
</asp:Panel>