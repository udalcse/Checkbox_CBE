<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRowSelectorBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRowSelectorBehavior" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=_selectionType.ClientID %>").change(function () {
            var selectedValue = $(this).find("option:selected").val();
            if (selectedValue == "Single") {
                $("#<%= _singleSelectionTypePanel.ClientID %>").show();
                $("#<%= _multipleSelectionTypePanel.ClientID %>").hide();
            } else {
                $("#<%= _singleSelectionTypePanel.ClientID %>").hide();
                $("#<%= _multipleSelectionTypePanel.ClientID %>").show();
            }
        });

        $('#<%=_minToSelectTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_maxToSelectTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<!-- Alias -->
<ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/controlText/matrixRowSelectorEditor/alias" CssClass="itemEditorLabel_100" />
<asp:TextBox ID="_aliasText" runat="server" />
<br />

<div  style="margin:10px 0;">
<ckbx:MultiLanguageLabel ID="_selectionTypeLbl" runat="server" TextId="/controlText/matrixRowSelectorEditor/selectionType" CssClass="itemEditorLabel_100" />
<ckbx:MultiLanguageDropDownList ID="_selectionType" runat="server">
    <asp:ListItem Text="" Value="Single" TextId="/controlText/matrixRowSelectorEditor/singleRow" />
    <asp:ListItem Text="" Value="Multiple" TextId="/controlText/matrixRowSelectorEditor/multipleRows" />
</ckbx:MultiLanguageDropDownList>
</div>
<asp:Panel ID="_singleSelectionTypePanel" runat="server">
    <ckbx:MultiLanguageCheckBox ID="_answerRequired" runat="server" TextId="/controlText/matrixRowSelectorEditor/answerRequired" />
</asp:Panel>

<asp:Panel ID="_multipleSelectionTypePanel" runat="server">
    <ckbx:MultiLanguageLabel ID="_minToSelectLbl" runat="server" TextId="/controlText/matrixRowSelectorEditor/minToSelect" CssClass="itemEditorLabel_100"/>
    <asp:TextBox ID="_minToSelectTxt" runat="server" />   
    <br />
    <ckbx:MultiLanguageLabel ID="_maxToSelectLbl" runat="server" TextId="/controlText/matrixRowSelectorEditor/maxToSelect" CssClass="itemEditorLabel_100"/>
    <asp:TextBox ID="_maxToSelectTxt" runat="server" />  
    <ckbx:MultiLanguageLabel ID="_maxMinError" runat="server" TextId="/controlText/matrixRowSelectorEditor/maxMinError" Visible="false" CssClass="error message" />
</asp:Panel>
