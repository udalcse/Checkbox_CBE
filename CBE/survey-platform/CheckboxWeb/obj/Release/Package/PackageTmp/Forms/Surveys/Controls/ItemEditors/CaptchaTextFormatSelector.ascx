<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CaptchaTextFormatSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CaptchaTextFormatSelector" %>


<div>
    <div>
        <asp:CheckBoxList ID="_formatList" runat="server" RepeatColumns="4" RepeatDirection="Vertical" />
    </div>
    
    <div style="clear:both;"></div>
    
    <asp:Panel ID="_tooltipZone" runat="server">
        <!--
        <hr size="1" />
        <ckbx:MultiLanguageLabel ID="_previewLbl" runat="server" TextId="/controlText/captchaItemEditor/mouseOverToPreview" />-->
        <asp:Image ID="_previewImg" runat="server" Visible="false" />
    </asp:Panel>
</div>