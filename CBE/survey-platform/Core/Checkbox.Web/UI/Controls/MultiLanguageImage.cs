/****************************************************************************
 * Control based on the asp:Image that supports multilanguage functionality	*
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;

using Checkbox.Web;

namespace Checkbox.Web.UI.Controls
{
	/// <summary>
	/// Image supporting localized tooltip text
	/// </summary>
	public class MultiLanguageImage : Image, IMultiLanguageControl
	{
	    /// <summary>
        /// Set localized text on prerender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ToolTip = WebTextManager.GetText(ToolTipTextId, LanguageCode, ToolTip);
            AlternateText = WebTextManager.GetText(ToolTipTextId, LanguageCode, AlternateText);
        }

	    /// <summary>
	    /// Get/Set the text id of alternate text for this control
	    /// </summary>
	    public string ToolTipTextId { get; set; }

	    /// <summary>
	    /// Get/Set the language code for the text string to be displayed
	    /// </summary>
	    public string LanguageCode { get; set; }

	    /// <summary>
		/// Added for conformance to IMultiLanguageControl interface
		/// </summary>
		public string TextId
		{
			get{ return string.Empty; }
			set{}
		}
	}
}
