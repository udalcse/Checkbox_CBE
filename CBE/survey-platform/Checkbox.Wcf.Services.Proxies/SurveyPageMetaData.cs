using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for metadata associated with survey pages.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyPageMetaData : PageMetaData
    {
        /// <summary>
        /// Get/Set the layout template id.
        /// </summary>
        [DataMember]
        public int? LayoutTemplateId { get; set; }

        /// <summary>
        /// Get/Set the page type.
        /// </summary>
        [DataMember]
        public string PageType { get; set; }
    }
}
