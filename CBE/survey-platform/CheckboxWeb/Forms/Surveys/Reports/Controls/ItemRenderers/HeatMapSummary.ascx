<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeatMapSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.HeatMapSummary" %>

<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/HeatMapGraph.ascx" TagPrefix="ckbx" TagName="HeatMapGraph" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:HeatMapGraph ID="_heatMapGraph" runat="server" />

</div>