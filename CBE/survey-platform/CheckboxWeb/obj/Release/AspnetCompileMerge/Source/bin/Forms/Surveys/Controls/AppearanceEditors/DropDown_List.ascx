<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DropDown_List.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.DropDown_List" %>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_numberLabelsChk" ID="_numberLabelsLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/showNumberLabels" /></p>
</div>
<div class="formInput left">
    <asp:CheckBox ID="_numberLabelsChk" runat="server" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_itemPositionLbl" AssociatedControlID="_itemPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_labelPositionLbl" AssociatedControlID="_labelPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/labelPosition" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server">
        <asp:ListItem Value="Top" TextId="/enum/labelPosition/top" />
        <asp:ListItem Value="Left" TextId="/enum/labelPosition/left" />
        <asp:ListItem Value="Bottom" TextId="/enum/labelPosition/bottom" />
        <asp:ListItem Value="Right" TextId="/enum/labelPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_showAsComboboxChk" ID="_showAsComboboxLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/showAsCombobox" /></p>
</div>
<div class="formInput left">
    <asp:CheckBox ID="_showAsComboboxChk" runat="server" />
</div>
<br class="clear"/>
