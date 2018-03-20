<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Multi_Line_Text.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Multi_Line_Text" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_rowsTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_columnsTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_rowsTxt" ID="_rowsLbl" runat="server" TextId="/controlText/multiLineTextAppearanceEditor/rows" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_rowsTxt" runat="server" Width="50px" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
     <p><ckbx:MultiLanguageLabel AssociatedControlID="_columnsTxt" ID="_columnsLbl" runat="server" TextId="/controlText/multiLineTextAppearanceEditor/columns" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_columnsTxt" runat="server" Width="50px" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/multiLineTextAppearanceEditor/itemPosition" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" Text="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_labelPositionList" ID="_labelPositionLbl" runat="server" TextId="/controlText/multiLineTextAppearanceEditor/labelPosition" /></p>
</div>
<div class="formInput left">
     <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server">
        <asp:ListItem Value="Top" Text="Top" TextId="/enum/labelPosition/top" />
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/labelPosition/left" />
        <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/labelPosition/bottom" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/labelPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>
