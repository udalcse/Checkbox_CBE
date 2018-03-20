using System.Text;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Item filter that supports querying in the database for more efficient querying
    /// </summary>
    public abstract class ItemQueryFilter : ItemFilter, IQueryFilter
    {
        #region IQueryFilter Members

        /// <summary>
        /// Get the item filter string
        /// </summary>
        public string FilterString
        {
            get 
            {
                return GetFilterString();
            }
        }

        /// <summary>
        /// Get the value as an escaped string
        /// </summary>
        /// <returns></returns>
        protected virtual string GetEscapedValueString()
        {
            if (Value != null)
            {
                return Value.ToString().Replace("'", "''");
            }

            return string.Empty;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilterString()
        {
            StringBuilder sb = new StringBuilder();

            //Add the item id clause
            sb.Append("ItemID = ");
            sb.Append(ItemID);
            sb.Append(" AND (");
            sb.Append(GetValueFilterClause());
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Get the filter clause for the value portion
        /// </summary>
        /// <returns></returns>
        protected abstract string GetValueFilterClause();
        
        /// <summary>
        /// Get whether to use not-in syntax
        /// </summary>
        public abstract bool UseNotIn{get;}

        #endregion
    }
}
