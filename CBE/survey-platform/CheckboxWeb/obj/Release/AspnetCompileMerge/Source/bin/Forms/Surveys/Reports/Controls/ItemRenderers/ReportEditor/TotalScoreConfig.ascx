<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TotalScoreConfig.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.TotalScoreConfig" %>
<%@ Import Namespace="Checkbox.Web"%>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/frequencyItemRenderer/graphType") %></li>
    <li><%= WebTextManager.GetText("/enum/graphType/" + AnalysisItem.Metadata["GraphType"]) %></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>
