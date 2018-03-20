<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CaptchaBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CaptchaBehavior" %>

<script type="text/javascript" language="javascript">
    $(document).ready(function() {
        $('#<%=_minCodeLengthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_maxCodeLengthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_imageWidthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_imageHeightTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>

<!-- Alias -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" />
    <asp:TextBox ID="_aliasText" runat="server" /></p>
</div>

<br class="clear" />

<!-- Max code length -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_minCodeLengthTxt" ID="_minCodeLengthLbl" runat="server" TextId="/controlText/captchaItemEditor/minCodeLength" />
    <asp:TextBox ID="_minCodeLengthTxt" runat="server" Width="50px" /></p>
</div>

<!-- Min code length -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_maxCodeLengthTxt" ID="_maxCodeLengthLbl" runat="server" TextId="/controlText/captchaItemEditor/maxCodeLength" />
    <asp:TextBox ID="_maxCodeLengthTxt" runat="server" Width="50px" /></p>
</div>

<br class="clear" />

<!-- Code type -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_codeTypeList" ID="_codeTypeLbl" runat="server" TextId="/controlText/captchaItemEditor/codeType" />
    <asp:DropDownList ID="_codeTypeList" runat="server">
        <asp:ListItem Text="Alpha" Value="Alpha" />
        <asp:ListItem Text="Numeric" Value="Numeric" />
        <asp:ListItem Text="AlphaNumeric" Value="AlphaNumeric" />
    </asp:DropDownList></p>
</div>

<br class="clear" />

<!-- Image width -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_imageWidthTxt" ID="_imageWidthLbl" runat="server" TextId="/controlText/captchaItemEditor/imageWidth" />
    <asp:TextBox ID="_imageWidthTxt" runat="server" Width="50px" /></p>
</div>

<!-- Image height -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_imageHeightTxt" ID="_imageHeightLbl" runat="server" TextId="/controlText/captchaItemEditor/imageHeight" />
    <asp:TextBox ID="_imageHeightTxt" runat="server" Width="50px" /></p>
</div>

<br class="clear" />

<!-- Image format -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_imageFormatList" ID="_imageFormatLbl" runat="server" TextId="/controlText/captchaItemEditor/imageFormat" />
    <asp:DropDownList ID="_imageFormatList" runat="server">
        <asp:ListItem Text="Gif" Value="Gif" />
        <asp:ListItem Text="Jpg" Value="Jpeg" />
        <asp:ListItem Text="Png" Value="Png" />
        <asp:ListItem Text="Bmp" Value="Bmp" />
    </asp:DropDownList></p>
</div>

<br class="clear" />

<!-- Enable sound 
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_enableSoundChk" ID="_enableSoundLbl" runat="server" TextId="/controlText/captchaItemEditor/enableSound" /></p>
    <asp:CheckBox ID="_enableSoundChk" runat="server" />
</div>
-->
<br class="clear" />