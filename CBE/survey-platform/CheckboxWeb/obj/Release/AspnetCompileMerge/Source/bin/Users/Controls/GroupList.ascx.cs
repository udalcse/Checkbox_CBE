using System.Web.UI;

namespace CheckboxWeb.Users.Controls
{
    public partial class GroupList : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Callback for group selected
        /// </summary>
        public string GroupSelectedClientCallback { get; set; }
    }
}