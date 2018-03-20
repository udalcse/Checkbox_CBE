using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.UI
{
    [Serializable]
    class MatrixMessage : MatrixSelectLayout
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_MESSAGE"; }
        }
    }
}
