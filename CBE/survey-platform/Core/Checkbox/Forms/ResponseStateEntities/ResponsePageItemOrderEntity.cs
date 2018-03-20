using System;
using Checkbox.Common;

namespace Checkbox.Forms.ResponseStateEntities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResponsePageItemOrderEntity : EntityBase
    {
        public ResponsePageItemOrderEntity(int id)
        {
        }

        public ResponsePageItemOrderEntity()
        {
        }

        public long ResponseID { set; get; }
        public int? PageID { set; get; }
        public int? ItemID { set; get; }
        public int? Position { set; get; }
    }
}
