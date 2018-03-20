using System.Web.UI;
using Checkbox.Forms.PageLayout;

namespace Checkbox.Web.Forms.UI.Templates
{
    /// <summary>
    /// Interface definition for web control based page layout template zones.
    /// </summary>
    public interface IWebLayoutTemplate : IPageLayoutTemplate
    {
        /// <summary>
        /// Add a control to the specified zone
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="control"></param>
        void AddControlToZone(string zoneName, Control control);

        /// <summary>
        /// Remove a control from the specified zone
        /// </summary>
        /// <param name="zoneName"></param>
        /// <param name="control"></param>
        void RemoveControlFromZone(string zoneName, Control control);

        /// <summary>
        /// Get the header zone
        /// </summary>
        ILayoutZone HeaderZone { get; }

        /// <summary>
        /// Get the zone for headers
        /// </summary>
        ILayoutZone FooterZone { get; }

        /// <summary>
        /// Get the zone for the next button
        /// </summary>
        ILayoutZone NextButtonZone { get;}

        /// <summary>
        /// Get the zone for the previous button
        /// </summary>
        ILayoutZone PreviousButtonZone { get;}

        /// <summary>
        /// Get the zone for the save and quit button
        /// </summary>
        ILayoutZone SaveAndQuitButtonZone { get;}

        /// <summary>
        /// Get the zone for the finish button
        /// </summary>
        ILayoutZone FinishButtonZone { get;}

        /// <summary>
        /// Get the zone for the survey title
        /// </summary>
        ILayoutZone TitleZone { get;}

        /// <summary>
        /// Get the zone for the progress bar
        /// </summary>
        ILayoutZone ProgressBarTopZone { get;}

        /// <summary>
        /// Get the zone for the progress bar
        /// </summary>
        ILayoutZone ProgressBarBottomZone { get; }

        /// <summary>
        /// Get the zone for the page number
        /// </summary>
        ILayoutZone PageNumberZone { get;}
        
        /// <summary>
        /// Get the zone an item should go in
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        ILayoutZone GetItemZone(int itemID);
    }
}
