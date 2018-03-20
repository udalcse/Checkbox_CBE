<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ExternalUsers.aspx.cs" Inherits="CheckboxWeb.Settings.ExternalUsers" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/externalUsers")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/externalUsers.aspx/externalUsersSectionTitle")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" TextId="/pageText/settings/externalUsers.aspx/dynamicSettings" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox runat="server" id="_allowExternalUsers" autopostback="true" TextId="/pageText/settings/externalUsers.aspx/allowExternalAuthentication" />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/externalUsers.aspx/options")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox runat="server" id="_requireRegistration" autopostback="true" TextId="/pageText/settings/externalUsers.aspx/requireRegistration" />
            </div>
            
            <div class="left input" style="display:none;">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" CssClass="PrezzaNormal" TextId="/pageText/settings/externalUsers.aspx/NTAuthenticationVariableName">External user variable name</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input" style="display:none;">
                <asp:TextBox runat="server" id="_authenticationVariableName" autopostback="true" CssClass="PrezzaNormal"></asp:TextBox>
            </div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader" >
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/externalUsers.aspx/defaultRoles")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <asp:Panel ID="_standardRolesPanel" runat="server">
                <div class="dialogInstructions">
                    <ckbx:MultiLanguageLabel id="_rolesLabel" runat="server" TextId="/pageText/settings/externalUsers.aspx/selectRoles">Select the roles to be assigned to network users that have not been registered in the system</ckbx:MultiLanguageLabel>
                </div>
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" TextId="/pageText/settings/externalUsers.aspx/availableRoles">Available Roles</ckbx:MultiLanguageLabel>
                </div>
                <div class="input">
                   <ckbx:MultiLanguageCheckBoxList ID="_roles" runat="server" RepeatColumns="1" RepeatDirection="Vertical" />  
                </div>
            </asp:Panel>
            <asp:Panel ID="_simpleRolesPanel" runat="server" Visible="false">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" TextId="/pageText/settings/externalUsers.aspx/availableRoles">Available Roles</ckbx:MultiLanguageLabel>
                </div>
                <div>
                    <ckbx:MultiLanguageRadioButtonList ID="_simpleRolesList" runat="server" CssClass="PrezzaNormal" >
                        <asp:ListItem Text="System Administrator" Value="System Administrator"></asp:ListItem>
                        <asp:ListItem Text="Survey Editor" Value="User Administrator,Survey Administrator,Respondent,Report Viewer,Report Administrator,Survey Editor,Group Administrator"></asp:ListItem>
                        <asp:ListItem Text="Respondent" Value="Respondent,Report Viewer" Selected="True"></asp:ListItem>
                    </ckbx:MultiLanguageRadioButtonList>
                </div>
            </asp:Panel>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
