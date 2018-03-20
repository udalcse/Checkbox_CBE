using System;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Survey status page
    /// </summary>
    public partial class Status : ResponseTemplatePage
    {
        /// <summary>
        /// Get page-specific text
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Form.Administer"; } }

        /// <summary>
        /// Set values for inputs and
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Use static method on survey status control to get a status summary
            _statusLbl.Text = GetActiveMessage(DateTime.Now, ResponseTemplate);

            _activateBtn.Visible = !ResponseTemplate.BehaviorSettings.IsActive;
            _deactivateBtn.Visible = ResponseTemplate.BehaviorSettings.IsActive;

            _pauseLbl.Text = ResponseTemplate.BehaviorSettings.IsActive
                ? WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/clickToPause")
                : WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/clickToResume");

            //Initialize child controls
            _timeLimits.Initialize(ResponseTemplate.BehaviorSettings);
            _responseLimits.Initialize(ResponseTemplate.BehaviorSettings);

            //Bind click events
            _activateBtn.Click += _activateBtn_Click;
            _deactivateBtn.Click += _deactivateBtn_Click;

            Master.OkClick += _okBtn_Click;

            //Set dialog title
            Master.Title = WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/title") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64);
        }

        /// <summary>
        /// Get the "Survey is active/not active/paused/etc." method.
        /// </summary>
        /// <returns></returns>
        public static string GetActiveMessage(DateTime refDate, ResponseTemplate rt)
        {
            //Check activation
            string inactiveReason;

            bool isActive = rt.BehaviorSettings.GetIsActiveOnDate(refDate, out inactiveReason);


            //If survey is active, check response count
            if (isActive)
            {
                //If response limit is set and exceeded, display message
                if (rt.BehaviorSettings.MaxTotalResponses.HasValue
                    && ResponseManager.GetResponseCount(rt.ID.Value, false, false) >= rt.BehaviorSettings.MaxTotalResponses.Value)
                {
                    return WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/totalResponseLimitExceeded");
                }

                return WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/active");
            }

            //Check if survey is "paused" and let user know that survey will be unavailable
            // regardless of activation dates.
            if (inactiveReason.Equals("notactive", StringComparison.InvariantCultureIgnoreCase))
            {
                return rt.BehaviorSettings.GetIsDateInActivationDateRange(refDate)
                    ? WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/pausedInActiveDateRange")
                    : WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/pausedOutOfActiveDateRange");
            }

            //Otherwise display a message stating survey not available due to dates.
            if (inactiveReason.Equals("afterenddate", StringComparison.InvariantCultureIgnoreCase))
            {
                return WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/notActiveEndDate");
            }

            //Only other possibility is that start date is in the future
            return WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/notActiveStartDate");
        }

        /// <summary>
        /// Get message indicating what activation dates are
        /// </summary>
        /// <returns></returns>
        public static string GetDatesMessage(DateTime refDate, ResponseTemplate rt)
        {
            //Both dates specified
            if (rt.BehaviorSettings.ActivationStartDate.HasValue
                && rt.BehaviorSettings.ActivationEndDate.HasValue)
            {
                string dateString = refDate > rt.BehaviorSettings.ActivationEndDate.Value
                    ? WebTextManager.GetText("/pageText/forms/surveys/survey.aspx/bothDatesEndPast")
                    : WebTextManager.GetText("/pageText/forms/surveys/survey.aspx/bothDatesEndFuture");

                return string.Format(dateString, rt.BehaviorSettings.ActivationStartDate, rt.BehaviorSettings.ActivationEndDate);
            }

            //Start date only
            if (rt.BehaviorSettings.ActivationStartDate.HasValue)
            {
                string dateString = refDate > rt.BehaviorSettings.ActivationStartDate.Value
                    ? WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/startDateOnlyPast")
                    : WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/startDateOnlyFuture");

                return string.Format(dateString, rt.BehaviorSettings.ActivationStartDate);
            }

            //End date only
            if (rt.BehaviorSettings.ActivationEndDate.HasValue)
            {
                string dateString = refDate > rt.BehaviorSettings.ActivationEndDate.Value
                    ? WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/endDateOnlyPast")
                    : WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/endDateOnlyFuture");

                return string.Format(dateString, rt.BehaviorSettings.ActivationEndDate);
            }

            //No dates
            return WebTextManager.GetText("/controlText/forms/surveys/infoWidgets/status/noDateRange");
        }

        /// <summary>
        /// Active survey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _deactivateBtn_Click(object sender, EventArgs e)
        {
            ResponseTemplate.BehaviorSettings.IsActive = false;
            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);

            _statusLbl.Text = GetActiveMessage(DateTime.Now, ResponseTemplate);

            _pauseLbl.Text = WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/clickToResume");

            _deactivateBtn.Visible = false;
            _activateBtn.Visible = true;
        }

        /// <summary>
        /// Deactivate survey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _activateBtn_Click(object sender, EventArgs e)
        {
            ResponseTemplate.BehaviorSettings.IsActive = true;
            ResponseTemplate.Save();
            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);

            _statusLbl.Text = GetActiveMessage(DateTime.Now, ResponseTemplate);

            _pauseLbl.Text = WebTextManager.GetText("/pageText/forms/surveyStatus.aspx/clickToPause");

            _deactivateBtn.Visible = true;
            _activateBtn.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            //Update and save settings
            _timeLimits.UpdateSettings(ResponseTemplate.BehaviorSettings);
            _responseLimits.UpdateSettings(ResponseTemplate.BehaviorSettings);

            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplateId);

            //Close window
            Master.CloseDialog("status", false);
        }
    }
}
