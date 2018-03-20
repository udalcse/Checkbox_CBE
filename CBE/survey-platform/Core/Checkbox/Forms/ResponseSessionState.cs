using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Enumeration of states for view control
    /// </summary>
    public enum ResponseSessionState
    {
        /// <summary>
        /// No display
        /// </summary>
        None = 0,

        /// <summary>
        /// Display survey UI
        /// </summary>
        TakeSurvey = 1,

        /// <summary>
        /// Display language selection dialog
        /// </summary>
        SelectLanguage = 2,

        /// <summary>
        /// Display survey password dialog
        /// </summary>
        EnterPassword = 3,

        /// <summary>
        /// Display progress saved dialog
        /// </summary>
        SavedProgress = 4,

        /// <summary>
        /// Display response list
        /// </summary>
        EditResponse = 5,

        /// <summary>
        /// Go to login page
        /// </summary>
        LoginRequired = 6,

        /// <summary>
        /// Display an error
        /// </summary>
        Error = 7
    }
}
