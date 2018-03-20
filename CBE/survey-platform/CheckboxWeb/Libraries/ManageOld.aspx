<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ManageOld.aspx.cs" Inherits="CheckboxWeb.Libraries.ManageOld" MasterPageFile="~/LeftMenu.Master" %>
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
        <li><a id="_createLibrary" href='<%= "Create.aspx" %>'><%= Checkbox.Web.WebTextManager.GetText("/pageText/libraries/manage.aspx/createLibrary") %></a></li>
        <li class="last"><a id="_importLibrary" href="Import.aspx"><%= Checkbox.Web.WebTextManager.GetText("/pageText/libraries/manage.aspx/importLibrary") %></a></li>
      </ul>
    </div>
  </asp:Panel>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_mainFrameContent" >
    <telerik:RadWindowManager ID="_windowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <telerik:RadWindow ID="_newLibraryWindow" runat="server" OpenerElementID="_createLibrary" NavigateUrl="Create.aspx" SkinID="CreateSurveyDialog" />
            <telerik:RadWindow ID="_importWindow" runat="server" OpenerElementID="_importLibrary" NavigateUrl="Import.aspx" SkinID="CreateSurveyDialog" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadAjaxLoadingPanel ID="_loadingPanel" runat="server" Transparency="5" >
        <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' style="border:0;" />
    </telerik:RadAjaxLoadingPanel>
    
    <telerik:RadAjaxManagerProxy ID="_radAjaxManager" runat="server">
         <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="_librariesGrid">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="_librariesGrid" LoadingPanelID="_loadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="_statusControl" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <!-- Currently it would require too much work to allow searching for library names and since searching didn't
         exist in 4.6 we will implement this when we have more time in a later release -->
    <!-- <filter:SearchControl ID="_searchControl" runat="server" OnSearch="SearchControl_Search" OnClearSearch="SearchControl_ClearSearch">
        <SearchFields>
            <asp:ListItem Value="Name" TextId="/pageText/libraries/filter/name" />
            <asp:ListItem Value="TemplateID" TextId="/pageText/libraries/filter/id" />
        </SearchFields>
    </filter:SearchControl> -->
    <status:StatusControl ID="_statusControl" runat="server" />
    <telerik:RadGrid ID="_librariesGrid" runat="server"
            AllowMultiRowSelection="true"
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
                <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="15px" />
                <telerik:GridBoundColumn UniqueName="ID" DataField="ID" Visible="false"></telerik:GridBoundColumn>
                <telerik:GridHyperLinkColumn DataTextField="Name" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="Edit.aspx?id={0}" HeaderText="Library Name" />
                <telerik:GridBoundColumn DataField="Description" HeaderText="Description" />
                <telerik:GridBoundColumn DataField="ItemsCount" HeaderText="(#) Items" />
            </Columns>
            <CommandItemTemplate>
                <div style="padding:3px;">
                    <btn:CheckboxButton ID="_btnDeleteSelected" runat="server" CommandName="DeleteSelected" TextID="/common/delete" CssClass="SubmitButton" />
                    <btn:CheckboxButton ID="_btnCopySelected" runat="server" CommandName="CopySelected" TextID="/common/copy" CssClass="SubmitButton" />
                    <btn:CheckboxButton ID="_btnExportSelected" runat="server" CommandName="ExportSelected" TextID="/common/export" CssClass="SubmitButton" />
            </CommandItemTemplate>
            <PagerStyle Mode="NextPrevNumericAndAdvanced"  AlwaysVisible="true" PageButtonCount="5" ShowPagerText="true" />
         </MasterTableView>
    </telerik:RadGrid>
        
</asp:Content>
