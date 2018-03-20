using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// A simple container for saving a list of name value pairs.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExpressionRightParamData
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection Options
        {
            get;
            set;
        }

        /// <summary>
        /// Available types: Text, Date, Select and None
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor a new SimpleNameValueCollection
        /// </summary>
        public ExpressionRightParamData()
        {
            Type = "None";
        }
    }
}
