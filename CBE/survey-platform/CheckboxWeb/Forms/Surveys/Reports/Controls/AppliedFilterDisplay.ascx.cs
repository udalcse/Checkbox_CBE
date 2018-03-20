using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Web.Analytics.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AppliedFilterDisplay : Checkbox.Web.Common.UserControlBase, IFilterDisplay
    {
        /// <summary>
        /// 
        /// </summary>
        public Unit Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliedFilters"></param>
        /// <param name="languageCode"></param>
        public void InitializeAndBind(List<FilterData> appliedFilters, string languageCode)
        {
            LanguageCode = languageCode;
            _filterListView.DataSource = appliedFilters;
            _filterListView.DataBind();

            _viewPanel.Width = Width;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appliedFilters"></param>
        public void InitializeAndBind(string[] appliedFilters)
        {
            _filterListView.DataSource = appliedFilters;
            _filterListView.DataBind();
            _viewPanel.Width = Width;
        }
    }
}