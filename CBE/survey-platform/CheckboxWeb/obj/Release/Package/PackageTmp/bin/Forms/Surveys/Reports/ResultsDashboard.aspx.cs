using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ResultsDashboard : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Id of survey
        /// </summary>
        [QueryParameter("s", IsRequired = true)]
        public int SurveyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string  ControllableEntityRequiredPermission
        {
            get { return "Analyis.Run"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }
    }
}