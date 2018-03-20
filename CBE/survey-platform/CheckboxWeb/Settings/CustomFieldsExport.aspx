<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="CustomFieldsExport.aspx.cs" Inherits="CheckboxWeb.Settings.CustomFieldsExport" %>

<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="margin: 0 10px 0 10px">
        <h3 style="padding-top: 10px"><%= WebTextManager.GetText("/pageText/settings/customFieldsExport.aspx/title")%></h3>
        <div class="warning message" runat="server" id="StatusMessage" visible="false"></div>
      
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader"><span class="mainStats left">&nbsp;</span></div>
            <div class="dashStatsContent">
                <asp:Repeater ID="_profileFieldRepeater" runat="server" OnItemDataBound="PropertyList_ItemDataBound">
                    <HeaderTemplate>
                        <table style="width: 100%" cellspacing="0" rules="all" border="1">
                            <tr>
                                <th scope="col" style="width: 50%; text-align: left">Property name</th>
                                <th scope="col" style="width: 50%; text-align: left">Field type</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="_propertyCheckBox" AutoPostBack="True" runat="server" Text='<%# Eval("Key") %>' OnCheckedChanged="PropertyCheckBox_Click" /></td>
                            <td>
                                <asp:Label ID="_propertyType" runat="server" Text='<%# Eval("Value") %>' /></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <asp:Button ID="_exportProperties" runat="server" Text="Export" class="header-button ckbxButton silverButton saveButton right" />
    </div>
</asp:Content>

