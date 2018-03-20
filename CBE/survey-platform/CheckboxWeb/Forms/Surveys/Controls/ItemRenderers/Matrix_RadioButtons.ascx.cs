using System.Collections.Generic;
using System.Linq;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Matrix_RadioButtons : UserControlSurveyItemRendererBase
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
    }
}