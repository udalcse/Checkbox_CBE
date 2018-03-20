using System;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for a text item
    /// </summary>
    [Serializable]
    public class TextItemDecorator : LabelledItemTextDecorator
    {
        private string _defaultText;
        private bool _defaultTextModified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public TextItemDecorator(TextItemData data, string language)
            : base(data, language)
        {
            _defaultText = string.Empty;
            _defaultTextModified = false;
        }

        /// <summary>
        /// Get the data for the decorator
        /// </summary>
        new public TextItemData Data
        {
            get { return (TextItemData)base.Data; }
        }

        /// <summary>
        /// Get/set the default text for the item
        /// </summary>
        public string DefaultText
        {
            get
            {
                if (!_defaultTextModified)
                {
                    return GetText(Data.DefaultTextID);
                }
                else
                {
                    return _defaultText;
                }
            }

            set
            {
                _defaultText = value;
                _defaultTextModified = true;
            }
        }

        /// <summary>
        /// Set the localized texts for the item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            base.SetLocalizedTexts();

            if (_defaultTextModified && Data.DefaultTextID != string.Empty)
            {
                SetText(Data.DefaultTextID, _defaultText);
            }
        }

        /// <summary>
        /// Copy localized text associated with this item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(TextItemData));

            string textID = ((TextItemData)data).DefaultTextID;

            if (textID != null && textID != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(textID);

                if (texts != null)
                {
                    foreach (string key in texts.Keys)
                    {
                        SetText(Data.DefaultTextID, texts[key], key);
                    }
                }
            }
        }
    }
}
