using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Enumeration of states for view control
    /// </summary>
    [Serializable]
    public enum ResponseSessionState
    {
        /// <summary>
        /// No display
        /// </summary>
        None = 0,

        /// <summary>
        /// Display survey UI
        /// </summary>
        TakeSurvey = 1,

        /// <summary>
        /// Display language selection dialog
        /// </summary>
        SelectLanguage = 2,

        /// <summary>
        /// Display survey password dialog
        /// </summary>
        EnterPassword = 3,

        /// <summary>
        /// Display progress saved dialog
        /// </summary>
        SavedProgress = 4,

        /// <summary>
        /// Display response list
        /// </summary>
        EditResponse = 5,

        /// <summary>
        /// Respondent needs to be identified
        /// </summary>
        RespondentRequired = 6,

        /// <summary>
        /// Go to login page
        /// </summary>
        LoginRequired = 7,

        /// <summary>
        /// 
        /// </summary>
        Completed = 8,

        /// <summary>
        /// Survey not active
        /// </summary>
        SurveyNotActive = 9,

        /// <summary>
        /// Respondent not authorized
        /// </summary>
        RespondentNotAuthorized = 10,

        /// <summary>
        /// Respondent attempting to resume response hosted in a different workflow
        /// </summary>
        ResumeExistingWorkflow = 11,

        /// <summary>
        /// Display an error
        /// </summary>
        Error = 12,

        /// <summary>
        /// Response limit reached
        /// </summary>
        ResponseLimitReached = 13,

        /// <summary>
        /// Survey activation date is in the future.
        /// </summary>
        BeforeStartDate = 14,

        /// <summary>
        /// Survey activation date is in the past.
        /// </summary>
        AfterEndDate = 15,

        /// <summary>
        /// Survey requires invitation but none found.
        /// </summary>
        InvitationRequired = 16,

        /// Survey has been deleted
        /// </summary>
        SurveyDeleted = 17
    }
}
