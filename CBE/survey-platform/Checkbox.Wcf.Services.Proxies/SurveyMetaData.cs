using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for metadata associated with surveys.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyMetaData
    {
        /// <summary>
        /// The survey id.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Get/set the non-localized name of the survey
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get/set the localized title.  The title will be localized in the default language of the survey.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Survey security type.  Can be overridden by ACL settings.
        /// </summary>
        [DataMember]
        public string SecurityType { get; set; }

        /// <summary>
        /// Get/set the creator
        /// </summary>
        /// <remarks>This value is not modified in the Checkbox database when updating a survey.</remarks>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get/set the default language code
        /// </summary>
        [DataMember]
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Get/set the languages the survey supports.
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection SupportedLanguages { get; set; }

        /// <summary>
        /// Get/set the language source
        /// </summary>
        [DataMember]
        public string LanguageSource { get; set; }

        /// <summary>
        /// Get/set the language source token
        /// </summary>
        [DataMember]
        public string LanguageSourceToken { get; set; }

        /// <summary>
        /// Get/set template description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Get/set whether the survey is active.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Get/set the survey activation start date.
        /// </summary>
        [DataMember]
        public DateTime? ActivationStartDate { get; set; }

        /// <summary>
        /// Get/set the survey activation end data.
        /// </summary>
        [DataMember]
        public DateTime? ActivationEndDate { get; set; }

        /// <summary>
        /// Get/set the maximum total responses for the survey.
        /// </summary>
        [DataMember]
        public int? MaxTotalResponses { get; set; }

        /// <summary>
        /// Get/set the maximum total responses for a user.
        /// </summary>
        [DataMember]
        public int? MaxResponsesPerUser { get; set; }

        /// <summary>
        /// Get/set whether the back button is disabled/hidden.
        /// </summary>
        [DataMember]
        public bool DisableBackButton { get; set; }

        /// <summary>
        /// Get/set whether to allow resume.
        /// </summary>
        [DataMember]
        public bool AllowResume { get; set; }

        /// <summary>
        /// Get/set whether to allow editing
        /// </summary>
        [DataMember]
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Get/set whether responses are anonymized.
        /// </summary>
        [DataMember]
        public bool AnonymizeResponses { get; set; }

        /// <summary>
        /// Get/set whether to allow editing the survey while it is active.
        /// </summary>
        [DataMember]
        public bool AllowSurveyEditWhileActive { get; set; }

        /// <summary>
        /// Get/set whether to allow editing the survey while it is active.
        /// </summary>
        [DataMember]
        public bool DisplayPDFDownloadButton { get; set; }

        /// <summary>
        /// Get/set the database id of the style template associated with this response template.
        /// </summary>
        [DataMember]
        public int? StyleTemplateID { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> in tablet browsers
        /// </summary>
        [DataMember]
        public int? TabletStyleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> in smartphone browsers
        /// </summary>
        [DataMember]
        public int? SmartPhoneStyleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> for mobile browsers
        /// </summary>
        [DataMember]
        public int? MobileStyleID { get; set; }
        
        /// <summary>
        /// Get/set whether the current user can assign style for this survey
        /// </summary>
        [DataMember]
        public bool CanAssignStyleTemplates { get; set; }

        /// <summary>
        /// Get/set whether the current user can add custom url for this survey
        /// </summary>
        [DataMember]
        public bool CanRewriteSurveyUrl { get; set; }

        /// <summary>
        /// Get/set whether the current user can assign style for this survey
        /// </summary>
        [DataMember]
        public bool CanChangeLanguages { get; set; }

        /// <summary>
        /// Get/set whether to dynamically calculate item numbers based on conditions and branching
        /// </summary>
        [DataMember]
        public bool UseDynamicItemNumbers { get; set; }

        /// <summary>
        /// Get/set whether to dynamically calculate page nubmers based on conditions and branching.
        /// </summary>
        [DataMember]
        public bool UseDynamicPageNumbers { get; set; }

        /// <summary>
        /// Get/set whether to show a javascript alert when a survey page fails validation.
        /// </summary>
        [DataMember]
        public bool ShowInputErrorAlert { get; set; }

        /// <summary>
        /// Get/set whether to show page numbers.
        /// </summary>
        [DataMember]
        public bool ShowPageNumbers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show top survey buttons].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show top survey buttons]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool ShowTopSurveyButtons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show top survey buttons on first and last page].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show top survey buttons on first and last page]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool HideTopSurveyButtonsOnFirstAndLastPage { get; set; }

        /// <summary>
        /// Get/set whether to show item numbers.
        /// </summary>
        [DataMember]
        public bool ShowItemNumbers { get; set; }

        /// <summary>
        /// Get/set whether to show the progress bar.
        /// </summary>
        [DataMember]
        public bool ShowProgressBar { get; set; }

        /// <summary>
        /// Get/set progress bar.
        /// </summary>
        [DataMember]
        public int ProgressBarOrientation { get; set; }

        /// <summary>
        /// Get/set whether to show the save and quit button to respondents.
        /// </summary>
        [DataMember]
        public bool ShowSaveAndQuit { get; set; }

        /// <summary>
        /// Get/set whether to randomize item positions within the page that contains them.
        /// </summary>
        [DataMember]
        public bool RandomizeItems { get; set; }

        /// <summary>
        /// Get/set whether to enable scoring for the survey.
        /// </summary>
        [DataMember]
        public bool EnableScoring { get; set; }

        /// <summary>
        /// Get/set whether to enable scoring for the survey.
        /// </summary>
        [DataMember]
        public bool AllowFormReset { get; set; }

        /// <summary>
        /// Get/set whether to show the survey title.
        /// </summary>
        [DataMember]
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Get/set whether to show asterisk symbols for required items.
        /// </summary>
        [DataMember]
        public bool ShowAsterisks { get; set; }

        /// <summary>
        /// Get/set whether to hide footer and header on mobile devices
        /// </summary>
        [DataMember]
        public bool HideFooterHeader { get; set; }

        /// <summary>
        /// Get/set whether survey is available to respondents based on IsActive setting and
        /// activation start/end date.
        /// </summary>
        [DataMember]
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Get/set code indicating why survey is not available.  Possible values are:
        ///   notactive - Survey Active flag not set.
        ///   beforestartdate - Current date is earlier than survey start date.
        ///   afterenddate - Current end date is later than survey end date.
        /// </summary>
        [DataMember]
        public string NotAvailableCode { get; set; }

        /// <summary>
        /// Get/set the response template GUID.
        /// </summary>
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// Get/set the survey URL.
        /// </summary>
        /// <remarks>This property returns 2 or 3 urls depending on Checkbox application
        /// settings , following this precedence order: a url using a GUID,  a url using an integer ID, custom survey url</remarks>
        [DataMember]
        public string[] SurveyUrls { get; set; }

        /// <summary>
        /// Get/set the date the survey was created.
        /// </summary>
        /// <remarks>This value is not modified in the Checkbox database when updating a survey.</remarks>
        [DataMember]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Get/set date survey was last modified.
        /// </summary>
        [DataMember]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Get/Set the survey password.
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Get ids of pages associated with survey.
        /// </summary>
        [DataMember]
        public int[] PageIds { get; set; }

        /// <summary>
        /// Get ids of items contained in survey. 
        /// </summary>
        [DataMember]
        public int[] ItemIds { get; set; }

        /// <summary>
        /// Get invitations mode
        /// </summary>
        [DataMember]
        public bool SheduledInvitations { get; set; }

        /// <summary>
        /// Get whether Email is enabled in the application
        /// </summary>
        [DataMember]
        public bool ShowInvitations { get; set; }

        ///<summary>
        /// Get whether Facebook links should be shown or not
        /// </summary>
        [DataMember]
        public bool ShowFacebook { get; set; }

        ///<summary>
        /// Get full url to share via twitter
        /// </summary>
        [DataMember]
        public string TwitterUrl { get; set; }

        ///<summary>
        /// Get full url to share via Facebook
        /// </summary>
        [DataMember]
        public string FacebookUrl { get; set; }
        
        ///<summary>
        /// Get full url to share via GooglePlus
        /// </summary>
        [DataMember]
        public string GplusUrl { get; set; }
        
        ///<summary>
        /// Get full url to share via LinkedIn
        /// </summary>
        [DataMember]
        public string LinkedInUrl { get; set; }

        ///<summary>
        /// Is Form.Edit permission available
        /// </summary>
        [DataMember]
        public bool FormEditPermission { get; set; }

        ///<summary>
        /// Google analytics tracking ID
        /// </summary>
        [DataMember]
        public string GoogleAnalyticsTrackingID { get; set; }

        ///<summary>
        /// Determines whether we should insert physical page break after page
        /// </summary>
        [DataMember]
        public Dictionary<int?, bool?> PageBreaks { get; set; }

        ///<summary>
        /// Determines whether we should insert physical page break after page
        /// </summary>
        [DataMember]
        public List<TermsMetaData> Terms { get; set; }
    }
}
