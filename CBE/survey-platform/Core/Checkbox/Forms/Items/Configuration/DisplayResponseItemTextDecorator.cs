using System;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for display response item
    /// </summary>
    [Serializable]
    public class DisplayResponseItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _linkText;
        private bool _linkTextChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public DisplayResponseItemTextDecorator(DisplayResponseItemData data, string language)
            : base(data, language)
        {
            _linkText = string.Empty;
            _linkTextChanged = false;
        }

        /// <summary>
        /// Get the data for the item
        /// </summary>
        new public DisplayResponseItemData Data
        {
            get
            {
                return (DisplayResponseItemData)base.Data;
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
                SetText(Data.LinkTextID, _linkText);
            }
        }

        /// <summary>
        /// Get/set the text for the link
        /// </summary>
        public string LinkText
        {
            get
            {
                if (!_linkTextChanged && Data.LinkTextID != string.Empty)
                {
                    return GetText(Data.LinkTextID);
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
        /// Copy the text associated with the passed-in item to this item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(DisplayResponseItemData));

            string textID = ((DisplayResponseItemData)data).LinkTextID;

            if (textID != null && textID.Trim() != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(textID);

                foreach (string key in texts.Keys)
                {
                    SetText(Data.LinkTextID, texts[key], key);
                }
            }
        }
    }
}
