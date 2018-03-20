using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item to display a score depending on a the total score for the survey.
    /// </summary>
    [Serializable]
    public class ScoreMessageItem : Message
    {
        private ScoreMessageDictionary _scoreMessages;
        private List<ScoreMessage> _orderedMessages;

        /// <summary>
        /// 
        /// </summary>
        public int? PageId { set; get; }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            var data = (ScoreMessageItemData)configuration;

            _scoreMessages = new ScoreMessageDictionary();
            _orderedMessages = data.Messages;

            foreach (ScoreMessage message in _orderedMessages)
            {
                _scoreMessages[message.MessageId] = message;
            }

            PageId = data.PageId;
        }

        /// <summary>
        /// Get the current score
        /// </summary>
        /// <returns></returns>
        protected double GetScore()
        {
            double score = 0;

            if (Response != null)
            {
                if (PageId.HasValue)
                    score = Response.GetPage(PageId.Value).GetItems().OfType<IScored>().Sum(item => (item).GetScore());
                else
                    score = Response.GetResponsePages().SelectMany(p => p.GetItems()).OfType<IScored>().Sum(item => (item).GetScore());
            }

            return score;
        }

        /// <summary>
        /// Get the item text
        /// </summary>
        public override string Text
        {
            get
            {
                //If taking a survey, get the message, otherwise display the data
                if (Response != null)
                {
                    ScoreMessage scoreMessage = _scoreMessages.GetMessageForScoreValue(GetScore());

                    if (scoreMessage != null)
                    {
                        return GetPipedText(
                            "ScoreMessage_" + scoreMessage.MessageId,
                            GetText("/scoreMessageData/" + ID + "/" + scoreMessage.MessageId));

                    }
                    
                    return string.Empty;
                }

                //Return text representation of all score messages
                return ScoreMessagesToString();
            }
        }

        /// <summary>
        /// Return a string representation of all score messages
        /// </summary>
        /// <returns></returns>
        private string ScoreMessagesToString()
        {
            var sb = new StringBuilder();

            foreach (ScoreMessage scoreMessage in _orderedMessages)
            {
                string scoreText = string.Empty;

                if (scoreMessage.LowScore.HasValue
                    && scoreMessage.HighScore.HasValue)
                {
                    scoreText = scoreMessage.LowScore + " - " + scoreMessage.HighScore;
                }

                if (!scoreMessage.LowScore.HasValue
                    && scoreMessage.HighScore.HasValue)
                {
                    scoreText = "<= " + scoreMessage.HighScore;
                }

                if (scoreMessage.LowScore.HasValue
                    && !scoreMessage.HighScore.HasValue)
                {
                    scoreText = ">= " + scoreMessage.LowScore;
                }

                if (Utilities.IsNotNullOrEmpty(scoreText))
                {
                    sb.Append(scoreText);
                    sb.Append(" -- ");
                    sb.Append(Utilities.AdvancedHtmlEncode(GetText("/scoreMessageData/" + ID + "/" + scoreMessage.MessageId)));
                    sb.Append("<br />");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get meta data for serialization to renderers
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            var values = base.GetMetaDataValuesForSerialization();

            if (Response == null)
            {
                values["ScoreMessages"] = Text;
            }

            return values;
        }
    }
}
