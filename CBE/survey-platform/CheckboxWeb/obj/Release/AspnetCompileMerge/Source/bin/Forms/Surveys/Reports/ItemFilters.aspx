<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ItemFilters.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.ItemFilters" MasterPageFile="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" TagPrefix="ckbx" TagName="FilterSelector" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="PageTitle" style="padding-bottom:5px;"><ckbx:MultiLanguageLabel id="_titleLbl" runat="server" TextId="/pageText/filterSelector.aspx/applyFilters" /></div>
        
        <ckbx:FilterSelector ID="_filterSelector" runat="server" />

        <hr size="1" />

        <div style="position:absolute;bottom:15px;right:15px;width:175px;">
            <btn:CheckboxButton ID="_saveButton" runat="server" TextID="/common/save" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" />
            <btn:CheckboxButton ID="_cancelButton" runat="server" TextID="/common/cancel" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" OnClientClick="closeWindowAndRefreshParentPage('');return false;" />
        </div>
    </div>
</asp:Content>