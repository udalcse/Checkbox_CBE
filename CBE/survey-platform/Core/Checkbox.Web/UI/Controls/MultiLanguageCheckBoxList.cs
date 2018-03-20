using System;
using System.Web.UI.WebControls;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// CheckBox list supporting localized values
    /// </summary>
    public class MultiLanguageCheckBoxList : CheckBoxList, IMultiLanguageControl
    {
        /// <summary>
        /// Set option localized values on pre render
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
            set { }
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
