using System;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for email items
    /// </summary>
    [Serializable()]
    public class EmailItemTextDecorator : LocalizableResponseItemTextDecorator
    {
        private string _subject;
        private string _body;

        private bool _subjectModified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public EmailItemTextDecorator(EmailItemData data, string language)
            : base(data, language)
        {
        }

        /// <summary>
        /// Get the item data
        /// </summary>
        new public EmailItemData Data
        {
            get { return (EmailItemData)base.Data; }
        }


        /// <summary>
        /// Set the item localized texts
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            if (Data.BodyTextID != string.Empty)
            {
                SetText(Data.BodyTextID, Body);
            }

            if (Data.SubjectTextID != string.Empty)
            {
                SetText(Data.SubjectTextID, Subject);
            }
        }

        /// <summary>
        /// Get/set the body
        /// </summary>
        public string Body
        {
            get
            {
                if (Data.BodyTextID != string.Empty && !_subjectModified)
                {
                    return GetText(Data.BodyTextID);
                }
                else
                {
                    return _body;
                }
            }

            set
            {
                _body = value;
            }
        }

        /// <summary>
        /// Get/set the subject
        /// </summary>
        public string Subject
        {
            get
            {
                if (Data.SubjectTextID != string.Empty && !_subjectModified)
                {
                    return GetText(Data.SubjectTextID);
                }
                else
                {
                    return _subject;
                }
            }

            set
            {
                _subject = value;
                _subjectModified = true;
            }
        }

        /// <summary>
        /// Copy the localized text for an item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(EmailItemData));

            //Handle the subject
            string subjectTextID = ((EmailItemData)data).SubjectTextID;

            if (subjectTextID != null && subjectTextID.Trim() != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(subjectTextID);

                foreach (string key in texts.Keys)
                {
                    SetText(Data.SubjectTextID, texts[key], key);
                }
            }

            //Handle the body
            string bodyTextID = ((EmailItemData)data).BodyTextID;

            if (bodyTextID != null && bodyTextID.Trim() != string.Empty)
            {
                Dictionary<string, string> texts = GetAllTexts(bodyTextID);

                foreach (string key in texts.Keys)
                {
                    SetText(Data.BodyTextID, texts[key], key);
                }
            }
        }
    }
}
