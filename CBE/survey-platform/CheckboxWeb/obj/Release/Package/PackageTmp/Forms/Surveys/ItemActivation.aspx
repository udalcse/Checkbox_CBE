<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ItemActivation.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.ItemActivation" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content runat="server" ID="_content" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/forms/surveys/ItemActivation.aspx/chooseAction", null, "Select Item Activation status")%></div>
        <div class="dialogSubContainer">
            <ckbx:MultiLanguageRadioButton ID="_active" runat="server" Checked="true" Text="Activated" TextId="/pageText/forms/surveys/ItemActivation.aspx/activated" GroupName="action" />
            <br />
            <ckbx:MultiLanguageRadioButton ID="_deactive" runat="server" Text="Deactivated" TextId="/pageText/forms/surveys/ItemActivation.aspx/deactivated" GroupName="action" />
            <br />
        </div>
    </div>
</asp:Content>
