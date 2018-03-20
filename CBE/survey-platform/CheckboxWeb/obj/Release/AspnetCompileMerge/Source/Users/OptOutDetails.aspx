<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="OptOutDetails.aspx.cs" Inherits="CheckboxWeb.Users.OptOutDetails" MasterPageFile="~/Dialog.master" %>
<%@ Import Namespace="Checkbox.Globalization" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="opted-out-details-row">
        <span class="opted-out-details-header left fixed_100"><%= TextManager.GetText("/pageText/Users/optOutDetails.aspx/email") %></span>
        <span class="opted-out-details-text left" ><%= Email %></span>
    </div>
    <div class="opted-out-details-row">
        <span  class="opted-out-details-header left fixed_100"><%= TextManager.GetText("/pageText/Users/optOutDetails.aspx/survey") %></span>
        <span  class="opted-out-details-text left" ><%= SurveyName %></span>
    </div>
    <div class="opted-out-details-row">
        <span  class="opted-out-details-header left fixed_100"><%= TextManager.GetText("/pageText/Users/optOutDetails.aspx/date") %></span>
        <span  class="opted-out-details-text left" ><%= GlobalizationManager.FormatTheDate(Date) %></span>
    </div>
    <div>
        <div class="opted-out-details-header left fixed_100"><%= TextManager.GetText("/pageText/Users/optOutDetails.aspx/comment") %></div>
        <div class="opted-out-details-text left" style="overflow-y: auto; max-height: 90px;" ><%= UserComment %></div>
    </div>
</asp:Content>