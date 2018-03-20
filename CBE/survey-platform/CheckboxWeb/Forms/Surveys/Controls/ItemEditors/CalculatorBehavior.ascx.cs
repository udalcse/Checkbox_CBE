using System;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class CalculatorBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected EditMode EditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int TemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Initialize pipeSelector
            switch (EditMode)
            {
                case EditMode.Survey:
                    _pipeSelector.Initialize(TemplateId, PagePosition, LanguageCode, _formula.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelector.Initialize(null, null, LanguageCode, _formula.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelector.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Initialize the editor with the data
        /// </summary>
        /// <param name="itemTextDecorator"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="currentLanguage"></param>
        public void Initialize(LabelledItemTextDecorator itemTextDecorator, int templateId, int? pagePosition, EditMode editMode, string currentLanguage)
        {
            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;
            LanguageCode = currentLanguage;

            var itemData = itemTextDecorator.Data as CalculatorItemData;

            if (itemData == null)
                return;

            _formula.Text = itemData.Formula;

            _roundToList.SelectedValue = itemData.RoundToPlaces.ToString();

        }

        
        /// <summary>
        /// Update data with user inputs
        /// </summary>
        /// <param name="itemTextDecorator"></param>
        public void UpdateData(LabelledItemTextDecorator itemTextDecorator)
        {
            var itemData = itemTextDecorator.Data as CalculatorItemData;

            if (itemData == null)
                return;

            itemData.Formula = _formula.Text;

            itemData.RoundToPlaces = int.Parse(_roundToList.SelectedValue);
        }

        /// <summary>
        /// Validate the behavior of calculator item
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return true;
        }

    }
}