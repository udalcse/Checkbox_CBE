using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Search data about one found entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class SearchAnswerData
    {
        /// <summary>
        /// Type of the found entity: Survey, User, Response, Invitation, Item, Report, ...
        /// </summary>
        [DataMember]
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Matched field name
        /// </summary>
        [DataMember]
        public string MatchedField
        {
            get;
            set;
        }

        /// <summary>
        /// The text that matches the search term
        /// </summary>
        [DataMember]
        public string MatchedText
        {
            get;
            set;
        }

        /// <summary>
        /// Object ID as long int if exists. Makes sense for surveys, items, reports, invitations, etc
        /// </summary>
        [DataMember]
        public long? ObjectID
        {
            get;
            set;
        }

        /// <summary>
        /// Object ID as string if exists. Makes sense for users
        /// </summary>
        [DataMember]
        public string ObjectIDString
        {
            get;
            set;
        }

        /// <summary>
        /// Object ID as GUID if exists. Makes sense for responses, surveys
        /// </summary>
        [DataMember]
        public Guid? ObjectGUID
        {
            get;
            set;
        }

        /// <summary>
        /// User friendly name
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            set;
        }
    }
}
