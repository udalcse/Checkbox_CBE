using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Analytics.Items;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Add item to survey
    /// </summary>
    public partial class AddItem : SecuredPage
    {
        private const int LAST_ITEM_INDEX = -1;
        private const int FIRST_ITEM_POSITION = 1;
        
        private ResponseTemplate _responseTemplate;
        private List<string> _selectedItemTypes;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("p")]
        public int? PageId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(LanguageCode)
                           ? LanguageCode
                           : ResponseTemplate.LanguageSettings.DefaultLanguage;
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

                    if (_currentTabTxt.Text == "0"
                        && Utilities.IsNotNullOrEmpty(_itemList.SelectedItemType))
                    {
                        _selectedItemTypes.Add(_itemList.SelectedItemType);
                    }
                }

                return _selectedItemTypes;
            }
            set { _selectedItemTypes = value; }
        }

        /// <summary>
        /// Get position of selected page
        /// </summary>
        private TemplatePage SelectedPage
        {
            get
            {
                //int id;

                //if (int.TryParse(_pageList.SelectedValue, out id))
                return ResponseTemplate.GetPage(PageId.Value);

                //return null;
            }
        }

        /// <summary>
        /// Determine is import succeded
        /// </summary>
        private bool IsImportSucceeded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private bool HiddenItemImportError { get; set; }

        /// <summary>
        /// Get response template to add items to
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null && ResponseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool AllowLibraries
        {
            get
            {
                return !ApplicationManager.AppSettings.EnableMultiDatabase ||
                       ApplicationManager.AppSettings.AllowLibraries;
            }
        }

        /// <summary>
        /// Bind event handlers
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            RegisterClientScriptInclude(
                "DialogHandler",
                ResolveUrl("~/Resources/DialogHandler.js"));


            //TODO: Handle No Template

            //if (_selectedIndex.Value < 0)
            //{
            //    _tabList.SelectedIndex = 0;
            //}

            //Bind completion button clicks
            _restartButton.Click += _restartButton_Click;
            _editItemButton.Click += _editItemButton_Click;
            _closeWizardButton.Click += _closeWizardButton_Click;

            _itemListFromLibrary.Visible = AllowLibraries;

            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/addItemToSurvey");
            Master.HideDialogButtons();

            switch (SelectedPage.PageType)
            {
                case TemplatePageType.Completion:
                    _itemList.AllowedCategories = "Display,Action";
                    _itemList.DefaultCategory = "Display";
                    _itemList.AllowedItems =
                        "Html,Message,Image,HorizontalLine,EmailResponse,DisplayResponse,Email,ProfileUpdater,Redirect,DisplayAnalysis,CloseWindow";
                    if (_responseTemplate.BehaviorSettings.EnableScoring)
                        _itemList.AllowedItems += ",CurrentScore,ScoreMessage";
                    _itemList.IncludeHidden = false;
                    break;

                default:
                    _itemList.AllowedCategories = "Display,Action,Question";
                    _itemList.DefaultCategory = "Question";
                    _itemList.IncludeHidden = false;
                    break;
            }
            _itemList.IncludeScoreDependable = ResponseTemplate.BehaviorSettings.EnableScoring;

            //if page is hidden, add hidden item by default
            if (SelectedPage.PageType == TemplatePageType.HiddenItems)
            {
                SelectedItemTypes.Add("HiddenItem");
                _wizard_FinishButtonClick(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            _itemListFromLibrary.LanguageCode = EditLanguage;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _wizard_CancelButtonClick(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeWindow", "closeWindow();", true);
        }

        /// <summary>
        /// Handles Close button click to close dialog and transfer added ItemId
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _closeWizardButton_Click(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>();
            args.Add("op", "addItem");
            args.Add("result", "ok");
            args.Add("pageId", PageId.ToString());
            if (ItemID > 0)
            {
                args.Add("itemId", ItemID.ToString());
            }

            Master.CloseDialog(args);
        }

        /// <summary>
        /// Handle finish click to populate completion page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Populate messages on complete step
            EventHandlerWrapper(AddItemsToSurvey);
            EventHandlerWrapper(PopulateCompletionStep);
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
                //Figure out page, position, and base summary text
                string summaryText = String.Empty;
                string selectedPageText;
                string selectedItemPositionText;
                int itemCount;

                //Determine text for page position
                if (SelectedPage.PageType == TemplatePageType.HiddenItems)
                {
                    selectedPageText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/hiddenItemsPage");
                }
                else if (SelectedPage.PageType == TemplatePageType.Completion)
                {
                    selectedPageText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/completionPage");
                }
                else
                {
                    selectedPageText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/page") +
                                       (SelectedPage.Position - 1);
                }

                selectedItemPositionText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/lastItemAlt");

                switch (_currentTabTxt.Text)
                {
                    case "0":
                        itemCount = SelectedItemTypes.Count;
                        break;
                    case "1":
                        itemCount = _itemListFromLibrary.SelectedItemIds.Count;
                        break;
                    case "2": //itemImport
                        itemCount = IsImportSucceeded ? 1 : 0;
                        break;
                    default:
                        itemCount = 0;
                        break;
                }


                if (itemCount > 1)
                {
                    //The selected <b>{0}</b> items were added to {1} {2}.
                    summaryText =
                        WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/addMultipleItemSummary");
                    summaryText = string.Format(
                        summaryText,
                        itemCount,
                        selectedPageText,
                        selectedItemPositionText,
                        string.Empty);
                }
                else if (itemCount == 1)
                {
                    //If adding single item, go directly to item editor
                    _editItemButton_Click(this, new EventArgs());

                    /* Keep around text update just incase behavior should be restored
                     * 
                    //The new <b>{0}</b> item was added to {1} {2}.
                    summaryText =
                        WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/addSingleNewItemSummary");
                    string itemTypeName = WebTextManager.GetText("/itemType/" + SelectedItemTypes[0] + "/name");

                    summaryText = string.Format(
                        summaryText,
                        itemTypeName,
                        selectedPageText,
                        selectedItemPositionText,
                        WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/clickToEdit"));
                     */
                }
                else
                {
                    if (HiddenItemImportError)
                        summaryText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/hiddenCannotBeImported");
                    else
                        summaryText = WebTextManager.GetText("/pageText/forms/surveys/addItem.aspx/noItemsSummary");
                }

                //Show/hide edit item button
                _editItemButton.Visible = itemCount == 1;

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
        /// Add new/imported/copied item(s) to survey.
        /// </summary>
        private void AddItemsToSurvey()
        {
            if (_currentTabTxt.Text == "0")
                AddItemOfSelectedType((CheckboxPrincipal)User);

            if (_currentTabTxt.Text == "1")
                AddItemsFromLibrary((CheckboxPrincipal)User);

            if (_currentTabTxt.Text == "2")
                AddItemFromXml((CheckboxPrincipal)User);
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddItemFromXml(CheckboxPrincipal principal)
        {
            var xmlDoc = _itemImport.GetUploadedFileDocument();

            try
            {
                if (xmlDoc == null)
                    return;

                var itemElement = xmlDoc.SelectSingleNode("Item");

                ItemData item = ItemConfigurationManager.ImportItem((CheckboxPrincipal)HttpContext.Current.User, itemElement);

                if (item == null)
                    return;

                if (!IsValidItemType(item))
                {
                    IsImportSucceeded = false;
                    return;
                }


                ResponseTemplate.AddItemToPage(SelectedPage.ID.Value, item.ID.Value, LAST_ITEM_INDEX);
                ResponseTemplate.ModifiedBy = principal.Identity.Name;
                ResponseTemplate.Save();

                ItemID = item.ID;

                ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

                _editItemButton.CommandArgument = ItemID.Value.ToString();
                IsImportSucceeded = true;
            }
            finally
            {
                Session.Remove("PostedXmlFile");
            }
        }

        /// <summary>
        /// Add all selected items from the selected library.
        /// </summary>
        private void AddItemsFromLibrary(CheckboxPrincipal principal)
        {
            if (!AllowLibraries)
                return;

            ReadOnlyCollection<int> selectedIds = _itemListFromLibrary.SelectedItemIds;

            int currentPosition = FIRST_ITEM_POSITION;
            int firstId = -1;

            //getting the last page element position and set up currentPosition field
            var currentPage = ResponseTemplate.TemplatePages.FirstOrDefault(item => item.Key.Equals(this.PageId));
            var pageIds = currentPage.Value.ListItemIds();

            if (pageIds.Any())
            {
                var finalElement = pageIds.Last();

                var itemPositoon = ResponseTemplate.GetItemPositionWithinPage(finalElement);
                //set next position for library element 
                if (itemPositoon.HasValue)
                    currentPosition = itemPositoon.Value + 1;
                
            }

            foreach (int selectedId in selectedIds)
            {
                ItemData toCopy = _itemListFromLibrary.CurrentLibrary.GetItem(selectedId);
                ItemData copy = ItemConfigurationManager.CopyItem(toCopy, (CheckboxPrincipal)HttpContext.Current.User);

                if (copy == null)
                    continue;

                if (!IsValidItemType(copy))
                {
                    _itemListFromLibrary.RemoveItemFromSelectedItems(selectedId);
                    continue;
                }

                if (firstId == -1)
                {
                    firstId = copy.ID.Value;
                }

                ResponseTemplate.AddItemToPage(SelectedPage.ID.Value, copy.ID.Value, currentPosition);

                ItemID = copy.ID;

                currentPosition++;
            }

            selectedIds = _itemListFromLibrary.SelectedItemIds;
            if (selectedIds.Count < 1)
            {
                _currentTabTxt.Text = "2";
                IsImportSucceeded = false;
                return;
            }

            if (selectedIds.Count > 1)
            {
                //When adding new items to surveys and reports, the user should automatically be redirected to the item editor for the item just added. The only exception to this case should be when MORE THAN 1 item is added from an item library.
                ItemID = null;
            }

            ResponseTemplate.ModifiedBy = principal.Identity.Name;
            ResponseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

            if (selectedIds.Count == 1)
            {
                _editItemButton.CommandArgument = firstId.ToString();
            }
        }

        /// <summary>
        /// Verify item type for page type
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsValidItemType(ItemData item)
        {
            var isHiddenPage = SelectedPage.PageType == TemplatePageType.HiddenItems;
            var isHiddenItem = item.ItemDataTableName == "HiddenItemData";
            if (isHiddenPage && !isHiddenItem)
            {
                return false;
            }

            if (!isHiddenPage && isHiddenItem)
            {
                HiddenItemImportError = true;
                return false;
            }

            var isCompletionPage = SelectedPage.PageType == TemplatePageType.Completion;
            if (isCompletionPage)
            {
                if (item.ItemIsIAnswerable)
                    return false;
            }
            return true;
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
        /// Add single item of selected type.
        /// </summary>
        private void AddItemOfSelectedType(CheckboxPrincipal principal)
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
                itemData.SetDefaults(ResponseTemplate);
                itemData.CreatedBy = User.Identity.Name;
                itemData.Save();
                itemData.SetDefaultTexts();

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);

                // Change graph type
                if (SelectedItemTypes[0] == "SummaryChart")
                    appearanceData.SetPropertyValue("GraphType", _itemList.ChartType);

                if (appearanceData != null)
                {
                    //Set default options for the appearance
                    appearanceData.SetDefaults();
                    appearanceData.Save(itemData.ID.Value);
                }

                _editItemButton.CommandArgument = itemData.ID.ToString();

                ResponseTemplate.AddItemToPage(SelectedPage.ID.Value, itemData.ID.Value, LAST_ITEM_INDEX);
                ResponseTemplate.ModifiedBy = principal.Identity.Name;
                ResponseTemplate.Save();

                ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

                if (itemData.ID != null)
                    ItemID = itemData.ID.Value;
            }
        }

        /// <summary>
        /// Edit the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _editItemButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                string.Format("EditItem.aspx?isNew=true&i={0}&p={1}&s={2}&l={3}&added=true", _editItemButton.CommandArgument,
                              SelectedPage.ID, ResponseTemplateId, LanguageCode), false);
        }

        /// <summary>
        /// Restart the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _restartButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                string.Format("AddItem.aspx?s={0}&p={1}&l={2}", ResponseTemplateId, SelectedPage.ID, LanguageCode),
                false);
        }
    }
}
