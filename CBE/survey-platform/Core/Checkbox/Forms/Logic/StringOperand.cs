using System;
using System.Collections.Generic;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand representing free text string.
    /// </summary>
    [Serializable()]
    public class StringOperand : Operand
    {
        private string _value;

        /// <summary>
        /// Construct a new string operand
        /// </summary>
        /// <param name="value"></param>
        public StringOperand(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Get a value for this operand
        /// </summary>
        /// <returns></returns>
        protected override object GetValue(Response response)
        {
            return _value;
        }
    }
}
