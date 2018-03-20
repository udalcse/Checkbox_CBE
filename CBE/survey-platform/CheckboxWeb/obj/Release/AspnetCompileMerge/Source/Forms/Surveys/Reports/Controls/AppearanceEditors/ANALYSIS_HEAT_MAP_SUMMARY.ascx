<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ANALYSIS_HEAT_MAP_SUMMARY.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.ANALYSIS_HEAT_MAP_SUMMARY" %>

<div class="formInput">
    <p>
        <ckbx:MultiLanguageLabel AssociatedControlID="_font" ID="fontLbl" runat="server" Text="Label font" /></p>
    <ckbx:MultiLanguageDropDownList ID="_font" runat="server" />

    <p>
        <ckbx:MultiLanguageLabel ID="titleSizeLbl" AssociatedControlID="_titleSize" runat="server" Text="Label size" /></p>
    <ckbx:MultiLanguageDropDownList ID="_titleSize" runat="server" />

    <p>
        <ckbx:MultiLanguageLabel AssociatedControlID="_gridLine" ID="_gridLineLabel" runat="server" Text="Grid line" />
        <asp:CheckBox ID="_gridLine" runat="server" CssClass="leftMargin10" />
    </p>
    <p>
        <ckbx:MultiLanguageLabel AssociatedControlID="_respondentLabels" ID="_showValuesOnBarsLbl" runat="server" Text="Show respondent labels" />
        <asp:CheckBox ID="_respondentLabels" runat="server" CssClass="leftMargin10" />
    </p>
</div>
