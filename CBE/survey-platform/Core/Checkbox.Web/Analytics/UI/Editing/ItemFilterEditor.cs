using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Collections;

using Checkbox.Analytics;
using Checkbox.Analytics.Items;
using Checkbox.Forms;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for ItemFilterEditor.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ItemFilterEditor runat=server></{0}:ItemFilterEditor>")]
	public class ItemFilterEditor  : Checkbox.Web.UI.Controls.CompositeControl 
	{
		private ListBox sourceList;
		private ListBox destinationList;
		private Button addButton;
		private Button removeButton;


		private Checkbox.Forms.Form mForm;
		private AnalysisItem mItem;
	
		

		/// <summary>
		/// Initializes the item filter editor with the current item
		/// </summary>
		/// <param name="item">The item to edit filters for</param>
		public void Initialize(AnalysisItem item)
		{
			mItem = item;

			//Get the form for this item
			Checkbox.Analytics.Analysis analysis = Checkbox.Analytics.AnalysisManager.GetAnalysis(mItem.AnalysisID);
			mForm = Checkbox.Forms.FormManager.GetForm(analysis.FormID);
			analysis = null;

			EnsureChildControls();

			//Populate the source list box
			DataTable availableFilters = Checkbox.Data.Responses.Filters.FilterManager.GetAvailableFilters(mForm.ID);

			foreach (DataRow filterRow in availableFilters.Rows)
			{
				ListItem filterItem = new ListItem(filterRow["FilterName"].ToString(), filterRow["FilterID"].ToString());
				sourceList.Items.Add(filterItem);
			}
			availableFilters = null;

			//Attempt to populate the destination list box
			if (mItem != null)
			{
				foreach(Checkbox.Data.Responses.Filters.Filter filter in mItem.Filters)
				{
					ListItem listItem = new ListItem(filter.Name, filter.FilterID.ToString());
					destinationList.Items.Add(listItem);
					sourceList.Items.Remove(listItem);
				}

				foreach(Checkbox.Data.Responses.Filters.Filter filter in mItem.AnalysisFilters)
				{
					ListItem listItem = new ListItem(filter.Name, filter.FilterID.ToString());
					sourceList.Items.Remove(listItem);
				}
			}

			if(sourceList.Items.Count == 0 && destinationList.Items.Count == 0)
			{
				this.Context.Response.Redirect("EditAnalysis.aspx");
			}


		}

		/// <summary>
		/// Create editor child controls.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls ();

			sourceList = new ListBox();
			sourceList.SelectionMode = ListSelectionMode.Multiple;
			destinationList = new ListBox();
			destinationList.SelectionMode = ListSelectionMode.Multiple;
			addButton = new Button();
			removeButton = new Button();


			addButton.Text = ">>";
			addButton.CssClass = "PrezzaButton";
			removeButton.Text= "<<";
			removeButton.CssClass = "PrezzaButton";

			sourceList.Width = 350;
			sourceList.Attributes["Style"] =  "width: 350px;height: 400px";
			destinationList.Width = 350;
			destinationList.Attributes["Style"] =  "width: 350px;height: 400px";

			//Set up the button event handlers
			addButton.Click += new EventHandler(addButton_Click);
			removeButton.Click += new EventHandler(removeButton_Click);

			//Add the controls to the collection
			this.Controls.Add(new LiteralControl("<table><tr><td colspan=\"2\"><span class=\"PrezzaLabel\">Select filters to apply to this item:</span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td><span class=\"PrezzaLabel\">Available Filters</span></td><td></td><td><span class=\"PrezzaLabel\">Associated Filters</span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td valign=\"top\">"));
			this.Controls.Add(sourceList);
			this.Controls.Add(new LiteralControl("</td><td valign=\"top\">"));
			this.Controls.Add(addButton);
			this.Controls.Add(new LiteralControl("<br><br>"));
			this.Controls.Add(removeButton);
			this.Controls.Add(new LiteralControl("</td><td valign=\"top\">"));
			this.Controls.Add(destinationList);
			this.Controls.Add(new LiteralControl("</td>"));
			this.Controls.Add(new LiteralControl(@"</tr></table>"));
		}

		/// <summary>
		/// Moves a filter from the source list to the destination list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addButton_Click(object sender, EventArgs e)
		{
			EnsureChildControls();
			

			ArrayList temp = new ArrayList();
			foreach(ListItem selected in this.sourceList.Items)
			{
				if(selected.Selected)
				{
					temp.Add(selected);
				}
			}

			foreach(object o in temp)
			{
				sourceList.Items.Remove((ListItem)o);
				destinationList.Items.Add((ListItem)o);
			}


			
		}

		/// <summary>
		/// Moves an item from the destination list back to the source list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void removeButton_Click(object sender, EventArgs e)
		{
			EnsureChildControls();
			
			ArrayList temp = new ArrayList();
			foreach(ListItem selected in this.destinationList.Items)
			{
				if(selected.Selected)
				{
					temp.Add(selected);
				}
			}

			foreach(object o in temp)
			{
				destinationList.Items.Remove((ListItem)o);
				sourceList.Items.Add((ListItem)o);
			}
		}

		/// <summary>
		/// Saves the changes made to the item's filter collection
		/// </summary>
		public void CommitChanges()
		{
			//Clear the existing filters
			mItem.ClearFilters();

			//Add back the selected filters
			foreach (ListItem selectedItem in destinationList.Items)
			{
				int filterID = Convert.ToInt32(selectedItem.Value);
				Checkbox.Data.Responses.Filters.Filter filter = Checkbox.Data.Responses.Filters.FilterManager.GetFilter(filterID);

				
				mItem.AddFilter(filter);
			}
			}
		}
	}

