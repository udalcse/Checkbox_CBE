<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EditItem.aspx.cs" Inherits="CheckboxWeb.Libraries.EditItem" MasterPageFile="~/Dialog.Master" ValidateRequest="false" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Libraries/Controls/LibraryItemEditor.ascx" TagName="ItemEditor" TagPrefix="ckbx" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/jquery-ui-timepicker-addon.js" />

    <script type="text/javascript">
        var _choiceTabIsOld = false;
    </script>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:ItemEditor ID="_itemEditor" runat="server" />
</asp:Content>