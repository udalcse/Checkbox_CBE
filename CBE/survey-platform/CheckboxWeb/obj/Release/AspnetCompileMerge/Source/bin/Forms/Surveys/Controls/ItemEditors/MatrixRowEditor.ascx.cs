using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using System;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowEditor : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Matrix id
        /// </summary>
        public MatrixItemTextDecorator MatrixItemTextDecorator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PagePosition { get; set; }

        /// <summary>
        /// Fires before redirect to html options editor
        /// </summary>
        public event EventHandler OnHtmlEditorRedirect;

        /// <summary>
        /// Matrix item to edit
        /// </summary>
        /// <param name="matrixItemTextDecorator"></param>
        /// <param name="surveyId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="languageCode"></param>
        public void Initialize(int surveyId, int pagePosition, MatrixItemTextDecorator matrixItemTextDecorator, EditMode editMode, string languageCode)
        {
            MatrixItemTextDecorator = matrixItemTextDecorator;
            SurveyId = surveyId;
            PagePosition = pagePosition;

           _normalEntry.Initialize(surveyId, pagePosition, matrixItemTextDecorator, editMode, languageCode);
           _normalEntry.OnHtmlEditorRedirect +=  _normalEntry_OnHtmlEditorRedirect;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _normalEntry_OnHtmlEditorRedirect(object sender, System.EventArgs e)
        {
            _normalEntry.UpdateData(MatrixItemTextDecorator);
            if (OnHtmlEditorRedirect != null)
            {
                OnHtmlEditorRedirect(sender, e);
            }            
        }

        /// <summary>
        /// Update item with configured options
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(MatrixItemTextDecorator textDecorator)
        {
                _normalEntry.UpdateData(textDecorator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //TODO: NormalEntry and QuickEntryValidation
            return true;
        }
    }
}