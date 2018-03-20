<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="GroupMemberships.aspx.cs" Inherits="CheckboxWeb.Users.GroupMemberships" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<%@ Register src="Controls/GroupSelector.ascx" tagname="GroupSelector" tagprefix="grp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<div class="padding10">
    <div class="dialogSubTitle">
        <ckbx:MultiLanguageLabel ID="_groupTitle" runat="server" TextId="/pageText/users/properties.aspx/groupTitle" Text="User groups" />
    </div>
    <div class="dialogInstructions">
        <ckbx:MultiLanguageLabel ID="_groupInstructions" runat="server" TextId="/pageText/users/properties.aspx/groupInstructions" Text="Enter the user profile properties.  All fields are optional." />
    </div>
    <asp:Panel ID="_readOnlyUserWarningPanel" runat="server" Visible="false">
        <div class="warning message">
            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/users/properties.aspx/readOnlyUserWarning" />
        </div>
    </asp:Panel>

    <status:statuscontrol id="_statusControl" runat="server" />
    <ckbx:MultiLanguageLabel ID="_groupRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/groupRequired" />
    <grp:groupselector id="_groupSelector" runat="server" />
</div>
</asp:Content>
