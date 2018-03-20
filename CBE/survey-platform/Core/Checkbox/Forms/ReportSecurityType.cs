using System;

namespace Checkbox.Forms
{
    /// <summary>
    /// Default security type for responses associated with a survey.
    /// </summary>
    [Serializable]
    public enum ReportSecurityType
    {
        /// <summary>
        /// Deprecated -- Equivalent to SummaryPrivate
        /// </summary>
        Private = 1,

        /// <summary>
        /// Deprecated -- Equivalent to SummaryPublic
        /// </summary>
        Public,

        /// <summary>
        /// Response details not available and only owner/system administrator can create new reports.
        /// </summary>
        SummaryPrivate,

        /// <summary>
        /// Response details are available and only owner/system administrator can create new reports
        /// </summary>
        DetailsPrivate,

        /// <summary>
        /// Response details are not available and all users can create new reports
        /// </summary>
        SummaryPublic,

        /// <summary>
        /// Response details are available and all users create new reports
        /// </summary>
        DetailsPublic,

        /// <summary>
        /// Response details are not available and only acl members can create new reports
        /// </summary>
        SummaryAcl,

        /// <summary>
        /// Response details are available for download and only acl members can create new reports
        /// </summary>
        DetailsAcl,

        /// <summary>
        /// Response details are not available for download and only registered users can create new reports
        /// </summary>
        SummaryRegisteredUsers,

        /// <summary>
        /// Response details are available for download and only registered users can create new reports
        /// </summary>
        DetailsRegisteredUsers
    }
}