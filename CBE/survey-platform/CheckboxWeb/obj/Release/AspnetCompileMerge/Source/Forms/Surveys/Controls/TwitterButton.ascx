<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TwitterButton.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TwitterButton" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="System.Web.UI"%>

<a href="https://twitter.com/intent/tweet?url=<%=Server.HtmlEncode(SurveyURL)%>&text=<%=Server.HtmlEncode(Text)%>" target="_blank" class="twitterSmall roundedCorners statistics_TwitterShare">
    Tweet
</a>
