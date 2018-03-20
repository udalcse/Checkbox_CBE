using System;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Users;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// Edit survey page
    /// </summary>
    public partial class Edit : ResponseTemplatePage
    {
        private AnalysisTemplate _reportTemplate;

        /// <summary>
        /// Require analysis administer permission to create a report
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Edit"; }
        }
        /// <summary>
        /// Require analysis administer permission to create a report
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Analysis.Edit"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Prezza.Framework.Security.IAccessControllable GetControllableEntity()
        {
            return ReportTemplate;
        }
        
        /// <summary>
        /// Get/set survey id
        /// </summary>
        [QueryParameter("r", IsRequired = true)]
        public int? ReportId { get; set; }

        /// <summary>
        /// Get survey
        /// </summary>
        protected AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_reportTemplate == null
                    && ReportId.HasValue)
                {
                    _reportTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);
                }

                return _reportTemplate;
            }
        }

        /// <summary>
        /// Determine if confirm message should be shown before deleting survey's page or items
        /// </summary>
        public string IsConfirmationNeeded
        {
            get { return ApplicationManager.AppSettings.ShowDeleteConfirmationPopups.ToString().ToLower(); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return WebTextManager.GetText("/pageText/forms/surveys/reports/edit.aspx/title"); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool ShowFacebook
        {
            get { return !string.IsNullOrEmpty(ApplicationManager.AppSettings.FacebookAppID); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string TwitterUrl
        {
            get
            {
                string urlWithGuid = "/RunAnalysis.aspx?ag=" + _reportTemplate.Guid.ToString().Replace("-", string.Empty);
                string text = string.Format("{0} {1}", WebTextManager.GetText("/twitterButton/tweetReportText"), _reportTemplate.Name);

                return "https://twitter.com/intent/tweet?url=" + Utilities.AdvancedHtmlEncode(ApplicationManager.ApplicationPath + urlWithGuid) +
                                  "&text=" + Utilities.AdvancedHtmlEncode(text); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string RunReportUrl => HttpContext.Current.Request.Url.Scheme
                                         + "://"
                                         + HttpContext.Current.Request.Url.Authority
                                         + "/RunAnalysis.aspx" + "?ag=" + _reportTemplate.Guid.ToString().Replace(" - ", string.Empty);

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
        protected bool IsProtectedReport
        {
            get
            {
                return _reportTemplate != null && !_reportTemplate.DefaultPolicy.HasPermission("Analysis.Run");
            }
        }

        /// <summary>
        /// Initialize language list
        /// </summary>
        protected override void OnPageInit()
        {
            //Set the response template id so that the navigation link can be created.
            //This neds to be set before base.OnPageInit() is called.
            ResponseTemplateId = ReportTemplate.ResponseTemplateID;

            base.OnPageInit();

            var surveyLanguages = WebTextManager.GetSurveyLanguagesDictionary();

            foreach (string languageCode in ResponseTemplate.LanguageSettings.SupportedLanguages)
            {
                var localizedLanguageName = surveyLanguages.ContainsKey(languageCode)
                    ? surveyLanguages[languageCode]
                    : languageCode;

                _languageList.Items.Add(new ListItem(
                    localizedLanguageName,
                    languageCode));
            }

            if (_languageList.Items.FindByValue(LanguageCode) != null)
            {
                _languageList.SelectedValue = LanguageCode;
            }

            _languageList.Visible = _languageList.Items.Count > 1;

            //Hiding the language list until ML functionality can be added to reports 
            _languageList.Visible = false;

            //Set page title
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/reports/edit.aspx/title") + " - " + Utilities.StripHtml(ReportTemplate.Name, null));

            string returnUrl = "";
            CheckboxPrincipal principal = UserManager.GetCurrentPrincipal();
            
            returnUrl = ResolveUrl("~/Forms/Surveys/Reports/Manage.aspx?s=" + ResponseTemplateId);

            Master.ShowBackButton(returnUrl, true);
        }

        /// <summary>
        /// Initialization and ensure necessary scripts loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Report management JS service
            RegisterClientScriptInclude(
                "svcReportManagement.js",
                ResolveUrl("~/Services/js/svcReportManagement.js"));

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
                "reportEditor.js",
                ResolveUrl("~/Resources/reportEditor.js"));

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
                GetType(),
                "StatusControl.js",
                ResolveUrl("~/Resources/statusControl.js"));
        }
    }
}