using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowEditWidget : UserControl
    {
        /// <summary>
        /// Set initial empty texts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _rowText.EmptyMessage = WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionEditWidget/enterChoiceText");
            _rowAlias.EmptyMessage = WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/enterAlias");
        }

        /// <summary>
        /// Simple widget for editing list options
        /// </summary>
        /// <param name="rowInfo"></param>
        public void Initialize(MatrixRowEditor.RowEditorBindingObject rowInfo)
        {
            _rowText.Text = rowInfo.Text;

            if (_rowTypeList.Items.FindByValue(rowInfo.RowType.ToString()) != null)
            {
                _rowTypeList.SelectedValue = rowInfo.RowType.ToString();
            }
            else
            {
                _rowTypeList.SelectedValue = "Normal";
            }
            
            _rowAlias.Text = rowInfo.Alias;
            _deleteBtn.CommandArgument = rowInfo.RowNumber.ToString();
        }

        /// <summary>
        /// Update option
        /// </summary>
        /// <param name="rowInfo"></param>
        public void UpdateRow(MatrixRowEditor.RowEditorBindingObject rowInfo)
        {
            rowInfo.RowType = (RowType)Enum.Parse(typeof(RowType), _rowTypeList.SelectedValue);
            rowInfo.Text = _rowText.Text.Trim();
            rowInfo.Alias = _rowAlias.Text.Trim();
        }
    }
}