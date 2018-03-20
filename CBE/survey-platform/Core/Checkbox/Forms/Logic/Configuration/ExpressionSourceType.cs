using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Simple enum for storing source type in a more easily queryable value
    /// </summary>
    public enum ExpressionSourceType
    {
        Question = 0,
        CategorizedType = 1,
        UserAttribute = 2,
        ResponseProperty = 3,
        SourceTypeNotSpecified = 4
    }


}
