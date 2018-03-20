using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Marker interface that denotes an Item collects data and is accountable for that data
    /// </summary>
    public interface IAnswerable
    {
        /// <summary>
        /// Event indicating an item's answer has changed
        /// </summary>
        event EventHandler AnswerChanged;

        /// <summary>
        /// Get a human-readable answer for an item that implements this interface
        /// </summary>
        /// <returns></returns>
        string GetAnswer();

        /// <summary>
        /// Get a raw answer for an item that implements this interface
        /// </summary>
        /// <returns></returns>
        string GetRawAnswer();

        /// <summary>
        /// Set the answer to the item
        /// </summary>
        /// <param name="answer">Answer value</param>
        void SetAnswer(object answer);

        /// <summary>
        /// Get a boolean indicating if the item has an answer
        /// </summary>
        bool HasAnswer { get; }

        /// <summary>
        /// Delete the answer(s) for the item.
        /// </summary>
        void DeleteAnswers();

        /// <summary>
        /// Updates item answer if SPC was triggered or the response was resumed 
        /// </summary>
        void UpdateMergedText();
    }
}
