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
    public partial class MatrixSumTotal : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;
        private EditMode _editMode;
        private MatrixItemTextDecorator _textDecorator;
        private MatrixItemData _matrixItemData;

        /// <summary>
        /// Get the tab which should be loaded in the begining
        /// </summary>
        public int StartTab { get; private set; }

        /// <summary>
        /// Editor supports embedded editors
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }

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
        /// Add appearance editor to item control hierarchy.
        /// </summary>
        /// <param name="appearanceEditor"></param>
        protected override void AddAppearanceEditorToControl(IAppearanceEditor appearanceEditor)
        {
            _appearanceEditorPlace.Controls.Clear();
            _appearanceEditorPlace.Controls.Add((Control)appearanceEditor);
        }


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

            if (!isPagePostBack)
                StartTab = hidePreview ? 1 : 0;

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is TextItemDecorator)
            {

                _questionTextEditor.Initialize(((TextItemDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _behaviorEditor.Initialize((TextItemDecorator)TextDecorator, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);

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

            if (TextDecorator is TextItemDecorator)
            {
                ((TextItemDecorator)TextDecorator).Text = _questionTextEditor.Text;

                _behaviorEditor.UpdateData((TextItemDecorator)TextDecorator);
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
            return _behaviorEditor.Validate();
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
            return true;
        }
    }
}