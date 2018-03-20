namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Interface for items that support a score
    /// </summary>
    public interface IScored
    {
        /// <summary>
        /// Get the "score" for the item
        /// </summary>
        /// <returns></returns>
        double GetScore();

        /// <summary>
        /// Get max possible for the item
        /// </summary>
        /// <returns></returns>
        double GetPossibleMaxScore();
    }
}
