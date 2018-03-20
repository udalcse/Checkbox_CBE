
using System.Collections.ObjectModel;

namespace Checkbox.Forms.PageLayout
{
    /// <summary>
    /// Interface for page layout templates
    /// </summary>
    public interface IPageLayoutTemplate
    {
        /// <summary>
        /// Get the default layout zone
        /// </summary>
        ILayoutZone DefaultZone { get;}

        /// <summary>
        /// Get all the layout zones.
        /// </summary>
        ReadOnlyCollection<ILayoutZone> Zones { get;}

        /// <summary>
        /// Get/set whether the template is operating in design mode
        /// </summary>
        bool LayoutDesignMode { get;set;}

        /// <summary>
        /// Get a collection of zone names that are reserved for non-question purposes
        /// </summary>
        ReadOnlyCollection<string> ReservedZones { get;}

        /// <summary>
        /// Clear the item zones of any children
        /// </summary>
        void ClearZones();

        /// <summary>
        /// Get a type name for the layout template.
        /// </summary>
        string TypeName { get;}
    }
}
