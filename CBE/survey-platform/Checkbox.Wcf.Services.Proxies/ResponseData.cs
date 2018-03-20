using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Encapsulation of summary data associated with survey response.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResponseData
    {
        /// <summary>
        /// Get the GUID associated with the response.
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Get the user identifier associated with the respondent.
        /// </summary>
        /// <remarks>This field will return 'AnonymousRespondent' for anonymous responses and 'AnonymizedRespondent' for anonymized responses.</remarks>
        [DataMember]
        public string UserIdentifier { get; set; }

        /// <summary>
        /// Get the date/time the response was completed.
        /// </summary>
        /// <remarks>For incomplete responses, this value will be null.</remarks>
        [DataMember]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// Get the IP address recorded for the respondent.
        /// </summary>
        [DataMember]
        public string RespondentIp { get; set; }

        /// <summary>
        /// Get the language for the response.
        /// </summary>
        [DataMember]
        public string ResponseLanguage { get; set; }

        /// <summary>
        /// Get the Active Directory user name associated with the response. 
        /// </summary>
        /// <remarks>This value is set by IIS and is dependent upon configuration of the web server.  If the
        /// Checkbox LogNetworkUser setting is disabled, this value will always be null.</remarks>
        [DataMember]
        public string NetworkUser { get; set; }

        /// <summary>
        /// Get the date/time the response was last edited.
        /// </summary>
        [DataMember]
        public DateTime? LastEditDate { get; set; }

        /// <summary>
        /// Get the date/time the response was started.
        /// </summary>
        [DataMember]
        public DateTime? Started { get; set; }

        /// <summary>
        /// Get the numeric database ID of the response.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>User
        /// Get the position of the last survey page viewed for the response.
        /// </summary>
        [DataMember]
        public int LastPageViewed { get; set; }

        /// <summary>
        /// Get the GUID associated with anonymous respondents to the survey.  The guid is stored as
        /// a cookie on the respondents computer so it is subject to any browser or privacy
        /// restrictions set by the user.
        /// </summary>
        [DataMember]
        public Guid? AnonymousRespondentGuid { get; set; }

        /// <summary>
        /// Get invitee associated with response.  If responses are anonymized for the survey associated with the 
        /// invitation, no data will be returned.
        /// </summary>
        [DataMember]
        public string Invitee { get; set; }

        /// <summary>
        /// Get indication of whether response is anonymized due to survey settings.
        /// </summary>
        [DataMember]
        public bool IsAnonymized { get; set; }

        /// <summary>
        /// Get indication of whether response is a "test" response.
        /// </summary>
        [DataMember]
        public bool IsTest { get; set; }

        /// <summary>
        /// Get instance id of related workflow
        /// </summary>
        [DataMember]
        public Guid? WorkflowSessionId { get; set; }

        /// <summary>
        /// Name of survey for the response
        /// </summary>
        [DataMember]
        public string SurveyName { get; set; }

        /// <summary>
        /// Response Answers
        /// </summary>
        [DataMember]
        public ResponseItemAnswerData[] Answers { get; set; }
    }
}
