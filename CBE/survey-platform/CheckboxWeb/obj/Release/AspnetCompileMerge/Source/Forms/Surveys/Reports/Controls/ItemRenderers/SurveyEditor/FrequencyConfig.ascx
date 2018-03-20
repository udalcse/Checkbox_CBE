<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FrequencyConfig.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SurveyEditor.FrequencyConfig" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="field_200">
    <%= WebTextManager.GetText("/controlText/frequencyItemRenderer/otherAnswers") %>
</div>

<div class="input">
    <%= WebTextManager.GetText("/enum/analysisOtherBehavior/" + AnalysisItem.Metadata["OtherOption"]) %>
</div>

<div class="clear"></div>

