using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand with a value equal to the sum of the answers for a list of items.
    /// </summary>
    public class SumOperand : ItemDependentOperand
    {
        private readonly List<int> _answerableItemIds;

        /// <summary>
        /// Construct a list of items
        /// </summary>
        /// <param name="answerableItemIds"></param>
        public SumOperand(List<int> answerableItemIds)
        {
            _answerableItemIds = answerableItemIds;
        }

        /// <summary>
        /// Create the operand value object
        /// </summary>
        /// <returns></returns>
        public override OperandValue CreateOperandValue()
        {
            return new SumOperandValue();
        }

        /// <summary>
        /// Get a list of values
        /// </summary>
        /// <returns></returns>
        protected override object GetValue(Response response)
        {
            List<double> values = new List<double>();

            foreach (int answerableItemId in _answerableItemIds)
            {
                IAnswerable answerableItem = response.GetItem(answerableItemId) as IAnswerable;

                if (answerableItem == null)
                {
                    continue;
                }

                double? answerDouble = Utilities.GetDouble(answerableItem.GetAnswer());

                if (answerDouble.HasValue)
                {
                    values.Add(answerDouble.Value);
                }             
            }

            return values;
        }

        /// <summary>
        /// Item ID
        /// </summary>
        public override List<int> SourceItemIds
        {
            get { return _answerableItemIds; }
        }
    }
}
