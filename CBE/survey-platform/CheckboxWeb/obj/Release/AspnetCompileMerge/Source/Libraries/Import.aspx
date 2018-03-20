<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Import.aspx.cs" Inherits="CheckboxWeb.Libraries.Import" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Libraries/Controls/LibraryUploader.ascx" TagName="LibraryUploader" TagPrefix="ckbx"%>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div class="dialogSubTitle">
            <%=WebTextManager.GetText("/pageText/styles/forms/import.aspx/importLibrary")%>
        </div>

        <div class="dialogInstructions">
            <ckbx:MultiLanguageLabel ID="_selectFileLbl" runat="server" TextId="/controlText/forms/libraries/libraryUploader.ascx/selectUploadLibrary" />
        </div>
        <ckbx:LibraryUploader id="_uploader" runat="server" />
        <asp:Label ID="_uploadValidation" runat="server" CssClass="error message" Visible="false"></asp:Label>
      </div>
</asp:Content>