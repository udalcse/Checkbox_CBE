using System;
using System.Collections.Generic;
using Checkbox.Common;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for localizing messages in a score message item.
    /// </summary>
    [Serializable]
    public class ScoreMessageTextDecorator : LocalizableResponseItemTextDecorator
    {
        private readonly Dictionary<string, string> _scoreTexts;
        private readonly Dictionary<string, bool> _scoreTextsModified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public ScoreMessageTextDecorator(ItemData data, string language)
            : base(data, language)
        {
            _scoreTexts = new Dictionary<string, string>();
            _scoreTextsModified = new Dictionary<string, bool>();

            if (data is ScoreMessageItemData)
            {
                (data as ScoreMessageItemData).MessagesUpdated += ScoreMessageTextDecorator_MessagesUpdated;
                (data as ScoreMessageItemData).MessagesDeleted += ScoreMessageTextDecorator_MessagesDeleted;
            }
        }

        void ScoreMessageTextDecorator_MessagesDeleted(object sender, MessagesDeletedEventArgs e)
        {
            string key = "scoreMessageText_" + e.Id;

            _scoreTexts.Remove(key);
            _scoreTextsModified.Remove(key);
        }

        void ScoreMessageTextDecorator_MessagesUpdated(object sender, MessagesUpdatedEventArgs e)
        {
            if (e.NewId != e.OldId)
            {
                string text = GetMessageText(e.OldId);
                string key = "scoreMessageText_" + e.OldId;

                _scoreTexts.Remove(key);
                _scoreTextsModified.Remove(key);

                SetMessageText(e.NewId, text);
            }
        }

        /// <summary>
        /// Get the object data
        /// </summary>
        new public ScoreMessageItemData Data
        {
            get { return (ScoreMessageItemData)base.Data; }
        }

        /// <summary>
        /// Get the texts asscociated with this item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            base.SetLocalizedTexts();

            foreach (string key in _scoreTextsModified.Keys)
            {
                if (_scoreTextsModified[key] && _scoreTexts.ContainsKey(key))
                {
                    int? id = null;

                    if (key.ToLower() != "default")
                    {
                        if (key.Contains("_"))
                        {
                            id = Convert.ToInt32(key.Split('_')[1]);
                        }
                    }

                    string textID = Data.GetMessageTextId(id.Value);

                    if (Utilities.IsNotNullOrEmpty(textID))
                    {
                        SetText(textID, _scoreTexts[key]);
                    }
                }
            }
        }

        /// <summary>
        /// Get the message text
        /// </summary>
        /// <param name="scoreMessageID"></param>
        /// <param name="text"></param>
        public void SetMessageText(Int32 scoreMessageID, string text)
        {
            string key = "scoreMessageText_" + scoreMessageID;

            _scoreTexts[key] = text;
            _scoreTextsModified[key] = true;
        }

        /// <summary>
        /// Get the text of a score message
        /// </summary>
        /// <param name="scoreMessageID"></param>
        /// <returns></returns>
        public string GetMessageText(Int32 scoreMessageID)
        {
            string key = "scoreMessageText_" + scoreMessageID;

            if (_scoreTextsModified.ContainsKey(key) && _scoreTextsModified[key])
            {
                if (_scoreTexts.ContainsKey(key))
                {
                    return _scoreTexts[key];
                }

                return string.Empty;
            }

            string textID = Data.GetMessageTextId(scoreMessageID);

            if (textID != string.Empty)
            {
                return GetText(textID);
            }

            return string.Empty;

        }

        /// <summary>
        /// Copy texts for this item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(ScoreMessageItemData));

            var sourceData = (ScoreMessageItemData)data;

            List<ScoreMessage> scoreMessages = sourceData.Messages;

            foreach (ScoreMessage scoreMessage in scoreMessages)
            {
                string messageTextId = sourceData.GetMessageTextId(scoreMessage.MessageId);

                if (Utilities.IsNotNullOrEmpty(messageTextId))
                {
                    Dictionary<string, string> texts = GetAllTexts(messageTextId);

                    ScoreMessage matchingMessage = Data.Messages.Find(msg => msg.LowScore == scoreMessage.LowScore && msg.HighScore == scoreMessage.HighScore);

                    if (matchingMessage != null)
                    {
                        string matchingTextID = Data.GetMessageTextId(matchingMessage.MessageId);

                        foreach (string key in texts.Keys)
                        {
                            SetText(matchingTextID, texts[key], key);
                        }
                    }
                }
            }            
        }
    }
}
