/****************************************************************************
 * Control based on the asp:Label that supports multilanguage functionality	*
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Button supporting localized text values for text and tooltip
    /// </summary>
    public class MultiLanguageButton : Button, IMultiLanguageControl
    {
        /// <summary>
        /// Ensures that localized strings were uploaded and if not uploads them.
        /// 
        /// OnPreRender is never being called if parent control calls Render method, so we
        /// have to check Text and Tooltip here.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (Utilities.IsNotNullOrEmpty(TextId) && string.IsNullOrEmpty(Text))
            {
                Text = WebTextManager.GetText(TextId, LanguageCode, Text);
            }

            if (Utilities.IsNotNullOrEmpty(ToolTipTextId) && string.IsNullOrEmpty(ToolTip))
            {
                ToolTip = WebTextManager.GetText(ToolTipTextId, LanguageCode, ToolTip);
            }

            base.Render(writer);
        }

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
