using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Matrix_CheckBoxes : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetAllOptions()
        {
            foreach (var option in Model.Options)
            {
                option.Text = GetOptionText(option);
            }

            return Model.Options;
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetNonOtherOptions()
        {
            foreach (var option in Model.Options)
            {
                option.Text = GetOptionText(option);
            }

            return Model.Options.Where(a => !a.IsOther);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionsSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }
    }
}