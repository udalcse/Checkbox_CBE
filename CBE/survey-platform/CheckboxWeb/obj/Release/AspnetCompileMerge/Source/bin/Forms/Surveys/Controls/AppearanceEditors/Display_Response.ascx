<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Display_Response.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Display_Response" %>

<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="_itemPositionLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/itemPosition" />
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
    <ckbx:MultiLanguageLabel ID="_widthLbl" runat="server" TextId="/controlText/AppearanceEditor/width" />
</div>
<br />
<div class="itemEditorInput">
    <asp:TextBox ID="_widthTxt" runat="server" />
</div>
<div class="itemEditorLabel_100">
    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/controlText/AppearanceEditor/height" />
</div>

<div class="itemEditorInput">
    <asp:TextBox ID="_heightTxt" runat="server" />
</div>


