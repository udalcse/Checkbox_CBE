<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ExceptionEntry.aspx.cs" Inherits="CheckboxWeb.Settings.ExceptionEntry" %>
<%@ Register TagPrefix="grid" Namespace="Checkbox.Web.UI.Controls.GridTemplates" Assembly="Checkbox.Web" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
  <asp:panel runat="server" id="entryPanel" Width="700px"></asp:panel>
  <div class="spacing">&nbsp;</div>
  <ckbx:MultiLanguageLinkButton runat="server" ID="_back" TextId="/pageText/forms/manage.aspx/back" CssClass="ckbxButton roundedCorners border999 shadow999 redButton"/>
  <div class="dialogFormPush">&nbsp;</div>
</asp:Content>