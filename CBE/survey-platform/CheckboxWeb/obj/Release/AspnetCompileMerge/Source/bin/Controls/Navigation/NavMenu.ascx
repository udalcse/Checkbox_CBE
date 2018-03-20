<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NavMenu.ascx.cs" Inherits="CheckboxWeb.Controls.Navigation.NavMenu" %>
<ul class="headerMenu allMenu left" role="navigation">
    <asp:Repeater ID="_menuRepeater" runat="server" DataSourceID="_siteMapSource">        
        <ItemTemplate>
            <li><a class="<%# IsSectionActive((SiteMapNode)Container.DataItem) ? "selected" : "" %>" href="<%#((SiteMapNode)Container.DataItem).Url %>"><%#((SiteMapNode)Container.DataItem).Title %></a></li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
<asp:SiteMapDataSource ID="_siteMapSource" SiteMapProvider="navMenuSiteMap" runat="server" ShowStartingNode="false" StartFromCurrentNode="false" EnableViewState="false" />