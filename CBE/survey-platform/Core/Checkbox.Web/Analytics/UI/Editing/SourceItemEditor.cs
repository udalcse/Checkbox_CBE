using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

using Checkbox.Analytics.Items;
using Checkbox.Web.UI.Controls;
using Checkbox.Management;
using Checkbox.Forms.Items;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for SourceItemEditor.
	/// </summary>
	[ToolboxData("<{0}:SourceItemEditor runat=server></{0}:SourceItemEditor>")]
	public class SourceItemEditor : Checkbox.Web.UI.Controls.CompositeControl 
	{
		
		private ListBox sourceList;
		private ListBox destinationList;
		private Button addButton;
		private Button removeButton;
		private bool includeOpenEnded;
		private bool includeCloseEnded;
		private bool includeMatrixOptions;
		private bool includeMatrixOpenEnded;
		private int itemCount;

		//if there are no source items, entire editor is invalid
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool isValid()
		{
			if(itemCount == 0)
				return false;
			else
				return true;

		}

		/// <summary>
		/// 
		/// </summary>
		public int ItemCount
		{
			get
			{
				return itemCount;
			}

		}

		/// <summary>
		/// 
		/// </summary>
		public bool IncludeOpenEnded
		{
			get
			{
				return includeOpenEnded;
			}
			set
			{
				includeOpenEnded = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IncludeCloseEnded
		{
			get
			{
				return includeCloseEnded;
			}
			set
			{
				includeCloseEnded = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IncludeMatrixOpenEnded
		{
			get
			{
				return includeMatrixOpenEnded;
			}
			set
			{
				includeMatrixOpenEnded = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IncludeMatrixOptions
		{
			get
			{
				return includeMatrixOptions;
				}
			set
			{
				includeMatrixOptions = value;
			}
		}
				

		/// <summary>
		/// 
		/// </summary>
		public ListItemCollection SelectedItems
		{
			get
			{
				return this.destinationList.Items;
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

			if (this.sourceList.Items.Count == 0)
			{
				//Populate the source list box
				foreach (Checkbox.Forms.Items.InputItem sourceItem in form.InputItems)
				{
					if(((this.includeOpenEnded == true && (sourceItem.ItemType == Checkbox.Forms.ItemType.Date || sourceItem.ItemType == Checkbox.Forms.ItemType.MultilineTextBox || sourceItem.ItemType == Checkbox.Forms.ItemType.SinglelineTextBox))) 
						|| ((this.includeCloseEnded == true && (sourceItem.ItemType != Checkbox.Forms.ItemType.Date && sourceItem.ItemType != Checkbox.Forms.ItemType.MultilineTextBox && sourceItem.ItemType != Checkbox.Forms.ItemType.SinglelineTextBox && sourceItem.ItemType != Checkbox.Forms.ItemType.Matrix)))  
						|| ((this.includeMatrixOptions == true && (sourceItem.ItemType == Checkbox.Forms.ItemType.Matrix))) 
						|| ((this.includeMatrixOpenEnded == true && (sourceItem.ItemType == Checkbox.Forms.ItemType.Matrix)))
						)
					{
						bool addItem = false;
						if(sourceItem.ItemType == Checkbox.Forms.ItemType.Matrix)
						{
							foreach(Checkbox.Forms.Items.MatrixSet ms in ((Checkbox.Forms.Items.SurveyItem)sourceItem).MatrixSets)
							{
								if(ms.Type == Checkbox.Forms.Items.MatrixSetType.SingleLineInputs)
								{
									if(this.includeMatrixOpenEnded == true)
									{
										addItem = true;
										break;
									}
								}
								else if(ms.Type != Checkbox.Forms.Items.MatrixSetType.SingleLineInputs)
								{
									if(this.includeMatrixOptions == true)
									{
										addItem = true;
										break;
									}
								}
							}
						}
						else
						{
							addItem = true;
						}
						
						if(addItem == true)
						{

							string itemPrefix;
							if(((Checkbox.Forms.Page)sourceItem.ItemContainer).IsHidden == true)
								itemPrefix = "0." + sourceItem.Position.ToString();
							else
								itemPrefix = ((Checkbox.Forms.Page)sourceItem.ItemContainer).Position + "." + sourceItem.Position.ToString();

							ListItem listItem = new ListItem(itemPrefix + " " + sourceItem.Text, sourceItem.ID.ToString());
							sourceList.Items.Add(listItem);
						}
					}
				}

				//Attempt to populate the destination list box
				if (item != null)
				{
					foreach(int sourceItemID in item.ItemIDs)
					{
						Checkbox.Forms.Items.InputItem tempItem = (Checkbox.Forms.Items.InputItem)Checkbox.Forms.Items.Item.GetItem(sourceItemID);
						ListItem listItem = new ListItem(tempItem.Text, tempItem.ID.ToString());
						destinationList.Items.Add(listItem);
						sourceList.Items.Remove(listItem);
						tempItem = null;
					}
				}
			}
			itemCount = this.sourceList.Items.Count;
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void CreateChildControls()
		{
			sourceList = new ListBox();
			destinationList = new ListBox();
			addButton = new Button();
			removeButton = new Button();

            addButton.Text = WebTextManager.GetText("/controlText/sourceItemEditor/addBtn");
			addButton.CssClass = "PrezzaButton";
            removeButton.Text = WebTextManager.GetText("/controlText/sourceItemEditor/removeBtn");
			removeButton.CssClass = "PrezzaButton";

			sourceList.Width = 250;
			sourceList.Attributes["Style"] =  "width: 250px";
			sourceList.Height = 250;
			sourceList.SelectionMode = ListSelectionMode.Multiple;
			destinationList.Width = 250;
			destinationList.Attributes["Style"] =  "width: 250px";
			destinationList.Height = 250;
			destinationList.SelectionMode = ListSelectionMode.Multiple;

			//Add event handlers
			addButton.Click += new EventHandler(addButton_Click);
			removeButton.Click += new EventHandler(removeButton_Click);

			//Add the controls to the collection
			this.Controls.Add(new LiteralControl("<table><tr><td colspan=\"2\"><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/sourceItemEditor/selecItems") + "</span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/sourceItemEditor/availableItems") + "</span></td><td></td><td><span class=\"PrezzaLabel\">" + WebTextManager.GetText("/controlText/sourceItemEditor/associatedItems") + "</span></td></tr>"));
			this.Controls.Add(new LiteralControl("<tr><td valign=\"top\" rowspan=\"2\">"));
			this.Controls.Add(sourceList);
			this.Controls.Add(new LiteralControl("</td><td valign=\"top\">"));
			this.Controls.Add(addButton);
			this.Controls.Add(new LiteralControl("<br>"));
			this.Controls.Add(new LiteralControl("<br>"));
			this.Controls.Add(removeButton);
			this.Controls.Add(new LiteralControl("</td><td rowspan=\"2\" valign=\"top\">"));
			this.Controls.Add(destinationList);
			this.Controls.Add(new LiteralControl(@"</td></tr><tr><td>"));
			this.Controls.Add(new LiteralControl(@"</td></tr></table>"));
		}

		//Copy an item from the source box to the destination box
		private void addButton_Click(object sender, EventArgs e)
		{
			EnsureChildControls();
			ArrayList temp = new ArrayList();
			foreach(ListItem s in this.sourceList.Items)
			{
				if (s.Selected)
				{
					temp.Add(s);
				}
			}

			foreach(object o in temp)
			{
				this.destinationList.Items.Add((ListItem)o);
				this.sourceList.Items.Remove((ListItem)o);
			}
		}

		/// <summary>
		/// Removes an item from the list of souceitems associated with this analaysisitem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void removeButton_Click(object sender, EventArgs e)
		{
			
			EnsureChildControls();
			ArrayList temp = new ArrayList();
			foreach(ListItem s in this.destinationList.Items)
			{
				if (s.Selected)
				{
					temp.Add(s);
				}
			}

			foreach(object o in temp)
			{
				this.destinationList.Items.Remove((ListItem)o);
				this.sourceList.Items.Add((ListItem)o);
			}
		}
	}
}
