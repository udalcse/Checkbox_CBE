using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Security;

namespace Checkbox.Pagination
{
    /// <summary>
    /// An encapsulation of properties that are common to all paged/filtered/sorted lists of items.
    /// </summary>
    public class PaginationContext
    {
        ///<summary>
        /// Filter by start date
        ///</summary>
        public DateTime? StartDate { set; get; }

        ///<summary>
        /// Filter by end date
        ///</summary>
        public DateTime? EndDate { set; get; }

        ///<summary>
        /// Filter by event
        ///</summary>
        public string DateFieldName { set; get; }

        /// <summary>
        /// The index of the current page being displayed in the grid.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The property/column that grid data is filtered by.
        /// </summary>
        public string FilterField { get; set; }

        /// <summary>
        /// The value to filter grid data by.
        /// </summary>
        public string FilterValue { get; set; }

        /// <summary>
        /// Indicates if the content of the grid is filtered.
        /// </summary>
        public bool IsFiltered
        {
            get { return Utilities.IsNotNullOrEmpty(FilterField) && Utilities.IsNotNullOrEmpty(FilterValue); }
        }

        /// <summary>
        /// Indicates if the content of the grid is sorted.
        /// </summary>
        public bool IsSorted
        {
            get { return Utilities.IsNotNullOrEmpty(SortField); }
        }

        /// <summary>
        /// The total number of items that fulfill the filter and permission requirements.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// The minimum permissions a user must have in order to view an item in the grid.
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// The maximum number of items which can be displayed on a page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Determines if grid data is sorted in ascending or descending order.
        /// </summary>
        public bool SortAscending { get; set; }

        /// <summary>
        /// The property/column that grid data is sorted by.
        /// </summary>
        public string SortField { get; set; }

        /// <summary>
        /// Get/set how permissions should be joined.
        /// </summary>
        public PermissionJoin PermissionJoin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PaginationContext()
        {
            CurrentPage = -1;
            PageSize = -1;
            Permissions = new List<string>();
            FilterField = string.Empty;
            FilterValue = string.Empty;
            SortAscending = true;
            SortField = string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetStartIndex()
        {
            if (PageSize > 0 && CurrentPage > 0)
                return (CurrentPage - 1) * PageSize;

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public int GetEndIndex(int rowCount)
        {
            if (PageSize > 0 && CurrentPage > 0)
            {
                int startIndex = (CurrentPage - 1) * PageSize;
                    return startIndex + PageSize - 1;
            }

            return rowCount - 1;
        }
    }
}