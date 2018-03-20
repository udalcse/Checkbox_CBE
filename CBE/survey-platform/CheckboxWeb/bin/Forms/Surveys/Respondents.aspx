<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Respondents.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Respondents" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>
<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
    <security:GrantAccess ID="_grantSurveyAccess" runat="server" DelayLoad="False" />    
</asp:Content>
