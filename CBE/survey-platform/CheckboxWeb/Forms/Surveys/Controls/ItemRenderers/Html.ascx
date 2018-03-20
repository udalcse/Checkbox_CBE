<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Html.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Html" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms" %>

<div class="itemContent textContainer HtmlItemControl" style="width: 100%;">
    <%= Utilities.ReplaceHtmlAttributes(Model.InstanceData["Html"], RenderMode == RenderMode.SurveyEditor).Replace("<iframe", "<div class='iframe'").Replace("</iframe>", "</div>")%>
</div>
    