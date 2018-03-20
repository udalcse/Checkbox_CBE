namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Interface definition for formatting items to string formats
    /// </summary>
    public interface IItemFormatter
    {
        /// <summary>
        /// Format the specified item to the desired format.
        /// </summary>
        /// <param name="item">Item to format.</param>
        /// <param name="format">Desired format.</param>
        /// <param name="showScores">Control whether to show scores</param>
        /// <returns>Item formatted as a string.</returns>
        string Format(Item item, string format, bool showScores);
    }
}
