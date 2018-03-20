<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GovernancePrioritySummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.GovernancePrioritySummary" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/GovernancePrioritySummary.ascx" TagPrefix="ckbx" TagName="GovernancePrioritySummary" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>

<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:GovernancePrioritySummary ID="_governancePriorityGraph" runat="server" />
</div>

