using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Styles;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Preview survey page
    /// </summary>
    public partial class Preview : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Id of survey to preview
        /// </summary>
        [QueryParameter("s", IsRequired = true)]
        public int SurveyId { get; set; }

        /// <summary>
        /// Optional page of survey to start on
        /// </summary>
        [QueryParameter("p")]
        public int? PageId { get; set; }

        /// <summary>
        /// Survey render mode
        /// </summary>
        [QueryParameter("mode")]
        public RenderMode? RenderMode { get; set; }

        /// <summary>
        /// Print prameter
        /// </summary>
        [QueryParameter("print")]
        public string Print;

        /// <summary>
        /// 
        /// </summary>
        public ExportMode ExportMode
        {
            get
            {
                if (Print == null)
                    return ExportMode.None;

                switch (Print.ToLower())
                {
                    case "pdf":
                        return ExportMode.Pdf;
                    case "default":
                        return ExportMode.Default;
                    case "clientpdf":
                        return ExportMode.ClientPdf;
                    default:
                        return ExportMode.None;
                }
            }
        }

        /// <summary>
        /// Override for style template id
        /// </summary>
        [QueryParameter("st")]
        public int? StyleTemplateIdOverride { get; set; }

        ///<summary>
        /// Language of the survey to preview
        /// </summary>
        [QueryParameter("loc")]
        public string LocalizationCode { get; set; }

        ///<summary>
        /// Theme name for mobile preview
        /// </summary>
        [QueryParameter("theme")]
        public int? MobileTheme { get; set; }

        /// <summary>
        /// Survey to preview.
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool isMobileBrowser
        {
            get { return RenderMode.HasValue && RenderMode.Value == Checkbox.Forms.RenderMode.SurveyMobilePreview; }
        }

        /// <summary>
        /// Initialize page
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            int? styleTemplateId = StyleTemplateIdOverride ?? ResponseTemplate.StyleSettings.StyleTemplateId;

            if (isMobileBrowser)
            {
                int? mobileStyleId = ResponseTemplate.StyleSettings.MobileStyleId;
                MobileTheme = MobileTheme ?? mobileStyleId; 

                MobileStyle style = MobileTheme.HasValue ?
                    MobileStyleManager.GetStyle(MobileTheme.Value, TextManager.DefaultLanguage) :
                    MobileStyleManager.GetDefaultStyle(TextManager.DefaultLanguage);

                if (style != null)
                {
                    _mobileStyleInclude.Source = style.CssUrl;
                    if (!style.IsDefault)
                        styleTemplateId = null;
                }
                else
                    _mobileStyleInclude.Source = "~/Resources/mobile_themes/default/jquery.mobile-latest.css";                    
            }

            if (styleTemplateId.HasValue)
            {
                _surveyStylePlace.Controls.Add(new LiteralControl(
                    string.Format("<link rel=\"Stylesheet\" type=\"text/css\" media=\"screen, print\" href=\"{0}\" />",
                    ResolveUrl("~/ViewContent.aspx?st=" + styleTemplateId + "&mode=" + RenderMode))));
            }

            if (Request.Browser.Browser == "IE" && Request.Browser.MajorVersion <= 9)
                _jqueryInclude.Source = "~/Resources/jquery-1.10.2.min.js";
            else
                _jqueryInclude.Source = "~/Resources/jquery-latest.min.js";

            _previewControl.Initialize(ResponseTemplate, ExportMode, (LocalizationCode ?? "en-US"), styleTemplateId, RenderMode);
        }
    }
}
