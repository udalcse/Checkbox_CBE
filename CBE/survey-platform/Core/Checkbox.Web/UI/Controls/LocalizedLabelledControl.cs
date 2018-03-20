using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Labelled control supporting localization
    /// </summary>
    public class LocalizedLabelledControl : LabelledControl
    {
        private string _languageCode;
        private string _labelTextID;

        private Label _theLabel;

        /// <summary>
        /// Get the label
        /// </summary>
        protected Label Label
        {
            get 
            {
                if (_theLabel == null)
                {
                    _theLabel = new Label();
                }

                return _theLabel;
            }
        }

        /// <summary>
        /// Get/set the language code if the current user's language isn't to be used
        /// </summary>
        public virtual string LanguageCode
        {
            get 
            {
                if (Utilities.IsNullOrEmpty(_languageCode))
                {
                    return WebTextManager.GetUserLanguage();
                }
                else
                {
                    return _languageCode;
                }
            }
            set { _languageCode = value; }
        }

        /// <summary>
        /// Get/set css class for the label
        /// </summary>
        public virtual string LabelCssClass
        {
            get { return Label.CssClass; }
            set { Label.CssClass = value; }
        }

        /// <summary>
        /// Get/set label skin id
        /// </summary>
        public virtual string LabelSkinID
        {
            get { return Label.SkinID; }
            set { Label.SkinID = value; }
        }

        /// <summary>
        /// Get/set label text id
        /// </summary>
        public virtual string LabelTextID
        {
            get { return _labelTextID; }
            set { _labelTextID = value; }
        }

        /// <summary>
        /// Handle init by loading data
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            Label.Text = GetLabelText();

            base.OnInit(e);
        }

        /// <summary>
        /// Get the text for the label
        /// </summary>
        /// <returns></returns>
        protected virtual string GetLabelText()
        {
            if (Utilities.IsNotNullOrEmpty(LabelTextID))
            {
                if (Utilities.IsNotNullOrEmpty(LanguageCode))
                {
                   return TextManager.GetText(LabelTextID, LanguageCode);
                }
                else
                {
                    return WebTextManager.GetText(LabelTextID);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the label panel
        /// </summary>
        /// <returns></returns>
        protected override Control GetLabelControl()
        {
            if (LabelControl != null)
            {
                return LabelControl;
            }
            else
            {
                return Label;
            }
        }
    }
}
