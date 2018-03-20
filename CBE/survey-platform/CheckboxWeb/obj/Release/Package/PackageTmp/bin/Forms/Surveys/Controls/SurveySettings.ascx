<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveySettings.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SurveySettings" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.LicenseLibrary" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Page" %>

<!-- Survey Urls -->
<ul id="setting_tabs" class="tabContainer tabsMenu" style="display: block; padding-top: 0;">
    <li>
        <a href="#">General</a>
    </li>
    <li>
        <a href="#">Appearance</a>
    </li>
    <li>
        <a href="#">Permissions & security</a>
    </li>
    <li>
        <a href="#">Languages & texts</a>
    </li>
    <li>
        <a href="#">Terms & links</a>
    </li>
</ul>
<div id="setting_sections" class="tabContentContainer" style="display: block;">
    <!-- Basic Info -->
    <div class="setting-section">
        <div class="setting-item descriptive">
        </div>
        <div class="setting-item" togglesettingname="IsActive">
            <label class="inline"><%= WebTextManager.GetText("/controlText/surveyDashboard/active") %></label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="AllowSurveyEditWhileActive">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/locked") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/locked") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>

         <div class="setting-item" togglesettingname="DisplayPDFDownloadButton">
            <label class="inline" style="width: 220px;">Display PDF download button <a class="tooltip" title="Determines if we need to display print pdf button at the end of a survey" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>

        <div class="setting-item">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/defaultUrl") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/defaultUrl") %>" href="#"></a>
            </label>
            <span class="static url-default"></span>
            <div class="actions inline">
                <a href="#" class="newWindow url-default" target="_blank"></a>
            </div>
        </div>
        <% if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            { %>
        <div class="setting-item">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/customUrl") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/customUrl") %>" href="#"></a>
            </label>
            <span class="static url-custom"></span>
            <input type="text" class="url-custom" /><span id="url-custom-status" class="status"></span>
            <select id="newAltUrlExtension" class="inline">
                <% foreach (var ext in ApplicationManager.AppSettings.AllowedUrlRewriteExtensions)
                    { %>
                <option value=".<%= ext %>">.<%= ext %></option>
                <% } %>
            </select>
            <div class="actions inline inactive save-options">
                <a id="saveNewAltUrlBtn" href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a><a id="cancelNewAltUrlBtn" href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
            <div class="actions inline">
                <a href="#" class="newWindow url-custom" target="_blank"></a>
            </div>
        </div>
        <% } %>

        <div class="url-errors-list">
            <div class="error message inline" style="margin-left: 10px; display: none;" id="URL_INVALID_Error"><%= WebTextManager.GetText("/pageText/surveyProperties.aspx/invalidName") %></div>
            <div class="error message inline" style="margin-left: 10px; display: none;" id="URL_MAPPING_IN_USE_Error"><%= WebTextManager.GetText("/pageText/surveyProperties.aspx/mappingExists") %></div>
            <div class="error message inline" style="margin-left: 10px; display: none;" id="URL_FILE_EXISTS_Error"><%= WebTextManager.GetText("/pageText/surveyProperties.aspx/mappedFileExists") %></div>
            <div class="error message inline" style="margin-left: 10px; display: none;" id="URL_TOO_LONG_Error"><%= WebTextManager.GetText("/pageText/surveyProperties.aspx/alternateURLTooLong") %></div>
        </div>

        <div class="setting-section-divider"></div>
        <div class="setting-item" settingname="ActivationStartDate">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/startDate") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/startDate") %>" href="#"></a>
            </label>
            <input class="date datetimepicker" type="text" style="position: relative; width: 130px; z-index: 1000;" /><span class="status"></span>
            <div class="actions inline">
                <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
            </div>
            <div class="actions inline inactive save-options">
                <a href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
        </div>
        <div class="setting-item" settingname="ActivationEndDate">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/endDate") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/endDate") %>" href="#"></a>
            </label>
            <input class="date datetimepicker" type="text" style="position: relative; width: 130px; z-index: 1000;" /><span class="status"></span>
            <div class="actions inline">
                <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
            </div>
            <div class="actions inline inactive save-options">
                <a href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
        </div>


        <div class="setting-item" settingname="MaxTotalResponses">
            <label class=""><%= WebTextManager.GetText("/controlText/forms/surveys/responseLimits.ascx/limitTotalResponses") %></label><input class="number" type="text" /><span class="status"></span>
            <div class="actions inline inactive save-options">
                <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a><a href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
        </div>
        <div class="setting-item" settingname="MaxResponsesPerUser">
            <label class=""><%= WebTextManager.GetText("/controlText/forms/surveys/responseLimits.ascx/limitResponsesPerRespondent") %></label><input class="number" type="text" /><span class="status"></span>
            <div class="actions inline inactive save-options">
                <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a><a href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
        </div>
        <div class="setting-item" togglesettingname="EnableScoring">
            <label class="inline">
                <%= WebTextManager.GetText("/controlText/surveyDashboard/scoredSurvey") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/surveyDashboard/tooltip/scoredSurvey") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="AnonymizeResponses">
            <label class="inline">
                <%= WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/anonymize") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/tooltip/anonymize") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" settingname="GoogleAnalyticsTrackingID">
            <label class=""><%= WebTextManager.GetText("/controlText/surveyDashboard/googleAnalyticsTrackingID") %></label><input type="text" /><span class="status"></span>
            <div class="actions inline inactive save-options">
                <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a><a href="#" class="cancel"><%= WebTextManager.GetText("/common/cancel") %></a>
            </div>
        </div>

    </div>

    <!-- Appearance -->
    <div class="setting-section">
        <div class="left" style="min-width: 400px;">
            <div class="setting-item" settingname="StyleTemplateID">
                <label class="inline">
                    <%= WebTextManager.GetText("/pageText/forms/surveystyle.aspx/stylePC") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/styleTemplate") %>" href="#"></a>
                </label>
                <% if (PcStyles.Count > 1)
                    { %>
                <select class="autosave">
                    <% foreach (var style in PcStyles)
                        { %>
                    <option value="<%= style.Key %>"><%= style.Value %></option>
                    <% } %>
                </select><span class="status"></span>
                <% }
                    else
                    { %>
                <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/noStyleTemplates") %>
                <% } %>
            </div>
            <div class="setting-item" settingname="MobileStyleID">
                <label class="inline">
                    <%= WebTextManager.GetText("/pageText/forms/surveystyle.aspx/mobileStyle") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/mobileStyle") %>" href="#"></a>
                </label>
                <% if (MobileStyles.Count > 1)
                    { %>
                <select class="autosave">
                    <% foreach (var style in MobileStyles)
                        { %>
                    <option value="<%= style.Key %>"><%= style.Value %></option>
                    <% } %>
                </select><span class="status"></span>
                <% }
                    else
                    { %>
                <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/noStyleTemplates") %>
                <% } %>
            </div>
            <div class="setting-section-divider"></div>
            <div class="setting-item" togglesettingname="ShowTitle">
                <label class="inline long"><%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showSurveyTitle") %></label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="ShowPageNumbers">
                <label class="inline long"><%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showPageNumbers") %></label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="ShowItemNumbers">
                <label class="inline long"><%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showQuestionNumbers") %></label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
             <div class="setting-item" togglesettingname="ShowTopSurveyButtons">
                <label class="inline long">Show navigation buttons at top of all survey pages</label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
             <div class="setting-item" togglesettingname="HideTopSurveyButtonsOnFirstAndLastPage" style="display: none;">
                <label class="inline long">Hide navigation buttons at top of first and last pages</label><input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="UseDynamicPageNumbers">
                <label class="inline long">
                    <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/useDynamicPageNumbers") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/useDynamicPageNumbers") %>" href="#"></a>
                </label>
                <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="RandomizeItems">
                <label class="inline long">
                    <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/randomizeItems") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/randomizeItems") %>" href="#"></a>
                </label>
                <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="ShowInputErrorAlert">
                <label class="inline long"><%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showValidationAlert") %></label><input type="checkbox" value="yes" class="switch" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="ShowAsterisks">
                <label class="inline long">
                    <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showAsterisks") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/showAsterisks") %>" href="#"></a>
                </label>
                <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="HideFooterHeader">
                <label class="inline long">
                    <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/hideHeaderFooter") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/hideHeaderFooterOnMobile") %>" href="#"></a>
                </label>
                <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" togglesettingname="ShowProgressBar">
                <label class="inline long">
                    <%= WebTextManager.GetText("/pageText/surveyStyle.aspx/showProgressBar") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyStyle.aspx/tooltip/showProgressBar") %>" href="#"></a>
                </label>
                <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
            </div>
            <div class="setting-item" settingname="ProgressBarOrientation" style="display: none;">
                <label class="inline long"><%= WebTextManager.GetText("/pageText/surveyStyle.aspx/progressBarOrientation") %></label>
                <select class="autosave" style="margin-left: -40px;">
                    <% foreach (var option in ProgressBarOrientationOptions)
                        { %>
                    <option value="<%= option.Key %>"><%= option.Value %></option>
                    <% } %>
                </select><span class="status"></span>
            </div>
        </div>
        <div id="appearance-preview-wrapper" class="left preview-background-smartphone">
            <iframe class="preview-smartphone" style="top: -82px; left: -76px; position: relative;"></iframe>
        </div>

    </div>

    <!-- Permissions -->
    <div class="setting-section">
        <div class="setting-item" settingname="SecurityType">
            <label class="inline">
                <%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/privacyLevel") %><a class="tooltip" title="" href="#"></a>
            </label>
            <select class="autosave alter-tooltip has-additional-options">
                <option value="Public" data-type="public" title-data="<%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/tooltip/public") %>"><%= WebTextManager.GetText("/enum/securityType/public") %></option>
                <option value="PasswordProtected" data-type="password" title-data="<%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/tooltip/password") %>"><%= WebTextManager.GetText("/enum/securityType/passwordProtected") %></option>
                <option value="AccessControlList" data-type="acl" title-data="<%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/tooltip/acl") %>"><%= WebTextManager.GetText("/enum/securityType/accessControlList") %></option>
                <option value="AllRegisteredUsers" data-type="registered" title-data="<%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/tooltip/registred") %>"><%= WebTextManager.GetText("/enum/securityType/allRegisteredUsers") %></option>
                <option value="InvitationOnly" data-type="invite" title-data="<%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/tooltip/invitation") %>"><%= WebTextManager.GetText("/enum/securityType/invitationOnly") %></option>
            </select><span class="status"></span>
            <div class="actions inactive additional-options additional-options-acl">
                <div class="setting-item">
                    <a href="javascript:void(0);" id="accessList" class="ckbxButton silverButton"><%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/respondentsList") %></a>
                </div>
            </div>
            <div class="actions inactive additional-options additional-options-password">
                <div class="setting-item" settingname="Password">
                    <label class="inline"><%= WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/password") %></label><input type="text" placeholder="Enter password" /><span class="status"></span>
                    <div class="actions inline save-options">
                        <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
                    </div>
                </div>
            </div>
        </div>
        <div class="setting-item" togglesettingname="DisableBackButton">
            <label class="inline long">
                <%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowBackButton") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/tooltip/allowBackButton") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="AllowFormReset">
            <label class="inline long">
                <%= WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/formReset") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/tooltip/formReset") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="AllowEdit">
            <label class="inline long">
                <%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowEdit") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/tooltip/allowEdit") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="AllowResume">
            <label class="inline long">
                <%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowResume") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/tooltip/allowResume") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="setting-item" togglesettingname="ShowSaveAndQuit" style="display: none;">
            <label class="inline long">
                <%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/showSaveAndExit") %><a class="tooltip" title="<%= WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/tooltip/showSaveAndExit") %>" href="#"></a>
            </label>
            <input type="checkbox" class="switch" value="yes" checked="checked" /><span class="status"></span>
        </div>
        <div class="advanced-settings" id="advancedPermissions">
            <div class="toggle-settings">
                <a href="#">+ Show Advanced Settings</a>
            </div>
            <div class="setting-container">
                <iframe height="650" width="800"></iframe>
            </div>
        </div>
    </div>

    <!-- Languages & Texts -->
    <div class="setting-section">
        <div class="setting-item" settingname="DefaultLanguage">
            <label class="inline"><%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/defaultLanguage") %></label>
            <select id="surveyDefaultLanguageList" class="autosave"></select>
        </div>
        <div class="actions inline manageLanguagesArea" style="padding-bottom: 10px;">
            <% if (ApplicationManager.AppSettings.AllowMultiLanguage)
                { %>
            <a href="#" class="ckbxButton silverButton" id="manageLanguagesButton"><%= WebTextManager.GetText("/pageText/surveyProperties.aspx/manageSurveyLanguages") %></a>
            <% } %>

            <div id="availableLanguages" class="languagesDiv floating-system-languages-wrapper"></div>
            <span class="status" />
        </div>


        <% if (ApplicationManager.AppSettings.AllowMultiLanguage)
            { %>
        <div class="setting-item" settingname="LanguageSource">
            <label id="language-selection-label" class="inline auto">
                <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/languageSelection") %><a class="tooltip" title="" href="#"></a>
            </label>
            <select class="autosave alter-tooltip has-additional-options">
                <option value="Prompt" data-type="prompt" title-data="<%= WebTextManager.GetText("/enum/formLanguageSource/tooltip/prompt") %>"><%= WebTextManager.GetText("/enum/formLanguageSource/prompt") %></option>
                <option value="QueryString" data-type="query" title-data="<%= WebTextManager.GetText("/enum/formLanguageSource/tooltip/queryString") %>"><%= WebTextManager.GetText("/enum/formLanguageSource/queryString") %></option>
                <option value="User" data-type="user" title-data="<%= WebTextManager.GetText("/enum/formLanguageSource/tooltip/user") %>"><%= WebTextManager.GetText("/enum/formLanguageSource/user") %></option>
                <option value="Session" data-type="session" title-data="<%= WebTextManager.GetText("/enum/formLanguageSource/tooltip/session") %>"><%= WebTextManager.GetText("/enum/formLanguageSource/session") %></option>
                <option value="Browser" data-type="browser" title-data="<%= WebTextManager.GetText("/enum/formLanguageSource/tooltip/browser") %>"><%= WebTextManager.GetText("/enum/formLanguageSource/browser") %></option>
            </select><span class="status"></span>
            <div class="actions inactive additional-options additional-options-query">
                <div class="setting-item" settingname="LanguageSourceToken">
                    <label class="inline">
                        <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/variableNameQuery") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/tooltip/variableNameQuery") %>" href="#"></a>
                    </label>
                    <input type="text" /><span class="status"></span>
                    <div class="actions inline save-options">
                        <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
                    </div>
                </div>
            </div>
            <div class="actions inactive additional-options additional-options-user">
                <div class="setting-item" settingname="LanguageSourceToken">
                    <label class="inline">
                        <%= WebTextManager.GetText("/enum/formLanguageSource/user") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/tooltip/userAttribute") %>" href="#"></a>
                    </label>
                    <input type="text" /><span class="status"></span>
                    <div class="actions inline save-options">
                        <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
                    </div>
                </div>
            </div>
            <div class="actions inactive additional-options additional-options-session">
                <div class="setting-item" settingname="LanguageSourceToken">
                    <label class="inline">
                        <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/variableNameSession") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/tooltip/variableNameSession") %>" href="#"></a>
                    </label>
                    <input type="text" /><span class="status"></span>
                    <div class="actions inline save-options">
                        <a href="#" class="submit ckbxButton silverButton"><%= WebTextManager.GetText("/common/save") %></a>
                    </div>
                </div>
            </div>
            <div class="actions inactive additional-options additional-options-browser">
                <div class="setting-item" settingname="LanguageSourceToken">
                    <label class="inline">
                        <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/unsuccessBrowserDetection") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/tooltip/browserDetection") %>" href="#"></a>
                    </label>
                    <select class="autosave">
                        <option value="Default Language"><%= WebTextManager.GetText("/enum/formLanguageSource/unsuccessDetectionUseDefault") %></option>
                        <option value="Prompt User"><%= WebTextManager.GetText("/enum/formLanguageSource/unsuccessDetectionPromt") %></option>
                    </select><span class="status"></span>
                </div>
            </div>
        </div>
        <% } %>
        <div class="setting-section-divider"></div>
        <div class="setting-item">
            <label class="inline">
                <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/import") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/importTooltip") %>" href="#"></a>
            </label>
            <span class="status"></span>
            <div class="actions inline save-options">
                <a href="javascript:void(0);" id="importText" class="ckbxButton silverButton"><%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/import") %></a>
            </div>
        </div>
        <div class="setting-item">
            <label class="inline">
                <%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/export") %><a class="tooltip" title="<%= WebTextManager.GetText("/pageText/surveyLanguage.aspx/exportTooltip") %>" href="#"></a>
            </label>
            <span class="status"></span>
            <div class="actions inline save-options">
                <btn:CheckboxButton ID="_exportText" runat="server" CssClass="ckbxButton silverButton" TextId="/pageText/surveyLanguage.aspx/export" />
            </div>
        </div>
        <div class="setting-section-divider"></div> 
        <div class="setting-item header-item">
            <label class="inline auto">Customize Texts Below For:</label>
            <select id="surveyTextLanguageSelect">
            </select>
            <span class="status"></span>
        </div>
        <div id="survey-custom-text">
        </div>


    </div>

    <!-- Terms & links -->
    <div class="setting-section">
        <div class="dashStatsContentHeader">
            <div class="fixed_125 left">
                Name 
            </div>
            <div class="fixed_125 left">
                Term
            </div>
            <div class="fixed_275 left">
                Definition 
            </div>
            <div class="fixed_275 left">
                Link  
            </div>
            <div class="fixed_175 left" style="border-style: none;">
                &nbsp;
            </div>
            <div class="clear">
            </div>
        </div>
        <div id="termsList" class="ui-sortable">
        </div>
        <div id="termTemplate" style="display:none">
             <div class="rowSelect dashStatsContent allMenu detailZebra">
                <div class="fixed_125 left input">
                    <input type="text" style="width: 100px;" class="uniform-input text hover termName">
                </div>
                <div class="fixed_125 left input">
                    <input type="text" style="width: 100px;" class="uniform-input text hover termTerm">
                </div>
                <div class="fixed_275 left input">
                    <input type="text" style="width: 250px;" class="uniform-input text hover termDefinition">
                </div>
                <div class="fixed_275 left input">
                    <input type="text" style="width: 250px;" class="uniform-input text hover termLink">
                </div>
                <div class="fixed_175 left input">
                    <div id="rowButtons_1">
                    </div>
                    <a href="javascript:void(0);" class="deleteRowLink ckbxButton roundedCorners border999 shadow999 redButton OptionEditorButton termAction" >
                        <span>+
                        </span>
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>
<!-- .tabContentContainer -->

<script type="text/javascript">
    var _customUrl = '';

    $(function() {
        updateSurveySettings();

        $('select.autosave.alter-tooltip.has-additional-options').change(function(e) {
            var newTitle = $(this.options[this.selectedIndex]).attr('title-data');
            $($($(this).siblings('label.inline')[0]).children('a.tooltip')).attr('title', newTitle);
        });

        $('[settingName="LanguageSource"] .has-additional-options').on('change', function() {
            var div = $(this).parent().find('.additional-options-' + $(this).find('option:selected').attr('data-type'));
            div.find('input').val('').change();
            div.find('select').val('Default Language').change();
        });

        $('.datetimepicker').datetimepicker({
            beforeShow: function(ele) {
                $(ele).datepicker('widget').css('z-index', 1001);
            },
            onSelect: function() {
                toggleSettingSaveOpts(this, true);
            }
        });
        $('.datetimepicker').datepicker({ defaultDate: null });

        $("input.number").keydown(function(event) {
            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            } else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });

        $('[settingname="MobileStyleID"] select, [settingname="StyleTemplateID"] select').on('change', function() {
            updateMobilePreview($('[settingname="MobileStyleID"] select').val());
        });

        //load permissions iframe only on "advancedPermissions" click
        $('#advancedPermissions .toggle-settings').on('click', function() {
            var iframe = $('#advancedPermissions iframe');
            if (typeof iframe.attr('src') == "undefined" || iframe.attr('src') == '') {
                reloadAcl();
                $('select.autosave.alter-tooltip.has-additional-options').change(function() { reloadAcl(); });
            }
        });
    });

    function reloadAcl() {
        $('#advancedPermissions iframe').attr('src', '<%= ResolveUrl("~/Forms/Surveys/Permissions.aspx") + "?s=" + ResponseTemplateId + "&dialog=true" %>');
    };

    function updateSurveySettings() {
        svcSurveyManagement.getSurveyMetaDataD(_at, <%= ResponseTemplateId %>)
            .then(bindSettings)
            .then(bindClickToEditEvents);
    }

    function bindAvailableLanguageList(surveyMetaData) {
        $('#availableLanguages').empty();
        <%
    //Check for multiLanguage support
    string errorMsg;
    if (Page is LicenseProtectedPage && (Page as LicenseProtectedPage).ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
    { %>
        $('.manageLanguagesArea').hide();
        <% }

    if (TextManager.SurveyLanguages != null)
    {
        foreach (var textCode in TextManager.SurveyLanguages)
        {
            var text = WebTextManager.GetText("/languageText/surveyEditor/" + textCode) + " [" + textCode + "]"; %>
        var checked = false;
        for (var i = 0; i < surveyMetaData.SupportedLanguages.NameValueList.length; i++) {
            if (surveyMetaData.SupportedLanguages.NameValueList[i].Name === '<%= textCode %>') {
                checked = true;
            }
        }
        $('#availableLanguages').append("<li class='availableLanguageListItem'><input type='checkbox' id='lang_<%= textCode %>' value='<%= textCode %>' class='languageCheckbox'><%= text %></input></li>");
        $('#lang_<%= textCode %>').attr('checked', checked);
        <% }
    } %>
        $('.availableLanguageListItem').wrapAll('<ul/>');
        //bind ckicks to language checkboxes
        $('.languageCheckbox').bind('click', function(event) {
            if ($(this).prop('checked')) {
                addDefaultLanguage($(this).attr("value"), <%= ResponseTemplateId %>);
            } else {
                removeDefaultLanguage($(this).attr("value"), <%= ResponseTemplateId %>);
            }
        });

        $('#manageLanguagesButton').bind('click', function(event) {
            event.stopPropagation();
            if (!$('#availableLanguages').is(":visible")) {
                $('#availableLanguages').show();

            } else {
                $('#availableLanguages').hide();
            }
        });
        $('#availableLanguages').hide();

        //Hide  availableLanguages if click on another DOM element detected
        $('html').click(function(event) {
            $('#availableLanguages').hide();
        });
    }

    //add default language to survey
    function addDefaultLanguage(languageCode, surveyId) {
        svcSurveyEditor.addDefaultLanguage(
            _at,
            surveyId,
            languageCode).then(function() {
                var defLangList = document.getElementById('surveyDefaultLanguageList');
                defLangList.options.add(new Option(languageCode, languageCode));
                var defLangTextSelect = document.getElementById('surveyTextLanguageSelect');
                defLangTextSelect.add(new Option(languageCode, languageCode));
            });
    };

    //remove default language from survey settings
    function removeDefaultLanguage(languageCode, surveyId) {
        svcSurveyEditor.removeDefaultLanguage(
            _at,
            surveyId,
            languageCode).then(function() {
                $("#surveyDefaultLanguageList [value='" + languageCode + "']").remove();
                $("#surveyTextLanguageSelect [value='" + languageCode + "']").remove();
            });
    }

    //
    function bindSettings(surveyMetaData) {
        //check "locked" warning
        if (!surveyMetaData.AllowSurveyEditWhileActive) {
            $('.isLocked').show().slideDown(250);
        }
        //show addition option in case of AllowResume is true
        if (surveyMetaData.AllowResume) {
            $('div[togglesettingname="ShowSaveAndQuit"]').show().slideDown(250);
        }
        
        if (surveyMetaData.ShowTopSurveyButtons) {
            $('div[togglesettingname="HideTopSurveyButtonsOnFirstAndLastPage"]').show().slideDown(250);
        }
        if (surveyMetaData.ShowProgressBar) {
            $('div[settingname="ProgressBarOrientation"]').show().slideDown(250);
        }


        //fill default texts
        var defLangList = document.getElementById('surveyDefaultLanguageList');
        var textLangSel = document.getElementById('surveyTextLanguageSelect');
        var lang = surveyMetaData.SupportedLanguages.NameValueList;
        for (var i = 0; i < lang.length; i++) {
            defLangList.options.add(new Option(lang[i].Value, lang[i].Name));
            textLangSel.options.add(new Option(lang[i].Value, lang[i].Name));
        }
        bindAvailableLanguageList(surveyMetaData);
        bindSurveyUrls(surveyMetaData.SurveyUrls);

        //set toggle settings
        $.each($('div[settingName]'), function(ind, elem) {
            var setting = $(elem).attr('settingName');
            var property = surveyMetaData[setting];
            switch (setting) {
                case 'ActivationStartDate':
                case 'ActivationEndDate':
                    property = dateUtils.jsonDateToInvariantDateString(property, "No Restriction", false);
                case 'LanguageSource':
                    if (property == null || property == '') {
                        property = 'Prompt';
                    }
                    var tooltip = $(elem).find('select option[value="' + property + '"]').attr('title');
                    $(elem).find('#language-selection-label').attr('title', tooltip);
                    if (property == 'QueryString') {
                        $('.additional-options-query').show();
                    }
                    if (property == 'User') {
                        $('.additional-options-user').show();
                    }
                    if (property == 'Session') {
                        $('.additional-options-session').show();
                    }
                    if (property == 'Browser') {
                        $('.additional-options-browser').show();
                    }
                case 'MaxTotalResponses':
                case 'MaxResponsesPerUser':
                    if (property == null) property = 'No Limit';
                case 'SecurityType':
                    if (property == 'AccessControlList') {
                        $('.additional-options-acl').show();
                    }
                    if (property == 'PasswordProtected') {
                        $('.additional-options-password').show();
                    }
                case 'StyleTemplateID':
                    if (property == null) property = 0;
                default:
                    $(elem).attr('data-oldValue', property).find('input, select').val(property);
                    break;
            }
        });

        updateSurveyStatus();
        bindCustomText($('#surveyDefaultLanguageList').val());

        //set toggle settings
        $.each($('div[togglesettingname]'), function(ind, elem) {
           
            var setting = $(elem).attr('togglesettingname');
            var property = surveyMetaData[setting];
            if (setting == 'AllowSurveyEditWhileActive' || setting == 'DisableBackButton') {
                if (property) {
                    $(elem).find('.switch').slickswitch('toggleOff');
                    $(elem).find('.ss-on').hide();
                }
            } else if (property == false) {
                $(elem).find('.switch').slickswitch('toggleOff');
                $(elem).find('.ss-on').hide();
            }
        });

        bindTermsList(surveyMetaData.Terms);
    }

    function bindTermsList(terms) {
        window.SurveyTerms = terms;

        for(var i in terms) {
            var $template = $($("#termTemplate").children()[0]).clone();
            $template.attr("termId", terms[i].Id);
            $template.find(".termName").val(terms[i].Name);
            $template.find(".termTerm").val(terms[i].Term);
            $template.find(".termDefinition").val(terms[i].Definition);
            $template.find(".termLink").val(terms[i].Link);

            $template.find(".termAction").bind("click", function() {
                var $term = $(this).parent().parent();
                svcSurveyEditor.updateSurveySettingD(_at, <%= ResponseTemplateId %>, "removeterm", escapeInjections($term.attr("termId"))).then(function(result) {
                    $("#termsList").find(".rowSelect[termid='" + $term.attr("termId") + "']").remove();
                });
            });

            $template.find("input").bind("focusout", function() {
                addOrUpdate($(this), false);
            });
            $template.find("input").bind("focus", function() {
                $(this).data("previous-value", $(this).val());
            });

            $template.find(".termAction").html("-");
            
            if (i % 2 != 0) {
                $template.removeClass("detailZebra");
            }

            $("#termsList").append($template);
        }

        var $template = $($("#termTemplate").children()[0]).clone();
        $template.removeClass("detailZebra");
        
        $template.find(".termAction").bind("click", function() {
            addOrUpdate($(this), true);
        });

        $("#termsList").append("<br/>");
        $("#termsList").append($template);
    }

    function addOrUpdate($input, validateAll) {
        var $row = $input.parent().parent();
        
        if ($row.find(".termName").val().trim() == '' && (validateAll || $input.hasClass("termName"))) {
            alert('Name could not be empty');
            $row.find(".termName").val($row.find(".termName").data("previous-value"));
            return;
        }
        if (/[^\w]/.test($row.find(".termName").val())) {
            alert('Name should contain only alphanumeric characters');
            return;
        }
        if ($row.find(".termTerm").val().trim() == '' && (validateAll || $input.hasClass("termTerm"))) {
            alert('Term could not be empty');
            $row.find(".termTerm").val($row.find(".termTerm").data("previous-value"));
            return;
        }
        if (validateAll && $row.find(".termDefinition").val().trim() == '' && $row.find(".termLink").val().trim() == '') {
            alert('Definition or Link should have value');
            return;
        }
        if(!(($row.find(".termLink").val().indexOf("http://") == 0 || $row.find(".termLink").val().indexOf("https://") == 0) || !$row.find(".termDefinition").val().trim() == '')){
            alert('Link must start with http or https');
            return;
        }

        var jsonValues = JSON.stringify({
            responseTemplateId : '<%= ResponseTemplateId %>',
            id : $row.attr("termId"),
            Name : $row.find(".termName").val(),
            Term : $row.find(".termTerm").val(),
            Definition : $row.find(".termDefinition").val(),
            Link : $row.find(".termLink").val(),
        });

        svcSurveyEditor.updateSurveySettingD(_at, <%= ResponseTemplateId %>, "term", escapeInjections(jsonValues)).then(function(result) {
            if (parseInt(result) != 0) {
                var term = JSON.parse(jsonValues);
                var $template = $($("#termTemplate").children()[0]).clone();
                $template.attr("termId", result);
                $template.find(".termName").val(term.Name);
                $template.find(".termTerm").val(term.Term);
                $template.find(".termDefinition").val(term.Definition);
                $template.find(".termLink").val(term.Link);

                $template.find(".termAction").bind("click", function() {
                    var $term = $(this).parent().parent();
                    svcSurveyEditor.updateSurveySettingD(_at, <%= ResponseTemplateId %>, "removeterm", escapeInjections($term.attr("termId"))).then(function(result) {
                        $("#termsList").find(".rowSelect[termid='" + $term.attr("termId") + "']").remove();
                    });
                });

                $template.find("input").bind("focusout", function() {
                    addOrUpdate($(this), false);
                });

                $template.find(".termAction").html("-");

                if ( $("#termsList").find(".rowSelect").size() % 2 == 0) {
                    $template.removeClass("detailZebra");
                }

                $("#termsList").find(".rowSelect").last().prev().before($template);

                var $cleatAdd = $("#termsList").find(".rowSelect").last();
                $cleatAdd.find(".termName").val("");
                $cleatAdd.find(".termTerm").val("");
                $cleatAdd.find(".termDefinition").val("");
                $cleatAdd.find(".termLink").val("");
            }
        });
    }

    //
    function bindClickToEditEvents(surveyMetaData) {
        //Check to see whether titles should be set or edit buttons enabled
        svcAuthorization
            .authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, <%= ResponseTemplateId %>, 'Form.Edit')
            .then(function(result) {
                //false result = no access, so do nothing
                if (!result) {
                    return;
                }

                var toggleTitle = textHelper.getTextValue("/controlText/surveyDashboard/clickToToggle", "Click to toggle");

                $('div[toggleSettingName] a.switch')
                    .attr('title', toggleTitle)
                    .bind('click', toggleSurveySetting);

                $('div[settingName] .submit')
                    .bind('click', updateSurveySetting);

                $('div[settingName] select.autosave ')
                    .bind('change', updateSurveySetting);

                $('div[settingName] .cancel')
                    .bind('click', cancelSurveySetting);

                $('#accessList').bind('click', accesListClick);
                $('#importText').bind('click', importTextClick);
            });
        }

        //
        function toggleSurveySetting() {
            var settingName = $(this).parent('div[toggleSettingName]').attr('toggleSettingName');

            if (settingName == null || settingName == '') {
                return;
            }

            svcSurveyEditor.toggleSurveySettingD(
                    _at,
                    <%= ResponseTemplateId %>,
                settingName,
                $(this))
            .then(function(result) {
                switch (settingName) {
                    case 'AllowSurveyEditWhileActive':
                        if (result) {
                            $('.isLocked').slideUp(250);
                        } else {
                            $('.isLocked').show().slideDown(250);
                        }
                        break;
                    case 'AllowResume':
                        if (!result) {
                            $('div[togglesettingname="ShowSaveAndQuit"]').slideUp(250);
                        } else {
                            $('div[togglesettingname="ShowSaveAndQuit"]').show().slideDown(250);
                        }
                        break;
                    case 'ShowProgressBar':
                        if (!result) {
                            $('div[settingname="ProgressBarOrientation"]').slideUp(250);
                        } else {
                            $('div[settingname="ProgressBarOrientation"]').show().slideDown(250);
                        }
                        break;
                    case 'ShowTopSurveyButtons':
                        if (!result) {
                            $('div[togglesettingname="HideTopSurveyButtonsOnFirstAndLastPage"]').slideUp(250);
                        } else {
                            $('div[togglesettingname="HideTopSurveyButtonsOnFirstAndLastPage"]').show().slideDown(250);
                        }
                        break;
                    case 'IsActive':
                        updateSurveyStatus();
                        break;
                }
            }).fail(function(result) {});
        }

        //
        function updateSurveySetting() {
            var parent = $(this).parents('div[settingName]')[0];
            var settingName = $(parent).attr('settingName');
            var settingInput = $(parent).find('select').first();
            if (settingInput.length == 0) {
                settingInput = $(parent).find('input').first();
            }

            var newValue = settingInput.val();

            var dateTimePicker = settingInput.filter('.forDatepicker');
            if (dateTimePicker.length > 0) {
                newValue = dateTimePicker.val();
            }

            var oldValue = $(parent).attr('data-oldValue');

            if (settingName == null || settingName == '' || newValue == oldValue) {
                return;
            }

            svcSurveyEditor.updateSurveySettingD(
                    _at,
                    <%= ResponseTemplateId %>,
                settingName,
                escapeInjections(newValue))
            .then(function(result) {
                if (dateTimePicker.length > 0) {
                    if (result != '') {
                        var date = new Date(result);
                        settingInput.filter('.date').datepicker('setDate', date);
                    } else
                        settingInput.val("No Restriction");
                } else {
                    settingInput.val(result == null ? "No Restriction" : result);
                }
                $(parent).attr('data-oldValue', result);
                updateSurveyStatus();
            })
            .fail(function(result) {
                $(parent).find('.status').addClass('error');
                settingInput.val(oldValue);
                showError(parent, result.FailureMessage);
            });
        }

        function cancelSurveySetting() {
            var parent = $(this).parents('div[settingName]')[0];
            var settingInput = $(parent).find('input');
            var oldValue = $(parent).attr('data-oldValue');
            settingInput.val(oldValue);
        }

        function showError(element, errorText) {
            var errorDiv = $('<div class="error message inline" style="margin-left:10px;display: none;">' + errorText + '</div>');
            $(element).after(errorDiv);
            errorDiv.slideDown(250);
            setTimeout(function() { errorDiv.slideUp(250); }, 4000);
        }

        function accesListClick(e) {
            var link = '<%= ResolveUrl("~/Forms/Surveys/Respondents.aspx") + "?s=" + ResponseTemplateId %>' + '&onClose=onDialogClosed';
            showDialog(link, 'security');
            e.preventDefault();
        }

        function importTextClick(e) {
            var link = '<%= ResolveUrl("~/Forms/Surveys/Languages.aspx") + "?s=" + ResponseTemplateId %>' + '&onClose=onDialogClosed';
            showDialog(link, 250, 400, 'security');
            e.preventDefault();
        }

    function updateSurveyStatus() {
            svcSurveyEditor.getStatus(
                    _at,
                    <%= ResponseTemplateId %>)
            .then(function(result) {
                $('#surveyStatusDate').text(result);
            });
        }

        //
        //Survey text functions
        //
    function bindCustomText(selectedLanguage) {
            svcSurveyEditor
                .getLocalizableTextsD(
                    _at,
                    <%= ResponseTemplateId %>,
                selectedLanguage
            )
            .then(function(result) {
                bindSurveyTextTemplate(result, selectedLanguage);
            });
    }

    //
    function bindSurveyTextTemplate(textData, selectedLanguage) {
        //Bind to template
        var textTemplateData = { Texts: textData };

        templateHelper.loadAndApplyTemplateD(
                'surveyTextTemplate.html',
                '<%= ResolveUrl("~/Forms/jqtmpl/surveyTextTemplate.html") %>',
                textTemplateData,
                null,
                'survey-custom-text',
                true)
            .then(bindSurveyTextClick)
            .then($('#surveyTextLanguageSelect').val(selectedLanguage));
            }

            //
    function bindSurveyTextClick() {
                //Check to see whether titles should be set or edit buttons enabled
                svcAuthorization
                    .authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, <%= ResponseTemplateId %>, 'Form.Administer')
            .then(function(result) {
                //false result = no access, so do nothing
                if (!result) {
                    return;
                }
                //Set titles & click events
                var editTitle = textHelper.getTextValue("/controlText/surveyDashboard/clickToEdit", "Click to edit");

                $.each($('div[customtextid]'), function(index, elem) {
                    var val = $(elem).find('input').val();
                    $(elem).attr('data-oldValue', val);
                    $(elem).find('.submit').bind('click', updateSurveyTextValue);
                    $(elem).find('.cancel').bind('click', cancelSurveyText);
                });

                $('#surveyTextLanguageSelect').change(function() { bindCustomText($(this).val()); });
            });
            }

            //
    function updateSurveyTextValue() {
                var parent = $(this).parents('div[customtextid]')[0];
                var textId = $(parent).attr('customtextid');
                var settingInput = $(parent).find('input');

                var newValue = settingInput.val();
                var oldValue = $(parent).attr('data-oldValue');

                if (newValue == oldValue) {
                    return;
                }

                svcSurveyEditor.updateSurveyTextD(
                        _at,
                        <%= ResponseTemplateId %>,
                textId,
                newValue,
                $('#surveyTextLanguageSelect').val())
            .then(function(result) {
                $(parent).attr('data-oldValue', newValue);
            });
            }

            function cancelSurveyText() {
                var parent = $(this).parents('div[customtextid]')[0];
                var settingInput = $(parent).find('input');
                var oldValue = $(parent).attr('data-oldValue');
                settingInput.val(oldValue);
            }

            //
            //Survey url functions
            // 
            function bindSurveyUrls(urls) {
                $('#saveNewAltUrlBtn')
                    .unbind('click')
                    .bind('click', onUpdateSurveyUrl);
                $('#cancelNewAltUrlBtn')
                    .unbind('click')
                    .bind('click', onCancelSurveyUrl);
                $('#newAltUrlExtension')
                    .unbind('change')
                    .bind('change', function() { toggleSettingSaveOpts($(this), true); });

                //set default url
                $('.actions .url-default').attr('href', urls[0]);
                $('.static.url-default').html(urls[0]);

                //set custom url
                if (urls.length > 1)
                    _customUrl = urls[1];
                $('.actions .url-custom').attr('href', _customUrl);
                $('.static.url-custom').html('<%= ApplicationManager.ApplicationPath %>/');
                setAltUrlExtension(true);
            }

            //
            function onCancelSurveyUrl() {
                setAltUrlExtension(false);
                toggleSettingSaveOpts($(this), false);
            }

            //
            function setAltUrlExtension(addOldValue) {
                var urlParts = splitCustomUrl(_customUrl);
                var urlCustom = $('[type="text"].url-custom');
                urlCustom.val(urlParts[0]);
                if (addOldValue)
                    urlCustom.parent().attr('data-oldValue', urlParts[0]);

                var sel = $('#newAltUrlExtension');
                if (urlParts[1] && urlParts[1] != '')
                    sel.val(urlParts[1]);
                else
                    sel.val(sel.find('option:first').val());
            }

            //
            function onUpdateSurveyUrl() {
                //Hide errors
                $('.url-errors-list div.error').hide();
                $('#saveNewAltUrlBtn').hide();
                $('#cancelNewAltUrlBtn').hide();

                //Get new Url.  Blank url = remove mapping
                var newUrl = $('[type="text"].url-custom').val() + $('#newAltUrlExtension').val();

                //Attempt to update the URL -
                svcSurveyEditor.setAlternateUrlD(
                        _at,
                        <%= ResponseTemplateId %>,
                newUrl)
            .then(function(result) {
                $('#saveNewAltUrlBtn').show();
                $('#cancelNewAltUrlBtn').show();

                if (result == "URL_UPDATE_SUCCESS") {
                    if (newUrl != '') {
                        _customUrl = '<%= ApplicationManager.ApplicationPath %>/' + newUrl;
                    }
                    $('.actions .url-custom').attr('href', _customUrl);
                    $('[type="text"].url-custom').parent().attr('data-oldValue', $('[type="text"].url-custom').val());
                } else { //Show the error
                    $('#url-custom-status').addClass('error');
                    var error = $('#' + result + '_Error').slideDown(250);
                    setTimeout(function() { error.slideUp(250); }, 4000);
                }
            });
        }

        function initializeMobilePreview() {
            var prev = $('#appearance-preview-wrapper iframe');
            if (prev && (typeof prev.attr('src') == 'undefined' || prev.attr('src') == '')) {
                var themeId = $('[settingname="MobileStyleID"] select').val();
                var src = 'Preview.aspx?preview=true&s=<%= ResponseTemplateId %>&loc=<%= ResponseTemplate.LanguageSettings.DefaultLanguage %>&mode=SurveyMobilePreview&theme=' + themeId;
                prev.attr('src', src);
            }
        }

        //update appearance preview
        function updateMobilePreview(mobileThemeId) {
            var prev = $('#appearance-preview-wrapper iframe');

            if (prev && typeof mobileThemeId != 'undefined') {
                var src = prev.attr('src');

                //replace mobile theme
                var ind = src.indexOf('&theme');
                if (ind >= 0) {
                    var subStringToRep = src.substring(ind, src.length);
                    src = src.replace(subStringToRep, '&theme=' + mobileThemeId);
                } else {
                    src += '&theme=' + mobileThemeId;
                }
                prev.attr('src', src);
            }
        }

        function splitCustomUrl(url) {
            //Figure out URL extension and file name
            var slashIndex = url.lastIndexOf('/');
            var dotIndex = url.lastIndexOf('.');

            var extension = '';
            var fileName = '';

            if (slashIndex > 0 && dotIndex > 0) {
                //Get extension
                extension = url.substring(dotIndex);
                fileName = url.substring(slashIndex + 1, dotIndex);
            }

            var urls = new Array(3);
            urls[0] = fileName;
            urls[1] = extension;
            return urls;
        }
</script>
