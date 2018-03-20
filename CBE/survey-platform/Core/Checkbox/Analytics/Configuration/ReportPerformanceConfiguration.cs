using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Checkbox.Analytics.Configuration
{
    ///<summary>
    /// Reports settings for best performance
    ///</summary>
    public class ReportPerformanceConfiguration : ConfigurationBase, IXmlConfigurationBase
    {
        private const int DEFAULT_MAX_RESPONSE_COUNT_FOR_USER_INPUT_VISIBILITY = 100000;
        private const int DEFAULT_MAX_RESPONSE_DETAILS_ITEM_ROWS_FOR_PDF_EXPORT = 250;

        private int _maxResponseCountForUserInputItemsVisibility;
        private int _maxResponseDetailsItemRowsForPdfExport;

        ///<summary>
        /// Max response count for user input items visibility
        ///</summary>
        public int MaxResponseCountForUserInputItemsVisibility
        {
            get { return _maxResponseCountForUserInputItemsVisibility; }
        }

        ///<summary>
        /// Max response rows count for details table
        ///</summary>
        public int MaxResponseDetailsItemRowsForPdfExport
        {
            get { return _maxResponseDetailsItemRowsForPdfExport; }
        }

        ///<summary>
        /// Constructor
        ///</summary>
        public ReportPerformanceConfiguration()
            : this(string.Empty)
		{
		}

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="name"></param>
        public ReportPerformanceConfiguration(string name)
            : base(name)
		{
		}

        public void LoadFromXml(XmlNode node)
        {
            if (!int.TryParse(XmlUtility.GetNodeText(
                    node.SelectSingleNode("/reportPerformanceConfiguration/maxResponseCountForUserInputItemsVisibility"),
                    true), out _maxResponseCountForUserInputItemsVisibility))
            {
                _maxResponseCountForUserInputItemsVisibility = DEFAULT_MAX_RESPONSE_COUNT_FOR_USER_INPUT_VISIBILITY;
            }
            if (!int.TryParse(XmlUtility.GetNodeText(
                    node.SelectSingleNode("/reportPerformanceConfiguration/maxResponseDetailsItemRowsForPdfExport"),
                    true), out _maxResponseDetailsItemRowsForPdfExport))
            {
                _maxResponseDetailsItemRowsForPdfExport = DEFAULT_MAX_RESPONSE_DETAILS_ITEM_ROWS_FOR_PDF_EXPORT;
            }
        }
    }
}
