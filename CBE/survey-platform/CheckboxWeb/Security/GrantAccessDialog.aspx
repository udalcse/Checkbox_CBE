<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="GrantAccessDialog.aspx.cs" Inherits="CheckboxWeb.Security.GrantAccessDialog" %>
<%@ Register TagPrefix="security" Src="~/Controls/Security/GrantAccessControl.ascx" TagName="GrantAccess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingCssElement runat="server" Source="../ControlSkins/ACLEditor/Grid.ACLEditor.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel ID="_grantAccessWrapper" runat="server">
        <security:GrantAccess ID="_grantAccess" runat="server" />
    </asp:Panel>
    <div class="clear"></div>
    <div style="margin-right: auto; margin-left: auto; width: 150px; margin-top:10px;">   
        <btn:CheckboxButton ID="_closeButton" runat="server" TextID="/pageText/security/grantAccessDialog.aspx/closeButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="CloseButton_Click" />
    </div>
</asp:Content>