using System;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Forms.Items.UI;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
	public partial class Analysis_Details : UserControlAppearanceEditorBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			RegisterClientScriptInclude(
				"jquery.numeric.js",
				ResolveUrl("~/Resources/jquery.numeric.js"));
		}


		/// <summary>
		/// Initialize the control
		/// </summary>
		/// <param name="data"></param>
		public override void Initialize(AppearanceData data)
		{
			base.Initialize(data);

			_widthTxt.Text = data["Width"] ?? string.Empty;
			_heightTxt.Text = data["Height"] ?? string.Empty;

		}

		/// <summary>
		/// Update data
		/// </summary>
		public override void UpdateData()
		{
			base.UpdateData();

			AppearanceData["Width"] = _widthTxt.Text;
			AppearanceData["Height"] = _heightTxt.Text;
		}

	}
}