using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class StyleListItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? DateCreated { get; set; }

        [DataMember]
        public bool IsInUse { get; set; }

        [DataMember]
        public bool CanBeEdited { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }
    }
}
