using System;
using System.Web;

using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Security.Principal;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validate whether a survey name is in use or not
    /// </summary>
    public class CalloutFolderNameInUseValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Validate the s
        /// </summary>
        public string CurrentFolderName { get; set; }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = HttpContext.Current.Server.HtmlEncode(value);
            }

            if (value.Equals(CurrentFolderName, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            
            return !FolderManager.FolderExists(null, value, HttpContext.Current.User as CheckboxPrincipal);
        }

    }
}
