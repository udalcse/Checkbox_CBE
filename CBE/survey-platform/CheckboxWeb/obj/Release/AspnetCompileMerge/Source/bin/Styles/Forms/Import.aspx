<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Import.aspx.cs" Inherits="CheckboxWeb.Styles.Forms.Import" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Styles/Forms/Controls/StyleUploader.ascx" TagName="StyleUploader" TagPrefix="ckbx" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <div class="dialogSubTitle" style="width:250px;">
            <%=WebTextManager.GetText("/pageText/styles/forms/import.aspx/uploadTitle") %>
        </div>
        
       

        <ckbx:StyleUploader id="_uploader" runat="server" />
        <asp:Label ID="_uploadValidation" runat="server" CssClass="error message" Visible="false"></asp:Label>
      </div>
</asp:Content>