<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="true" CodeBehind="GroupsUserProfile.aspx.cs" Inherits="CheckboxWeb.Users.GroupsUserProfile" %>

<%@ Register TagPrefix="status" TagName="statuscontrol" Src="~/Controls/Status/StatusControl.ascx" %>
<%@ Register Src="../Users/Controls/CredentialsEditor.ascx" TagName="CredentialsEditor" TagPrefix="editor" %>
<%@ Register TagPrefix="prf" TagName="ProfileEditor" Src="~/Users/Controls/ProfilePropertyEditor.ascx" %>
<%@ Register TagPrefix="role" TagName="roleselector" Src="~/Users/Controls/RoleSelector.ascx" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>


<asp:Content ID="_groupsProfileContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div id="groupsPropertyEditor">
        <status:statuscontrol id="_statusControl" runat="server" />
        <div class="padding10">
            <editor:CredentialsEditor ID="_credentialEditor" runat="server"></editor:CredentialsEditor>
        </div>
        <div class="padding10">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/profileTitle" />
            </div>
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/properties.aspx/profileInstructions" />
            </div>
            <asp:Panel ID="Panel1" runat="server" Visible="false">
                <div class="warning message">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/pageText/users/properties.aspx/readOnlyUserWarning" />
                </div>
            </asp:Panel>
            <status:statuscontrol id="Statuscontrol2" runat="server" />
            <div class="dialogSubContainer">
                <prf:ProfileEditor ID="_profileEditor" runat="server" />
            </div>
        </div>

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

            <ckbx:MultiLanguageLabel ID="_roleRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/roleRequired" />
            <role:roleselector id="_roleSelector" runat="server" />
        </div>
    </div>
</asp:Content>
