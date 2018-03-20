<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Roles.aspx.cs" Inherits="CheckboxWeb.Users.Roles" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<%@ Register src="Controls/RoleSelector.ascx" tagname="RoleSelector" tagprefix="role" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<div class="padding10">
    <div class="dialogSubTitle">
        <ckbx:MultiLanguageLabel ID="_roleTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/roleTitle" />
    </div>
    <div class="dialogInstructions">
        <ckbx:MultiLanguageLabel ID="_roleInstructions" runat="server" CssClass="" TextId="/pageText/users/properties.aspx/roleInstructions" />
    </div>
    <asp:Panel ID="_readOnlyUserWarningPanel" runat="server" Visible="false">
        <div class="warning message">
            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/users/properties.aspx/readOnlyUserWarning" />
        </div>
    </asp:Panel>

    <status:statuscontrol id="_statusControl" runat="server" />
    <ckbx:MultiLanguageLabel ID="_roleRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/roleRequired" />
    <role:roleselector id="_roleSelector" runat="server" />
</div>
</asp:Content>
