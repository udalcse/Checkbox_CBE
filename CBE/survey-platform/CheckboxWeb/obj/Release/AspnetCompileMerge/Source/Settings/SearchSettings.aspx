<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="SearchSettings.aspx.cs"
    Inherits="CheckboxWeb.Settings.SearchSettings" %>

<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="SearchObjects" Src="~/Settings/Controls/SearchObjects.ascx" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/search")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/pageText/settings/searchsettings.aspx/caption/caching" />
        </div>
        <div style="width: 100%" class="dashStatsContent">
            <div class="left">
                <div class="fixed_300 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_searchAccessibleObjectExpPeriodSeconds" TextId="/pageText/settings/searchsettings.aspx/searchAccessibleObjectExpPeriodSeconds"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_searchAccessibleObjectExpPeriodSeconds" runat="server" class="settingsNumericInput" Style="margin: -4px 0px 0px 62px"
                        MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_searchAccessibleObjectExpPeriodSecondsValidator" runat="server"
                        ControlToValidate="_searchAccessibleObjectExpPeriodSeconds" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/invalidformat")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />

                <div class="spacing">&nbsp;</div>
                <div class="fixed_300 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_searchResultsExpPeriodSeconds"
                        TextId="/pageText/settings/searchsettings.aspx/searchResultsExpPeriodSeconds"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_searchResultsExpPeriodSeconds" runat="server" class="settingsNumericInput"
                        Style="margin: -4px 0px 0px 62px" MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_searchResultsExpPeriodSecondsValidator" runat="server"
                        ControlToValidate="_searchResultsExpPeriodSeconds" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/invalidformat")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />

                <div class="spacing">&nbsp;</div>
                <div class="fixed_300 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_searchPageSize"
                        TextId="/pageText/settings/searchsettings.aspx/searchPageSize"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_searchPageSize" runat="server" class="settingsNumericInput"
                        Style="margin: -4px 0px 0px 62px" MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_searchPageSizeValidator" runat="server"
                        ControlToValidate="_searchPageSize" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/invalidformat")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />

                <div class="spacing">&nbsp;</div>
                <div class="fixed_300 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_searchMaxResultRecordsPerObjectType"
                        TextId="/pageText/settings/searchsettings.aspx/searchMaxResultRecordsPerObjectType"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_searchMaxResultRecordsPerObjectType" runat="server" class="settingsNumericInput"
                        Style="margin: -4px 0px 0px 62px" MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_searchMaxResultRecordsPerObjectTypeValidator" runat="server"
                        ControlToValidate="_searchMaxResultRecordsPerObjectType" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/invalidformat")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />


                <div class="spacing">&nbsp;</div>
                <div class="fixed_300 left input">
                    <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_searchCachePeriodDays"
                        TextId="/pageText/settings/searchsettings.aspx/searchCachePeriodDays"></ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox ID="_searchCachePeriodDays" runat="server" class="searchCachePeriodDays"
                        Style="margin: -4px 0px 0px 62px" MaxLength="5" />
                </div>
                <div class="left input">
                    <asp:RegularExpressionValidator ID="_searchCachePeriodDaysValidator" runat="server"
                        ControlToValidate="_searchCachePeriodDays" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$"
                        CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/invalidformat")%>
                    </asp:RegularExpressionValidator>
                </div>
                <br class="clear" />

            </div>
        </div>
    </div>


    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <ckbx:MultiLanguageLabel class="mainStats left" runat="server" TextId="/pageText/settings/searchsettings.aspx/caption/results" />
        </div>
        <div style="width: 100%" class="dashStatsContent">
            <ckbx:SearchObjects ID="_objectTypes" runat="server"/>
        </div>
        <div class="note dashStatsContent">
            <%= WebTextManager.GetText("/pageText/settings/searchsettings.aspx/hint")%>
        </div>
    </div>
    <br class="clear" />
</asp:Content>
