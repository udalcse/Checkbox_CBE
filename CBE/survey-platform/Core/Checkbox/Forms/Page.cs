using System;
using System.ComponentModel;
using System.Collections.Generic;

using Checkbox.Forms.Items;

namespace Checkbox.Forms
{
    /// <summary>
    /// Provides access to an ordered view of <see cref="Item"/>s within a <see cref="Response"/>
    /// </summary>
    [Serializable]
    public abstract class Page
    {
        [NonSerialized]
        private EventHandlerList _eventHandlers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="templatePageID"></param>
        /// <param name="position"></param>
        protected Page(Int32 templatePageID, Int32 position)
        {
            PageId = templatePageID;
            Position = position;
            ItemIDs = new List<int>();
        }

        /// <summary>
        /// Get the page id
        /// </summary>
        public Int32 PageId { get; private set; }

        /// <summary>
        /// Get the page position
        /// </summary>
        public Int32 Position { get; private set; }

        /// <summary>
        /// Get the list of item ids
        /// </summary>
        public List<int> ItemIDs { get; private set; }

        /// <summary>
        /// Specify whether page contains an item with the specified id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool ContainsItem(int itemId)
        {
            return ItemIDs.Contains(itemId);
        }

        /// <summary>
        /// Add an item to the page
        /// </summary>
        /// <param name="itemID"></param>
        internal virtual void AddItemID(Int32 itemID)
        {
            ItemIDs.Add(itemID);
        }

        /// <summary>
        /// Get the list of items
        /// </summary>
        public abstract List<Item> Items { get; }

        /// <summary>
        /// Cheat for decreasing size of the binary serialized instance
        /// </summary>
        /// <returns></returns>
        public List<Item> GetItems()
        {
            return Items;
        }

        /// <summary>
        /// List of event handlers
        /// </summary>
        /// <remarks>Auto property not used because the backing field needs to be
        /// marked NonSerialized</remarks>
        protected EventHandlerList EventHandlers
        {
            get { return _eventHandlers; }
            set { _eventHandlers = value; }
        }
    }
}
