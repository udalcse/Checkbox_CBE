<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ManagerNavigation.ascx.cs" Inherits="CheckboxWeb.Users.Controls.ManagerNavigation" %>
<div class="UserNav">
    <ul>
        <li ID="_userManagerTab" runat="server" class="first selected"><asp:HyperLink ID="_userLink" runat="server" NavigateUrl="~/Users/Manage.aspx" Text="Users" /></li>
        <li ID="_groupManagerTab" runat="server"><asp:HyperLink ID="_groupLink" runat="server" NavigateUrl="~/Users/Groups/Manage.aspx" Text="Groups" /></li>
        <li ID="_emailListManagerTab" runat="server"><asp:HyperLink ID="_emailListLink" runat="server" NavigateUrl="~/Users/EmailLists/Manage.aspx" Text="Email Lists" /></li>
    </ul>
</div>