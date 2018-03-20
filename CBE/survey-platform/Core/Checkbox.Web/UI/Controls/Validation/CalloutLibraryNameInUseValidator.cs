using System;
using System.Web;

using Checkbox.Forms;
using Checkbox.Management;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Validate that the specified library name is not in use
    /// </summary>
    public class CalloutLibraryNameInUseValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Get/set the id of the existing library
        /// </summary>
        public int? ExistingLibraryID { get; set; }

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

            return !LibraryTemplateManager.LibraryTemplateExists(value, ExistingLibraryID);
        }
    }
}
