using System;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb
{
    public partial class ResetPassword : ApplicationPage
    {
        [QueryParameter("u")]
        public Guid? UserGuid { get; set; }

        private MultiLanguageButton _okBtn;
 
        /// <summary>
        /// If reset and/or email not enabled, return to login page
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

         //   string title = WebTextManager.GetText("/pageText/users/ResetPassword.aspx/title", null, "Reset Password");
         //   (Master as Admin).SetTitle(title);

            if (!ApplicationManager.AppSettings.EmailEnabled || !ApplicationManager.AppSettings.AllowPasswordReset)
                Response.Redirect(ResolveUrl("~/Login.aspx"), false);

            if (UserGuid.HasValue)
            {
                _processPlace.Visible = true;
                _enterEmailPlace.Visible = false;
                _okBtn = ValidateTicket() ? ResetConfirm : ResetBtn;
            }
            else
            {
                _processPlace.Visible = false;
                _enterEmailPlace.Visible = true;
                _okBtn = ResetBtn;
            }

            _okBtn.Click += _okBtn_Click;
            //   _cancelBtn.Click += _cancelBtn_Click;

            //Check for the available emails.
            if (_enterEmailPlace.Visible && !ApplicationManager.AppSettings.AllowSendEmail)
            {
                _emailsCannotBeSentWarningPanel.Visible = true;
                _emailTxt.Enabled = false;
                _okBtn.Enabled = false;
            }
        }

        /// <summary>
        /// Validate user ticket.
        /// </summary>
        private bool ValidateTicket()
        {

            //Ensure guid refers to a user
            CheckboxPrincipal user = null;

            if (UserGuid.HasValue)
                user = UserManager.GetUserByGuid(UserGuid.Value);

            if (user == null)
            {
                _processPlace.Visible = false;
                _enterEmailPlace.Visible = true;
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/passwordProcess.aspx/invalidTicket");

                return false;
            }

            //Ensure the ticket is valid
            if (!Ticketing.ValidateTicket(UserGuid.Value))
            {
                _processPlace.Visible = false;
                _enterEmailPlace.Visible = true;
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/passwordProcess.aspx/invalidTicket");

                return false;
            }

            _userNameLbl.Text = user.Identity.Name;

            return true;
        }

        /// <summary>
        /// Return to login page on cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cancelBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Login.aspx"), false);
        }

        /// <summary>
        /// Handle user clicking OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (_enterEmailPlace.Visible)
                {
                    ValidateAndSendEmail();
                }
                else
                {
                    DoResetPassword();
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _errorPanel.Visible = true;
                _errorLbl.Text = "An error occurred while retrieving login information: <br />" + ex.Message;
            }
        }

        /// <summary>
        /// Reset the password
        /// </summary>
        private void DoResetPassword()
        {
            //Hide error panel to start
            _errorPanel.Visible = false;
            var newPassword = _newPasswordTxt.Text.Trim();
            var confirmPassword = _confirmPasswordTxt.Text.Trim();

            if (Utilities.IsNullOrEmpty(newPassword))
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/register.aspx/passwordRequired");
                return;
            }

            if (Utilities.IsNullOrEmpty(newPassword))
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/register.aspx/confirmPasswordRequired");
                return;
            }

            if (newPassword != confirmPassword)
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/register.aspx/passwordMisMatch");
                return;
            }

            if (ApplicationManager.AppSettings.EnforcePasswordLimitsGlobally)
            {
                PasswordValidator passwordValidator = new PasswordValidator();

                if (!passwordValidator.Validate(newPassword))
                {
                    _errorPanel.Visible = true;
                    _errorLbl.Text = passwordValidator.GetMessage("en-US");
                    return;
                }
            }

            //Update user
            CheckboxPrincipal userToUpdate = UserManager.GetUserByGuid(UserGuid.Value);

            string status;

            UserManager.UpdateUser(
                userToUpdate.Identity.Name,
                userToUpdate.Identity.Name,
                string.Empty,
                newPassword,
                userToUpdate.Email,
                HttpContext.Current.User.Identity.Name,
                out status);

            _processPlace.Visible = false;

            _sendSuccessPanel.Visible = true;
            _successLbl.Text = WebTextManager.GetText("/pageText/passwordProcess.aspx/passwordUpdated2");

            _okBtn.Visible = false;
         //   _cancelBtn.Visible = false;

            GoBackLinkPanel.Visible = true;

            //Delete ticket so it can't be used again
            Ticketing.DeleteTicket(UserGuid.Value);
        }

        /// <summary>
        /// Validate user email and send reset message
        /// </summary>
        private void ValidateAndSendEmail()
        {
            //Validate input
            if (Utilities.IsNullOrEmpty(_emailTxt.Text.Trim()))
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/PasswordReset.aspx/emailRequired");
                return;
            }

            EmailValidator emailValidator = new EmailValidator();

            if (!emailValidator.Validate(_emailTxt.Text.Trim()))
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/PasswordReset.aspx/emailNotValid");
                return;
            }

            //Attempt to lookup account
            CheckboxPrincipal userPrincipal = UserManager.GetUserWithEmail(_emailTxt.Text.Trim());

            if (userPrincipal == null)
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/passwordReset.aspx/noUserFound");
                return;
            }

            //Handle network/external users
            if (!UserManager.PASSWORD_USER_AUTHENTICATION_TYPE.Equals(userPrincipal.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase))
            {
                _errorPanel.Visible = true;
                _errorLbl.Text = WebTextManager.GetText("/pageText/PasswordReset.aspx/networkUserError");
                return;
            }

            //Otherwise, send message
            SendMessage(userPrincipal);

            //Show success
            string sendSuccessMsg = WebTextManager.GetText("/pageText/PasswordReset.aspx/sendSuccess");

            _errorPanel.Visible = false;
            _sendSuccessPanel.Visible = true;

            _successLbl.Text = sendSuccessMsg.Contains("{0}") ? string.Format(sendSuccessMsg, userPrincipal.Email) : sendSuccessMsg;

            _enterEmailPlace.Visible = false;
            _okBtn.Visible = false;
        //    _cancelBtn.Visible = false;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="userPrincipal"></param>
        private void SendMessage(CheckboxPrincipal userPrincipal)
        {
            EmailMessage msg = new EmailMessage { Format = MailFormat.Text };

            string sitename = WebTextManager.GetText("/siteText/siteName");

            //Compose the email
            msg.From = ApplicationManager.AppSettings.SystemEmailAddress;
            msg.To = userPrincipal.Email;

            string subjectTxt = WebTextManager.GetText("/pageText/PasswordReset.aspx/msgSubject");
            string dearTxt = WebTextManager.GetText("/pageText/PasswordReset.aspx/msgDear");
            string userTxt = WebTextManager.GetText("/pageText/PasswordReset.aspx/userName");

            msg.Subject = subjectTxt.Contains("{0}")
                ? string.Format(subjectTxt, sitename)
                : subjectTxt;

            if (dearTxt.Contains("{0}"))
            {
                dearTxt = string.Format(dearTxt, userPrincipal.Email);
            }

            DateTime now = DateTime.Now;

            if (userTxt.Contains("{0}")
                && userTxt.Contains("{1}"))
            {
                userTxt = string.Format(userTxt, userPrincipal.Identity.Name, GetExpirationDateString(now));
            }

            //Put the message together
            msg.Body = dearTxt + Environment.NewLine + Environment.NewLine + userTxt + Environment.NewLine + Environment.NewLine + GetResetUrl(userPrincipal, now);

            //Send the message
            EmailGateway.Send(msg);
        }

        /// <summary>
        /// Get date/time link will expire
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        private static string GetExpirationDateString(DateTime now)
        {
            return now.Add(new TimeSpan(1, 0, 0, 0)).ToString("r");
        }

        /// <summary>
        /// Get the reset url
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <returns></returns>
        private string GetResetUrl(CheckboxPrincipal userPrincipal, DateTime now)
        {
            //Delete any existing tickets for the login
            Ticketing.DeleteTicket(userPrincipal.UserGuid);

            //Create a ticket
            Ticketing.CreateTicket(userPrincipal.UserGuid, now.Add(new TimeSpan(1, 0, 0, 0)));

            return ApplicationManager.ApplicationURL + ResolveUrl("~/ResetPassword.aspx") + "?u=" + userPrincipal.UserGuid.ToString().Replace("-", string.Empty);
        }
    }
}