/****************************************************************************
 * Control based on the asp:Label that supports multilanguage functionality	*
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Label supporting localized text value
	/// </summary>
	public class MultiLanguageLiteral : Literal, IMultiLanguageControl
	{
	    /// <summary>
        /// Set localized values on prerender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Utilities.IsNotNullOrEmpty(TextId))
            {
                Text = WebTextManager.GetText(TextId, LanguageCode, Text);
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
