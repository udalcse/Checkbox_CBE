<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatrixGrid.ascx.cs" Inherits="CheckboxWeb.Users.Controls.MatrixGrid" %>

<asp:GridView runat="server" ID="_matrixField" OnRowDataBound="OnRowDataBound" AutoGenerateColumns="False" ></asp:GridView>
<asp:Table runat="server" ID="_matrixTable" CssClass="add-matrix-row-table" border="1" CellPadding="0" CellSpacing="0" Style="border-collapse: collapse" Visible="false">
    <asp:TableRow runat="server" ID="_actionTableRow"></asp:TableRow>
</asp:Table>
<asp:Panel runat="server" ID="_columnControls">
    <asp:Button runat="server" CssClass="remove-column-btn ckbxButton silverButton" ID="_deleteColumn" Text="Remove" OnClientClick="return false" Visible="False" />
    <asp:Button runat="server" CssClass="add-column-btn ckbxButton silverButton" ID="_addColumnBtn" Text="Add" OnClientClick="return false" Visible="False" />
</asp:Panel>
<asp:Panel runat="server" CssClass="grid-structure-panel">
    <asp:TextBox runat="server" ID="_rowsCountTxt"></asp:TextBox>
    <asp:TextBox runat="server" ID="_columnsCountTxt"></asp:TextBox>
</asp:Panel>

