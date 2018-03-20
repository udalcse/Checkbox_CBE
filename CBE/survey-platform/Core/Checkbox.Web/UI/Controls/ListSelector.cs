using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class ListSelectorEventArgs : EventArgs
    {
        ///<summary>
        ///</summary>
        ///<param name="listItems"></param>
        public ListSelectorEventArgs(List<ListItem> listItems)
        {
            Items = listItems;
        }

        ///<summary>
        ///</summary>
        public List<ListItem> Items { get; private set; }
    }

    ///<summary>
    ///</summary>
    ///<param name="sender"></param>
    ///<param name="args"></param>
    public delegate void ListSelectionChanged(object sender, ListSelectorEventArgs args);

    /// <summary>
    /// List selector item
    /// </summary>
    public class ListSelector : Common.WebControlBase
    {
        public ListSelector()
        {
            Height = Unit.Pixel(200);
            ListBoxWidth = Unit.Pixel(150);
            ButtonWidth = Unit.Pixel(50);
        }
    
        /// <summary>
        /// 
        /// </summary>
        public event ListSelectionChanged ItemsSelected;

        /// <summary>
        /// 
        /// </summary>
        public event ListSelectionChanged ItemsUnSelected;

        private ListBox _availableList;
        private ListBox _selectedList;

        private Button _selectItemsButton;
        private Button _unSelectItemsButton;

        private Label _availableItemsLabel;
        private Label _selectedItemsLabel;

        /// <summary>
        /// Width of the each List Box
        /// </summary>
        public Unit ListBoxWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Button Width
        /// </summary>
        public Unit ButtonWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Create child controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();
        }

        /// <summary>
        /// Get/set the list width
        /// </summary>
        public Unit ListWidth
        {
            get
            {
                EnsureChildControls();
                return _availableList.Width;
            }
            set
            {
                EnsureChildControls();
                _availableList.Width = value;
                _selectedList.Width = value;
            }
        }

        /// <summary>
        /// Create child controls
        /// </summary>
        protected override void CreateChildControls()
        {
            //Panel p = new Panel();
            Panel left = new Panel() { CssClass = "ListSelectorLeft" };
            Panel center = new Panel() { CssClass = "ListSelectorCenter" };
            Panel buttonsContainer = new Panel() { CssClass = "ListSelectorInner" };
            Panel right = new Panel() { CssClass = "ListSelectorRight" };

            _availableItemsLabel = new Label();
            left.Controls.Add(_availableItemsLabel);

            _selectedItemsLabel = new Label();
            right.Controls.Add(_selectedItemsLabel);

            _availableList = new ListBox { Height = this.Height, SelectionMode = ListSelectionMode.Multiple, Width = ListBoxWidth };
            left.Controls.Add(_availableList);

            _selectItemsButton = new Button { Width = ButtonWidth};
            _selectItemsButton.Click += _selectItemsButton_Click;
            _selectItemsButton.Text = ">>";

            _unSelectItemsButton = new Button { Width = ButtonWidth };
            _unSelectItemsButton.Click += _unSelectItemsButton_Click;
            _unSelectItemsButton.Text = "<<";

            buttonsContainer.Controls.Add(_selectItemsButton);
            buttonsContainer.Controls.Add(new LiteralControl("<br><br>"));
            buttonsContainer.Controls.Add(_unSelectItemsButton);

            _selectedList = new ListBox { Height = this.Height, SelectionMode = ListSelectionMode.Multiple, Width = ListBoxWidth };
            right.Controls.Add(_selectedList);


            center.Controls.Add(buttonsContainer);

            Controls.Add(left);
            Controls.Add(center);
            Controls.Add(right);

            //Controls.Add(p);
        }

        /// <summary>
        /// Get/set the selected items
        /// </summary>
        public List<ListItem> SelectedItems
        {
            get
            {
                EnsureChildControls();

                List<ListItem> items = new List<ListItem>();

                foreach (ListItem item in _selectedList.Items)
                {
                    ListItem li = new ListItem(item.Text, item.Value);
                    items.Add(li);
                }

                return items;
            }

            set
            {
                EnsureChildControls();

                _selectedList.Items.Clear();

                foreach (ListItem li in value)
                {
                    _selectedList.Items.Add(new ListItem(li.Text, li.Value));
                }
            }
        }

        /// <summary>
        /// Get/set the available items
        /// </summary>
        public List<ListItem> AvailableItems
        {
            get
            {
                EnsureChildControls();

                List<ListItem> items = new List<ListItem>();

                foreach (ListItem item in _availableList.Items)
                {
                    ListItem li = new ListItem(item.Text, item.Value);
                    items.Add(li);
                }

                return items;
            }

            set
            {
                EnsureChildControls();

                _availableList.Items.Clear();

                foreach (ListItem li in value)
                {
                    _availableList.Items.Add(new ListItem(li.Text, li.Value));
                }
            }
        }

        /// <summary>
        /// Unselect items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _unSelectItemsButton_Click(object sender, EventArgs e)
        {
            List<ListItem> itemsToRemove = new List<ListItem>();
            foreach (ListItem li in _selectedList.Items)
            {
                if (li.Selected)
                {
                    _availableList.Items.Add(li);
                    itemsToRemove.Add(li);
                }
            }

            foreach (ListItem li in itemsToRemove)
            {
                _selectedList.Items.Remove(li);
            }

            if (ItemsUnSelected != null)
            {
                ItemsUnSelected(this, new ListSelectorEventArgs(itemsToRemove));
            }
        }

        /// <summary>
        /// Select items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _selectItemsButton_Click(object sender, EventArgs e)
        {
            List<ListItem> itemsToRemove = new List<ListItem>();
            foreach (ListItem li in _availableList.Items)
            {
                if (li.Selected)
                {
                    _selectedList.Items.Add(li);
                    itemsToRemove.Add(li);
                }
            }

            foreach (ListItem li in itemsToRemove)
            {
                _availableList.Items.Remove(li);
            }

            if (ItemsSelected != null)
            {
                ItemsSelected(this, new ListSelectorEventArgs(itemsToRemove));
            }
        }
    }
}
