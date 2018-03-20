using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using Checkbox.Web.Page;

namespace CheckboxWeb.Install.Controls
{
    public partial class TestSMTP : Checkbox.Web.Common.UserControlBase
    {
        public string Host
        {
            get { return _mailHostUI.Text.Trim(); }
        }

        public int Port
        {
            get
            {
                int value;
                if (Int32.TryParse(_portUI.Text.Trim(), out value))
                {
                    return value;
                }
                return 25;
            }
        }

        public bool UseSsl
        {
            get { return _enableSslUI.Checked; }
        }

        public bool UseSmtpAuthentication
        {
            get { return _useSmtpAuthenticationUI.Checked; }
        }

        public string UserName
        {
            get { return _userNameUI.Text.Trim(); }
        }

        public string Password
        {
            get { return _passwordUI.Text.Trim(); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _userNameUI.Enabled = UseSmtpAuthentication;
            _passwordUI.Enabled = UseSmtpAuthentication;
            _enableSslUI.Text = EnableSSLLabel;
            _useSmtpAuthenticationUI.Text = UseSMTPAuthenticationLabel;
            if (!IsPostBack)
            {
                _subject.Text = DefaultTitle;
                _body.Text = DefaultBody;
            }
        }


        /// <summary>
        /// Send the test email with the provided settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendEmail_ClickEvent(object sender, EventArgs e)
        {
            try
            {
                MailMessage message = new MailMessage(_from.Text, _to.Text, _subject.Text, _body.Text);

                SmtpClient client = new SmtpClient(Host, Port) { EnableSsl = UseSsl };

                if (UseSmtpAuthentication)
                {
                    string userName = _userNameUI.Text;
                    string newUserName = string.Empty;
                    string domain = string.Empty;

                    if (userName.Contains("/") || userName.Contains("\\"))
                    {
                        int indexOfForwardSlash = userName.IndexOf("/");
                        int indexOfBackSlash = userName.IndexOf("\\");

                        if (indexOfForwardSlash > 0 && indexOfForwardSlash < userName.Length - 1)
                        {
                            newUserName = userName.Substring(0, indexOfForwardSlash);
                            domain = userName.Substring(indexOfForwardSlash + 1);
                        }
                        else if (indexOfBackSlash > 0 && indexOfBackSlash < userName.Length - 1)
                        {
                            newUserName = userName.Substring(0, indexOfBackSlash);
                            domain = userName.Substring(indexOfBackSlash + 1);

                        }
                    }

                    if (domain == string.Empty || newUserName == string.Empty)
                        client.Credentials = new System.Net.NetworkCredential(UserName, Password);
                    else
                        client.Credentials = new System.Net.NetworkCredential(newUserName, Password, domain);
                }

                client.Send(message);

                string status = string.Format("{0}<br/>", SuccessStatusMessage);
                ShowStatusMessage(status, StatusMessageType.Success);
            }
            catch (Exception ex)
            {
                ShowStatusMessage(ex.ToString(), StatusMessageType.Error);
            }
        }

        private void ShowStatusMessage(string p, StatusMessageType statusMessageType)
        {
            _message.InnerHtml = p;
            _message.Visible = true;
            _message.Attributes["class"] = "message " + statusMessageType.ToString().ToLower();
        }

        #region Control Texts
        public string ToLabel { get; set; }

        public string FromLabel { get; set; }

        public string SubjectLabel { get; set; }

        public string BodyLabel { get; set; }

        public string SMTPHostLabel { get; set; }

        public string PortLabel { get; set; }

        public string EnableSSLLabel { get; set; }

        public string UseSMTPAuthenticationLabel { get; set; }

        public string UserNameLabel { get; set; }

        public string PasswordLabel { get; set; }

        public string SendTestMailLabel { get; set; }

        public string CannotBeEmptyValidatorText { get; set; }

        public string InvalidPortNumberValidatorText { get; set; }

        public string EmptyPortNumberValidatorText { get; set; }

        public string DefaultTitle { get; set; }

        public string DefaultBody { get; set; }

        public string SuccessStatusMessage { get; set; }
        #endregion Control Texts

    }
}