<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScoreBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.NetPromoterScoreBehavior" %>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_aliasTextlbl" AssociatedControlID="_aliasText" runat="server" TextId="/controlText/listEditor/alias" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear"/>
<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_requiredChecklbl" runat="server" TextId="/controlText/selectItemEditor/answerRequired" AssociatedControlID="_requiredCheck"/></p>
</div>
<div class="formInput left">
    <asp:CheckBox runat="server" ID="_requiredCheck"/>
</div>
<br class="clear"/>
