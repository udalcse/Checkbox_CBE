using System;
using Checkbox.Common;

namespace Checkbox.Forms.ResponseStateEntities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResponseLogEntity : EntityBase
    {
        public ResponseLogEntity(int id)
        {
        }

        public ResponseLogEntity()
        {
        }

        public long? PageLogID { set; get; }
        public long ResponseID { set; get; }
        public int PageID { set; get; }
        public DateTime? PageStartTime { set; get; }
        public DateTime? PageEndTime { set; get; }
    }
}
