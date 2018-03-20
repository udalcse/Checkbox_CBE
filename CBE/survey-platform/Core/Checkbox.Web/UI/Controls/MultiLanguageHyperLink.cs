/****************************************************************************
 * Control based on the asp:Label that supports multilanguage functionality	*
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;

using Checkbox.Web;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Hyperlink supporting localized text and tooltips.
	/// </summary>
	public class MultiLanguageHyperLink : HyperLink,IMultiLanguageControl
	{
	    /// <summary>
        /// Set localized text on prerender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Utilities.IsNotNullOrEmpty(TextId))
            {
                Text = WebTextManager.GetText(TextId, LanguageCode, Text);
            }

            if (Utilities.IsNotNullOrEmpty(ToolTipTextId))
            {
                ToolTip = WebTextManager.GetText(ToolTipTextId, LanguageCode, ToolTip);
            }
        }

	    /// <summary>
	    /// Get/Set the identifier for the localized text string
	    /// </summary>
	    public string TextId { get; set; }

	    /// <summary>
	    /// Get/Set the language code for the text string to be displayed
	    /// </summary>
	    public string LanguageCode { get; set; }

	    /// <summary>
	    /// Get/Set the text id associated with this tool tip
	    /// </summary>
	    public string ToolTipTextId { get; set; }
	}
}
