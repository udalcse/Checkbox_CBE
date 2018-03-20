using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

using Checkbox.Web.UI.Controls;
using Checkbox.Analytics.Items;
using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for OtherBehaviorEditor.
	/// </summary>
	[ToolboxData("<{0}:OtherBehaviorEditor runat=server></{0}:OtherBehaviorEditor>")]
	public class OtherBehaviorEditor : Checkbox.Web.UI.Controls.CompositeControl 
	{
		
		private RadioButtonList behaviorOptions;
		private string selectedValue;
		private int selectedIndex;
				

		/// <summary>
		/// 
		/// </summary>
		public string SelectedValue
		{
			get
			{
				return this.behaviorOptions.SelectedValue;
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
			behaviorOptions = new RadioButtonList();

            behaviorOptions.Items.Add(new ListItem(WebTextManager.GetText("/enum/analysisOtherBehavior/aggregate") + " - " + WebTextManager.GetText("/enum/analysisOtherBehavior/aggregateDesc"), "Aggregate"));
            behaviorOptions.Items.Add(new ListItem(WebTextManager.GetText("/enum/analysisOtherBehavior/displayAll") + " - " + WebTextManager.GetText("/enum/analysisOtherBehavior/displayAllDesc"), "DisplayAll"));
            behaviorOptions.Items.Add(new ListItem(WebTextManager.GetText("/enum/analysisOtherBehavior/aggregateAndDisplay") + " - " + WebTextManager.GetText("/enum/analysisOtherBehavior/aggregateAndDisplayDesc"), "AggregateAndDisplay"));
			
			behaviorOptions.CssClass = "PrezzaNormal";

			try
			{
				behaviorOptions.SelectedIndex = selectedIndex;
			}
			catch
			{
				
			}

			if(this.selectedValue != null && this.selectedValue != String.Empty)
				behaviorOptions.SelectedValue = this.selectedValue;

			//Add the controls to the collection
			this.Controls.Add(new LiteralControl("<table>"));
			this.Controls.Add(new LiteralControl("<tr><td><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/otherBehaviorEditor/otherBehavior") + " </span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td>"));
			this.Controls.Add(behaviorOptions);
			this.Controls.Add(new LiteralControl(@"</td></tr></table>"));
		}
	}
}
