<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CopyItem.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.CopyItem" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content runat="server" ID="_content" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/chooseAction") %></div>
        <div class="dialogSubContainer">
            <ckbx:MultiLanguageRadioButton ID="_radCopy" runat="server" Checked="true"  TextId="/pageText/forms/surveys/reports/copyMoveItem.aspx/copy" GroupName="action" />
            <br />
            <ckbx:MultiLanguageRadioButton ID="_radMove" runat="server"  TextId="/pageText/forms/surveys/reports/copyMoveItem.aspx/move" GroupName="action" />
        </div>
        
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/chooseDestinationPage")%></div>
        <div class="dialogSubContainer">
            <asp:DropDownList ID="_destinationPageList" runat="server" OnSelectedIndexChanged="_destinationPageList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/newItemPosition")%></div>
        <div class="dialogSubContainer">
            <asp:DropDownList ID="_destinationItemList" runat="server" ></asp:DropDownList>
        </div>
    </div>
</asp:Content>
