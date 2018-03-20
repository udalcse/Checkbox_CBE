<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Performance.aspx.cs" Inherits="CheckboxWeb.Settings.Performance" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/performance")%></h3>

    <!-- Site wide options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/performance.aspx/systemOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <!-- Controls whether or not surveys are cached in memory. Enabling this option will reduce the amount of time it takes for surveys to load at the cost of increased memory usage. -->
                <ckbx:MultiLanguageCheckBox id="_cacheResponseTemplates" runat="server" TextId="/pageText/settings/performance.aspx/cacheResponseTemplates" />
            </div>
            <div class="input">
                <!-- When this setting is enabled the ViewState is saved to the Checkbox database rather than respondents computers.  Saving this information to the database will reduce the page size but increase the amount of work your database server needs to perform. -->
                <ckbx:MultiLanguageCheckBox id="_persistViewStateToDb" runat="server" TextId="/pageText/settings/performance.aspx/persistViewStateToDb" />
            </div>
            <div class="input">
                <!-- Controls whether or not authorization queries check for access control list entries which revoke a user/groups rights. -->
                <ckbx:MultiLanguageCheckBox id="_allowExclusionaryAclEntries" runat="server" TextId="/pageText/settings/performance.aspx/allowExclusionaryAclEntries" />
            </div>
            <div class="fixed_125 left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_groupCacheSize" TextId="/pageText/settings/performance.aspx/groupCacheSize">Group cache size</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:TextBox id="_groupCacheSize" runat="server" class="settingsNumericInput_2" MaxLength="2"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator ID="_groupCacheSizeFormatValidator" runat="server" ControlToValidate="_groupCacheSize" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/groupCacheSize/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="_groupCacheSizeRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_groupCacheSize">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/groupCacheSize/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
        </div>
    </div>

    <!-- Report options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/performance.aspx/reportOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <!-- Determines whether to buffer exported results and send them after the export is finished processing. -->
                <ckbx:MultiLanguageCheckBox id="_bufferResponseExport" runat="server" TextId="/pageText/settings/performance.aspx/bufferResponseExport" />
            </div>
            <div class="left input fixed_175">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_responseDataExportChunckSize" TextId="/pageText/settings/performance.aspx/responseDataExportChunckSize">Response data export chunck size</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The number of lines to export at a time before flushing the Response output buffer when exporting response data. -->
                <asp:TextBox id="_responseDataExportChunckSize" runat="server" class="settingsNumericInput_4" MaxLength="4"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator CssClass="error message" ID="_responseDataExportChunckSizeFormatValidator" runat="server" Display="Dynamic" ControlToValidate="_responseDataExportChunckSize" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/responseDataExportChunckSize/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator CssClass="error message" ID="_responseDataExportChunckSizeRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_responseDataExportChunckSize">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/responseDataExportChunckSize/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
            <div class="left input fixed_175">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" AssociatedControlID="_maxReportPreviewOptions" TextId="/pageText/settings/performance.aspx/maxReportPreviewOptions">Max report preview options</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The maximum number of options, per item, to include in report previews. -->
                <asp:TextBox id="_maxReportPreviewOptions" runat="server" class="settingsNumericInput_2" MaxLength="2"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator CssClass="error message" ID="_maxReportPreviewOptionsFormatValidator" runat="server" Display="Dynamic" ControlToValidate="_maxReportPreviewOptions" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/maxReportPreviewOptions/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator CssClass="error message" ID="_maxReportPreviewOptionsRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_maxReportPreviewOptions">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/maxReportPreviewOptions/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
        </div>
    </div>
    
    <!-- user options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/performance.aspx/userOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <!-- Determines whether to buffer exported results and send them after the export is finished processing. -->
                <ckbx:MultiLanguageCheckBox ID="_disableAdUserList" runat="server" TextId="/pageText/settings/performance.aspx/disableAdUserList" />
            </div>
            <br class="clear" />
        </div>
    </div>

    <!-- Invitation options -->
    <div class="dashStatsWrapper border999 shadow999" style="display:none;">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/performance.aspx/emailOptions")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="left input fixed_125">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel6" runat="server" AssociatedControlID="_messageThrottle" TextId="/pageText/settings/performance.aspx/messageThrottle">Message throttle</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The number of email invitations to send at once. -->
                <asp:TextBox id="_messageThrottle" runat="server" class="settingsNumericInput_2" MaxLength="2"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator CssClass="error message" ID="_messageThrottleFormatValidator" runat="server" Display="Dynamic" ControlToValidate="_messageThrottle" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/messageThrottle/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator  CssClass="error message" ID="_messageThrottleRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_messageThrottle">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/messageThrottle/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
            <div class="left input fixed_125">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" AssociatedControlID="_messageThrottleDelay" TextId="/pageText/settings/performance.aspx/messageThrottleDelay">Message throttle delay</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The number of lines to export at a time before flushing the Response output buffer when exporting response data. -->
                <asp:TextBox id="TextBox1" runat="server" class="settingsNumericInput_4" MaxLength="4"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator CssClass="error message" ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ControlToValidate="TextBox1" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/responseDataExportChunckSize/invalidFormat")%>
                </asp:RegularExpressionValidator>                
            </div>
            <br class="clear" />
            <div class="left input fixed_125">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel5" runat="server" AssociatedControlID="_maxReportPreviewOptions" TextId="/pageText/settings/performance.aspx/maxReportPreviewOptions">Max report preview options</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The amount of time to delay between batches of emails, measured in seconds. -->
                <asp:TextBox id="_messageThrottleDelay" runat="server" class="settingsNumericInput_2" MaxLength="2"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator CssClass="error message" ID="_messageThrottleDelayFormatValidator" runat="server" Display="Dynamic" ControlToValidate="_messageThrottleDelay" ValidationExpression="^[1-9][0-9]*$">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/messageThrottleDelay/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator CssClass="error message" ID="_messageThrottleDelayRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_messageThrottleDelay">
                    <%= WebTextManager.GetText("/pageText/settings/performance.aspx/messageThrottleDelay/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
