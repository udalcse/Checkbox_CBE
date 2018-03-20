using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class ResponseItemAnswerData : ItemProxyObject
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public ResponseItemAnswer[] Answers { get; set; }
    }
}
