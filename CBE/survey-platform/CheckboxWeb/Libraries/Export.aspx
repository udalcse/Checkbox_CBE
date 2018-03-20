<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Export.aspx.cs" Inherits="CheckboxWeb.Libraries.Export" MasterPageFile="~/LeftMenu.Master" %>
<%@ Register Src="../Controls/Search/SearchControl.ascx" TagPrefix="filter" TagName="SearchControl" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ MasterType TypeName="Checkbox.Web.Page.SimpleBaseMasterPage" %>

<asp:Content ContentPlaceHolderID="_head" runat="server" ID="header">
    <link rel="Stylesheet" type="text/css" href="../ControlSkins/Checkbox/Grid.Checkbox.css" />
</asp:Content>

<asp:Content ID="_menu" runat="server" ContentPlaceHolderID="_menuContent" >
  <asp:Panel id="_menuPanel" runat="server" CssClass="sidebarGreenMenuWrapper" >
    <div class="sidebarGreenMenuHeader"><ckbx:MultiLanguageLabel ID="_menuHeader" runat="server" TextId="/common/actions"></ckbx:MultiLanguageLabel></div>
    <div class="sidebarGreenMenuContent">
      <ul class="sidebarGreenMenu">
        <li><a id="_returnLink" href='<%= "Manage.aspx" %>'><%= Checkbox.Web.WebTextManager.GetText("/common/cancel") %></a></li>
      </ul>
    </div>
  </asp:Panel>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_mainFrameContent" >
    <status:StatusControl ID="_statusControl" runat="server" />
    <telerik:RadGrid ID="_librariesGrid" runat="server"
            AllowMultiRowSelection="false"
            AllowSorting="true"
            OnSortCommand="_librariesGrid_SortCommand"
            AutoGenerateColumns="false"
            ShowHeader="true"
            PageSize="25" 
            ShowStatusBar="false" 
            AllowPaging="true" 
            AllowCustomPaging="true" 
            PagerStyle-AlwaysVisible="true"
            OnNeedDataSource="_librariesGrid_NeedDataSource"
            OnItemCommand="_librariesGrid_ItemCommand"
            Skin="Checkbox"
            EnableEmbeddedBaseStylesheet="false"
            EnableEmbeddedSkins="false" >
         <ClientSettings AllowKeyboardNavigation="true">
            <Selecting AllowRowSelect="true" />
            <KeyboardNavigationSettings FocusKey="F" />
         </ClientSettings>
         <MasterTableView CommandItemDisplay="TopAndBottom" AllowNaturalSort="false" >
            <SortExpressions><telerik:GridSortExpression FieldName="Name" SortOrder="Ascending" /></SortExpressions>
            <Columns>
                <%--<telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="15px" />--%>
                <telerik:GridBoundColumn UniqueName="ID" DataField="ID" Visible="false" />
                <telerik:GridBoundColumn UniqueName="Name" DataField="Name" HeaderText="Library Name" />
                <telerik:GridHyperLinkColumn DataTextField="Description" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="DownloadXml.aspx?id={0}" HeaderText="Export XML" />
                <telerik:GridBoundColumn DataField="ItemsCount" HeaderText="(#) Items" />
            </Columns>
            <CommandItemTemplate>
                <div style="padding:3px;">
                    <btn:CheckboxButton ID="_btnCancelExport" runat="server" CommandName="CancelExport" OnClientClick="history.back(-1);" TextID="/common/cancel" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" />
            </CommandItemTemplate>
            <PagerStyle Mode="NextPrevNumericAndAdvanced"  AlwaysVisible="true" PageButtonCount="5" ShowPagerText="true" />
         </MasterTableView>
    </telerik:RadGrid>
</asp:Content>