using System;
using Checkbox.Common;

namespace Checkbox.Forms.ResponseStateEntities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResponseItemOptionOrderEntity : EntityBase
    {
        public ResponseItemOptionOrderEntity(int id)
        {
        }

        public ResponseItemOptionOrderEntity()
        {
        }

        public long ResponseID { set; get; }
        public int? ItemID { set; get; }
        public int? OptionID { set; get; }
        public int? Position { set; get; }
    }
}
