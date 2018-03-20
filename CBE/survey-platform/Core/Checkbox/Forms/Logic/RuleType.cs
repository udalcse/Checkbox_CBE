using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Rule type enumeration.
    /// </summary>
    [Serializable]
    public enum RuleType
    {
        /// <summary>
        /// Rule is associated with conditional display of an item.
        /// </summary>
        ItemCondition,

        /// <summary>
        /// Rule is associated with conditional display of a page.
        /// </summary>
        PageCondition,

        /// <summary>
        /// Rule is associated with page branching logic.
        /// </summary>
        PageBranchCondition
    }
}
