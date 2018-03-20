using System;
using System.Globalization;

using Checkbox.Common;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand value where the comparison is made against a sum of values
    /// </summary>
    public class SumOperandValue : GroupOperandValue<double>
    {
        /// <summary>
        /// Get the value of the item
        /// </summary>
        public override object Value
        {
            get
            {
                double sum = 0;

                foreach (double value in Values)
                {
                    sum += value;
                }

                return sum;
            }
        }

        /// <summary>
        /// Compare to another group
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected override int CompareTo(GroupOperandValue<double> other)
        {
            throw new NotImplementedException("Comparison against lists of items not supported.");
        }
    }
}
