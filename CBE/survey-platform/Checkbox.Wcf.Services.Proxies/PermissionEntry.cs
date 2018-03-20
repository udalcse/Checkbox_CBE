using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class PermissionEntry
    {
        [DataMember]
        public string PermissionName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool Selected { get; set; }

        [DataMember]
        public bool Disabled { get; set; }
    }
}
