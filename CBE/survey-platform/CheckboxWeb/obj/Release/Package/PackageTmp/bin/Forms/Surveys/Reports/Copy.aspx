<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Copy.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Copy" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content runat="server" ID="_content" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <div class="formInput">
            <p><label for="<%=_nameText.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/name", null, "Report Name: ")%></label></p>
            <asp:TextBox ID="_nameText" Width="60%" runat="server" />
        </div>
    </div>
</asp:Content>
