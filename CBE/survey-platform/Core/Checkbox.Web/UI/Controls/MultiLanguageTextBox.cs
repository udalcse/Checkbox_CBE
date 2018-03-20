/****************************************************************************
 * Control based on the asp:Label that supports multilanguage functionality	*
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;

using Checkbox.Web;

namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Textbox supporting localized tooltip
	/// </summary>
	public class MultiLanguageTextBox : TextBox, IMultiLanguageControl
	{
	    /// <summary>
        /// Set localized text on pre render
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ToolTip = WebTextManager.GetText(ToolTipTextId, LanguageCode, ToolTip);
        }

	    /// <summary>
	    /// Get/Set the language code for the text string to be displayed
	    /// </summary>
	    public string LanguageCode { get; set; }

	    /// <summary>
	    /// Get/Set the text id associated with this tool tip
	    /// </summary>
	    public string ToolTipTextId { get; set; }

	    /// <summary>
		/// Added for conformance to IMultiLanguageControl interface
		/// </summary>
		public string TextId
		{
			get{ return string.Empty; }
			set{ }
		}

	}
}
