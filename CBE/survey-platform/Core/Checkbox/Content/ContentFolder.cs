using System.Collections.Generic;

namespace Checkbox.Content
{
    /// <summary>
    /// Generic representation of a content folder
    /// </summary>
    public abstract class ContentFolder
    {
        private Dictionary<string, ContentItem> _contentItems;
        private Dictionary<string, ContentFolder> _contentFolders;

        /// <summary>
        /// Get/set folder name
        /// </summary>
        public virtual string FolderName { get; set; }

        /// <summary>
        /// Get/set the folder path
        /// </summary>
        public virtual string FolderPath { get; set; }

        /// <summary>
        /// Get/set whether the folder is public
        /// </summary>
        public virtual bool IsPublic { get; set; }

        /// <summary>
        /// Get the list of content items
        /// </summary>
        protected Dictionary<string, ContentItem> Items
        {
            get { return _contentItems ?? (_contentItems = LoadItems()); }
        }

        /// <summary>
        /// Get the folders that are children of this folder
        /// </summary>
        protected Dictionary<string, ContentFolder> Folders
        {
            get { return _contentFolders ?? (_contentFolders = LoadFolders()); }
        }

        /// <summary>
        /// Add a content item to the collection
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddContentItem(ContentItem item)
        {
            StoreContentItem(item);
            Items[item.ItemName] = item;
        }

        /// <summary>
        /// Remove the specified content item
        /// </summary>
        /// <param name="item"></param>
        public virtual void RemoveContentItem(ContentItem item)
        {
            DeleteContentItem(item);

            if (Items.ContainsKey(item.ItemName))
            {
                Items.Remove(item.ItemName);
            }
        }

        /// <summary>
        /// Store the specified content item in the folder
        /// </summary>
        /// <param name="item"></param>
        protected abstract void StoreContentItem(ContentItem item);

        /// <summary>
        /// Delete the specified content item
        /// </summary>
        /// <param name="item"></param>
        protected abstract void DeleteContentItem(ContentItem item);

        /// <summary>
        /// Load the child items
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, ContentItem> LoadItems();

        /// <summary>
        /// Load the content folders
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<string, ContentFolder> LoadFolders();
    }
}
