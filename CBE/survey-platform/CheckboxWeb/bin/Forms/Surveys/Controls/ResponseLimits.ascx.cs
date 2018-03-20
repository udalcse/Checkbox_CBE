using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Editor for survey response limits
    /// </summary>
    public partial class ResponseLimits : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize control.
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void Initialize(SurveyBehaviorSettings behaviorSettings)
        {
            _perRespondentLimit.Text = behaviorSettings.MaxResponsesPerUser.HasValue
                ? behaviorSettings.MaxResponsesPerUser.ToString()
                : string.Empty;

            _totalResponseLimit.Text = behaviorSettings.MaxTotalResponses.HasValue
                ? behaviorSettings.MaxTotalResponses.ToString()
                : string.Empty;
        }

        /// <summary>
        /// Update template with user input
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void UpdateSettings(SurveyBehaviorSettings behaviorSettings)
        {
            behaviorSettings.MaxResponsesPerUser = Utilities.AsInt(_perRespondentLimit.Text);
            behaviorSettings.MaxTotalResponses = Utilities.AsInt(_totalResponseLimit.Text);
        }
    }
}