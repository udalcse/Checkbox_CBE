using System.Web.UI;

using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Analytics.UI.Editing
{
    /// <summary>
    /// Base class for user controls that render analysis item configuration data, including source item data, appearance
    /// details, etc.
    /// </summary>
    public class AnalysisItemConfigRendererBase : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/set item to render configuration for
        /// </summary>
        protected ReportItemInstanceData AnalysisItem { get; private set; }

        /// <summary>
        /// Initialize configuration renderer
        /// </summary>
        /// <param name="analysisItem"></param>
        public void Initialize(ReportItemInstanceData analysisItem)
        {
            AnalysisItem = analysisItem;

            InitializeConfigControl(analysisItem);
        }

        /// <summary>
        /// Initialize configuration-specific control
        /// </summary>
        protected virtual void InitializeConfigControl(ReportItemInstanceData analysisItem)
        {
            
        }
    }
}
