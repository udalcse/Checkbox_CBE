<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="WizardNavigator.ascx.cs" Inherits="CheckboxWeb.Controls.Wizard.WizardControls.WizardNavigator" %>
<asp:Repeater ID="_wizardNavigationRepeater" runat="server" OnItemDataBound="WizardNavigationRepeater_ItemDataBound">
    <HeaderTemplate><ul class="WizardNav"></HeaderTemplate>
    <ItemTemplate><asp:PlaceHolder ID="_navListItem" runat="server" /></ItemTemplate>
    <SeparatorTemplate></SeparatorTemplate>
    <FooterTemplate></ul></FooterTemplate>
</asp:Repeater>

