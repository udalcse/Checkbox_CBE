using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class FacebookMenu : SecuredPage
    {
        [QueryParameter("s")]
        public int? ResponseTemplateId { get; set; }

        [QueryParameter("r")]
        public int? AnalysisTemplateId { get; set; }

        private ResponseTemplate _responseTemplate = null;
        private AnalysisTemplate _analysisTemplate = null;

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

            _facebookButton.HideButton = true;
            _facebookButton.ResponseTemplateTitleTextID = ResponseTemplate.TitleTextID;
            
            Master.CancelTextId = "/common/close";
            Master.SetTitle(string.Format(WebTextManager.GetText("/pageText/forms/surveys/facebookMenu.aspx/title")));
            Master.CancelVisible = true;

            string url = string.Empty;
            if (ResponseTemplateId.HasValue)
            {
                url = ApplicationManager.ApplicationURL +
                      ResolveUrl("~/Survey.aspx") + "?s=" + 
                      ResponseTemplate.GUID.ToString().Replace("-", string.Empty);

                _facebookButton.ResponseTemplateName = ResponseTemplate.Name;
                _facebookButton.InitSurveyText();
            }

            if (AnalysisTemplateId.HasValue)
            {
                url = ApplicationManager.ApplicationURL +
                      ResolveUrl("~/RunAnalysis.aspx") + "?ag=" +
                      AnalysisTemplate.Guid.ToString().Replace("-", string.Empty);

                _facebookButton.ResponseTemplateName = AnalysisTemplate.Name;
                _facebookButton.InitReportText();
            }

            Master.OkBtn.Attributes.Add("onclick", "facebookShare('" + Utilities.AdvancedHtmlEncode(url) + "');");
            
            Master.OkBtn.CssClass += " statistics_FacebookShare";
            Master.OkTextId = "/facebookButton/shareViaFacebook";
            _facebookButton.HideShareButton = true;
        }
    }
}