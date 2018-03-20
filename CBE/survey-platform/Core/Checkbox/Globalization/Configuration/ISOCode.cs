/****************************************************************************
 * Simple class to contain an ISOCode name/value pair                       *
 ****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Globalization.Configuration
{
    /// <summary>
    /// Simple class to contain ISO code information
    /// </summary>
    [Serializable()]
    public class ISOCode
    {
        private string code;
        private string name;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ISOCode()
        {
            code = string.Empty;
            name = string.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">ISO name</param>
        /// <param name="code">ISO Code</param>
        public ISOCode(string name, string code)
        {
            this.name = name;
            this.code = code;
        }

        /// <summary>
        /// Get/Set the ISO Code
        /// </summary>
        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        /// <summary>
        /// Get/Set the code name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
