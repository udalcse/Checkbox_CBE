using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class SourceItemFilter : Filter, IQueryFilter
    {
        private IEnumerable<int> _sourceItemIds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceItemIds"></param>
        public SourceItemFilter(IEnumerable<int> sourceItemIds )
        {
            _sourceItemIds = sourceItemIds;
        }


        /// <summary>
        /// 
        /// </summary>
        public string FilterString
        {
            get { var sb = new StringBuilder();
                sb.Append("ItemId in (");

                var counter = 0;

                foreach (var itemId in _sourceItemIds)
                {
                    if (counter > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(itemId);
                    
                    counter++;
                }

                sb.Append(")");

                return sb.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UseNotIn
        {
            get { return false; }
        }
    }
}
