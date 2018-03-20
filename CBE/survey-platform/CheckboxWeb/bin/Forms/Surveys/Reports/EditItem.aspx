<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="EditItem.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.EditItem" MasterPageFile="~/Dialog.Master" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/ReportItemEditor.ascx" TagName="ItemEditor" TagPrefix="ckbx" %>


<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript">
        function onItemAdded(itemId, pageId) {
                var args = { op: 'addItem', result: 'ok', itemId: itemId, pageId: pageId };
                executeStringCallback('templateEditor.onItemAdded', args);
        }

        function okClick(itemId, pageId, isNew) {
            var args;

            if (isNew) {
                args = { op: 'addItem', result: 'ok', itemId: itemId, pageId: pageId };
                closeWindow('templateEditor.onItemAdded', args);
            }
            else {
                args = { op: 'editItem', result: 'ok', itemId: itemId, pageId: pageId };
                closeWindow('templateEditor.onItemEdited', args);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:ItemEditor ID="_itemEditor" runat="server" />
</asp:Content>
