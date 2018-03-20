using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Checkbox.Common
{
    /// <summary>
    /// Simple extension to a string writer to support stripping any well-formed HTML/XML
    /// markup from the string to write.
    /// </summary>
    /// <remarks>Overrides the Write(string) and WriteLine(string) methods.</remarks>
    public class HtmlStrippingStringWriter : StringWriter
    {
        /// <summary>
        /// Get/set whether to strip HTML.
        /// </summary>
        /// <remarks>Default value is false.</remarks>
        public bool StripHtml { get; set; }

        /// <summary>
        /// Write a string value
        /// </summary>
        /// <param name="value">Value to write.</param>
        public override void Write(string value)
        {
            if (StripHtml)
            {
                base.Write(Utilities.StripHtml(value, null));
            }
            else
            {
                base.Write(value);
            }
        }

        /// <summary>
        /// Write a string as a separate line.
        /// </summary>
        /// <param name="value">Line to write.</param>
        public override void WriteLine(string value)
        {
            if (StripHtml)
            {
                base.WriteLine(Utilities.StripHtml(value, null));
            }
            else
            {
                base.WriteLine(value);
            }

        }
    }
}
