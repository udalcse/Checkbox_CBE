using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Matrix_DropdownList : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetOptions()
        {
            var result = new List<SurveyResponseItemOption>(Model.Options);

            foreach (SurveyResponseItemOption o in result)
                o.Text = Checkbox.Common.Utilities.SimpleHtmlDecode(o.Text);

            var defaultText = new SurveyResponseItemOption
            {
                Text = WebTextManager.GetText("/common/dropDownDefault", Model.LanguageCode)
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