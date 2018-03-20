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
using Checkbox.Forms.Periods;


namespace CheckboxWeb.Forms.Surveys.Periods
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Edit: SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        [QueryParameter("id")]
        public int PeriodId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return null;
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
            SurveyPeriod period = ResponseTemplateManager.GetPeriod(PeriodId);
            if (period == null)
            {
                Master.ShowError(WebTextManager.GetText("/pageText/forms/surveys/periods/edit.aspx/periodNotFound"), null);
                Master.OkVisible = false;
                return;
            }
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/periods/edit.aspx/title") + " - " + Utilities.StripHtml(period.PeriodName, null));
            _name.Text = period.PeriodName;
            _start.Text = period.Start == null ? "" : period.Start.Value.ToShortDateString();
            _finish.Text = period.Finish == null ? "" : period.Finish.Value.ToShortDateString();


            Master.OkTextId = "/pageText/forms/surveys/periods/edit.aspx/save";
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
                SurveyPeriod period = ResponseTemplateManager.GetPeriod(PeriodId);
                period.PeriodName = _name.Text;

                //validate the given args
                if (string.IsNullOrEmpty(_name.Text))
                {
                    Master.ShowError("Period name can not be empty.", null);
                    return;
                }

                if (!string.IsNullOrEmpty(_start.Text))
                {
                    DateTime tmp = DateTime.Now;
                    if (!DateTime.TryParse(_start.Text,
                        new System.Globalization.CultureInfo(WebTextManager.GetUserLanguage()), System.Globalization.DateTimeStyles.AdjustToUniversal,
                        out tmp))
                        throw new ApplicationException("Start date has invalid format.");
                    period.Start = tmp;
                }

                if (!string.IsNullOrEmpty(_finish.Text))
                {
                    DateTime tmp = DateTime.Now;
                    if (!DateTime.TryParse(_finish.Text,
                        new System.Globalization.CultureInfo(WebTextManager.GetUserLanguage()), System.Globalization.DateTimeStyles.AdjustToUniversal,
                        out tmp))
                        throw new ApplicationException("Finish date has invalid format.");
                    period.Finish = tmp;
                }

                period.Save();

                var args = new Dictionary<string, string>          
                {
                               {"op", "editPeriod"},
                               {"periodId", PeriodId.ToString()}
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
