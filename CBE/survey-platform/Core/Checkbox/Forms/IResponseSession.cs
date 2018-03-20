using System;
using Checkbox.Security.Principal;

namespace Checkbox.Forms
{
    /// <summary>
    /// Interface definition for container of session information for a respondent's
    /// journey through a survey.
    /// </summary>
    public interface IResponseSession
    {
        /// <summary>
        /// Clear any values that are persisted as part of the response session.
        /// </summary>
        void ClearPersistedValues();

        /// <summary>
        /// Get/set the GUID identifying the survey.  Nulls are allowed so that the controller and 
        /// survey interface can detect and provide appropriate error information to the 
        /// respondent.
        /// </summary>
        Guid? SurveyGuid { get; set; }

        /// <summary>
        /// Get the GUID of the current response.  This value may be null when first starting
        /// a survey, or if a respondent's session is lost mid-survey.
        /// </summary>
        Guid? ResponseGuid { get; set; }

        /// <summary>
        /// Get the GUID of the invitation associated with this response.  If no invitation
        /// recipient is associated, a null value should be returned.
        /// </summary>
        Guid? RecipientGuid { get; set; }

        /// <summary>
        /// Get/set the index of the current page in the response.
        /// </summary>
        int? CurrentPageId { get; set; }

        /// <summary>
        /// Get/set the password entered by the respondent for the survey.  This value may be null.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Get/set the language code for the response session.
        /// </summary>
        string LanguageCode { get; set; }

        /// <summary>
        /// Get/set the respondent associated with the response session.
        /// </summary>
        CheckboxPrincipal Respondent { get; set; }

        /// <summary>
        /// Get/set name of the server user context.
        /// </summary>
        string ServerUserContext { get; set; }

        /// <summary>
        /// Get guid of current user
        /// </summary>
        Guid? UserGuid { get; set; }

        /// <summary>
        /// Get/set a guid to track anonymous respondents.  If anonymous user tracking
        /// is disabled, null values should be allowed.
        /// </summary>
        Guid? AnonymousRespondentGuid { get; set; }

        /// <summary>
        /// Load the response session information.
        /// </summary>
        void Load();

        /// <summary>
        /// Persist the response session information.
        /// </summary>
        void Save();

        /// <summary>
        /// Determine the respondent language code.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        string GetRespondentLanguageCode(string source, string token);

        /// <summary>
        /// Get/set IP address of respondent
        /// </summary>
        string RespondentIPAddress { get; set; }

        /// <summary>
        /// Get/set whether respondent is editing.
        /// </summary>
        bool RespondentEditMode { get; set; }

        /// <summary>
        /// Get/set whether administrator is editing.
        /// </summary>
        bool AdminEditMode { get; set; }

        /// <summary>
        /// Get/set whether to force creation of a new response
        /// </summary>
        bool ForceNew { get; set; }

        /// <summary>
        /// Get/set whether a response is a test
        /// </summary>
        bool IsTest { get; set; }

        /// <summary>
        /// Get a boolean indicating if the current page is a form postback or not
        /// </summary>
        bool IsFormPost { get; }
    }
}
