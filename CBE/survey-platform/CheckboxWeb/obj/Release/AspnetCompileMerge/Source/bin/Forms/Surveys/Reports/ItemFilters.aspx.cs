using System.Collections.Generic;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ItemFilters : SecuredPage
    {

        private AnalysisItemData _itemData;
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get/set item id
        /// </summary>
        [QueryParameter("i", IsRequired = true)]
        public int? ItemId { get; set; }

        [QueryParameter("p")]
        public int? PageId { get; set; }

        /// <summary>
        /// Require analysis administer permission to create a report
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Analysis.Administer"; }
        }
        
        /// <summary>
        /// Get survey
        /// </summary>
        private AnalysisItemData ItemData
        {
            get
            {
                if (_itemData == null
                    && ItemId.HasValue)
                {
                    _itemData = ItemConfigurationManager.GetConfigurationData(ItemId.Value) as AnalysisItemData;
                }

                return _itemData;
            }
        }

        /// <summary>
        /// Get a reference to response template associated with report item
        /// </summary>
        private ResponseTemplate SourceTemplate
        {
            get
            {
                if(_responseTemplate == null
                    && ItemData != null
                    && ItemData.ResponseTemplateIds.Count > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ItemData.ResponseTemplateIds[0]);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Bind filter selector
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            _saveButton.Click += _saveButtton_Click;

            if (ItemData != null && SourceTemplate != null)
            {
                _filterSelector.Initialize(ItemData.Filters, SourceTemplate.GetFilterDataObjects(), SourceTemplate.LanguageSettings.DefaultLanguage, Page.IsPostBack);
            }
        }

        /// <summary>
        /// Update filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _saveButtton_Click(object sender, System.EventArgs e)
        {
            if (ItemData != null)
            {
                ItemData.ClearFilters();

                List<FilterData> appliedFilters = _filterSelector.GetAppliedFilters();

                foreach (FilterData appliedFilter in appliedFilters)
                {
                    ItemData.AddFilter(appliedFilter);
                }

                ItemData.SaveFilters();
                ItemData.ModifiedBy = User.Identity.Name;
                ItemData.Save();

                Page.ClientScript.RegisterClientScriptBlock(
                    GetType(),
                    "CloseWindow",
                    "closeWindowAndRefreshParentPage('')",
                    true);
            }
        }
    }
}
