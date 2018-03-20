<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeatMapSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.HeatMapSummary" %>

<%@ Register Src="~/Controls/Charts/HeatMapGraph.ascx" TagPrefix="ckbx" TagName="HeatMapGraph" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>

<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:HeatMapGraph ID="_heatMapGraph" runat="server" />

</div>
