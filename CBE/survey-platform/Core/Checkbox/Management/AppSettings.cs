using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Management.Licensing.Limits.Static;
using Prezza.Framework.Data;
using Checkbox.Common;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management
{
    /// <summary>
    /// Contains the Property accessors for all application-wide settings
    /// </summary>
    public class AppSettings
    {
        #region Methods

        /// <summary>
        /// Get a setting value
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="settingName">Name of the setting</param>
        /// <param name="defaultValue">Default value of the setting</param>
        /// <returns></returns>
        public T GetValue<T>(string settingName, T defaultValue)
        {
            try
            {
                object settingValue = ApplicationManager.GetAppSetting(settingName);

                if (settingValue == null)
                {
                    return defaultValue;
                }


                if (typeof(T).IsEnum)
                {
                    return (T)(Enum.Parse(typeof(T), (string)settingValue));
                }

                if (typeof(T) == typeof(Int32))
                {
                    return (T)(object)Convert.ToInt32(settingValue);
                }

                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)Convert.ToBoolean(settingValue);
                }

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)Convert.ToString(settingValue);
                }

                if (typeof(T) == typeof(double))
                {
                    return (T)(object)Convert.ToDouble(settingValue);
                }

                if (typeof(T) == typeof(float))
                {
                    return (T)(object)float.Parse((string)settingValue);
                }

                return (T)settingValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set the setting value
        /// </summary>
        /// <typeparam name="T">Setting Type</typeparam>
        /// <param name="settingName">Name of the setting</param>
        /// <param name="settingValue">Value for the setting</param>
        /// <returns></returns>
        public void SetValue<T>(string settingName, T settingValue)
        {
            try
            {
                if (ApplicationManager.GetAppSetting(settingName) == null)
                {
                    ApplicationManager.AddNewAppSetting(
                        settingName,
                        settingValue != null
                            ? settingValue.ToString()
                            : string.Empty
                    );
                }
                else
                {
                    ApplicationManager.UpdateAppSetting(
                        settingName,
                        settingValue != null
                            ? settingValue.ToString()
                            : string.Empty
                    );
                }
            }
            catch (Exception ex)
            {
                //suppress exception
            }
        }


        #endregion

        #region DB Settings

        /// <summary>
        /// Get/set whether to enable single installation and multi-database operation.
        /// </summary>
        public bool UseReportingService
        {
            get { return GetValue("UseReportingService", false); }
            set { SetValue("UseReportingService", value); }
        }

        /// <summary>
        /// Specify whether running in web farm or not.
        /// </summary>
        public bool WebFarm
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebFarm"]))
                {
                    bool value;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["WebFarm"], out value))
                    {
                        return value;
                    }
                }

                return false;
            }
        }


        /// <summary>
        /// Get whether to enable single installation and multi-database operation.
        /// </summary>
        public bool EnableMultiDatabase
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableMultiDatabase"]))
                {
                    bool value;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["EnableMultiDatabase"], out value))
                    {
                        return value;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Get environment name to place it to footer
        /// </summary>
        public string EnvironmentName
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnvironmentName"]))
                {
                    return ConfigurationManager.AppSettings["EnvironmentName"];
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get whether to enable ssl offloading.
        /// </summary>
        public bool EnableSslOffloading
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableSslOffloading"]))
                {
                    bool value;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["EnableSslOffloading"], out value))
                    {
                        return value;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Get/set whether to enable single installation and multi-database operation.
        /// </summary>
        public bool InstallSuccess
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["InstallSuccess"]))
                {
                    bool value;

                    if (Boolean.TryParse(ConfigurationManager.AppSettings["InstallSuccess"], out value))
                    {
                        return value;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Get/set whether to show the preview watermark for report item previews.
        /// </summary>
        public bool ShowReportItemPreviewWatermark
        {
            get { return GetValue("ShowReportItemPreviewWatermark", true); }
            set { SetValue("ShowReportItemPreviewWatermark", value); }
        }

        /// <summary>
        /// Get/set maximum number of options per item to include in report previews
        /// </summary>
        public int MaxReportPreviewOptions
        {
            get { return GetValue("MaxReportPreviewOptions", 10); }
            set { SetValue("MaxReportPreviewOptions", value); }
        }

        /// <summary>
        /// Length of time a response workflow may be idle before it
        /// is unloaded.
        /// </summary>
        public int WorkflowInstanceUnloadTime
        {
            get { return GetValue("WorkflowInstanceUnloadTime", 5); }
            set { SetValue("WorkflowInstanceUnloadTime", value); }
        }

        /// <summary>
        /// Length of time a response workflow may be idle before it
        /// is unloaded.
        /// </summary>
        public bool PreventWorkflowLocking
        {
            get { return GetValue("PreventWorkflowLocking", true); }
            set { SetValue("PreventWorkflowLocking", value); }
        }

        /// <summary>
        /// Get/set the number of lines to export at a time before flushing
        /// the Response output buffer when exporting response data.  Note that 
        /// ASP.NET may cause flushing earlier, so this is technically the
        /// MAX number of lines to write before flushing.
        /// </summary>
        public int ResponseDataExportChunkSize
        {
            get { return GetValue("ResponseDataExportChunkSize", 50); }
            set { SetValue("ResponseDataExportChunkSize", value); }
        }

        /// <summary>
        /// Get/set the duration (in minutes) of the login ticket created when a
        /// respondent completes a survey and is redirected to particular report
        /// or the view response page.
        /// The ticket is used to grant access to the report, regardless of the
        /// reports security settings.
        /// </summary>
        public int ViewReportTicketDuration
        {
            get { return GetValue("ViewReportTicketDuration", 30); }
            set { SetValue("ViewReportTicketDuration", value); }
        }

        /// <summary>
        /// Get/set whether to set the default button on a survey.
        /// </summary>
        public bool SetSurveyDefaultButton
        {
            get { return GetValue("SetSurveyDefaultButton", true); }
            set { SetValue("SetSurveyDefaultButton", value); }
        }

        /// <summary>
        /// Get/set whether to show invitation opt out functionality
        /// </summary>
        public bool EnableOptOutScreen
        {
            get { return GetValue("EnableOptOutScreen", true); }
            set { SetValue("EnableOptOutScreen", value); }
        }

        /// <summary>
        /// Get/set "greetings" text which could be shown to the user on opt out screen
        /// </summary>
        public string OptOutScreenGreetingsText
        {
            get { return GetValue("OptOutScreenGreetingsText", string.Empty); }
            set { SetValue("OptOutScreenGreetingsText", value); }
        }

        /// <summary>
        /// Get/set "user's reasons" text which could be shown to the user on opt out screen
        /// </summary>
        public string OptOutScreenUserReasonsText
        {
            get { return GetValue("OptOutScreenUserReasonsText", string.Empty); }
            set { SetValue("OptOutScreenUserReasonsText", value); }
        }

        /// <summary>
        /// Get/set "thank you" text which could be shown to the user on opt out screen
        /// </summary>
        public string OptOutScreenThankYouText
        {
            get { return GetValue("OptOutScreenThankYouText", string.Empty); }
            set { SetValue("OptOutScreenThankYouText", value); }
        }

        /// <summary>
        /// Get/set whether to set the default button on a survey.
        /// </summary>
        public bool IncludeIncompleteResponsesToTotalAmount
        {
            get { return GetValue("IncludeIncompleteResponsesToTotalAmount", true); }
            set { SetValue("IncludeIncompleteResponsesToTotalAmount", value); }
        }

        /// <summary>
        /// Get/set the password for smtp email authentication
        /// </summary>
        public string SmtpPassword
        {
            get { return GetValue("SmtpPassword", string.Empty); }
            set { SetValue("SmtpPassword", value); }
        }

        /// <summary>
        /// Get/set the username to use for Smtp authentication
        /// </summary>
        public string SmtpUserName
        {
            get { return GetValue("SmtpUserName", string.Empty); }
            set { SetValue("SmtpUserName", value); }
        }

        /// <summary>
        /// Get/set whether the connection to the STMP server uses authentication or not
        /// </summary>
        public bool EnableSmtpAuthentication
        {
            get { return GetValue("EnableSmtpAuthentication", false); }
            set { SetValue("EnableSmtpAuthentication", value); }
        }

        /// <summary>
        /// Get/set the SMTP port for the mail server
        /// </summary>
        public int SmtpPort
        {
            get { return GetValue("SmtpPort", 25); }
            set { SetValue("SmtpPort", value); }
        }

        /// <summary>
        /// Get/set whether to use SSL for smtp connections
        /// </summary>
        public bool EnableSmtpSsl
        {
            get { return GetValue("EnableSmtpSsl", false); }
            set { SetValue("EnableSmtpSsl", value); }
        }

        /// <summary>
        /// Get/set user name for Messaging Service
        /// </summary>
        public string MessagingServiceUserName
        {
            get { return GetValue("MessagingServiceUserName", ""); }
            set { SetValue("MessagingServiceUserName", value); }
        }

        /// <summary>
        /// Get/set password for Messaging Service
        /// </summary>
        public string MessagingServicePassword
        {
            get { return GetValue("MessagingServicePassword", ""); }
            set { SetValue("MessagingServicePassword", value); }
        }

        /// <summary>
        /// Get/set whether survey URL rewriting is enabled
        /// </summary>
        public bool AllowSurveyUrlRewriting
        {
            get { return GetValue("AllowSurveyUrlRewriting", false); }
            set { SetValue("AllowSurveyUrlRewriting", value); }
        }

        /// <summary>
        /// Get/set whether users are allowed to edit their own information by clicking on the Welcome: UserName link.
        /// </summary>
        public bool AllowEditSelf
        {
            get { return GetValue("AllowEditSelf", true); }
            set { SetValue("AllowEditSelf", value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating if upload items can be used
        /// </summary>
        public bool EnableUploadItem
        {
            get { return GetValue("EnableUploadItem", true); }
            set { SetValue("EnableUploadItem", value); }
        }

        /// <summary>
        /// Indicates if Javascript item is allowed by license.
        /// </summary>
        public bool AllowJavascriptItem
        {
            get
            {
                string message;
                var limit = new JavascriptItemLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicates if autocomplete remote source is allowed by license.
        /// </summary>
        public bool AllowAutocompleteRemoteSource
        {
            get
            {
                return false;

                string message;
                var limit = new AutocompleteRemoteSourceLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating if javascript items can be used
        /// </summary>
        public bool EnableJavascriptItem
        {
            get
            {
                return GetValue("EnableJavascriptItem", false); 
            }
            set { SetValue("EnableJavascriptItem", value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating that exporting file uploads should be restricted to
        /// only users in the System Administrator role.
        /// </summary>
        public bool RestrictUploadFileExport
        {
            get { return GetValue("RestrictUploadFileExport", true); }
            set { SetValue("RestrictUploadFileExport", value); }
        }

        /// <summary>
        /// Get/set whether response templates should be cached to enhance
        /// </summary>
        public bool CacheResponseTemplates
        {
            get { return GetValue("EnableResponseTemplateCaching", false); }
            set { SetValue("EnableResponseTemplateCaching", value); }
        }

        /// <summary>
        /// Get/set whether to limit maximum email message line length
        /// </summary>
        public bool IsInvitationTextEnabled
        {
            get { return GetValue("IsInvitationTextEnabled", false); }
            set { SetValue("IsInvitationTextEnabled", value); }
        }

        /// <summary>
        /// Get/set whether to limit maximum email message line length
        /// </summary>
        public bool LimitEmailMessageLineLength
        {
            get { return GetValue("InsertLineBreaksInEmails", false); }
            set { SetValue("InsertLineBreaksInEmails", value); }
        }

        /// <summary>
        /// Get set the maximum length for email message lines
        /// </summary>
        public Int32 MaxEmailMessageLineLength
        {
            get { return GetValue("MaximumEmailLineLength", 2000); }
            set { SetValue("MaximumEmailLineLength", value); }
        }

        /// <summary>
        /// Get set the messaging service mode
        /// </summary>
        public string MSSMode
        {
            get { return GetValue("MSSMode", EnableMultiDatabase ? "SES" : "SMTP"); }
            set { SetValue("MSSMode", value); }
        }


        /// <summary>
        /// Get the upper bound for constructing identity query "in" clauses
        /// </summary>
        public int MaxIdentityInClauseSize
        {
            get { return GetValue("MaxIdentityInClauseSize", 1000); }
            set { SetValue("MaxIdentityInClauseSize", value); }
        }

        /// <summary>
        /// Get/set whether to show javascript confirmation popups when deleting pages or items
        /// </summary>
        public bool ShowDeleteConfirmationPopups
        {
            get { return GetValue("ShowConfirmationPopups", true); }
            set { SetValue("ShowConfirmationPopups", value); }
        }

        /// <summary>
        /// Get/set the prefix for pipes
        /// </summary>
        public string PipePrefix
        {
            get { return GetValue("PipePrefix", "@@"); }
            set { SetValue("PipePrefix", value); }
        }

        /// <summary>
        /// Get/set the prefix for pipes
        /// </summary>
        public string TermPrefix
        {
            get { return GetValue("TermPrefix", "%%"); }
            set { SetValue("TermPrefix", value); }
        }

        /// <summary>
        /// Gets and sets whether to show the left hand navigation menu to users who have not been authenticated
        /// </summary>
        public bool ShowNavWhenNotAuthenticated
        {
            get { return GetValue("ShowNavWhenNotAuthenticated", true); }
            set { SetValue("ShowNavWhenNotAuthenticated", value); }
        }

        /// <summary>
        /// Get/set whether to display the list of publicly available surveys
        /// </summary>
        public bool DisplayAvailableSurveyList
        {
            get { return GetValue("DisplayAvailableSurveyList", false); }
            set { SetValue("DisplayAvailableSurveyList", value); }
        }

        /// <summary>
        /// Get/set whether to display the list of publicly available reports
        /// </summary>
        public bool DisplayAvailableReportList
        {
            get { return GetValue("DisplayAvailableReportList", false); }
            set { SetValue("DisplayAvailableReportList", value); }
        }

        /// <summary>
        /// Get/set whether to include incomplete responses in reporting
        /// </summary>
        public bool ReportIncompleteResponses
        {
            get { return GetValue("ReportIncompleteResponses", false); }
            set { SetValue("ReportIncompleteResponses", value); }
        }

        /// <summary>
        /// Get/set whether to include test responses in reporting
        /// </summary>
        public bool ReportTestResponses
        {
            get { return GetValue("ReportTestResponses", true); }
            set { SetValue("ReportTestResponses", value); }
        }

        /// <summary>
        /// Get/set whether to log error information to the UI when an unhandled error occurs
        /// </summary>
        public bool LogErrorsToUI
        {
            get { return GetValue("LogErrorsToUI", true); }
            set { SetValue("LogErrorsToUI", value); }
        }


        /// <summary>
        /// Get/sets whether to log stack trace information to the UI when an unhandled error occurs
        /// </summary>
        public bool LogStackTraceToUI
        {
            get { return GetValue("LogStackTraceToUI", true); }
            set { SetValue("LogStackTraceToUI", value); }

        }

        /// <summary>
        /// Get/sets whether server variables are logged to the UI when an unhandled error occurs
        /// </summary>
        public bool LogServerParamsToUI
        {
            get { return GetValue("LogServerParamsToUI", true); }
            set { SetValue("LogServerParamsToUI", value); }
        }

        /// <summary>
        /// Get/set the preview path for uploaded images
        /// </summary>
        public string ImageUploadPreviewPath
        {
            get { return GetValue("ImageUploadPreviewPath", string.Empty); }
            set { SetValue("ImageUploadPreviewPath", value); }
        }

        /// <summary>
        /// Get/set the preview url for upload images
        /// </summary>
        public string ImageUploadPreviewUrl
        {
            get { return GetValue("ImageUploadPreviewUrl", string.Empty); }
            set { SetValue("ImageUploadPreviewUrl", value); }
        }

        /// <summary>
        /// Gets and sets the path to the image used in the header logo of the application
        /// </summary>
        public string HeaderLogo
        {
            get { return GetValue("HeaderLogo", "~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png"); }
            set { SetValue("HeaderLogo", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [company signature enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [company signature enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool CompanySignatureEnabled
        {
            get { return GetValue("CompanySignatureEnabled", false); }
            set { SetValue("CompanySignatureEnabled", value); }
        }

        /// <summary>
        /// Gets and sets the path to the image used in the header logo of the application
        /// </summary>
        public string CompanySignatureImageUrl
        {
            get { return GetValue("CompanySignatureImageUrl", string.Empty); }
            set { SetValue("CompanySignatureImageUrl", value); }
        }

        /// <summary>
        /// Gets and sets the default view when editing a Form (All Pages, Single Page)
        /// </summary>
        public EditSurveyView DefaultEditView
        {
            get { return GetValue("DefaultEditView", EditSurveyView.IndividualPages); }
            set { SetValue("DefaultEditView", value); }
        }

        /// <summary>
        /// Get/set whether to show preview graphs in the analysis editor
        /// </summary>
        public bool ShowPreviewInAnalysis
        {
            get { return GetValue("ShowPreviewInAnalysis", false); }
            set { SetValue("ShowPreviewInAnalysis", value); }
        }

        /// <summary>
        /// Get/set whether to use the enhanced HTML editor
        /// </summary>
        public bool UseHTMLEditor
        {
            get { return GetValue("UseHTMLEditor", true); }
            set { SetValue("UseHTMLEditor", value); }
        }

        /// <summary>
        /// Prevent session id reuse
        /// </summary>
        public bool PreventSessionReuse
        {
            get { return GetValue("PreventSessionReuse", false); }
            set { SetValue("PreventSessionReuse", value); }
        }

        /// <summary>
        /// Get/set whether or not HTML is displayed as plain text
        /// </summary>
        public bool DisplayHtmlItemsAsPlainText
        {
            get { return GetValue("DisplayHtmlItemsAsPlainText", false); }
            set { SetValue("DisplayHtmlItemsAsPlainText", value); }
        }

        /// <summary>
        /// Get/set whether to use HTML editor by default.
        /// </summary>
        /// <remarks>Valid values are "HTML" or "TEXT"</remarks>
        public string DefaultQuestionEditorView
        {
            get { return GetValue("DefaultQuestionEditorView", "HTML"); }
            set { SetValue("DefaultQuestionEditorView", value); }
        }

        /// <summary>
        /// Gets and sets the default entry type for adding item options
        /// </summary>
        public OptionEntryType DefaultOptionEntryType
        {
            get { return GetValue("DefaultOptionEntryType", OptionEntryType.Normal); }
            set { SetValue("DefaultOptionEntryType", value); }
        }

        /// <summary>
        /// Gets and sets the number of email invitations to send at once
        /// </summary>
        public int MessageThrottle
        {
            get { return GetValue("MessageThrottle", 25); }
            set { SetValue("MessageThrottle", value); }
        }

        /// <summary>
        /// Get/set the wait time between batches of emails
        /// </summary>
        public int MessageThrottleDelay
        {
            get { return GetValue("MessageThrottleDelay", 1); }
            set { SetValue("MessageThrottleDelay", value); }
        }

        /// <summary>
        /// Get/set clients time zone for current application. It is used to show proper time stamp values
        /// for users.
        /// </summary>
        public double TimeZone
        {
            get { return GetValue("TimeZone", ApplicationManager.ServersTimeZone); }
            set { SetValue("TimeZone", value); }
        }

        /// <summary>
        /// Determine if time zone for the system is set or not.
        /// </summary>
        public bool IsTimeZoneSet
        {
            get { return Math.Abs(GetValue("TimeZone", 10000.0) - 10000) < 0.1; }
        }

        /// <summary>
        /// Get/set whether to store uploaded images in the database
        /// </summary>
        public bool StoreImagesInDatabase
        {
            get { return GetValue("StoreImagesInDatabase", true); }
            set { SetValue("StoreImagesInDatabase", value); }
        }

        /// <summary>
        /// Get/set whether surveys can be accessed by database ID in addition to by GUID
        /// </summary>
        public bool AllowResponseTemplateIDLookup
        {
            get { return GetValue("AllowResponseTemplateIDLookup", true); }
            set { SetValue("AllowResponseTemplateIDLookup", value); }
        }


        /// <summary>
        /// Get/set whether prevent Admin auto login
        /// </summary>
        public bool PreventAdminAutoLogin
        {
            get { return GetValue("PreventAdminAutoLogin", false); }
            set { SetValue("PreventAdminAutoLogin", value); }
        }

        /// <summary>
        /// Get/set whether redirect http requests to https
        /// </summary>
        public bool RedirectHTTPtoHTTPS
        {
            get { return GetValue("RedirectHTTPtoHTTPS", false); }
            set { SetValue("RedirectHTTPtoHTTPS", value); }
        }
        
        /// <summary>
        /// Get/set whether security mode is simple
        /// </summary>
        public bool SimpleSecurity
        {
            get { return GetValue("SimpleSecurity", false); }
            set { SetValue("SimpleSecurity", value); }
        }

        /// <summary>
        /// Get/set whether survey session key can be stored in HTTP session as a backup
        /// to view state.
        /// </summary>
        public bool StoreSessionKeyInHttpSesssion
        {
            get { return GetValue("StoreSessionKeyInHttpSesssion", true); }
            set { SetValue("StoreSessionKeyInHttpSesssion", value); }
        }

        /// <summary>
        /// Gets and sets whether to log the network user name of users responding to anonymous surveys
        /// </summary>
        public bool LogNetworkUser
        {
            get { return GetValue("LogNetworkUser", true); }
            set { SetValue("LogNetworkUser", value); }
        }

        /// <summary>
        /// Gets and sets whether to log the respondent's ip addresses
        /// </summary>
        public bool LogIpAddresses
        {
            get { return GetValue("LogIpAddresses", true); }
            set { SetValue("LogIpAddresses", value); }
        }

        /// <summary>
        /// Gets and sets whether to encrypt user passwords
        /// </summary>
        public bool UseEncryption
        {
            get { return GetValue("UseEncryption", false); }
            set { SetValue("UseEncryption", value); }
        }

        /// <summary>
        /// Gets and sets whether to use the javascript datepicker in the application.  Used to limit incompatibility
        /// </summary>
        public bool UseDatePicker
        {
            get { return GetValue("UseDatePicker", true); }
            set { SetValue("UseDatePicker", value); }
        }

        /// <summary>
        /// Gets and sets whether email is enabled for this application
        /// </summary>
        public bool EmailEnabled
        {
            get
            {
                return ApplicationManager.AppSettings.EnableMultiDatabase || this.GetValue("EmailEnabled", true);
            }
            set { SetValue("EmailEnabled", value); }
        }

        /// <summary>
        /// Gets and sets whether respondent's are able to email the survey resume url to themselves
        /// </summary>
        public bool EmailSurveyResumeUrlEnabled
        {
            get { return EmailEnabled && GetValue("EmailSurveyResumeUrlEnabled", true); }
            set { SetValue("EmailSurveyResumeUrlEnabled", value); }
        }


        /// <summary>
        /// Gets and sets whether email is in test mode or not. When in test mode no emails are sent.
        /// </summary>
        public bool EmailTestModeEnabled
        {
            get { return GetValue("EmailTestModeEnabled", false); }
            set { SetValue("EmailTestModeEnabled", value); }
        }

        /// <summary>
        /// Gets and sets the amount of time that the thread sleeps when EmailTestModeEnabled is enabled.
        /// </summary>
        public int EmailTestModeSleepTime
        {
            get { return GetValue("EmailTestModeSleepTime", 1000); }
            set { SetValue("EmailTestModeSleepTime", value); }
        }

        /// <summary>
        /// Gets and sets whether to allow public registration of Users
        /// </summary>
        public bool AllowPublicRegistration
        {
            get { return GetValue("AllowPublicRegistration", false); }
            set { SetValue("AllowPublicRegistration", value); }
        }

        /// <summary>
        /// Gets and sets whether to allow Checkbox Users are allowed to reset their passwords
        /// </summary>
        public bool AllowPasswordReset
        {
            get { return GetValue("AllowPasswordReset", true); }
            set { SetValue("AllowPasswordReset", value); }
        }

        /// <summary>
        /// Gets the default username for pre-populating the login fields
        /// </summary>
        public string DefaultLoginUsername
        {
            get { return GetValue("DefaultLoginUsername", String.Empty); }
        }

        /// <summary>
        /// Gets the default password for pre-populating the login fields
        /// </summary>
        public string DefaultLoginPassword
        {
            get { return GetValue("DefaultLoginPassword", String.Empty); }
        }

        /// <summary>
        /// Get/set size of group cache.  Value should be relatively small since the
        /// cache is designed to be used in places where iteration occurs.
        /// </summary>
        /// <remarks>Default value is 20 groups.</remarks>
        public int GroupCacheSize
        {
            get { return GetValue("GroupCacheSize", 20); }
            set { SetValue("GroupCacheSize", value); }
        }

        /// <summary>
        /// Gets and sets whether to activate NT domain authentication of Users
        /// </summary>
        public bool NTAuthentication
        {
            get { return GetValue("NTAuthentication", false); }
            set { SetValue("NTAuthentication", value); }
        }

        /// <summary>
        /// Gets and sets the name of the server variable used to store information about NT authenticated users.
        /// </summary> 
        public string NTAuthenticationVariableName
        {
            get { return GetValue("NTAuthenticationVariableName", "LOGON_USER"); }
            set { SetValue("NTAuthenticationVariableName", value); }
        }

        /// <summary>
        /// Gets and sets whether network users should be automatically logged-in without attempting
        /// to authenticate them via the application's configured authentication provider.
        /// </summary>
        /// <remarks>This setting has no effect if NTAuthentication is set to false.</remarks>
        public bool RequireRegisteredUsers
        {
            get { return GetValue("RequireRegisteredUsers", false); }
            set { SetValue("RequireRegisteredUsers", value); }
        }

        /// <summary>
        /// Gets and sets the Naming Context used to access Active Directory.
        /// </summary>
        /// <example>DC=prezzatech,DC=com</example>
        public string ActiveDirectoryNamingContext
        {
            get { return GetValue("ActiveDirectoryNamingContext", String.Empty); }
            set { SetValue("ActiveDirectoryNamingContext", value); }
        }

        /// <summary>
        /// Get/set the default roles to apply to network users that are authenticated but do not exist in the ultimate survey identity store
        /// </summary>
        public string[] DefaultRolesForUnAuthenticatedNetworkUsers
        {
            get
            {
                if (ApplicationManager.GetAppSetting("DefaultRolesForUnAuthenticatedNetworkUsers") == null)
                {
                    ApplicationManager.AddNewAppSetting("DefaultRolesForUnAuthenticatedNetworkUsers", "Respondent,Report Viewer");
                }

                string roles = ApplicationManager.GetAppSetting("DefaultRolesForUnAuthenticatedNetworkUsers");

                if (roles == string.Empty)
                {
                    ArrayList rolesList = new ArrayList();
                    return (string[])rolesList.ToArray(typeof(string));
                }

                return roles.Split(',');
            }
            set
            {
                if (value == null)
                {
                    ApplicationManager.UpdateAppSetting("DefaultRolesForUnAuthenticatedNetworkUsers", string.Empty);
                }
                else
                {
                    string setting = string.Empty;

                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0)
                        {
                            setting += ",";
                        }

                        setting += value[i];
                    }

                    ApplicationManager.UpdateAppSetting("DefaultRolesForUnAuthenticatedNetworkUsers", setting);
                }
            }
        }

        /// <summary>
        /// Gets and sets the name of the cookie used by the application
        /// </summary>
        public string CookieName
        {
            get { return GetValue("CookieName", "CheckboxWebSurvey"); }
            set { SetValue("CookieName", value); }
        }

        /// <summary>
        /// Gets and sets the default from name when creating email invitations
        /// </summary>
        public string DefaultEmailFromName
        {
            get { return GetValue("DefaultEmailFromName", string.Empty); }
            set { SetValue("DefaultEmailFromName", value); }
        }

        // In an effort to clean up the settings there will be only one default email address in the 5.0 product.
        // To accommodate this change each survey will now need to store the email address they would like
        // the save and continue  emails sent from.
        //        /// <summary>
        //        /// Gets and sets the default from address when creating email invitations
        //        /// </summary>
        //        public string DefaultFromEmailAddress
        //        {
        //            get { return GetValue("DefaultFromEmailAddress", string.Empty); }
        //            set { SetValue("DefaultFromEmailAddress", value); }
        //        }

        /// <summary>
        /// Gets or sets a flag indicating whether HTML is allowed in folder, survey, and report names
        /// </summary>
        public bool AllowHTMLNames
        {
            get { return GetValue("AllowHTMLNames", false); }
            set { SetValue("AllowHTMLNames", value); }
        }

        /// <summary>
        /// Gets the default style template for new Forms
        /// </summary>
        public int DefaultStyleTemplate
        {
            get { return GetValue("DefaultStyleTemplate", -1); }
            set { SetValue("DefaultStyleTemplate", value); }
        }

        /// <summary>
        /// Gets the default style template for new Forms
        /// </summary>
        public int DefaultStyleTemplateTablet
        {
            get { return GetValue("DefaultStyleTemplateTablet", -1); }
            set { SetValue("DefaultStyleTemplateTablet", value); }
        }

        /// <summary>
        /// Gets the default style template for new Forms
        /// </summary>
        public int DefaultStyleTemplateSmartPhone
        {
            get { return GetValue("DefaultStyleTemplateSmartPhone", -1); }
            set { SetValue("DefaultStyleTemplateSmartPhone", value); }
        }

        /// <summary>
        /// Get/set the default chart style template for new reports
        /// </summary>
        public int DefaultChartStyle
        {
            get { return GetValue("DefaultChartStyle", -1); }
            set { SetValue("DefaultChartStyle", value); }
        }

        /// <summary>
        /// Gets or sets the default survey security type
        /// </summary>
        public SecurityType DefaultSurveySecurityType
        {
            get { return GetValue("DefaultSurveySecurityType", SecurityType.Public); }
            set { SetValue("DefaultSurveySecurityType", value); }
        }

        /// <summary>
        /// Gets or sets the default report security type
        /// </summary>
        public ReportSecurityType DefaultReportSecurityType
        {
            get { return GetValue("DefaultReportSecurityType", ReportSecurityType.SummaryPrivate); }
            set { SetValue("DefaultReportSecurityType", value); }
        }

        /// <summary>
        /// Gets or sets the default total response limit per survey. -1 = no limit
        /// </summary>
        public int DefaultTotalResponseLimit
        {
            get { return GetValue("DefaultTotalResponseLimit", -1); }
            set { SetValue("DefaultTotalResponseLimit", value); }
        }

        /// <summary>
        /// Gets or sets the default per-user response limit per survey. -1 = no limit
        /// </summary>
        public int DefaultUserResponseLimit
        {
            get { return GetValue("DefaultUserResponseLimit", -1); }
            set { SetValue("DefaultUserResponseLimit", value); }
        }

        /// <summary>
        /// Gets or sets the default value of the allow resume property for a new survey
        /// </summary>
        public bool DefaultAllowResumeResponse
        {
            get { return GetValue("DefaultAllowResumeResponse", false); }
            set { SetValue("DefaultAllowResumeResponse", value); }
        }

        /// <summary>
        /// Gets or sets the default value of the allow edit property for a new survey
        /// </summary>
        public bool DefaultAllowEditResponse
        {
            get { return GetValue("DefaultAllowEditResponse", false); }
            set { SetValue("DefaultAllowEditResponse", value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating if the survey's style template may be changed
        /// </summary>
        public bool AllowEditSurveyStyleTemplate
        {
            get { return GetValue("AllowEditSurveyStyleTemplate", true); }
            set { SetValue("AllowEditSurveyStyleTemplate", value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating if the survey may be changed while active
        /// </summary>
        public bool AllowEditActiveSurvey
        {
            get { return GetValue("AllowEditActiveSurvey", true); }
            set { SetValue("AllowEditActiveSurvey", value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating if partial responses should be saved when a respondent navigates 
        /// backward in a survey
        /// </summary>
        public bool SavePartialResponsesOnBackNavigation
        {
            get { return GetValue("SavePartialResponsesOnBackNavigation", false); }
            set { SetValue("SavePartialResponsesOnBackNavigation", value); }
        }

        /// <summary>
        /// Gets the default email subject used for email invitations
        /// </summary>
        public string DefaultEmailInvitationSubject
        {
            get { return GetValue("DefaultEmailInvitationSubject", string.Empty); }
            set { SetValue("DefaultEmailInvitationSubject", value); }
        }

        /// <summary>
        /// Gets the default email body used for email invitations
        /// </summary>
        public string DefaultEmailInvitationBody
        {
            get { return GetValue("DefaultEmailInvitationBody", string.Empty); }
            set { SetValue("DefaultEmailInvitationBody", value); }
        }

        /// <summary>
        /// Gets and sets the session timeout length for the application
        /// </summary>
        public int SessionTimeOut
        {
            get { return GetValue("SessionTimeOut", 30); }
            set { SetValue("SessionTimeOut", value); }
        }

        /// <summary>
        /// Gets and sets the SMTP server used by the application to send email
        /// </summary>
        public string SmtpServer
        {
            get { return GetValue("SmtpServer", string.Empty); }
            set { SetValue("SmtpServer", value); }
        }

        /// <summary>
        /// Determines if email address' are analyzed for valid composition.
        /// When enabled email addresses must adhere to the format specified in RFC 822 and 5321
        /// When disabled no validation of the address format is performed.
        /// </summary>
        public bool EnableEmailAddressValidation
        {
            get { return GetValue("EnableEmailAddressValidation", true); }
            set { SetValue("EnableEmailAddressValidation", value); }
        }

        /// <summary>
        /// A comma separated list of valid but uncommon characters that are allowed in the local part of an email address.
        /// </summary>
        public string EmailAddressOptionalCharacters
        {
            get { return GetValue("EmailAddressOptionalCharacters", ".,!,#,$,%,&,',*,+,-,/,=,?,^,_,`,{,|,},~"); }
            set { SetValue("EmailAddressOptionalCharacters", value); }
        }

        /// <summary>
        /// Gets and sets the email address used by the system when sending emails
        /// </summary>
        public string SystemEmailAddress
        {
            get { return GetValue("SystemEmailAddress", string.Empty); }
            set { SetValue("SystemEmailAddress", value); }
        }


        /// <summary>
        /// Gets and sets the uploaded images URL
        /// </summary>
        public string UploadedImagesUrl
        {
            get { return GetValue("UploadedImagesURL", string.Empty); }
            set { SetValue("UploadedImagesURL", value); }
        }

        /// <summary>
        /// Gets and sets the uploaded images folder
        /// </summary>
        public string UploadedImagesFolder
        {
            get { return GetValue("UploadedImagesFolder", string.Empty); }
            set { SetValue("UploadedImagesFolder", value); }
        }

        /// <summary>
        /// Gets and sets the Type of header used by the application (HTML or Logo/Image)
        /// </summary>
        public HeaderType HeaderTypeChosen
        {
            get { return GetValue("HeaderType", HeaderType.Logo); }
            set { SetValue("HeaderType", value); }
        }

        /// <summary>
        /// Gets and sets the session mode (cookies or session)
        /// </summary>
        public SessionType SessionMode
        {
            get { return GetValue("SessionMode", SessionType.Cookies); }
            set { SetValue("SessionMode", value); }
        }

        /// <summary>
        /// Gets and sets whether to prevent the same user id from logging in more than once at a time
        /// </summary>
        public ConcurrentLoginMode ConcurrentLoginMode
        {
            //As of 4.4, user role limitations have been put in place so concurrent login is
            // not a good idea any more.  Make logout current as fixed value so that customers
            // with high session timeout values don't get accidentally locked out for long periods
            // of time when the web browser gets disconnected from the session.
            get { return ConcurrentLoginMode.LogoutCurrent; }
            set { SetValue("ConcurrentLoginMode", value); }
        }

        /// <summary>
        /// Gets and sets the header font size
        /// </summary>
        public string HeaderFontSize
        {
            get { return GetValue("HeaderFontSize", "18"); }
            set { SetValue("HeaderFontSize", value); }
        }

        /// <summary>
        /// Gets and sets the font used in the header
        /// </summary>
        public string HeaderFont
        {
            get { return GetValue("HeaderFont", string.Empty); }
            set { SetValue("HeaderFont", value); }
        }

        /// <summary>
        /// Gets and sets the font color used in the header
        /// </summary>
        public string HeaderTextColor
        {
            get { return GetValue("HeaderTextColor", string.Empty); }
            set { SetValue("HeaderTextColor", value); }
        }

        /// <summary>
        /// Gets and sets the center menu font color
        /// </summary>
        public string CenterMenuTextColor
        {
            get { return GetValue("CenterMenuTextColor", string.Empty); }
            set { SetValue("CenterMenuTextColor", value); }
        }

        /// <summary>
        /// Gets and sets the default width for SingleLineText items.
        /// </summary>
        public int DefaultSingleLineTextWidth
        {
            get { return GetValue("DefaultSingleLineTextWidth", 150); }
            set { SetValue("DefaultSingleLineTextWidth", value); }
        }

        /// <summary>
        /// Whether or not to use aliases in reports by default
        /// </summary>
        public bool DefaultAlias
        {
            get { return GetValue("DefaultAlias", false); }
            set { SetValue("DefaultAlias", value); }
        }

        /// <summary>
        /// Gets and sets the results per page used in paging
        /// </summary>
        public int PagingResultsPerPage
        {
            get { return GetValue("PagingResultsPerPage", 25); }
            set { SetValue("PagingResultsPerPage", value); }
        }

        /// <summary>
        /// Gets and sets the results per page used in paging
        /// </summary>
        public bool AllowDeviceAutoRegister
        {
            get { return GetValue("AllowDeviceAutoRegister", false); }
            set { SetValue("AllowDeviceAutoRegister", value); }
        }

        /// <summary>
        /// Gets and sets whether to require an email address during User registration
        /// </summary>
        public bool RequireEmailAddressOnRegistration
        {
            get { return GetValue("RequireEmailAddressOnRegistration", true); }
            set { SetValue("RequireEmailAddressOnRegistration", value); }
        }

        /// <summary>
        /// Gets and sets whether to require Terms of Use agreement on user login
        /// </summary>
        public bool RequireTermsofUseAgreement
        {
            get { return GetValue("TermsofUseAgreement", false); }
            set { SetValue("TermsofUseAgreement", value); }
        }

        /// <summary>
        /// Gets and sets the default roles for User registration
        /// </summary>
        public string[] DefaultUserRoles
        {
            get
            {
                if (ApplicationManager.GetAppSetting("DefaultUserRoles") == null)
                {
                    ApplicationManager.AddNewAppSetting("DefaultUserRoles", "Respondent,Report Viewer");
                }

                string roles = ApplicationManager.GetAppSetting("DefaultUserRoles");

                if (roles == string.Empty)
                {
                    var rolesList = new ArrayList();
                    return (string[])rolesList.ToArray(typeof(string));
                }

                return roles.Split(',');
            }
            set
            {
                if (value == null)
                {
                    ApplicationManager.UpdateAppSetting("DefaultUserRoles", string.Empty);
                }
                else
                {
                    string setting = string.Empty;

                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i > 0)
                        {
                            setting += ",";
                        }

                        setting += value[i];
                    }

                    ApplicationManager.UpdateAppSetting("DefaultUserRoles", setting);
                }
            }
        }

        // autogenerate reports wizard settings
        /// <summary>
        /// Gets and sets the default analysis item to use for open-ended single-line text items
        /// </summary>
        public string AutogenReportDefaultSingleLineText
        {
            get { return GetValue("AutogenReportDefaultSingleLineText", "Details"); }
            set { SetValue("AutogenReportDefaultSingleLineText", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for open-ended multi-line text items
        /// </summary>
        public string AutogenReportDefaultMultiLineText
        {
            get { return GetValue("AutogenReportDefaultMultiLineText", "Details"); }
            set { SetValue("AutogenReportDefaultMultiLineText", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for checkbox items
        /// </summary>
        public string AutogenReportDefaultCheckboxes
        {
            get { return GetValue("AutogenReportDefaultCheckboxes", "ColumnGraph"); }
            set { SetValue("AutogenReportDefaultCheckboxes", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for radio button items
        /// </summary>
        public string AutogenReportDefaultRadioButtons
        {
            get { return GetValue("AutogenReportDefaultRadioButtons", "PieGraph"); }
            set { SetValue("AutogenReportDefaultRadioButtons", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for slider item
        /// </summary>
        public string AutogenReportDefaultSlider
        {
            get { return GetValue("AutogenReportDefaultSlider", "PieGraph"); }
            set { SetValue("AutogenReportDefaultSlider", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for net promoter score item
        /// </summary>
        public string AutogenReportDefaultNetPromoterScore
        {
            get { return GetValue("AutogenReportDefaultNetPromoterScore", "NetPromoterScoreTable"); }
            set { SetValue("AutogenReportDefaultNetPromoterScore", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for rank order item
        /// </summary>
        public string AutogenReportDefaultRankOrder
        {
            get { return GetValue("AutogenReportDefaultRankOrder", "RankOrderSummaryTable"); }
            set { SetValue("AutogenReportDefaultRankOrder", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for rating items
        /// </summary>
        public string AutogenReportDefaultRadioButtonScale
        {
            get { return GetValue("AutogenReportDefaultRadioButtonScale", "StatisticsTable"); }
            set { SetValue("AutogenReportDefaultRadioButtonScale", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for drop down list items
        /// </summary>
        public string AutogenReportDefaultDropDownList
        {
            get { return GetValue("AutogenReportDefaultDropDownList", "PieGraph"); }
            set { SetValue("AutogenReportDefaultDropDownList", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for matrix items
        /// </summary>
        public string AutogenReportDefaultMatrix
        {
            get { return GetValue("AutogenReportDefaultMatrix", "MatrixSummary"); }
            set { SetValue("AutogenReportDefaultMatrix", value); }
        }

        /// <summary>
        /// Gets and sets the default maximum number of options a SelectItemData may have before using a Summary Table in a report 
        /// </summary>
        public int AutogenReportDefaultMaxOptions
        {
            get { return GetValue("AutogenReportDefaultMaxOptions", 10); }
            set { SetValue("AutogenReportDefaultMaxOptions", value); }
        }

        /// <summary>
        /// Gets and sets the default analysis item to use for hidden items
        /// </summary>
        public string AutogenReportDefaultHiddenItems
        {
            get { return GetValue("AutogenReportDefaultHiddenItems", "SummaryTable"); }
            set { SetValue("AutogenReportDefaultHiddenItems", value); }
        }

        /// <summary>
        /// Gets and sets the default setting for using aliases in autogenerated reports
        /// </summary>
        public bool AutogenReportDefaultUseAliases
        {
            get { return GetValue("AutogenReportDefaultUseAliases", false); }
            set { SetValue("AutogenReportDefaultUseAliases", value); }
        }

        /// <summary>
        /// Gets and sets the default setting for displaying dataistics tables for the charts in autogenerated reports
        /// </summary>
        public bool AutogenReportDisplayStatistics
        {
            get { return GetValue("AutogenReportDisplayStatistics", false); }
            set { SetValue("AutogenReportDisplayStatistics", value); }
        }

        /// <summary>
        /// Gets and sets the default setting for displaying dataistics tables for the charts in autogenerated reports
        /// </summary>
        public bool AutogenReportDisplayAnswers
        {
            get { return GetValue("AutogenReportDisplayAnswers", false); }
            set { SetValue("AutogenReportDisplayAnswers", value); }
        }
        
        /// <summary>
        /// Gets and sets the default setting for including incomplete responses in autogenerated reports
        /// </summary>
        public bool AutogenReportDefaultIncludeIncompleteResponses
        {
            get { return GetValue("AutogenReportIncludeIncompleteResponses", false); }
            set { SetValue("AutogenReportIncludeIncompleteResponses", value); }
        }

        /// <summary>
        /// Gets and sets the default for whether to use the survey paging scheme when autogenerating a report
        /// </summary>
        public bool AutogenReportDefaultMultiplePages
        {
            get { return GetValue("AutogenReportDefaultMultiplePages", false); }
            set { SetValue("AutogenReportDefaultMultiplePages", value); }
        }

        /// <summary>
        /// Gets and sets the default for whether to display the survey title when autogenerating a report
        /// </summary>
        public bool DisplaySurveyTitle
        {
            get { return GetValue("DisplaySurveyTitleOnReport", false); }
            set { SetValue("DisplaySurveyTitleOnReport", value); }
        }

        /// <summary>
        /// Gets and sets the default for whether to display the pdf-export button when autogenerating a report
        /// </summary>
        public bool DisplayPdfExportButton
        {
            get { return GetValue("DisplayPdfExportButton", true); }
            set { SetValue("DisplayPdfExportButton", value); }
        }

        /// <summary>
        /// Gets and sets the default item position for the Autogenerate Report Wizard
        /// </summary>
        public string AutogenReportDefaultItemPosition
        {
            get { return GetValue("AutogenReportDefaultItemPosition", "Left"); }
            set { SetValue("AutogenReportDefaultItemPosition", value); }
        }

        /// <summary>
        /// Gets and sets the default label style for pie/doughnut charts
        /// </summary>
        public string AutogenReportDefaultPieLabelStyle
        {
            get { return GetValue("AutogenReportDefaultPieLabelStyle", "Inside"); }
            set { SetValue("AutogenReportDefaultPieLabelStyle", value); }
        }

        /// <summary>
        /// Determines if incomplete responses are displayed by default when viewing
        /// the response details page. When this option is disabled a user can toggle
        /// the display of incomplete on the page if they would like to see them.
        /// </summary>
        public bool ResponseDetailsDisplayIncompleteResponses
        {
            get { return GetValue("ResponseDetailsDisplayIncompleteResponses", false); }
            set { SetValue("ResponseDetailsDisplayIncompleteResponses", value); }
        }

        /// <summary>
        /// The default number of responses to display when viewing the response details page.
        /// </summary>
        public int ResponseDetailsResultsPerPage
        {
            get { return GetValue("ResponseDetailsResultsPerPage", 20); }
            set { SetValue("ResponseDetailsResultsPerPage", value); }
        }

        /// <summary>
        /// Determines if detailed response information is displayed when viewing the response details.
        /// </summary>
        public bool ResponseDisplayDetails
        {
            get { return GetValue("ResponseDisplayDetails", true); }
            set { SetValue("ResponseDisplayDetails", value); }
        }

        /// <summary>
        /// Determines if user information is displayed when viewing the response details.
        /// </summary>
        public bool ResponseDisplayUserDetails
        {
            get { return GetValue("ResponseDisplayUserDetails", true); }
            set { SetValue("ResponseDisplayUserDetails", value); }
        }

        /// <summary>
        /// Determines if question numbers are displayed when viewing the response details.
        /// </summary>
        public bool ResponseDisplayQuestionNumbers
        {
            get { return GetValue("ResponseDisplayQuestionNumbers", true); }
            set { SetValue("ResponseDisplayQuestionNumbers", value); }
        }

        /// <summary>
        /// Determines if unanswered questions are displayed when viewing the response details.
        /// </summary>
        public bool ResponseDisplayUnansweredQuestions
        {
            get { return GetValue("ResponseDisplayUnansweredQuestions", true); }
            set { SetValue("ResponseDisplayUnansweredQuestions", value); }
        }

        /// <summary>
        /// Shows Rank OrderPoint near each option
        /// </summary>
        public bool ResponseDisplayRankOrderPoints
        {
            get { return GetValue("ResponseDisplayRankOrderPoints", false); }
            set { SetValue("ResponseDisplayRankOrderPoints", value); }
        }

        /// <summary>
        /// Get/set the default export type for exported survey results.
        /// </summary>
        public string DefaultExportType
        {
            get { return GetValue("DefaultExportType", "Standard"); }
            set { SetValue("DefaultExportType", value); }
        }

        /// <summary>
        /// Get/set the default file encoding for exported survey results.
        /// </summary>
        public string DefaultExportEncoding
        {
            get { return GetValue("DefaultExportEncoding", "UTF8"); }
            set { SetValue("DefaultExportEncoding", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export incomplete responses" response export option.
        /// </summary>
        public bool CsvExportIncludeResponseDetails
        {
            get { return GetValue("CsvExportIncludeResponseDetails", false); }
            set { SetValue("CsvExportIncludeResponseDetails", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Detailed user info" response export option.
        /// </summary>
        public bool CsvExportIncludeUserDetails
        {
            get { return GetValue("CsvExportIncludeUserDetails", false); }
            set { SetValue("CsvExportIncludeUserDetails", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Merge checkbox results" response export option.
        /// </summary>
        public bool CsvExportMergeCheckboxResults
        {
            get { return GetValue("CsvExportMergeCheckboxResults", false); }
            set { SetValue("CsvExportMergeCheckboxResults", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export open ended results" response export option.
        /// </summary>
        public bool CsvExportIncludeOpenendedResults
        {
            get { return GetValue("CsvExportIncludeOpenendedResults", false); }
            set { SetValue("CsvExportIncludeOpenendedResults", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export with aliases" response export option.
        /// </summary>
        public bool CsvExportUseAliases
        {
            get { return GetValue("CsvExportUseAliases", false); }
            set { SetValue("CsvExportUseAliases", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export hidden items" response export option.
        /// </summary>
        public bool CsvExportIncludeHiddenItems
        {
            get { return GetValue("CsvExportIncludeHiddenItems", false); }
            set { SetValue("CsvExportIncludeHiddenItems", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export incomplete responses" response export option.
        /// </summary>
        public bool CsvExportIncludeIncompleteResponses
        {
            get { return GetValue("CsvExportIncludeIncompleteResponses", false); }
            set { SetValue("CsvExportIncludeIncompleteResponses", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Split export" response export option.
        /// </summary>
        public bool CsvExportSplitExport
        {
            get { return GetValue("CsvExportSplitExport", false); }
            set { SetValue("CsvExportSplitExport", value); }
        }

        /// <summary>
        /// Get/set html tags cut from answers in csv export of a response
        /// </summary>
        public bool CsvExportStripHtmlTagsFromAnswers
        {
            get { return GetValue("CsvExportStripHtmlTagsFromAnswers", false); }
            set { SetValue("CsvExportStripHtmlTagsFromAnswers", value); }
        }

        /// <summary>
        /// Get/set rank order export mode
        /// </summary>
        public bool CsvExportRankOrderPoints
        {
            get { return GetValue("CsvExportRankOrderPoints", false); }
            set { SetValue("CsvExportRankOrderPoints", value); }
        }

        /// <summary>
        /// Get/set where the detailed score info should be included into the file
        /// </summary>
        public bool CsvExportIncludeDetailedScoreInfo
        {
            get { return GetValue("CsvExportIncludeDetailedScoreInfo", false); }
            set { SetValue("CsvExportIncludeDetailedScoreInfo", value); }
        }

        /// <summary>
        /// Get/set where the possible score should be included into the file
        /// </summary>
        public bool CsvExportIncludePossibleScore
        {
            get { return GetValue("CsvExportIncludePossibleScore", false); }
            set { SetValue("CsvExportIncludePossibleScore", value); }
        }

        /// <summary>
        /// Get/set where the test responses should be included into the file
        /// </summary>
        public bool ExportIncludeTestResponses
        {
            get { return GetValue("ExportIncludeTestResponses", false); }
            set { SetValue("ExportIncludeTestResponses", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Include response id in export" response SPSS export option.
        /// </summary>
        public bool SpssExportIncludeResponseId
        {
            get { return GetValue("SpssExportIncludeResponseId", false); }
            set { SetValue("SpssExportIncludeResponseId", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export incomplete responses" response SPSS export option.
        /// </summary>
        public bool SpssExportIncludeIncompleteResponses
        {
            get { return GetValue("SpssExportIncludeIncompleteResponses", false); }
            set { SetValue("SpssExportIncludeIncompleteResponses", value); }
        }

        /// <summary>
        /// Get/Set the default value for the "Export open ended results" response SPSS export option.
        /// </summary>
        public bool SpssExportIncludeOpenendedResults
        {
            get { return GetValue("SpssExportIncludeOpenendedResults", false); }
            set { SetValue("SpssExportIncludeOpenendedResults", value); }
        }

        /// <summary>
        /// Get whether license limit debug mode is enabled.  When enabled, all license limits will have a limit value of 1
        /// </summary>
        public bool LimitDebugMode
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["LimitDebugMode"]))
                {
                    return false;
                }

                if (string.Compare(ConfigurationManager.AppSettings["LimitDebugMode"], "true", true) == 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Determines if product tour messages should be displayed.
        /// </summary>
        public bool DisplayProductTour
        {
            get { return GetValue("DisplayProductTour", false); }
            set { SetValue("DisplayProductTour", value); }
        }
        /// <summary>
        /// Determines if the new line character should be removed from exported responses.
        /// </summary>
        public bool ReplaceNewLine
        {
            get { return GetValue("ReplaceNewLine", true); }
            set { SetValue("ReplaceNewLine", value); }
        }

        /// <summary>
        /// The value used to replace new line characters when exporting responses.
        /// </summary>
        public string NewLineReplacement
        {
            get { return GetValue("NewLineReplacement", " "); }
            set { SetValue("NewLineReplacement", value); }
        }

        /// <summary>
        /// The maximum consecutive number of times an idle client connection is 
        /// kept alive before a client is disconnected. A value of zero disables 
        /// the keep alive functionality.
        /// </summary>
        public int KeepAlive
        {
            get { return GetValue("KeepAlive", 0); }
            set { SetValue("KeepAlive", value); }
        }

        /// <summary>
        /// Controls whether or not authorization queries check for access control list 
        /// entries which revoke a user/groups rights.
        /// </summary>
        public bool AllowExclusionaryAclEntries
        {
            get { return GetValue("AllowExclusionaryAclEntries", true); }
            set { SetValue("AllowExclusionaryAclEntries", value); }
        }

        /// <summary>
        /// Return a value indicating whether file uploads should be sent to an S3 Bucket
        /// </summary>
        public bool UseS3ForUploadedFiles
        {
            get { return GetValue("UseS3ForUploadedFiles", false); }
            set { SetValue("UseS3ForUploadedFiles", value); }
        }
        /// <summary>
        /// Return the Amazon S3 Access Key ID
        /// </summary>
        public string S3AccessKeyID
        {
            get { return GetValue("S3AccessKeyID", string.Empty); }
            set { SetValue("S3AccessKeyID", value); }
        }
        /// <summary>
        /// Return the Amazon S3 Secret Access Key
        /// </summary>
        public string S3SecretAccessKey
        {
            get { return GetValue("S3SecretAccessKey", string.Empty); }
            set { SetValue("S3SecretAccessKey", value); }
        }

        /// <summary>
        /// Name of S3 Bucket to use to store files
        /// </summary>
        public string S3BucketName
        {
            get { return GetValue("S3BucketName", string.Empty); }
            set { SetValue("S3BucketName", value); }
        }

        /// <summary>
        /// Get/set whether to use S3 repository for temporary export files
        /// </summary>
        public bool UseS3ForTempFiles
        {
            get { return GetValue("UseS3ForTempFiles", false); }
            set { SetValue("UseS3ForTempFiles", value); }
        }


        /// <summary>
        /// Get/set whether to use S3 repository for temporary export files
        /// </summary>
        public string S3TempBucketName
        {
            get { return GetValue("S3TempBucketName", string.Empty); }
            set { SetValue("S3TempBucketName", value); }
        }

        /// <summary>
        /// Get/set the seconds count after which Timeline request is considered as expired
        /// </summary>
        public int TimelineRequestExpiration
        {
            get { return GetValue("TimelineRequestExpiration", 30); }
            set { SetValue("TimelineRequestExpiration", value); }
        }

        /// <summary>
        /// Get/set the seconds count after which Timeline request is considered as expired
        /// </summary>
        public int TimelineRecordsPerPage
        {
            get { return GetValue("TimelineRecordsPerPage", 5); }
            set { SetValue("TimelineRecordsPerPage", value); }
        }

        ///<summary>
        /// Show created by for folders
        /// </summary>
        public bool ShowCreatedBy
        {
            get { return GetValue ("ShowCreatedBy", false); } 
            set { SetValue("ShowCreatedBy", value); }
        }

        public bool DisplayMachineName
        {
            get { return GetValue("DisplayMachineName", false); }
            set { SetValue("DisplayMachineName", value); }
        }

        public bool CustomerUpdateHeader
        {
            get { return GetValue("CustomerUpdateHeader", true); }
            set { SetValue("CustomerUpdateHeader", value); }
        }
        

        #endregion

        #region Search Settings

        /// <summary>
        /// Get/set the seconds count after which available objects in the cache will be expired
        /// </summary>
        public int SearchAccessibleObjectExpPeriodSeconds
        {
            get { return GetValue("SearchAccessibleObjectExpPeriodSeconds", 300); }
            set { SetValue("SearchAccessibleObjectExpPeriodSeconds", value); }
        }

        /// <summary>
        /// Get/set the seconds count after which search results in the cache will be expired
        /// </summary>
        public int SearchResultsExpPeriodSeconds
        {
            get { return GetValue("SearchResultsExpPeriodSeconds", 10); }
            set { SetValue("SearchResultsExpPeriodSeconds", value); }
        }

        /// <summary>
        /// Get/set the records count of each object type to be displayed on a quick search div
        /// </summary>
        public int SearchPageSize
        {
            get { return GetValue("SearchPageSize", 5); }
            set { SetValue("SearchPageSize", value); }
        }

        /// <summary>
        /// Get/set the records count of each object type to be collect in the cache
        /// </summary>
        public int SearchMaxResultRecordsPerObjectType
        {
            get { return GetValue("SearchMaxResultRecordsPerObjectType", 100); }
            set { SetValue("SearchMaxResultRecordsPerObjectType", value); }
        }


        /// <summary>
        /// Get/set the days count after that all cached results must be deleted
        /// </summary>
        public int SearchCachePeriodDays
        {
            get { return GetValue("SearchCachePeriodDays", 7); }
            set { SetValue("SearchCachePeriodDays", value); }
        }        
        #endregion

        #region web.config settings

        /// <summary>
        /// Gets and sets the ApplicationRoot directory
        /// </summary>
        public bool CacheVolatileDataInApplication
        {
            get
            {
                return string.Compare(ConfigurationManager.AppSettings["CacheVolatileDataInApplication"], "true", true) == 0;
            }
        }

        /// <summary>
        /// Get whether to cache app settings
        /// </summary>
        public bool CacheAppSettings
        {
            get
            {
                return string.Compare(ConfigurationManager.AppSettings["CacheAppSettings"], "true", true) == 0;
            }
        }

        /// <summary>
        /// Get whether to cache app settings
        /// </summary>
        public bool CacheRolePermissions
        {
            get
            {
                return string.Compare(ConfigurationManager.AppSettings["CacheRolePermissions"], "true", true) == 0;
            }
        }

        /// <summary>
        /// Connection string to the email relay database
        /// </summary>
        public string MailDbConnectionString 
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["MailDbConnectionString"].ConnectionString;
            }
            set
            {
                ConfigurationManager.ConnectionStrings["MailDbConnectionString"].ConnectionString = value;
            }
        }


        /// <summary>
        /// Get whether to cache app settings
        /// </summary>
        public bool CacheIdentityRoles
        {
            get { return GetValue("CacheIdentityRoles", true); }
            set { SetValue("CacheIdentityRoles", value); }
        }

        /// <summary>
        /// Get whether to cache user non profile properties
        /// </summary>
        public bool CacheUserNonProfileProperties
        {
            get { return GetValue("CacheUserNonProfileProperties", true); }
            set { SetValue("CacheUserNonProfileProperties", value); }
        }
        /// <summary>
        /// Gets and sets the database connection string used by the application
        /// </summary>
        /// <remarks>Present only because it is required for backwards compatibility.</remarks>
        public string DefaultConnectionString
        {
            get
            {
                if (ConfigurationManager.ConnectionStrings["DefaultConnectionString"] != null)
                {
                    return ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Get whether language debug mode is enabled.  When enabled, the text manager will return the textID of the string if no matching
        /// text is found.  Otherwise it will return null.  Value is configured through settings in web.config.
        /// </summary>
        public bool LanguageDebugMode
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["LanguageDebugMode"]))
                {
                    return false;
                }

                return string.Compare(ConfigurationManager.AppSettings["LanguageDebugMode"], "true", true) == 0;
            }
        }

        /// <summary>
        /// Get whether simple security debug mode is enabled.  When enabled, simple security will be turned on regardless
        /// of end-user's license setting.  Value is configured through settings in web.config.
        /// </summary>
        public bool SimpleSecurityDebugMode
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SimpleSecurityDebugMode"]))
                {
                    return false;
                }

                return string.Compare(ConfigurationManager.AppSettings["SimpleSecurityDebugMode"], "true", true) == 0;
            }
        }

        /// <summary>
        /// Min percent of Available Limit. If actual percent is less than it, the limit should be highlighted.-->
        /// </summary>
        public int MinPercentOfAvailableLimit
        {
            get
            {
                int temp;
                if (int.TryParse(ConfigurationManager.AppSettings["MinPercentOfAvailableLimit"], out temp))
                    return temp;
                return 0;
            }
        }

        /// <summary>
        /// Get the list of allowed extensions for URL Rewriting
        /// </summary>
        public List<string> AllowedUrlRewriteExtensions
        {
            get
            {
                var allowedExtensions = new List<string>();

                if (ConfigurationManager.AppSettings["AllowedUrlRewriteExtensions"] == null || ConfigurationManager.AppSettings["AllowedUrlRewriteExtensions"].Trim() == string.Empty)
                {
                    return allowedExtensions;
                }

                string[] extensions = ConfigurationManager.AppSettings["AllowedUrlRewriteExtensions"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                allowedExtensions.AddRange(extensions.Select(extension => extension.Trim()));

                return allowedExtensions;
            }
        }

        /// <summary>
        /// The time span that is used for locking scheduled invitation before sending
        /// 
        /// By default : 5 minutes
        /// </summary>
        public int InvitationLockMinutes
        {
            get
            {
                int res = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["InvitationLockMinutes"], out res))
                {
                    return res;
                }
                return 5;
            }
        }


        #endregion

        #region Other Settings

        /// <summary>
        /// Gets a collection of reserved property names
        /// </summary>
        public ArrayList UserMapColumnNames { get { return new ArrayList { "UserName", "Password", "Domain", "UniqueIdentifier" }; } }

        /// <summary>
        /// Gets a collection of custom user field names 
        /// </summary>
        public ArrayList CustomFields
        {
            get
            {
                var customFields = new ArrayList();
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetFields");

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            customFields.Add(reader[0]);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return customFields;

            }
        }

        /// <summary>
        /// Returns a value indicating whether the application enables / disabled buffering when exporting response data.
        /// </summary>
        public bool BufferResponseExport
        {
            get { return GetValue("BufferResponseExport", true); }
            set { SetValue("BufferResponseExport", value); }
        }

        /// <summary>
        /// Returns a value indicating whether the application enables / disabled user list preloading.
        /// </summary>
        public bool DisableUserListForAD
        {
            get { return GetValue("DisableUserListForAD", false); }
            set { SetValue("DisableUserListForAD", value); }
        }

        /// <summary>
        /// Return a value indicating whether viewstate should be read/written to/from database
        /// </summary>
        public bool PersistViewStateToDb
        {
            get { return GetValue("PersistViewStateToDb", false); }
            set { SetValue("PersistViewStateToDb", value); }
        }

        /// <summary>
        /// Return a value indicating whether address finder item is enabled
        /// </summary>
        public bool AddressFinderEnabled
        {
            get { return GetValue("AddressFinderEnabled", false); }
            set { SetValue("AddressFinderEnabled", value); }
        }

        /// <summary>
        /// Get address finder key
        /// </summary>
        public string AddressFinderScriptKey
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AddressFinderScriptKey"]))
                {
                    return ConfigurationManager.AppSettings["AddressFinderScriptKey"];
                }

                throw new Exception("Please define AddressFinderScriptKey in the AppSettions section of the web.config file.");
            }
        }

        /// <summary>
        /// Tells address finder to show text fields with coordinates
        /// </summary>
        public bool AddressFinderShowCoordinates
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AddressFinderShowCoordinates"]))
                {
                    return ConfigurationManager.AppSettings["AddressFinderShowCoordinates"].ToLowerInvariant().Equals("true");
                }

                return false;
            }
        }


        #endregion

        #region ProductTierLimits
        /// <summary>
        /// Get/set whether https is supported. This feature is only intended to be used with hosted installations.
        /// </summary>
        public bool AllowHttps
        {
            get { return GetValue("AllowHttps", true); }
            set { SetValue("AllowHttps", value); }
        }

        /// <summary>
        /// Indicates if the use of invitations is allowed.
        /// </summary>
        public bool AllowInvitations
        {
            get
            {
                string message;
                var limit = new InvitationLimit();
                LimitValidationResult result = limit.Validate(out message);

                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicates if native spss export is allowed.
        /// </summary>
        public bool AllowNativeSpssExport
        {
            get
            {
                string message;
                var limit = new SpssLimit();
                LimitValidationResult result = limit.Validate(out message);

                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicates if user can send one email or not.
        /// </summary>
        public bool AllowSendEmail
        {
            get
            {
                string message;
                var limit = new EmailLimit();
                LimitValidationResult result = limit.Validate(out message);

                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicated if MLS is allowed.
        /// </summary>
        public bool AllowMultiLanguage
        {
            get
            {
                string message;
                var limit = new MultiLanguageLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicated if MLS is allowed.
        /// </summary>
        public bool AllowLibraries
        {
            get
            {
                string message;
                var limit = new LibraryLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicates if simpleSecurityMode is enabled.
        /// </summary>
        public bool UseSimpleSecurity
        {
            get
            {
                string message;
                if (EnableMultiDatabase)
                {
                    //check the limit
                    var limit = new SimpleSecurityLimit();
                    var result = limit.Validate(out message);
                    if (result == LimitValidationResult.LimitExceeded)
                        return SimpleSecurity;
                    return true;
                }
                else
                {
                    //check if settings flag has true value
                    return SimpleSecurity;
                }
            }
        }

        /// <summary>
        /// Indicates if simpleSecurityMode is enabled.
        /// </summary>
        public bool IsPrepMode => ApplicationManager.AppSettings.GetValue("SystemMode", string.Empty).Equals("PrepMode");


        /// <summary>
        /// Indicates if Checkbox logo footer should be shown mandatory.
        /// </summary>
        public bool UseMandatoryCheckboxFooter
        {
            get
            {
                string message;
                var limit = new MandatoryCheckboxFooterLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }



        /// <summary>
        /// Indicates if RatingScaleStatistics item is allowed in Report.
        /// </summary>
        public bool AllowRatingScaleStatisticsReportItem
        {
            get
            {
                string message;
                var limit = new RatingScaleStatisticsReportItemLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }


        /// <summary>
        /// Indicates if scoredSurveys are allowed.
        /// </summary>
        public bool AllowScoredSurveys
        {
            get
            {
                string message;
                var limit = new ScoredSurveyLimit();
                var result = limit.Validate(out message);
                return (result == LimitValidationResult.LimitNotReached);
            }
        }

        /// <summary>
        /// Indicated if users are allowed to authenticate against active directroy.
        /// </summary>
        public bool AllowNetworkUsers
        {
            get
            {
                string message;
                var limit = new NetworkUserLimit();
                LimitValidationResult result = limit.Validate(out message);

                return (result == LimitValidationResult.LimitNotReached);
            }
        }
        #endregion

        #region Enums

        /// <summary>
        /// Describes the different Session types available for state management
        /// </summary>
        public enum SessionType
        {
            /// <summary>
            /// Stores persistent cookies on the user's browser
            /// </summary>
            Cookies = 1,

            /// <summary>
            /// Stores cookies only in the user's browser memory.  These are destroyed when the browser closes
            /// </summary>
            Cookieless
        }

        /// <summary>
        /// Describes the different header types for the admin interface
        /// </summary>
        public enum HeaderType
        {
            /// <summary>
            /// Uses a graphical logo for the header
            /// </summary>
            Logo = 1,
            /// <summary>
            /// Uses text for the header
            /// </summary>
            Text
        }
        /// <summary>
        /// Describes the different available views when editing a Form
        /// </summary>
        public enum EditSurveyView
        {
            ///<summary>
            ///View all pages of a survey on one screen when editing a survey
            ///</summary>
            AllPages = 1,

            ///<summary>
            ///View each page individually when editing a survey
            ///</summary>
            IndividualPages
        }

        /// <summary>
        /// 
        /// </summary>
        public enum OptionEntryType
        {
            /// <summary>
            /// Normal entry interface for options.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// Use quick-entry interface
            /// </summary>
            QuickEntry,

            /// <summary>
            /// Use XML import interface
            /// </summary>
            XMLImport
        }

        #endregion

        #region Password Requirements
        /// <summary>
        /// Minimum length allowed for user passwords. 
        /// </summary>
        public int MinPasswordLength
        {
            get { return GetValue("MinPasswordLength", -1); }
            set { SetValue("MinPasswordLength", value); }
        }

        /// <summary>
        /// Minimum number of non-alphanumeric characters required for a password.  
        /// </summary>
        public int MinPasswordNonAlphaNumeric
        {
            get { return GetValue("MinPasswordNonAlphaNumeric", -1); }
            set { SetValue("MinPasswordNonAlphaNumeric", value); }
        }

        /// <summary>
        /// Maximum number of consecutive failed login attempts allowed for an account until it is “locked”. 
        /// </summary>
        public int MaxFailedLoginAttempts
        {
            get { return GetValue("MaxFailedLoginAttempts", -1); }
            set { SetValue("MaxFailedLoginAttempts", value); }
        }

        /// <summary>
        /// By default, password requirements (if enabled) will only apply to users using Register.aspx 
        /// to self-register.  When the EnforcePasswordLimitsGlobally setting is turned on, 
        /// the requirements will also apply to creating and importing users in the User Manager
        /// </summary>
        public bool EnforcePasswordLimitsGlobally
        {
            get { return true; }
        }
        #endregion Password/Login Enhancements

        /// <summary>
        /// Facebook Application ID
        /// </summary>
        public string FacebookAppID 
        { 
            get
            {
                return EnableMultiDatabase
                           ? GetValue("FacebookAppID", "")
                           : ConfigurationManager.AppSettings["FacebookAppID"];
            } 
        }

        /// <summary>
        /// Endpoint for OpenId Support - By default this is Google Authentication
        /// </summary>
        public string SsoProviderOPEndpoint
        {
            get { return ConfigurationManager.AppSettings["SsoProviderOPEndpoint"]; }
        }

        /// <summary>
        /// Provider for OpenId Support - By default this is Google Authentication
        /// </summary>
        public string SsoProviderOPIdentifier
        {
            get { return ConfigurationManager.AppSettings["SsoProviderOPIdentifier"]; }
        }

        /// <summary>
        /// Guid for the typekit embed URL 
        /// </summary>
        public string TypeKitGuid
        {
            get
            {
                return ConfigurationManager.AppSettings["TypeKitGuid"];
            }
        }

        /// <summary>
        /// Access key for Zendesk SSO
        /// </summary>
        public string JwtAccessKey
        {
            get
            {
                return EnableMultiDatabase
                           ? ConfigurationManager.AppSettings["ZendeskToken"]
                           : GetValue("ZendeskToken", "");
            }
        }

        /// <summary>
        /// Template for the invitation footer
        /// </summary>
        public bool FooterEnabled
        {
            get
            {
                if (EnableMultiDatabase)
                    return true;
                return GetValue("FooterEnabled", false);
            }
            set
            {
                if (EnableMultiDatabase)
                    return;
                SetValue("FooterEnabled", value);
            }
        }

        /// <summary>
        /// Template for the invitation footer
        /// </summary>
        public string FooterText 
        { 
            get 
            {
                return GetValue("FooterText", InitialFooterText);
            }
            set
            {
                SetValue("FooterText", value);
            }
        }

        /// <summary>
        /// Initial template for the invitation footer
        /// </summary>
        public string InitialFooterText 
        { 
            get
            {
                return GetValue("InitialFooterText", "");
            } 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal string GetCompanyProperty(string p)
        {
            return GetValue("CompanyProperty." + p, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal void SetCompanyProperty(string p, string value)
        {
            SetValue("CompanyProperty." + p, value);
        }

        /// <summary>
        /// Gets the PDF export margin top.
        /// </summary>
        /// <value>
        /// The get PDF export margin top.
        /// </value>
        /// <param name="name">The name.</param>
        public float GetPdfExportMarginTop => GetValue<float>("ExportPdfMarginTop", 0);

        /// <summary>
        /// Gets the get PDF export margin bottom.
        /// </summary>
        /// <value>
        /// The get PDF export margin bottom.
        /// </value>
        public float GetPdfExportMarginBottom => GetValue<float>("ExportPdfMarginBottom", 0);
    }
}
