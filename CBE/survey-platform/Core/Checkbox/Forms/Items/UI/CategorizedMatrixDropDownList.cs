using System;
namespace Checkbox.Forms.Items.UI
{
    [Serializable]
    public class CategorizedMatrixDropDownList : MatrixSelectLayout
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CATEGORIZED_MATRIX_DROPDOWN_LIST"; }
        }
    }
}