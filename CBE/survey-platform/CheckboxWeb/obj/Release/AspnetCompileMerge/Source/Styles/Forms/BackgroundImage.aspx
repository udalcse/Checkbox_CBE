<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="BackgroundImage.aspx.cs" Inherits="CheckboxWeb.Styles.Forms.BackgroundImage" MasterPageFile="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageSelector.ascx" TagPrefix="ckbx" TagName="ImageSelector" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ContentPlaceHolderID="_pageContent" ID="content" runat="server" >
    <div class="padding10">
        <div class="dialogSubTitle">
            <ckbx:MultiLanguageLabel ID="locationLbl" runat="server" TextId="/pageText/styles/forms/backgroundImage.aspx/imageSource"></ckbx:MultiLanguageLabel>
        </div>
        
        <ckbx:ImageSelector ID="_imageSelector" runat="server" />        
        
        <div class="formInput left fixed_225 padding10">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_isBackgroundRepeatDataChk" ID="_isBackgroundRepeat" runat="server" TextId="/pageText/styles/forms/backgroundImage.aspx/backgroundRepeat" /></p>
        </div>
        <div class="left padding10">
            <asp:CheckBox ID="_isBackgroundRepeatDataChk" runat="server"/>
        </div>
            
    </div>
</asp:Content>