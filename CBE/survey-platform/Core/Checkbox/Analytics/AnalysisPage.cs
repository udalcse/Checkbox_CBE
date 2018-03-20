using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Forms;
using Checkbox.Forms.Items;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Run time representation of a group of items (page) in a report.
    /// </summary>
    [Serializable]
    public class AnalysisPage : Page
    {
        /// <summary>
        /// Construct an AnalysisPage object.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <param name="layoutTemplateId"></param>
        internal AnalysisPage(Int32 pageId, Int32 position, int? layoutTemplateId) : base(pageId, position)
        {
            LayoutTemplateId = layoutTemplateId;
        }

        /// <summary>
        /// 
        /// </summary>
        internal Analysis Parent { get; set; }

        /// <summary>
        /// Get the id of the page layout template used by this page.
        /// </summary>
        public int? LayoutTemplateId { get; private set; }

        /// <summary>
        /// Get the list of items on the page in order of appearance.  If randomization
        /// is on, the list will be randomized.
        /// </summary>
        public override List<Item> Items
        {
            get
            {
                return ItemIDs.Select(itemID => Parent.GetItem(itemID)).Where(theItem => theItem != null && theItem.IsActive).ToList();
            }
        }
    }
}