using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container to expose user properties via services.
    /// </summary>
    [DataContract]
    [Serializable]
    public class PageItemUserData
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [DataMember]
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// "Escaped" version of unique identifier that replaces backslash with
        /// _BACKSLASH_ to avoid issues where jquery ajax removes characters from
        /// URL
        /// </summary>
        [DataMember]
        public string EscapedUniqueIdentifier {
            get { return UniqueIdentifier.Replace(@"\", @"_BACKSLASH_"); }
            set {; }
        }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [DataMember]
        public List<string> AllEmails { get; set; }

        /// <summary>
        /// Uses to check user availability
        /// </summary>
        [DataMember]
        public bool IsInList { get; set; }
    }
}
