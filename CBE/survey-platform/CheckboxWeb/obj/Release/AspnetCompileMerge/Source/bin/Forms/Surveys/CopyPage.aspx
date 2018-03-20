<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CopyPage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.CopyPage" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_pageContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/movePage.aspx/chooseAction") %></div>
        <div class="dialogSubContainer">
            <ckbx:MultiLanguageRadioButton ID="_radCopy" runat="server" Checked="true"  TextId="/pageText/forms/movePage.aspx/copy" GroupName="action" AutoPostBack="True"/>
            <br />
            <ckbx:MultiLanguageRadioButton ID="_radMove" runat="server"  TextId="/pageText/forms/movePage.aspx/move" GroupName="action" AutoPostBack="True"/>
        </div>

        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/movePage.aspx/chooseDestination")%></div>
        <div class="dialogSubContainer">
            <asp:DropDownList ID="_pageList" runat="server" />
        </div>
    </div>
</asp:Content>