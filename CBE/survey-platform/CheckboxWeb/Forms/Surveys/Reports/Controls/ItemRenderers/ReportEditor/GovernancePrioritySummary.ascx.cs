using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor
{
     partial class GovernancePrioritySummary : UserControlAnalysisItemRendererBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void InlineInitialize()
        {
            base.InlineInitialize();
            _basicConfig.Initialize(Model);
            //_scoreConfig.Initialize(Model);
        }

        /// <summary>
        /// Bind score graph
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _governancePriorityGraph.InitializeAndBind(Model, Appearance);

        }
    }
}