using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class Analysis_Total_Score : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (data is AnalysisItemAppearanceData)
            {
                _graphTypeList.SelectedValue = data["GraphType"];
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (AppearanceData is AnalysisItemAppearanceData)
            {
                 AppearanceData["GraphType"] = _graphTypeList.SelectedValue;
            }
        }
    }
}