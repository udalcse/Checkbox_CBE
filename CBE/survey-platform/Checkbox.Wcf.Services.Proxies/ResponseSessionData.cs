using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for survey response session data.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ResponseSessionData
    {
        [DataMember]
        public Guid SessionGuid { get; set; }

        [DataMember]
        public Int64? ResponseId { get; set; }

        [DataMember]
        public Guid? ResponseGuid { get; set; }

        [DataMember]
        public Int32? ResponseTemplateId { get; set; }

        [DataMember]
        public Guid? ResponseTemplateGuid { get; set; }

        [DataMember]
        public string AuthenticatedRespondentUid { get; set; }

        [DataMember]
        public Guid? AnonymousRespondentGuid { get; set; }

        [DataMember]
        public Guid? InvitationRecipientGuid { get; set; }

        [DataMember]
        public Guid? DirectInvitationRecipientGuid { get; set; }

        [DataMember]
        public long? InvitationRecipientId { get; set; }

        [DataMember]
        public string EnteredPassword { get; set; }

        [DataMember]
        public string SelectedLanguage { get; set; }

        [DataMember]
        public ResponseSessionState SessionState { get; set; }

        [DataMember]
        public string RespondentIpAddress { get; set; }

        [DataMember]
        public string NetworkUser { get; set; }

        [DataMember]
        public bool IsTest { get; set; }

        [DataMember]
        public bool IsEdit { get; set; }

        [DataMember]
        public bool ForceNew { get; set; }

        [DataMember]
        public Guid? ResumeInstanceId { get; set; }

        [DataMember]
        public string Invitee { get; set; }

        [DataMember]
        public string UserToRestore { get; set; }

        [DataMember]
        public Dictionary<string,string> AnonymousBindedFields { get; set; }
    }
}
