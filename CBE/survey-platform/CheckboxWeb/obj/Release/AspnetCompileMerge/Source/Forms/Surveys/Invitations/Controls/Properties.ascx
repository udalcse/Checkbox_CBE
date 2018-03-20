<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Properties.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.Properties" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<% if (!ApplicationManager.AppSettings.EnableMultiDatabase)
   {%>
    <div class="dialogSubTitle"><%= WebTextManager.GetText("/controlText/invitationDashboard.ascx/allowOptOut") %></div>
    <div class="padding10">
        <div class="input"><ckbx:MultiLanguageRadioButton ID="_optOutOn" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/optOutReviewOn" GroupName="OptOut" /></div>
        <div class="input"><ckbx:MultiLanguageRadioButton ID="_optOutOff" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/optOutReviewOff" GroupName="OptOut" /></div>
    </div>
<% } %>
<div class="dialogSubTitle"><%=WebTextManager.GetText("/controlText/invitationDashboard.ascx/loginOption")%></div>
<div class="padding10">
    <div class="input"><ckbx:MultiLanguageCheckBox ID="_autoLoginStatus" runat="server" TextId="/pageText/forms/surveys/invitations/properties.aspx/autoLogin" /></div>
</div>