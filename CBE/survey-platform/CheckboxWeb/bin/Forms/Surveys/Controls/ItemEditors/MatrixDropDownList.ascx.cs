using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixDropDownList : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;
        private EditMode _editMode;
        private MatrixItemTextDecorator _textDecorator;
        private MatrixItemData _matrixItemData;

        /// <summary>
        /// Get response template being edited
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get 
            {
                if (_editMode != Checkbox.Forms.EditMode.Survey)
                    return null;
                return _responseTemplate ??
                       (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(TemplateId));
            }
        }


        /// <summary>
        /// Get SelectItemData
        /// </summary>
        private SelectItemData SelectItemData
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_SelectItemData"] == null)
                {

                    HttpContext.Current.Session[ID + "_SelectItemData"] = ItemData as SelectItemData;

                    if (HttpContext.Current.Session[ID + "_SelectItemData"] == null)
                    {
                        throw new Exception("Unable to load select item data for item with id: " + ItemData.ID);
                    }
                }

                return HttpContext.Current.Session[ID + "_SelectItemData"] as SelectItemData;
            }
            set { HttpContext.Current.Session[ID + "_SelectItemData"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private MatrixItemTextDecorator MatrixTextDecorator
        {
            get
            {
                if (_textDecorator == null)
                {
                    _textDecorator =
                        HttpContext.Current.Session[Surveys.Controls.ItemEditors.Matrix.InMemoryMatrixItemEditorSessionKey] as MatrixItemTextDecorator;
                }

                if (_textDecorator == null)
                {
                    throw new Exception("Unable to find item to edit in session.");
                }

                return _textDecorator;
            }
        }

        /// <summary>
        /// Get MatrixItemData, associated with the item. 
        /// </summary>
        private MatrixItemData MatrixItemData
        {
            get
            {
                if (_matrixItemData == null)
                {
                    if (MatrixTextDecorator == null)
                    {
                        return null;
                    }

                    _matrixItemData = MatrixTextDecorator.Data;
                }

                return _matrixItemData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Initialize item editor
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            _editMode = editMode;
            //Call base initialize.  If item is not new, not allow new text decorator to be created by setting preventDecoratorReuse parameter
            // to false.
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview, !data.ID.HasValue);

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is SelectItemTextDecorator)
            {
                _questionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, isPagePostBack, ResponseTemplate, PagePosition, false, editMode);
                _behaviorEditor.Initialize(((SelectItemTextDecorator)TextDecorator), false, false, templateId, pagePosition, editMode);

                int? columnPosition = MatrixItemData == null || !ItemData.ID.HasValue ? null : MatrixItemData.GetColumnPosition(ItemData.ID.Value);
                _columnOptionsEditor.Initialize(MatrixItemData, columnPosition, isPagePostBack);
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator != null
                && TextDecorator is SelectItemTextDecorator)
            {
                ((SelectItemTextDecorator)TextDecorator).Text = _questionTextEditor.Text;

                _selectOptionsEditor.UpdateData((SelectItemTextDecorator)TextDecorator);
                _behaviorEditor.UpdateData(((SelectItemTextDecorator)TextDecorator));
                if (MatrixItemData != null && ItemData.ID.HasValue)
                {
                    int? columnPosition = MatrixItemData.GetColumnPosition(ItemData.ID.Value);
                    _columnOptionsEditor.UpdateData(MatrixItemData, columnPosition.Value);
                }
            }
        }

        /// <summary>
        /// Save item data
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            if (MatrixItemData != null)
                MatrixItemData.Save();
            return base.SaveData();
        }


        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            return _selectOptionsEditor.Validate();
        }

        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        public override bool DoesItemContainEnoughInformation(out string message)
        {
            //UpdateData(); //-- commented to avoid options duplication
            message = null;
            if (String.IsNullOrEmpty(_questionTextEditor.Text))
            {
                message = TextManager.GetText("/pageText/forms/surveys/addMatrixColumn.aspx/confirmEmptyText",
                                              CurrentLanguage);
                return false;
            }
            if (_selectOptionsEditor.OptionsCount == 0)//(SelectItemData.Options.Count == 0)
            {
                message = TextManager.GetText("/pageText/forms/surveys/addMatrixColumn.aspx/confirmWithoutChoices",
                                              CurrentLanguage);
                return false;
            }
            return true;
        }
    }
}