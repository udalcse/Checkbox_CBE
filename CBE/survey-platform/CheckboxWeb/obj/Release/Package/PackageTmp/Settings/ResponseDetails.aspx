<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ResponseDetails.aspx.cs" Inherits="CheckboxWeb.Settings.ResponseDetails" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/responseDetails")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats"><%= WebTextManager.GetText("/pageText/settings/responseDetails.aspx/listOptions") %></span>
        </div>
        <div class="dashStatsContent">
            <div class="fixed_175 input">
                <ckbx:MultiLanguageCheckBox id="_displayIncompleteResponses" runat="server" TextId="/pageText/settings/responseDetails.aspx/displayIncompleteResponses" />
            </div>
            <div class="spacing">&nbsp;</div>
            <div class="fixed_100 left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_resultsPerPage" TextId="/pageText/settings/responseDetails.aspx/resultsPerPage">Results per page:</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:TextBox id="_resultsPerPage" runat="server" class="settingsNumericInput_4" MaxLength="4"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator ID="_resultsPerPageFormatValidator" runat="server" ControlToValidate="_resultsPerPage" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$" CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/responseDetails.aspx/resultsPerPage/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="_resultsPerPageRequiredValidator" runat="server" ControlToValidate="_resultsPerPage" Display="Dynamic" CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/responseDetails.aspx/resultsPerPage/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
        </div>
    </div>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/responseDetails.aspx/detailsOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayResponseDetails" runat="server" TextId="/pageText/settings/responseDetails.aspx/displayResponseDetails" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayDetailedUserInfo" runat="server" TextId="/pageText/settings/responseDetails.aspx/displayDetailedUserInfo" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayQuestionNumbers"  runat="server" TextId="/pageText/settings/responseDetails.aspx/displayQuestionNumbers" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayUnansweredQuestions" runat="server" TextId="/pageText/settings/responseDetails.aspx/displayUnansweredQuestions" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayRankOrderPoints" runat="server" TextId="/pageText/settings/responseDetails.aspx/displayRankOrderPoints" />
            </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
