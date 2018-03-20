using System.Collections.Generic;
using Checkbox.Analytics.Filters.Configuration;

namespace Checkbox.Web.Analytics.UI.Editing
{
    /// <summary>
    /// Interface definition for filter selector control
    /// </summary>
    public interface IFilterSelector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliedFilters"></param>
        /// <param name="allFilters"></param>
        /// <param name="languageCode"></param>
        /// <param name="pageIsPostBack"></param>
        void Initialize(List<FilterData> appliedFilters, List<FilterData> allFilters, string languageCode, bool pageIsPostBack);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<FilterData> GetAppliedFilters();
    }
}
