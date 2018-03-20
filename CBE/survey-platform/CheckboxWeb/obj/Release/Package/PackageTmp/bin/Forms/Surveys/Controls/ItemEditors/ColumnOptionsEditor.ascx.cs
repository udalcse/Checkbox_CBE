using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class ColumnOptionsEditor : Checkbox.Web.Common.UserControlBase
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

            _uniqueAnswerDropDownList.Items.Add(new ListItem(WebTextManager.GetText("/common/no"), "NO"));
            _uniqueAnswerDropDownList.Items.Add(new ListItem(WebTextManager.GetText("/common/yes"), "YES"));

            if (!isPagePostBack)
            {
                try
                {
                    if (itemData == null || !columnPosition.HasValue)
                    {
                        _uniqueAnswerDropDownList.SelectedValue = "NO";
                        _columnWidthTextBox.Text = String.Empty;
                    }
                    else
                    {
                        _uniqueAnswerDropDownList.SelectedValue = itemData.GetColumnUniqueness(columnPosition.Value)
                                                                      ? "YES"
                                                                      : "NO";
                        int? width = itemData.GetColumnWidth(columnPosition.Value);
                        if (width.HasValue)
                            _columnWidthTextBox.Text = width.ToString();
                    }
                }
                catch (Exception)
                {
                    _uniqueAnswerDropDownList.SelectedValue = "NO";
                    _columnWidthTextBox.Text = String.Empty;
                }

            }
        }

        /// <summary>
        /// Update the itemData
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="columnPosition"></param>
        public void UpdateData(MatrixItemData itemData, int columnPosition)
        {
            itemData.SetColumnUniqueness(columnPosition, _uniqueAnswerDropDownList.SelectedValue == "YES");

            if (String.IsNullOrEmpty(_columnWidthTextBox.Text))
                itemData.SetColumnWidth(columnPosition, null);
            else
                itemData.SetColumnWidth(columnPosition, int.Parse(_columnWidthTextBox.Text));
        }
    }
}