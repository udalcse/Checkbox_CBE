using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Messaging.Email;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Security.Principal;
using Checkbox.Pagination;
using Checkbox.Common;

namespace CheckboxWeb.Forms.Surveys.Periods
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Add : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        [QueryParameter("s")]
        public int SurveyId { get; set; }

        /// <summary>
        /// Get current response template
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(SurveyId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return ResponseTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Form.Administer"; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkTextId = "/pageText/forms/surveys/periods/add.aspx/create";
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/periods/add.aspx/title") + " - " + Utilities.StripHtml(ResponseTemplate.Name, null));

            Master.OkClick += new EventHandler(Master_OkClick);
        }

        /// <summary>
        /// Save a period
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            try
            {
                int newPeriodId = ResponseTemplate.AddPeriod(_name.Text, _start.Text, _finish.Text);

                var args = new Dictionary<string, string>
                           {
                               {"op", "addPeriod"},
                               {"periodId", newPeriodId.ToString()}
                           };
                Master.CloseDialog(args);
            }
            catch (Exception ex)
            {
                Master.ShowError(ex.Message, ex);
            }
        }


        /// <summary>
        /// Bind events to store cursor position for pipe insertion
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
        }

    }
}
