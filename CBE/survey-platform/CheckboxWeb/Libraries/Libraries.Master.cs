using System;
using System.Text;

using Telerik.Web.UI;

using Checkbox.Web.Page;

namespace CheckboxWeb.Libraries
{
	public partial class Libraries : BaseMasterPage
    {
		/// <summary>
		/// Register client scripts
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			//Load the path to jQuery
			StringBuilder jQueryTag = new StringBuilder();
			jQueryTag.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jQueryTag.Append(ResolveUrl("~/Resources/jquery-1.5.min.js\"></script>"));
			jQueryTag.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
			jQueryTag.Append(ResolveUrl("~/Resources/DialogHandler.js\"></script>"));
			_jQuery.Text = jQueryTag.ToString();

		}

		public string Title
		{
			get { return Page.Title; }
			set { Page.Title = value; }
		}
	}
}
