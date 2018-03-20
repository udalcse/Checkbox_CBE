using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// IQueryFilter objects are capable of providing filter strings suitable for use in a "where ResponseID in" or "not in" clause.
    /// </summary>
    interface IQueryFilter
    {
        /// <summary>
        /// Get the query filter clause
        /// </summary>
        string FilterString { get;}

        /// <summary>
        /// Specify whether to join this filter's clause to other clauses with IN or NOT IN
        /// </summary>
        bool UseNotIn { get;}
    }
}
