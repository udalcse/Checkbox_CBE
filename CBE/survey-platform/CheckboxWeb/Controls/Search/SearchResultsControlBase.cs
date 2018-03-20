using System.Web.UI;

namespace CheckboxWeb.Controls.Search
{
    /// <summary>
    /// Base class for control designed to show search results.
    /// </summary>
    public abstract class SearchResultsControlBase : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get the name of the method the client can call to instantiate the search
        /// </summary>
        public virtual string ClientSearchMethod { get { return ID + "_startSearch"; } }

        /// <summary>
        /// Get the name of the method the client can call to clear search results.
        /// </summary>
        public virtual string ClientClearResultsMethod { get { return ID + "_clearResults"; } }
        
        /// <summary>
        /// Client (javascript) method to call when a particular result is displayed
        /// </summary>
        public string ClientResultSelectedHandler { get; set; }

        /// <summary>
        /// Client Id of container for search item dashboard
        /// </summary>
        public string DashContainer { get; set; }

        /// <summary>
        /// Container for status messages
        /// </summary>
        public string StatusContainer { get; set; }
    }
}