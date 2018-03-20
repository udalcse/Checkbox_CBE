using System;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Simple container for score messages
    /// </summary>
    [Serializable]
    public class ScoreMessage
    {
        /// <summary>
        /// Get/set message id
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Get/set low end of score range
        /// </summary>
        public double? LowScore { get; set; }

        /// <summary>
        /// Get/set high end of score range
        /// </summary>
        public double? HighScore { get; set; }
    }
}
