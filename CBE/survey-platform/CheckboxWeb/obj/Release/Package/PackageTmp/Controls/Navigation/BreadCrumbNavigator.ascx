<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BreadCrumbNavigator.ascx.cs" Inherits="CheckboxWeb.Controls.Navigation.BreadCrumbNavigator" %>

<asp:Panel runat="server" ID="_crumbPanel" CssClass="crumb-panel" style="display:none;">
    <ul class="breadcrumb-navagator left">
        <% var crumbs = BuildCrumbs(); %>
        <% foreach (var crumb in crumbs) { %>
            <% if (crumb.Key == crumbs.Last().Key) { %>
                <li><%= crumb.Value %></li>                
            <% } else { %>
                <li><a href="<%= crumb.Key %>"><%= crumb.Value %></a></li>
                <li><span class="arrow" />&nbsp</li>
            <% } %>
        <% } %>
    </ul>
</asp:Panel>

<script type="text/javascript">
    <% if(ShowPanel) { %>
        $(function () {
            $('.rightPanel .overview').css('top', 20);
            $('#<%= _crumbPanel.ClientID %>').show();
        });
    <% } %>
</script>