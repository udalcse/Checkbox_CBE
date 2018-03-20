using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Analytics.Configuration;
using Checkbox.Analytics.Items;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Charts;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Analytics.Items.UI;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class AddItem : SecuredPage
    {
        private AnalysisTemplate _reportTemplate;
        private List<string> _selectedItemTypes;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("r")]
        public int ReportId { get; set; }

        [QueryParameter("p")]
        public int? PageId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string WarningMessage
        {
            get
            {
                int maxCount = ((ReportPerformanceConfiguration)
                    Prezza.Framework.Configuration.ConfigurationManager.
                        GetConfiguration("checkboxReportPerformanceConfiguration")).
                            MaxResponseCountForUserInputItemsVisibility;

                return string.Format(
                    TextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/maxResponseCount"),
                        maxCount);
            }
        }

        /// <summary>
        /// Get list of selected item types
        /// </summary>
        private List<string> SelectedItemTypes
        {
            get
            {
                if (_selectedItemTypes == null)
                {
                    _selectedItemTypes = new List<string>();

                    if (Utilities.IsNotNullOrEmpty(_itemList.SelectedItemType))
                    {
                        _selectedItemTypes.Add(_itemList.SelectedItemType);
                    }
                }

                return _selectedItemTypes;
            }
        }

        /// <summary>
        /// Get position of selected page
        /// </summary>
        private TemplatePage SelectedPage
        {
            get
            {
                return ReportTemplate.GetPage(PageId.Value);
            }
        }

        /// <summary>
        /// Get position for item
        /// </summary>
        private int SelectedItemPosition
        {
            get
            {
                int position;

                if (int.TryParse(_itemPositionList.SelectedValue, out position))
                {
                    return position;
                }

                return -1;
            }
        }

        /// <summary>
        /// Get response template to add items to
        /// </summary>
        public AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_reportTemplate == null && ReportId > 0)
                {
                    _reportTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId);
                }

                return _reportTemplate;
            }
        }

        /// <summary>
        /// Edit language
        /// </summary>
        private string EditLanguage { get; set; }

        /// <summary>
        /// Check if the response template contains at least one matrix item
        /// </summary>
        /// <returns></returns>
        private bool IsResponseTemplateContainsMatrixItem()
        {
            var response = ResponseTemplateManager.GetResponseTemplate(ReportTemplate.ResponseTemplateID);

            foreach (var itemId in response.ListTemplateItemIds())
            {
                var typeInfo = ItemConfigurationManager.GetItemTypeInfo(itemId);
                if (typeInfo.TypeName.Equals("Matrix"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Bind event handlers
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!IsResponseTemplateContainsMatrixItem())
                _itemList.TypesToExclude = "MatrixSummary";

            _itemList.DefaultCategory = "Report";
            _wizard.NextButtonClick += _wizard_NextButtonClick;
            _wizard.FinishButtonClick += _wizard_FinishButtonClick;
            _wizard.CancelButtonClick += _wizard_CancelButtonClick;

            RegisterClientScriptInclude(
              "DialogHandler",
              ResolveUrl("~/Resources/DialogHandler.js"));

            //Bind completion button clicks
            _restartButton.Click += _restartButton_Click;
            _editItemButton.Click += _editItemButton_Click;
            _closeWizardButton.Click += _closeItemButton_Click;

            var lightWeightRt = ResponseTemplateManager.GetLightweightResponseTemplate(ReportTemplate.ResponseTemplateID);

            EditLanguage = lightWeightRt.DefaultLanguage;

            if (string.IsNullOrEmpty(EditLanguage))
            {
                EditLanguage = TextManager.DefaultLanguage;
            }

            //Hide default buttons & set title
            Master.HideDialogButtons();
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/addItemToReport"));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_CancelButtonClick(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeWindow", "closeWindow();", true);
        }

        /// <summary>
        /// Update page options on wizard next click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Populate list of positions for new item
            EventHandlerWrapper(PopulateItemPositionOptions);

            _maxResponseCountWarningPanel.Visible =
                !AnalysisTemplateManager.ShowUserInputItemsForReport(ReportTemplate.ResponseTemplateID) &&
                string.Equals(_itemList.SelectedItemType, "Details", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Handle finish click to populate completion page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Populate messages on complete step
            EventHandlerWrapper(AddItemsToSurvey);
            EventHandlerWrapper(PopulateCompletionStep);
        }


        /// <summary>
        /// Populate options for item positions.
        /// </summary>
        private void PopulateItemPositionOptions()
        {
            _itemPositionList.Items.Clear();

            //Add "First Item" Option
            _itemPositionList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/firstItem"), "1"));

            if (SelectedPage == null)
            {
                //Page not found, show an error
                throw new Exception("Unable to load selected page.");
            }

            //Now list items
            int[] pageItemIds = SelectedPage.ListItemIds();
            int positionCount = 1;

            string afterItemText = WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/afterItem", WebTextManager.GetUserLanguage(), "After item {0} - {1}");

            foreach (int itemId in pageItemIds)
            {
                ItemData itemData = ReportTemplate.GetItem(itemId);

                if (itemData != null)
                {
                    string itemText = Utilities.StripHtml(
                        ItemConfigurationManager.GetItemText(itemData.ID.Value, EditLanguage, null, false, false),
                        64);

                    _itemPositionList.Items.Add(new ListItem(
                        string.Format(afterItemText, positionCount, itemText),
                        (positionCount + 1).ToString()));       //Add 1 to position count since we want to add item "after"

                    positionCount++;
                }
            }

            //Add "Last Item" option
            _itemPositionList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/lastItem"),
                (positionCount + 1).ToString()));

            //Make "last item" default option
            _itemPositionList.SelectedIndex = _itemPositionList.Items.Count - 1;
        }

        /// <summary>
        /// Populate values on completion page.
        /// </summary>
        private void PopulateCompletionStep()
        {
            if (SelectedPage == null)
            {
                //Page not found, show an error
                throw new Exception("Unable to load selected page.");
            }

            //Wrap all text manipulation in try...catch so that borked text lacking necessary tokens
            // for string formatting won't cause an add item fail.
            try
            {
                //Not allow to return to the previous step
                _locationStep.AllowReturn = false;

                //Figure out page, position, and base summary text
                string summaryText = String.Empty;
                string selectedItemPositionText;

                string selectedPageText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/page") + SelectedPage.Position;

                //Determine text for item position
                if (_itemPositionList.SelectedIndex == 0)
                {
                    selectedItemPositionText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/firstItemAlt");
                }
                else if (_itemPositionList.SelectedIndex == (_itemPositionList.Items.Count - 1))
                {
                    selectedItemPositionText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/lastItemAlt");
                }
                else
                {
                    selectedItemPositionText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/afterItemAlt");

                    int[] pageItemIds = SelectedPage.ListItemIds();

                    //Items positions are 1-index based
                    if (pageItemIds.Length >= SelectedItemPosition
                        && SelectedItemPosition > 0)
                    {
                        int itemId = pageItemIds[SelectedItemPosition - 1];

                        selectedItemPositionText = string.Format(
                            selectedItemPositionText,
                            SelectedItemPosition,
                            Utilities.StripHtml(ItemConfigurationManager.GetItemText(itemId, EditLanguage, null, false, false), 64));
                    }
                }

                //Determine item type/count text
                //Customize text for single item added or multiple items added
                if (SelectedItemTypes.Count > 1)
                {
                    //The selected <b>{0}</b> items were added to {1} {2}.
                    summaryText = WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/addMultipleItemSummary");
                    summaryText = string.Format(
                        summaryText,
                        SelectedItemTypes.Count,
                        selectedPageText,
                        selectedItemPositionText,
                        string.Empty);
                }
                else
                {
                    //If adding single item, go directly to item editor
                    _editItemButton_Click(this, new EventArgs());

                    /* Keep around text update just incase behavior should be restored
                     * 
                    //The new <b>{0}</b> item was added to {1} {2}.
                    summaryText = WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/addSingleNewItemSummary");
                    string itemTypeName = WebTextManager.GetText("/itemType/" + SelectedItemTypes[0] + "/name");

                    summaryText = string.Format(
                        summaryText,
                        itemTypeName,
                        selectedPageText,
                        selectedItemPositionText,
                        WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/clickToEdit"));
                    */

                }


                //Show/hide edit item button
                _editItemButton.Visible = SelectedItemTypes.Count == 1;

                //Set summary text
                _addedItemSummaryLabel.Text = summaryText;
            }
            catch (Exception ex)
            {
                //Log error
                ExceptionPolicy.HandleException(ex, "UIProcess");

                //Set default text
                _addedItemSummaryLabel.Text = "New item(s) were added to the survey.";
            }
        }

        /// <summary>
        /// ID of the item that has been created during this opened window session
        /// </summary>
        public int? ItemID
        {
            get
            {
                return ViewState["ItemID"] == null ? null : (int?)(int)ViewState["ItemID"];
            }
            set
            {
                if (value == null)
                    ViewState.Remove("ItemID");
                else
                    ViewState["ItemID"] = value;
            }
        }

        /// <summary>
        /// ID of the page that has been created during this opened window session
        /// </summary>
        public int? PageID
        {
            get
            {
                return ViewState["PageID"] == null ? null : (int?)(int)ViewState["PageID"];
            }
            set
            {
                if (value == null)
                    ViewState.Remove("PageID");
                else
                    ViewState["PageID"] = value;
            }
        }


        /// <summary>
        /// Add new/imported/copied item(s) to survey.
        /// </summary>
        private void AddItemsToSurvey()
        {
            //Item editor only makes sense when working with a single item
            if (SelectedItemTypes.Count != 1)
            {
                return;
            }

            ItemData itemData = ItemConfigurationManager.CreateConfigurationData(SelectedItemTypes[0]);

            if (itemData != null)
            {
                //Set some default options
                SetItemDataDefaults(itemData);
                itemData.CreatedBy = User.Identity.Name;
                itemData.Save();

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);

                // Change graph type
                if (SelectedItemTypes[0] == "SummaryChart")
                    appearanceData.SetPropertyValue("GraphType", _itemList.ChartType);

                if (SelectedItemTypes[0] == "Frequency")
                    appearanceData.SetPropertyValue("OptionsOrder", OptionsOrder.Survey.ToString());

                if (_itemList.ChartType == "PieGraph" || _itemList.ChartType == "Doughnut")
                    appearanceData.SetPropertyValue("ShowLegend", "True");

                //Make for Doughnut chart bigger margin between legend and chart
                if (_itemList.ChartType == "Doughnut")
                {
                    appearanceData.SetPropertyValue("ChartMarginBottom", "100");
                }

                //Set default options for the appearance
                appearanceData.SetDefaults();

                if (appearanceData != null)
                {
                    appearanceData.Save(itemData.ID.Value);
                }

                _editItemButton.CommandArgument = itemData.ID.ToString();

                ReportTemplate.AddItemToPage(SelectedPage.ID.Value, itemData.ID.Value, SelectedItemPosition);
                ReportTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ReportTemplate.Save();

                ItemID = itemData.ID;
                PageID = SelectedPage.ID;
            }
        }
        
        /// <summary>
        /// Set some default options for new item
        /// </summary>
        /// <param name="itemData"></param>
        private void SetItemDataDefaults(ItemData itemData)
        {
            if (itemData is StatisticsItemData)
            {
                ((StatisticsItemData) itemData).ReportOption = StatisticsItemReportingOption.All;
            }
 
            if (itemData is DetailsItemData)
            {
                ((DetailsItemData)itemData).GroupAnswers = true;
                ((DetailsItemData)itemData).LinkToResponseDetails = true;
            }
       }

        /// <summary>
        /// Edit the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _editItemButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("EditItem.aspx?isNew=true&i={0}&p={1}&r={2}", _editItemButton.CommandArgument, SelectedPage.ID, ReportId), false);
        }

        /// <summary>
        /// Close button handler. Returns the ID of the added item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _closeItemButton_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("op", "addItem");
            args.Add("result", "ok");
            if (PageID > 0)
            {
                args.Add("pageId", PageID.ToString());
            }
            if (ItemID > 0)
            {
                args.Add("itemId", ItemID.ToString());
            }

            Master.CloseDialog(args);
        }

        /// <summary>
        /// Restart the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _restartButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddItem.aspx?r=" + ReportId, false);
        }
    }
}
