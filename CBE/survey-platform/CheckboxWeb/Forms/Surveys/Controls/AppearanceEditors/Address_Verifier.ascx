<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Address_Verifier.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Address_Verifier" %>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_widthLbl" runat="server" TextId="/controlText/addressVerifierAppearanceEditor/width" />
</div>

<div class="itemEditorInput">
    <asp:TextBox ID="_widthTxt" runat="server" />
</div>

<div style="clear:both;"></div>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_itemPositionLbl" runat="server" TextId="/controlText/addressVerifierAppearanceEditor/itemPosition" />
</div>

<div class="itemEditorInput">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" Text="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear:both;"></div>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_labelPositionLbl" runat="server" TextId="/controlText/addressVerifierAppearanceEditor/labelPosition" />
</div>

<div class="itemEditorInput">
  <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server">
        <asp:ListItem Value="Top" Text="Top" TextId="/enum/labelPosition/top" />
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/labelPosition/left" />
        <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/labelPosition/bottom" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/labelPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>

<div style="clear:both;"></div>