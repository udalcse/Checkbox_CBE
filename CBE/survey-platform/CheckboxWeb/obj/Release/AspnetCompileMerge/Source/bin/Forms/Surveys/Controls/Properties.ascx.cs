using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Survey properties control
    /// </summary>
    public partial class Properties : Checkbox.Web.Common.UserControlBase
    {
        public bool IsDetailedForm { set; get; }

        /// <summary>
        /// Get/set survey name
        /// </summary>
        public string SurveyName
        {
            get
            {
                var surveyName = _surveyNameTxt.Text.Trim();

                //Strip HTML
                if (!ApplicationManager.AppSettings.AllowHTMLNames)
                {
                    surveyName = Utilities.StripHtml(surveyName, null);
                }

                return surveyName;
            }

            set { _surveyNameTxt.Text = value; }
        }

        /// <summary>
        /// Get/set whether survey is scored
        /// </summary>
        public bool EnableScoring
        {
            get { return _surveyOptionsList.Items.FindByValue("SCORED_SURVEY").Selected; }
            set { _surveyOptionsList.Items.FindByValue("SCORED_SURVEY").Selected = value; }
        }

        /// <summary>
        /// Get/set status of active survey checkbox
        /// </summary>
        public bool ActivateSurvey
        {
            get { return _surveyOptionsList.Items.FindByValue("ACTIVATE_SURVEY").Selected; }
            set { _surveyOptionsList.Items.FindByValue("ACTIVATE_SURVEY").Selected = value; }
        }

        public string CustomUrl
        {
            get { return _shortUrlTxt.Text.Trim(); }
            set
            {
                _shortUrlTxt.Text = Regex.Replace(value, string.Format("{0}/", ApplicationManager.ApplicationRoot), string.Empty, RegexOptions.IgnoreCase);
            }
        }

        public string CustomUrlExtension
        {
            get { return _extensionDrop.SelectedValue; }
            set
            {
                if (_extensionDrop.Items.FindByValue(value) != null)
                    _extensionDrop.SelectedValue = value;
            }
        }

        public bool ExtensionExists(string extension)
        {
            return _extensionDrop.Items.FindByValue(extension) != null;
        }

        /// <summary>
        /// A pass through to the survey name validator. The validator needs to know the survey's
        /// original name in order to exclude it when checking to see if the name is in use. 
        /// </summary>
        public string CurrentSurveyName { get; set; }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _surveyOptionsListContainer.Visible = IsDetailedForm;
            _dialogSubTitle.Visible = IsDetailedForm;
            _welcomeText.Visible = !IsDetailedForm;
            _helpText.Visible = !IsDetailedForm;

            //Show override message if app setting will override survey specific allow edit setting.
            _appSettingOverridePanel.Visible = !ApplicationManager.AppSettings.AllowEditActiveSurvey;

            //Populate redirect extensions
            foreach (string extension in ApplicationManager.AppSettings.AllowedUrlRewriteExtensions)
            {
                _extensionDrop.Items.Add(new ListItem("." + extension, extension));
            }

            //Allow URL rewriting and scored survey
            _urlRewritingPlace.Visible = ApplicationManager.AppSettings.AllowSurveyUrlRewriting && _extensionDrop.Items.Count != 0 && IsDetailedForm;
            _scoredChk.Visible = ApplicationManager.AppSettings.AllowScoredSurveys;

            //Validation 
            _nameRequiredValidator.Text = WebTextManager.GetText("/pageText/forms/surveys/create.aspx/nameRequired");
            _nameInUseValidator.Text = WebTextManager.GetText("/pageText/forms/surveys/create.aspx/nameInUse");

            _nameInUseValidator.ServerValidate += _nameInUseValidator_ServerValidate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        void _nameInUseValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string value = (args.Value ?? string.Empty).Trim();

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = Server.HtmlEncode(value);
            }

            if (value.Equals(CurrentSurveyName, StringComparison.InvariantCultureIgnoreCase))
            {
                args.IsValid = true;
                return;
            }
            
            args.IsValid = !ResponseTemplateManager.ResponseTemplateExists(value);
        }
        
        /// <summary>
        /// Ensure that the custom survey url is properly formatted and not in use by another survey or file
        /// </summary>
        /// <returns></returns>
        public bool CustomUrlIsValid()
        {
            _customUrlErrorPanel.Style["display"] = "none";

            if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting && Utilities.IsNotNullOrEmpty(CustomUrl))
            {
                var userShortURL = _shortUrlTxt.Text.Trim();

                if (userShortURL.Length > AlternateURLValidator.MaxLenght)
                {
                    _customUrlErrorPanel.Style.Remove("display");
                    _shortUrlErrorLbl.Text =
                        string.Format(WebTextManager.GetText("/pageText/surveyProperties.aspx/alternateURLTooLong"), AlternateURLValidator.MaxLenght);
                    return false;
                }

                var shortUrl = string.Format("{0}/{1}.{2}",
                                                ApplicationManager.ApplicationRoot,
                                                userShortURL,
                                                _extensionDrop.SelectedValue);

                //See if a survey url is already mapped to this short url
                var mappingExists = UrlMapper.SourceMappingExists(shortUrl);

                if (mappingExists)
                {
                    //Mapping is not for this survey
                    _customUrlErrorPanel.Style.Remove("display");
                    _shortUrlErrorLbl.Text = WebTextManager.GetText("/pageText/surveyProperties.aspx/thisCustomUrlIsUsed");
                    return false;
                }

                //Make sure the mapping does not point to an existing file
                if (File.Exists(Server.MapPath(shortUrl)))
                {
                    _customUrlErrorPanel.Style.Remove("display");
                    _shortUrlErrorLbl.Text =
                        WebTextManager.GetText("/pageText/surveyProperties.aspx/mappedFileExists");
                    return false;
                }

                //Finally, ensure that there are no special characterss
                //Make sure there are no special characters other than the underscore
                var validator = new AlternateURLValidator();

                if (!validator.Validate(_shortUrlTxt.Text))
                {
                    _customUrlErrorPanel.Style.Remove("display");
                    _shortUrlErrorLbl.Text = WebTextManager.GetText("/pageText/surveyProperties.aspx/invalidName");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveCustomUrl(string key)
        {
            var mappedUrl = string.Format("{0}/{1}.{2}",
                                             ApplicationManager.ApplicationRoot,
                                             CustomUrl,
                                             CustomUrlExtension);


            if (CustomUrl == string.Empty)
            {
                UrlMapper.RemoveMapping(key);
            }
            else
            {
                UrlMapper.AddMapping(mappedUrl, key);
            }
        }
    }
}