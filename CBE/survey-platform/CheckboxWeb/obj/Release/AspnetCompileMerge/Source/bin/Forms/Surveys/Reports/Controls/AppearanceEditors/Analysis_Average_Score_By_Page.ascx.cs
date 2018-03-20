using System;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class Analysis_Average_Score_By_Page : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize control and children
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (AppearanceData is AverageScoreByPageItemAppearanceData)
            {
                _margins.Initialize((AverageScoreByPageItemAppearanceData)data);
                _border.Initialize((AverageScoreByPageItemAppearanceData)data);
                _legend.Initialize((AverageScoreByPageItemAppearanceData)data);
                _other.Initialize((AverageScoreByPageItemAppearanceData)data);
                _options.Initialize((AverageScoreByPageItemAppearanceData)data);
                _text.Initialize((AverageScoreByPageItemAppearanceData)data);
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