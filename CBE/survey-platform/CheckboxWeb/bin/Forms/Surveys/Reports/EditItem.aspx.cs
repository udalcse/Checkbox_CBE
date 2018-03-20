using System;
using System.Collections.Generic;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class EditItem : SecuredPage
    {
        private AnalysisTemplate _reportTemplate;

        [QueryParameter("i", IsRequired = true)]
        public int ItemId { get; set; }

        [QueryParameter("r", IsRequired = true)]
        public int ReportId { get; set; }

        [QueryParameter("p", IsRequired = false)]
        public int? PageID { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }

        /// <summary>
        /// Edit language
        /// </summary>
        private string EditLanguage { get; set; }

        /// <summary>
        /// Require analysis administer permission to create a report
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Analysis.Administer"; }
        }
        
        /// <summary>
        /// Get response template being edited
        /// </summary>
        public AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_reportTemplate == null)
                {
                    _reportTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId);

                    if (_reportTemplate == null)
                    {
                        throw new Exception(string.Format("Unable to load report with id {0}.", ReportId));
                    }
                }

                return _reportTemplate;
            }
        }

        /// <summary>
        /// Get/set data
        /// </summary>
        private ItemData Data { get; set; }

        /// <summary>
        /// Get/set appearance
        /// </summary>
        private AppearanceData Appearance { get; set; }

        /// <summary>
        /// Bind item to editor.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Data = ReportTemplate.GetItem(ItemId);

            if (Data == null)
            {
                throw new Exception(string.Format("Unable to load item with id {0}.", ItemId));
            }

            var lightWeightRt = ResponseTemplateManager.GetLightweightResponseTemplate(ReportTemplate.ResponseTemplateID);

            EditLanguage = lightWeightRt.DefaultLanguage;

            if (string.IsNullOrEmpty(EditLanguage))
            {
                EditLanguage = TextManager.DefaultLanguage;
            }

            //Not all items have appearances, so it's ok for appearance data to be null
            Appearance = AppearanceDataManager.GetAppearanceDataForItem(ItemId);

            //Initialize component on initial view of page.
            _itemEditor.Initialize(
                ReportId,
                -1,
                Data,
                Appearance,
                EditLanguage,
                Page.IsPostBack,
                IsNew.HasValue && IsNew.Value);

            //Item already has been added -- refresh of the item list is needed
            if (IsNew.HasValue && IsNew.Value)
            {
                Master.CancelClick += _cancelButton_Click;

                //Update page/item view if just added item
                if (!IsPostBack)
                {
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "refreshEditor",
                        "onItemAdded(" + ItemId + "," + PageID + ");",
                        true);
                }
            }

            //Localization for datepicker
            RegisterClientScriptInclude(
                "jquery.localize.js",
                ResolveUrl("~/Resources/jquery-ui-timepicker-addon.js"));

            LoadDatePickerLocalized();

            //Bind click events
            Master.OkClick += _saveButton_Click;

            var itemTypeName = WebTextManager.GetText("/itemType/" + Data.ItemTypeName + "/name");

            if (string.IsNullOrEmpty(itemTypeName))
            {
                itemTypeName = Data.ItemTypeName;
            }

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/reports/editItem.aspx/editReportItem") + " - " + itemTypeName);
            Master.IsDialog = IsNew ?? false;
        }

        /// <summary>
        /// Handle save click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _saveButton_Click(object sender, EventArgs e)
        {
            if (_itemEditor.Validate())
            {
                //Update data
                _itemEditor.UpdateData(IsNew.HasValue && IsNew.Value);

                //Save item & appearance data
                _itemEditor.SaveData();

                //Update template
                ReportTemplate.SetItem(_itemEditor.ItemData, true);
                ReportTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ReportTemplate.Save();

                //Cleanup appearance caches of the item
                AppearanceDataManager.CleanUpAppearanceDataCacheForItem(ItemId);

                //Fire event on finish
                var isNewArg = IsNew.HasValue && IsNew.Value
                    ? "true"
                    : "false";

                ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "okClick(" + ItemId + "," + PageID + ", " + isNewArg + ");", true);
            }
        }

        /// <summary>
        /// Handle cancel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cancelButton_Click(object sender, EventArgs e)
        {
            //Fire event on finish
            var isNewArg = IsNew.HasValue && IsNew.Value
                ? "true"
                : "false";

            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "okClick(" + ItemId + "," + PageID + ", " + isNewArg + ");", true);
        }

    }
}
