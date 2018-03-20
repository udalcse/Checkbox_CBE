<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SettingsSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SettingsSummary" %>
<%@ Import Namespace="Checkbox.Web" %>

<div class="dialogSubTitle">
    <%=WebTextManager.GetText("/controlText/forms/surveys/settingsSummary.ascx/selectedOptions") %>
</div>

<!-- Permissions -->
<div class="summaryWrapper border999 shadow999">
    <div class="summaryHeader">
        <div><%=WebTextManager.GetText("/controlText/forms/surveys/settingsSummary.ascx/permissionsOptions") %></div>
    </div>

    <div class="zebra">
        <ckbx:MultiLanguageLabel ID="_permissionLbl" runat="server" CssClass="left fixed_175" TextId="/controlText/forms/surveys/settingsSummary.ascx/permission">Survey Permission: </ckbx:MultiLanguageLabel>
        <asp:Label ID="_selectedPermissionLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>
</div>
<div class="dialogInstructions">
    <asp:Label ID="_permissionDescriptionLbl" runat="server" />
</div>

<!-- Options -->
<div class="summaryWrapper border999 shadow999">
    <div class="summaryHeader">
        <div><%=WebTextManager.GetText("/controlText/forms/surveys/settingsSummary.ascx/behaviorOptions") %></div>
    </div>

    <div class="zebra">
        <ckbx:MultiLanguageLabel ID="_backButtonLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/showBackButton" />
        <asp:Label ID="_backButtonValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>

    <div class="detailZebra">
        <ckbx:MultiLanguageLabel ID="_allowEditLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/allowEdit" />
        <asp:Label ID="_allowEditValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>

    <div class="zebra">
        <ckbx:MultiLanguageLabel ID="_allowResumeLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/allowResume" />
        <asp:Label ID="_allowResumeValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>

    <div class="detailZebra">
        <ckbx:MultiLanguageLabel ID="_saveAndQuitLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/showSaveAndQuit" />
        <asp:Label ID="_saveAndQuitValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>
</div>

<!-- Response Limits -->
<div class="summaryWrapper border999 shadow999">
    <div class="summaryHeader">
        <div><%=WebTextManager.GetText("/controlText/forms/surveys/settingsSummary.ascx/responseLimits")%></div>
    </div>

    <div class="zebra">
        <ckbx:MultiLanguageLabel ID="_totalLimitLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/totalResponseLimit" />
        <asp:Label ID="_totalLimitValueLbl" runat="server"  CssClass="left" />
        <br class="clear" />
    </div>
        
    <div class="detailZebra">
        <ckbx:MultiLanguageLabel ID="_perRespondentLimitLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/respondentResponseLimit" />
        <asp:Label ID="_perRespondentLimitValueLbl" runat="server"  CssClass="left" />
        <br class="clear" />
    </div>
</div>

<!-- Time Limits -->
<div class="summaryWrapper border999 shadow999">
    <div class="summaryHeader">
        <div><%=WebTextManager.GetText("/controlText/forms/surveys/settingsSummary.ascx/timeLimits")%></div>
    </div>

    <div class="zebra">
        <ckbx:MultiLanguageLabel ID="_startDateLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/minAvailableDate" />
        <asp:Label ID="_startDateValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>

    <div class="detailZebra">
        <ckbx:MultiLanguageLabel ID="_endDateLbl" runat="server" CssClass="left fixed_300" TextId="/pageText/forms/surveys/controls/settingsSummary.ascx/maxAvailableDate" />
        <asp:Label ID="_endDateValueLbl" runat="server" CssClass="left" />
        <br class="clear" />
    </div>
</div>