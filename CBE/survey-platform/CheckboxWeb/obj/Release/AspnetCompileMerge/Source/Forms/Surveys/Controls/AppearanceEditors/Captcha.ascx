<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Captcha.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Captcha" %>


<div class="formInput left fixed_250">
    <ckbx:MultiLanguageLabel ID="_itemPositionLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" />
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

