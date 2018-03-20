using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Page logic (branching ang conditions count).
    /// </summary>
    [Serializable]
    [DataContract]
    public class PageLogic
    {
        /// <summary>
        /// Page id
        /// </summary>
        [DataMember]
        public int PageId { get; set; }

        /// <summary>
        /// Number of branches  of page
        /// </summary>
        [DataMember]
        public int BranchesCount { get; set; }

        /// <summary>
        /// Number of conditions of page
        /// </summary>
        [DataMember]
        public int ConditionsCount { get; set; }
    }
}
