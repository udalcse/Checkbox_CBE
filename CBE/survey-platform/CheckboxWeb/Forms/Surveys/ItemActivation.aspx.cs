using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Forms.Items.Configuration;


namespace CheckboxWeb.Forms.Surveys
{
	/// <summary>
	/// Move/Copy survey page
	/// </summary>
	public partial class ItemActivation : SecuredPage
	{
		[QueryParameter("i", IsRequired = true)]
		public int? ItemId { get; set; }


        [QueryParameter("s", IsRequired = true)]
        public int? ResponeseTemplateId { get; set; }

		/// <summary>
		/// Bind events
		/// </summary>
		protected override void OnPageInit()
		{
			base.OnPageInit();

            Master.OkClick += _okButton_Click;

			if (!IsPostBack)
			{
				ItemData item = ItemConfigurationManager.GetConfigurationData(ItemId.Value);

				_active.Checked = item.IsActive;
				_deactive.Checked = !item.IsActive;
			}

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/ItemActivation.aspx/title"));
		}


		/// <summary>
		/// Handle ok button click.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _okButton_Click(object sender, System.EventArgs e)
		{
			if (!ItemId.HasValue)
			{
				CloseWindow();

				return;
			}

			ItemData item = ItemConfigurationManager.GetConfigurationData(ItemId.Value);

			item.IsActive = _active.Checked;

			item.Save();

            //Mark survey updated
            ResponseTemplateManager.MarkTemplateUpdated(ResponeseTemplateId.Value);

            CloseWindow();
		}

        /// <summary>
        /// Close window and refresh item dashboard.
        /// </summary>
		void CloseWindow()
		{
            Master.CloseDialog("{op: 'activateItem', result: 'ok', itemId: " + ItemId + "}", true);            
		}
	}
}
