using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Data type represented by logic operands
    /// </summary>
    [Flags]
    public enum OperandDataType
    {
        /// <summary>
        /// Operand supports string comparisons
        /// </summary>
        String = 0x0001,

        /// <summary>
        /// Operand supports integer comparisons
        /// </summary>
        Integer = 0x0002,

        /// <summary>
        /// Operand supports double comparisons
        /// </summary>
        Double = 0x0004,

        /// <summary>
        /// Operand supports date comparisons
        /// </summary>
        Date = 0x0008,

        /// <summary>
        /// Operand supports currency comparisons
        /// </summary>
        Currency = 0x0010,

        /// <summary>
        /// Operand supports option comparisons
        /// </summary>
        Option = 0x0020,

        /// <summary>
        /// Operand supports another type of comparison
        /// </summary>
        Other = 0x0040
    }
}
