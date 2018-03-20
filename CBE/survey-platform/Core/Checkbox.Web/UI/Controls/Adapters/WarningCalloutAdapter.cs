using System.Web.UI;
using System.Web.UI.WebControls.Adapters;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Adapters
{
    /// <summary>
    /// Adapter to simplify rendering of warning callouts for non-IE browsers
    /// </summary>
    public class WarningCalloutAdapter : WebControlAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (Control is WarningCallout)
            {
                WarningCallout calloutControl = (WarningCallout)Control;
                
                writer.Write("<span class=\"");
                writer.Write(calloutControl.TextCssClass);
                writer.Write("\"");

                string displayStyle = calloutControl.Style[HtmlTextWriterStyle.Display];

                if (Utilities.IsNotNullOrEmpty(displayStyle))
                {
                    //Change inline display to block
                    if ("block".Equals(displayStyle, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        displayStyle = "inline";
                    }

                    writer.Write(" style=\"display:");
                    writer.Write(displayStyle);
                    writer.Write(";\"");
                }

                writer.Write(">");
                writer.Write(calloutControl.Text);
                writer.Write("</span>");
            }
            else
            {
                base.Render(writer);
            }
        }
    }
}
