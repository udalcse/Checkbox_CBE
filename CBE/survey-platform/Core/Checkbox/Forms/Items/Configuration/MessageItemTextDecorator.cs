using System;
using System.Collections.Generic;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for localizing message item text.
    /// </summary>
    [Serializable]
    public class MessageItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _message;
        private bool _messageModified;
        private bool _reportableSection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public MessageItemTextDecorator(MessageItemData data, string language)
            : base(data, language)
        {
            _message = string.Empty;
            _messageModified = false;
        }

        /// <summary>
        /// Get the item's data
        /// </summary>
        new public MessageItemData Data
        {
            get
            {
                return (MessageItemData)base.Data;
            }
        }

        /// <summary>
        /// Set message localized texts
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            base.SetLocalizedTexts();

            if (Data.TextID != string.Empty)
            {
                SetText(Data.TextID, Message);
            }
        }

        /// <summary>
        /// Copy localized text for the item.
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(MessageItemData));

            Dictionary<string, string> texts = GetAllTexts(((MessageItemData)data).TextID);

            if(texts != null)
            {
                foreach(string key in texts.Keys)
                {
                    SetText(Data.TextID, texts[key], key);
                }
            }
        }

        /// <summary>
        /// Get set the message
        /// </summary>
        public string Message
        {
            get
            {
                if (Data.TextID != string.Empty && !_messageModified)
                {
                    return GetText(Data.TextID);
                }
                
                return _message;
            }

            set
            {
                _message = value;
                _messageModified = true;
            }
        }

        /// <summary>
        /// Get set the message
        /// </summary>
        public bool ReportableSectionBreak
        {
            get { return Data.ReportableSectionBreak; }

            set { Data.ReportableSectionBreak = value; }
        }

    }
}
