using System;

namespace Checkbox.Forms
{
    /// <summary>
    /// Flags indicating what should be displayed in a response view
    /// </summary>
    [Flags]
    public enum ResponseViewDisplayFlags
    {
        /// <summary>
        /// Nothing is displayed.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Include survey title.
        /// </summary>
        Title = 0x0004,

        /// <summary>
        /// Show page numbers.
        /// </summary>
        PageNumbers = 0x0008,

        /// <summary>
        /// Show item numbers.
        /// </summary>
        ItemNumbers = 0x0010,

        /// <summary>
        /// Show progress bar.
        /// </summary>
        ProgressBar = 0x0020,

        /// <summary>
        /// Show back button.
        /// </summary>
        BackButton = 0x0040,

        /// <summary>
        /// Show save and exit button.
        /// </summary>
        SaveButton = 0x0080,

        /// <summary>
        /// Show next button.
        /// </summary>
        NextButton = 0x0100,

        /// <summary>
        /// Show finish button.
        /// </summary>
        FinishButton = 0x0200,

        /// <summary>
        /// Show create new response (when listing already completed responses)
        /// </summary>
        CreateNewButton = 0x0400,

        /// <summary>
        /// Show Checkbox footer
        /// </summary>
        CheckboxFooter = 0x0800,

        /// <summary>
        /// Show form reset button
        /// </summary>
        FormResetButton = 0x1000,

        /// <summary>
        /// Show asterisks for required Items
        /// </summary>
        Asterisks = 0x2000
    }
}
