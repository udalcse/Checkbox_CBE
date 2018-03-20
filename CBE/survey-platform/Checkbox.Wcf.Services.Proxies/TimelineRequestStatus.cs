using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Status for the timeline request
    /// </summary>
    [DataContract]
    public enum TimelineRequestStatus
    {
        None,
        Created,
        Pending,
        Succeeded,
        Error
    }
}
