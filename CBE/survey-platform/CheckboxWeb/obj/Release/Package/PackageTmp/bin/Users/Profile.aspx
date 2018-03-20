<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false"  CodeBehind="Profile.aspx.cs" Inherits="CheckboxWeb.Users.Profile" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/ProfilePropertyEditor.ascx" TagName="ProfileEditor" TagPrefix="prf" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<div class="padding10">
    <div class="dialogSubTitle">
        <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/profileTitle" />
    </div>
    <div class="dialogInstructions">
        <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/properties.aspx/profileInstructions" />
    </div>
    <asp:Panel ID="_readOnlyUserWarningPanel" runat="server" Visible="false">
        <div class="warning message">
            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/users/properties.aspx/readOnlyUserWarning" />
        </div>
    </asp:Panel>
    <status:statuscontrol id="_statusControl" runat="server" />
    <div class="dialogSubContainer">
        <prf:ProfileEditor ID="_profileEditor" runat="server" />
    </div>
</div>
</asp:Content>
