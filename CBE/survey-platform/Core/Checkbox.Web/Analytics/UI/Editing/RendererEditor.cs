using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;

using Checkbox.Analytics;
using Checkbox.Analytics.Items;
using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for RendererEditor.
	/// </summary>
	[ToolboxData("<{0}:RendererEditor runat=server></{0}:RendererEditor>")]
	public class RendererEditor : RadioButtonList//, AnalysisItemEditor 
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Initialize(AnalysisItem item)
		{
			//Get the type of this item
			DataTable itemData = Checkbox.Analytics.AnalysisManager.GetAnalysisItemRegistration(item.ID);
			int typeID = Convert.ToInt32(itemData.Rows[0]["TypeID"]);
			itemData.Dispose();
			
			//Populate the list of available renderers
			this.Initialize(typeID);
			
			//Select the appropriate item in the list
			this.SelectedIndex = this.Items.IndexOf(this.Items.FindByValue(item.RendererID.ToString()));

			this.CssClass = "prezzanormal";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemTypeID"></param>
		public void Initialize(int itemTypeID)
		{
			//Get the list of supported renderers for this item type
			DataTable renderers = Checkbox.Analytics.AnalysisManager.GetSupportedRenderers(itemTypeID);

			if (this.Items.Count != renderers.Rows.Count)
			{
				this.Items.Clear();
				
				//Add each type to the items collection
				foreach (DataRow row in renderers.Rows)
				{
					ListItem newItem = new ListItem(row["RendererName"].ToString(), row["AnalysisRendererID"].ToString());
					this.Items.Add(newItem);
				}
			}		
			this.SelectedIndex = 0;
			this.CssClass = "prezzanormal";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("<table><tr><td><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/rendererEditor/selectRenderer") +  " </span></td></tr><tr><td>");
			base.Render (writer);
			writer.Write(@"</td></tr></table>");
		}
	}
}
