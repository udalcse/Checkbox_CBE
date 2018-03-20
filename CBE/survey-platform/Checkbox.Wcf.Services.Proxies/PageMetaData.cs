using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Base class for page metada classes
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class PageMetaData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int[] ItemIds { get; set; }
    }
}
