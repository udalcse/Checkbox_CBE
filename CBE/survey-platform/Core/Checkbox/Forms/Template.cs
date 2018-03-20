using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Data;
using Checkbox.Globalization.Text;
using Checkbox.Progress;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Checkbox.Security.Principal;
using System.Xml.Serialization;

namespace Checkbox.Forms
{
    /// <summary>
    /// Delegate for template update events
    /// </summary>
    /// <param name="source">Template that was updated.</param>
    /// <param name="e">Event Args</param>
    public delegate void TemplateUpdated(Template source, EventArgs e);

    /// <summary>
    /// Template is the abstract base class for all paged ItemData containers.  
    /// </summary>
    [Serializable]
    public abstract class Template : AccessControllablePersistedDomainObject
    {
        public const string TEMPLATE_DATA_TABLE_NAME = "TemplateData";

        #region Members

        /// <summary>
        /// The Template's collection of ItemData
        /// </summary>
        protected Dictionary<int, ItemData> _itemData;

        /// <summary>
        /// Get template pages for the template.
        /// </summary>
        private Dictionary<int, TemplatePage> _templatePages;

        /// <summary>
        /// Get list of items for a template page.
        /// </summary>
        private Dictionary<int, List<int>> _templatePageItems;

        /// <summary>
        /// List of items in the survey.
        /// </summary>
        private List<int> _itemIds;

        /// <summary>
        /// List of child items in the survey
        /// </summary>
        private Dictionary<int, int> _childItemIds;

        /// <summary>
        /// List of pages in the survey.
        /// </summary>
        private List<int> _pageIds;

        /// <summary>
        /// Get dictionary of items in the template
        /// </summary>
        protected Dictionary<int, ItemData> ItemDictionary
        {
            get { return _itemData ?? (_itemData = new Dictionary<int, ItemData>()); }
        }

        /// <summary>
        /// Get list of childItem ids in the template
        /// </summary>
        /// <remarks>ChildItemIds[childItemId] = parentItemId</remarks>
        protected Dictionary<int, int> ChildItemIds
        {
            get { return _childItemIds ?? (_childItemIds = new Dictionary<int, int>()); }
        }


        /// <summary>
        /// Get list of item ids in the template
        /// </summary>
        protected List<int> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new List<int>()); }

            private set { _itemIds = value; }
        }

        /// <summary>
        /// Get list of page ids
        /// </summary>
        public List<int> PageIds
        {
            get { return _pageIds ?? (_pageIds = new List<int>()); }

            private set { _pageIds = value; }
        }

        /// <summary>
        /// Get list of template pages
        /// </summary>
        public Dictionary<int, TemplatePage> TemplatePages
        {
            get { return _templatePages ?? (_templatePages = new Dictionary<int, TemplatePage>()); }
        }

        ///<summary>
        ///</summary>
        public int PageCount
        {
            get
            {
                return _templatePages == null ? 0 : _templatePages.Count;
            }
        }

        /// <summary>
        /// Return pages table name
        /// </summary>
        protected virtual string PageDataTableName
        {
            get { return "Pages"; }
        }

        /// <summary>
        /// Get list of items on a page.
        /// </summary>
        protected Dictionary<int, List<int>> PageItems
        {
            get { return _templatePageItems ?? (_templatePageItems = new Dictionary<int, List<int>>()); }
        }

        /// <summary>
        /// Get the creator of the response template
        /// </summary>
        public string CreatedBy { get; set; }

        #endregion

        #region Constructor/Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="supportedPermissionMasks"></param>
        /// <param name="supportedPermissions"></param>
        protected Template(string[] supportedPermissionMasks, string[] supportedPermissions)
            : base(supportedPermissionMasks, supportedPermissions)
        {
            _itemData = new Dictionary<int, ItemData>();
        }


        #endregion

        #region Items/Pages
        #endregion

        #region Loading


        /// <summary>
        /// Load template data and items data.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            if (!(data is TemplateDataSet))
            {
                return;
            }

            //Load pages
            LoadTemplatePages((TemplateDataSet)data);

            //Load template items
            LoadTemplateItems((TemplateDataSet)data);

            //Load template page/item mappings
            LoadTemplatePageItems((TemplateDataSet)data);

            //Load page data
            LoadPages();

            //Load item data
            LoadItemDatas();
        }

        /// <summary>
        /// Load item data
        /// </summary>
        private void LoadItemDatas()
        {
            foreach (var i in ItemIds)
            {
                GetItem(i);
            }
        }

        /// <summary>
        /// Load template pages from template data set.
        /// </summary>
        /// <param name="ds"></param>
        protected virtual void LoadTemplatePages(TemplateDataSet ds)
        {
            PageIds = ds.PageDataTable != null
                ? DbUtility.ListDataColumnValues<int>(ds.PageDataTable, "PageId", null, "PagePosition ASC", true)
                : new List<int>();
        }

        /// <summary>
        /// Load template items
        /// </summary>
        /// <param name="ds"></param>
        protected virtual void LoadTemplateItems(TemplateDataSet ds)
        {
            //Only load template items that are actually on pages and also in template items table
            if (ds.TemplateItemsTable == null
                || ds.PageItemsTable == null)
            {
                ItemIds = new List<int>();
                return;
            }

            //Reset item id collection
            ItemIds = new List<int>();

            List<int> pageItemIds = DbUtility.ListDataColumnValues<int>(ds.PageItemsTable, "ItemId", null, "Position ASC", true);
            List<int> templateItemIds = DbUtility.ListDataColumnValues<int>(ds.TemplateItemsTable, "ItemId", null, null, true);

            templateItemIds.Sort();

            //get all items type info for optimization purposes
            var itemsData = ItemConfigurationManager.ListDataForItems(pageItemIds);

            //Now build a list of items in both tables.  These checks shouldn't be necessary, but can hopefully prevent some loading errors
            // in cases where people have mucked with the DB or have relational integrity issues due to multiple upgrades
            foreach (int pageItemId in pageItemIds)
            {
                if (templateItemIds.BinarySearch(pageItemId) >= 0)
                {
                    ItemIds.Add(pageItemId);
                    
                    var data = itemsData.ContainsKey(pageItemId) 
                        ? itemsData[pageItemId] 
                        : GetItem(pageItemId);

                    UpdateChildItemIds(data);
                }
            }
        }

        /// <summary>
        /// Load template page items
        /// </summary>
        /// <param name="ds"></param>
        protected virtual void LoadTemplatePageItems(TemplateDataSet ds)
        {
            PageItems.Clear();
            ItemIds.Sort();

            foreach (int pageId in PageIds)
            {
                PageItems[pageId] = new List<int>();

                //List items for page
                List<int> pageItemIds = DbUtility.ListDataColumnValues<int>(ds.PageItemsTable, "ItemId", "PageId = " + pageId, "Position ASC", true);

                //Now ensure each item is in the template too
                foreach (int itemId in pageItemIds)
                {
                    if (ItemIds.BinarySearch(itemId) >= 0)
                    {
                        PageItems[pageId].Add(itemId);
                    }
                }
            }
        }

        //NWC 1/9/2007
        // This is a workaround for the session serialization / deserialization issue that causes
        // item data event handlers to become unbound.  Instead of relying on the event handler, 
        // we'll rely on a caller to notify the template when the item has changed and to provide
        // the transaction context (or not).
        ///<summary>
        ///Notify the template that an item data it contains has been saved.
        ///</summary>
        ///<param name="itemData">Data that was saved.</param>
        ///<param name="transaction">Transaction for save operation.</param>
        public void ItemSaved(ItemData itemData, IDbTransaction transaction)
        {
            OnItemSaved(itemData, transaction);
        }

        /// <summary>
        /// Overridable handler for an item being saved
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="transaction"></param>
        protected virtual void OnItemSaved(ItemData itemData, IDbTransaction transaction)
        {
            //Replace the item in the internal collection if it is being updated
            if (ItemDictionary.ContainsKey(itemData.ID.Value))
            {
                ItemDictionary[itemData.ID.Value] = itemData;
                UpdateChildItemIds(itemData);
            }
        }

        #endregion

        #region Saving
        /// <summary>
        /// Create template data in database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_Insert");
            command.AddInParameter("ModifiedDate", DbType.DateTime, LastModified);
            command.AddInParameter("CreatedDate", DbType.DateTime, CreatedDate ?? DateTime.Now);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);
            command.AddInParameter("DefaultPolicy", DbType.Int32, DefaultPolicyID);
            command.AddInParameter("AclID", DbType.Int32, AclID);
            command.AddOutParameter("TemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            object id = command.GetParameterValue("TemplateID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save template data.");
            }

            ID = Convert.ToInt32(id);
        }

        /// <summary>
        /// Update template data in database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            LastModified = DateTime.Now;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_Update");
            command.AddInParameter("TemplateID", DbType.Int32, ID.Value);
            command.AddInParameter("ModifiedDate", DbType.DateTime, LastModified);
            command.AddInParameter("DefaultPolicy", DbType.Int32, DefaultPolicyID);
            command.AddInParameter("AclID", DbType.Int32, AclID);
            command.AddInParameter("ModifiedBy", DbType.String, ModifiedBy);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Delete the template data in the database
        /// </summary>
        public override void Delete(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_Delete");
            command.AddInParameter("TemplateID", DbType.Int32, ID.Value);

            db.ExecuteNonQuery(command, t);
        }

        #endregion

        /// <summary>
        /// List ids of pages for the survey.
        /// </summary>
        /// <returns></returns>
        public virtual int[] ListTemplatePageIds()
        {
            return PageIds.ToArray();
        }

        /// <summary>
        /// List ids of items for the survey.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Children of composite items are not listed here.</remarks>
        public virtual int[] ListTemplateItemIds()
        {
            return ItemIds.ToArray();
        }

        /// <summary>
        /// Get an item from the internal items collection.  If an item with the specified ID
        /// is not found, null will be returned.  The search will include children of any composite
        /// items contained by the template.
        /// </summary>
        /// <param name="itemDataID"></param>
        /// <returns></returns>
        public ItemData GetItem(int itemDataID)
        {
            return GetItem(itemDataID, false);
        }

        /// <summary>
        /// Get an item from the internal items collection.  If an item with the specified ID
        /// is not found, null will be returned.  The search will include children of any composite
        /// items contained by the template.
        /// </summary>
        /// <param name="itemDataID"></param>
        /// <param name="forceReload"></param>
        /// <returns></returns>
        public ItemData GetItem(int itemDataID, bool forceReload)
        {
            //Make sure item is in internal dictionary
            if (!ItemIds.Contains(itemDataID) && !ChildItemIds.ContainsKey(itemDataID))
            {
                return null;
            }

            //Check dictionary
            if (!forceReload && ItemDictionary.ContainsKey(itemDataID))
            {
                return ItemDictionary[itemDataID];
            }

            //Otherwise, attempt to load item
            ItemData itemData = ItemConfigurationManager.GetConfigurationData(itemDataID, !forceReload);

            if (itemData != null)
            {
                ItemDictionary[itemDataID] = itemData;
            }

            //Ensure children loaded

            return itemData;
        }


        /// <summary>
        /// Update ChildItemIds if itemData is composite itemData
        /// </summary>
        /// <param name="itemData"></param>
        private void UpdateChildItemIds(ItemData itemData)
        {
            var compositeItemData = itemData as ICompositeItemData;
            if (compositeItemData != null && itemData.ID.HasValue)
            {
                RemoveChildItemIds(itemData.ID.Value);

                ReadOnlyCollection<Int32> childItemDataIds = compositeItemData.GetChildItemDataIDs();
                foreach (int childItemDataId in childItemDataIds)
                {
                    ChildItemIds.Add(childItemDataId, itemData.ID.Value);
                }
            }
        }

        /// <summary>
        /// Remove child item ids of specified parent item.
        /// </summary>
        /// <param name="parentItemId"></param>
        private void RemoveChildItemIds(int parentItemId)
        {
            //Find all items which should be removed.
            var itemsToRemove = (from childItemId in ChildItemIds where childItemId.Value == parentItemId select childItemId.Key).ToList();

            foreach (var itemId in itemsToRemove)
            {
                ChildItemIds.Remove(itemId);
            }
        }

        /// <summary>
        /// Get id of the parent item for the specified item. If the item hasn't any parent, null is returned.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public int? GetParentItemDataId(int itemId)
        {
            if (ChildItemIds.ContainsKey(itemId))
                return ChildItemIds[itemId];
            return null;
        }

        /// <summary>
        /// Called when an ItemData is removed from the Template's ItemDataDictionary
        /// </summary>
        /// <param name="itemId">the id of the removed ItemData</param>
        protected virtual void OnItemRemoved(int itemId) { }

        /// <summary>
        /// Called when a TemplatePage is removed
        /// </summary>
        /// <param name="pageId">the id of the removed TemplatePage</param>
        protected virtual void OnPageRemoved(int pageId) { }


        /// <summary>
        /// Set an item in the internal items collection. This does not add an item to a page or to the template
        /// items collection. This method should generally not be used except in cases where using the add item to page
        /// methods are not appropriate.  The item will not be added if it has a NULL id or if an item with the specified ID
        /// already exists.
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        /// <remarks>This method is used when building an on-the-fly template that is not intended to be
        /// persisted to the database.</remarks>
        public void SetItem(ItemData itemData)
        {
            SetItem(itemData, false);
        }

        /// <summary>
        /// Set an item in the internal items collection. This does not add an item to a page or to the template
        /// items collection. This method should generally not be used except in cases where using the add item to page
        /// methods are not appropriate, such as when an item is updated during editing.
        /// If an item with the specified ID already exists, it will be replaced when the 
        /// flag is set to TRUE
        /// </summary>
        public void SetItem(ItemData itemData, bool replace)
        {
            if (itemData.ID.HasValue && (replace || !ItemDictionary.ContainsKey(itemData.ID.Value)))
            {
                if (!ItemIds.Contains(itemData.ID.Value))
                {
                    ItemIds.Add(itemData.ID.Value);
                }

                ItemDictionary[itemData.ID.Value] = itemData;
                UpdateChildItemIds(itemData);
            }
        }

        /// <summary>
        /// Creates a new <see cref="TemplatePage"/> at the specified position
        /// </summary>
        /// <param name="position">the position to add the page</param>
        /// <param name="ensureIntrinsicPages"></param>
        /// <returns>a new TemplatePage</returns>
        protected virtual TemplatePage CreatePage(Int32 position, bool ensureIntrinsicPages)
        {
            return CreatePage(position, TemplatePageType.ContentPage);
        }

        /// <summary>
        /// Create a page of a specific type.
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="pageType"></param>
        /// <returns></returns>
        protected virtual TemplatePage CreatePage(int position, TemplatePageType pageType)
        {
            //Ensure position is valid value
            if (position <= 0 || position > PageIds.Count)
            {
                //Page positions are 1-based
                position = PageIds.Count + 1;
            }

            //Create and save page
            var page = new TemplatePage(ID.Value, position, pageType, string.Empty);
            if (page.Position == 2) page.ShouldForceBreak = true;

            page.Save();

            //Insert page into page list and items dictionary
            if (page.ID.HasValue && page.ID.Value > 0)
            {
                //Convert page position to index of page id list
                int listIndex = position - 1;

                if (listIndex < 0)
                {
                    listIndex = 0;
                }

                //Add page id to list
                if (listIndex >= PageIds.Count)
                {
                    PageIds.Add(page.ID.Value);
                }
                else
                {
                    PageIds.Insert(listIndex, page.ID.Value);
                }

                //Add page object to dictionary
                TemplatePages[page.ID.Value] = page;

                //Add placeholder for page items
                PageItems[page.ID.Value] = new List<int>();

                //Ensure page positions are congiguous after adding a new page
                EnsureContiguousPagePositions();
            }

            return page;
        }

        /// <summary>
        /// Ensure page positions are contiguous
        /// </summary>
        protected virtual void EnsureContiguousPagePositions()
        {
            int position = 1;

            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            //PageIds list is assumed to be in proper order
                            foreach (int pageId in PageIds)
                            {
                                //Get each page and check position
                                TemplatePage page = GetPage(pageId);

                                if (page != null)
                                {
                                    if (page.Position != position)
                                    {
                                        page.Position = position;
                                        page.Save(transaction);
                                    }
                                    //Ensure page positions are 1 greater
                                    // than previous page position.
                                    position++;
                                }
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Ensure items on a page are contiguously positioned.
        /// </summary>
        /// <param name="pageId"></param>
        protected void EnsureContiguousItemPositions(int pageId)
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            //Update item positions in db for items on page
                            var pageItems = new List<int>(PageItems[pageId]);

                            for (int itemPosition = 1; itemPosition <= pageItems.Count; itemPosition++)
                            {
                                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_UpsertItem");
                                command.AddInParameter("PageId", DbType.Int32, pageId);
                                command.AddInParameter("ItemId", DbType.Int32, pageItems[itemPosition - 1]);
                                command.AddInParameter("Position", DbType.Int32, itemPosition);

                                db.ExecuteNonQuery(command, transaction);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Remove all items from a template page
        /// </summary>
        /// <param name="pageId"></param>
        public virtual void DeletePageItems(int pageId)
        {
            List<int> items = this._templatePageItems[pageId];
            if (items != null)
            {
                int[] itemsToDelete = items.ToArray();
                foreach (int itemId in itemsToDelete)
                {
                    this.DeleteItemFromPage(pageId, itemId);
                }
            }
            return;
        }

        /// <summary>
        /// Add page to template.  If position not specified, it will be added as last survey page.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="ensureIntrinsicPages"></param>
        public int AddPageToTemplate(int? position, bool ensureIntrinsicPages)
        {
            if (!position.HasValue)
                position = PageIds.Count + 1;//Page positions are 1-based
            //check if page id list has Completion page
            if (position == PageIds.Count + 1 &&
                (TemplatePages.Count > 0 && TemplatePages.Any(p => p.Value.PageType == TemplatePageType.Completion)))
                position = PageIds.Count;
            TemplatePage tp = CreatePage(position.Value, ensureIntrinsicPages);

            return tp != null ? tp.ID.Value : -1;
        }

        /// <summary>
        /// Removes a <see cref="TemplatePage"/> from the TemplatePages collection
        /// </summary>
        /// <param name="pageId">Page to delete</param>
        public virtual void DeletePage(int pageId)
        {
            TemplatePage page = GetPage(pageId);
            if (page == null)
                return;

            if (page.Position < 1)
                return;

            DeletePageItems(pageId);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeletePage");
            command.AddInParameter("TemplateID", DbType.Int32, ID.Value);
            command.AddInParameter("PageID", DbType.Int32, pageId);
            db.ExecuteNonQuery(command);

            TemplatePages.Remove(pageId);
            _pageIds.Remove(pageId);

            //Ensure page positions are congiguous after adding a new page
            EnsureContiguousPagePositions();
        }

        /// <summary>
        /// Gets a <see cref="TemplatePage"/> within this Template with a given ID
        /// </summary>
        /// <param name="id">the ID of the TemplatePage</param>
        /// <returns>the TemplatePage, if found; otherwise null</returns>
        public virtual TemplatePage GetPage(Int32 id)
        {
            //Make sure page is in template
            if (!_pageIds.Contains(id))
            {
                return null;
            }

            //Check pages dictionary
            if (TemplatePages.ContainsKey(id))
            {
                return TemplatePages[id];
            }

            TemplatePage tp = LoadPage(id);

            if (tp != null)
            {
                TemplatePages[id] = tp;
            }

            return tp;
        }

        /// <summary>
        /// Get the page at a certain position in the template.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public virtual TemplatePage GetPageAtPosition(int pagePosition)
        {
            pagePosition--;

            if (pagePosition < 0 || pagePosition >= PageIds.Count)
            {
                return null;
            }

            return GetPage(PageIds[pagePosition]);
        }

        /// <summary>
        /// Get the page at a certain position in the template.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <returns></returns>
        public virtual int? GetPageIdAtPosition(int pagePosition)
        {
            if (pagePosition < 0 || pagePosition >= PageIds.Count)
                return null;

            return PageIds[pagePosition];
        }

        ///<summary>
        /// Load template page data from the database
        ///</summary>
        ///<returns></returns>
        public virtual IEnumerable<TemplatePage> LoadPages()
        {
            return ListTemplatePageIds().Select(GetPage).ToList();
        }

        /// <summary>
        /// Load template page data from the database
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        protected virtual TemplatePage LoadPage(int pageId)
        {
            var tp = new TemplatePage();
            tp.Load(pageId);

            //Add items
            List<int> pageItems = PageItems.ContainsKey(pageId)
                ? PageItems[pageId]
                : new List<int>();

            tp.Initialize(ID.Value, pageItems, new List<int>(), -1);

            return tp;
        }

        /// <summary>
        /// Gets the 1-based position index of an item within a TemplatePage
        /// <remarks>This operation does not support searches for child ItemDatas</remarks>
        /// </summary>
        /// <param name="itemId">the item id</param>
        /// <returns>the position, if found</returns>
        public virtual int? GetItemPositionWithinPage(int itemId)
        {

            foreach (var pageItem in PageItems)
            {
                if (pageItem.Value.Contains(itemId))
                    return pageItem.Value.IndexOf(itemId) + 1;
            }

            return null;
        }

        /// <summary>
        /// Gets the index of the TemplatePage containing a given ItemData
        /// <remarks>
        /// This operation performs a recursive search of <see cref="ICompositeItemData"/> Items
        /// </remarks>
        /// </summary>
        /// <param name="itemId">the ItemData</param>
        /// <returns>the 1-based index of the TemplatePage, if found</returns>
        public virtual int? GetPagePositionForItem(int itemId)
        {
            var pageIds = PageIds.ToArray();

            var pagePos = 1;

            foreach (var page in pageIds.Select(GetPage))
            {
                if (page.ContainsItem(itemId))
                    return pagePos;
                pagePos++;
            }

            return null;
        }

        /// <summary>
        /// Gets the id of the TemplatePage containing a given ItemData
        /// </summary>
        /// <param name="itemId">the ItemData</param>
        /// <returns>the 1-based index of the TemplatePage, if found</returns>
        public virtual int? GetPageIdForItem(int itemId)
        {
            int? itemParentId = null;
            if (ChildItemIds.ContainsKey(itemId))
                itemParentId = ChildItemIds[itemId];

            var pageIds = PageIds.ToArray();

            foreach (var page in pageIds.Select(GetPage))
            {
                if (page.ContainsItem(itemId) || (itemParentId.HasValue && page.ContainsItem(itemParentId.Value)))
                    return page.ID;
            }

            return null;
        }

        /// <summary>
        /// Deletes a <see cref="TemplatePage"/> at the specified position
        /// </summary>
        /// <param name="position">the position of the TemplatePage to delete</param>
        public virtual void DeletePageAt(Int32 position)
        {
            //TODO: Delete Page
        }

        /// <summary>
        /// Set the id of the layout template for a given page
        /// </summary>
        /// <param name="pageID"></param>
        /// <param name="layoutTemplateID"></param>
        public virtual void SetPageLayoutTemplate(Int32 pageID, int? layoutTemplateID)
        {
            //TODO: Set layout template
        }

        /// <summary>
        /// Copyes a <see cref="TemplatePage"/> within this <see cref="ResponseTemplate"/> adding functionality to account for Rule logic associated with the 
        /// <see cref="TemplatePage"/> to be removed for new page
        /// </summary>
        /// <param name="pageId">the <see cref="TemplatePage"/> to copy</param>
        /// <param name="position">the 'to' position</param>
        public virtual TemplatePage CopyPage(int pageId, int position, CheckboxPrincipal principal)
        {
            TemplatePage page = GetPage(pageId);

            int[] items = page.ListItemIds();

            int newPageId = AddPageToTemplate(position, false);

            foreach (int itemId in items)
            {
                ItemData item = GetItem(itemId);

                var copy = ItemConfigurationManager.CopyItem(item, principal);

                if (copy == null)
                    continue;

                AddItemToPage(newPageId, copy.ID.Value, null);
            }

            TemplatePage newPage = GetPage(newPageId);
            newPage.Save();

            return newPage;
        }


        /// <summary>
        /// Moves a TemplatePage within this ResponseTemplate
        /// </summary>
        /// <param name="pageId">the  id of the <see cref="TemplatePage"/></param>
        /// <param name="position">the 'to' position</param>
        public virtual void MovePage(int pageId, Int32 position)
        {
            TemplatePage page = GetPage(pageId);

            Int32 start = page.Position;

            if (start < position)
            {
                for (int i = start - 1; i < position - 1; i++)
                {
                    PageIds[i] = PageIds[i + 1];
                }
            }
            else
            {
                for (int i = start - 1; i > position - 1; i--)
                {
                    PageIds[i] = PageIds[i - 1];
                }
            }

            PageIds[position - 1] = pageId;
            EnsureContiguousPagePositions();
        }

        /// <summary>
        /// Moves an <see cref="ItemData"/> from one TemplatePage to another
        /// </summary>
        /// <param name="itemId">the <see cref="ItemData"/> to move</param>
        /// <param name="fromPageId">The <see cref="TemplatePage"/> to move the item from.</param>
        /// <param name="toPageId">the <see cref="TemplatePage"/> to move it to</param>
        /// <param name="position">the position of the item on the page</param>
        public virtual void MoveItemToPage(int itemId, int fromPageId, int toPageId, int position)
        {
            //Don't bother adding if page is not part of page collection
            if (!PageIds.Contains(fromPageId) || !PageIds.Contains(toPageId))
            {
                return;
            }

            // First remove the item from its initial page
            if (PageItems.ContainsKey(fromPageId) && PageItems[fromPageId].Contains(itemId))
            {
                List<int> itemIDsToRefresh = new List<int>();
                itemIDsToRefresh.AddRange(PageItems[fromPageId]);
                if (fromPageId != toPageId)
                    itemIDsToRefresh.AddRange(PageItems[toPageId]);

                RemoveItemFromPage(fromPageId, itemId);
                AddItemToPage(toPageId, itemId, position);

                ////Ensure item positions are contiguous
                EnsureContiguousItemPositions(fromPageId);
                EnsureContiguousItemPositions(toPageId);

                //Add item to template
                //AddItemToTemplate(itemId);

                //Reload affected page
                TemplatePages[fromPageId] = LoadPage(fromPageId);
                TemplatePages[toPageId] = LoadPage(toPageId);

                //clean the cache
                foreach(int itemID in itemIDsToRefresh)
                    SurveyMetaDataProxy.RemoveItemFromCache(itemID);
            }
        }

        /// <summary>
        /// Add an item to the template.  Only used for items that should be associated with the template
        /// but are not part of any pages
        /// </summary>
        /// <param name="itemId"></param>
        private void AddItemToTemplate(int itemId)
        {
            //Add item to internal collection
            if (!ItemIds.Contains(itemId))
            {
                ItemIds.Add(itemId);
                UpdateChildItemIds(GetItem(itemId));
            }

            //Call sproc to add item, the sproc knows not to add duplicates
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_AddItem");
            command.AddInParameter("TemplateId", DbType.Int32, ID);
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            db.ExecuteNonQuery(command);
        }



        /// <summary>
        /// Removes the ItemData from the Template's ItemData table and ItemDataDictionary
        /// </summary>
        /// <param name="itemId">the ItemData to remove</param>
        private void RemoveItemFromTemplate(int itemId)
        {
            //Remove item from internal collection
            if (ItemIds.Contains(itemId))
            {
                ItemIds.Remove(itemId);
                RemoveChildItemIds(itemId);
            }

            //Call sproc to remove item
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeleteItem");
            command.AddInParameter("TemplateId", DbType.Int32, ID);
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Adds an ItemData to the final position within a TemplatePage
        /// </summary>
        /// <param name="pageId">the id <see cref="TemplatePage"/></param>
        /// <param name="itemId">Item to add to page.</param>
        /// <param name="position">Position of item.  Insert item at end if position is null.</param>
        public virtual void AddItemToPage(int pageId, int itemId, int? position)
        {
            //Don't bother adding if page is not part of page collection
            if (!PageIds.Contains(pageId))
            {
                return;
            }

            if (PageItems.ContainsKey(pageId)
                && !PageItems[pageId].Contains(itemId))
            {
                //If no position, append item
                if (!position.HasValue
                    || position > PageItems[pageId].Count
                    || position <= 0)
                {
                    //Add item to page items collection
                    PageItems[pageId].Add(itemId);
                }
                else
                {
                    //Otherwise, insert item into list at correct position
                    PageItems[pageId].Insert(position.Value - 1, itemId);
                }

                //Ensure item positions are contiguous
                EnsureContiguousItemPositions(pageId);

                //Add item to template
                AddItemToTemplate(itemId);

                //Reload affected page
                TemplatePages[pageId] = LoadPage(pageId);
            }
        }

        /// <summary>
        /// Removes an ItemData from a given TemplatePage and deletes the ItemData from the Template
        /// </summary>
        /// <param name="pageId">the containing TemplatePage</param>
        /// <param name="itemId">the ItemData</param>
        public virtual void DeleteItemFromPage(int pageId, int itemId)
        {
            RemoveItemFromPage(pageId, itemId);
            RemoveItemFromTemplate(itemId);
            RemoveItemFromFieldMapping(itemId);
        }


        /// <summary>
        /// Removes the item from field mapping.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public virtual void RemoveItemFromFieldMapping(int itemId)
        {
            //Call sproc to remove item
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeleteItemFieldMapping");
            command.AddInParameter("ItemID", DbType.Int32, itemId);

            db.ExecuteNonQuery(command);
        }


        /// <summary>
        /// Removes an ItemData from a TemplatePage and reorders the ItemData positions 
        /// within the TemplatePage
        /// </summary>
        /// <param name="pageId">the TemplatePage</param>
        /// <param name="itemId">the ItemData</param>
        public virtual void RemoveItemFromPage(int pageId, int itemId)
        {
            //Don't bother removing if page is not part of page collection
            if (!PageIds.Contains(pageId))
            {
                return;
            }

            if (PageItems.ContainsKey(pageId)
                && PageItems[pageId].Contains(itemId))
            {
                //Remove item from page items collection
                PageItems[pageId].Remove(itemId);

                //Ensure item positions are contiguous
                EnsureContiguousItemPositions(pageId);

                //Reload affected page
                TemplatePages[pageId] = LoadPage(pageId);

                //Call sproc to remove item
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_DeleteItem");
                command.AddInParameter("PageID", DbType.Int32, pageId);
                command.AddInParameter("ItemID", DbType.Int32, itemId);

                db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Copies an ItemData from a TemplatePage
        /// </summary>
        /// <param name="pageId">the TemplatePage id</param>
        /// <param name="itemId">the ItemData id</param>
        public virtual void CopyItemFromPage(int pageId, int itemId, CheckboxPrincipal principal)
        {
            ItemData itemData = GetItem(itemId);

            if (itemData != null)
            {
                var copy = ItemConfigurationManager.CopyItem(itemData, principal);

                if (copy == null)
                    return;

                AddItemToPage(pageId, copy.ID.Value, null);
            }
        }

        /// <summary>
        /// Moves an ItemData within a TemplatePage
        /// </summary>
        /// <param name="pageId">the TemplatePage holding the ItemData</param>
        /// <param name="itemId">the ItemData to move</param>
        /// <param name="position">the position to move to</param>
        public void MoveItem(int pageId, int itemId, Int32 position)
        {
        }

        /// <summary>
        /// Get the name of the item appearances table
        /// </summary>
        protected virtual string ItemAppearanceTableName
        {
            get { return "AppearanceData"; }
        }

        /// <summary>
        /// Get name of template table
        /// </summary>
        public override string DomainDBTableName
        {
            get { return "ckbx_Template"; }
        }

        /// <summary>
        /// Return template data table name
        /// </summary>
        public override string DataTableName
        {
            get { return TEMPLATE_DATA_TABLE_NAME; }
        }

        /// <summary>
        /// Get the name of the table containing page items
        /// </summary>
        protected virtual string PageItemsTableName
        {
            get { return "PageItems"; }
        }

        /// <summary>
        /// Get the name of the template items table
        /// </summary>
        protected virtual string TemplateItemsTableName
        {
            get { return "Items"; }
        }

        /// <summary>
        /// Template id column
        /// </summary>
        public override string DomainDBIdentityColumnName
        {
            get { return "TemplateID"; }
        }

        /// <summary>
        /// Get the name of the table containing item appearances
        /// </summary>
        protected virtual string ItemAppearanceMapTableName
        {
            get { return "ItemAppearanceMap"; }
        }

        /// <summary>
        /// Get the text PageId prefix for template text
        /// </summary>
        protected virtual string TextIDPrefix
        {
            get { return "templateData"; }
        }

        #region CRUD



        #endregion

        #region DBCommands

        /// <summary>
        /// Get command to insert a page association with the template
        /// </summary>
        /// <returns></returns>
        private DBCommandWrapper GetInsertPageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper insert = db.GetStoredProcCommandWrapper("ckbx_sp_Template_InsertPage");
            insert.AddInParameter("TemplateID", DbType.Int32, ID);
            insert.AddInParameter("PagePosition", DbType.Int32, "PagePosition", DataRowVersion.Current);
            insert.AddInParameter("LayoutTemplateID", DbType.Int32, "LayoutTemplateID", DataRowVersion.Current);
            insert.AddParameter("PageID", DbType.Int32, ParameterDirection.Output, "PageID", DataRowVersion.Current, null);

            return insert;
        }

        /// <summary>
        /// Get command to update page
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetUpdatePageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper update = db.GetStoredProcCommandWrapper("ckbx_sp_Template_UpdatePage");
            update.AddInParameter("TemplateID", DbType.Int32, ID);
            update.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);
            update.AddInParameter("PagePosition", DbType.Int32, "PagePosition", DataRowVersion.Current);
            update.AddInParameter("LayoutTemplateID", DbType.Int32, "LayoutTemplateID", DataRowVersion.Current);

            return update;
        }

        /// <summary>
        /// Get command to delete page
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeletePageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeletePage");
            delete.AddInParameter("TemplateID", DbType.Int32, ID);
            delete.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);

            return delete;
        }

        /// <summary>
        /// Get a command to add an item to a page
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetAddItemToPageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper insertItem = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_AddItem");
            insertItem.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);
            insertItem.AddInParameter("ItemID", DbType.Int32, "ItemID", DataRowVersion.Current);
            insertItem.AddInParameter("Position", DbType.Int32, "Position", DataRowVersion.Current);

            return insertItem;
        }

        /// <summary>
        /// Get a command to delete an item/page association
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeleteItemFromPageCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper deleteItem = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_DeleteItem");
            deleteItem.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);
            deleteItem.AddInParameter("ItemID", DbType.Int32, "ItemID", DataRowVersion.Current);

            return deleteItem;
        }

        /// <summary>
        /// Get a command to set an item's page position
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetSetPageItemPositionCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper updateItem = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_SetItemPos");
            updateItem.AddInParameter("PageID", DbType.Int32, "PageID", DataRowVersion.Current);
            updateItem.AddInParameter("ItemID", DbType.Int32, "ItemID", DataRowVersion.Current);
            updateItem.AddInParameter("Position", DbType.Int32, "Position", DataRowVersion.Current);

            return updateItem;
        }


        /// <summary>
        /// Get a command to add an item to a template
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetAddTemplateItemCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper addItem = db.GetStoredProcCommandWrapper("ckbx_sp_Template_AddItem");
            addItem.AddInParameter("TemplateID", DbType.Int32, ID);
            addItem.AddInParameter("ItemID", DbType.Int32, "ItemID", DataRowVersion.Current);

            return addItem;
        }

        /// <summary>
        /// Get a command to remove an item from a template
        /// </summary>
        /// <returns></returns>
        protected virtual DBCommandWrapper GetDeleteItemFromTemplateCommand()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper deleteItem = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeleteItem");
            deleteItem.AddInParameter("TemplateID", DbType.Int32, ID);
            deleteItem.AddInParameter("ItemID", DbType.Int32, "ItemID", DataRowVersion.Current);

            return deleteItem;
        }
        #endregion

        #region Xml Serializing

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteConfigurationToXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            WriteTemplateData(writer);
            WriteCustomTextData(writer);
            WriteTemplatePageData(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void WriteCustomTextData(XmlWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void WriteTemplateData(XmlWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteTemplatePageData(XmlWriter writer)
        {
            writer.WriteStartElement("TemplatePages");
            int[] pageIds = ListTemplatePageIds();
            writer.WriteAttributeString("Count", pageIds.Length.ToString());

            var pageDataWriter = GetPageExportWriter();

            foreach (int pageId in pageIds)
            {
                TemplatePage page = GetPage(pageId);
                page.WriteXml(writer, pageDataWriter.WritePageData);
            }

            writer.WriteEndElement(); // TemplatePages
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual TemplatePageExportWriter GetPageExportWriter()
        {
            return new TemplatePageExportWriter(null, GetItem);
        }

        /// <summary>
        /// Import template and track progress
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="progressKey"></param>
        /// <param name="progressLanguage"></param>
        /// <param name="callback"></param>
        /// <param name="forCopy">Determine if this xml import is used to create a copy, or to create a new template from xml file</param>
        public void Import(CheckboxPrincipal principal, XmlNode xmlNode, string progressKey, string progressLanguage, ReadExternalDataCallback callback = null, bool forCopy = false)
        {
             if (!ID.HasValue)
                Save();

             if (Utilities.IsNotNullOrEmpty(progressKey) && Utilities.IsNotNullOrEmpty(progressLanguage))
             {
                 ProgressProvider.SetProgress(progressKey, TextManager.GetText("/controlText/templateImport/importingTemplateData", progressLanguage), 0);
             }

            //Load template information
            LoadTemplateData(xmlNode);

            if (Utilities.IsNotNullOrEmpty(progressKey) && Utilities.IsNotNullOrEmpty(progressLanguage))
            {
                ProgressProvider.SetProgress(progressKey, TextManager.GetText("/controlText/templateImport/importingPages", progressLanguage), 10);
            }

            //Load template page information
            LoadTemplatePageData(principal, xmlNode, progressKey, progressLanguage, null, forCopy);

            //Load template page information
            LoadTemplateCustomTextData(xmlNode);

            if (callback != null)
            {
                callback(this, xmlNode, principal);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"></param>
        public override void Import(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            Import((CheckboxPrincipal)creator, xmlNode, null, null, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected abstract void LoadTemplateData(XmlNode xmlNode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected abstract void LoadTemplateCustomTextData(XmlNode xmlNode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        protected static string GetSurveySpecificTextId(string textKey, int surveyId)
        {
            //Generate text id
            return string.Format("/templateData/{0}/{1}", surveyId, textKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="progressKey">Key used for progress tracking wiht ProgressProvider</param>
        /// <param name="progressLanguage">Language code for progress messages.</param>
        /// <param name="pageImportReader">Page reader to used for additional page import processing, such as loading rule data, etc.</param>
        /// <param name="forCopy">Determine if this xml import is used to create a copy, or to create a new template from xml file</param>
        protected virtual void LoadTemplatePageData(CheckboxPrincipal principal, XmlNode xmlNode, string progressKey, string progressLanguage, PageImportReader pageImportReader = null, bool forCopy = false)
        {
            var pageNodes = xmlNode.SelectNodes("TemplatePages/Page");

            pageImportReader = pageImportReader ?? new PageImportReader(null);

            var pageCount = pageNodes.Count;
            int currentPage = 0;

            //Load pages
            foreach (XmlNode pageNode in pageNodes)
            {
                currentPage++;

                //Figure out page type
                var pType = (TemplatePageType) XmlUtility.GetAttributeEnum(pageNode, "Type", typeof (TemplatePageType));

                //Create page
                TemplatePage page = CreatePage(-1, pType);

                //Copy/Import
                if (forCopy)               
                    page.Import(pageNode, pageImportReader.CopyPageData, principal);
                else
                    page.Import(pageNode, pageImportReader.ReadPageData, principal);

                if (Utilities.IsNotNullOrEmpty(progressKey) && Utilities.IsNotNullOrEmpty(progressLanguage))
                {
                    //Update progress for page.  Parameters mean that this step occupies 80% of the import process and when complete process will be at
                    // 90% overall.
                    ProgressProvider
                        .SetProgressCounter(
                            progressKey,
                            string.Format(
                                TextManager.GetText("/controlText/templateImport/importingPages", progressLanguage),
                                currentPage, pageCount),
                            currentPage,
                            pageCount,
                            80,
                            90
                        );
                }
            }

            if (Utilities.IsNotNullOrEmpty(progressKey) && Utilities.IsNotNullOrEmpty(progressLanguage))
            {
                ProgressProvider.SetProgress(progressKey, TextManager.GetText("/controlText/templateImport/importingPageItems", progressLanguage), 90);
            }

            //Now add items to pages
            foreach(var pageId in pageImportReader.PageItemMap.Keys)
            {
                var pageItems = pageImportReader.PageItemMap[pageId];

                if(pageItems == null)
                {
                    continue;
                }

                foreach(var pageItem in pageItems)
                {
                    AddItemToPage(pageId, pageItem, null);
                }
            }
        }

        #endregion


    }
}
