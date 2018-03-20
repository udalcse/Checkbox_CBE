using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Configure time limits for a survey
    /// </summary>
    public partial class TimeLimits : Checkbox.Web.Common.UserControlBase
    {
        public string ValidationErrorText { set; get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _validationError.Visible = false;

            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void Initialize(SurveyBehaviorSettings behaviorSettings)
        {
            var start = WebUtilities.ConvertToClientTimeZone(behaviorSettings.ActivationStartDate);
            var end = WebUtilities.ConvertToClientTimeZone(behaviorSettings.ActivationEndDate);

            _startDatePicker.DateTime = start;
            _endDatePicker.DateTime = end;
        }

        /// <summary>
        /// Update template
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void UpdateSettings(SurveyBehaviorSettings behaviorSettings)
        {
            var start = WebUtilities.ConvertFromClientToServerTimeZone(_startDatePicker.DateTime);
            var end = WebUtilities.ConvertFromClientToServerTimeZone(_endDatePicker.DateTime);

            behaviorSettings.ActivationStartDate = start;
            behaviorSettings.ActivationEndDate = end;
        }

        /// <summary>
        /// Validate values
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            var startDate = Utilities.GetDate(_startDatePicker.Text);
            var endDate = Utilities.GetDate(_endDatePicker.Text);

            if (!string.IsNullOrWhiteSpace(_startDatePicker.Text) && !startDate.HasValue)
            {
                ValidationErrorText = "Unable to convert [" + _startDatePicker.Text + "] to date.";
                _validationError.Visible = true;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_endDatePicker.Text) && !endDate.HasValue)
            {
                ValidationErrorText = "Unable to convert [" + _endDatePicker.Text + "] to date.";
                _validationError.Visible = true;
                return false;
            }

            startDate = WebUtilities.ConvertFromClientToServerTimeZone(startDate);

            //Check date value
            if (endDate.HasValue && startDate.HasValue && endDate < startDate)
            {
                ValidationErrorText = "Activation start date must be earlier than the activation end date.";
                _validationError.Visible = true;
                return false;
            }

            return true;
        }
    }
}