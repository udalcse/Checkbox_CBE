<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserStoreSelect.ascx.cs" Inherits="CheckboxWeb.Users.Controls.UserStoreSelect" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>

<% if(MembershipProviderManager.IsChainingProviderDefault && MembershipProviderManager.Providers.Count > 2) { %>
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/users/general.js" />

    <asp:Panel runat="server" ID="_userStoreSelectPanel" CssClass="userStoreSelect clearfix">
    <% if (ShowLabel) { %>
        <label>Viewing:</label>
    <% } %>
        <select class="membership-provider-select" name="store">
            <% foreach (var provider in MembershipProviderManager.ProvidersInfo) { %> 
                <option value="<%= provider.Name %>"><%= provider.Title %></option>
            <% } %>
        </select>
        <div class="custom-select-container">
            <a class="custom-select-value" href="#">Checkbox Users</a>
            <div class="groupMenu custom-select" data-select-name="store">
                <ul>
                    <% foreach (var provider in MembershipProviderManager.ProvidersInfo) { %> 
                        <li><a class="ckbxButton" href="#" data-type="<%= provider.Type.Name %>" data-select-value="<%= provider.Name %>"><%= provider.Title %></a></li>
                    <% } %>
                </ul>
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        $(function () {
            $('#<%= _userStoreSelectPanel.ClientID %> a.ckbxButton').click(function () {
                $(this).trigger("membershipProviderChanged", ['<%= EventID %>', $(this).attr('data-select-value'), $(this).attr('data-type')]);
            });
        });
    </script>
<% } %>
