using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Common;
using Checkbox.Web.Forms.UI.Editing;
using CheckboxWeb.Forms.Surveys.Controls.ItemEditors;
using System.Web;
using System.Linq;
using Checkbox.Web;

namespace CheckboxWeb.Libraries.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LibraryItemEditor : UserControlBase
    {
        /// <summary>
        /// Get item data edited
        /// </summary>
        public ItemData ItemData { get; private set; }

        /// <summary>
        /// Get/set appearance data
        /// </summary>
        public AppearanceData AppearanceData { get; private set; }

        /// <summary>
        /// Item editor control
        /// </summary>
        private IItemEditor ItemEditor { get; set; }

        /// <summary>
        /// Appearance editor control
        /// </summary>
        private IAppearanceEditor AppearanceEditor { get; set; }

        /// <summary>
        /// Initialize item with item and appearance data.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="itemData"></param>
        /// <param name="appearanceData"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="libraryTemplateId"></param>
        /// <param name="hidePreview"></param>
        public void Initialize(int libraryTemplateId, int pagePosition, ItemData itemData, AppearanceData appearanceData, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, bool hidePreview)
        {
            //Store inputs
            ItemData = itemData;
            AppearanceData = appearanceData;

            //Check values
            if (itemData == null)
            {
                throw new Exception("Library item editor was initialized with NULL item data.");
            }

            //Get an editor for the item data
            ItemEditor = ItemEditorFactory.CreateEditor(itemData.ItemTypeName);

            if (ItemEditor == null)
            {
                throw new Exception("Unable to create item editor for item type: " + itemData.ItemTypeName);
            }

            if (!(ItemEditor is Control))
            {
                throw new Exception("Item editor type: " + ItemEditor.GetType() + " is not a control and can't be added to library item editor control collection.");
            }

            if (IsMatrixColumn && (ItemEditor is UserControlHostItemEditor))
            {
                if (!itemData.ItemTypeName.StartsWith("Matrix")) // Some items (e.g. MatrixSumTotal) can be used only for Matrix Item, so they contain this prefix. For all other it should be added.
                    (ItemEditor as UserControlHostItemEditor).ControlNameOverride = "Matrix" + itemData.ItemTypeName;
            }

            ItemEditor.Initialize(libraryTemplateId, pagePosition, itemData, currentLanguage, surveyLanguages, isPagePostBack, EditMode.Library, hidePreview);
            _itemEditorPlace.Controls.Add((Control)ItemEditor);

            //Get an editor for appearance data, if any
            if (appearanceData == null)
            {
                return;
            }

            //describe OnHtmlEditorRedirect event
            var optionsEditor = FindFirstChildControl<OptionsNormalEntry>();
            if (optionsEditor != null)
                optionsEditor.OnHtmlEditorRedirect += BeforeRedirectToHtmlEditor;

            var rankOrderOptionsEditor = FindFirstChildControl<RankOrderOptionsNormalEntry>();
            if (rankOrderOptionsEditor != null)
                rankOrderOptionsEditor.OnHtmlEditorRedirect += BeforeRedirectToHtmlEditor;                

            //Not all items have appearances, so it's ok if there is no appearance editor
            AppearanceEditor = AppearanceEditorFactory.CreateEditor(appearanceData.AppearanceCode);

            if (AppearanceEditor == null)
            {
                _appearanceEditorPlace.Visible = false;
                return;
            }

            //Initialize
            AppearanceEditor.Initialize(appearanceData);

            //Add to appropriate place in control hierarchy
            if (ItemEditor.SupportsEmbeddedAppearanceEditor)
            {
                ItemEditor.EmbedAppearanceEditor(AppearanceEditor);
                _appearanceEditorPlace.Visible = false;
            }
            else
            {
                _appearanceEditorPlace.Visible = true;

                if (!(AppearanceEditor is Control))
                {
                    throw new Exception("Appearance editor type: " + AppearanceEditor.GetType() + " is not a control and can't be added to library item editor control collection.");
                }

                _appearanceEditorPlace.Controls.Add((Control)AppearanceEditor);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeforeRedirectToHtmlEditor(object sender, EventArgs e)
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.UpdateData();
            }

            ItemEditor.UpdateData(false);
        }

        /// <summary>
        /// Validate editor
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //Validate item and appearance data. If appearance editor is null, simply
            // assign a true value for appearance editor validation.                
            return ItemEditor.Validate() && (AppearanceEditor == null || AppearanceEditor.Validate())
            && GetValidationMessage() == string.Empty;
        }

        /// <summary>
        /// Validates alias for library items
        /// </summary>
        /// <returns></returns>
        public string GetValidationMessage()
        {
            var currItemAlias = WebUtilities.GetRequestFormValueBySubstring("_aliasText");
            var maxCharAllowed = 30;
            //var aliases = ItemConfigurationManager.GetItemAliases();
            var message = string.Empty;

            if (currItemAlias.Length > maxCharAllowed) message = "Alias exceeds maximum of 30 characters allowed.";

            //if (string.IsNullOrWhiteSpace(currItemAlias)) message = "Alias is required for library items";
            //else 
            //else if (aliases.Any(a => a == currItemAlias)) message = "Alias must be unique. Choose a different alias";

            return message;
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateData()
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.UpdateData();
            }

            ItemEditor.UpdateData();
        }

        /// <summary>
        /// Save data
        /// </summary>
        public int SaveData()
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.SaveData();
            }

            return ItemEditor.SaveData();
        }

        public bool DoesItemContainEnoughInformation(out string message)
        {
            return ItemEditor.DoesItemContainEnoughInformation(out message);
        }

        /// <summary>
        /// Determine if this item is a matrix column
        /// </summary>
        public bool IsMatrixColumn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ItemTextDecorator TextDecorator
        {
            get
            {
                if (ItemEditor != null
                    && ItemEditor is UserControlHostItemEditor)
                {
                    return ((UserControlHostItemEditor)ItemEditor).TextDecorator;
                }

                return null;
            }
        }

    }
}