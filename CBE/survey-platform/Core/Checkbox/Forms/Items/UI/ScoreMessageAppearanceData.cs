using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for score message items.
    /// </summary>
    [Serializable]
    public class ScoreMessageAppearanceData : Message
    {
        /// <summary>
        /// Get the score message appearance code
        /// </summary>
        public override string AppearanceCode { get { return "SCORE_MESSAGE"; } }
    }
}
