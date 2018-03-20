/****************************************************************************
 * MultiLanguage implementation of a drop-down list.                        *
 ****************************************************************************/
using System;
using System.Web.UI.WebControls;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Dropdown list supporting multi language contents
    /// </summary>
    public class MultiLanguageDropDownList : DropDownList, IMultiLanguageControl
    {
        /// <summary>
        /// Set list item texts to be localized texts on prerender
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (ListItem item in Items)
            {
                string textId = item.Attributes["TextId"];

                if (Utilities.IsNotNullOrEmpty(textId))
                {
                    string text = WebTextManager.GetText(textId, LanguageCode, item.Text);

                    if (Utilities.IsNotNullOrEmpty(text))
                    {
                        item.Text = text;
                    }
                }
            }
        }
        #region IMultiLanguageControl Members

        /// <summary>
        /// Get/Set the language code to use when rendering this control
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Placeholder for interface.  Not implemented
        /// </summary>
        public string ToolTipTextId
        {
            get { return string.Empty; }
            set {}
        }

        /// <summary>
        /// Placeholder for interface.  Not implemented
        /// </summary>
        public string TextId
        {
            get { return string.Empty; }
            set { }
        }

        #endregion
    }
}
