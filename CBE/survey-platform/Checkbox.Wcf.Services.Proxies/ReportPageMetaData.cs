using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class ReportPageMetaData : PageMetaData
    {
        /// <summary>
        /// 
        /// </summary>
        public int? LayoutTemplateId { get; set; }
    }
}
