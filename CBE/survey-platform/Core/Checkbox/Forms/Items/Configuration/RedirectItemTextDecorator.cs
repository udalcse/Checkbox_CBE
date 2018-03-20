using System;
using System.Collections.Generic;

using Prezza.Framework.Common;


namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Localized text decorator for a Redirect Item
    /// </summary>
    [Serializable]
    public class RedirectItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _linkText;
        private bool _linkTextChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public RedirectItemTextDecorator(RedirectItemData data, string language)
            : base(data, language)
        {
            _linkText = string.Empty;
            _linkTextChanged = false;
        }

        /// <summary>
        /// Get the data for the item
        /// </summary>
        new public RedirectItemData Data
        {
            get
            {
                return (RedirectItemData)base.Data;
            }
        }

        /// <summary>
        /// Set the localized texts for the item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            base.SetLocalizedTexts();

            if (_linkTextChanged)
            {
                SetText(Data.URLTextID, _linkText);
            }
        }

        /// <summary>
        /// Get/set the text for the link
        /// </summary>
        public string LinkText
        {
            get
            {
                if (!_linkTextChanged && Data.URLTextID != string.Empty)
                {
                    return GetText(Data.URLTextID);
                }
                
                return _linkText;
            }

            set
            {
                _linkText = value;
                _linkTextChanged = true;
            }
        }

        /// <summary>
        /// Copy the localized text associated with thisitem
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(RedirectItemData));

            string textID = ((RedirectItemData)data).URLTextID;

            if (textID != null && textID.Trim() != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(textID);

                foreach (string key in texts.Keys)
                {
                    SetText(Data.URLTextID, texts[key], key);
                }
            }
        }
    }
}
