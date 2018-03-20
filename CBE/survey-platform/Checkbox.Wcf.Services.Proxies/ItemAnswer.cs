using System;

namespace Checkbox.Analytics.Data
{

    /// <summary>
    /// Lightweight container to store answer information for a response.
    /// </summary>
    /// <remarks><see cref="Checkbox.Forms.Items.Configuration.ItemData"/> and <see cref="Checkbox.Forms.Items.Configuration.ListData" /> objects are fairly
    /// heavyweight and often contain internal <see cref="System.Data.DataSet"/> to store their data.  Since report data may be cached, and therefore serialized in
    /// multi-machine/process environments, this serves as a more efficient container class to use for reporting.</remarks>
    [Serializable]
    public class ItemAnswer : IEquatable<ItemAnswer>
    {
        public ItemAnswer()
        {
            int i = 0;
        }
        /// <summary>
        /// Get/set id of the response
        /// </summary>
        public long ResponseId { get; set; }

        /// <summary>
        /// Get/set response guid
        /// </summary>
        public Guid? ResponseGuid { get; set; }

        /// <summary>
        /// Get/set answer id
        /// </summary>
        public long AnswerId { get; set; }

        /// <summary>
        /// Get/set item id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Get/set option id
        /// </summary>
        public int? OptionId { get; set; }

        /// <summary>
        /// Get/set count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Get/set answer text
        /// </summary>
        public string AnswerText { get; set; }

        /// <summary>
        /// Get/set whether the answer is an "other" answer
        /// </summary>
        public bool IsOther { get; set; }

        /// <summary>
        /// Get/set answer points
        /// </summary>
        public double? Points { get; set; }

        /// <summary>
        /// Get/set answer points
        /// </summary>
        public int? PageId { get; set; }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ItemAnswer other)
        {
            return other.AnswerId == AnswerId;
        }
    }
}
