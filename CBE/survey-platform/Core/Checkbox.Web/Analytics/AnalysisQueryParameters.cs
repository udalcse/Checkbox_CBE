using System;
using Checkbox.Common;
using Checkbox.PdfExport;

namespace Checkbox.Web.Analytics
{
    /// <summary>
    /// Simple container for reading variables from query string.
    /// </summary>
    public class AnalysisQueryParameters
    {
        /// <summary>
        /// Language code for report
        /// </summary>
        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Guid associated with analysis to run
        /// </summary>
        [QueryParameter("ag")]
        public Guid? AnalysisGuid { get; set; }

        /// <summary>
        /// Id of analysis to run
        /// </summary>
        [QueryParameter("aid")]
        public int AnalysisId { get; set; }

        /// <summary>
        /// Id of analysis to run
        /// </summary>
        [QueryParameter("a")]
        public Guid? AnalysisGuid_4_7 { get; set; }

        /// <summary>
        /// Guid associated with temporary auth ticket
        /// </summary>
        [QueryParameter("tg")]
        public Guid? TicketGuid { get; set; }

        /// <summary>
        /// Print/Export mode
        /// </summary>
        [QueryParameter("print")]
        public string Print { get; set; }

        /// <summary>
        /// Print/Export orientation
        /// </summary>
        [QueryParameter("orientation")]
        public PdfExportOrientation Orientation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ExportMode ExportMode
        {
            get
            {
                if (Print == null)
                    return ExportMode.None;

                switch (Print.ToLower())
                {
                    case "pdf":
                        return ExportMode.Pdf;
                    default:
                        return ExportMode.None;
                }
            }
        }
        
        /// <summary>
        /// Progress Key
        /// </summary>
        [QueryParameter("progresskey")]
        public string ProgressKey { get; set; }

        /// <summary>
        /// Get whether we are showing "ALL_PAGES" or not.
        /// </summary>
        public bool IsAllPages
        {
            get { return "ALL_PAGES".Equals(PageNumberString, StringComparison.InvariantCultureIgnoreCase); }
        }
        
        /// <summary>
        /// Current analysis page
        /// </summary>
        [QueryParameter("p")]
        public string PageNumberString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber
        {
            get
            {
                var theValue = Utilities.AsInt(PageNumberString);

                return theValue.HasValue
                           ? theValue.Value
                           : -1;
            }
        } 

        /// <summary>
        /// Ensure supplied parameters are valid.  
        /// </summary>
        /// <returns></returns>
        public bool ValidateQueryParamters()
        {
            return AnalysisGuid.HasValue || AnalysisGuid_4_7.HasValue || AnalysisId > 0;
        }
    }
}
