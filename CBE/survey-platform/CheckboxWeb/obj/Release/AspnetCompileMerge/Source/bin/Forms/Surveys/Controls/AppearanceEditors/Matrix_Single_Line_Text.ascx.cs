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
    public partial class Matrix_Single_Line_Text : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _widthTxt.Text = data["Width"] ?? string.Empty;
        }

        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            AppearanceData["Width"] = _widthTxt.Text;
        }
    }
}