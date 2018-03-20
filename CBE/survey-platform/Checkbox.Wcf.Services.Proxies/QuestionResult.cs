using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// The class that contains information about yes/no questions.
    /// </summary>
    [DataContract]
    [Serializable]    
    public class QuestionResult
    {
        /// <summary>
        /// Determine if the answer is positive or no.
        /// </summary>
        [DataMember]
        public bool Yes { get; set; }

        /// <summary>
        /// Message that describes operation result.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}
