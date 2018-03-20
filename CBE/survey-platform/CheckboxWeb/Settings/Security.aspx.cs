using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Security : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/security.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.OkClick += Master_OkClick;

            //Show encrypt button if encryption is not enabled OR unencrypted passwords exist
            if (!ApplicationManager.AppSettings.UseEncryption)
            {
                _passwordStatusLbl.Text = WebTextManager.GetText("/pageText/settings/security.aspx/passwordHashingDisabled");
            }
            else if (UserManager.CountUnencryptedPasswords() > 0)
            {
                _passwordStatusLbl.Text = WebTextManager.GetText("/pageText/settings/security.aspx/encryptionEnabledIncomplete");
            }
            else
            {
                _passwordStatusLbl.Text = WebTextManager.GetText("/pageText/settings/security.aspx/passwordHashingEnabled");
            }

            if (_sessionType.Items.FindByValue(ApplicationManager.AppSettings.SessionMode.ToString()) != null)
            {
                _sessionType.SelectedValue = ApplicationManager.AppSettings.SessionMode.ToString();
            }

            _cookieName.Text = ApplicationManager.AppSettings.CookieName;
            _cookieName.Enabled = "Cookies".Equals(ApplicationManager.AppSettings.SessionMode.ToString());

            _allowNumericSurveyIds.Checked = ApplicationManager.AppSettings.AllowResponseTemplateIDLookup;

            // Remove this section when simple security has been fixed // 
            _simpleSecurity.Visible = false;
            // Hiding simple security option until the bugs with it can be fixed -- 
            //A second way to enable/disable simple security will be to turn it on in the Checkbox Settings screens. 
            //This option will only be available for server customers and for CheckboxOnline customers that do not have 
            //the SimpleSecurity limit set in their databases.
            
            if (ApplicationManager.AppSettings.EnableMultiDatabase && this.ActiveLicense.SimpleSecurityLimit.RuntimeLimitValue.Value)
            {
                _securityMode.Visible = false;
            }/*
            else
            {
                _simpleSecurity.Checked = ApplicationManager.AppSettings.SimpleSecurity;
            }*/

            _preventAdminAutoLogin.Checked = ApplicationManager.AppSettings.PreventAdminAutoLogin;
            
            _redirectHTTPtoHTTPS.Enabled = ApplicationManager.ApplicationURL.StartsWith("https://");
            if (_redirectHTTPtoHTTPS.Enabled)
                _redirectHTTPtoHTTPS.Checked = ApplicationManager.AppSettings.RedirectHTTPtoHTTPS;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            bool valid = true;
            _cookieNameFormatValidator.Visible = false;
            _cookieNameRequiredValidator.Visible = false;

            if ("Cookies".Equals(_sessionType.SelectedValue, StringComparison.InvariantCultureIgnoreCase))
            {
                Regex cookieFormat = new Regex("\\w");

                if (_sessionType.SelectedValue == AppSettings.SessionType.Cookies.ToString())
                {
                    if (Utilities.IsNullOrEmpty(_cookieName.Text))
                    {
                        _cookieNameRequiredValidator.Visible = true;
                        valid = false;
                    }
                    else if (!cookieFormat.Match(_cookieName.Text.Trim()).Success)
                    {
                        _cookieNameFormatValidator.Visible = true;
                        valid = false;
                    }
                }
            }

            return valid;
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                ApplicationManager.AppSettings.SessionMode = (AppSettings.SessionType)Enum.Parse(typeof (AppSettings.SessionType), _sessionType.SelectedValue);
                ApplicationManager.AppSettings.CookieName = _cookieName.Text;
                ApplicationManager.AppSettings.AllowResponseTemplateIDLookup = _allowNumericSurveyIds.Checked;
                ApplicationManager.AppSettings.SimpleSecurity = _simpleSecurity.Checked;
                ApplicationManager.AppSettings.PreventAdminAutoLogin = _preventAdminAutoLogin.Checked;
                if (_redirectHTTPtoHTTPS.Enabled)
                    ApplicationManager.AppSettings.RedirectHTTPtoHTTPS = _redirectHTTPtoHTTPS.Checked;

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }

      
        protected void SessionType_ClickEvent(object sender, EventArgs e)
        {
            bool isEnabled = "Cookies".Equals(_sessionType.SelectedValue);

            _cookieName.Enabled = isEnabled;
            _cookieNameFormatValidator.Enabled = isEnabled;
            _cookieNameRequiredValidator.Enabled = isEnabled;
        }
    }
}
