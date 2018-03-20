using System;
using Checkbox.Management;

namespace Checkbox.Forms
{
    /// <summary>
    /// Container class for survey related settings
    /// </summary>
    [Serializable]
    public class SurveyBehaviorSettings
    {
        /// <summary>
        /// Gets or sets the SecurityType mask for this <see cref="ResponseTemplate"/>
        /// </summary>
        public SecurityType SecurityType { get; set; }

        /// <summary>
        /// Gets or sets the Report security mask for this <see cref="ResponseTemplate"/>
        /// </summary>
        public ReportSecurityType ReportSecurityType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> before which this <see cref="ResponseTemplate"/> will be inactive
        /// </summary>
        public DateTime? ActivationStartDate { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> after which this <see cref="ResponseTemplate"/> will deactivate
        /// </summary>
        public DateTime? ActivationEndDate { get; set; }

        /// <summary>
        /// Gets or sets a limit on the total number or Responses allowed for this <see cref="ResponseTemplate"/>
        /// If no limit, returns null.
        /// </summary>
        public int? MaxTotalResponses { get; set; }

        /// <summary>
        /// Gets or sets a limit to the number of times a single Respondent can respond to this <see cref="ResponseTemplate"/>
        /// If no limit, returns null.
        /// </summary>
        public int? MaxResponsesPerUser { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether a Respondent to this <see cref="ResponseTemplate"/> can leave and return to a 
        /// partially completed Response
        /// </summary>
        public bool AllowContinue { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether a respondent will be able to save and quit in the middle
        /// of the survey.
        /// </summary>
        public bool ShowSaveAndQuit { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether a Respondent to this <see cref="ResponseTemplate"/> can edit his or her Response
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Gets/sets a flag that indicates whether this <see cref="ResponseTemplate"/> can be edited while it's active
        /// </summary>
        public bool AllowSurveyEditWhileActive { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [display PDF download button].
        /// </summary>
        /// <value>
        /// <c>true</c> if [display PDF download button]; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayPDFDownloadButton { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether a Respondent can navigate back
        /// </summary>
        public bool DisableBackButton { get; set; }

        /// <summary>
        /// Gets or sets a global password for the Response
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this <see cref="ResponseTemplate"/> is active and may collect Responses
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the action to take on completion of a Response
        /// </summary>
        public int CompletionType { get; set; }
        
        /// <summary>
        /// Gets or sets whether to randomize Items within a Page
        /// </summary>
        public bool RandomizeItemsInPages { get; set; }

        /// <summary>
        /// Gets or sets whether to enable scoring for this <see cref="ResponseTemplate"/>
        /// </summary>
        public bool EnableScoring { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether a Respondent to this <see cref="ResponseTemplate"/> can reset form on the page to default values
        /// </summary>
        public bool AllowFormReset { get; set; }

        /// <summary>
        /// Get/set whether send resume email is enabled.
        /// </summary>
        public bool EnableSendResumeEmail { get; set; }

        /// <summary>
        /// Get/set "FROM" address for resume email from address
        /// </summary>
        public string ResumeEmailFromAddress { get; set; }

        /// <summary>
        /// Get/set whether responses will be anonymized, regardless of security settings.
        /// </summary>
        public bool AnonymizeResponses { get; set; }

        /// <summary>
        /// Get/set google analytics tracking ID
        /// </summary>
        public string GoogleAnalyticsTrackingID { get; set; }

        /// <summary>
        /// Return a boolean indicating if the specified date is in the activation date range.
        /// No check of "IsActive" flag is made.
        /// </summary>
        /// <param name="refDate"></param>
        /// <returns></returns>
        public bool GetIsDateInActivationDateRange(DateTime refDate)
        {
            //Check activation dates
            if (ActivationStartDate.HasValue && (ActivationStartDate > refDate))
            {
                return false;
            }

            return !ActivationEndDate.HasValue || (ActivationEndDate >= refDate);
        }

        /// <summary>
        /// Return a boolean indicating if a template is active on the specified date.
        /// </summary>
        /// <param name="refDate"></param>
        /// <param name="inactiveReason"></param>
        /// <returns></returns>
        public bool GetIsActiveOnDate(DateTime refDate, out string inactiveReason)
        {
            return SurveyIsActiveOnDate(refDate, IsActive, ActivationStartDate, ActivationEndDate, out inactiveReason);
        }

        /// <summary>
        /// Return a boolean indicating if survey is active on specified date
        /// </summary>
        /// <param name="refDate"></param>
        /// <param name="isActive"></param>
        /// <param name="activationStartDate"></param>
        /// <param name="activationEndDate"></param>
        /// <param name="inactiveReason"></param>
        /// <returns></returns>
        public static bool SurveyIsActiveOnDate(DateTime refDate, bool isActive, DateTime? activationStartDate, DateTime? activationEndDate, out string inactiveReason)
        {
            //Check if survey is active
            if (!(isActive))
            {
                inactiveReason = "surveynotactive";
                return false;
            }

            //Check activation dates
            if (activationStartDate.HasValue && (activationStartDate > refDate))
            {
                inactiveReason = "beforestartdate";
                return false;
            }

            if (activationEndDate.HasValue && (activationEndDate < refDate))
            {
                inactiveReason = "afterenddate";
                return false;
            }

            inactiveReason = string.Empty;
            return true;
        }

        /// <summary>
        /// Get display flags baased on survey behavior settings.  Currently only options are 
        /// None or SaveButton
        /// </summary>
        /// <returns></returns>
        public ResponseViewDisplayFlags GetDisplayFlags()
        {
            ResponseViewDisplayFlags displayFlags = ResponseViewDisplayFlags.None;

            if (ShowSaveAndQuit)
            {
                displayFlags |= ResponseViewDisplayFlags.SaveButton;
            }

            if(!DisableBackButton)
            {
                displayFlags |= ResponseViewDisplayFlags.BackButton;
            }

            return displayFlags;
        }
    }
}
