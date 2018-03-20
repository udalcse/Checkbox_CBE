<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Properties.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.Controls.Properties" %>

<div class="field_150">
    <ckbx:MultiLanguageLabel ID="_styleNameLbl" runat="server" TextId="/controlText/styles/styleName" />
</div>
<div class="input">
    <asp:TextBox ID="_styleNameTxt" runat="server" Width="350" />
    <br />
    <div>
    <asp:RequiredFieldValidator Display="Dynamic" ID="_nameRequiredValidator" runat="server" ControlToValidate="_styleNameTxt" />
    <asp:CustomValidator Display="Dynamic" ID="_nameInUseValidator" runat="server" ControlToValidate="_styleNameTxt" />
    </div>
</div>
<div style="clear:both"></div>

<div class="field_150"></div>
<div class="input">
    <ckbx:MultiLanguageCheckBox ID="_publicChk" runat="server" TextAlign="Right" TextId="/controlText/styles/public" ></ckbx:MultiLanguageCheckBox>
</div>
<div class="clear"></div>

<div class="field_150"></div>
<div class="input">
    <ckbx:MultiLanguageCheckBox ID="_allowEditChk" runat="server" TextAlign="Right" TextId="/controlText/styles/editable" ></ckbx:MultiLanguageCheckBox>
</div>
<div class="clear"></div>