<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="TestSmtp.aspx.cs" Inherits="CheckboxWeb.Install.TestSmtp" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register Src="~/Install/Controls/TestSMTP.ascx" TagName="TestSMTP" TagPrefix="ckbx" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <ckbx:TestSMTP id="_TestSMTP" runat="server"/>
</asp:Content>
