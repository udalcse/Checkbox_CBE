using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class GradientColorDirectorSkillsMatrixOptions : UserControlBase
    {
        /// <summary>
        /// Initialize options
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(GradientColorDirectorSkillsMatrixGraphData itemData)
        {
            _aliasesChk.Checked = itemData.UseAliases;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(GradientColorDirectorSkillsMatrixGraphData itemData)
        {
            itemData.UseAliases = _aliasesChk.Checked;
        }
    }
}