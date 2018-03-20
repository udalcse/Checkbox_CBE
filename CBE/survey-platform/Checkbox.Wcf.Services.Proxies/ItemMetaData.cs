using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Base class for item meta data configured through item editor service interface.  This includes full meta data information
    /// that may go beyond the basic metadata name/value collection that is passed
    /// </summary>
    [Serializable]
    [DataContract]
    public class ItemMetaData : IItemMetadata
    {
        /// <summary>
        /// Id of item.
        /// </summary>
        [DataMember]
        public int ItemId { get; set; }

        /// <summary>
        /// Type name of item.
        /// </summary>
        [DataMember]
        public string TypeName { get; set; }

        /// <summary>
        /// Position of page containing item.
        /// </summary>
        [DataMember]
        public int PagePosition { get; set; }

        /// <summary>
        /// Position of item within containing page.
        /// </summary>
        [DataMember]
        public int ItemPosition { get; set; }

        /// <summary>
        /// Property collection associated with item.
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection Properties { get; set; }

        /// <summary>
        /// Additional data associated with item.
        /// </summary>
        [DataMember]
        public object AdditionalData { get; set; }

        /// <summary>
        /// Text data associated with item
        /// </summary>
        [DataMember]
        public TextData[] TextData { get; set; }

        /// <summary>
        /// Property indexor
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        public string this[string propertyKey]
        {
            get { return Properties != null ? Properties[propertyKey] : string.Empty; }
            set
            {
                if (Properties == null)
                {
                    Properties = new SimpleNameValueCollection();
                }

                Properties[propertyKey] = value;
            }
        }
    }
}
