namespace Checkbox.Forms
{
    /// <summary>
    /// Enumeration of edit modes supported for item editors.  Value can be used by item editors
    /// to provide different behavior based on context.
    /// </summary>
    public enum EditMode
    {
        /// <summary>
        /// Item is in or is being added to a survey.
        /// </summary>
        Survey,

        /// <summary>
        /// Item is in or is being added to an item library.
        /// </summary>
        Library,

        /// <summary>
        /// Item is in or is being added to a report.
        /// </summary>
        Report
    }
}
