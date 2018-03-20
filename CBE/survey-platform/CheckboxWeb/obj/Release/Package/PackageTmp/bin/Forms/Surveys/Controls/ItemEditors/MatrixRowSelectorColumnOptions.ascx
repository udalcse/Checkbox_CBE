<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRowSelectorColumnOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRowSelectorColumnOptions" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_columnWidthTextBox.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>


<ckbx:MultiLanguageLabel ID="_columnWidthLabel" runat="server" TextId="/controlText/matrixSetHeaderHeader/columnWidth" CssClass="itemEditorLabel_100" />        
<asp:TextBox ID="_columnWidthTextBox" runat="server" />
<ckbx:MultiLanguageLabel ID="_pxLabel" runat="server" TextId="/pageText/matrixItemEditor.aspx/px" />
    