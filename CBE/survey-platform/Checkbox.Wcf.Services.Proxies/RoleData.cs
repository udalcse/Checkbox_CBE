using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for the role
    /// </summary>
    [DataContract]
    [Serializable]
    public class RoleData
    {
        ///<summary>
        /// Role name
        ///</summary>
        [DataMember]
        public string Name { set; get; }

        ///<summary>
        /// Role description
        ///</summary>
        [DataMember]
        public string Description { set; get; }

        ///<summary>
        /// List of the permissions that could be assigned to the role
        ///</summary>
        [DataMember]
        public string[] Permissions { set; get; }
    }
}
