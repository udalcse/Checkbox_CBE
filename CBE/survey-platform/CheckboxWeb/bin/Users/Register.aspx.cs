using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using System.Text.RegularExpressions;
using System.Web;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// Self-registration page
    /// </summary>
    public partial class Register : ApplicationPage
    {
        private int? _surveyId;

        /// <summary>
        /// Guid of survey to access
        /// </summary>
        [QueryParameter("s")]
        public string SurveyGuid { get; set; }

        /// <summary>
        /// Guid of survey invitation
        /// </summary>
        [QueryParameter("i")]
        public string InvitationGuid { get; set; }

        /// <summary>
        /// Language code
        /// </summary>
        [QueryParameter("l")]
        public string LanguageCodeParameter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode
        {
            get
            {
                return string.IsNullOrEmpty(LanguageCodeParameter) ? TextManager.DefaultLanguage : LanguageCodeParameter;
            }
        }

        /// <summary>
        /// Overridden return url.  When specified, used in place
        /// of return to survey url or login page.
        /// </summary>
        [QueryParameter("returnUrl")]
        public string ReturnUrl { get; set; }

        private Dictionary<string, TextBox> _customFieldInputs;

        private Dictionary<string, TextBox> CustomFieldInputs
        {
            get { return _customFieldInputs ?? (_customFieldInputs = new Dictionary<string, TextBox>()); }
        }

        /// <summary>
        /// Survey id
        /// </summary>
        private int? GetSurveyId()
        {
            if (_surveyId.HasValue)
            {
                return _surveyId.Value;
            }

            if (Utilities.IsNotNullOrEmpty(SurveyGuid))
            {
                try
                {
                    _surveyId = ResponseTemplateManager.GetResponseTemplateIdFromGuid(new Guid(SurveyGuid));
                }
                catch
                {
                }
            }

            if (Utilities.IsNotNullOrEmpty(InvitationGuid))
            {
                try
                {
                    Guid? surveyGuid = InvitationManager.GetResponseTemplateGuidForInvitation(new Guid(InvitationGuid));

                    if (surveyGuid.HasValue)
                    {
                        _surveyId = ResponseTemplateManager.GetResponseTemplateIdFromGuid(new Guid(SurveyGuid));
                    }
                }
                catch
                {
                }
            }

            return _surveyId;   
        }

        private void RegisterText(string languageCode)
        {
            string instructionsText = TextManager.GetText("/selfRegistrationScreen/registerInstructions", languageCode);
            _registerInstructionsLbl.Text = instructionsText.Replace("[ASTERISK_PLACE]", "<span class=\"registerRequired\">*</span>");

            _confirmPasswordRequiredError.Text = TextManager.GetText("/selfRegistrationScreen/confirmPasswordRequired", languageCode);
            _emailErrorLbl.Text = TextManager.GetText("/selfRegistrationScreen/invalidEmail", languageCode);

            _cancelBtn.Text = TextManager.GetText("/selfRegistrationScreen/cancelButton", languageCode);
            _cancelBtn.ToolTip = TextManager.GetText("/selfRegistrationScreen/cancelTooltip", languageCode);
            _okBtn.Text = TextManager.GetText("/selfRegistrationScreen/registerButton", languageCode);
            _okBtn.ToolTip = TextManager.GetText("/selfRegistrationScreen/registerTooltip", languageCode);

            _returnBtn.ToolTip = TextManager.GetText("/selfRegistrationScreen/goToLoginPage", languageCode);
        }

        /// <summary>
        /// Redirect to login page when self-registration not enabled
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Do nothing if self-registration not enabled
            if (!ApplicationManager.AppSettings.AllowPublicRegistration)
            {
                Response.Redirect(ResolveUrl("~/Login.aspx"), false);
                return;
            }

            //text
            RegisterText(LanguageCode);

            //Bind event handlers
            _customFieldsRepeater.ItemCreated += _customFieldsRepeater_ItemCreated;
            _okBtn.Click += _okBtn_Click;
            _cancelBtn.Click += _cancelBtn_Click;
            _returnBtn.Click += _returnBtn_Click;

            //Load custom user field information
            LoadCustomFields();

            //Show/hide required flag for emails
            _emailRequiredLbl.Visible = ApplicationManager.AppSettings.RequireEmailAddressOnRegistration;

            //Hide error panel by default
            _errorPanel.Visible = false;

            //Hide success panel by default too
            _userCreatedPlace.Visible = false;
            Master.HideFooter();

            //Show/hide chrome
            if (!GetSurveyId().HasValue) return;
            Master.HideHeader();
        }

        /// <summary>
        /// Load style settings if redirected here from a survey.
        /// </summary>
        private void LoadStyle()
        {
            int? surveyId = GetSurveyId();

            if (!surveyId.HasValue)
            {
                return;
            }

            //Attempt to load survey & style template
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId.Value);

            //Do nothing if survey not found and style template not specified
            if (lightweightRt == null
                || !lightweightRt.StyleTemplateId.HasValue)
            {
                return;
            }

            var st = StyleTemplateManager.GetStyleTemplate(lightweightRt.StyleTemplateId.Value);

            if (st == null)
            {
                return;
            }

            _styleCssPlace.Controls.Add(new LiteralControl(st.GetCss()));

            _styleHeaderPlace.Controls.Add(new LiteralControl(TextManager.GetText(st.HeaderTextID, lightweightRt.DefaultLanguage ?? TextManager.DefaultLanguage)));
            _styleFooterPlace.Controls.Add(new LiteralControl(TextManager.GetText(st.FooterTextID, lightweightRt.DefaultLanguage ?? TextManager.DefaultLanguage)));
        }

        /// <summary>
        /// Populate custom fields
        /// </summary>
        private void LoadCustomFields()
        {
            CustomFieldInputs.Clear();
            _customFieldsRepeater.DataSource =
                ProfileManager.ListPropertyNames().Where(prop => 
                    !ProfileManager.IsFieldHidden(prop) 
                    && !string.Equals("Email", prop, StringComparison.InvariantCultureIgnoreCase));
            _customFieldsRepeater.DataBind();
        }

        /// <summary>
        /// Handle item created to store reference to text input for field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _customFieldsRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            var fieldName = e.Item.DataItem as string;
            var inputTxt = e.Item.FindControl("_valueTxt") as TextBox;

            if (Utilities.IsNullOrEmpty(fieldName)
                || inputTxt == null)
            {
                return;
            }

            //Store value
            CustomFieldInputs[fieldName] = inputTxt;
        }

        /// <summary>
        /// Return to the page from whence we came.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cancelBtn_Click(object sender, EventArgs e)
        {
            RedirectToReturnUrl();
        }

        /// <summary>
        /// Return to login page after successful registration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _returnBtn_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Attempt to create the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //Validate inputs
                if (!ValidateInputs())
                {
                    return;
                }

                //Otherwise, create the user & redirect
                if (CreateUser())
                {
                    FormsAuthentication.SetAuthCookie(_userName.Text, false);
                    HttpContext.Current.User = UserManager.GetUserPrincipal(_userName.Text);

                    RedirectToReturnUrl();
                }
            }
            catch (Exception ex)
            {
                //Handle & log exception
                ExceptionPolicy.HandleException(ex, "UIProcess");

                //Show message
                _topErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/errorOccurred", LanguageCode) + "<br />" + ex.Message;
                _errorPanel.Visible = true;
            }
        }

        /// <summary>
        /// Create user based on current inputs
        /// </summary>
        private bool CreateUser()
        {
            string status;

            var newUser = UserManager.CreateUser(
                _userName.Text.Trim(),
                _password.Text.Trim(),
                null,
                ApplicationManager.AppSettings.RequireEmailAddressOnRegistration ? _email.Text.Trim() : string.Empty,
                _userName.Text.Trim(), 
                out status);

            if (newUser == null)
            {
                _topErrorLbl.Text = status;
                _errorPanel.Visible = true;
                return false;
            }

            //Apply default roles to user
            RoleManager.AddUserToRoles(newUser.Identity.Name, ApplicationManager.AppSettings.DefaultUserRoles);

            //Update the user's profile
            var userProfile = new Dictionary<string, string>();

            foreach (string key in CustomFieldInputs.Keys)
            {
                userProfile[key] = CustomFieldInputs[key].Text.Trim();
            }

            ProfileManager.StoreProfile(newUser.Identity.Name, userProfile);

            return true;
        }

        /// <summary>
        /// Validate user inputs
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            //Attempt to create the user, but first validate fields.
            var validationErrors = new List<string>();

            //1. Validate user name
            //  a. Name 
            if (Utilities.IsNullOrEmpty(_userName.Text.Trim()))
            {
                _userNameErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/userNameRequired");
                _userNameErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationUserNameRequired"));
            }
            //  b. Name in use
            else if (UserManager.UserExists(_userName.Text.Trim()))
            {
                _userNameErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/userNameInUse");
                _userNameErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationUserNameInUse"));
            }
            //  c. User name format
            else if (!UserManager.ValidateUniqueIdentifierFormat(_userName.Text.Trim()))
            {
                _userNameErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/userNameInvalid");
                _userNameErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationUserNameInvalid"));
            }

            //2. Validate password
            if (Utilities.IsNullOrEmpty(_password.Text.Trim()))
            {
                _passwordErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/passwordRequired");
                _passwordErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationPasswordRequired"));
            }
            else if (Utilities.IsNullOrEmpty(_confirmPassword.Text.Trim()))
            {
                _passwordErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/confirmPasswordRequired");
                _passwordErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationConfirmPasswordRequired"));

            }
            else if (_confirmPassword.Text.Trim() != _password.Text.Trim())
            {
                _passwordErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/passwordMisMatch");
                _passwordErrorLbl.Visible = true;
                validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationConfirmPasswordFailed"));
            }
            else if (ApplicationManager.AppSettings.MinPasswordLength > 0 && _password.Text.Trim().Length < ApplicationManager.AppSettings.MinPasswordLength)
            {
                _passwordErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/passwordTooShort");
                _passwordErrorLbl.Visible = true;
                validationErrors.Add(string.Format(WebTextManager.GetText("/validationMessages/regex/password/minLength"), ApplicationManager.AppSettings.MinPasswordLength));
            }
            else if (ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric > 0)
            {
                const string raw = @"[\d\w\s]";
                raw.Normalize();
                var passwordRegex = new Regex(raw, RegexOptions.None);
                var matchedExpressions = passwordRegex.Matches(_password.Text.Trim());

                if ((_password.Text.Trim().Length - matchedExpressions.Count) < ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric)
                {
                    _passwordErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/nonAlphanumericCharsNeeded");
                    _passwordErrorLbl.Visible = true;
                    validationErrors.Add(string.Format(WebTextManager.GetText("/validationMessages/regex/password/minNonAlphaNumericCount"), ApplicationManager.AppSettings.MinPasswordNonAlphaNumeric));
                }
            }

            //3. Email Address
            if (ApplicationManager.AppSettings.RequireEmailAddressOnRegistration)
            {
                if (string.IsNullOrEmpty(_email.Text))
                {
                    _emailErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/emailRequired");
                    _emailErrorLbl.Visible = true;
                    validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationEmailRequired"));
                }
                else 
                {
                    var emailValidator = new EmailValidator();

                    if (!emailValidator.Validate(_email.Text.Trim()))
                    {
                        _emailErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/invalidEmail");
                        _emailErrorLbl.Visible = true;
                        validationErrors.Add(WebTextManager.GetText("/selfRegistrationScreen/topValidationEmailInvalid"));
                    }
                }
            }

            //Show errors
            if (validationErrors.Count == 0)
            {
                _errorPanel.Visible = false;
                return true;
            }

            _errorPanel.Visible = true;
            _topErrorLbl.Text = WebTextManager.GetText("/selfRegistrationScreen/inputValidationError");
            _errorRepeater.DataSource = validationErrors;
            _errorRepeater.DataBind();

            return false;
        }

        /// <summary>
        /// Redirect back to login page.  This will either be a survey url or 
        /// main login page.
        /// </summary>
        private void RedirectToReturnUrl()
        {
            if(!string.IsNullOrWhiteSpace(ReturnUrl))
            {
                Response.Redirect(ReturnUrl, false);
                return;
            }

            if (GetSurveyId().HasValue)
            {
                Response.Redirect(ResolveUrl("~/Survey.aspx") + Request.Params["q"], false);
                return;
            }
            
            Response.Redirect(UserDefaultRedirectUrl, false);
        }
    }
}
