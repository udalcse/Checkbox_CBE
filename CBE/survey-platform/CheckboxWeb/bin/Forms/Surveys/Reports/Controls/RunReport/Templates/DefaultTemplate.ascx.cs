using Checkbox.Web.Forms.UI.Templates;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport.Templates
{
    /// <summary>
    /// Default layout template to apply when one is not specified for the page.
    /// </summary>
    public partial class DefaultTemplate : UserControlLayoutTemplate
    {
        /// <summary>
        /// Template has no configurable properties
        /// </summary>
        public override bool ReadOnly { get { return true; } }
    }
}