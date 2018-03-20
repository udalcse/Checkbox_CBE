namespace Checkbox.Forms
{
    /// <summary>
    /// Template page type enumeration
    /// </summary>
    public enum TemplatePageType
    {
        /// <summary>
        /// Normal content page in a template.
        /// </summary>
        ContentPage = 1,

        /// <summary>
        /// Page contains items that are not shown
        /// </summary>
        HiddenItems,

        /// <summary>
        /// Page contains items that are shown, but do not take
        /// input.
        /// </summary>
        Completion
    }
}
