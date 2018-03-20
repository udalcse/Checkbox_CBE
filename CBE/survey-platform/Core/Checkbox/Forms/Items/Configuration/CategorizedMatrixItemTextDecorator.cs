using System;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for a matrix item
    /// </summary>
    [Serializable]
    public class CategorizedMatrixItemTextDecorator : MatrixItemTextDecorator
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="language"></param>
        public CategorizedMatrixItemTextDecorator(CategorizedMatrixItemData itemData, string language)
            : base(itemData, language) { }

        /// <summary>
        /// Get the data
        /// </summary>
        public new CategorizedMatrixItemData Data
        {
            get { return (CategorizedMatrixItemData)base.Data; }
        }
    }
}