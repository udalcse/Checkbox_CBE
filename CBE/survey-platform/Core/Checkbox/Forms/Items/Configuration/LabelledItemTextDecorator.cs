using System;
using System.Collections.Generic;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Base text decorator for localizing item text and description.
    /// </summary>
    [Serializable]
    public class LabelledItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _text;
        private string _subText;

        protected bool TextModified;
        protected bool SubTextModified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="language"></param>
        public LabelledItemTextDecorator(LabelledItemData itemData, string language)
            : base(itemData, language)
        {
            _text = string.Empty;
            _subText = string.Empty;

            TextModified = false;
            SubTextModified = false;
        }

        /// <summary>
        /// Utility getter
        /// </summary>
        new public LabelledItemData Data
        {
            get { return (LabelledItemData)base.Data; }
            protected set { base.Data = value; }
        }

        /// <summary>
        /// Set the item localized texts
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            if (Data.TextID != string.Empty)
            {
                SetText(Data.TextID, Text);
            }

            if (Data.SubTextID != string.Empty)
            {
                SetText(Data.SubTextID, SubText);
            }
        }

        /// <summary>
        /// Copy the localized text for the item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(LabelledItemData));

            if (Data.TextID != string.Empty)
            {
                Dictionary<string, string> textIDTexts = GetAllTexts(Data.TextID);

                if (textIDTexts != null)
                {
                    foreach (string key in textIDTexts.Keys)
                    {
                        SetText(((LabelledItemData)data).TextID, textIDTexts[key], key);
                    }
                }
            }

            if (Data.SubTextID != string.Empty)
            {
                Dictionary<string, string> subTextIDTexts = GetAllTexts(Data.SubTextID);
                if (subTextIDTexts != null)
                {
                    foreach (string key in subTextIDTexts.Keys)
                    {
                        SetText(((LabelledItemData)data).SubTextID, subTextIDTexts[key], key);
                    }
                }
            }
        }

        /// <summary>
        /// Ensure texts loaded, optionally can be overridden by children
        /// for batch text loading.  It is up to child to prevent multiple
        /// text loadings.
        /// </summary>
        protected virtual void EnsureTextsLoaded()
        {
        }


        /// <summary>
        /// Get/set the text
        /// </summary>
        public string Text
        {
            get
            {
                if (Data.TextID != string.Empty && !TextModified)
                {
                    return GetText(Data.TextID);
                }
                
                return _text;
            }

            set
            {
                _text = value;
                TextModified = true;
            }
        }

        /// <summary>
        /// Get/set the sub text
        /// </summary>
        public string SubText
        {
            get
            {
                if (Data.SubTextID != string.Empty && !SubTextModified)
                {
                    return GetText(Data.SubTextID);
                }
                
                return _subText;
            }

            set
            {
                _subText = value;
                SubTextModified = true;
            }
        }
    }
}
