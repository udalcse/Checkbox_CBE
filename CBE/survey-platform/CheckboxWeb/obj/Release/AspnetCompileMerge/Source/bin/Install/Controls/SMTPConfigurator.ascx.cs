using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Install.Controls
{
    /// <summary>
    /// SMTP Options Editor
    /// </summary>
    public partial class SMTPConfigurator : Checkbox.Web.Common.UserControlBase
    {
        #region Control Texts
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string ServerAddressValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PortRequiredValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PortValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string UsernameValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PasswordValidatorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string ServerAddressCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PortCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string EnableSSLCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string UseSmtpAuthenticationCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string UsernameCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PasswordCaption
        {
            get;
            set;
        }
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string TestSMTPCaption
        {
            get;
            set;
        }
        #endregion

        #region Public Data Properties
        /// <summary>
        /// SMTP Server Address
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return _serverAddress.Text;
            }
            set
            {
                _serverAddress.Text = value;
            }
        }
        /// <summary>
        /// Port
        /// </summary>
        public string Port
        {
            get
            {
                return _port.Text;
            }
            set
            {
                _port.Text = value;
            }
        }
        /// <summary>
        /// Enable SSL
        /// </summary>
        public bool EnableSSL
        {
            get
            {
                return _enableSsl.Checked;
            }
            set
            {
                _enableSsl.Checked = value;
            }
        }
        /// <summary>
        /// Authenticate or not
        /// </summary>
        public bool UseSmtpAuthentication
        {
            get
            {
                return _useSmtpAuthentication.Checked;
            }
            set
            {
                _useSmtpAuthentication.Checked = value;
            }
        }
        /// <summary>
        /// SMTP User name
        /// </summary>
        public string Username
        {
            get
            {
                return _username.Text;
            }
            set
            {
                _username.Text = value;
            }
        }
        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get
            {
                return _password.Text;
            }
            set
            {
                _password.Text = value;
            }
        }
        #endregion Public Properties

        /// <summary>
        /// Validate user input
        /// </summary>
        /// <returns></returns>
        public bool ValidateInputs()
        {
            bool valid = true;
            if (string.Equals(_serverAddress.Text, "None Configured"))
            {
                _serverAddress.Text = String.Empty;
                _serverAddressValidator.IsValid = false;
                _serverAddress.Focus();
            }

            if (string.IsNullOrEmpty(_serverAddress.Text))
            {
                _serverAddressValidator.Visible = true;
                valid = false;
            }

            if (string.IsNullOrEmpty(_port.Text))
            {
                _portRequiredValidator.Visible = true;
                valid = false;
            }

            int port;
            if (!Int32.TryParse(_port.Text, out port))
            {
                _portValidator.Visible = true;
                valid = false;
            }

            if (_useSmtpAuthentication.Checked)
            {
                if (string.IsNullOrEmpty(_username.Text))
                {
                    _usernameValidator.Visible = true;
                    valid = false;
                }

                if (string.IsNullOrEmpty(_password.Text))
                {
                    _passwordValidator.Visible = true;
                    valid = false;
                }
            }

            return valid;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _enableSsl.Text = EnableSSLCaption;
            _useSmtpAuthentication.Text = UseSmtpAuthenticationCaption;
            _testSmtp.Text = TestSMTPCaption;
            _password.Attributes["value"] = _password.Text;
        }

        /// <summary>
        /// Delegate for recieving sender email from another control
        /// </summary>
        /// <returns></returns>
        public delegate string GetEMailAddressDelegate();
        /// <summary>
        /// Callback for recieving sender email from another control
        /// </summary>
        public GetEMailAddressDelegate GetEMailAddressCallback;

        /// <summary>
        /// Call the send email dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EmailTest_Click(object sender, EventArgs e)
        {
            Session["Email_enableSsl"] = EnableSSL;
            Session["Email_useSmtpAuthentication"] = UseSmtpAuthentication;
            Session["Email_port"] = Port;
            Session["Email_serverAddress"] = ServerAddress;
            if (GetEMailAddressCallback != null)
                Session["Email_systemEmailAddress"] = GetEMailAddressCallback();;
            Session["Email_username"] = Username;
            Session["Email_password"] = Password;

            //Response.Redirect("TestSmtp.aspx", false);
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "testSmtp",
                "showDialog('TestSmtp.aspx?onClose=onDialogClosed', 550, 650);",
                true);
        }

    }
}
