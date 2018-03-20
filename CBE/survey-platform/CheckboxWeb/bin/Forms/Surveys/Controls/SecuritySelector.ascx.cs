using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class SecuritySelector : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/set selected security type
        /// </summary>
        public SecurityType SecurityType
        {
            get 
            {
                try
                {
                    if(Utilities.IsNotNullOrEmpty(_securitySelection.SelectedValue))
                    {
                        return (SecurityType)Enum.Parse(typeof(SecurityType), _securitySelection.SelectedValue);
                    }
                }
                catch
                {
                }

                return ApplicationManager.AppSettings.DefaultSurveySecurityType;
            }
            set 
            {
                string valueAsString = value.ToString();

                if (_securitySelection.Items.FindByValue(valueAsString) != null)
                {
                    _securitySelection.SelectedValue = valueAsString;
                }
            }
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="responseTemplate"></param>
        /// <param name="currentPrincipal"></param>
        /// <param name="isPagePostback"></param>
        /// <param name="loadGrantAccessList"></param>
        public void Initialize(ResponseTemplate responseTemplate, CheckboxPrincipal currentPrincipal, bool isPagePostback, bool loadGrantAccessList)
        {
            if (!isPagePostback)
            {
                SecurityType = responseTemplate.BehaviorSettings.SecurityType;
                _passwordTxt.Text = responseTemplate.BehaviorSettings.Password;
            }

            _grantSurveyAccess.Initialize(SecuredResourceType.Survey, responseTemplate.ID.Value, "Form.Fill");
        }

        /// <summary>
        /// Apply selected settings to response template
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void UpdateSettings(SurveyBehaviorSettings behaviorSettings)
        {
            //Set security values
            behaviorSettings.SecurityType = (SecurityType)Enum.Parse(typeof(SecurityType), _securitySelection.SelectedValue);

            if (behaviorSettings.SecurityType == SecurityType.PasswordProtected)
            {
                behaviorSettings.Password = _passwordTxt.Text.Trim();
            }
        }
    }
}