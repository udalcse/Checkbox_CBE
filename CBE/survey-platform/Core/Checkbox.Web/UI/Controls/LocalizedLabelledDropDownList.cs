using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Localized dropdown list
    /// </summary>
    public class LocalizedLabelledDropDownList : LocalizedLabelledControl
    {
        private DropDownList _theList;

        /// <summary>
        /// Get/set the input control
        /// </summary>
        public override Control InputControl
        {
            get { return List; }
            set{}
        }

        /// <summary>
        /// Get the dropdown list
        /// </summary>
        protected DropDownList List
        {
            get 
            {
                EnsureChildControls();
                return _theList; 
            }
        }

        /// <summary>
        /// Get/set an identifier for the list
        /// </summary>
        public string ListID
        {
            get { return List.ID; ; }
            set { List.ID = value; }
        }

        /// <summary>
        /// Get/set the list css class
        /// </summary>
        public string ListCssClass
        {
            get { return List.CssClass; }
            set { List.CssClass = value; }
        }

        /// <summary>
        /// Get/set the list skin id
        /// </summary>
        public string ListSkinID
        {
            get { return List.SkinID; }
            set { List.SkinID = value; }
        }

        /// <summary>
        /// Get/set the selected value
        /// </summary>
        public string SelectedValue
        {
            get { return List.SelectedValue; }
            set { SetSelectedValue(value); }
        }

        /// <summary>
        /// Get a boolean indicating if the list contains the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsValue(string value)
        {
            return List.Items.FindByValue(value) != null;
        }

        /// <summary>
        /// Get/set the selected text
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (List.SelectedItem != null)
                {
                    return List.SelectedItem.Text;
                }
                return null;
            }
            set { SetSelectedText(value); }
        }

        /// <summary>
        /// Set the selected value
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetSelectedValue(string value)
        {
           if(List.Items.FindByValue(value) != null)
           {
               List.SelectedValue = value;
           }
        }

        /// <summary>
        /// Set the selected value
        /// </summary>
        /// <param name="text"></param>
        protected virtual void SetSelectedText(string text)
        {
            ListItem item = List.Items.FindByText(text);

            if(item != null)
            {
                List.ClearSelection();
                item.Selected = true;
            }
        }

        /// <summary>
        /// Set autopostback properly
        /// </summary>
        public virtual bool AutoPostBack
        {
            get { return List.AutoPostBack; }
            set { List.AutoPostBack = true; }
        }

        /// <summary>
        /// Bind the selected index changed event
        /// </summary>
        /// <param name="indexChangedDelegate"></param>
        public void BindSelectedIndexChangedEvent(EventHandler indexChangedDelegate)
        {
            List.SelectedIndexChanged += indexChangedDelegate;
        }

        /// <summary>
        /// Get the list item count
        /// </summary>
        public int ItemCount
        {
            get { return List.Items.Count; }
        }

        /// <summary>
        /// Get the input control
        /// </summary>
        /// <returns></returns>
        protected virtual DropDownList GetList()
        {
            return new DropDownList();
        }

        /// <summary>
        /// Get custom list items
        /// </summary>
        protected virtual List<ListItem> GetListItems() { return new List<ListItem>();}

        /// <summary>
        /// Override create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            //Add items to the list
            _theList = new DropDownList();

            base.CreateChildControls();
        }

        /// <summary>
        /// Load items
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Add list items that come from child class
            AddListItems();

            base.OnInit(e);
        }

        /// <summary>
        /// Add items to the list
        /// </summary>
        protected void AddListItems()
        {
            List<ListItem> listItems = GetListItems();

            if (listItems != null && listItems.Count > 0)
            {
                //Clear the list
                ClearItems();

                //Add the custom items
                foreach (ListItem listItem in listItems)
                {
                    List.Items.Add(listItem);
                }
            }
        }

        /// <summary>
        /// Clear the list of items
        /// </summary>
        public virtual void ClearItems()
        {
            List.Items.Clear();
        }

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        /// <param name="itemValue"></param>
        public virtual void RemoveItem(string itemValue)
        {
            if (List.Items.FindByValue(itemValue) != null)
            {
                List.Items.Remove(List.Items.FindByValue(itemValue));
            }
        }

        /// <summary>
        /// Add an item to the list if an item with the same value doesn't already
        /// exist.
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(ListItem item)
        {
            if (List.Items.FindByValue(item.Value) == null)
            {
                List.Items.Add(item);
            }
        }

        /// <summary>
        /// Add list items
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddItems(List<ListItem> items)
        {
            List.Items.AddRange(items.ToArray());
        }
    }
}
