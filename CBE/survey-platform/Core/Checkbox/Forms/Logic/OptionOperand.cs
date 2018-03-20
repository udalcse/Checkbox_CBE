using System;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Exposes the ID of an <see cref="ListOption"/> for value comparison.  Currently, only option ids
    /// are used for comparison.
    /// </summary>
    [Serializable]
    public class OptionOperand : Operand
    {
        /// <summary>
        /// Construct an option operand
        /// </summary>
        public OptionOperand(int optionId)
        {
            OptionId = optionId;
        }

        /// <summary>
        /// Get the option id
        /// </summary>
        public int OptionId { get; private set; }

        /// <summary>
        /// Get the option value
        /// </summary>
        /// <returns></returns>
        protected override object GetValue(Response response)
        {
            return OptionId;
        }

        /// <summary>
        /// Create the operand value object
        /// </summary>
        /// <returns></returns>
        public override OperandValue CreateOperandValue()
        {
            return new SelectOperandValue();
        }
    }
}
