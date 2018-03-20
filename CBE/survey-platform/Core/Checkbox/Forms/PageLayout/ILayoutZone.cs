

namespace Checkbox.Forms.PageLayout
{
    /// <summary>
    /// Simple marker interface for layout zones
    /// </summary>
    public interface ILayoutZone
    {
        /// <summary>
        /// Get/set whether to operate in design mode
        /// </summary>
        bool LayoutDesignMode { get;set;}

        /// <summary>
        /// Get the name of the layout zone
        /// </summary>
        string ZoneName { get;}
    }
}
