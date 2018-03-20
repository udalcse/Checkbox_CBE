using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Enumeration of types of objects that can be targets of rule actions.
    /// </summary>
    public enum ActionReceiverType
    {
        /// <summary>
        /// Target is an item in a template.
        /// </summary>
        Item = 1,

        /// <summary>
        /// Target is a page.
        /// </summary>
        Page = 2
    }
}
