<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Properties" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ReportProperties.ascx" TagPrefix="ckbx" TagName="ReportProperties" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:ReportProperties ID="_properties" runat="server" />
    <div class="clear"></div>
</asp:Content>