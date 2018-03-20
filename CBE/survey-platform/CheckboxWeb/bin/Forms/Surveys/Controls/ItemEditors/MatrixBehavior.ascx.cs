using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Editor widget for configuring radio button behaviors
    /// </summary>
    public partial class MatrixBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize the behavior editor with the specified item data
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="showRandomize"></param>
        /// <param name="showAllOther"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(MatrixItemTextDecorator textDecorator)
        {
            _aliasText.Text = textDecorator.Data.Alias;
        }

        /// <summary>
        /// Initialize the editor item id
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(MatrixItemTextDecorator textDecorator)
        {
            textDecorator.Data.Alias = _aliasText.Text;
        }
    }
}