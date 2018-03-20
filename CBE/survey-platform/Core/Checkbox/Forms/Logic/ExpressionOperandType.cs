using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Type information for expression operands.
    /// </summary>
    public enum ExpressionOperandType
    {
        /// <summary>
        /// Operand is a survey item
        /// </summary>
        Item,

        /// <summary>
        /// Operand is a user attribute
        /// </summary>
        Profile,

        /// <summary>
        /// Operand is a property of a survey response
        /// </summary>
        Response
    }
}
