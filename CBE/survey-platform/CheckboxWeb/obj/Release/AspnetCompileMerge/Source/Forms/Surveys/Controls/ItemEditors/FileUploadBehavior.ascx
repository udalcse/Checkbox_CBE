<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FileUploadBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.FileUploadBehavior" %>


<!-- Required -->
<div class="formInput">
    <div class="left">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_requiredChk" ID="_requiredLbl" runat="server" TextId="/controlText/uploadItemEditor/isRequired" /></p>
    </div>
    <div class="left checkBox" style="margin-left:5px;">
        <asp:CheckBox ID="_requiredChk" runat="server" />
    </div>
</div>

<br class="clear" />
<br class="clear" />

<!-- Alias -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
    <asp:TextBox ID="_aliasText" runat="server" />
</div>

<br class="clear" />

<!-- Default Text -->
<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_fileTypesList" ID="_defaultValueLbl" runat="server" TextId="/controlText/uploadItemEditor/allowedFileTypes" /></p>
    <div style="padding-left:15px;">
        <asp:CheckBoxList ID="_fileTypesList" runat="server" RepeatColumns="2" RepeatDirection="Vertical" />
    </div>    
</div>

<br class="clear" />
