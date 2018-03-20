using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Representation of a data source for piping tokens
    /// </summary>
    [Serializable]
    [DataContract]
    public class PipeSource
    {
        /// <summary>
        /// Source "type" for pipe
        /// </summary>
        [DataMember]
        public string SourceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string PipeToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DisplayText { get; set; }

        /// <summary>
        /// field describing filed type
        /// </summary>
        [DataMember]
        public string FieldType { get; set; }

        /// <summary>
        /// Representing binding
        /// </summary>
        [DataMember]
        public List<int> BindedItemId { get; set; }
    }
}
