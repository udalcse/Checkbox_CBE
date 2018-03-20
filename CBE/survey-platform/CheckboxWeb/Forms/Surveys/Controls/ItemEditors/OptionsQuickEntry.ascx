<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="OptionsQuickEntry.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.OptionsQuickEntry" %>
<ckbx:MultiLanguageLabel ID="_helpLabel" TextId="/controlText/quickEntry/instructions"
    runat="server"></ckbx:MultiLanguageLabel>
<br />
<ckbx:MultiLanguageLabel ID="_formatLabel" runat="server" CssClass="label"></ckbx:MultiLanguageLabel>
<br />
<asp:Panel ID="_errorPanel" Visible="false" runat="server">
    <ckbx:MultiLanguageLabel ID="_errorLable" TextId="/controlText/quickEntry/pointsFormat"
        runat="server" CssClass="ErrorMessageFld"></ckbx:MultiLanguageLabel>
    <br />
</asp:Panel>
<ckbx:MultiLanguageTextBox ID="_inputTextBox" runat="server" TextMode="MultiLine" Width="400px" Height="350px" style="overflow-y:auto;" CssClass="quickEntryText"></ckbx:MultiLanguageTextBox>