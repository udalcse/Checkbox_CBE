using System;
using System.Collections.Generic;

namespace Checkbox.Forms
{
    /// <summary>
    /// Compare template pages based on position.
    /// </summary>
    public class TemplatePageComparer : IComparer<TemplatePage>
    {
        #region IComparer<TemplatePage> Members

        /// <summary>
        /// Compare template pages based on position.
        /// </summary>
        public int Compare(TemplatePage x, TemplatePage y)
        {
            if (x.Position < y.Position)
            {
                return -1;
            }
            else if (x.Position > y.Position)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
