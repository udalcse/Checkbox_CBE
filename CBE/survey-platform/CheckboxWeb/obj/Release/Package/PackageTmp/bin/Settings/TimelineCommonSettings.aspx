<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="TimelineCommonSettings.aspx.cs"
    Inherits="CheckboxWeb.Settings.TimelineCommonSettings" %>

<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/timeline/eventsettings/caption/common_settings" />
        </div>
        <div style="width: 100%" class="dashStatsContent">
            <div class="left">
                <div class="fixed_100 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_recordsPerPage" TextId="/timeline/eventsettings/description/timeline_records_per_page"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_recordsPerPage" runat="server" class="settingsNumericInput" Style="margin: -4px 0px 0px 62px"
                        MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_recordsPerPageFormatValidator" runat="server"
                        ControlToValidate="_recordsPerPage" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/timeline/eventsettings/invalid_format")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />
                <div class="spacing">&nbsp;</div>
                <div class="fixed_100 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_requestExpiration"
                        TextId="/timeline/eventsettings/description/timeline_request_expiration"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_requestExpiration" runat="server" class="settingsNumericInput"
                        Style="margin: -4px 0px 0px 62px" MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_requestExperationFormatValidator" runat="server"
                        ControlToValidate="_requestExpiration" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/timeline/eventsettings/invalid_format")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />
            </div>
        </div>
    </div>
        <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
