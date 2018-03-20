using System;

namespace Checkbox.Forms.Data
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BasicItemMetaData
    {
        public int ItemId { set; get; }
        public string ItemText { set; get; }
        public int PagePosition { set; get; }
        public int? ParentId { set; get; }
        public int? RowNumber { set; get; }
    }
}
