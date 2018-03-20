<%@ Page Title="" Language="C#"  MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="JavascriptItem.aspx.cs" Inherits="CheckboxWeb.Settings.JavascriptItem" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">        
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/javascriptItem")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/javascriptItem.aspx/javascriptItemTitle")%></span>
            <br class="clear" />
        </div>
        <div class="warning message margin10">
            <%= WebTextManager.GetText("/pageText/settings/javascriptItem.aspx/enableJavascriptItemWarning") %>
            <a href="https://en.wikipedia.org/wiki/Cross-site_scripting" uframeignore="true" target="_blank">XSS</a>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_enableJavascriptItem" runat="server" AutoPostBack="false" TextId="/pageText/settings/javascriptItem.aspx/enableJavascriptItem" /><br />
            </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>