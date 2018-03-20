using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

using Checkbox.Analytics.Items;
using Checkbox.Web.UI.Controls;
using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for UseAliasEditor.
	/// </summary>
	[ToolboxData("<{0}:UseAliasEditor runat=server></{0}:UseAliasEditor>")]
	public class UseAliasEditor : Checkbox.Web.UI.Controls.CompositeControl 
	{
		
		private RadioButtonList aliasOptions;
		private string selectedValue;
		private int selectedIndex;
				

		/// <summary>
		/// 
		/// </summary>
		public string SelectedValue
		{
			get
			{
				return this.aliasOptions.SelectedValue;
			}
			set
			{
				selectedValue = value;
			}	
		}

		/// <summary>
		/// 
		/// </summary>
		public int SelectedIndex
		{
			set
			{
				selectedIndex = value;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		public void Initialize(Checkbox.Forms.Form form)
		{
			this.Initialize(form, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		/// <param name="item"></param>
		public void Initialize(Checkbox.Forms.Form form, AnalysisItem item)
		{
			EnsureChildControls();

		}

		/// <summary>
		/// 
		/// </summary>
		protected override void CreateChildControls()
		{
			aliasOptions = new RadioButtonList();

            aliasOptions.Items.Add(new ListItem(WebTextManager.GetText("/common/no"), "No"));
            aliasOptions.Items.Add(new ListItem(WebTextManager.GetText("/common/yes"), "Yes"));
			
			aliasOptions.CssClass = "PrezzaNormal";

			try
			{
				aliasOptions.SelectedIndex = selectedIndex;
			}
			catch
			{
				
			}

			if(this.selectedValue != null && this.selectedValue != String.Empty)
				aliasOptions.SelectedValue = this.selectedValue;
	
			//Add the controls to the collection
			this.Controls.Add(new LiteralControl("<table>"));
			this.Controls.Add(new LiteralControl("<tr><td><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/useAliasEditor/useAliases") + "</span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td>"));
			this.Controls.Add(aliasOptions);
			this.Controls.Add(new LiteralControl(@"</td></tr></table>"));
		}
	}
}
