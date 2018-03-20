using System;
using Checkbox.Globalization.Text;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class InvitationText : TextSettings
    {
        
        /// <summary>
        /// The name of configuration setting to enable or disable invitation text
        /// </summary>
        private const string InvitationTextEnabledSetting = "IsInvitationTextEnabled";

        protected void Page_Load(object sender, EventArgs e)
        {
            base.OnPageLoad();

            if (!Page.IsPostBack)
            {
                BindInputs();
            }

            //Selected index changed is not being tracked by .net for some reason, so do it manually
            if (Page.IsPostBack)
            {
                //Rebind inputs
                if (!_miscTextLanguageList.SelectedValue.Equals(ViewState["CurrentLanguage"] as string, StringComparison.InvariantCultureIgnoreCase))
                {
                    BindInputs();
                }
            }

            ViewState["CurrentLanguage"] = _miscTextLanguageList.SelectedValue;

            //Page.ClientScript.RegisterClientScriptInclude(
            //    "tiny_mce.js",
            //    ResolveUrl("~/Resources/tiny_mce/jquery.tinymce.min.js")
            //    );
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindInputs()
        {
            string htmlText = TextManager.GetText("/siteText/invitationHtmlDefaultText", _miscTextLanguageList.SelectedValue.Trim());
            string textText = TextManager.GetText("/siteText/invitationTextDefaultText", _miscTextLanguageList.SelectedValue.Trim());

            //_invitationHtmlMessage.Text = htmlText.Replace(InvitationManager.INITIALIZING_SURVEY_URL_PLACEHOLDER_DO_NOT_CHANGE, InvitationManager.SURVEY_URL_PLACEHOLDER);
            //_invitationTextMessage.Text = textText.Replace(InvitationManager.INITIALIZING_SURVEY_URL_PLACEHOLDER_DO_NOT_CHANGE, InvitationManager.SURVEY_URL_PLACEHOLDER);

            _invitationHtmlMessage.Text = htmlText;
            _invitationTextMessage.Text = textText;
        }

        protected override string PageTitleTextId { get { return "/pageText/settings/invitationText.aspx/title"; } }

        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            BindLanguageDropDown(_miscTextLanguageList, SurveyLanguages, DefaultLanguage);

            Master.OkClick += Master_OkClick;

            string errorMsg;

            //Check for multiLanguage support.            
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                _multiLanguageNotAllowedWarningPanel.Visible = true;
                _miscTextLanguageList.Enabled = false;
            }

            BindInputs();

            _htmlPipeSelector.Initialize(null, null, WebTextManager.GetUserLanguage(), _invitationHtmlMessage.ClientID);
            _textPipeSelector.Initialize(null, null, WebTextManager.GetUserLanguage(), _invitationTextMessage.ClientID);

            _invitationTextEnabled.Checked = ApplicationManager.AppSettings.GetValue<bool>(InvitationTextEnabledSetting, false);

        }

        protected override void OnInit(EventArgs e)
        {
            _htmlPipeSelector.ID = ID + "_" + _htmlPipeSelector.ID;
            _textPipeSelector.ID = ID + "_" + _textPipeSelector.ID;
            base.OnInit(e);
        }

        /// <summary>
        /// Save text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_miscTextLanguageList.SelectedValue))
            {
                var languageCode = _miscTextLanguageList.SelectedValue.Trim();

                TextManager.SetText("/siteText/invitationHtmlDefaultText", languageCode, _invitationHtmlMessage.Text);
                TextManager.SetText("/siteText/invitationTextDefaultText", languageCode, _invitationTextMessage.Text);

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/invitationText.aspx/textChangesSaved"),
                                         StatusMessageType.Success);

                ApplicationManager.AppSettings.SetValue(InvitationTextEnabledSetting, _invitationTextEnabled.Checked);
            }
        }
    }
}