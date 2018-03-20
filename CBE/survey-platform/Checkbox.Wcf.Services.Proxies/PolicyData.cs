using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class PolicyData
    {
        /// <summary>
        /// Database id of policy
        /// </summary>
        [DataMember]
        public int PolicyId { get; set; }


    }
}
