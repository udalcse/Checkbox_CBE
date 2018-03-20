using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Messaging.Email;
using CheckboxWeb.Install.Controls;
using System.Web.Configuration;
using System.Configuration;
using Checkbox.Configuration.Install;

namespace CheckboxWeb.Settings
{
    public partial class Email : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.OkClick += Master_OkClick;
            Master.CancelVisible = false;

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/email.aspx/title");

            _emailOptions.SystemEmailAddressValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/systemEmailAddress/requiredField");
            _emailOptions.DefaultInvitationSenderNameValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/defaultSenderName/requiredField");
            _emailOptions.LineLengthValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/lineLengthinvalidFormat/");
            _emailOptions.SystemEmailAddressCaption = WebTextManager.GetText("/pageText/settings/email.aspx/systemEmailAddress");
            _emailOptions.DefaultInvitationSenderNameCaption = WebTextManager.GetText("/pageText/settings/email.aspx/defaultSenderName");
            _emailOptions.LineLengthCaption = WebTextManager.GetText("/pageText/settings/email.aspx/lineLength");
            _emailOptions.LimitEmailLineLengthCaption = WebTextManager.GetText("/pageText/settings/email.aspx/limitEmailLineLength");           

            _SMTPConfigurator.ServerAddressValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/serverAddress/requiredField");
            _SMTPConfigurator.PortRequiredValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/port/requiredField");
            _SMTPConfigurator.PortValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/port/invalidFormat");
            _SMTPConfigurator.UsernameValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/username/requiredField");
            _SMTPConfigurator.PasswordValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/password/requiredField");
            _SMTPConfigurator.ServerAddressCaption = WebTextManager.GetText("/pageText/settings/email.aspx/serverAddress");
            _SMTPConfigurator.PortCaption = WebTextManager.GetText("/pageText/settings/email.aspx/port");
            _SMTPConfigurator.EnableSSLCaption = WebTextManager.GetText("/pageText/settings/email.aspx/enableSsl");
            _SMTPConfigurator.UseSmtpAuthenticationCaption = WebTextManager.GetText("/pageText/settings/email.aspx/useSmtpAuthentication");
            _SMTPConfigurator.UsernameCaption = WebTextManager.GetText("/pageText/settings/email.aspx/username");
            _SMTPConfigurator.PasswordCaption = WebTextManager.GetText("/pageText/settings/email.aspx/password");
            _SMTPConfigurator.TestSMTPCaption = WebTextManager.GetText("/pageText/settings/email.aspx/testSmtp");
            _SMTPConfigurator.GetEMailAddressCallback = new SMTPConfigurator.GetEMailAddressDelegate(GetEmailAddress);

            _emailDBSelector.SQLServerAuthenticationCaption = WebTextManager.GetText("/pageText/settings/email.aspx/SQLServerAuthenticationCaption");
            _emailDBSelector.WindowsAuthenticationCaption = WebTextManager.GetText("/pageText/settings/email.aspx/WindowsAuthenticationCaption");
            _emailDBSelector.FreeformConnectionStringCaption = WebTextManager.GetText("/pageText/settings/email.aspx/FreeformConnectionStringCaption");
            _emailDBSelector.ServerTxtLabel = WebTextManager.GetText("/pageText/settings/email.aspx/ServerTxtLabel");
            _emailDBSelector.DbNameTxtLabel = WebTextManager.GetText("/pageText/settings/email.aspx/DbNameTxtLabel");
            _emailDBSelector.UsernameTxtLabel = WebTextManager.GetText("/pageText/settings/email.aspx/UsernameTxtLabel");
            _emailDBSelector.PasswordTxtLabel = WebTextManager.GetText("/pageText/settings/email.aspx/PasswordTxtLabel");
            _emailDBSelector.TrustedServerLabel = WebTextManager.GetText("/pageText/settings/email.aspx/TrustedServerLabel");
            _emailDBSelector.TrustedDbNameLabel = WebTextManager.GetText("/pageText/settings/email.aspx/TrustedDbNameLabel");
            _emailDBSelector.FreeformConnectionStringLabel = WebTextManager.GetText("/pageText/settings/email.aspx/FreeformConnectionStringLabel");
            _emailDBSelector.RequiredFieldValidatorMessage = WebTextManager.GetText("/pageText/settings/email.aspx/RequiredFieldValidatorMessage");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

           
            if (!IsPostBack)
            {
                _emailEnabled.Checked = ApplicationManager.AppSettings.EmailEnabled
                    || "true".Equals(Request["isRefresh"], StringComparison.InvariantCultureIgnoreCase);

                _SMTPConfigurator.ServerAddress = ApplicationManager.AppSettings.SmtpServer;
                _SMTPConfigurator.Port = ApplicationManager.AppSettings.SmtpPort.ToString();

                _emailOptions.SystemEmailAddress = ApplicationManager.AppSettings.SystemEmailAddress;
                _emailOptions.DefaultInvitationSenderName = ApplicationManager.AppSettings.DefaultEmailFromName;
                _SMTPConfigurator.EnableSSL = ApplicationManager.AppSettings.EnableSmtpSsl;
                _SMTPConfigurator.UseSmtpAuthentication = ApplicationManager.AppSettings.EnableSmtpAuthentication;
                _SMTPConfigurator.Username = ApplicationManager.AppSettings.SmtpUserName;
                _SMTPConfigurator.Password = ApplicationManager.AppSettings.SmtpPassword;
                _emailOptions.LimitEmailLineLength = ApplicationManager.AppSettings.LimitEmailMessageLineLength;
                _emailOptions.LineLength = ApplicationManager.AppSettings.MaxEmailMessageLineLength.ToString();                
                
                if (!ApplicationManager.AppSettings.EnableMultiDatabase)
                {
                    if (EmailGateway.HasBatchSupportiveProvider)
                    {
                        _emailServiceUnavailableOption.Visible = false;
                        _MSSmodeSES.Enabled = true;
                        _MSSmodeSMTP.Enabled = true;
                        if (ApplicationManager.AppSettings.MSSMode.Equals("SES"))
                            _MSSmodeSES.Checked = true;
                        else
                            _MSSmodeSMTP.Checked = true;

                        _emailDBSelector.ConnectionString = ApplicationManager.AppSettings.MailDbConnectionString;
                    }
                    else
                    {
                        _emailServiceUnavailableOption.Visible = true;
                        _MSSmodeSES.Enabled = false;
                        _MSSmodeSMTP.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetEmailAddress()
        {
            return _emailOptions.SystemEmailAddress;
        }

        protected string MailDBConnectError
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private string SesConnectionString
        {
            get
            {
                string cs = Session["_sesConnectionString"] as string;
                return string.IsNullOrEmpty(cs) ? string.Empty : cs;
            }
            set { Session["_sesConnectionString"] = value; }
        }

        private bool ValidateInputs()
        {
            bool valid = true;

            if ((!_SMTPConfigurator.ValidateInputs() || !_emailOptions.ValidateInputs()) && !_MSSmodeSES.Checked)
            {
                return false;
            }

            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                if (EmailGateway.HasBatchSupportiveProvider)
                {
                    if (_MSSmodeSES.Checked)
                    {
                        try
                        {
                            System.Data.SqlClient.SqlConnection connect = new System.Data.SqlClient.SqlConnection(_emailDBSelector.ConnectionString);
                            connect.Open();
                            connect.Close();
                            SesConnectionString = _emailDBSelector.ConnectionString;
                        }
                        catch (Exception ex)
                        {
                            if (!string.IsNullOrEmpty(SesConnectionString))
                            {
                                try
                                {
                                    System.Data.SqlClient.SqlConnection connect =
                                        new System.Data.SqlClient.SqlConnection(SesConnectionString);
                                    connect.Open();
                                    connect.Close();
                                }
                                catch (Exception)
                                {
                                    MailDBConnectError = ex.Message;
                                    _emailDBConnectionFailed.Visible = true;
                                    valid = false;
                                }
                            }
                            else
                            {
                                MailDBConnectError = ex.Message;
                                _emailDBConnectionFailed.Visible = true;
                                valid = false;
                            }
                        }
                    }
                }
            }

            return valid;
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (_emailEnabled.Checked)
            {
                bool success = true;
                if (ValidateInputs())
                {
                    ApplicationManager.AppSettings.EmailEnabled = true;
                    ApplicationManager.AppSettings.SmtpServer = _SMTPConfigurator.ServerAddress;
                    ApplicationManager.AppSettings.SmtpPort = Int32.Parse(_SMTPConfigurator.Port);
                    ApplicationManager.AppSettings.SystemEmailAddress = _emailOptions.SystemEmailAddress;
                    ApplicationManager.AppSettings.DefaultEmailFromName = _emailOptions.DefaultInvitationSenderName;
                    ApplicationManager.AppSettings.EnableSmtpSsl = _SMTPConfigurator.EnableSSL;

                    ApplicationManager.AppSettings.EnableSmtpAuthentication = _SMTPConfigurator.UseSmtpAuthentication;
                    if (_SMTPConfigurator.UseSmtpAuthentication)
                    {
                        ApplicationManager.AppSettings.SmtpUserName = _SMTPConfigurator.Username;
                        ApplicationManager.AppSettings.SmtpPassword = _SMTPConfigurator.Password;                        
                    }

                    ApplicationManager.AppSettings.LimitEmailMessageLineLength = _emailOptions.LimitEmailLineLength;
                    if (_emailOptions.LimitEmailLineLength)
                    {
                        ApplicationManager.AppSettings.MaxEmailMessageLineLength = Int32.Parse(_emailOptions.LineLength);
                    }

                    if (!ApplicationManager.AppSettings.EnableMultiDatabase)
                    {
                        if (EmailGateway.HasBatchSupportiveProvider)
                        {
                            _emailDBConnectionFailed.Visible = false;
                            try
                            {                                
                                ApplicationManager.AppSettings.MSSMode = _MSSmodeSES.Checked ? "SES" : "SMTP";
                                if (_MSSmodeSES.Checked)
                                {
                                    var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                                    var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                                    if (section.ConnectionStrings["MailDbConnectionString"].ConnectionString != SesConnectionString)
                                    {
                                        ApplicationInstaller installer = new ApplicationInstaller(Server.MapPath("~"));
                                        installer.EmailDBConnectionString = SesConnectionString;
                                        installer.DatabaseProvider = "sqlserver";
                                        string connectionError = "";
                                        if (!installer.TestDBConnectivity(SesConnectionString, out connectionError))
                                        {
                                            MailDBConnectError = connectionError;
                                            return;
                                        }
                                        else
                                        {
                                            if (_emailDBOperationConfirmed.Value != "no")
                                            {
                                                string installError = "";
                                                if (installer.InstallIsNeededForEmailDB())
                                                {
                                                    if (!InstallEmailDBScripts("install", installer))
                                                        return;
                                                }
                                                if (installer.UpgradeIsNeededForEmailDB())
                                                {
                                                    if (!InstallEmailDBScripts("upgrade", installer))
                                                        return;
                                                }
                                            }
                                            else
                                            {
                                                _emailDBOperationConfirmed.Value = null;
                                            }
                                        }
                                        section.ConnectionStrings["MailDbConnectionString"].ConnectionString = SesConnectionString;
                                        configuration.Save();
                                    }
                                }
                                Global.InitializeEMailProvider();
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                _emailDBConnectionFailed.Visible = true;
                                MailDBConnectError = ex.Message;
                            }
                            
                        }
                    }

                    if (success)
                    {
                        Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
                        return;
                    }
                }

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/email.aspx/validationError"), StatusMessageType.Warning);
            }
            else
            {
                ApplicationManager.AppSettings.EmailEnabled = false;
                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }

            
        }

        /// <summary>
        /// Install email scripts using initialized installer
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="installer"></param>
        /// <returns></returns>
        private bool InstallEmailDBScripts(string mode, ApplicationInstaller installer)
        {
            string installError = "";
            installer.LoadInstallEmailFilesAndScripts(mode);
            if (_emailDBOperationConfirmed.Value == "yes")
            {
                _emailDBOperationConfirmed.Value = null;
                if (!installer.RunEMailDatabaseScripts(out installError))
                {
                    MailDBConnectError = installError;
                    return false;
                };
            }
            else
            {
                showEmailDBDialog(mode);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Text for confirmation
        /// </summary>
        protected string EMailDBConfirmation
        {
            get;
            set;
        }


        /// <summary>
        /// Shows a dialog with confirmation
        /// </summary>
        /// <param name="operation"></param>
        private void showEmailDBDialog(string mode)
        {
            EMailDBConfirmation = WebTextManager.GetText("/pageText/settings/email.aspx/emaildbconfirm/" + mode);
            Page.ClientScript.RegisterStartupScript(GetType(), "dbEmailDialog", "showEmailDBConfirmDialog();", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static string QDelimiter(int count)
        {
            return count == 0 ? "?" : "&";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack && ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                ApplicationManager.AppSettings.EmailEnabled = _emailEnabled.Checked;
                ApplicationManager.AppSettings.SystemEmailAddress = _emailOptions.SystemEmailAddress;
                ApplicationManager.AppSettings.DefaultEmailFromName = _emailOptions.DefaultInvitationSenderName;
                ApplicationManager.AppSettings.LimitEmailMessageLineLength = _emailOptions.LimitEmailLineLength;

                int limit;
                bool parsed = int.TryParse(_emailOptions.LineLength, out limit);
                if (_emailOptions.LimitEmailLineLength && parsed)
                    ApplicationManager.AppSettings.MaxEmailMessageLineLength = limit;

                if (!parsed)
                    _emailOptions.LineLength = ApplicationManager.AppSettings.MaxEmailMessageLineLength.ToString();
            }
        }
    }
}