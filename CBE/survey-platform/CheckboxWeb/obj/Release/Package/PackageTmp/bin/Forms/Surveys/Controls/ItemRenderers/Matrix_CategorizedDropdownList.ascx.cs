using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Matrix_CategorizedDropdownList : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetOptions()
        {
            var result = new List<SurveyResponseItemOption>(Model.Options);

            var defaultText = new SurveyResponseItemOption
            {
                Text = WebTextManager.GetText("/common/dropDownDefault")
            };

            if (string.IsNullOrEmpty(defaultText.Text))
                defaultText.Text = " ";

            defaultText.IsSelected = true;
            defaultText.IsDefault = true;

            result.Insert(0, defaultText);

            return result;
        }

        /// <summary>
        /// Got 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionsSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }
    }
}