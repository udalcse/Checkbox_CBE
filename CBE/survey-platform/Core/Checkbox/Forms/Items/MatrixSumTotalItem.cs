using System;

using Checkbox.Forms.Logic;
using Checkbox.Forms.Items.Configuration;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Matrix sum total item
    /// </summary>
    [Serializable()]
    public class MatrixSumTotalItem : SingleLineTextBoxItem
    {
        private double _totalValue;
        private LogicalOperator _operator;

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckForNullReference(configuration, "MatrixSumTotalItemData");
            ArgumentValidation.CheckExpectedType(configuration, typeof(MatrixSumTotalItemData));

            base.Configure(configuration, languageCode, templateId);

            _totalValue = ((MatrixSumTotalItemData)configuration).TotalValue;
            _operator = ((MatrixSumTotalItemData)configuration).ComparisonOperator;            
        }

        /// <summary>
        /// Get the value
        /// </summary>
        public double TotalValue
        {
            get { return _totalValue; }
        }

        /// <summary>
        /// Get the comparison
        /// </summary>
        public LogicalOperator ComparisonOperator
        {
            get { return _operator; }
        }
    }
}
