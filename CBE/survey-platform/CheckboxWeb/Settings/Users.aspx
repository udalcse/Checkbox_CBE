<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Users.aspx.cs" Inherits="CheckboxWeb.Settings.Users" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/users")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/users.aspx/registration")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_allowUsersToEditOwnInfo" runat="server" TextId="/pageText/settings/users.aspx/allowUsersToEditOwnInfo" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_allowUsersToResetPassword" runat="server" TextId="/pageText/settings/users.aspx/allowUsersToResetPassword" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_allowPublicRegistration" runat="server" autopostback="true" TextId="/pageText/settings/users.aspx/allowPublicRegistration" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_requireEmailAddress" runat="server" TextId="/pageText/settings/users.aspx/requireEmailAddress" />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/users.aspx/defaultRoles")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel id="_rolesLabel" runat="Server" TextId="/pageText/settings/users.aspx/selectRoles">Select the roles to be assigned to publically registered users</ckbx:MultiLanguageLabel>
            </div>
            <div class="dashStatsContentHeader">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/settings/users.aspx/availableRoles">Available Roles:</ckbx:MultiLanguageLabel>
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBoxList ID="_roles" runat="server" RepeatColumns="1" RepeatDirection="Vertical" /> 
            </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
