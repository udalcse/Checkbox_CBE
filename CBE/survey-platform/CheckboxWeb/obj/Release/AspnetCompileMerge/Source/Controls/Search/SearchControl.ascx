<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SearchControl.ascx.cs" Inherits="CheckboxWeb.Controls.Search.SearchControl" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Panel ID="_searchContainer" runat="server">
    <div class="SearchControlWrapper">
        <div class="SearchControls">
            <ckbx:MultiLanguageLabel ID="_searchLabel" runat="server" TextID="/controlText/SearchControl.ascx/SearchLabel" CssClass="SearchLabel">SEARCH</ckbx:MultiLanguageLabel>
            <asp:TextBox ID="_searchTerm" runat="server" Width="300" ></asp:TextBox>
            <ckbx:MultiLanguageDropDownList ID="_searchField" runat="server" />
        </div>
        <div class="SearchButtonWrapper" style="margin-top:15px;">
            <btn:CheckboxButton ID="_searchButton" runat="server" TextID="/controlText/SearchControl.ascx/SearchButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="SearchButton_Click" />
        </div>
    </div>    
    <asp:Panel ID="_appliedSearchPanel" runat="server" Visible="false" CssClass="SearchControlStatus">
        <asp:Label ID="_appliedSearchLabel" runat="server" />
            <ckbx:MultiLanguageLinkButton  ID="_removeAppliedSearch" runat="server" TextID="/controlText/SearchControl.ascx/ClearSearchButton" CssClass="ClearSearch" OnClick="ClearSearchButton_Click" />    
    </asp:Panel>
</asp:Panel>
<div style="height:5px;">&nbsp;</div>
