using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Content
{
    /// <summary>
    /// Abstract content item base class
    /// </summary>
    public abstract class ContentItem
    {
        private byte[] _itemData;

        /// <summary>
        /// Get/set the item name
        /// </summary>
        public virtual string ItemName { get; set; }

        /// <summary>
        /// Get/set item url
        /// </summary>
        public virtual string ItemUrl { get; set; }

        /// <summary>
        /// Get/set item type
        /// </summary>
        public virtual string ContentType { get; set; }

        /// <summary>
        /// Get/set is public
        /// </summary>
        public virtual bool IsPublic { get; set; }

        /// <summary>
        /// Get/set last updated
        /// </summary>
        public virtual DateTime LastUpdated { get; set; }

        /// <summary>
        /// Get/set data
        /// </summary>
        public virtual byte[] Data
        {
            get { return _itemData ?? (_itemData = LoadItemData()); }
            set { _itemData = value; }
        }

        /// <summary>
        /// Load the item data
        /// </summary>
        protected abstract byte[] LoadItemData();
    }
}
