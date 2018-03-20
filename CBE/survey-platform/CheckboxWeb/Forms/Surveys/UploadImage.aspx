<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="UploadImage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.UploadImage" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageSelector.ascx" TagPrefix="sel" TagName="ImageSelector" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <sel:ImageSelector ID="_imageSelector" runat="server" />
</asp:Content>
