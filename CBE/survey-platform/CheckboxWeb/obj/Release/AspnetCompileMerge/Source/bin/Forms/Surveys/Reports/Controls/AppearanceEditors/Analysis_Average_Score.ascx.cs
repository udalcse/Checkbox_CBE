using System;
using Checkbox.Analytics.Items.UI;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class Analysis_Average_Score : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Initialize control and children
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            if (AppearanceData is AverageScoreItemAppearanceData)
            {
                _margins.Initialize((AverageScoreItemAppearanceData)data);
                _border.Initialize((AverageScoreItemAppearanceData)data);
                _legend.Initialize((AverageScoreItemAppearanceData)data);
                _other.Initialize((AverageScoreItemAppearanceData)data);
                _options.Initialize((AverageScoreItemAppearanceData)data);
                _text.Initialize((AverageScoreItemAppearanceData)data);
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