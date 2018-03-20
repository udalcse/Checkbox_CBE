using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Charts;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance Editor for rank order summary item.
    /// </summary>
    public partial class ANALYSIS_RANK_ORDER_SUMMARY_CHART : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize control and children
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (AppearanceData is RankOrderSummaryItemAppearanceData)
            {
                _graphOptions.Initialize((RankOrderSummaryItemAppearanceData)data);
                _border.Initialize((RankOrderSummaryItemAppearanceData)data);
                _text.Initialize((RankOrderSummaryItemAppearanceData)data);
                _other.Initialize((RankOrderSummaryItemAppearanceData)data);
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