using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class PermissionMaskEntry
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MaskName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PermissionMaskState MaskState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PermissionEntry[] Permissions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Disabled { get; set; }
    }
}
