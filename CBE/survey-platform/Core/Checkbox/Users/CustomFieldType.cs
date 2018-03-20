using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Users
{
    /// <summary>
    /// Profile field types
    /// </summary>
    public enum CustomFieldType
    {
        /// <summary>
        /// The single line field
        /// </summary>
        SingleLine = 1,

        /// <summary>
        /// The multi line
        /// </summary>

        MultiLine = 2,

        /// <summary>
        /// The radio button field
        /// </summary>
        RadioButton = 3,

		 /// <summary>
        /// Matrix field type 
        /// </summary>
        Matrix = 4 ,

        /// <summary>
        /// The email
        /// </summary>
        Email = 5

    }
}
