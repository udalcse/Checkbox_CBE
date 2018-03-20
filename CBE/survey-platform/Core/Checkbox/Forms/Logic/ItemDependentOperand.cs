using System;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ItemDependentOperand : Operand
    {
        /// <summary>
        /// Item ID
        /// </summary>
        public abstract List<int> SourceItemIds
        {
            get;
        }
    }
}
