using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors
{
    /// <summary>
    /// Appearance editor for Slider items
    /// </summary>
    public partial class MatrixSlider : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// Inititalize appearance editor
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);
            _showLabelCkbx.Checked = 
                data["ShowValue"] != null 
                && data["ShowValue"].Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Update data with control inputs.
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["ShowValue"] = _showLabelCkbx.Checked.ToString();
        }
    }
}