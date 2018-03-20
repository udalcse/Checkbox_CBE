
using System.Web.UI;
using Checkbox.Forms.PageLayout;

namespace Checkbox.Web.Forms.UI.Templates
{
    /// <summary>
    /// Interface for web layout zone that supports adding/removing controls.
    /// </summary>
    public interface IWebLayoutZone: ILayoutZone
    {
        /// <summary>
        /// Add a control to the zone.
        /// </summary>
        /// <param name="control"></param>
        void AddControl(Control control);

        /// <summary>
        /// Remove a control from the zone.
        /// </summary>
        /// <param name="control"></param>
        void RemoveControl(Control control);

        /// <summary>
        /// Remove all controls from the zone
        /// </summary>
        void Clear();
    }
}
