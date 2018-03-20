using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for metadata surveys terms.
    /// </summary>
    [DataContract]
    [Serializable]
    public class TermsMetaData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Term { get; set; }

        [DataMember]
        public string Definition { get; set; }

        [DataMember]
        public string Link { get; set; }

    }
}
