using System;
using Checkbox.Forms.Items;

namespace Checkbox.Forms
{
    /// <summary>
    /// Container for style related survey settings
    /// </summary>
    [Serializable]
    public class SurveyStyleSettings
    {
        private ProgressBarOrientation? _progressBarOrientation;

        /// <summary>
        /// Get/set border width
        /// </summary>
        public int? BorderWidth { get; set; }

        /// <summary>
        /// Get/set border style
        /// </summary>
        public string BorderStyle { get; set; }

        /// <summary>
        /// Get/set border color
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// Get/set Height
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Get/set Height
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/>
        /// </summary>
        public int? StyleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> in tablet browsers
        /// </summary>
        public int? TabletStyleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> in smartphone browsers
        /// </summary>
        public int? SmartPhoneStyleTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/> for mobile browsers
        /// </summary>
        public int? MobileStyleId { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/>
        /// </summary>
        public int? StyleTemplateIdTablet { get; set; }

        /// <summary>
        /// Gets or sets the database ID of the Style used by this <see cref="ResponseTemplate"/>
        /// </summary>
        public int? StyleTemplateIdSmartphone { get; set; }

        /// <summary>
        /// Get/set chart style id associated with the response template.  Currently, this value is only
        /// used for polls.
        /// </summary>
        public int? ChartStyleId { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to display numbers with Items
        /// </summary>
        public bool ShowItemNumbers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show top survey buttons].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show top survey buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowTopSurveyButtons { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show top survey buttons on first and last page].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show top survey buttons on first and last page]; otherwise, <c>false</c>.
        /// </value>
        public bool HideTopSurveyButtonsOnFirstAndLastPage { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to display Page numbers
        /// </summary>
        public bool ShowPageNumbers { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to dynamically number Items accounting for conditions
        /// </summary>
        public bool EnableDynamicItemNumbers { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to dynamically number Pages accounting for conditions
        /// </summary>
        public bool EnableDynamicPageNumbers { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to display a progress bar
        /// </summary>
        public bool ShowProgressBar { get; set; }

        /// <summary>
        /// Gets or sets a progress bar orientation
        /// </summary>
        public ProgressBarOrientation? ProgressBarOrientation
        {
            get
            {
                return _progressBarOrientation.HasValue 
                    ? _progressBarOrientation.Value
                    : Items.ProgressBarOrientation.Top_Left;
            }
            set { _progressBarOrientation = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether show the <see cref="ResponseTemplate"/> Title
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets whether to show a validation message summary of all validation errors contained on the Page
        /// </summary>
        public bool ShowValidationMessage { get; set; }

        /// <summary>
        /// Gets or sets whether to pop-up a validation alert when a Page is invalid
        /// </summary>
        public bool ShowValidationErrorAlert { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to display asterisk symbols for required Items
        /// </summary>
        public bool ShowAsterisks { get; set; }

        /// <summary>
        /// Get/set whether to hide footer and header on mobile devices
        /// </summary>
        public bool HideFooterHeader { get; set; }

        /// <summary>
        /// Get a response view display flags enum based on the current style settings.
        /// </summary>
        /// <returns></returns>
        public ResponseViewDisplayFlags GetDisplayFlags()
        {
            ResponseViewDisplayFlags displayFlags = new ResponseViewDisplayFlags();

            if (ShowItemNumbers)
            {
                displayFlags |= ResponseViewDisplayFlags.ItemNumbers;
            }

            if (ShowPageNumbers)
            {
                displayFlags |= ResponseViewDisplayFlags.PageNumbers;
            }

            if (ShowProgressBar)
            {
                displayFlags |= ResponseViewDisplayFlags.ProgressBar;
            }

            if (ShowTitle)
            {
                displayFlags |= ResponseViewDisplayFlags.Title;
            }

            if (ShowAsterisks)
            {
                displayFlags |= ResponseViewDisplayFlags.Asterisks;
            }

            return displayFlags;
        }
    }
}
