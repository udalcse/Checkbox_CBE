using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Composite Item for Response Export
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResponseExportData
    {
        [DataMember]
        public ResponseData[] Responses { get; set; }

        [DataMember]
        public UserData[] Respondents { get; set; }

        //[DataMember]
        //public ResponseItemAnswerData[] Answers { get; set; }
    }
}
