<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Security.aspx.cs" Inherits="CheckboxWeb.Settings.Security" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">

    <script type="text/javascript">
        $(function() {
            <% if (!ApplicationManager.AppSettings.UseEncryption || UserManager.CountUnencryptedPasswords() > 0) { %>
                $('#encryptPasswordsBtn').click(function () {
                    showDialog('<%=ResolveUrl("~/Settings/HashPasswords.aspx") %>', 'properties');
                    return false;
                });
            <% } else { %>
                    $('#encryptPasswordsBtn').hide();
            <% } %>
        });
    </script>
</asp:Content>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <!-- Controls whether or not Checkbox uses cookies to track anonymous respondents. When cookies are disabled it is not possible for anonymous respondents to resume incomplete responses or for Checkbox to enforce the maximum number of responses per user. -->
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/security")%></h3>

    <div class="dashStatsWrapper  border999 shadow999" id="_securityMode" runat="server">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/security.aspx/securityMode")%></span>
        </div>
        <div class="dashStatsContent">
            <ckbx:MultiLanguageCheckBox id="_simpleSecurity" runat="server" CssClass="input" TextId="/pageText/settings/security.aspx/simpleSecurity" />
            <div class="clear"></div>
            <ckbx:MultiLanguageCheckBox id="_preventAdminAutoLogin" runat="server" CssClass="input" TextId="/pageText/settings/security.aspx/preventAdminAutoLogin" />
         </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/security.aspx/surveyOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="dashStatsContentHeader">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_sessionType" TextId="/pageText/settings/security.aspx/sessionType">Session type</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <ckbx:MultiLanguageRadioButtonList ID="_sessionType" runat="server" RepeatDirectio="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="SessionType_ClickEvent">
                    <asp:ListItem TextId="/pageText/settings/security.aspx/cookies" Text="Cookies" Value="Cookies" />
                    <asp:ListItem TextId="/pageText/settings/security.aspx/cookieless" Text="Cookieless" Value="Cookieless" />
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <br class="clear" />
            <div class="spacing">&nbsp;</div>
            <div class="left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_cookieName" TextId="/pageText/settings/security.aspx/cookieName">Cookie name</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- The name of the cookie used to track anonymous respondents. -->            
                <asp:TextBox id="_cookieName" runat="server" />
            </div>
            <div class="left input">
                <asp:RegularExpressionValidator ID="_cookieNameFormatValidator" runat="server" ControlToValidate="_cookieName" Display="Dynamic" ValidationExpression="^\w+$">
                    <%= WebTextManager.GetText("/pageText/settings/security.aspx/cookieName/invalidFormat")%>
                </asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="_cookieNameRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_cookieName">
                    <%= WebTextManager.GetText("/pageText/settings/security.aspx/cookieName/requiredField")%>
                </asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_allowNumericSurveyIds" runat="server" TextId="/pageText/settings/security.aspx/allowNumericSurveyIds" />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/security.aspx/passwordOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left warning message">
                <asp:Label ID="_passwordStatusLbl" runat="server" />
            </div>
            <!-- When selected, user passwords will be encrypted using a one-way hash so that a user's password cannot be discovered by looking at the database -->
            <a id="encryptPasswordsBtn" class="ckbxButton roundedCorners border999 shadow999 silverButton smallButton left" uframeignore="true" style="margin-left: 10px;padding: 5px;">
                <%= WebTextManager.GetText("/pageText/settings/security.aspx/encryptPasswords")%>
            </a>
            <br class="clear" />
        </div>
    </div>
    
    <!-- Feature will be released 5.5.0 -->
    <div class="dashStatsWrapper  border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/security.aspx/sslOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <ckbx:MultiLanguageCheckBox id="_redirectHTTPtoHTTPS" runat="server" CssClass="input" TextId="/pageText/settings/security.aspx/redirecthttptohttps" />
         </div>
    </div>

    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
