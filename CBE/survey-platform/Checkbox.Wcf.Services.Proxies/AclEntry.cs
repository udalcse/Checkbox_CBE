using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// A lightweight object which describes an Access Control List (ACL) entry.
    /// </summary>
    [DataContract]
    [Serializable]
    public class AclEntry
    {
        /// <summary>
        /// Indicates the type of the the entity.
        /// <remarks>
        /// Entries will either be a user or user group.
        /// <para>User will be of type: "Prezza.Framework.Security.ExtendedPrincipal"</para>
        /// <para>User groups will be of type: "Checkbox.Users.Group"</para>
        /// </remarks>
        /// </summary>
        [DataMember]
        public string EntryType { get; set; }

        /// <summary>
        /// The unique identifier of entity being granted access.
        /// <remarks>
        /// <para>If the entity is a user the value is their unique identifier.</para>
        /// <para>If the entity is a user groups, the value is the group id as a string. In most cases the value will need to be cast to an int before it can be used.</para>
        /// </remarks>
        /// </summary>
        [DataMember]
        public string EntryIdentifier { get; set; }
        
        /// <summary>
        /// Database id of policy associated with ACL entry
        /// </summary>
        [DataMember]
        public int AclPolicyId { get; set; }

        /// <summary>
        /// A truncated version of the FullEntryIdentifier. 
        /// <remarks>
        /// <para>If the EntryType is a user the identifier is the user's unique identifier.</para>
        /// <para>If the EntryType is a user group the identifier will be the group's name.</para>
        /// </remarks>
        /// </summary>
        [DataMember]
        public string ShortEntryIdentifier { get; set; }

        /// <summary>
        /// <remarks>
        /// <para>If the EntryType is a user the identifier is the user's unique identifier.</para>
        /// <para>If the EntryType is a user group the identifier will be the group's name.</para>
        /// </remarks>
        /// </summary>
        [DataMember]
        public string FullEntryIdentifier { get; set; }

        ///<summary>
        /// Uses to check entry availability
        ///</summary>
        [DataMember]
        public bool IsInList { get; set; }
    }
}
