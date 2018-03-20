using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Interface definition for class that compares to locigal operand values.
    /// </summary>
    public interface IOperandComparer
    {
        /// <summary>
        /// Compares two <see cref="Operand"/> instances based on a <see cref="LogicalOperator"/>
        /// </summary>
        /// <remarks>
        /// Implementers of IOperandComparer should take care to ensure thread-safety
        /// </remarks>
        /// <param name="left">the left <see cref="Operand"/></param>
        /// <param name="operation">the <see cref="LogicalOperator"/></param>
        /// <param name="right">the right <see cref="Operand"/></param>
        /// <returns>true, if comparable; otherwise false</returns>
        bool Compare(Operand left, LogicalOperator operation, Operand right);
    }
}
