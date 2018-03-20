<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RoleSelector.ascx.cs" Inherits="CheckboxWeb.Users.Controls.RoleSelector" %>

<asp:Panel id="_roleWarningPanel" runat="server" CssClass="warning message">
    <ckbx:MultiLanguageLabel ID="_roleWarning" runat="server" TextId="/pageText/users/controls/roleSelector.ascx/roleWarning" Text="The maximum number of Survey Administrators allowed by your license have been created. In order to assign the Survey Administrator or System Administrator role to a new user these roles will need to be revoked from an existing user."/>
</asp:Panel>
<asp:Panel ID="_normalRoleSelector" runat="server" Visible="true" CssClass="formInput">
    <asp:CheckBoxList ID="_roleList" runat="server"></asp:CheckBoxList>
</asp:Panel>
<asp:Panel ID="_simpleRoleSelector" runat="server" Visible="true" CssClass="formInput radioList">
    <ckbx:MultiLanguageRadioButtonList ID="_simpleRolesList" runat="server">
        <asp:ListItem TextId="/simpleSecurity/systemAdministrator" Value="1"></asp:ListItem>
        <asp:ListItem TextId="/simpleSecurity/surveyAdministrator" Value="2,3,4,5,6,7,8"></asp:ListItem>
        <asp:ListItem TextId="/simpleSecurity/respondent" Value="4,5" Selected="True"></asp:ListItem>
    </ckbx:MultiLanguageRadioButtonList>
</asp:Panel>