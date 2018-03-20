<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="FacebookMenu.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.FacebookMenu" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/FacebookButton.ascx" TagPrefix="ckbx" TagName="FacebookButton" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding15">
         <% if (IsProtectedReport) { %>
            <div class="padding10">
                <div class="StatusPanel warning">
                    <span><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/reportWarning")%></span>
                </div>
            </div>
        <% } %>

        <ckbx:FacebookButton ID="_facebookButton" runat="server" />
    </div>
</asp:Content>