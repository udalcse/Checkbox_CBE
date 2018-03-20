<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Create.aspx.cs" Inherits="CheckboxWeb.Forms.Folders.Create" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <script>
        $(document).ready(function() {
            $('#<%=_folderNameTxt.ClientID%>').focus();
        });
                                  
</script>
    <div class="padding10">
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel ID="_folderNameLbl" runat="server" TextId="/pagetext/forms/folders/create.aspx/folderName" AssociatedControlID="_folderNameTxt" /></p>
            <asp:TextBox ID="_folderNameTxt" runat="server" Width="375" MaxLength="30" />
            <br class="clear"/>
            <asp:RequiredFieldValidator ID="_nameRequiredValidator" runat="server" ControlToValidate="_folderNameTxt" Display="Dynamic" />
            <asp:CustomValidator ID="_folderNameInUseValidator" runat="server" ControlToValidate="_folderNameTxt" Display="Dynamic" />
            <asp:Label id="_nameErrorLbl" runat="server" CssClass="ErrorMessageFld" Width="300"></asp:Label>
        </div>
    </div>
</asp:Content>