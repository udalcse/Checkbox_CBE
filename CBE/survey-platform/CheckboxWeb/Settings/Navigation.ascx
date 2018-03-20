<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Navigation.ascx.cs" Inherits="CheckboxWeb.Settings.Navigation" %>
<%@ Import Namespace="Checkbox.Web"%>
<div class="grid_3 alpha">
    <asp:Panel ID="_menuPanel" runat="server" CssClass="sidebarBlueMenuWrapper">
        <div class="sidebarBlueMenuHeader"><ckbx:MultiLanguageLabel ID="_menuTitle" runat="server" TextId="/pageText/settings/navigation.ascx/sections"></ckbx:MultiLanguageLabel></div>
        <div class="sidebarBlueMenuContent">
            <ul class="sidebarBlueMenu">
                <asp:Repeater runat="server" ID="_items">
                    <ItemTemplate>
                        <%# Container.DataItem %>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <br />
        <br />
    </asp:Panel>

-    <asp:Panel ID="_backPanel" runat="server" CssClass="sidebarBlueMenuWrapper">
        <div class="sidebarBlueMenuHeader"><ckbx:MultiLanguageLabel ID="_backTitle" runat="server" TextId="/pageText/settings/navigation.ascx/navigation"></ckbx:MultiLanguageLabel></div>
        <div class="sidebarBlueMenuContent">
            <ul class="sidebarBlueMenu">
                <li  class="last"><a href="./Manage.aspx"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/back")%></a></li>
            </ul>
        </div>
    </asp:Panel>
</div>
