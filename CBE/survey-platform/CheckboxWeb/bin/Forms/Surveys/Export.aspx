<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Export.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Export" MasterPageFile="~/Dialog.Master" %>

<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10" style="height: 230px;">
        <div style="padding-left:10px;">
        <div class="dialogSubTitle">
            <%= WebTextManager.GetText("/pageText/forms/surveys/export.aspx/options")%>
        </div>
            </div>
        <div class="dialogSubContainer">
            <div class="formInput condensed">
                <div class="left fixed_200" >
                    <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_documentOrientation" TextId="/pageText/forms/surveys/export.aspx/documentLayout" /></p>
                </div>
                <div class="left">
                    <ckbx:MultiLanguageDropDownList ID="_documentOrientation" runat="server" AutoPostBack="false" />
                </div>
            </div>
        </div>

        <asp:Panel runat="server" ID="_languageListPanel" class="dialogSubContainer">
            <div class="formInput condensed">
                <div class="left fixed_200" >
                    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_languageList" TextId="/pageText/forms/surveys/export.aspx/language" /></p>
                </div>
                <div class="left">
                    <ckbx:MultiLanguageDropDownList ID="_languageList" runat="server" AutoPostBack="false" />
                </div>
            </div>
        </asp:Panel>

        <div style="clear:both" />
    </div>
</asp:Content>
