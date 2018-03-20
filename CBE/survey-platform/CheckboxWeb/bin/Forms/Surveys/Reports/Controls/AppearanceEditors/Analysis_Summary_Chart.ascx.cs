using System;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Charts;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for summary charts
    /// </summary>
    public partial class Analysis_Summary_Chart : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize control and children
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (AppearanceData is SummaryChartItemAppearanceData)
            {
                _graphOptions.Initialize((SummaryChartItemAppearanceData)data);
                _border.Initialize((SummaryChartItemAppearanceData)data);
                _text.Initialize((SummaryChartItemAppearanceData)data);
                _margins.Initialize((SummaryChartItemAppearanceData)data);
                _other.Initialize((SummaryChartItemAppearanceData)data);
                _legend.Initialize((SummaryChartItemAppearanceData)data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }
    }
}