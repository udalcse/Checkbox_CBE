<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Forms.Folders.Edit" MasterPageFile="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="PageTitle" style="padding-bottom:5px;"><ckbx:MultiLanguageLabel id="_titleLbl" runat="server" TextId="/pageText/forms/manage.aspx/editFolderProperties" /></div>
    <div style="margin-left:15px;height:200px;">
        <div class="field_150">
            <ckbx:MultiLanguageLabel ID="_folderNameLbl" runat="server" TextId="/pagetext/forms/folders/edit.aspx/folderName" />
        </div>
        <div class="input">
            <asp:TextBox ID="_folderNameTxt" runat="server" Width="300" />
            <ckbx:CalloutRequiredFieldValidator ID="_nameRequiredValidator" runat="server" ControlToValidate="_folderNameTxt" TextID="/pageText/forms/folders/edit.aspx/nameRequired" />
            <ckbx:CalloutFolderNameInUseValidator ID="_nameInUseValidator" runat="server" ControlToValidate="_folderNameTxt" TextID="/pageText/forms/folders/edit.aspx/nameInUse" />
        </div>
    
        <div style="clear:both" />
    
        <hr size="1" />

        <div style="float:right">
            <btn:CheckboxButton runat="server" ID="_cancelBtn" TextId="/common/cancel" CausesValidation="false" OnClientClick="closeWindow();return false;" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" />
        </div>
        <div style="float:right">
            <btn:CheckboxButton runat="server" ID="_okBtn" TextId="/common/save" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" CausesValidation="true" />
        </div>
    </div>
</asp:Content>
