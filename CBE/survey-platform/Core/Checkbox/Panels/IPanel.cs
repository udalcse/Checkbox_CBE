using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Checkbox.Panels
{
    /// <summary>
    /// Interface definition for invitation panels.
    /// </summary>
    public interface IPanel
    {
        /// <summary>
        /// Get string representation of panel type
        /// </summary>
        string PanelTypeName { get; }

        /// <summary>
        /// Gets the <see cref="Panelist"/> collection for the IPanel implementation
        /// </summary>
        List<Panelist> Panelists { get; }

        /// <summary>
        /// Gets a single <see cref="Panelist"/> given a string key
        /// </summary>
        /// <param name="identifier">the unique string id of the Panelist</param>
        /// <returns>a Panelist</returns>
        Panelist GetPanelist(string identifier);
    }
}
