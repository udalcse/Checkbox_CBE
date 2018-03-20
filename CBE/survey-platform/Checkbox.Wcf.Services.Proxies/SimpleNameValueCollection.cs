using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for saving name value pairs.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SimpleNameValue
    {
        /// <summary>
        /// The name.
        /// </summary>
        [DataMember]
        [XmlElement("Name")]
        public string Name { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        [DataMember]
        [XmlElement("Value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// A simple container for saving a list of name value pairs.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SimpleNameValueCollection
    {
        private List<SimpleNameValue> _nameValueList;

        /// <summary>
        /// Constructor a new SimpleNameValueCollection
        /// </summary>
        public SimpleNameValueCollection()
        {
            _nameValueList = new List<SimpleNameValue>();
        }

        /// <summary>
        /// Initialize from source name value collection
        /// </summary>
        /// <param name="source">The NameValueCollection used to initialize the SimpleNameValueCollection.</param>
        public SimpleNameValueCollection(NameValueCollection source)
        {
            _nameValueList = new List<SimpleNameValue>();

            if (source == null)
            {
                return;
            }

            foreach (var key in source.Keys)
            {
                this[key.ToString()] = source[key.ToString()];
            }
        }


        /// <summary>
        /// Initialize from source name value collection
        /// </summary>
        /// <param name="source">The Dictionary used to initialize the SimpleNameValueCollection.</param>
        public SimpleNameValueCollection(Dictionary<string, string> source)
        {
            _nameValueList = new List<SimpleNameValue>();

            if (source == null)
            {
                return;
            }

            foreach (var key in source.Keys)
            {
                this[key] = source[key];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SimpleNameValue Add(string name, string value)
        {
            var elem = new SimpleNameValue {Name = name, Value = value};
            _nameValueList.Add(elem);
            return elem;
        }

        /// <summary>
        /// An array of name value pairs.
        /// </summary>
        [DataMember]
        public SimpleNameValue[] NameValueList { set { _nameValueList = new List<SimpleNameValue>(value);} get { return _nameValueList.ToArray(); } }

        /// <summary>
        /// Get simple name value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get 
            {
                SimpleNameValue nameValue = _nameValueList.Find(val => val.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                return nameValue != null ? nameValue.Value : string.Empty;
            }
            
            set 
            {
                SimpleNameValue nameValue = _nameValueList.Find(val => val.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                if (nameValue == null)
                {
                    _nameValueList.Add(new SimpleNameValue { Name = name, Value = value ?? string.Empty });
                }
                else
                {
                    nameValue.Value = value ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Get simple name value by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                SimpleNameValue nameValue = _nameValueList.ElementAt(index);

                if (nameValue == null)
                    throw new IndexOutOfRangeException();

                return nameValue.Value;
            }

            set
            {
                SimpleNameValue nameValue = _nameValueList.ElementAt(index);

                if (nameValue == null)
                    throw new IndexOutOfRangeException();

                nameValue.Value = value;
            }
        }

    }
}
