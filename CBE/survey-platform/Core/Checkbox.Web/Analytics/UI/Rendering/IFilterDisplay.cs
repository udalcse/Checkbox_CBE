using System.Collections.Generic;
using Checkbox.Analytics.Filters.Configuration;

namespace Checkbox.Web.Analytics.UI.Rendering
{
    /// <summary>
    /// Interface for displaying text representation of list of filters
    /// </summary>
    public interface IFilterDisplay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="languageCode"></param>
        void InitializeAndBind(List<FilterData> filters, string languageCode);
    }
}
