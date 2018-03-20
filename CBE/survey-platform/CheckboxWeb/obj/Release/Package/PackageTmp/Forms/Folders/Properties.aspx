<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Forms.Folders.Properties" MasterPageFile="~/Dialog.Master"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10 formInput">
        <ckbx:MultiLanguageLabel ID="_folderNameLbl" runat="server" TextId="/pagetext/forms/folders/create.aspx/folderName" AssociatedControlID="_folderNameTxt" />
        <asp:TextBox ID="_folderNameTxt" runat="server" Width="250" MaxLength="50" />
        <br class="clear" />
        <asp:RequiredFieldValidator ID="_nameRequiredValidator" runat="server" ControlToValidate="_folderNameTxt" Display="Dynamic" />
        <asp:CustomValidator ID="_folderNameInUseValidator" runat="server" ControlToValidate="_folderNameTxt" Display="Dynamic" />
        <asp:Label id="_nameErrorLbl" runat="server" CssClass="ErrorMessageFld" Width="300"></asp:Label>
    </div>
</asp:Content>


