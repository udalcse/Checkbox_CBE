using System;
using System.Linq;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item that appends the current survey score to the predefined message.
    /// </summary>
    [Serializable]
    public class CurrentScore : Message
    {
        /// <summary>
        /// 
        /// </summary>
        public int? PageId { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            var data = (CurrentScoreItemData)configuration;
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
                {
                    var page = Response.GetPage(PageId.Value);
                    if (page != null)
                        score = page.GetItems().OfType<IScored>().Sum(item => (item).GetScore());
                }
                else
                    score = Response.GetResponsePages().SelectMany(p => p.GetItems()).OfType<IScored>().Sum(item => (item).GetScore());
            }

            return score;
        }

        /// <summary>
        /// Get the text of the message.
        /// </summary>
        /// <returns>Base message text with score appended.</returns>
        public override string Text
        {
            get
            {
                //Base text may include HTML.  If so, insert text before last < character.
                var baseText = base.Text;
                var scoreString = GetScore().ToString();

                var insertIndex = baseText.LastIndexOf("<");

                if (insertIndex > 0)
                {
                    return baseText.Insert(insertIndex, scoreString);
                }

                return baseText + scoreString;
            }
        }
    }
}
