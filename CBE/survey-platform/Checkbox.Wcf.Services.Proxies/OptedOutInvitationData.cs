using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class OptedOutInvitationData
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Email { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Type { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Date { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ResponseTemplateId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ResponseTemplateName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string UserComment { set; get; }
    }
}
