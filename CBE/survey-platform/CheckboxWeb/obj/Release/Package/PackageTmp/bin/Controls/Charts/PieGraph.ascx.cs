using Checkbox.Common;
using System;
using System.Drawing;

namespace CheckboxWeb.Controls.Charts
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PieGraph : ChartControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual string[] Colors
        {
            get
            {
                if (_colors != null)
                    return _colors;

                _colors = ParsePieGraphColors(Appearance["PieGraphColors"], Utilities.AsInt(Appearance["Transparency"], 100));
                return _colors;
            }
        }
    }
}