<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Dialog.Master" CodeBehind="ReportFilters.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.ReportFilters" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" TagPrefix="ckbx" TagName="FilterSelector" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="dialogSubTitle"><ckbx:MultiLanguageLabel id="_titleLbl" runat="server" TextId="/pageText/filterSelector.aspx/applyFilters" /></div>
    <ckbx:FilterSelector ID="_filterSelector" runat="server" />
</asp:Content>
