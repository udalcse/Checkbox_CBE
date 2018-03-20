using System;
using System.IO;
using System.Web.UI.WebControls;
using System.Text;
using Checkbox.Common;
using Checkbox.Configuration.Install;
using Checkbox.Users;
using Checkbox.Web.Page;
using Prezza.Framework.Security.Principal;
using System.Web;

namespace CheckboxWeb.Install
{
    public partial class Default : CheckboxServerProtectedPage
    {
        private ApplicationInstaller _appInstaller;
        public bool _navEnabled = true;
        protected ApplicationInstaller AppInstaller
        {
            set { Session["_appInstaller"] = _appInstaller = value; }
            get { return _appInstaller ?? (_appInstaller = Session["_appInstaller"] as ApplicationInstaller); }
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _SMTPConfigurator.GetEMailAddressCallback = GetEMailAddress;

            _userContextLabel.Text = Environment.UserName;

            var version = new Version(4, 0, 0, 0);

            _aspNetVersionLabel.Text = Environment.Version.ToString();

            if (version > Environment.Version)
            {
                _failAspNetPanel.Visible = true;
                _successAspNetPanel.Visible = false;
                _navEnabled = false;
            }
            else
            {
                _successAspNetPanel.Visible = true;
                _failAspNetPanel.Visible = false;
            }
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                _secureConnectionFail.Visible = true;
                _secureConnectionSuccess.Visible = false;
                _navEnabled = false;
            }
            else
            {
                _secureConnectionSuccess.Visible = true;
                _secureConnectionFail.Visible = false;
            }

            CheckPermissions();


            //Javascript required to handle the existing database dialog
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "setNextClientId",
                "nextClientId = '" +_installWizard.FindControl("StepNavigationTemplateContainerID").FindControl("_stepNavigationButtons").FindControl("_nextButton").ClientID + "';",
                true);
            _adminUserProfileDiv.Visible = !(isUpgrade || (AppInstaller != null && !AppInstaller.InstallDatabase));
            
            _installWizard.FindControl("StartNavigationTemplateContainerID").FindControl("_startNavigationButtons").FindControl("_nextButton").Visible = _navEnabled;
             

            /*
            var dialogJS = new StringBuilder("function OnDBExistsDialogClose(sender, args) { if (args.get_argument() != null) { $(\"#");
            dialogJS.Append(_dbExistsDialogResult.ClientID);
            dialogJS.Append("\").val(args.get_argument()); ");
            dialogJS.Append("WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(\'");
            dialogJS.Append(MakeButtonEventTarget(_installWizard.FindControl("StepNavigationTemplateContainerID").FindControl("_stepNavigationButtons").FindControl("_nextButton").ClientID));
            dialogJS.Append("\', \'\', false, \'\', \'\', false, true)); ");

            dialogJS.Append("} }");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "DialogCloseHandler", dialogJS.ToString(), true);
            */
            bool forceInstall = false;

            if (!Page.IsPostBack)
            {
                //If attempting to force a new install, require a login
                if (Request.QueryString["ForceInstall"] != null && Request.QueryString["ForceInstall"].ToLower() == "true")
                {
                    var principal = (ExtendedPrincipal)UserManager.GetCurrentPrincipal();

                    if (principal == null || !principal.IsInRole("System Administrator"))
                    {
                        Response.Redirect(ResolveUrl("~/Login.aspx") + "?RedirectUrl=" + Server.UrlEncode("Install/Default.aspx?ForceInstall=true"), false);
                    }
                    else
                    {
                        forceInstall = true;
                    }
                }

                AppInstaller = new ApplicationInstaller(Server.MapPath("~"), forceInstall, "sqlserver");

                string currverr = AppInstaller.ProductVersion;
                string instverr = AppInstaller.Version;
                if (!AppInstaller.IsNewInstall && !forceInstall)
                {
                    //Redirect to a separate wizard for upgrade
                    Response.Redirect(ResolveUrl("~/Install/Maintenance.aspx"));
                }

                //Attempt to parse the application root URLs
                string url = Context.Request.Url.AbsoluteUri.Replace(Context.Request.Url.AbsolutePath, String.Empty);

                int urlParamStart = url.IndexOf("?");

                if (urlParamStart > 0)
                {
                    url = url.Substring(0, urlParamStart);
                }

                _applicationURL.Text = url;

                string absPath = Context.Request.Url.AbsolutePath;

                // attempt to parse out the /install folder....
                if (absPath.ToLower().LastIndexOf("/install/default.aspx") >= 0)
                {
                    absPath = absPath.Substring(0, absPath.ToLower().LastIndexOf("/install/default.aspx"));
                }
                else if (absPath.ToLower().LastIndexOf("/install/") >= 0)
                {
                    absPath = absPath.Substring(0, absPath.ToLower().LastIndexOf("/install/"));
                }

                _applicationRoot.Text = absPath;

                _SMTPConfigurator.Port = "25";
                _SMTPConfigurator.UseSmtpAuthentication = true;

            }
            else
            {
                if (AppInstaller == null)
                {
                    Response.Redirect("~/Install/Default.aspx", true);
                }
                else
                {
                    bool isSlaAccepted;
                    AppInstaller.IsSlaAccepted = bool.TryParse(_isSlaConfirmed.Value, out isSlaAccepted) && isSlaAccepted;
                }
            }

            //BuildDatabaseList();
        }
        

        #region Event handlers

        /// <summary>
        /// Handles the finish button click event of the wizard
        /// - Performs final validation then runs the installation process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InstallWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            AppInstaller.InstallSuccess = false;

            ClientScript.RegisterStartupScript(GetType(), "InstallProgress", "$(document).ready(function(){showDialog('" + ResolveUrl("~/Install/Install.aspx") + "', 240, 450);});", true);
            //Response.Redirect(ResolveUrl("~/Install/Install.aspx"));
        }


        /// <summary>
        /// Indicates whether we are upgrading an existing Checkbox installation
        /// </summary>
        protected bool isUpgrade
        {
            get
            {
                return _upgradeRad.Checked;
            }
        }

        /// <summary>
        /// Handles the next button click of the wizard
        /// - Handles step-specific validation while moving forward through the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InstallWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            AppInstaller.InstallType = _upgradeRad.Checked ? "upgrade" : "install";

            _upgradeLiteral.Text = isUpgrade
                ? "upgrade your existing Checkbox&reg; 4 installation to version 6"
                : "create a new Checkbox&reg; 6 installation";

            if (String.Compare(_installWizard.WizardSteps[e.CurrentStepIndex].ID, "WelcomeStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (_failAspNetPanel.Visible
                    || _filePermissionsFail.Visible)
                {
                    e.Cancel = true;
                }

                //check for SLA 
                if (!AppInstaller.IsSlaAccepted)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "slaModal",
                                                            "$(document).ready(function() { showDialog('slaModal', 190, 620); });",
                                                            true);
                    e.Cancel = true;
                }
            }
            
            //Database connection validation
            else if (String.Compare(_installWizard.WizardSteps[e.CurrentStepIndex].ID, "DatabaseStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                #region Database validation

                //Check to see if we've just come back from the DB exists dialog
                if (String.Compare(_dbExistsDialogResult.Value, "NotShown", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    //Check if the database exists
                    Boolean dbExists = false;
                    String connectionString = _checkboxDBSelector.ConnectionString;

                    String errorMessage;
                    dbExists = AppInstaller.TestDBConnectivity(connectionString, out errorMessage);
                    if (!dbExists)
                    {
                        if (_checkboxDBSelector.ConnectionType == CheckboxWeb.Install.Controls.DatabaseSelector.ConnectionStringType.FreeForm)
                        {
                            ShowConnectionError(GetLocalResourceObject("FreeFormConnectionError") + errorMessage);
                            e.Cancel = true;
                        }
                        else
                        {
                            ShowConnectionError(GetLocalResourceObject("ConnectionError") + "<b>" + errorMessage + "</b>");
                            e.Cancel = true;
                        }
                        
                        //If we get here, show connection error
                        return;
                    }

                    //If all went well, set the database properties on the installer
                    AppInstaller.InstallDatabase = true;
                    AppInstaller.InstallConnectionString = connectionString;
                    AppInstaller.DatabaseProvider = "sqlserver";

                    //Load the scripts since the DB info is now available
                    AppInstaller.LoadInstallFilesAndScripts();

                    //Now check to see if Checkbox has already been installed 
                    int? version;
                    Boolean previouslyInstalled = AppInstaller.TestForExistingDatabase(connectionString, out version);

                    //Check upgrade cases
                    if (isUpgrade)
                    {
                        if (!previouslyInstalled)
                        {
                            ShowConnectionError("Unable to find Checkbox 4 information in specified database. Please verify database information and ensure that specified login has access to Checkbox tables.");
                            e.Cancel = true;
                            return;
                        }

                        if (version.HasValue && version >= 5)
                        {
                            ShowConnectionError(string.Format("Version of provided database is {0}. Please, follow the Patch Guide in order to update provided database.", decimal.Round(version.Value)));
                            e.Cancel = true;
                            return;
                        }

                        //If we get here, systems are go for upgrade
                        return;
                    }

                    //Otherwise check install case
                    if (previouslyInstalled)
                    {
                        Session["dbPW"] = _checkboxDBSelector.Password;
                        Page.ClientScript.RegisterStartupScript(GetType(), "dbExistsDialog", "$(document).ready(function() { showDialog('dbExistsModal', 220, 570); });", true);
                        e.Cancel = true;
                    }
                }
                else if (String.Compare(_dbExistsDialogResult.Value, "UseDB", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    AppInstaller.InstallDatabase = false;
                    _dbExistsDialogResult.Value = "NotShown";
                }
                else if (String.Compare(_dbExistsDialogResult.Value, "Overwrite", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    AppInstaller.InstallDatabase = true;
                    _dbExistsDialogResult.Value = "NotShown";
                }
                else //Change settings
                {
                    e.Cancel = true;
                    _dbExistsDialogResult.Value = "NotShown";
                }

                #endregion
            }
            else if (String.Compare(_installWizard.WizardSteps[e.CurrentStepIndex].ID, "EMailDatabaseStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (_MSSmodeSES.Checked)
                {
                    //Check to see if we've just come back from the DB exists dialog
                    if (String.Compare(_emailDbExistsDialogResult.Value, "NotShown", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        String connectionString = _emailDBSelector.ConnectionString;

                        String errorMessage;
                        bool dbExists = AppInstaller.TestDBConnectivity(connectionString, out errorMessage);
                        if (!dbExists)
                        {
                            if (_emailDBSelector.ConnectionType ==
                                CheckboxWeb.Install.Controls.DatabaseSelector.ConnectionStringType.FreeForm)
                            {
                                ShowEmailConnectionError(GetLocalResourceObject("FreeFormConnectionError") +
                                                         errorMessage);
                                _MSSmodeSMTP.Checked = true;
                                e.Cancel = true;
                            }
                            else
                            {
                                ShowEmailConnectionError(GetLocalResourceObject("ConnectionError") + "<b>" + errorMessage + "</b>");
                                _MSSmodeSMTP.Checked = true;
                                e.Cancel = true;
                            }
                        }

                        AppInstaller.InstallEMailDatabase = true;
                        AppInstaller.EmailDBConnectionString = connectionString;

                        AppInstaller.AddSetting("SystemEmailAddress", _EMailOptions.SystemEmailAddress);
                        AppInstaller.AddSetting("DefaultEmailFromName", _EMailOptions.DefaultInvitationSenderName);

                        //Now check to see if Checkbox Messaging Service database has already been installed 
                        bool previouslyInstalled = AppInstaller.TestForExistingMailDatabase(connectionString);

                        //Otherwise check install case
                        if (previouslyInstalled)
                        {
                            Session["dbPW"] = _checkboxDBSelector.Password;
                            Page.ClientScript.RegisterStartupScript(GetType(), "mailDbExistsDialog",
                                                                    "$(document).ready(function() { showDialog('mailDbExistsModal', 220, 570); });",
                                                                    true);
                            e.Cancel = true;
                        }
                    }
                    else if (String.Compare(_emailDbExistsDialogResult.Value, "UseDB", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        AppInstaller.InstallEMailDatabase = false;
                        _emailDbExistsDialogResult.Value = "NotShown";
                    }
                    else if (String.Compare(_emailDbExistsDialogResult.Value, "Overwrite", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        AppInstaller.InstallEMailDatabase = true;
                        _emailDbExistsDialogResult.Value = "NotShown";
                    }
                    else //Change settings
                    {
                        e.Cancel = true;
                        _emailDbExistsDialogResult.Value = "NotShown";
                    }
                }
                else if (_MSSmodeSMTP.Checked)
                {
                    AppInstaller.AddSetting("EmailEnabled", (_MSSmodeSES.Checked || _MSSmodeSMTP.Checked).ToString());
                    AppInstaller.AddSetting("SmtpServer", _SMTPConfigurator.ServerAddress);
                    AppInstaller.AddSetting("SystemEmailAddress", _EMailOptions.SystemEmailAddress);
                    AppInstaller.AddSetting("DefaultEmailFromName", _EMailOptions.DefaultInvitationSenderName);
                    AppInstaller.AddSetting("EnableSmtpSsl", _SMTPConfigurator.EnableSSL.ToString());

                    AppInstaller.AddSetting("EnableSmtpAuthentication", _SMTPConfigurator.UseSmtpAuthentication.ToString());
                    if (_SMTPConfigurator.UseSmtpAuthentication)
                    {
                        AppInstaller.AddSetting("SmtpUserName", _SMTPConfigurator.Username);
                        AppInstaller.AddSetting("SmtpPassword", _SMTPConfigurator.Password);
                    }

                    AppInstaller.AddSetting("InsertLineBreaksInEmails", _EMailOptions.LimitEmailLineLength.ToString());
                    if (_EMailOptions.LimitEmailLineLength)
                    {
                        AppInstaller.AddSetting("MaximumEmailLineLength", _EMailOptions.LineLength);
                    }
                }

                AppInstaller.AddSetting("EmailEnabled", _MSSmodeSES.Checked || _MSSmodeSMTP.Checked ? "true" : "false");
                AppInstaller.AddSetting("MSSMode", _MSSmodeSES.Checked ? "SES" : "SMTP");
            }
            else if (String.Compare(_installWizard.WizardSteps[e.CurrentStepIndex].ID, "AdditionalInfo", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AppInstaller.AdminUserName = Utilities.AdvancedHtmlEncode(_adminUsernameTxt.Text);
                AppInstaller.AdminPassword = _adminPasswordTxt.Text;

                _finalInstructionsTxt.Text = isUpgrade
                    ? "When the configuration process has completed, you will be redirected to the login screen. You will be able to login using your Checkbox 4 login and password.  <br /><br /> Please note that the upgrade may take several minutes for large databases."
                    : string.Format("When the configuration process has completed, you will be redirected to the login screen. The default username is <b>\"{0}\"</b> and password is <b>\"{1}\"</b>.",
                                    _adminUsernameTxt.Text, AppInstaller.AdminPassword);

                AppInstaller.ApplicationRoot = _applicationRoot.Text;
                AppInstaller.ApplicationURL = _applicationURL.Text;

                if (_timeZone.SelectedValue != "None")
                {
                    AppInstaller.AddSetting("TimeZone", _timeZone.SelectedValue);
                }
            }
            
            /*else if (String.Compare(_installWizard.WizardSteps[e.CurrentStepIndex].ID, "ConfigStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                foreach (ListItem item in _sampleList.Items)
                {
                    if (String.Compare(item.Value, "Surveys", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        _appInstaller.InstallSampleSurveys = item.Selected;
                    }
                    else if (String.Compare(item.Value, "Styles", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        _appInstaller.InstallSampleStyles = item.Selected;
                    }
                    else if (String.Compare(item.Value, "Libraries", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        _appInstaller.InstallSampleLibraries = item.Selected;
                    }
                }
                
                Session["_appInstaller"] = _appInstaller;
            }*/
        }

        ///// <summary>
        ///// Handle changing of install connection type
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void ConnectionTypeDrop_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //    if (_connectionTypeDrop.SelectedValue == "Standard")
        //    {
        //        _standardSecurityPlace.Visible = true;
        //        _trustedConnectionPlace.Visible = false;
        //        _freeFormPlace.Visible = false;
        //    }
        //    else if (_connectionTypeDrop.SelectedValue == "Trusted")
        //    {
        //        _standardSecurityPlace.Visible = false;
        //        _trustedConnectionPlace.Visible = true;
        //        _freeFormPlace.Visible = false;
        //    }
        //    else
        //    {
        //        _standardSecurityPlace.Visible = false;
        //        _trustedConnectionPlace.Visible = false;
        //        _freeFormPlace.Visible = true;
        //    }

        //}

/*        /// <summary>
        /// Handle changing of application connection type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> */
        //protected void AppConnectionTypeDrop_SelectedIndexChanged(object sender, System.EventArgs e)
        //{

        //    if (_appConnectionTypeDrop.SelectedValue == "Standard")
        //    {
        //        _appStandardSecurityPlace.Visible = true;
        //        _appTrustedConnectionPlace.Visible = false;
        //    }
        //    else
        //    {
        //        _appStandardSecurityPlace.Visible = false;
        //        _appTrustedConnectionPlace.Visible = true;
        //    }

        //}

        #endregion

        #region Utility

        /// <summary>
        /// Escapes any reserved characters contained in a specified value.
        /// </summary>
        /// <param name="value">The database configuration value to sanities.</param>
        /// <returns>The escaped value.</returns>
        private static string EscapeValue(string value)
        {
            //see: for more information. http://msdn2.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring(VS.80).aspx
            //The document details all the possible cases where a value needs to be escaped.
            //The worst case scenario is when a value contains semicolons, double quotes and single quotes.
            //In this situation you need to replace " with two "s and surrounded the value with double quotes.
            //Addressing the worst case scenario handles all other cases.

            if (value == null)
            {
                return value;
            }

            if (value.Contains(";") || value.Contains("\"") || value.Contains("'"))
            {
                return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
            }
            
            return value;
        }

        private void ShowConnectionError(String errorMessage)
        {
            _connectionErrorPanel.Visible = true;
            _connectionErrorMessage.Text = errorMessage;
        }

        private void ShowEmailConnectionError(String errorMessage)
        {
            _connectionEmailErrorPanel.Visible = true;
            _connectionEmailErrorMessage.Text = errorMessage;
        }

        private static String MakeButtonEventTarget(String controlClientID)
        {
            String eventTarget = Checkbox.Web.UI.Controls.Adapters.WebControlAdapterExtender.MakeNameFromId(controlClientID);
            eventTarget = "_" + eventTarget.Substring(1);
            eventTarget += "$ctl00";
            return eventTarget;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetEMailAddress()
        {
            return _EMailOptions.SystemEmailAddress;
        }


        #endregion

        #region Prerequisites
        /// <summary>
        /// Check permissions for installation
        /// </summary>
        protected void CheckPermissions()
        {
            bool errorsFound = false;
            string webConfigPath = Request.ServerVariables["APPL_PHYSICAL_PATH"] + "web.config";
            //string otherConfigPath = Request.ServerVariables["APPL_PHYSICAL_PATH"] + "Config";

            var sb = new StringBuilder();

            sb.Append("<div class=\"dialogSubTitle\">Permission errors were found.  To resolve these errors, ensure the specified User Context has read/write access to the listed files.</div>");

            FileStream theStream;

            //Make sure files can be written
            try
            {
                if (File.Exists(webConfigPath))
                {
                    if ((File.GetAttributes(webConfigPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        sb.AppendLine("<div class=\"error\">Unable to write to <span style=\"font-family:Courier New;\">" + webConfigPath + "</span> because it is Read-only.</div>");
                        errorsFound = true;
                    }
                    else
                    {
                        theStream = File.OpenWrite(webConfigPath);
                        theStream.Close();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                sb.AppendLine("<div class=\"error\">Unable to write to <span style=\"font-family:Courier New;\">" + webConfigPath + "</span> due to a permissions error.</div>");
                errorsFound = true;
            }

            //Only web.config written now during install
/*
            try
            {
                if (!Directory.Exists(otherConfigPath))
                {
                    sb.AppendLine("<li>Config directory <span style=\"font-family:Courier New;\">" + otherConfigPath + "</span> can't be read.</li>");
                    errorsFound = true;
                }
                else
                {
                    string[] configFiles = Directory.GetFiles(otherConfigPath);

                    foreach (string configFile in configFiles)
                    {
                        if (configFile.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                        {
                            try
                            {
                                if ((File.GetAttributes(configFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                {
                                    sb.AppendLine("<li>Unable to write to <span style=\"font-family:Courier New;\">" + configFile + "</span> because it is Read-only.</li>");
                                    errorsFound = true;
                                }
                                else
                                {
                                    theStream = File.OpenWrite(configFile);
                                    theStream.Close();
                                }
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                sb.AppendLine("<li>Unable to write to <span style=\"font-family:Courier New;\">" + configFile + "</span> due to a permissions error.</li>");
                                errorsFound = true;
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                sb.AppendLine("<li>Unable to open <span style=\"font-family:Courier New\";" + otherConfigPath + "</span>.</li>");
                errorsFound = true;
            }*/

            if (errorsFound)
            {
                 _filePermissionsSuccess.Visible = false;
                _filePermissionsFail.Visible = true;
                _filePermissionsErrorContainer.Visible = true;
                _filePermissionsErrorContainer.Controls.Add(new Literal { Text = sb.ToString() });
                _navEnabled = false;
                
            }
            else
            {
                _filePermissionsErrorContainer.Visible = false;
                _filePermissionsFail.Visible = false;
                _filePermissionsSuccess.Visible = true;
               
            }
        }
        #endregion
    }
}
