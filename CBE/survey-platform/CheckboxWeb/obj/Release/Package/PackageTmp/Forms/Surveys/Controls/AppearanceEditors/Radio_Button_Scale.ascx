<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Radio_Button_Scale.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Radio_Button_Scale" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_optionWidth.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_layoutLbl" AssociatedControlID="_layoutList" runat="server" TextId="/controlText/repeatDirectionEditor/layout" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_layoutList" runat="server">
        <asp:ListItem Value="Vertical" Text="Vertical" TextId="/enum/repeatDirection/vertical" />
        <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/enum/repeatDirection/horizontal" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" /></p>
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
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_labelPositionList" ID="_labelPositionLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/labelPosition" /></p>
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
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_showSeparatorList" ID="_showSeparatorLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/showSeparator" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_showSeparatorList" runat="server">
        <asp:ListItem Value="YES" Text="Yes" TextId="/common/yes" />
        <asp:ListItem Value="NO" Text="No" TextId="/common/no" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_optionWidth" ID="_optionWidthLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/optionWidth" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_optionWidth" runat="server" Width="75px" ></asp:TextBox> pixels
</div>
<br class="clear"/>

