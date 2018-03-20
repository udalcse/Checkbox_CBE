using System;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UsersSecurity : SettingsPage
    {
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                _maxFailedLoginAttempts.Text = ApplicationManager.AppSettings.MaxFailedLoginAttempts == -1 ? "" : ApplicationManager.AppSettings.MaxFailedLoginAttempts.ToString();
                _minPasswordLength.Text = ApplicationManager.AppSettings.MinPasswordLength == -1 ? "" : ApplicationManager.AppSettings.MinPasswordLength.ToString();
                _minPasswordNonAlphaNumeric.Text = ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric == -1 ? "" : ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric.ToString();
            }

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));

            base.OnLoad(e);
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            Master.OkClick += new EventHandler(Master_OkClick);

            base.OnPageInit();

        }

        /// <summary>
        /// Encryption is handled in HashPasswords dialog. This handler only updates the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _encryptPasswordsBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            bool valid = true;
            int tmp = 0;
            valid &= _maxFailedLoginAttempts.Text == "" || int.TryParse(_maxFailedLoginAttempts.Text, out tmp);
            valid &= _minPasswordLength.Text == "" || int.TryParse(_minPasswordLength.Text, out tmp);
            valid &= _minPasswordNonAlphaNumeric.Text == "" || int.TryParse(_minPasswordNonAlphaNumeric.Text, out tmp);
            return valid;
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                ApplicationManager.AppSettings.MaxFailedLoginAttempts = string.IsNullOrEmpty(_maxFailedLoginAttempts.Text) ? -1 : int.Parse(_maxFailedLoginAttempts.Text);
                ApplicationManager.AppSettings.MinPasswordLength = string.IsNullOrEmpty(_minPasswordLength.Text) ? -1 : int.Parse(_minPasswordLength.Text);
                ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric = string.IsNullOrEmpty(_minPasswordNonAlphaNumeric.Text) ? -1 : int.Parse(_minPasswordNonAlphaNumeric.Text);

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }
      
    }
}
