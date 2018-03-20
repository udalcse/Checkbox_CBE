using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Composite Item for Response Export
    /// </summary>
    [DataContract]
    [Serializable]
    public class TabularResponseExportData
    {
        [DataMember]
        public SimpleNameValueCollection[] Responses { get; set; }

        [DataMember]
        public UserData[] Respondents { get; set; }
    }
}
