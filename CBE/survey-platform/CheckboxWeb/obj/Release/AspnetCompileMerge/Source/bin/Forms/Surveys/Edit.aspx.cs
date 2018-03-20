using System;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using System.Web.Services;
using System.Web.Script.Services;
using Checkbox.Users;
using Checkbox.Security;
using System.Collections.Generic;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Edit survey page
    /// </summary>
    public partial class Edit : ResponseTemplatePage
    {
        /// <summary>
        /// Get/set handler for showing status message.
        /// The first parameter must be a message.
        /// The second parameter must determine if an operation was succeeded or not.
        /// </summary>
        public string ShowStatusMessageHandler { get; set; }

		/// <summary>
		/// Determine if confirm message should be shown before deleting survey's page or items
		/// </summary>
		public bool ShowConfirmation
		{
			get { return ApplicationManager.AppSettings.ShowDeleteConfirmationPopups; }
		}

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// ID of item to edit
        /// </summary>
        [QueryParameter("item", DefaultValue = "0")]
        public int ItemID { get; set; }

        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        protected override ResponseTemplate ResponseTemplate
        {
            get
            {
                if (ResponseTemplateId <= 0 && ItemID > 0)
                {
                    ResponseTemplate r = ResponseTemplateManager.GetResponseTemplateByItemID(ItemID);
                    this.ResponseTemplateId = r.ID.Value;
                }

                return base.ResponseTemplate;
            }
        }

        /// <summary>
        /// Get language code
        /// </summary>
        protected string LanguageCode
        {
            get
            {
                var queryLanguage = Request.QueryString["l"];

                if (string.IsNullOrEmpty(queryLanguage)
                    || !ResponseTemplate.LanguageSettings.SupportedLanguages.Contains(queryLanguage))
                {
                    return !string.IsNullOrEmpty(ResponseTemplate.LanguageSettings.DefaultLanguage)
                               ? ResponseTemplate.LanguageSettings.DefaultLanguage
                               : TextManager.DefaultLanguage;
                }

                return queryLanguage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool ShowFacebook
        {
            get
            {
                return !string.IsNullOrEmpty(ApplicationManager.AppSettings.FacebookAppID);
            }
        }

        /// <summary>
        /// Initialize language list
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

			if (ResponseTemplate == null)
				return;
            
            _settings.ResponseTemplateId = ResponseTemplateId;

            var surveyLanguages = WebTextManager.GetSurveyLanguagesDictionary();

            foreach (var languageCode in ResponseTemplate.LanguageSettings.SupportedLanguages)
            {
                var localizedLanguageName = surveyLanguages.ContainsKey(languageCode)
                    ? surveyLanguages[languageCode]
                    : languageCode;

                _languageList.Items.Add(new ListItem(
                    localizedLanguageName,
                    languageCode));
            }

            if (_languageList.Items.FindByValue(ResponseTemplate.LanguageSettings.DefaultLanguage) != null)
            {
                _languageList.SelectedValue = ResponseTemplate.LanguageSettings.DefaultLanguage;
            }

            string errorMsg;

            //Check for multiLanguage Support
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                if (_languageList.Items.FindByValue(TextManager.DefaultLanguage) != null)
                {
                    _languageList.SelectedValue = TextManager.DefaultLanguage;
                }
                _languageList.Enabled = false;
            }

            _languageList.Visible = _languageList.Items.Count > 1;

          //  warningPanel.Visible = IsFormReadOnly;            

            //Set page title
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/edit.aspx/editSurvey") + " - " + Utilities.StripHtml(ResponseTemplate.Name, null));
			Master.ShowBackButton(ResolveUrl("~/Forms/Manage.aspx?s=" + ResponseTemplateId), true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected string TwitterUrl
        {
           
            get
            {
                
                string urlWithGuid = "/Survey.aspx?s=" + ResponseTemplate.GUID.ToString().Replace("-", string.Empty);
                string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetText"), ResponseTemplate.Name);
                return "https://twitter.com/intent/tweet?url=" +
                       Utilities.AdvancedHtmlEncode(ApplicationManager.ApplicationPath + urlWithGuid) +
                       "&text=" + Utilities.AdvancedHtmlEncode(text);
                
            }
        }

        protected string LinkedInUrl
        {
            get
            {
                string urlWithGuid = "/Survey.aspx?s=" +
                                     ResponseTemplate.GUID.ToString().Replace("-", string.Empty);
                return Utilities.AdvancedHtmlEncode("http://www.linkedin.com/shareArticle?mini=true&url=" + ApplicationManager.ApplicationPath + urlWithGuid +"&title=" +
                       ResponseTemplate.Name);
            }
        }

        protected string FacebookUrl
        {
            get
            {
                string urlWithGuid = "/Survey.aspx?s=" + ResponseTemplate.GUID;
                return Utilities.AdvancedHtmlEncode("http://www.facebook.com/sharer.php?s=100&p[url]=" + ApplicationManager.ApplicationPath + urlWithGuid);
               
            }
        }

        protected string GplusUrl
        {
            get
            {
                string urlWithGuid = "/Survey.aspx?s=" + ResponseTemplate.GUID;
                return Utilities.AdvancedHtmlEncode("https://plus.google.com/share?url=" + ApplicationManager.ApplicationPath + urlWithGuid);
            }
        }

        /// <summary>
        /// Initialization and ensure necessary scripts loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			string surveyExportId = Request.Params["doexport"];

			if (!string.IsNullOrEmpty(surveyExportId))
			{
				var id = int.Parse(surveyExportId);
				var template = ResponseTemplateManager.GetResponseTemplate(id);

				GetDownloadResponse(FileUtilities.FixFileName(template.Name, string.Empty, "_") + "_export.xml");
               
			    var writer = new XmlTextWriter(Response.Output) {Formatting = Formatting.Indented};

			    template.Export(writer);

				Response.Flush();
				Response.End();

				return;
			}

            string itemExportId = Request.Params["doitemexport"];

            if (!string.IsNullOrEmpty(itemExportId))
            {
                int id = int.Parse(itemExportId);
                ItemData item = ItemConfigurationManager.GetConfigurationData(id);

                string name = "Item_" + item.ID.Value.ToString();
               

                GetDownloadResponse(FileUtilities.FixFileName(name, string.Empty, "_") + "_export.xml");

                var writer = new XmlTextWriter(Response.Output) { Formatting = Formatting.Indented };

                item.Export(writer);

                Response.Flush();
                Response.End();

                return;
            }

			if (ResponseTemplate == null)
				return;


            //Survey management JS service
            RegisterClientScriptInclude(
                "svcSurveyManagement.js",
                ResolveUrl("~/Services/js/svcSurveyManagement.js"));

            //Survey editor JS service
            RegisterClientScriptInclude(
                "svcSurveyEditor.js",
                ResolveUrl("~/Services/js/svcSurveyEditor.js"));

            //Service helper required by JS service
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            //Helper for loading jQuery templates
            RegisterClientScriptInclude(
                "templateHelper.js",
                ResolveUrl("~/Resources/templateHelper.js"));

            //Helper for editing survey/report/library templates
            RegisterClientScriptInclude(
                "templateEditor.js",
                ResolveUrl("~/Resources/templateEditor.js"));

            //Helper for editing survey/report/library templates
            RegisterClientScriptInclude(
                "surveyEditor.js",
                ResolveUrl("~/Resources/surveyEditor.js"));

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            //Sorting
            RegisterClientScriptInclude(
             "jquery.tinysort.min.js",
             ResolveUrl("~/Resources/jquery.tinysort.min.js"));

            //Hover intent
            RegisterClientScriptInclude(
             "jquery.hoverIntent.js",
             ResolveUrl("~/Resources/jquery.hoverIntent.min.js"));

            RegisterClientScriptInclude(
             "svcAuthorization.js",
             ResolveUrl("~/Services/js/svcAuthorization.js"));

            RegisterClientScriptInclude(
             "jquery.ckbxEditable.js",
             ResolveUrl("~/Resources/jquery.ckbxEditable.js"));

            RegisterClientScriptInclude(
             "dateUtils.js",
             ResolveUrl("~/Resources/dateUtils.js"));

            //Moment.js: datetime utilities
            RegisterClientScriptInclude(
                "moment.js",
                ResolveUrl("~/Resources/moment.js"));

            RegisterClientScriptInclude(
                "securityHelper.js",
                ResolveUrl("~/Resources/securityHelper.js"));

            RegisterClientScriptInclude(
              "jquery.ckbxprotect.js",
              ResolveUrl("~/Resources/jquery.ckbxprotect.js"));

            LoadDatePickerLocalized();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		protected void GetDownloadResponse(string fileName)
		{
			Response.Expires = -1;
			Response.BufferOutput = ApplicationManager.AppSettings.BufferResponseExport;
			Response.Clear();
			Response.ClearHeaders();
			Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
			Response.ContentType = "application/octet-stream";
		}

        /// <summary>
        /// 
        /// </summary>
        protected bool SheduledInvitations
        {
            get { return EmailGateway.ProviderSupportsBatches; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool ShowInvitations
        {
            get { return ApplicationManager.AppSettings.EmailEnabled; }
        }

        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Form.Edit"; }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static RadioButtonField GetBoundRadioField(string fieldName, int? itemId)
        {
            var radioButton =  ProfileManager.GetRadioButtonField(fieldName, UserManager.GetCurrentPrincipal().UserGuid);
            var optionAliases = ProfileManager.GetRadioOptionAliases(itemId.Value, fieldName);
            for(var i = 0; i < radioButton.Options.Count; i++)
            {
                radioButton.Options[i].Alias = string.Empty;
                if (optionAliases.ContainsKey(radioButton.Options[i].Name))
                {
                    radioButton.Options[i].Alias = optionAliases[radioButton.Options[i].Name];
                }
            }
            return radioButton;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void DeleteRadioButtonFieldOptionAliases(int itemId)
        {
            ProfileManager.DeleteRadioButtonFieldOptionAliases(itemId);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void AddRadioButtonFieldOptionAliases(string fieldName, int itemId, Dictionary<string, string> optionAliases)
        {
            if(optionAliases.Count > 0)
            {
                ProfileManager.DeleteRadioButtonFieldOptionAliases(itemId);
            }

            var radioButtonField = ProfileManager.GetRadioButtonField(fieldName, UserManager.GetCurrentPrincipal().UserGuid);
            if (radioButtonField != null)
            {
                foreach(var option in optionAliases)
                {
                    int index = radioButtonField.Options.FindIndex(o => o.Name == option.Key);
                    radioButtonField.Options[index].Alias = option.Value;
                }
                ProfileManager.AddRadioButtonFieldOptionAlias(itemId, radioButtonField.Options);
            }
        }
    }
}