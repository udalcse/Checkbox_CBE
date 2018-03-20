<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Image.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Image" %>


<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/multiLineTextAppearanceEditor/itemPosition" /></p>
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
