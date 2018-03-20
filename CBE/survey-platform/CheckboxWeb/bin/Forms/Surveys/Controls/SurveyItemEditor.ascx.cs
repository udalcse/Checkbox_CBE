using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Common;
using Checkbox.Web.Forms.UI.Editing;
using System.Web;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Web;
using CheckboxWeb.Forms.Surveys.Controls.ItemEditors;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SurveyItemEditor : UserControlBase
    {
        private const string APPEARANCE_DATA_SESSION_KEY = "SurveyItemEditor_AppearanceData";

        private string _uniqueName;

        /// <summary>
        /// Get item data edited
        /// </summary>
        public ItemData ItemData { get; private set; }

        /// <summary>
        /// Get/set appearance data
        /// </summary>
        protected AppearanceData AppearanceData
        {
            get { return HttpContext.Current.Session[APPEARANCE_DATA_SESSION_KEY + (ItemData != null ? ItemData.ID.ToString() : string.Empty)] as AppearanceData; }
            private set { HttpContext.Current.Session[APPEARANCE_DATA_SESSION_KEY + (ItemData != null ? ItemData.ID.ToString() : string.Empty)] = value; }
        }

        /// <summary>
        /// Item editor control
        /// </summary>
        private IItemEditor ItemEditor { get; set; }

        /// <summary>
        /// Appearance editor control
        /// </summary>
        public IAppearanceEditor AppearanceEditor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ItemTextDecorator TextDecorator
        {
            get
            {
                if (ItemEditor is UserControlHostItemEditor)
                {
                    return ((UserControlHostItemEditor) ItemEditor).TextDecorator;
                }

                return null;
            }
        }


        /// <summary>
        /// Determine if this item is a matrix column
        /// </summary>
        public bool IsMatrixColumn { get; set; }

        /// <summary>
        /// Initialize item with item and appearance data.
        /// </summary>
        /// <param name="pagePosition"></param>
        /// <param name="itemData"></param>
        /// <param name="appearanceData"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="hidePreview"></param>
        public void Initialize(int responseTemplateId, int pagePosition, ItemData itemData, AppearanceData appearanceData, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, bool hidePreview)
        {
            bool fromHtmlRedactor = Convert.ToBoolean(Request.Params["fromHtmlRedactor"]);

            //Store inputs
            ItemData = itemData;

            //Not all items have appearances, so it's ok if there is no appearance editor
            if ((!isPagePostBack && !fromHtmlRedactor) ||
                //fix for matrix single line appearence data. Without that it will recive parent matrix appearence
                (AppearanceData is Checkbox.Forms.Items.UI.MatrixSingleLineText &&
                appearanceData is Checkbox.Forms.Items.UI.Matrix))
                AppearanceData = appearanceData;

            _uniqueName =  appearanceData.AppearanceCode + appearanceData.ID + (itemData.ID ?? 0);

            //Check values
            if (itemData == null)
            {
                throw new Exception("Survey item editor was initialized with NULL item data.");
            }

            //Get an editor for the item data
            ItemEditor = ItemEditorFactory.CreateEditor(itemData.ItemTypeName);

            if (ItemEditor == null)
            {
                throw new Exception("Unable to create item editor for item type: " + itemData.ItemTypeName);
            }

            if (!(ItemEditor is Control))
            {
                throw new Exception("Item editor type: " + ItemEditor.GetType() + " is not a control and can't be added to survey item editor control collection.");
            }

            if (IsMatrixColumn && (ItemEditor is UserControlHostItemEditor))
            {
                if (!itemData.ItemTypeName.StartsWith("Matrix")) // Some items (e.g. MatrixSumTotal) can be used only for Matrix Item, so they contain this prefix. For all other it should be added.
                    (ItemEditor as UserControlHostItemEditor).ControlNameOverride = "Matrix" + itemData.ItemTypeName;
            }
       
            _itemEditorPlace.Controls.Add((Control)ItemEditor);
            ItemEditor.Initialize(responseTemplateId, pagePosition, itemData, currentLanguage, surveyLanguages, isPagePostBack, EditMode.Survey, hidePreview);

            //Get an editor for appearance data, if any
            if (AppearanceData == null)
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

            var otherOptionEntry = FindFirstChildControl<OtherOptionEntry>();
            if (otherOptionEntry != null)
                otherOptionEntry.OnHtmlEditorRedirect += BeforeRedirectToHtmlEditorFromOtherOption;
            var noneOfAboveEntry = FindFirstChildControl<NonOfAboveEntry>();
            if (noneOfAboveEntry != null)
                noneOfAboveEntry.OnHtmlEditorRedirect += BeforeRedirectToHtmlEditorFromOtherOption;                

            AppearanceEditor = AppearanceEditorFactory.CreateEditor(AppearanceData.AppearanceCode);

            if (AppearanceEditor == null)
            {
                _appearanceEditorPlace.Visible = false;
                return;
            }

            //Initialize
            AppearanceEditor.Initialize(AppearanceData);

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
                    throw new Exception("Appearance editor type: " + AppearanceEditor.GetType() + " is not a control and can't be added to survey item editor control collection.");
                }

                _appearanceEditorPlace.Controls.Add((Control)AppearanceEditor);

            }
        }

        private void BeforeRedirectToHtmlEditor(object sender, EventArgs e)
        {
            if (AppearanceEditor != null)
            {
                AppearanceEditor.UpdateData();
            }

            ItemEditor.UpdateData(false);
        }

        private void BeforeRedirectToHtmlEditorFromOtherOption(object sender, EventArgs e)
        {
            BeforeRedirectToHtmlEditor(sender, e);

            var optionsEditor = FindFirstChildControl<OptionsNormalEntry>();
            if (optionsEditor != null)
                optionsEditor.ProcessOptionsPost(false);
        }

        /// <summary>
        /// Validate editor
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //Validate item and appearance data. If appearance editor is null, simply
            // assign a true value for appearance editor validation.
            return ItemEditor.Validate() && (AppearanceEditor == null || AppearanceEditor.Validate());
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateData()
        {
            Update(true);
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void UpdateData(bool updateTextDecorator)
        {
            Update(updateTextDecorator);
        }

        /// <summary>
        /// Update data
        /// </summary>
        public void Update(bool updateTextDecorator)
        {
            if (AppearanceEditor != null)
            { 
                AppearanceEditor.UpdateData();
            }

            ItemData.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;

            ItemEditor.UpdateData(updateTextDecorator);
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

        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        public bool DoesItemContainEnoughInformation(out string message)
        {
            return ItemEditor.DoesItemContainEnoughInformation(out message);
        }

        public string GetValidationMessage()
        {
            var currItemAlias = WebUtilities.GetRequestFormValueBySubstring("_aliasText");
            var aliases = ItemConfigurationManager.GetItemAliases();
            var message = string.Empty;

            if (aliases.Any(a => a == currItemAlias)) message = "Alias must be unique. Choose a different alias";

            return message;
        }
    }
}