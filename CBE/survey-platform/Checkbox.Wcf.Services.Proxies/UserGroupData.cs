using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for user group data via services.
    /// </summary>
    [DataContract]
    [Serializable]
    public class UserGroupData
    {
        /// <summary>
        /// The group's unique id.
        /// </summary>
        [DataMember]
        public int DatabaseId { get; set; }

        /// <summary>
        /// The group's name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The group's description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// The total number of users in the group.
        /// </summary>
        [DataMember]
        public int MemberCount { get; set; }

        /// <summary>
        /// The name of the user that created the group.
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// The ability of current user to delete group
        /// </summary>
        [DataMember]
        public bool? CanDelete { get; set; }

        /// <summary>
        /// The ability of current user to copy group
        /// </summary>
        [DataMember]
        public bool? CanCopy { get; set; }
    }
}
