<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FrequencyConfig.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.FrequencyConfig" %>
<%@ Import Namespace="Checkbox.Web"%>

<ul class="dashStatsContent allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/frequencyItemRenderer/otherAnswers") %></li>
    <li><%= WebTextManager.GetText("/enum/analysisOtherBehavior/" + AnalysisItem.Metadata["OtherOption"]) %></li>
</ul>

<div class="clear"></div>

