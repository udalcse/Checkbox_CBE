using Checkbox.Web.Analytics.UI.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class HeatMapSummary : UserControlAnalysisItemRendererBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void InlineInitialize()
        {
            base.InlineInitialize();
        }

        /// <summary>
        /// Binds heat map chart to the model
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _heatMapGraph.InitializeAndBind(Model, Appearance);
        }
    }
}