using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Base definition of result grouped by a specific field.
    /// </summary>
    [DataContract]
    [Serializable]
    public class GroupedResult<T>
    {
        [DataMember]
        public string GroupField { get; set; }

        [DataMember]
        public string GroupKey { get; set; }

        /// <summary>
        /// Get/set result name values for grouped result.  It is up to consumer and generating item
        ///  to agree on significance of names/values.
        /// </summary>
        [DataMember]
        public T[] GroupResults { get; set; }
    }
}
