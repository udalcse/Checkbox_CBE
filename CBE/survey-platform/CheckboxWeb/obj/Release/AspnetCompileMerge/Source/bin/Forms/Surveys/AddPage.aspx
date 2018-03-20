<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AddPage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.AddPage" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="_pageContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <ckbx:MultiLanguageLabel ID="_pageLbl" runat="server" TextId="/pageText/forms/surveys/addPage.aspx/newPagePosition" />
        <br />
        <asp:DropDownList ID="_pageList" runat="server" />
    </div>
</asp:Content>