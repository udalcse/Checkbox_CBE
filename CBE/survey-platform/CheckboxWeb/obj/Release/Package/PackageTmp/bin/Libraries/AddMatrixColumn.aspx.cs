using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Page;
using CheckboxWeb.Controls.Wizard.WizardControls;
using CheckboxWeb.Forms.Surveys.Controls.ItemEditors;

namespace CheckboxWeb.Libraries
{
    /// <summary>
    /// Edit column prototype for matrix column
    /// </summary>
    public partial class AddMatrixColumn : SecuredPage
    {
        private LibraryTemplate _libraryTemplate;

        /// <summary>
        /// ID of matrix item column belongs to
        /// </summary>
        [QueryParameter("i")]
        public int? MatrixItemId { get; set; }

        /// <summary>
        /// Column nmber to edit prototype of
        /// </summary>
        [QueryParameter("c")]
        public int? ColumnNumber { get; set; }

        [QueryParameter("lid")]
        public int LibraryTemplateId { get; set; }

        [QueryParameter("p")]
        public int? PagePosition { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        [QueryParameter("categorized", DefaultValue = "false")]
        public bool AreColumnsCategorized { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }

        private Dictionary<int, ItemTextDecorator> _columnPrototypeWorkingCopies;
        private MatrixItemTextDecorator _textDecorator;
        private MatrixItemData _matrixItemData;

        /// <summary>
        /// 
        /// </summary>
        public LibraryTemplate LibraryTemplate
        {
            get
            {
                if (_libraryTemplate == null)
                {
                    _libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(LibraryTemplateId);
                }

                return _libraryTemplate;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public MatrixItemTextDecorator TextDecoratorClone
        {
            get
            {
                if (_textDecorator == null)
                {
                    _textDecorator =
                        Session[CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] as MatrixItemTextDecorator;
                }

                if (_textDecorator == null)
                {
                    throw new Exception("Unable to find item to edit in session.");
                }

                return _textDecorator;
            }
            set { Session[CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public MatrixItemData MatrixItemClone
        {
            get
            {
                if (_matrixItemData == null)
                {
                    _matrixItemData = TextDecoratorClone.Data;
                }

                if (_matrixItemData == null)
                {
                    throw new Exception("Unable to load matrix item data for item with id: " + MatrixItemId);
                }

                return _matrixItemData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, ItemTextDecorator> ColumnPrototypesWorkingCopies
        {
            get
            {
                if (_columnPrototypeWorkingCopies == null)
                {
                    _columnPrototypeWorkingCopies =
                        Session[CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixColumnsEditorSessionKey] as
                        Dictionary<int, ItemTextDecorator>;
                }

                if (_columnPrototypeWorkingCopies == null)
                {
                    throw new Exception("Unable to find working copies of column prototypes");
                }

                return _columnPrototypeWorkingCopies;
            }

            set { Session[CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixColumnsEditorSessionKey] = value; }
        }

        /// <summary>
        /// Data to edit
        /// </summary>
        private ItemData ItemData
        {
            get { return Session["ADD_MATRIX_COLUMN_ItemData_" + MatrixItemId] as ItemData; }
            set { Session["ADD_MATRIX_COLUMN_ItemData_" + MatrixItemId] = value; }
        }

        /// <summary>
        /// Appearance data
        /// </summary>
        private AppearanceData AppearanceData
        {
            get { return Session["ADD_MATRIX_COLUMN_ItemAppearanceData_" + MatrixItemId] as AppearanceData; }
            set { Session["ADD_MATRIX_COLUMN_ItemAppearanceData_" + MatrixItemId] = value; }
        }


        /// <summary>
        /// Determine if user confirm creating the empty item
        /// </summary>
        private bool IsConfirmed
        {
            get
            {
                bool temp;
                return bool.TryParse(_isConfirmed.Text, out temp) ? temp : false;
            }
            set { _isConfirmed.Text = value.ToString(); }
        }


        /// <summary>
        /// Check arguments
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!MatrixItemId.HasValue)
            {
                throw new Exception("Matrix id was not specified.");
            }

            if (!ColumnNumber.HasValue)
            {
                throw new Exception("Column number was not specified.");
            }

            if (!Page.IsPostBack)
            {
                ClearValues();
                if (AreColumnsCategorized)
                {
                    _itemList.TypesToInclude =
                        "SingleLineText,CategorizedRadioButtons,CategorizedDropdownList,CategorizedCheckboxes,MatrixSumTotal,RadioButtonScale,RowSelector";
                }
            }

            Master.SetTitle(WebTextManager.GetText("/pageText/matrixItemEditor.aspx/addItem"));
            Master.HideDialogButtons();

            _wizard.NextButtonClick += _wizard_NextButtonClick;
            _wizard.CancelButtonClick += _wizard_CancelButtonClick;
            _wizard.PreviousButtonClick += _wizard_PreviousButtonClick;
            _wizard.FinishButtonClick += _wizard_FinishButtonClick;
            _restartButton.Click += _restartButton_Click;
            //Initialize editor
            InitializeItemEditor(Page.IsPostBack);
        }

        /// <summary>
        /// Get navigation buttons
        /// </summary>
        protected WizardButtons NavigationButtons
        {
            get
            {
                return _wizard.FindControl("FinishNavigationTemplateContainerID").FindControl("_finishNavigationButtons") as
                       WizardButtons;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _restartButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("AddMatrixColumn.aspx?i={0}&c={1}&lid={2}&p={3}&l={4}&categorized={5}&isNew={6}", MatrixItemId, ColumnNumber + 1, LibraryTemplateId, PagePosition, LanguageCode, AreColumnsCategorized, IsNew ?? false));
        }

        /// <summary>
        /// Clear out values on previous click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            ClearValues();
        }

        /// <summary>
        /// Handle cancel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_CancelButtonClick(object sender, EventArgs e)
        {
            ClearValues();
            if (IsNew.HasValue && IsNew.Value)
            {
                ReturnToEditItemPage();
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "closeWindow",
                    "closeWindow();",
                    true);
            }
        }

        /// <summary>
        /// Return to edit item page. This method is used when a column is added just after item creation.
        /// </summary>
        private void ReturnToEditItemPage()
        {
            String url = "EditItem.aspx?";
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "c" && key != "showColumnsTab")
                    url += key + "=" + Request.QueryString[key] + "&";
            }
            url += "showColumnsTab=true";
            Response.Redirect(url);
        }


        /// <summary>
        /// Handle next click, and ensure an item type was selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (e.CurrentStepIndex == 0)
            {
                //Ensure item type selected
                if (Utilities.IsNullOrEmpty(_itemList.SelectedItemType))
                {
                    _errorPanel.Visible = true;
                    _errorMsgLbl.Text = WebTextManager.GetText("/pageText/matrixItemEditor.aspx/typeSelectError");

                    e.Cancel = true;
                    return;
                }

                //Otherwise, set the editor up for editing
                string itemTypeName = _itemList.SelectedItemType;

                ItemData itemData = ItemConfigurationManager.CreateConfigurationData(itemTypeName);

                SetItemDataDefaults(itemData);

                if (itemData == null)
                {
                    throw new Exception("Unable to create item data for type: " + itemTypeName);
                }

                string defaultAppearanceCode = AppearanceDataManager.GetDefaultAppearanceCodeForType(itemData.ItemTypeID);

                if (Utilities.IsNullOrEmpty(defaultAppearanceCode))
                {
                    throw new Exception("Unable to get default appearance code for item type: " + itemTypeName);
                }

                AppearanceData itemAppearanceData;

                if (defaultAppearanceCode.StartsWith("CATEGORIZED") || defaultAppearanceCode.StartsWith("MATRIX_"))
                {
                    itemAppearanceData = AppearanceDataManager.GetAppearanceDataForCode(defaultAppearanceCode);
                }
                else
                {
                    //Get matrix-specific appearance 
                    itemAppearanceData =
                        AppearanceDataManager.GetAppearanceDataForCode("MATRIX_" + defaultAppearanceCode);
                }

                if (itemAppearanceData == null)
                {
                    throw new Exception("Unable to get appearance data for code: MATRIX_" + defaultAppearanceCode);
                }

                ItemData = itemData;
                AppearanceData = itemAppearanceData;

                //Prime the text decorator used by the item editor so that it uses our copy instead of the matrix item's text decorator
                // which is already in memory.
                var workingTextDecorator = itemData.CreateTextDecorator(LanguageCode);

                Session[UserControlItemEditorBase.InMemoryItemEditorSessionKey] = workingTextDecorator;
                ItemData = workingTextDecorator.Data;

                InitializeItemEditor(false); // IsPagePostBack equals false to initialize controls.


                //NavigationButtons.NextButton.OnClientClick = String.Format("NotEmptyChecking('MatrixItemEditor_{0}_{1}'); return false;", MatrixItemId ?? -1,
                //                                                ColumnNumber ?? MatrixItemData.ColumnCount + 1);
                NavigationButtons.NextButton.Attributes["name"] = NavigationButtons.NextButton.UniqueID;
            }

        }

        /// <summary>
        /// Clear values
        /// </summary>
        private void ClearValues()
        {
            ItemData = null;
            AppearanceData = null;
            IsConfirmed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeItemEditor(bool isPagePostBack)
        {
            //Initialize editor
            //TODO: Language Code
            if (ItemData != null && AppearanceData != null)
            {
                //store column number for html editor
                Session["matrixColumn_r=" + LibraryTemplateId + "_i=" + MatrixItemId] = ColumnNumber;

                _itemEditor.IsMatrixColumn = true;
                _itemEditor.Initialize(LibraryTemplateId, PagePosition ?? -1, ItemData, AppearanceData, LanguageCode, null, isPagePostBack, true);

                var optionsEditor = FindFirstChildControl<OptionsNormalEntry>();
                if (optionsEditor != null)
                    optionsEditor.OnHtmlEditorRedirect += (sender, args) => SaveColumnInMemory(false);
            }
        }

        public int SaveColumnInMemory(bool updateEditor = true)
        {
            var newItemId = _itemEditor.SaveData();

            //Save appearance
            AppearanceData.Save(newItemId);

            //Now add column
            MatrixItemClone.AddColumn(
                newItemId,
                _itemEditor.ItemData.ItemTypeName,
                false,
                null);

            if (updateEditor)
            {
                //Column options can be saved only after creating the item. So we should update and save the item once more.
                _itemEditor.UpdateData();
                _itemEditor.SaveData();
            }

            //Get matrix editor from session
            var matrixTextDecorator = TextDecoratorClone;

            if (matrixTextDecorator == null)
            {
                throw new Exception("Unable to find matrix text decorator in session.");
            }

            //Add data to column prototypes dictionary.  Create a new text decorator to ensure it doesn't
            // get nulled due to byRef hijinx.  Go through multiple assignment to ensure changes make it to
            // session in out-of-proc session state scenarios
            var columnPrototypes = ColumnPrototypesWorkingCopies;
            columnPrototypes[ColumnNumber.Value] = _itemEditor.ItemData.CreateTextDecorator(matrixTextDecorator.Language);
            ColumnPrototypesWorkingCopies = columnPrototypes;

            matrixTextDecorator.SetData(MatrixItemClone);
            matrixTextDecorator.SetColumnText(ColumnNumber.Value, ((LabelledItemTextDecorator)_itemEditor.TextDecorator).Text);

            //Replace in session
            TextDecoratorClone = matrixTextDecorator;

            //Restore in-memory text decorator used by item editor so matrix preview is properly shown when matrix
            // editor reloads.
            Session[UserControlItemEditorBase.InMemoryItemEditorSessionKey] = matrixTextDecorator;

            return newItemId;
        }

        /// <summary>
        /// Set some default options for new item
        /// </summary>
        /// <param name="itemData"></param>
        private void SetItemDataDefaults(ItemData itemData)
        {
            //TODO: Move Item Defaults into ItemData Objects.  Possibly as method
            //      On ItemData Class
            if (itemData is RatingScaleItemData)
            {
                var scaleItemData = itemData as RatingScaleItemData;
                scaleItemData.StartValue = 1;
                scaleItemData.EndValue = 5;
            }

            if (itemData is SliderItemData)
            {
                ((SliderItemData)itemData).ValueType = SliderValueType.NumberRange;
                ((SliderItemData)itemData).ValueListOptionType = SliderValueListOptionType.Text;
                ((SliderItemData)itemData).MinValue = 0;
                ((SliderItemData)itemData).MaxValue = 100;
                ((SliderItemData)itemData).StepSize = 1;
            }
        }

        /// <summary>
        /// Add column to matrix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Validate the item            
            if (_itemEditor.Validate())
            {
                String message;
                if (!_itemEditor.DoesItemContainEnoughInformation(out message) && !IsConfirmed)
                {

                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage",
                                                                "window.onload=function() { ShowConfirmMessage(\"" +
                                                                message + "\"); }",
                                                                true);
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                e.Cancel = true;
                return;
            }


            //Save data without updating the item. This is needed to assign ID. "Update" operation will be invoked later. 
            //"Save" operation will be invoked once more.
            var newItemId = _itemEditor.SaveData();

            //Save appearance
            AppearanceData.Save(newItemId);

            //Now add column
            MatrixItemClone.AddColumn(
                newItemId,
                _itemEditor.ItemData.ItemTypeName,
                false,
                null);

            //Column options can be saved only after creating the item. So we should update and save the item once more.
            _itemEditor.UpdateData();
            _itemEditor.SaveData();

            //Get matrix editor from session
            var matrixTextDecorator = TextDecoratorClone;

            if (matrixTextDecorator == null)
            {
                throw new Exception("Unable to find matrix text decorator in session.");
            }


            //Add data to column prototypes dictionary.  Create a new text decorator to ensure it doesn't
            // get nulled due to byRef hijinx.  Go through multiple assignment to ensure changes make it to
            // session in out-of-proc session state scenarios
            var columnPrototypes = ColumnPrototypesWorkingCopies;
            columnPrototypes[ColumnNumber.Value] = _itemEditor.ItemData.CreateTextDecorator(matrixTextDecorator.Language);
            ColumnPrototypesWorkingCopies = columnPrototypes;

            matrixTextDecorator.SetData(MatrixItemClone);
            matrixTextDecorator.SetColumnText(ColumnNumber.Value, ((LabelledItemTextDecorator)_itemEditor.TextDecorator).Text);

            //Replace in session
            TextDecoratorClone = matrixTextDecorator;

            //Restore in-memory text decorator used by item editor so matrix preview is properly shown when matrix
            // editor reloads.
            Session[UserControlItemEditorBase.InMemoryItemEditorSessionKey] = matrixTextDecorator;

            //Reload parent
            ClearValues();

            if (IsNew.HasValue && IsNew.Value)
                ReturnToEditItemPage();
        }
    }

}