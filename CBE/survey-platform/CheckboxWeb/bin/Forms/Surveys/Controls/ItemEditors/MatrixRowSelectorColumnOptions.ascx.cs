using System;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowSelectorColumnOptions : Checkbox.Web.Common.UserControlBase
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
        /// Initialize the control with specified itemData
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="columnPosition"></param>
        /// <param name="isPagePostBack"></param>
        public void Initialize(MatrixItemData itemData, int? columnPosition, bool isPagePostBack)
        {
            if (isPagePostBack)
            {
                return;
            }

            try
            {
                if (itemData == null || !columnPosition.HasValue)
                {
                    _columnWidthTextBox.Text = String.Empty;
                }
                else
                {
                    int? width = itemData.GetColumnWidth(columnPosition.Value);
                    if (width.HasValue)
                        _columnWidthTextBox.Text = width.ToString();
                }
            }
            catch (Exception)
            {
                _columnWidthTextBox.Text = String.Empty;
            }
        }

        /// <summary>
        /// Update the itemData
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="columnPosition"></param>
        public void UpdateData(MatrixItemData itemData, int columnPosition)
        {
            if (String.IsNullOrEmpty(_columnWidthTextBox.Text))
                itemData.SetColumnWidth(columnPosition, null);
            else
                itemData.SetColumnWidth(columnPosition, int.Parse(_columnWidthTextBox.Text));
        }
    }
}