namespace Checkbox.Forms
{
    /// <summary>
    /// Enumeration of render modes for survey/report items and can be used by 
    /// renderers to provide different behavior depending on context.
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// Respondent taking a survey.
        /// </summary>
        Survey,

        /// <summary>
        /// Editing a survey.
        /// </summary>
        SurveyEditor,

        /// <summary>
        /// Previewing a survey.
        /// </summary>
        SurveyPreview,

        /// <summary>
        /// Editing a library
        /// </summary>
        LibraryEditor,

        /// <summary>
        /// Previewing a library.
        /// </summary>
        LibraryPreview,

        /// <summary>
        /// Running a report.
        /// </summary>
        Report,

        /// <summary>
        /// Editing a report.
        /// </summary>
        ReportEditor,

        /// <summary>
        /// Read-only view of a response
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Respondent taking a survey on a mobile device
        /// </summary>
        SurveyMobile,

        /// <summary>
        /// Previewing a mobile-styled survey.
        /// </summary>
        SurveyMobilePreview,

        /// <summary>
        /// 
        /// </summary>
        Default
    }
}
