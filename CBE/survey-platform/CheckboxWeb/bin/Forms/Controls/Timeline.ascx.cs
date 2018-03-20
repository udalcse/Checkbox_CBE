using Checkbox.Management;

namespace CheckboxWeb.Forms.Controls
{
    public partial class Timeline : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Shows the graph
        /// </summary>
        public bool ShowGraph
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the initial manager
        /// </summary>
        public string Manager
        {
            get;
            set;
        }

        /// <summary>
        /// Records per visible page
        /// </summary>
        public int RecordsPerPage
        {
            get
            {
                return ApplicationManager.AppSettings.TimelineRecordsPerPage;
            }
        }

        /// <summary>
        /// Request expiration. Specifies when to ask server for the new portion of data
        /// </summary>
        public int RequestExpiration
        {
            get
            {
                return ApplicationManager.AppSettings.TimelineRequestExpiration;
            }
        }

        /// <summary>
        /// Events that will be shown in the timeline
        /// </summary>
        public string VisibleEvents
        {
            get;
            set;
        }

        string[] _parsedEvents;
        /// <summary>
        /// Helps thansform the event array to the js config
        /// </summary>
        public string[] ParsedEvents
        {
            get
            {
                if (_parsedEvents == null)
                    _parsedEvents = string.IsNullOrEmpty(VisibleEvents) ? new string[]{} : VisibleEvents.Split(',');
                return _parsedEvents;
            }
        }

        /// <summary>
        /// Javascript to be run as timeline gets loaded
        /// </summary>
        public string OnClientLoad
        {
            get;
            set;
        }
    }
}