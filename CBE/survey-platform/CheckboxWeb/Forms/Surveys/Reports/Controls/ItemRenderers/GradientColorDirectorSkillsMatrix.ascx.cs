using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers
{
    public partial class GradientColorDirectorSkillsMatrix : UserControlAnalysisItemRendererBase
    {
        /// <summary>
        /// Bind score graph
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _gradientColorDirectorSkillsMatrixChart.InitializeAndBind(Model, Appearance);

            //_filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
            //_filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
        }
    }
}