using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Metadata for survey item options. Such as options available in select lists, etc.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyOptionMetaData
    {
        /// <summary>
        /// Option database id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Get positio of option
        /// </summary>
        [DataMember]
        public int Position { get; set; }

        /// <summary>
        /// Get whether option is selected by default
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Option is an "other" option.
        /// </summary>
        [DataMember]
        public bool IsOther { get; set; }

        /// <summary>
        /// Option is an "none of above" option.
        /// </summary>
        [DataMember]
        public bool IsNoneOfAbove { get; set; }

        /// <summary>
        /// Text data for options.  Text property is "OptionText"
        /// </summary>
        [DataMember]
        public TextData[] TextData { get; set; }

        /// <summary>
        /// Option alias
        /// </summary>
        [DataMember]
        public string Alias { get; set; }

        /// <summary>
        /// Point value of option
        /// </summary>
        [DataMember]
        public double Points { get; set; }
    }
}
