<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Single_Line_Text.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Single_Line_Text" %>

<script type="text/javascript">
    $(document).ready(function() {
        $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/itemPosition" /></p>
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
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_labelPositionList" ID="_labelPositionLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/labelPosition" /></p>
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

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_widthTxt" ID="_widthLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/width" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_widthTxt" runat="server" />
</div>
<br class="clear"/>

