using System;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace Checkbox.Web.Analytics.UI.Rendering
{
    /// <summary>
    /// Base renderer for analysis items.
    /// </summary>
    public class UserControlAnalysisItemRendererBase : UserControlItemRendererBase
    {
        /// <summary>
        /// Get survey item model
        /// </summary>
        public ReportItemInstanceData Model
        {
            get
            {
                if (!(DataTransferObject is ReportItemInstanceData))
                {
                    throw new Exception("Model for analysis item renderer must be of or extend type: Checkbox.Wcf.Proxies.ReportItemInstanceData.");
                }
                 
                return (ReportItemInstanceData)DataTransferObject;   
            }
        }
    }
}
