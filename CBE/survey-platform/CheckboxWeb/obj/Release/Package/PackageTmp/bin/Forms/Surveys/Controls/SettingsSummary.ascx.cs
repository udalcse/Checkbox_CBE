using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Summary control for response template settings
    /// </summary>
    public partial class SettingsSummary : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void Initialize(SurveyBehaviorSettings behaviorSettings)
        {
            //Permissions
            _selectedPermissionLbl.Text = WebTextManager.GetText("/enum/securityType/" + behaviorSettings.SecurityType);
            _permissionDescriptionLbl.Text = WebTextManager.GetText("/enum/securityType/" + behaviorSettings.SecurityType + "/description");
            
            //Behavior
            _allowEditValueLbl.Text = behaviorSettings.AllowEdit
                ? WebTextManager.GetText("/common/yes")
                : WebTextManager.GetText("/common/no");

            _allowResumeValueLbl.Text = behaviorSettings.AllowContinue
                ? WebTextManager.GetText("/common/yes")
                : WebTextManager.GetText("/common/no");

            _saveAndQuitValueLbl.Text = behaviorSettings.ShowSaveAndQuit
                ? WebTextManager.GetText("/common/yes")
                : WebTextManager.GetText("/common/no");

            _backButtonValueLbl.Text = !behaviorSettings.DisableBackButton
                ? WebTextManager.GetText("/common/yes")
                : WebTextManager.GetText("/common/no");
            
            //Response limits
            _totalLimitValueLbl.Text = behaviorSettings.MaxTotalResponses.HasValue
                ? behaviorSettings.MaxTotalResponses.ToString()
                : WebTextManager.GetText("/pageText/forms/surveys/controls/settingsSummary.ascx/noLimit");

            _perRespondentLimitValueLbl.Text = behaviorSettings.MaxResponsesPerUser.HasValue
                ? behaviorSettings.MaxResponsesPerUser.ToString()
                : WebTextManager.GetText("/pageText/forms/surveys/controls/settingsSummary.ascx/noLimit");

            //Time Limits
            _startDateValueLbl.Text = behaviorSettings.ActivationStartDate.HasValue
                ? WebUtilities.ConvertToClientTimeZone(behaviorSettings.ActivationStartDate).ToString()
                : WebTextManager.GetText("/pageText/forms/surveys/controls/settingsSummary.ascx/noLimit");

            _endDateValueLbl.Text = behaviorSettings.ActivationEndDate.HasValue
                ? WebUtilities.ConvertToClientTimeZone(behaviorSettings.ActivationEndDate).ToString()
                : WebTextManager.GetText("/pageText/forms/surveys/controls/settingsSummary.ascx/noLimit");
        }
    }
}