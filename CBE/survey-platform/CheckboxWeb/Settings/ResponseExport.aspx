<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ResponseExport.aspx.cs" Inherits="CheckboxWeb.Settings.ResponseExport" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/responseExport")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/responseExport.aspx/globalOptions") %></span>
        </div>
        <div class="dashStatsContent">
            <div class="dashStatsContentHeader">
                <%=WebTextManager.GetText("/pageText/settings/responseExport.aspx/defaultExportType") %>
            </div>
            <div class="input">
                <ckbx:MultiLanguageRadioButtonList ID="_defaultExportType" runat="server" />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <div class="mainStats"><%= WebTextManager.GetText("/pageText/settings/responseExport.aspx/csvOptions") %></div>
        </div>
        <div class="dashStatsContent">
            <div class="left">
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_detailedResponseInfo" runat="server" TextId="/pageText/settings/responseExport.aspx/detailedResponseInfo" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_detailedUserInfo" runat="server" TextId="/pageText/settings/responseExport.aspx/detailedUserInfo" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_mergeCheckboxResults" runat="server" TextId="/pageText/settings/responseExport.aspx/mergeCheckboxResults" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_exportOpenendedResults" runat="server" TextId="/pageText/settings/responseExport.aspx/exportOpenendedResults" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_exportWithAliases" runat="server" TextId="/pageText/settings/responseExport.aspx/exportWithAliases" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_exportHiddenItems" runat="server" TextId="/pageText/settings/responseExport.aspx/exportHiddenItems" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_exportIncompleteResponses" runat="server" TextId="/pageText/settings/responseExport.aspx/exportIncompleteResponses" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_exportStripHtmlTags" runat="server" TextId="/pageText/settings/responseExport.aspx/exportStripHtmlTags" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_splitExport" runat="server" TextId="/pageText/settings/responseExport.aspx/splitExport" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_rankOrderPoints" runat="server" TextId="/pageText/settings/responseExport.aspx/exportRankOrderPoints" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_detailedScoreInfo" runat="server" TextId="/pageText/settings/responseExport.aspx/detailedScoreInfo" />
                </div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox id="_possibleScore" runat="server" TextId="/pageText/settings/responseExport.aspx/possibleScore" />
                </div>
            </div>
            <div class="left fixed_50">&nbsp;</div>
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_outputEncoding" TextId="/pageText/settings/responseExport.aspx/outputEncoding">Output encoding</ckbx:MultiLanguageLabel>
                </div>
                <div>
                    <asp:RadioButtonList ID="_outputEncoding" runat="server">
                        <asp:ListItem Value="ASCII" />
                        <asp:ListItem Value="UTF8" />
                    </asp:RadioButtonList>
                </div>
                <div class="spacing">&nbsp;</div>
                <div class="input">
                    <ckbx:MultiLanguageCheckBox ID="_replaceNewLine" runat="server" AutoPostBack="true" OnCheckedChanged="NewLine_ClickEvent" TextId="/pageText/settings/responseExport.aspx/replaceNewLine" />
                </div>
                <div class="spacing">&nbsp;</div>
                <div class="left input">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_newLineReplacement" TextId="/pageText/settings/responseExport.aspx/newLineReplacement">New line replacement</ckbx:MultiLanguageLabel>
                </div>
                <div class="left input">
                    <asp:TextBox id="_newLineReplacement" runat="server" />
                </div>
                <br class="clear" />
            </div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/responseExport.aspx/spssOptions") %></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_includeResponseIdInExport" runat="server" TextId="/pageText/settings/responseExport.aspx/includeResponseIdInExport" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_exportIncompleteResponsesSpss" runat="server" TextId="/pageText/settings/responseExport.aspx/exportIncompleteResponses" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_exportOpenEndedResultsSpss" runat="server" TextId="/pageText/settings/responseExport.aspx/exportOpenEndedResults" />
            </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
