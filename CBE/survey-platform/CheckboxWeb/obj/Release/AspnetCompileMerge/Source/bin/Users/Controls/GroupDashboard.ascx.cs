using System.Web.UI;
using Checkbox.Web;

namespace CheckboxWeb.Users.Controls
{
    public partial class GroupDashboard : Checkbox.Web.Common.UserControlBase
    {
		/// <summary>
		/// Callback for group member selected
		/// </summary>
		public string GroupMemebrSelectedClientCallback { get; set; }

		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);
		}
    }
}