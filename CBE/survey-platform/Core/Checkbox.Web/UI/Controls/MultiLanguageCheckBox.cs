/****************************************************************************
 * Multilanguage implementation of a checkbox control.                      *
 * **************************************************************************/

using System;
using System.Web.UI.WebControls;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Checkbox supporting localized values for text and tooltip
    /// </summary>
    public class MultiLanguageCheckBox : CheckBox, IMultiLanguageControl
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

        #region IMultiLanguageControl Members

        /// <summary>
        /// Get/set the language code to use when displaying text for this control.
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Get/set the text id associated with this control's text
        /// </summary>
        public string TextId { get; set; }

        /// <summary>
        /// Get/set the id associated with this control's tooltip text
        /// </summary>
        public string ToolTipTextId { get; set; }

        #endregion
    }
}
