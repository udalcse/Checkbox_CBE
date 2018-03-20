using System;
using System.Web.UI;
using Checkbox.Globalization.Text;
using Checkbox.Styles;

namespace CheckboxWeb.Styles.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class StylePreview : Checkbox.Web.Common.UserControlBase
    {
        public string FooterHtml { get; set; }
        public string HeaderHtml { get; set; }
    }
}