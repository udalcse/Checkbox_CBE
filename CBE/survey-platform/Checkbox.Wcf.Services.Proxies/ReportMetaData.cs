using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for a list of survey reports
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportMetaData
    {
        /// <summary>
        /// Get the database id of the report
        /// </summary>
        [DataMember]
        public int ReportId { get; set; }

        /// <summary>
        /// Get a guid that uniquely identifies the report
        /// </summary>
        [DataMember]
        public Guid ReportGuid { get; set; }

        /// <summary>
        /// Get the non-localized name of the report.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get the id of the response template the report is associated with.
        /// </summary>
        [DataMember]
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Get the name of the response template the report is associated with.
        /// </summary>
        [DataMember]
        public string ResponseTemplateName { get; set; }

        /// <summary>
        /// Get minimum completion date for responses included in this report.
        /// </summary>
        [DataMember]
        public DateTime? MinCompletionDateFilter { get; set; }

        /// <summary>
        /// Get maximum completion date for responses included in this report.
        /// </summary>
        [DataMember]
        public DateTime? MaxCompletionDateFilter { get; set; }

        /// <summary>
        /// Date report was created
        /// </summary>
        [DataMember]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Date time last modified
        /// </summary>
        [DataMember]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Get name of report creator
        /// </summary>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Page ids
        /// </summary>
        [DataMember]
        public int[] PageIds { get; set; }

        /// <summary>
        /// Get default language for survey
        /// </summary>
        [DataMember]
        public string DefaultSurveyLanguage { get; set; }

        /// <summary>
        /// Get/set style template for report
        /// </summary>
        [DataMember]
        public int? StyleTemplateId { get; set; }

        /// <summary>
        /// Display survey title or not
        /// </summary>
        [DataMember]
        public bool DisplaySurveyTitle { get; set; }

        /// <summary>
        /// Display pdf export button or not
        /// </summary>
        [DataMember]
        public bool DisplayPdfExportButton { get; set; }

        /// <summary>
        /// Includes incomplete responses or not
        /// </summary>
        [DataMember]
        public bool IncludeIncompleteResponses { get; set; }

        /// <summary>
        /// Includes test responses or not
        /// </summary>
        [DataMember]
        public bool IncludeTestResponses { get; set; }

        /// <summary>
        /// Get/set whether the current user can edit the report
        /// </summary>
        [DataMember]
        public bool Editable { get; set; }

        /// <summary>
        /// Get/set whether the current user can delete the report
        /// </summary>
        [DataMember]
        public bool Deletable { get; set; }

        /// <summary>
        /// Start date for responses filtration
        /// </summary>
        [DataMember]
        public DateTime? FilterStartDate { get; set; }

        /// <summary>
        /// End date for responses filtration
        /// </summary>
        [DataMember]
        public DateTime? FilterEndDate { get; set; }
    }
}
