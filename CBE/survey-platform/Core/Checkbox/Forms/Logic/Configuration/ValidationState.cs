using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Validation state of a configuration file.
    /// </summary>
    public enum ValidationState
    {
        /// <summary>
        /// Configuration is invalid.
        /// </summary>
        Invalid = 1,

        /// <summary>
        /// Unknown or not yet validated.
        /// </summary>
        Unknown,

        /// <summary>
        /// State is valid.
        /// </summary>
        Valid
    }
}
