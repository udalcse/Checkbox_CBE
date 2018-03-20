using System;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class EmbedMenu : SecuredPage
    {
        [QueryParameter("s")]
        public int? ResponseTemplateId { get; set; }

        [QueryParameter("r")]
        public int? AnalysisTemplateId { get; set; }

        private ResponseTemplate _responseTemplate;
        private AnalysisTemplate _analysisTemplate;

        private ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate != null)
                    return _responseTemplate;

                if (ResponseTemplateId.HasValue)
                    return (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId.Value));

                if (AnalysisTemplateId.HasValue)
                    return (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(AnalysisTemplate.ResponseTemplateID));

                return null;
            }
        }

        private AnalysisTemplate AnalysisTemplate
        {
            get
            {
                if (_analysisTemplate != null)
                    return _analysisTemplate;

                if (AnalysisTemplateId.HasValue)
                    return (_analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(AnalysisTemplateId.Value));

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsProtectedReport
        {
            get
            {
                return AnalysisTemplate != null && !AnalysisTemplate.DefaultPolicy.HasPermission("Analysis.Run");
            }
        }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (AnalysisTemplateId.HasValue)
            {
                _htmlCode.Text = EmbededHtmlCodeReportUrl;
                _standardUrl.Value = ReportURL;

                Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/reportTitle"));
            }
            else if (ResponseTemplateId.HasValue)
            {
                _htmlCode.Text = EmbededHtmlCodeStandardUrl;
                _customUrlhtmlCode.Text = EmbededHtmlCodeCustomUrl;

                _standardUrl.Value = SurveyURL;
                _customUrl.Value = SurveyCustomURL;

                Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/title"));
            }

            _htmlCode.ToolTip = WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/clickToSelectTooltip");
            _customUrlhtmlCode.ToolTip = WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/clickToSelectTooltip");
            _standardUrl.Attributes["title"] = WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/clickToSelectTooltip");
            _customUrl.Attributes["title"] = WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/clickToSelectTooltip");

            Master.OkVisible = false;
            Master.CancelTextId = "/common/close";
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DialogInstructionsText
        {
            get
            {
                if (ResponseTemplateId.HasValue)
                {
                    return WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/instructions");
                }

                if (AnalysisTemplateId.HasValue)
                {
                    return WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/reportInstructions");
                }
                
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string EmbededHtmlCodeStandardUrl
        {
            get { return string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\"></iframe>", SurveyURL); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string EmbededHtmlCodeReportUrl
        {
            get { return string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\"></iframe>", ReportURL); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string EmbededHtmlCodeCustomUrl
        {
            get { return string.Format("<iframe width=\"800\" height=\"600\"\n src=\"{0}\"></iframe>", SurveyCustomURL); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string SurveyURL
        {
            get
            {
                string surveyUrlWithGuid = ResolveUrl("~/Survey.aspx") + "?s=" + ResponseTemplate.GUID.ToString().Replace("-", String.Empty);
                return ApplicationManager.ApplicationURL + surveyUrlWithGuid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string SurveyCustomURL
        {
            get
            {
                return ApplicationManager.ApplicationURL + SurveyCustomURLRelative;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string ReportURL
        {
            get
            {
                string url = ResolveUrl("~/RunAnalysis.aspx") + "?ag=" + AnalysisTemplate.Guid.ToString().Replace("-", String.Empty);
                return ApplicationManager.ApplicationURL + url;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string SurveyCustomURLRelative
        {
            get
            {
                return UrlMapper.GetSource(ApplicationManager.ApplicationRoot + "/Survey.aspx?s=" + ResponseTemplate.GUID.ToString().Replace("-", string.Empty));
            }
        }
    }
}