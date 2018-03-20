<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportWizardOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportWizardOptions" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ResponseProperties.ascx" TagName="ResponseProperties" TagPrefix="ckbx" %>

<div class="formInput">
    <div class="left fixed_20 checkBox">
        <asp:CheckBox ID="_useAliasesChk" runat="server" />
    </div>
    <div class="left">
        <p><ckbx:MultiLanguageLabel ID="_useAliasLbl" runat="server" AssociatedControlID="_useAliasesChk" TextId="/pageText/ReportWizardOptions.ascx/useAliasLbl" /></p>
    </div>
    <br class="clear"/>
</div>



<div class="formInput">
    <div class="left fixed_20 checkBox">
        <asp:CheckBox ID="multiPageCkbx" runat="server" />
    </div>
    <div class="left">
        <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="multiPageCkbx" TextId="/pageText/ReportWizardOptions.ascx/multiPageLbl" /></p>
    </div>
    <br class="clear"/>
</div>

<div class="formInput">
    <div class="left fixed_20 checkBox">
        <asp:CheckBox ID="_displayStatisticsForSummaryCharts" runat="server" />
    </div>
    <div class="left">
        <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" AssociatedControlID="_displayStatisticsForSummaryCharts" TextId="/pageText/ReportWizardOptions.ascx/displayStatisticsForSummaryCharts" /></p>
    </div>
    <br class="clear"/>
</div>

<div class="formInput">
    <div class="left fixed_20 checkBox">
        <asp:CheckBox ID="_displayAnswersForSummaryCharts" runat="server" />
    </div>
    <div class="left">
        <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" AssociatedControlID="_displayAnswersForSummaryCharts" TextId="/pageText/ReportWizardOptions.ascx/displayAnswersForSummaryCharts" /></p>
    </div>
    <br class="clear"/>
</div>

<!--
<div class="formInput">
    <p>
        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/ReportWizardOptions.ascx/itemPositionLbl" />
    </p>
    <ckbx:MultiLanguageDropDownList ID="itemPositionList" runat="server"></ckbx:MultiLanguageDropDownList>
</div> -->

<div class="formInput">
    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/pageText/ReportWizardOptions.ascx/maxOptionsLbl" AssociatedControlID="maxOptionsTxtBox" /></p>
    <ckbx:MultiLanguageTextBox runat="server" ID="maxOptionsTxtBox" Width="50" MaxLength="3"></ckbx:MultiLanguageTextBox>
    <div>
        <ckbx:MultiLanguageLabel ID="maxOptionsErrLbl" TextId="/pageText/ReportWizardOptions.ascx/maxOptionsErrLbl" runat="server" Visible="false">This field must contain a positive number</ckbx:MultiLanguageLabel>
    </div>
</div>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel runat="server" TextId="/pageText/ReportWizardOptions.ascx/responseSettings" AssociatedControlID="_responseProperties"  /></p>
    <ckbx:ResponseProperties ID="_responseProperties" runat="server"/>
</div>

<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $("#<%=maxOptionsTxtBox.ClientID%>").numeric();
    });
</script>