using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Dictionary container for score messages, keyed by message id
    /// </summary>
    [Serializable]
    public class ScoreMessageDictionary : Dictionary<int, ScoreMessage>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ScoreMessageDictionary()
        {
            
        }

        /// <summary>
        /// Constructor required for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ScoreMessageDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }

        /// <summary>
        /// Get a message based on score
        /// </summary>
        /// <param name="scoreValue"></param>
        /// <returns></returns>
        public ScoreMessage GetMessageForScoreValue(double scoreValue)
        {
            return Values.FirstOrDefault(msg =>
                ((msg.HighScore == null || msg.HighScore.Value >= scoreValue)
                && (msg.LowScore == null || msg.LowScore.Value <= scoreValue)));
        }

        /// <summary>
        /// Access message based on score range
        /// </summary>
        /// <param name="lowScore"></param>
        /// <param name="highScore"></param>
        /// <returns></returns>
        public ScoreMessage GetMessageForScoreRange(double? lowScore, double? highScore)
        {
            return Values.FirstOrDefault((msg =>
                msg.HighScore == highScore
                && msg.LowScore == lowScore));
        }

        /// <summary>
        /// Get a temporary, negative id for a new message
        /// </summary>
        /// <returns></returns>
        public int GetNextTempMessageId()
        {
            return Values.Count > 0
                ? Math.Min(Values.Min(msg => msg.MessageId) - 1, -1)
                : -1;
        }
    }
}
