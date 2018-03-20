<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="SurveyPreferences.aspx.cs" Inherits="CheckboxWeb.Settings.SurveyPreferences" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveyPreferences")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/newSurveyOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_defaultSurveySecurity" TextId="/pageText/settings/surveyPreferences.aspx/defaultSurveySecurity">Default survey security:</ckbx:MultiLanguageLabel>
                </div>
                <ckbx:MultiLanguageRadioButtonList ID="_defaultSurveySecurity" runat="server">
                    <asp:ListItem TextId="/enum/securityType/public" Value="Public">Public</asp:ListItem>
                    <asp:ListItem TextId="/enum/securityType/passwordProtected" Value="PasswordProtected">Password Protected</asp:ListItem>
                    <asp:ListItem TextId="/enum/securityType/accessControlList" Value="AccessControlList">Access List</asp:ListItem>
                    <asp:ListItem TextId="/enum/securityType/allRegisteredUsers" Value="AllRegisteredUsers">All Registered Users</asp:ListItem>
                    <asp:ListItem TextId="/enum/securityType/invitationOnly" Value="InvitationOnly">Invitation Only</asp:ListItem>
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <div class="fixed_50 left">&nbsp;</div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/defaultSurveyStyle")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_defaultSurveyStyle" TextId="/pageText/settings/surveyPreferences.aspx/PCStyle">PC Style</ckbx:MultiLanguageLabel>
                </div>
                <asp:DropDownList id="_defaultSurveyStyle" runat="server" />
                <ckbx:MultiLanguageLabel ID="_noPublicStyles" runat="server" Visible="false" TextId="/pageText/settings/surveyPreferences.aspx/noPublicStyles"/>
            </div>
            <div class="left fixed_50"/>
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" runat="server" AssociatedControlID="_defaultSurveyStyleTablet" TextId="/pageText/settings/surveyPreferences.aspx/TabletStyle">Tablet Style</ckbx:MultiLanguageLabel>
                </div>
                <asp:DropDownList id="_defaultSurveyStyleTablet" runat="server" />
                <ckbx:MultiLanguageLabel ID="_noPublicStylesTablet" runat="server" Visible="false" TextId="/pageText/settings/surveyPreferences.aspx/noPublicStyles"/>
            </div>
            <div class="left fixed_50"/>
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel9" runat="server" AssociatedControlID="_defaultSurveyStyleSmartPhone" TextId="/pageText/settings/surveyPreferences.aspx/SmartPhoneStyle">SmartPhone Style</ckbx:MultiLanguageLabel>
                </div>
                <asp:DropDownList id="_defaultSurveyStyleSmartPhone" runat="server" />
                <ckbx:MultiLanguageLabel ID="_noPublicStylesSmartPhone" runat="server" Visible="false" TextId="/pageText/settings/surveyPreferences.aspx/noPublicStyles"/>
            </div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/editorOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" AssociatedControlID="_defaultOptionEntryType" TextId="/pageText/settings/surveyPreferences.aspx/defaultOptionMode">Default option mode:</ckbx:MultiLanguageLabel>
                </div>
                <ckbx:MultiLanguageRadioButtonList id="_defaultOptionEntryType" runat="server">
                    <asp:ListItem TextId="/pageText/settings/surveyPreferences.aspx/normal" Value="Normal">Normal</asp:ListItem>
                    <asp:ListItem TextId="/pageText/settings/surveyPreferences.aspx/quickEntry" Value="QuickEntry">Quick Entry</asp:ListItem>
                    <asp:ListItem TextId="/pageText/settings/surveyPreferences.aspx/xmlImport" Value="XMLImport">XML Import</asp:ListItem>
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <div class="left fixed_50">&nbsp;</div>
            <div class="left">
                <div class="dashStatsContentHeader">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel5" runat="server" AssociatedControlID="_defaultSurveyQuestionEditorType" TextId="/pageText/settings/surveyPreferences.aspx/defaultSurveyQuestionEditorView">Default survey question editor view:</ckbx:MultiLanguageLabel>
                </div>
                <ckbx:MultiLanguageRadioButtonList id="_defaultSurveyQuestionEditorType" runat="server">
                    <asp:ListItem TextId="/pageText/settings/surveyPreferences.aspx/htmlEditor" Value="HTML">Html Editor</asp:ListItem>
                    <asp:ListItem TextId="/pageText/settings/surveyPreferences.aspx/textArea" Value="Textarea">Textarea</asp:ListItem>
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <br class="clear" />
        </div>
        <div class="spacing"></div>
        <div class="dashStatsContent">
            <div class="input"><ckbx:MultiLanguageCheckBox id="_confirmWhenDeletingSurveyItemsAndPages" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/confirmWhenDeletingSurveyItemsAndPages"/></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_allowEditSurveyStyleTemplate" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/allowEditSurveyStyleTemplate" /></div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/surveyPreferences.aspx/globalOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input"><ckbx:MultiLanguageCheckBox id="_enableCustomSurveyUrls" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/enableCustomSurveyUrls"/></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_allowEditingOfActiveSurveys" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/allowEditingOfActiveSurveys"/></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_saveWhenNavigatingBack" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/saveResponsesWhenNavigatingBack"/></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_setSurveyDefaultButton" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/setSurveyDefaultButton" /></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_displayHtmlItemAsPlainText" runat="server" TextId="/pageText/settings/surveyPreferences.aspx/displayHtmlItemAsPlainText" /></div>
            <div class="input"><ckbx:MultiLanguageCheckBox id="_includeIncompleteResponsesToTotalAmount" runat="server"  TextID="/pageText/settings/surveyPreferences.aspx/includeIncompleteResponsesToTotalAmount"/></div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>