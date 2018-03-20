<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResponseSettings.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.ResponseSettings" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ResponseProperties.ascx" TagPrefix="ckbx" TagName="ResponseProperties" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <ckbx:ResponseProperties ID="_properties" runat="server" />
    </div>
    <div class="clear"></div>
</asp:Content>