using System;
using System.Web;

using Checkbox.Forms;
using Checkbox.Management;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validate whether a survey name is in use or not
    /// </summary>
    public class CalloutSurveyNameInUseValidator : WarningCalloutValidator
    {
        private string _currentTemplateName;

        /// <summary>
        /// Validate the s
        /// </summary>
        public string CurrentTemplateName
        {
            get { return _currentTemplateName; }
            set { _currentTemplateName = value; }
        }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (value != null)
                value = value.Trim();

//            //there is a separate validator used to determine if no name has been specified
//            if (string.Empty.Equals(value))
//                return true;

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = HttpContext.Current.Server.HtmlEncode(value);
            }

            if (value.Equals(_currentTemplateName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            
            return !ResponseTemplateManager.ResponseTemplateExists(value);
        }
        
    }
}
