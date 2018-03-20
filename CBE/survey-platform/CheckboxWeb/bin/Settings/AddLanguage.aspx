<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="AddLanguage.aspx.cs" Inherits="CheckboxWeb.Settings.AddLanguage" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<div class="padding10">
    <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/settings/languages.aspx/addLanguage")%></div>
    <div>&nbsp;</div>
    <div class="left input fixed_125">
        <ckbx:MultiLanguageLabel ID="_languageCodeLbl" runat="server" TextId="/pageText/Settings/AddLanguage.aspx/newLanguageCode" />
    </div>
    <div class="left input">
        <asp:TextBox ID="_languageCode" MaxLength="5" runat="server" />
    </div>
    <br class="clear" />
</div>
</asp:Content>
