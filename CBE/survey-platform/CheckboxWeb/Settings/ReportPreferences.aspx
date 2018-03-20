<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ReportPreferences.aspx.cs" Inherits="CheckboxWeb.Settings.ReportPreferences" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/reportPreferences")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/itemOptions") %></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <div class="left">
                <div class="fixed_100 left input">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_radioButtons" TextId="/pageText/settings/reportPreferences.aspx/radioButtons">Radio buttons</ckbx:MultiLanguageLabel>
                </div>
                <div class="input left">
                    <asp:DropDownList id="_radioButtons" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_checkboxes" TextId="/pageText/settings/reportPreferences.aspx/checkboxes">Checkboxes</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_checkboxes" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" AssociatedControlID="_singleLineText" TextId="/pageText/settings/reportPreferences.aspx/slt">Slt</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_singleLineText" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" AssociatedControlID="_multiLineText" TextId="/pageText/settings/reportPreferences.aspx/mlt">Mlt</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_multiLineText" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_slider" TextId="/pageText/settings/reportPreferences.aspx/slider">Slider</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_slider" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel14" runat="server" AssociatedControlID="_netPromoterScore" TextId="/pageText/settings/reportPreferences.aspx/netPromoterScore">Net Promoter Score</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList Width="200px" id="_netPromoterScore" runat="server" />
                </div>
                <br class="clear" />
            </div>
            <div class="left fixed_50">&nbsp;</div>
            <div class="left">
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel13" runat="server" AssociatedControlID="_rankOrder" TextId="/pageText/settings/reportPreferences.aspx/rankOrder">Rank Order</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_rankOrder" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel5" runat="server" AssociatedControlID="_ratingScale" TextId="/pageText/settings/reportPreferences.aspx/ratingScale">Rating scale</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_ratingScale" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel6" runat="server" AssociatedControlID="_dropDownList" TextId="/pageText/settings/reportPreferences.aspx/dropDownList">Drop down list</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_dropDownList" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel7" runat="server" AssociatedControlID="_matrix" TextId="/pageText/settings/reportPreferences.aspx/matrix">Matrix</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_matrix" runat="server" />
                </div>
                <br class="clear" />
                <div class="fixed_100 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel8" runat="server" AssociatedControlID="_hiddenItems" TextId="/pageText/settings/reportPreferences.aspx/hiddenItems">Hidden items</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:DropDownList id="_hiddenItems" runat="server" />
                </div>
            </div>
            <br class="clear" />
            <div class="spacing">&nbsp;</div>
            <div class="left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel9" runat="server" AssociatedControlID="_maxNumberOfOptions" TextId="/pageText/settings/reportPreferences.aspx/maxNumberOfOptions">Maximum number of item options allowed before using a summary table</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:TextBox id="_maxNumberOfOptions" runat="server" class="settingsNumericInput" style="margin:-4px 0px 0px 62px" MaxLength="5"/>
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator ID="_resultsPerPageFormatValidator" runat="server" ControlToValidate="_maxNumberOfOptions" Display="Dynamic" ValidationExpression="^[1-9][0-9]*$" CssClass="error message">
                    <%= WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/maxNumberOfOptions/invalidFormat")%>
                </asp:RegularExpressionValidator>
            </div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/reportOptions") %></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <div class="fixed_100 left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel10" runat="server" AssociatedControlID="_styleTemplate" TextId="/pageText/settings/reportPreferences.aspx/styleTemplate">Style template</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:DropDownList id="_styleTemplate" runat="server" />
            </div>
            <br class="clear" />
            <div class="fixed_100 left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel11" runat="server" AssociatedControlID="_chartStyle" TextId="/pageText/settings/reportPreferences.aspx/chartStyle">Chart style</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:DropDownList id="_chartStyle" runat="server" />
            </div>
            <br class="clear" />
            <div class="fixed_100 left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel12" runat="server" AssociatedControlID="_itemPosition" TextId="/pageText/settings/reportPreferences.aspx/itemPosition">Item position</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <asp:DropDownList id="_itemPosition" runat="server" />
            </div>
            <br class="clear" />
            <div class="spacing">&nbsp;</div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_useAliases" runat="server" TextId="/pageText/settings/reportPreferences.aspx/useAliases" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_includeIncompleteResponses" runat="server" TextId="/pageText/settings/reportPreferences.aspx/includeIncompleteResponses" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_includeTestResponses" runat="server" TextId="/pageText/settings/reportPreferences.aspx/includeTestResponses" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_placeAllAnalysisItemsOnASinglePage" runat="server" TextId="/pageText/settings/reportPreferences.aspx/placeAllAnalysisItemsOnASinglePage" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox ID="_displaySurveyTitle" runat="server" TextId="/pageText/settings/reportPreferences.aspx/displaySurveyTitle" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox ID="_displayPdfExportButton" runat="server" TextId="/pageText/settings/reportPreferences.aspx/displayPdfExportButton" />
            </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
