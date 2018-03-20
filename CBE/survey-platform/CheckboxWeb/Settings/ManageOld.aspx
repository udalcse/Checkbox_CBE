<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" CodeBehind="ManageOld.aspx.cs" Inherits="CheckboxWeb.Settings.Manage" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Admin.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="grid_10 prefix_1 suffix_1">
    <h1><asp:Image runat="server" SkinID="H1LeftEdge" /><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/settings")%><asp:Image ID="Image1" runat="server" SkinID="H1RightEdge" /></h1>
    <ul class="SettingsSectionList">
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./SurveyPreferences.aspx?section=survey"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveySettings")%></a></li>
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./ReportPreferences.aspx?section=report"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/reportSettings")%></a></li>
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./Users.aspx?section=user"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/userSettings")%></a></li>
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./Branding.aspx?section=system"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/systemSettings")%></a></li>
    </ul>
     <br />
    <h1><asp:Image ID="Image2" runat="server" SkinID="H1LeftEdge" /><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/applicationText")%><asp:Image ID="Image4" runat="server" SkinID="H1RightEdge" /></h1>
    <ul class="SettingsSectionList">
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./Languages.aspx?section=text"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/textSettings")%></a></li>
    </ul>
    <br />
    <h1><asp:Image ID="Image3" runat="server" SkinID="H1LeftEdge" /><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/monitoringTools")%><asp:Image ID="Image5" runat="server" SkinID="H1RightEdge" /></h1>
    <ul class="SettingsSectionList">
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./LoggedInUsers.aspx"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/currentUsers")%></a></li>
        <li><asp:Image runat="server" SkinID="SettingsSectionBullet" />&nbsp;<a href="./Exceptions.aspx"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/exceptionLog")%></a></li>
    </ul>
    </div>
</asp:Content>
