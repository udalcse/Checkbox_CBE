<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CalculatorBehavior.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CalculatorBehavior" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<asp:Panel ID="_errorPanel" runat="server" CssClass="error message" Visible="false">
    <asp:Label ID="_errorLbl" runat="server" />
</asp:Panel>

<div class="input left fixed_100">
    <ckbx:MultiLanguageLabel ID="_formulaLbl" runat="server" TextId="/controlText/calculatorEditor/formula" />
</div>
<div class="left input">
    <asp:TextBox TextMode="multiline" ID="_formula" runat="server" Rows="10" Columns="60"/>
</div>
<br class="clear" />

<div class="input left fixed_100">
    <ckbx:MultiLanguageLabel ID="_valueTypeLbl" runat="server" TextId="/controlText/sliderEditor/valueType" />
</div>
<pipe:PipeSelector ID="_pipeSelector" runat="server" />
<br class="clear"/>

<div class="left input">
    <ckbx:MultiLanguageDropDownList ID="_roundToList" runat="server">
        <asp:ListItem Text="" Value="0" TextId="/controlText/calculatorEditor/roundplaces0" />
        <asp:ListItem Text="" Value="1" TextId="/controlText/calculatorEditor/roundplaces1" />
        <asp:ListItem Text="" Value="2" TextId="/controlText/calculatorEditor/roundplaces2" />
        <asp:ListItem Text="" Value="3" TextId="/controlText/calculatorEditor/roundplaces3" />
        <asp:ListItem Text="" Value="4" TextId="/controlText/calculatorEditor/roundplaces4" />
        <asp:ListItem Text="" Value="5" TextId="/controlText/calculatorEditor/roundplaces5" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear" />

