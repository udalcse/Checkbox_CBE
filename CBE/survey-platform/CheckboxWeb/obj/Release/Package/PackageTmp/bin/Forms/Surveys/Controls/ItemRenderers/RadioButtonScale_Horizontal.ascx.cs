using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Item renderer for horizontal radio button scale
    /// </summary>
    public partial class RadioButtonScale_Horizontal : RadioButtonScaleBase
    {
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