using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Checkbox.Management;
using Checkbox.Analytics;
using Checkbox.Analytics.Items;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for SummaryItemEditor.
	/// </summary>
	[DefaultProperty("Text"),
		ToolboxData("<{0}:SummaryItemEditor runat=server></{0}:SummaryItemEditor>")]
	public class SummaryItemEditor : AnalysisItemEditor 
	{
		/// <summary>
		/// 
		/// </summary>
		protected SourceItemEditor sourceEditor;
		/// <summary>
		/// 
		/// </summary>
		protected RendererEditor rendererEditor;
		/// <summary>
		/// 
		/// </summary>
		protected OtherBehaviorEditor otherBehaviorEditor;
		/// <summary>
		/// 
		/// </summary>
		protected UseAliasEditor useAliasEditor;

		/// <summary>
		/// 
		/// </summary>
		public override bool isValid
		{
			get
			{
				return sourceEditor.isValid();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public override void Initialize(AnalysisItem item)
		{
			this.mItem = item;
			this.Initialize(item.AnalysisID);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="analysisID"></param>
		public override void Initialize(int analysisID)
		{
			base.Initialize (analysisID);

			//Get the form associated with this analysis
			Checkbox.Analytics.Analysis analysis = Checkbox.Analytics.AnalysisManager.GetAnalysis(this.mAnalysisID);
			Checkbox.Forms.Form form = Checkbox.Forms.FormManager.GetForm(analysis.FormID);
			analysis = null;

			EnsureChildControls();

			//Initalize the child controls
			sourceEditor.Initialize(form, this.Item);

			if (this.Item != null)
			{
				rendererEditor.Initialize(this.Item);
				otherBehaviorEditor.SelectedValue = Item.GetProperty("OtherBehavior");
				useAliasEditor.SelectedValue = Item.GetProperty("UseAlias");
			}
			else
			{
				rendererEditor.Initialize(4);//4 = summaryitem type id
				otherBehaviorEditor.SelectedIndex = 0;
				useAliasEditor.SelectedValue = ApplicationManager.AppSettings.DefaultAlias;
			}	

		}

		/// <summary>
		/// 
		/// </summary>
		public override void CommitChanges()
		{
			
			Checkbox.Analytics.Analysis analysis = Checkbox.Analytics.AnalysisManager.GetAnalysis(this.mAnalysisID);
			
			if (this.Item == null)
			{
				CreateItem();
			}
			else
			{
				analysis.RemoveItem(this.Item.ID, false);
			}
			
			//Set the associated items
			this.Item.ClearSourceItems();
			foreach (ListItem item in sourceEditor.SelectedItems)
			{
				//Create the source item
				Checkbox.Forms.Items.InputItem sourceItem = (Checkbox.Forms.Items.InputItem)Checkbox.Forms.Items.Item.GetItem(Convert.ToInt32(item.Value));
				//Add it to the AnalysisItem
				this.Item.AddSourceItem(sourceItem);
			}

			Item.SetProperty("OtherBehavior", otherBehaviorEditor.SelectedValue);
			Item.SetProperty("UseAlias", useAliasEditor.SelectedValue);

			//Set the associated renderer
			this.Item.ClearRenderer();
			this.Item.AssociateRenderer(Convert.ToInt32(rendererEditor.SelectedValue));


		}

		/// <summary>
		/// 
		/// </summary>
		private void CreateItem()
		{
			this.mItem = Checkbox.Analytics.AnalysisManager.CreateItem(4, pageID, itemPosition); //4 = SummaryItem
		}


		/// <summary>
		/// 
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls ();

			sourceEditor = new SourceItemEditor();
			sourceEditor.IncludeCloseEnded = true;
			sourceEditor.IncludeOpenEnded = true;

			rendererEditor = new RendererEditor();

			otherBehaviorEditor = new OtherBehaviorEditor();

			useAliasEditor = new UseAliasEditor();

			this.Controls.Add(sourceEditor);
			this.Controls.Add(new LiteralControl("<HR width=\"100%\" SIZE=\"1\">"));
			this.Controls.Add(rendererEditor);
			this.Controls.Add(new LiteralControl("<HR width=\"100%\" SIZE=\"1\">"));
			this.Controls.Add(otherBehaviorEditor);
			this.Controls.Add(new LiteralControl("<HR width=\"100%\" SIZE=\"1\">"));
			this.Controls.Add(useAliasEditor);
			this.Controls.Add(new LiteralControl("<HR width=\"100%\" SIZE=\"1\">"));
		}

		
	}
}
