using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for style template data.
    /// </summary>
    [DataContract]
    public class StyleTemplateData
    {
        [DataMember]
        public int DatabaseId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public bool IsPublic { get; set; }

        [DataMember]
        public bool IsReadOnly { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }
    }
}
