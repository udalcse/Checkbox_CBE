using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web;
using Checkbox.Web.Page;

using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;
using System.Web;

namespace CheckboxWeb.Libraries
{
    public partial class AddItem : SecuredPage
    {
        private LibraryTemplate _libraryTemplate;
        private List<string> _selectedItemTypes;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("lib", IsRequired = true)]
        public int LibraryTemplateId { get; set; }

        /// <summary>
        /// Get Language Code
        /// </summary>
        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LibraryTemplate Library
        {
            get { return _libraryTemplate ?? (_libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(LibraryTemplateId)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity() { return Library; }

        /// <summary>
        /// Determine if import is succeded or not.
        /// </summary>
        private bool IsImportSucceded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Library.Edit"; } }

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
        }

        /// <summary>
        /// Get position of selected page
        /// </summary>
        private TemplatePage SelectedPage { get { return Library.GetPageAtPosition(1); } }

        /// <summary>
        /// Bind event handlers
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _wizard.FinishButtonClick += _wizard_FinishButtonClick;

            //Bind completion button clicks
            _restartButton.Click += _restartButton_Click;
            _editItemButton.Click += _editItemButton_Click;

            //Hide default buttons
            Master.HideDialogButtons();

            //
            Master.SetTitle(WebTextManager.GetText("/pageText/libraries/addItem.aspx/addItemToLibrary"));
            _itemList.ItemListTarget = "Library";
        }

        /// <summary>
        /// Handle finish click to populate completion page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Populate messages on complete step
            EventHandlerWrapper(AddItemsToLibrary);
            EventHandlerWrapper(PopulateCompletionStep);
        }

        /// <summary>
        /// Populate values on completion page.
        /// </summary>
        private void PopulateCompletionStep()
        {
            if (SelectedPage == null && Library.PageCount > 0)
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
                int selectedCount;

                switch (_currentTabTxt.Text)
                {
                    case "0":
                        selectedCount = SelectedItemTypes.Count;
                        break;
                    case "1":
                        selectedCount = _itemListFromLibrary.SelectedItemIds.Count;
                        break;
                    case "2":
                        selectedCount = _itemListFromSurvey.SelectedItemIds.Count;
                        break;
                    case "3":
                        selectedCount = IsImportSucceded ? 1 : 0;
                        break;
                    default:
                        selectedCount = 0;
                        break;
                }

                //Determine item type/count text
                //Customize text for single item added or multiple items added
                if (selectedCount > 1)
                {
                    //The selected <b>{0}</b> items were added to the library.
                    summaryText = WebTextManager.GetText("/pageText/libraries/addItem.aspx/addMultipleItemSummary");
                    summaryText = string.Format(summaryText, selectedCount);
                }
                else if (selectedCount == 1)
                {
                    //If adding single item, go directly to item editor
                    _editItemButton_Click(this, new EventArgs());

                    /* Keep around text update just incase behavior should be restored
                     * 
                    //The new <b>{0}</b> item was added to the library.
                    summaryText = WebTextManager.GetText("/pageText/libraries/addItem.aspx/addSingleNewItemSummary");
                    string itemTypeName = WebTextManager.GetText(string.Format("/itemType/{0}/name", SelectedItemTypes[0]));

                    summaryText = string.Format(summaryText, itemTypeName);
                    */
                }
                else
                {
                    summaryText = WebTextManager.GetText("/pageText/libraries/addItem.aspx/noItemsSummary");
                }


                //Show/hide edit item button
                _editItemButton.Visible = selectedCount == 1;


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
        /// Add new/imported/copied item(s) to library.
        /// </summary>
        private void AddItemsToLibrary()
        {
            if (_currentTabTxt.Text == "0")
                AddItemOfSelectedType();

            if (_currentTabTxt.Text == "1")
                AddItemsFromLibrary();

            if (_currentTabTxt.Text == "2")
                AddItemsFromSurvey();

            if (_currentTabTxt.Text == "3")
                AddItemFromXml();
        }

        /// <summary>
        /// Add an item of the selected type
        /// </summary>
        private void AddItemOfSelectedType()
        {
            //Item editor only makes sense when working with a single item
            if (SelectedItemTypes.Count != 1)
            {
                return;
            }

            var itemData = ItemConfigurationManager.CreateConfigurationData(SelectedItemTypes[0]);

            if (itemData != null)
            {
                //Set some default options
                itemData.SetDefaults(Library);
                itemData.CreatedBy = User.Identity.Name;
                itemData.Save();
                itemData.SetDefaultTexts();

                //Create/save appearance data
                AppearanceData appearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(itemData.ItemTypeID);

                if (appearanceData != null)
                {
                    //Set default options for the appearance
                    appearanceData.SetDefaults();
                    appearanceData.Save(itemData.ID.Value);
                }

                _editItemButton.CommandArgument = itemData.ID.ToString();

                if (Library.PageCount == 0)
                {
                    Library.AddPageToTemplate(null, false);
                }

                Library.AddItemToPage(SelectedPage.ID.Value, itemData.ID.Value, null);
                Library.Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddItemFromXml()
        {

            var xmlDoc = _itemImport.GetUploadedFileDocument();

            try
            {

                if (xmlDoc == null)
                    return;

                var itemElement = xmlDoc.SelectSingleNode("Item");

                ItemData item = ItemConfigurationManager.ImportItem((Checkbox.Security.Principal.CheckboxPrincipal)HttpContext.Current.User, itemElement);

                if (item == null)
                    return;

                if (Library.PageCount == 0)
                {
                    Library.AddPageToTemplate(null, false);
                }


                Library.AddItemToPage(SelectedPage.ID.Value, item.ID.Value, null);
                Library.Save();

                _editItemButton.CommandArgument = item.ID.ToString();

                IsImportSucceded = true;
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// Add all selected items from the selected library.
        /// </summary>
        private void AddItemsFromLibrary()
        {
            CopyItemsToTheLibrary(_itemListFromLibrary.SelectedItemIds, true);
        }

        /// <summary>
        /// Add all selected items from the selected survey
        /// </summary>
        private void AddItemsFromSurvey()
        {
            CopyItemsToTheLibrary(_itemListFromSurvey.SelectedItemIds, false);
        }

        private void CopyItemsToTheLibrary(ReadOnlyCollection<int> selectedIds, bool fromLibrary)
        {
            int firstId = -1;
            foreach (int selectedId in selectedIds)
            {
                ItemData toCopy;
                if (fromLibrary)
                    toCopy = _itemListFromLibrary.CurrentLibrary.GetItem(selectedId);
                else
                    toCopy = _itemListFromSurvey.CurrentSurvey.GetItem(selectedId);

                ItemData copy = ItemConfigurationManager.CopyItem(toCopy, (Checkbox.Security.Principal.CheckboxPrincipal)HttpContext.Current.User);

                if (copy == null)
                    return;

                if (firstId == -1)
                {
                    firstId = copy.ID.Value;
                }

                if (Library.PageCount == 0)
                {
                    Library.AddPageToTemplate(null, false);
                }

                Library.AddItemToPage(SelectedPage.ID.Value, copy.ID.Value, null);
            }

            Library.Save();

            if (selectedIds.Count == 1)
            {
                _editItemButton.CommandArgument = firstId.ToString();
            }
        }

        /// <summary>
        /// Edit the item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _editItemButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("EditItem.aspx?isNew=true&w=true&i={0}&p={1}&lid={2}&l={3}", _editItemButton.CommandArgument, SelectedPage.ID, LibraryTemplateId, LanguageCode), false);
        }

        /// <summary>
        /// Restart the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _restartButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("AddItem.aspx?lib={0}&l={1}", LibraryTemplateId, LanguageCode), false);
        }
    }
}
