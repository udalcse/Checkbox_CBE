using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor
{
    public partial class HeatMapSummary : UserControlAnalysisItemRendererBase
    {
        protected override void InlineInitialize()
        {
            base.InlineInitialize();
            _basicConfig.Initialize(Model);
        }

        /// <summary>
        /// Bind score graph
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _heatMapGraph.InitializeAndBind(Model, Appearance);

        }
    }
}