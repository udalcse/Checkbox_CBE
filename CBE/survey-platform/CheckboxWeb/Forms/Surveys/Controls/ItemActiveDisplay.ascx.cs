using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ItemActiveDisplay : Checkbox.Web.Common.UserControlBase
    {
        protected bool _isActive;
               
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(ItemData itemData)
        {
            _isActive = itemData == null || itemData.IsActive;
        }
    }
}