<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Languages.aspx.cs" Inherits="CheckboxWeb.Settings.Languages" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_script" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript">

        $(document).ready(function () {
            $("#<%=_languageToDelete.ClientID %>").val("");
        });


        function onDialogClosed(args) {
            UFrameManager._uFrames['_editFrame'].load();
        }


        function confirmDeleteLanguage(language) {
            showConfirmDialogWithCallback(
                        '<%=WebTextManager.GetText("/pageText/settings/languages.aspx/deleteLanguageConfirm") %>',
                        function () {
                            $("#<%=_languageToDelete.ClientID %>").val(language);
                            UFrameManager.executeASPNETPostback($('#<%=_deleteLanguage.ClientID %>'), $('#<%=_deleteLanguage.ClientID %>').attr('href'));
                        },
                        337,
                        200,
                        '<%=WebTextManager.GetText("/pageText/settings/languages.aspx/deleteLanguageTitle") %>'
                    );
                    }


    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%=WebTextManager.GetText("/pageText/settings/navigation.ascx/languages")%></h3>

    <!-- Application Languages Not Yet Used
    <div class="dashStatsWrapper border999 shadow999" ID="_defaultLanguagePlace" runat="server" Visible="false">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/languages.aspx/defaultLanguage")%></span>
        </div>
        
        <div class="dashStatsContent">
            <div class="dashStatsContentHeader">
                <ckbx:MultiLanguageLabel ID="_defaultLanguageLbl" runat="server" TextId="/pageText/settings/languages.aspx/defaultLanguage" />
            </div>
            <div class="input">
                <asp:DropDownList ID="_defaultLanguageList" runat="server" AutoPostBack="true" />
            </div>
        </div>
    </div> 
    -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/languages.aspx/surveyLanguages")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="fixed_75 left">&nbsp;</div>
            <div class="left input">
                <asp:Repeater ID="_surveyLanguageRepeater" runat="server">
                    <ItemTemplate>
                        <div class="left input fixed_50"><asp:Label ID="_surveyLanguageCode" runat="server" Text='<%# Container.DataItem %>' /></div>
                        <div class="left input">
                            <img src="../App_Themes/CheckboxTheme/Images/delete16.gif" style="border-width:0px;" onclick="confirmDeleteLanguage('<%# Container.DataItem %>');" alt="<%=WebTextManager.GetText("/pageText/settings/languages.aspx/deleteLanguageAlt")%>" class="hand"/>
                        </div>
                        <br class="clear" />
                    </ItemTemplate>
                </asp:Repeater>
                <div class="spacing"></div>
                <div class="left fixed_25">
                    <asp:ImageButton  ID="_addSurveyLanguageBtn" runat="server" ImageUrl="../App_Themes/CheckboxTheme/Images/Plus-blue.png" uframeignore="true" Height="16" Width="16" OnClientClick="showDialog('AddLanguage.aspx?type=survey&onClose=onDialogClosed'); return false;" />
                </div>
                <div class="left">
                    <ckbx:MultiLanguageLabel ID="_addSurveyLanguageLbl" runat="server" TextId="/pageText/settings/languages.aspx/addLanguage" />
                </div>
                <br class="clear" />
            </div>
            <br class="clear" />
        </div>
    </div>

    <asp:LinkButton runat="server" style="display:none" id="_deleteLanguage"/>
    <asp:HiddenField runat='server' ID="_languageToDelete"/>


    <!-- Application Languages Not Yet Used
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/languages.aspx/applicationLanguages")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="fixed_75 left">&nbsp;</div>
            <div class="left input">
                <asp:Repeater ID="_applicationLanguagesRepeater" runat="server">
                    <ItemTemplate>
                        <div class="left input fixed_50"><asp:Label ID="_applicationLanguageCode" runat="server" Text='<%# Container.DataItem %>' /></div>
                        <div class="left input"><asp:ImageButton OnClientClick="ShowSplash();return true;" ID="_deleteAppLanguageBtn" runat="server" ImageUrl="../App_Themes/CheckboxTheme/Images/delete16.gif" CommandName="DeleteAppLanguage" CommandArgument='<%# Container.DataItem %>' /></div>
                        <br class="clear" />
                    </ItemTemplate>
                </asp:Repeater>
                <div class="spacing"></div>
                <div class="left fixed_25">
                    <asp:ImageButton ID="_addAppLanguageBtn" runat="server" ImageUrl="../App_Themes/CheckboxTheme/Images/Plus-blue.png" Height="16" Width="16" uframeignore="true" OnClientClick="showDialog('AddLanguage.aspx?type=app'); return false;"/>
                </div>
                <div class="left">
                    <ckbx:MultiLanguageLabel ID="_addAppLanguageLbl" runat="server" TextId="/pageText/settings/languages.aspx/addLanguage" />
                </div>
                <br class="clear" />
            </div>
            <br class="clear" />
        </div>
    </div> -->
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
