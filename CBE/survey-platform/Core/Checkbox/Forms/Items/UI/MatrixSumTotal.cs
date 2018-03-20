using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.UI
{

    /// <summary>
    /// Appearance data for MatrixSumTotal
    /// </summary>
    [Serializable]
    class MatrixSumTotal : SingleLineText
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_SINGLE_LINE_TEXT"; }
        }
    }
}
