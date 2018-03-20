using System;
using System.Linq;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ScoredItem : SelectItem, IScored
    {
        /// <summary>
        /// Get score for this item
        /// </summary>
        /// <returns></returns>
        public virtual double GetScore()
        {
            return Options.Where(option => option.IsSelected).Sum(option => option.Points);
        }

        /// <summary>
        /// Get possible score for this item
        /// </summary>
        /// <returns></returns>
        public virtual double GetPossibleMaxScore()
        {
            return Options.Sum(option => option.Points);
        }
    }
}
