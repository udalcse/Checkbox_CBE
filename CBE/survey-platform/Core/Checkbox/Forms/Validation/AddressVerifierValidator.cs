using System;
using System.Globalization;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Validate the address
    /// </summary>
    public class AddressVerifierValidator : TextAnswerValidator
    {
        /// <summary>
        /// Perform base validation. For now does nothing special
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool Validate(TextItem input)
        {
            if (!base.Validate(input))
            {
                return false;
            }

            return true;
        }
    }
}
