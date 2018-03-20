using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Page;

namespace CheckboxWeb.Libraries
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EditMatrixColumn : SecuredPage
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

        [QueryParameter("lid", IsRequired = true)]
        public int LibraryTemplateId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }

        private Dictionary<int, ItemTextDecorator> _columnPrototypeWorkingCopies;
        private MatrixItemTextDecorator _textDecorator;
        private MatrixItemData _matrixItemData;

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
                        HttpContext.Current.Session[Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] as MatrixItemTextDecorator;
                }

                if (_textDecorator == null)
                {
                    throw new Exception("Unable to find item to edit in session.");
                }

                return _textDecorator;
            }
            set { HttpContext.Current.Session[Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] = value; }
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
                        HttpContext.Current.Session[Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixColumnsEditorSessionKey] as
                        Dictionary<int, ItemTextDecorator>;
                }

                if (_columnPrototypeWorkingCopies == null)
                {
                    throw new Exception("Unable to find working copies of column prototypes");
                }

                return _columnPrototypeWorkingCopies;
            }

            set { HttpContext.Current.Session[Forms.Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixColumnsEditorSessionKey] = value; }
        }

        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(LanguageCode)
                           ? LanguageCode
                           : TextDecoratorClone.Language;
            }
        }

        /// <summary>
        /// Get library template being edited
        /// </summary>
        public LibraryTemplate LibraryTemplate
        {
            get
            {
                if (_libraryTemplate == null)
                {
                    _libraryTemplate = LibraryTemplateManager.GetLibraryTemplate(LibraryTemplateId);

                    if (_libraryTemplate == null)
                    {
                        throw new Exception(string.Format("Unable to load library with id {0}.", LibraryTemplateId));
                    }
                }

                return _libraryTemplate;
            }
        }

        /// <summary>
        /// Data to edit
        /// </summary>
        private ItemData ItemData
        {
            get { return HttpContext.Current.Session["ADD_MATRIX_COLUMN_ItemData_" + MatrixItemId] as ItemData; }
            set { HttpContext.Current.Session["ADD_MATRIX_COLUMN_ItemData_" + MatrixItemId] = value; }
        }

        /// <summary>
        /// Appearance data
        /// </summary>
        private AppearanceData AppearanceData
        {
            get { return HttpContext.Current.Session["ADD_MATRIX_COLUMN_ItemAppearanceData_" + MatrixItemId] as AppearanceData; }
            set { HttpContext.Current.Session["ADD_MATRIX_COLUMN_ItemAppearanceData_" + MatrixItemId] = value; }
        }


        /// <summary>
        /// Bind item to editor.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Page.Title = WebTextManager.GetText("/pageText/forms/surveys/editItem.aspx/editSurveyItem");

            if (!Page.IsPostBack)
            {
                ClearValues();
            }

            if (!MatrixItemId.HasValue)
                throw new Exception("Matrix id was not specified.");

            if (!ColumnNumber.HasValue)
                throw new Exception("Column number was not specified.");

            if (!ColumnPrototypesWorkingCopies.ContainsKey(ColumnNumber.Value))
            {
                throw new Exception("Column prototype copies dictionary does not contain entry for column: " + ColumnNumber);
            }

            if (!Page.IsPostBack)
            {
                //store column number for html editor
                Session["matrixColumn_r=" + LibraryTemplateId + "_i=" + MatrixItemId] = ColumnNumber;

                //Prime the text decorator used by the item editor so that it uses our copy instead of a newly created on
                var workingTextDecorator = ColumnPrototypesWorkingCopies[ColumnNumber.Value];

                HttpContext.Current.Session[UserControlItemEditorBase.InMemoryItemEditorSessionKey] = workingTextDecorator;
                ItemData = workingTextDecorator.Data;
            }

            if (ItemData == null)
            {
                throw new Exception(string.Format("Unable to load prototype for column: {0}", ColumnNumber));
            }

            //Not all items have appearances, so it's ok for appearance data to be null
            AppearanceData = AppearanceDataManager.GetAppearanceDataForItem(ItemData.ID.Value);

            _itemEditor.IsMatrixColumn = true;

            //Initialize component on initial view of page.
            _itemEditor.Initialize(
                LibraryTemplateId,
                1,
                ItemData,
                AppearanceData,
                EditLanguage,
                TextManager.SurveyLanguages.ToList(),
                Page.IsPostBack,
                true);

            //Bind click events
            Master.OkClick += _saveButton_Click;
            if (IsNew.HasValue && IsNew.Value)
                Master.CancelClick += Master_CancelClick;

            //Prevent Enter-Key press handling
            Master.PreventEnterKeyBinding();
            Master.IsDialog = true;
        }

        void Master_CancelClick(object sender, EventArgs e)
        {
            ReturnToEditItemPage();
        }

        /// <summary>
        /// Clear values
        /// </summary>
        private void ClearValues()
        {
            ItemData = null;
            AppearanceData = null;
        }

        /// <summary>
        /// Return to edit item page. This method is used when a column is added just after item creation.
        /// </summary>
        private void ReturnToEditItemPage()
        {
            String url = "EditItem.aspx?";
            foreach (string key in HttpContext.Current.Request.QueryString.AllKeys)
            {
                if (key != "c" && key != "showColumnsTab")
                    url += key + "=" + HttpContext.Current.Request.QueryString[key] + "&";
            }
            url += "showColumnsTab=true";
            Response.Redirect(url);
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
                //Update item data
                _itemEditor.UpdateData();

                //Do not save data since we only want to save column prototype data when
                // saving matrix question itself.
                //_itemEditor.SaveData();

                //Save appearance
                AppearanceData.Save(ItemData.ID.Value);

                //TODO: Edit column options
                //_columnOptions.UpdateData(MatrixItemData);

                //Udpate column working copy
                var prototypeDictionary = ColumnPrototypesWorkingCopies;
                prototypeDictionary[ColumnNumber.Value] = _itemEditor.TextDecorator;

                //Replace in session
                ColumnPrototypesWorkingCopies = prototypeDictionary;

                //Update decorator for matrix
                var matrixTextDecorator = TextDecoratorClone;
                matrixTextDecorator.SetData(MatrixItemClone);
                matrixTextDecorator.SetColumnText(ColumnNumber.Value,
                                                  ((LabelledItemTextDecorator)_itemEditor.TextDecorator).Text);

                //Replace in session
                TextDecoratorClone = matrixTextDecorator;

                //Restore in-memory text decorator used by item editor so matrix preview is properly shown when matrix
                // editor reloads.
                HttpContext.Current.Session[UserControlItemEditorBase.InMemoryItemEditorSessionKey] = matrixTextDecorator;

                //Clear session data
                ClearValues();

                if (IsNew.HasValue && IsNew.Value)
                    ReturnToEditItemPage();
                else
                {
                    //Run dialog
                    ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "okClick();", true);
                }
            }
        }
    }
}