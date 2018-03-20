using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Prezza.Framework.Security;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base page for application pages that work with response templates.
    /// </summary>
    public abstract class ResponseTemplatePage : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Require form edit permission to view this page
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        /// <summary>
        /// Get controllable entity for security checks
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// Get whether editing active surveys is allowed
        /// </summary>
        protected bool AllowActiveEdit
        {
            get { return ApplicationManager.AppSettings.AllowEditActiveSurvey && ResponseTemplate.BehaviorSettings.AllowSurveyEditWhileActive; }
        }

        /// <summary>
        /// Get whether the page is read-only or not
        /// </summary>
        protected virtual bool IsFormReadOnly
        {            
            get { return ResponseTemplate.BehaviorSettings.IsActive && !AllowActiveEdit; }
        }

        /// <summary>
        /// ID of response template
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// ID of response template
        /// </summary>
        [QueryParameter("period")]
        public int TimelinePeriod { get; set; }

        /// <summary>
        /// True when we are in the dialog
        /// </summary>
        [QueryParameter("dialog")]
        public bool IsDialog { get; set; }
        
        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        protected virtual ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null && ResponseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Get specific title for page that will be appended to template name to 
        /// create page title.  
        /// </summary>
        protected abstract string PageSpecificTitle { get; }

        /// <summary>
        /// Override on page init to set page title and other values.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if(Master != null
                && Master is BaseMasterPage)
            {
                ((BaseMasterPage)Master).SetTitleControl(CreateTitleControl());
            }
        }

        /// <summary>
        /// Create the title control for the page
        /// </summary>
        /// <returns></returns>
        protected virtual Control CreateTitleControl()
        {
            PlaceHolder titlePlace = new PlaceHolder();

            if (ResponseTemplate == null)
            {
                return titlePlace;
            }

            titlePlace.Controls.Add(new HyperLink
            {
                NavigateUrl = GetSurveyHomeUrl(),
                Text = Utilities.StripHtml(ResponseTemplate.Name, null)
            });

            if (Utilities.IsNotNullOrEmpty(PageSpecificTitle))
            {
                titlePlace.Controls.Add(new LiteralControl(" - "));
                titlePlace.Controls.Add(new LiteralControl(PageSpecificTitle));
            }

            return titlePlace;
        }

        /// <summary>
        /// Redirect to survey home page.
        /// </summary>
        protected void RedirectToSurveyHome()
        {
            Response.Redirect(GetSurveyHomeUrl(), false);
        }

        /// <summary>
        /// Get url to survey home page.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSurveyHomeUrl()
        {
            return ResolveUrl("~/Forms/Surveys/Survey.aspx") + "?s=" + ResponseTemplateId;
        }
    }
}
