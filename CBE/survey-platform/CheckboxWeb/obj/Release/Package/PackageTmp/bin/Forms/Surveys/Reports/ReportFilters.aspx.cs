using System.Collections.Generic;
using Checkbox.Analytics;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// Report filter editor
    /// </summary>
    public partial class ReportFilters : SecuredPage
    {
        private AnalysisTemplate _reportTemplate;

        /// <summary>
        /// Get/set survey id
        /// </summary>
        [QueryParameter("r", IsRequired = true)]
        public int? ReportId { get; set; }

        /// <summary>
        /// Get survey
        /// </summary>
        private AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_reportTemplate == null
                    && ReportId.HasValue)
                {
                    _reportTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);
                }

                return _reportTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// Bind filter selector
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            Master.OkClick += new System.EventHandler(Master_OkClick);

            if (ReportTemplate != null)
            {
                var sourceTemplate = ResponseTemplateManager.GetResponseTemplate(ReportTemplate.ResponseTemplateID);

                if (sourceTemplate != null)
                {
                    _filterSelector.Initialize(ReportTemplate.GetFilterDataObjects(), sourceTemplate.GetFilterDataObjects(), "en-US", Page.IsPostBack);
                }
            }
        }

        /// <summary>
        /// Update filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, System.EventArgs e)
        {
            if (ReportTemplate != null)
            {
                ReportTemplate.ClearFilters();

                var appliedFilters = _filterSelector.GetAppliedFilters();

                foreach (var appliedFilter in appliedFilters)
                {
                    ReportTemplate.AddFilter(appliedFilter);
                }

                //cleanup the cache
                AnalysisTemplateManager.CleanupAnalysisTemplatesCache(ReportTemplate.ID.Value);

                ReportTemplate.SaveFilters();
                ReportTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ReportTemplate.Save();

                Master.CloseDialog(new Dictionary<string, string> { { "op", "properties" }, { "reportId", ReportTemplate.ID.ToString() } });
            }

        }

    }
}
