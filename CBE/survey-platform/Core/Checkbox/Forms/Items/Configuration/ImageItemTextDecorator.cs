using System;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for localized image item alt texts.
    /// </summary>
    [Serializable]
    public class ImageItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _altText;
        private bool _altTextModified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="language"></param>
        public ImageItemTextDecorator(ImageItemData itemData, string language) : base(itemData, language)
        {
            _altText = string.Empty;
            _altTextModified = false;
        }

        /// <summary>
        /// Get the associated item data
        /// </summary>
        new public ImageItemData Data
        {
            get { return (ImageItemData)base.Data; }
        }

        /// <summary>
        /// Set the localized text for the item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            if (Data.AlternateTextID != string.Empty)
            {
                SetText(Data.AlternateTextID, AlternateText);
            }
        }

        /// <summary>
        /// Get/set the alternate text for the image
        /// </summary>
        public string AlternateText
        {
            get
            {
                if (Data.AlternateTextID != string.Empty && !_altTextModified)
                {
                    return GetText(Data.AlternateTextID);
                }
                
                return _altText;
            }

            set
            {
                _altText = value;
                _altTextModified = true;
            }
        }

        /// <summary>
        /// Copy localized text for the item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(ImageItemData));

            string textID = ((ImageItemData)data).AlternateTextID;

            if (textID != null && textID.Trim() != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(textID);

                foreach (string key in texts.Keys)
                {
                    SetText(Data.AlternateTextID, texts[key], key);
                }
            }
        }
    }
}
