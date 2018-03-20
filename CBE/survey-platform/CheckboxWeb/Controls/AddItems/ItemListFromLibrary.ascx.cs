using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Users;
using Checkbox.Pagination;
using Checkbox.Web.UI.Controls.GridTemplates;

namespace CheckboxWeb.Controls.AddItems
{
    /// <summary>
    /// Control for displaying available libraries and items which is contained in this libraries.
    /// </summary>
    public partial class ItemListFromLibrary : Checkbox.Web.Common.UserControlBase
    {
        private List<LightweightLibraryTemplate> _libraries;
        private LibraryTemplate _currentLibrary;
        private DataTable _allItems;

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Item count per page
        /// </summary>
        public int ItemCountPerPage { get; set; }

        /// <summary>
        /// Get current library
        /// </summary>
        public LibraryTemplate CurrentLibrary
        {
            get
            {
                int currentLibraryId = int.Parse(_libraryList.SelectedValue);

                if (_currentLibrary == null || _currentLibrary.ID != currentLibraryId)
                {
                    _currentLibrary = LibraryTemplateManager.GetLibraryTemplate(currentLibraryId);
                }

                return _currentLibrary;
            }
        }

        /// <summary>
        /// List of available libraries
        /// </summary>
        private List<LightweightLibraryTemplate> AvailableLibraries
        {
            get
            {
                if (_libraries == null)
                {
                    var paginationContext = new PaginationContext
                    {
                        CurrentPage = 1,
                        PageSize = 100000, //Max count of libraries in this control
                        Permissions = new List<string>(new[] { "Library.View" })
                    };
                    _libraries = LibraryTemplateManager.GetAvailableLibraryTemplates(UserManager.GetCurrentPrincipal(), paginationContext);
                }

                return _libraries;
            }
        }


        /// <summary>
        /// Indexes os selected rows
        /// </summary>
        private List<int> SelectedIndexes
        {
            get
            {
                if (Session["_selectedItemsForLibrary"] == null)
                    Session["_selectedItemsForLibrary"] = new List<int>();
                return Session["_selectedItemsForLibrary"] as List<int>;
            }
            set { Session["_selectedItemsForLibrary"] = value; }
        }

        /// <summary>
        /// Get collection of selected item ids.
        /// </summary>
        public ReadOnlyCollection<int> SelectedItemIds
        {
            get
            {
                UpdateSelectedIndexes();
                var ids =new List<int>();
                foreach (var i in (SelectedIndexes))
                {
                    ids.Add(Convert.ToInt32(AllItems.Rows[i]["ItemID"]));
                }
                return new ReadOnlyCollection<int>(ids);
            }
        }

        
        /// <summary>
        /// Remove item form selected items
        /// </summary>
        /// <param name="itemId"></param>
        public void RemoveItemFromSelectedItems(int itemId)
        {
            if (Session["_selectedItemsForLibrary"] != null)
            {
                var selectedIndexes = Session["_selectedItemsForLibrary"] as List<int>;                
                foreach (var i in selectedIndexes)
                {
                    if (Convert.ToInt32(AllItems.Rows[i]["ItemID"]) == itemId)
                    {
                        (_itemList.Columns[1] as CustomCheckBoxField).Uncheck(i);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Data table that contains all items of selected library
        /// </summary>
        private DataTable AllItems
        {
            get { return _allItems ?? (_allItems = CreateLibraryTable()); }
        }

        /// <summary>
        /// Get/set previous selected page.
        /// </summary>
        private int PreviousPage
        {
            get
            {
                if (Session["ItemListFromLibrary_PrevPage"] == null)
                    Session["ItemListFromLibrary_PrevPage"] = 1;
                return (int)Session["ItemListFromLibrary_PrevPage"];
            }
            set { Session["ItemListFromLibrary_PrevPage"] = value; }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _panelLibraries.Visible = AvailableLibraries.Count > 0;
            _panelLibrariesNotFound.Visible = AvailableLibraries.Count == 0;            

            if (!IsPostBack)
            {
                SelectedIndexes = null;
                if (AvailableLibraries.Count > 0)
                {
                    PopulateAvailableLibraryList();
                    _pager.PageCount = AllItems.Rows.Count/ItemCountPerPage +
                                       ((AllItems.Rows.Count%ItemCountPerPage > 0) ? 1 : 0);
                    _pager.PageNumber = 1;
                    _pager.ItemCount = AllItems.Rows.Count;
                    LoadItemList();
                }
                else
                {
                    _panelItemList.Visible = false;
                    _panelEmptyItemList.Visible = false;
                }
            }
        }

        /// <summary>
        /// Update selected indexes based on the selected rows in _itemList
        /// </summary>
        private void UpdateSelectedIndexes()
        {
            int firstItemNumber = (PreviousPage - 1) * ItemCountPerPage;            
            
            //This collection contains indexes from 0 to (itemPerPage-1). So we have to add firstItemNumber to work with the all items
            ReadOnlyCollection<int> selected = (_itemList.Columns[1] as CustomCheckBoxField).SelectedIndexes;

            for (int i = 0; i < ItemCountPerPage; i++)
            {
                if (selected.Contains(i))
                {
                    if (!SelectedIndexes.Contains(i+firstItemNumber))
                        SelectedIndexes.Add(i+firstItemNumber);
                }
                else
                {
                    SelectedIndexes.Remove(i + firstItemNumber);
                }
            }
        }

        /// <summary>
        /// Page change handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _pager_OnPagingChanged(object sender, Checkbox.Web.UI.Controls.CustomPagingEventArgs e)
        {
            UpdateSelectedIndexes();
            LoadItemList();               
            PreviousPage = _pager.PageNumber;
        }

        /// <summary>
        /// Library change handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _libraryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pager.PageNumber = 1;
            PreviousPage = 1;
            _pager.PageCount = AllItems.Rows.Count / ItemCountPerPage + ((AllItems.Rows.Count % ItemCountPerPage > 0) ? 1 : 0);
            _pager.ItemCount = AllItems.Rows.Count;
            _pager.Reload();
            SelectedIndexes = null;
            LoadItemList();
        }


        /// <summary>
        /// Load item list for current page and current library.
        /// </summary>
        private void LoadItemList()
        {
            DataTable table = GetItemListForPage(_pager.PageNumber);
            if (table.Rows.Count > 0)
            {
                _itemList.DataSource = table;
                _itemList.DataBind();
            }
            _panelItemList.Visible = table.Rows.Count > 0;
            _panelEmptyItemList.Visible = table.Rows.Count == 0;
        }

        /// <summary>
        /// Populate available libraries.
        /// </summary>
        private void PopulateAvailableLibraryList()
        {
            _libraryList.Items.Clear();
            foreach (var library in AvailableLibraries)
            {
                _libraryList.Items.Add(new ListItem(Server.HtmlDecode(library.Name), library.ID.ToString()));
            }
        }

        /// <summary>
        /// Create a table for a library data source
        /// </summary>
        /// <returns></returns>
        private DataTable CreateLibraryTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ItemID", typeof(Int32));
            dt.Columns.Add("ItemType", typeof(string));

            var templatePages = CurrentLibrary.ListTemplatePageIds();

            foreach (var pageId in templatePages)
            {
                var page = CurrentLibrary.GetPage(pageId);
                var items = page.ListItemIds();

                foreach (var itemId in items)
                {
                    var item = CurrentLibrary.GetItem(itemId);
                    dt.Rows.Add(item.ID, item.ItemTypeName);
                }
            }

            return dt;
        }

        /// <summary>
        /// Get Item List for current page and current library
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        private DataTable GetItemListForPage(int pageNumber)
        {
            var dt = new DataTable();
            dt.Columns.Add("ItemID", typeof(Int32));
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

            var firstItemNumber = (pageNumber - 1) * ItemCountPerPage;
            var lastItemNumber = Math.Min(AllItems.Rows.Count, pageNumber * ItemCountPerPage);

            for (var i = firstItemNumber; i < lastItemNumber; i++)
            {
                dt.Rows.Add(AllItems.Rows[i]["ItemID"], AllItems.Rows[i]["ItemType"], SelectedIndexes.Contains(i));
            }

            return dt;
        }
    }
}