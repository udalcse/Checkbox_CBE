using System;
using Checkbox.Common;

namespace CheckboxWeb.Controls.Charts
{
    public partial class DoughnutGraph : ChartControlBase
    {
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