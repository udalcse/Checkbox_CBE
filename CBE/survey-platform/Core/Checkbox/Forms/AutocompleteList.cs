using System;
using System.Collections.Generic;

namespace Checkbox.Forms
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AutocompleteList
    {
        /// <summary>
        /// 
        /// </summary>
        public int ListId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Items { set; get; }
    }
}
