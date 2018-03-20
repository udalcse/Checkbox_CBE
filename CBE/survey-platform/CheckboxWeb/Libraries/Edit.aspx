<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Libraries.Edit" MasterPageFile="~/LeftMenu.Master" %>
<%@ Register Src="Controls/LibraryEditor.ascx" TagName="LibraryEditor" TagPrefix="ckbx" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/LeftMenu.Master" %>

<asp:Content ID="_headerContent" runat="server" ContentPlaceHolderID="_head">
    <link rel="Stylesheet" type="text/css" href='<%=ResolveUrl("~/GlobalSurveyStyles.css") %>' />
    
    <!--[if lte IE 7]>
        <link rel="Stylesheet" type="text/css" href='<%=ResolveUrl("~/GlobalSurveyStyles_IE.css") %>' />
    <![endif]-->

</asp:Content>

<asp:Content ID="_menuContent" runat="server" ContentPlaceHolderID="_menuContent">
    
    <asp:Panel ID="_menuPanel" runat="server" CssClass="sidebarGreenMenuWrapper">
        <div class="sidebarGreenMenuHeader"><ckbx:MultiLanguageLabel ID="_menuTitle" runat="server" TextId="/common/actions"></ckbx:MultiLanguageLabel></div>
        <div class="sidebarGreenMenuContent">
            <ul class="sidebarGreenMenu">
                <li class="last"><ckbx:MultiLanguageLinkButton ID="_addItemBtn" runat="server" TextId="/pageText/libraries/edit.aspx/addItem" /></li>
            </ul>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="_mainContent" runat="server" ContentPlaceHolderID="_mainFrameContent">
    <div class="PageTitle" style="padding-bottom:5px;"><asp:Label id="_titleLbl" runat="server" /></div>
    <telerik:RadAjaxLoadingPanel ID="_loadingPanel" runat="server" Transparency="50" CssClass="AjaxLoadingPanel" InitialDelayTime="500" MinDisplayTime="1000"></telerik:RadAjaxLoadingPanel>
    <ckbx:LibraryEditor ID="_editor" runat="server" />
    <div style="width:700px;">
        <!-- Spacer div to ensure editor frame fills window, even when there is no content -->
    </div>
</asp:Content>
