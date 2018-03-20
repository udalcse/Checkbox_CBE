using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;
using System.Collections.Generic;
using Checkbox.Globalization.Text;
using Checkbox.Web.Page;
using Checkbox.Management.Licensing.Limits;
using Page = System.Web.UI.Page;
using Checkbox.LicenseLibrary;
using System.Web;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Delegate for event fired when language list is changed
    /// </summary>
    public delegate void LanguageListChangedDelegate();

    /// <summary>
    /// User control for configuring survey language settings.
    /// </summary>
    public partial class LanguageSettings : Checkbox.Web.Common.UserControlBase
    {
        private SurveyLanguageSettings Settings { get; set; }

        private int SurveyId { get; set; }

        /// <summary>
        /// Event that fired when language list is changed.
        /// </summary>
        public event LanguageListChangedDelegate OnLanguageListChanged;

        /// <summary>
        /// Determine if LanguageList is changed or not.
        /// </summary>
        public bool IsLanguageListChanged
        {
            get
            {
                if (Session[SurveyId + "_IsLanguageListChanged"] == null)
                    Session[SurveyId + "_IsLanguageListChanged"] = false;
                return (bool)Session[SurveyId + "_IsLanguageListChanged"];
            }
            private set { Session[SurveyId + "_IsLanguageListChanged"] = value; }
        }

        /// <summary>
        /// Bind event handlers
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _languageSelectList.SelectedIndexChanged += LanguageSelectList_SelectedIndexChanged;
            _addLanguageBtn.Click += AddLanguageBtn_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                IsLanguageListChanged = false;
                BindAvailableLanguageList();
                BindDefaultLanguageList();
            }
        }
       
        /// <summary>
        /// Handle changing of language source option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLanguageVariableVisibility();
        }

        private void AddLanguageBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_availableLanguages.SelectedValue))
            {
                ResponseTemplate template = ResponseTemplateManager.GetResponseTemplate(SurveyId);
                template.AddSupportedLanguage(_availableLanguages.SelectedValue);
                template.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                template.Save();
                IsLanguageListChanged = true;

                ResponseTemplateManager.MarkTemplateUpdated(template.ID.Value);

                _languagesRepeater.DataSource = template.LanguageSettings.SupportedLanguages;
                _languagesRepeater.DataBind();
            }

            BindAvailableLanguageList();
            BindDefaultLanguageList();

            if (OnLanguageListChanged != null)
                OnLanguageListChanged();
        }

        /// <summary>
        /// Initialize control based on specified language settings
        /// </summary>
        /// <param name="ls"></param>
        public void Initialize(SurveyLanguageSettings ls)
        {
            SurveyId = ls.SurveyId;
            Settings = ls;

            //Set language source
            if(_languageSelectList.Items.FindByValue(ls.LanguageSource) != null)
            {
                _languageSelectList.SelectedValue = ls.LanguageSource;
            }

            //Set visibility of language token
            _variableNameTxt.Text = ls.LanguageSourceToken;
            SetLanguageVariableVisibility();

            if (!Page.IsPostBack)
            {
                //Bind available language repeater
                _languagesRepeater.DataSource = ls.SupportedLanguages;
                _languagesRepeater.DataBind();
            }
        }

        /// <summary>
        /// Bind list of default languages
        /// </summary>
        private void BindDefaultLanguageList()
        {
            if (Settings == null)
                return;
            _defaultLanguage.Items.Clear();

            foreach (string language in Settings.SupportedLanguages)
            {
                _defaultLanguage.Items.Add(new ListItem(
                    WebTextManager.GetText("/languageText/" + language, null, language),
                    language));
            }

            //Set default language
            if (Utilities.IsNotNullOrEmpty(Settings.DefaultLanguage)
                && _defaultLanguage.Items.FindByValue(Settings.DefaultLanguage) != null)
            {
                _defaultLanguage.SelectedValue = Settings.DefaultLanguage;
            }

            String errorMsg;
            //Check for multiLanguage support
            if ((Page is LicenseProtectedPage) && (Page as LicenseProtectedPage).ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                if (Utilities.IsNotNullOrEmpty(TextManager.DefaultLanguage)
                    && _defaultLanguage.Items.FindByValue(TextManager.DefaultLanguage) != null)
                {
                    _defaultLanguage.SelectedValue = TextManager.DefaultLanguage;
                }
                _defaultLanguage.Enabled = false;
            }
        }

        /// <summary>
        /// Bind list of availabe survey languages
        /// </summary>
        /// <param name="ls"></param>
        private void BindAvailableLanguageList()
        {
            if (Settings == null)
                return;
            _availableLanguages.Items.Clear();

            foreach (string textCode in TextManager.SurveyLanguages)
            {
                if (Settings != null && !Settings.SupportedLanguages.Contains(textCode))
                {
                    string text = WebTextManager.GetText("/languageText/" + textCode) + " [" + textCode + "]";
                    _availableLanguages.Items.Add(new ListItem(text, textCode));
                }
            }

            if (_availableLanguages.Items.Count == 0)
            {
                availablePanel.Visible = false;
                noneAvailablePanel.Visible = true;
            }
            else
            {
                availablePanel.Visible = true;
                noneAvailablePanel.Visible = false;
            }
        }

        /// <summary>
        /// Set visibility of language variable based on selected value.
        /// </summary>
        private void SetLanguageVariableVisibility()
        {
            _languageSelectOptionPlace.Visible = !"Prompt".Equals(_languageSelectList.SelectedValue, StringComparison.InvariantCultureIgnoreCase) &&
                !"Browser".Equals(_languageSelectList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            _browserDetectionUnsuccessOptionPlace.Visible = "Browser".Equals(_languageSelectList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            _optionLbl.Text = "User".Equals(_languageSelectList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                ? WebTextManager.GetText("/pageText/surveyLanguage.aspx/userAttribute")
                : WebTextManager.GetText("/pageText/surveyLanguage.aspx/variableName");
        }

        /// <summary>
        /// Update language settings with specified user options
        /// </summary>
        /// <param name="ls"></param>
        public void Update(SurveyLanguageSettings ls)
        {
            ls.DefaultLanguage = _defaultLanguage.SelectedValue;
            ls.LanguageSource = _languageSelectList.SelectedValue;
            ls.LanguageSourceToken = "Browser".Equals(_languageSelectList.SelectedValue, StringComparison.InvariantCultureIgnoreCase) ?
                _browserDetectionUnsuccessOption.SelectedValue : _variableNameTxt.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LanguagesRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            ResponseTemplate template = ResponseTemplateManager.GetResponseTemplate(SurveyId);

            switch (e.CommandName)
            {
                case "Delete":
                    template.RemoveSupportedLanguage(e.CommandArgument.ToString());
                    template.Save();
                    IsLanguageListChanged = true;

                    ResponseTemplateManager.MarkTemplateUpdated(template.ID.Value);

                    Settings.SupportedLanguages = template.LanguageSettings.SupportedLanguages;
                    
                    if (OnLanguageListChanged != null)
                        OnLanguageListChanged();
                    break;
            }

            BindAvailableLanguageList();
            BindDefaultLanguageList();

            _languagesRepeater.DataSource = Settings.SupportedLanguages;
            _languagesRepeater.DataBind();
        }
    }
}