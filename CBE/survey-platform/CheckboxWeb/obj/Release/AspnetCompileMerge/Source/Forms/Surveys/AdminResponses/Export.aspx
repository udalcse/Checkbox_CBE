<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Export.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.AdminResponses.Export"  MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10" style="height: 230px;">
        <div style="padding-left:10px;">
        <div class="dialogSubTitle">
            <%= WebTextManager.GetText("/pageText/forms/surveys/reports/export.aspx/options")%>
        </div>
            </div>
        <div class="dialogSubContainer">
            <div class="formInput condensed">
                <div class="left fixed_200" >
                    <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_documentOrientation" TextId="/pageText/forms/surveys/reports/export.aspx/documentLayout" /></p>
                </div>
                <div class="left">
                    <ckbx:MultiLanguageDropDownList ID="_documentOrientation" runat="server" AutoPostBack="false" />
                </div>
            </div>
        </div>
        <div style="clear:both" />
             
        <!--
        <asp:Panel ID="_fileDownloadPanel" runat="server" Visible="false">
            <div class="dashStatsWrapper">
                <ul class="dialogSubTitle">
                    <li class="mainStats"><%= WebTextManager.GetText("/pageText/forms/surveys/responses/export.aspx/uploadedFiles")%></li>
                    <div style="clear:both" />
                    <div style="clear:both" />
                </ul>
                <div style="padding-top:5px;">
                    <btn:CheckboxButton ID="_dlFilesBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" TextId="/pageText/forms/surveys/responses/export.aspx/downloadFiles"/>
                </div>
            </div>
        </asp:Panel>
        <div style="clear:both" /> -->
    </div>
</asp:Content>
