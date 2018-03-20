using System.Web.UI;

namespace CheckboxWeb.Users.Controls
{
    public partial class EmailListList : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Callback for invitation selected
        /// </summary>
        public string EmailListSelectedClientCallback { get; set; }
    }
}