using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowSelector : UserControlItemEditorBase
    {
        private MatrixItemTextDecorator _textDecorator;
        private MatrixItemData _matrixItemData;

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
            //Call base initialize.  If item is not new, not allow new text decorator to be created by setting preventDecoratorReuse parameter
            // to false.
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview, !data.ID.HasValue);

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is SelectItemTextDecorator)
            {
                _questionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _behaviorEditor.Initialize((SelectItemTextDecorator)TextDecorator, isPagePostBack);

                int? columnPosition = MatrixItemData == null || !ItemData.ID.HasValue ? null : MatrixItemData.GetColumnPosition(ItemData.ID.Value);
                _columnOptions.Initialize(MatrixItemData, columnPosition, isPagePostBack);
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
                var data = TextDecorator.Data as RowSelectData;
                var selectItemTextDecorator = TextDecorator as SelectItemTextDecorator;

                if (data.Options.Count == 0)
                {
                    string category = WebTextManager.GetText("/controlText/matrixRowSelector/category");
                    
                    //1 is assigned as the text and alias because this value will be displayed when answered result exports.
                    data.AddOption("1", category, false, 1, 0, false, false, null);
                    selectItemTextDecorator.SetOptionText(1, "1");
                }

                selectItemTextDecorator.Text = _questionTextEditor.Text;
                _behaviorEditor.UpdateData(selectItemTextDecorator);

                if (MatrixItemData != null && ItemData.ID.HasValue)
                {
                    int? columnPosition = MatrixItemData.GetColumnPosition(ItemData.ID.Value);
                    _columnOptions.UpdateData(MatrixItemData, columnPosition.Value);
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
    }
}