using System;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Logic;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MatrixSumTotalBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize the behavior editor with the specified item data
        /// </summary>
        /// <param name="textItemDecorator"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(TextItemDecorator textItemDecorator, bool isPagePostBack, string currentLanguage, int templateId, int? pagePosition, EditMode editMode)
        {

            var mstItemData = (MatrixSumTotalItemData)textItemDecorator.Data;
            _total.Text = mstItemData.TotalValue.ToString();
            _operator.SelectedValue = mstItemData.ComparisonOperator.ToString();

            _singleLineBehaviour.Initialize(textItemDecorator, true, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
        }

        /// <summary>
        /// Update the item data with the selected values
        /// </summary>
        /// <returns></returns>
        public void UpdateData(TextItemDecorator textItemDecorator)
        {

            _totalErrorLbl.Visible = false;

            if (_total.Text == null || _total.Text.Trim() == string.Empty)
            {
                _totalErrorLbl.Visible = true;
                return;
            }

            try
            {
                ((MatrixSumTotalItemData)textItemDecorator.Data).TotalValue = Convert.ToDouble(_total.Text);
                _singleLineBehaviour.UpdateData(textItemDecorator);

            }
            catch
            {
                _totalErrorLbl.Visible = true;
                return;
            }

            ((MatrixSumTotalItemData)textItemDecorator.Data).ComparisonOperator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), _operator.SelectedValue);
        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            _totalErrorLbl.Visible = false;

            if (_total.Text == null || _total.Text.Trim() == string.Empty)
            {
                _totalErrorLbl.Visible = true;
                return false;
            }
            return true;
        }
    }
}