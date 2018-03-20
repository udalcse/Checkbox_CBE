<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Libraries.Properties" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ContentPlaceHolderID="_pageContent" ID="content" runat="server" >
<div class="padding10">
    <div class="input left fixed_125">
        <ckbx:MultiLanguageLabel ID="libraryNameLbl" runat="server" TextId="/pageText/libraries/create.aspx/name"/>
    </div>
    <div class="left input">
        <asp:TextBox ID="_libraryName" runat="server" Width="300" SetFocusOnError="true" MaxLength="32" />
    </div>
    <div class="input left">
        <asp:RequiredFieldValidator ID="_nameRequired" runat="server" ControlToValidate="_libraryName" Display="Dynamic" />
        <asp:Panel runat="server" CssClass="ErrorMessage" ID="_duplicateName" Visible="false"><%=WebTextManager.GetText("/pageText/libraries/create.aspx/nameInUse")%></asp:Panel>
    </div>
    <br class="clear" />
    <div>&nbsp;</div>

    <div class="input left fixed_125">
        <ckbx:MultiLanguageLabel ID="libraryDescLbl" runat="server" TextId="/pageText/libraries/create.aspx/description" />
    </div>
    <div class="left input">
        <asp:TextBox ID="_description" runat="server" TextMode="MultiLine" Width="300"  Rows="5" />
    </div
    <br class="clear" />
</div>
</asp:Content>