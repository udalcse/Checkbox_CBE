/****************************************************************************
 * MultiLanguage implementation of a link button.                           *
 * **************************************************************************/
using System;
using System.Web.UI.WebControls;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Link buttons supporting localized text and tooltip
    /// </summary>
    public class MultiLanguageLinkButton : LinkButton, IMultiLanguageControl
    {
        /// <summary>
        /// Set localized text values on pre render
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
        /// Get/set text id for link text
        /// </summary>
        public string TextId { get; set; }

        /// <summary>
        /// Get/set text id for tooltip
        /// </summary>
        public string ToolTipTextId { get; set; }

        /// <summary>
        /// Get/set current language code
        /// </summary>
        public string LanguageCode { get; set; }

        #endregion
    }
}
