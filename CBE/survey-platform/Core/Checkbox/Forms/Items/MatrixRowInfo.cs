using System;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Simple container for matrix row information
    /// </summary>
    /// <remarks>Auto get/set properties are used instead of public fields since properties
    /// are required for DataBinder.Eval(...) syntax which is useful for item renderers.</remarks>
    [Serializable]
    public class MatrixRowInfo
    {
        /// <summary>
        /// Get/set matrix row number
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Get/set row's type
        /// </summary>
        public RowType RowType { get; set; }

        /// <summary>
        /// Get/set row text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Get/set row alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Create and populate an object to use for data transfer
        /// </summary>
        /// <returns></returns>
        public MatrixItemRow GetDataTransferObject()
        {
            return new MatrixItemRow
            {
                RowNumber = RowNumber,
                RowType = RowType.ToString(),
                Alias = Alias,
                Text = Text
            };
        }
    }

}
