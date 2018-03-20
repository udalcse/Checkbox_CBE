using System;

using Checkbox.Management;
using Checkbox.Security.Principal;

namespace Checkbox.Forms
{
    /// <summary>
    /// Abstract base class for UI specific implementations of response session objects.
    /// </summary>
    public abstract class ResponseSession : IResponseSession
    {
        private readonly bool _isFormPost;
        private Guid? _anonymousRespondentGuid;

        /// <summary>
        /// Construct a new response session
        /// </summary>
        /// <param name="isFormPost"></param>
        protected ResponseSession(bool isFormPost)
        {
            _isFormPost = isFormPost;
        }

        /// <summary>
        /// Clear any values persisted by the response session
        /// </summary>
        public abstract void ClearPersistedValues();

        /// <summary>
        /// Get a guid associated with the current anonymous respondent
        /// </summary>
        /// <returns></returns>
        protected abstract Guid? GetAnonymousRespondentGuid();

        /// <summary>
        /// Set a guid associated with the current anonymous respondent
        /// </summary>
        protected abstract void SetAnonymousRespondentGuid(Guid theGuid);

        /// <summary>
        /// Load the response session
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Save the response session
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Get the respondent language code from the specified location.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public abstract string GetRespondentLanguageCode(string source, string variableName);

        /// <summary>
        /// Get/set the anonymous respondent guid
        /// </summary>
        public virtual Guid? AnonymousRespondentGuid
        {
            get
            {
                //See if a guid has been present or stored
                if (_anonymousRespondentGuid.HasValue)
                {
                    return _anonymousRespondentGuid;
                }

                Guid? guid = null;

                //In cases where the session type is "cookieless" the effect is that anonymous responses are not tracked
                //since that is the only current way the app uses this setting.  For backwards compatibility we'll
                //use this setting until the tracking options are expanded, so a new guid is returned every time

                //Now try to get the value from a persistant store or other location
                if (ApplicationManager.AppSettings.SessionMode != AppSettings.SessionType.Cookieless)
                {
                    guid = GetAnonymousRespondentGuid();
                }

                //Generate a new guid
                if (!guid.HasValue)
                {
                    guid = Guid.NewGuid();
                }

                AnonymousRespondentGuid = guid;

                return guid;
            }
            set
            {
                if (value != null)
                {
                    _anonymousRespondentGuid = value;
                    SetAnonymousRespondentGuid(value.Value);
                }
            }
        }

        /// <summary>
        /// Get/set the server user context
        /// </summary>
        public virtual string ServerUserContext { get; set; }

        /// <summary>
        /// Get/set the survey guid
        /// </summary>
        public virtual Guid? SurveyGuid { get; set; }

        /// <summary>
        /// Get/set the response guid
        /// </summary>
        public virtual Guid? ResponseGuid { get; set; }

        /// <summary>
        /// Get/set invitation guid
        /// </summary>
        public virtual Guid? RecipientGuid { get; set; }

        /// <summary>
        /// Get/set guid of registered user
        /// </summary>
        public virtual Guid? UserGuid { get; set; }

        /// <summary>
        /// Get/set password
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Get/set language code
        /// </summary>
        public virtual string LanguageCode { get; set; }

        /// <summary>
        /// Get/set respondent
        /// </summary>
        public virtual CheckboxPrincipal Respondent { get; set; }

        /// <summary>
        /// Get/set edit mode for respondent
        /// </summary>
        public virtual bool RespondentEditMode { get; set; }

        /// <summary>
        /// Get/set edit mode for admin
        /// </summary>
        public virtual bool AdminEditMode { get; set; }

        /// <summary>
        /// Get/set page to go to
        /// </summary>
        public virtual int? CurrentPageId { get; set; }

        /// <summary>
        /// Get/set force new
        /// </summary>
        public virtual bool ForceNew { get; set; }

        /// <summary>
        /// Get/set test mode
        /// </summary>
        public virtual bool IsTest { get; set; }

        /// <summary>
        /// Get/set respondent ip address
        /// </summary>
        public virtual string RespondentIPAddress { get; set; }

        /// <summary>
        /// Get a boolean indicating if the current
        /// </summary>
        public virtual bool IsFormPost
        {
            get { return _isFormPost; }
        }
    }
}
