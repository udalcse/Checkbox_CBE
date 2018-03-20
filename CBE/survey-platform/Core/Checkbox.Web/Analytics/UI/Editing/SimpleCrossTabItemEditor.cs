using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Checkbox.Analytics;
using Checkbox.Analytics.Items;
using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for SimpleCrossTabItemEditor.
	/// </summary>
	public class SimpleCrossTabItemEditor : AnalysisItemEditor 
	{
		/// <summary>
		/// 
		/// </summary>
		protected RendererEditor rendererEditor;
		//protected OtherBehaviorEditor otherBehaviorEditor;
		/// <summary>
		/// 
		/// </summary>
		protected UseAliasEditor useAliasEditor;
		/// <summary>
		/// 
		/// </summary>
		protected SingleSourceItemEditor independentItem;
		/// <summary>
		/// 
		/// </summary>
		protected SingleSourceItemEditor dependentItem;

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public override bool isValid
		{
			get
			{
				return (independentItem.isValid && dependentItem.isValid);
			}
		}

		#endregion

		#region Initialize

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


			if (this.Item != null)
			{
				Checkbox.Forms.Items.InputItem indepSourceItem = (Checkbox.Forms.Items.InputItem)form.InputItems.FindItem(mItem.ItemIDs[0].ToString());
				independentItem.Initialize(form, indepSourceItem);

				Checkbox.Forms.Items.InputItem depSourceItem = (Checkbox.Forms.Items.InputItem)form.InputItems.FindItem(mItem.ItemIDs[1].ToString());
				dependentItem.Initialize(form, depSourceItem);

				rendererEditor.Initialize(this.Item);
				//otherBehaviorEditor.SelectedValue = Item.GetProperty("OtherBehavior");
				useAliasEditor.SelectedValue = Item.GetProperty("UseAlias");
			}
			else
			{
				independentItem.Initialize(form);
				dependentItem.Initialize(form);

				rendererEditor.Initialize(9);//9 = simplecrosstabitem
			//	otherBehaviorEditor.SelectedIndex = 0;
				useAliasEditor.SelectedValue = ApplicationManager.AppSettings.DefaultAlias;
			}	

		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls ();

			independentItem = new SingleSourceItemEditor();
			dependentItem = new SingleSourceItemEditor();
			independentItem.IncludeCloseEnded = true;
			independentItem.IncludeOpenEnded = false;

			dependentItem.IncludeCloseEnded = true;
			dependentItem.IncludeOpenEnded = false;

			rendererEditor = new RendererEditor();

			//otherBehaviorEditor = new OtherBehaviorEditor();

			useAliasEditor = new UseAliasEditor();

			this.Controls.Add(new LiteralControl("<table border=\"0\"><tr><td><span class=\"PrezzaLabel\">Independent item:</span></td><td>"));
			this.Controls.Add(independentItem);
			this.Controls.Add(new LiteralControl("</td></tr><tr><td><span class=\"PrezzaLabel\">Dependent item:</span></td><td>"));
			this.Controls.Add(dependentItem);
			this.Controls.Add(new LiteralControl("</td></tr></table><br>"));
			this.Controls.Add(rendererEditor);
			//this.Controls.Add(new LiteralControl(@"<br>"));
			//this.Controls.Add(otherBehaviorEditor);
			this.Controls.Add(new LiteralControl(@"<br>"));
			this.Controls.Add(useAliasEditor);
		}

		/// <summary>
		/// Saves changes made to the current analysis item
		/// </summary>
		public override void CommitChanges()
		{
			
			if(independentItem.SelectedItem.Value == dependentItem.SelectedItem.Value)
			{
				this.isValid = false;
				return;
			}
			
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



			//Create the independent item
			Checkbox.Forms.Items.InputItem indepSourceItem = (Checkbox.Forms.Items.InputItem)Checkbox.Forms.Items.Item.GetItem(Convert.ToInt32(independentItem.SelectedItem.Value));
			//Add it to the AnalysisItem
			this.Item.AddSourceItem(indepSourceItem);

			//Create the dependent item
			Checkbox.Forms.Items.InputItem depSourceItem = (Checkbox.Forms.Items.InputItem)Checkbox.Forms.Items.Item.GetItem(Convert.ToInt32(dependentItem.SelectedItem.Value));
			//Add it to the AnalysisItem
			this.Item.AddSourceItem(depSourceItem);
			
			//Make sure the list is in the proper order
			if (Convert.ToInt32(this.Item.ItemIDs[0]) != Convert.ToInt32(independentItem.SelectedItem.Value))
			{
				this.Item.ItemIDs.Reverse();
			}

			//Item.SetProperty("OtherBehavior", otherBehaviorEditor.SelectedValue);
			Item.SetProperty("UseAlias", useAliasEditor.SelectedValue);

			//Set the associated renderer
			this.Item.ClearRenderer();
			this.Item.AssociateRenderer(Convert.ToInt32(rendererEditor.SelectedValue));

		

		}

		private void CreateItem()
		{
			string xPath = "//CodeDependentResources/AnalysisItemTypes/AnalysisItem[@Name=\"";
			xPath += "Simple Cross Tabulation Item";
			xPath += "\"]";
			int typeID = Convert.ToInt32(XmlResourceManager.GetCodeDependentResourceUsingXPath(xPath).Attributes["ID"].Value);

			this.mItem = Checkbox.Analytics.AnalysisManager.CreateItem(typeID, pageID, itemPosition); //4 = SummaryItem
		}
		
	}
}
