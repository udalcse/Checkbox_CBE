/****************************************************************************
 * MultiLanguage implementation of an image button                          *
 * **************************************************************************/
using System;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Image button supporting multi language alt/tooltip text
    /// </summary>
    public class MultiLanguageImageButton : ImageButton, IMultiLanguageControl
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

        #region IMultiLanguageControl Members

        /// <summary>
        /// Place holder for property required by interface
        /// </summary>
        public string TextId
        {
            get { return string.Empty; }
            set {}
        }

        /// <summary>
        /// Get/Set the tool tip text id
        /// </summary>
        public string ToolTipTextId { get; set; }

        /// <summary>
        /// Get/Set the language code for this item
        /// </summary>
        public string LanguageCode { get; set; }

        #endregion
    }
}
