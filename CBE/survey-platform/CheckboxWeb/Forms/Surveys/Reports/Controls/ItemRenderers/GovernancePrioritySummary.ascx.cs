using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class GovernancePrioritySummary : UserControlAnalysisItemRendererBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void InlineInitialize()
        {
            base.InlineInitialize();

            //_filterPanel.Width = Unit.Pixel(Utilities.AsInt(Appearance["Width"], 600));
        }

        /// <summary>
        /// Bind score graph
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _governancePriorityGraph.InitializeAndBind(Model, Appearance);

            //_filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
            //_filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
        }
    }
}