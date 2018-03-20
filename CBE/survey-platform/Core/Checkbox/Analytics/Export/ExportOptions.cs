using System;
using System.Text;

namespace Checkbox.Analytics.Export
{
    /// <summary>
    /// Simple container for passing export options.
    /// </summary>
    [Serializable]
    public class ExportOptions
    {
        public bool UseAliases { get; set; }
        public bool MergeSelectMany { get; set; }
        public bool IncludeOpenEnded { get; set; }
        public bool IncludeHidden { get; set; }
        public bool IncludeIncomplete { get; set; }
        public bool IncludeDetailedResponseInfo { get; set; }
        public bool IncludeDetailedUserInfo { get; set; }
        public bool IncludeScore { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? FileSet { get; set; }
        public Encoding OutputEncoding { get; set; }
        public bool IncludeResponseId { get; set; }
        public bool StripHtmlTags { get; set; }
        public string ExportMode { get; set; }
        public bool ExportRankOrderPoints { get; set; }
        public bool IncludeTestResponses { get; set; }
        public bool IncludeDetailedScoreInfo { get; set; }
        public bool IncludePossibleScore { get; set; }
    }
}
