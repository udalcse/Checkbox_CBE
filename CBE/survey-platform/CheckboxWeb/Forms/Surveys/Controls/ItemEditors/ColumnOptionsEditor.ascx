<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ColumnOptionsEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.ColumnOptionsEditor" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_columnWidthTextBox.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>


<table>
    <tr class="Item">
        <td>
            <ckbx:MultiLanguageLabel ID="_uniqueAnswerLabel" runat="server" TextId="/pageText/matrixItemEditor.aspx/uniqueAnswers"></ckbx:MultiLanguageLabel>
        </td>
        <td colspan='2'>
            <ckbx:MultiLanguageDropDownList ID="_uniqueAnswerDropDownList" runat="server">
            </ckbx:MultiLanguageDropDownList>
        </td>
    </tr>
    <tr class="AlternatingItem">
        <td>
            <ckbx:MultiLanguageLabel ID="_columnWidthLabel" runat="server" TextId="/controlText/matrixSetHeaderHeader/columnWidth"></ckbx:MultiLanguageLabel>
        </td>
        <td>
            <asp:TextBox ID="_columnWidthTextBox" runat="server" />
        </td>
        <td>
            <ckbx:MultiLanguageLabel ID="_pxLabel" runat="server" TextId="/pageText/matrixItemEditor.aspx/px" />
        </td>
    </tr>
</table>
