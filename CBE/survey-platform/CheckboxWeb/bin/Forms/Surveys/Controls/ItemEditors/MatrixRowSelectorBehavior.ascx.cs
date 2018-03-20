using System;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixRowSelectorBehavior : Checkbox.Web.Common.UserControlBase
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
        /// Initialize the control
        /// </summary>
        public void Initialize(SelectItemTextDecorator decorator, bool isPagePostBack)
        {
            if (!isPagePostBack)
            {
                var data = decorator.Data as RowSelectData;
                _aliasText.Text = data.Alias;

                if (data.AllowMultipleSelection)
                {
                    _selectionType.Items.FindByValue("Multiple").Selected = true;
                    _minToSelectTxt.Text = data.MinToSelect.ToString();
                    _maxToSelectTxt.Text = data.MaxToSelect.ToString();
                    _singleSelectionTypePanel.Style["display"] = "none";
                }
                else
                {
                    _selectionType.Items.FindByValue("Single").Selected = true;
                    _answerRequired.Checked = data.IsRequired;
                    _multipleSelectionTypePanel.Style["display"] = "none";
                }
            }
        }

        /// <summary>
        /// Update the data
        /// </summary>
        public void UpdateData(SelectItemTextDecorator decorator)
        {
            var data = decorator.Data as RowSelectData;

            data.Alias = _aliasText.Text;
            data.AllowMultipleSelection = _selectionType.SelectedValue == "Multiple";

            if (data.AllowMultipleSelection)
            {
                data.IsRequired = false;
                data.MaxToSelect = null;
                data.MinToSelect = null;
                
                int temp;

                if (int.TryParse(_minToSelectTxt.Text, out temp))
                    data.MinToSelect = temp;

                if (int.TryParse(_maxToSelectTxt.Text, out temp))
                    data.MaxToSelect = temp;
            }
            else
            {
                data.IsRequired = _answerRequired.Checked;
                data.MinToSelect = data.MaxToSelect = null;
            }
        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            int min, max;
            bool result = true;
            
            //Ensure that min <= max
            if (int.TryParse(_minToSelectTxt.Text, out min) && int.TryParse(_maxToSelectTxt.Text, out max))
                result = min <= max;

            _maxMinError.Visible = !result;

            return result;
        }
    }
}