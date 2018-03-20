using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Editing
{
	/// <summary>
	/// Summary description for SingleSourceItemEditor.
	/// </summary>
	public class SingleSourceItemEditor : Checkbox.Web.UI.Controls.CompositeControl 
	{

		private bool includeOpenEnded = true;
		private bool includeCloseEnded = true;
		private bool includeMatrix = false;
		private int mItemCount = 0;
		private DropDownList mItems;

		#region Properties
	
		//if there are no source items, entire editor is invalid
		/// <summary>
		/// 
		/// </summary>
		public bool isValid
		{
			get
			{
				 return !(this.mItemCount == 0);					
			}

		}

		/// <summary>
		/// 
		/// </summary>
		public int ItemCount
		{
			get
			{
				return mItemCount;
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
		public bool IncludeMatrix
		{
			get
			{
				return includeMatrix;
			}
			set
			{
				includeMatrix = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ListItem SelectedItem
		{
			get
			{
				return mItems.SelectedItem;
			}
		}
		#endregion

		#region Methods

		/// <summary>
		/// Initializes the source item list with items from the current form
		/// </summary>
		/// <param name="form">The form to list items for</param>
		public void Initialize(Checkbox.Forms.Form form)
		{
			this.Initialize(form, null);
		}

		/// <summary>
		/// Initializes the source item list with items from the current form
		/// and selects the item passed in
		/// </summary>
		/// <param name="form">The form to list items for</param>
		/// <param name="item">The item to select</param>
		public void Initialize(Checkbox.Forms.Form form, Checkbox.Forms.Items.Item item)
		{
			this.EnsureChildControls();

			//Populate the drop down list
			if (mItems.Items.Count == 0)
			{
				foreach (Checkbox.Forms.Items.InputItem sourceItem in form.InputItems)
				{
					if(((this.includeOpenEnded && (sourceItem.ItemType == Checkbox.Forms.ItemType.Date || sourceItem.ItemType == Checkbox.Forms.ItemType.MultilineTextBox || sourceItem.ItemType == Checkbox.Forms.ItemType.SinglelineTextBox))) 
						|| ((this.includeCloseEnded && (sourceItem.ItemType != Checkbox.Forms.ItemType.Date && sourceItem.ItemType != Checkbox.Forms.ItemType.MultilineTextBox && sourceItem.ItemType != Checkbox.Forms.ItemType.SinglelineTextBox && sourceItem.ItemType != Checkbox.Forms.ItemType.Matrix)))  
						|| ((this.includeMatrix && (sourceItem.ItemType == Checkbox.Forms.ItemType.Matrix))) )
					{
						string itemPrefix;
						if(((Checkbox.Forms.Page)sourceItem.ItemContainer).IsHidden == true)
							itemPrefix = "0." + sourceItem.Position.ToString();
						else
							itemPrefix = ((Checkbox.Forms.Page)sourceItem.ItemContainer).Position + "." + sourceItem.Position.ToString();

						ListItem listItem = new ListItem(itemPrefix + " " + sourceItem.Text, sourceItem.ID.ToString());
						mItems.Items.Add(listItem);
						mItemCount++;
					}
				}
	
				//Select the appropriate item if one was passed in
				if (item != null)
				{
					mItems.Items.FindByValue(item.ID.ToString()).Selected = true;
				}
			}
		}

		/// <summary>
		/// Creates the child controls and adds them to this controls control collection
		/// </summary>
		protected override void CreateChildControls()
		{
			this.mItems = new DropDownList();
			this.mItems.Width = Unit.Pixel(300);
			this.mItems.Visible = true;

			this.Controls.Add(mItems);
		}
		
		#endregion
	}
}
