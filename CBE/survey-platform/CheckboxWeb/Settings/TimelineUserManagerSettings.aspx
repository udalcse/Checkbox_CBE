<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="TimelineUserManagerSettings.aspx.cs"
    Inherits="CheckboxWeb.Settings.TimelineSettings" %>

<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register TagPrefix="TimelineSettings" TagName="TimelineSettings" Src="~/Settings/Controls/TimelineSettings.ascx" %>
<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/user_manager_settings"  />
        </div>
        <div style="width:100%">
            <TimelineSettings:TimelineSettings ID="tlUserSettings" runat="server" Manager="UserManager"></TimelineSettings:TimelineSettings>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
