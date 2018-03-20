using System.Collections.Generic;
using Checkbox.Forms.Items;
using Checkbox.Forms.Logic;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate the sum total of several items
    /// </summary>
    public class ItemSumValidator : Validator<List<IAnswerable>>
    {
        private readonly double _sumValue;
        private readonly LogicalOperator _operator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sumValue"></param>
        /// <param name="comparisonOperator"></param>
        public ItemSumValidator(double sumValue, LogicalOperator comparisonOperator)
        {
            _sumValue = sumValue;
            _operator = comparisonOperator;
        }

        /// <summary>
        /// Get the sum value
        /// </summary>
        public double SumValue
        {
            get { return _sumValue; }
        }

        /// <summary>
        /// Get the comparison operator
        /// </summary>
        public LogicalOperator Operator
        {
            get { return _operator; }
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns></returns>
        public override bool Validate(List<IAnswerable> itemsToValidate)
        {
            bool answered = false;
            List<int> itemIds = new List<int>();

            foreach (IAnswerable item in itemsToValidate)
            {
                var textItem = item as TextItem;
                if (item.HasAnswer && (textItem == null || item.GetAnswer() != textItem.DefaultText))
                {
                    answered = true;
                }

                if (item is ResponseItem)
                {
                    itemIds.Add(((Item)item).ID);
                }
            }

            //Check the value
            if (answered)
            {
                //Use the sum operand and operand comparer to do the work
                SumOperand leftOperand = new SumOperand(itemIds);
                StringOperand rightOperand = new StringOperand(SumValue.ToString());

                return OperandComparer.Compare(leftOperand, Operator, rightOperand,((ResponseItem)itemsToValidate[0]).Response);
            }
            
            return true;
        }

        /// <summary>
        /// Get the validation message
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string GetMessage(string languageCode)
        {
            string message = TextManager.GetText("/validationMessages/itemSumTotal/sumError", languageCode);

            if (message != null)
            {
                string operatorText = TextManager.GetText("/validationMessages/itemSumTotal/" + _operator, languageCode);

                if (operatorText != null)
                {
                    message = message.Replace("{operator}", operatorText);
                }

                message = message.Replace("{value}", _sumValue.ToString());
            }

            return message;
        }
    }
}
