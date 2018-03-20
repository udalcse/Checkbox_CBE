using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Base class for data transfer objects for runtime survey and report items.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ItemProxyObject : IItemProxyObject
    {
        [DataMember]
        public int ItemId { get; set; }

        [DataMember]
        public int PageId { get; set; }

        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public int ParentTemplateId { get; set; }

        [DataMember]
        public SimpleNameValueCollection Metadata { get; set; }

        [DataMember]
        public SimpleNameValueCollection InstanceData { get; set; }

        [DataMember]
        public SimpleNameValueCollection AppearanceData { get; set; }

        [DataMember]
        public object AdditionalData { get; set; }

        [DataMember]
        public bool? IsLastItemOnPage { get; set; }

        
    }
}
