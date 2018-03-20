<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Appearance.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Appearance" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register TagPrefix="ckbx" TagName="AppearanceEditor" Src="~/Forms/Surveys/Controls/Appearance.ascx" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <ckbx:AppearanceEditor ID="_appearanceEditor" runat="server" />
   </div>
</asp:Content>
