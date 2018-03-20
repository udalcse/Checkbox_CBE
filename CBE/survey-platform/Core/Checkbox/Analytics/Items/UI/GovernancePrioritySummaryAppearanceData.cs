using Checkbox.Common;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// GovernancePrioritySummaryAppearanceData
    /// </summary>
    /// <seealso cref="Checkbox.Analytics.Items.UI.AnalysisItemAppearanceData" />
    public class GovernancePrioritySummaryAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Gets the string code for this AppearanceData, that represenets how this module will look like 
        /// <remarks>
        /// This would be the code used to map UI controls to the appearance, for example RADIO_BUTTONS
        /// </remarks>
        /// </summary>
        public override string AppearanceCode => "ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY";

        /// <summary>
        /// Initializes a new instance of the <see cref="GovernancePrioritySummaryAppearanceData"/> class.
        /// </summary>
        public GovernancePrioritySummaryAppearanceData()
        { 
            this.GridLine = true;
            this.ShowValuesOnBars = true;
            this.TitleFontSize = 14;
            this.BarColor = "#995001";
            this.FontFamily = "Times New Roman";
        }    
            
        /// <summary>
        /// Gets or sets a value indicating whether enable or disable numerical labels
        /// </summary>
        /// <value>
        ///   <c>true</c> if [numerical labels]; otherwise, <c>false</c>.
        /// </value>
        public int TitleFontSize
        {
            get { return Utilities.AsInt(this["TitleFontSize"], 0); }
            set { this["TitleFontSize"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public string FontFamily
        {
            get { return this["FontFamily"]; }
            set { this["FontFamily"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable numerical labels
        /// </summary>
        /// <value>
        ///   <c>true</c> if [numerical labels]; otherwise, <c>false</c>.
        /// </value>
        public string BarColor
        {
            get { return this["BarColor"]; }
            set { this["BarColor"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable numerical labels
        /// </summary>
        /// <value>
        ///   <c>true</c> if [numerical labels]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValuesOnBars
        {
            get { return Utilities.AsBool(this["ShowValuesOnBars"], true); }
            set { this["ShowValuesOnBars"] = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable grid line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [grid line]; otherwise, <c>false</c>.
        /// </value>
        public bool GridLine
        {
            get { return Utilities.AsBool(this["GridLine"], true); }
            set { this["GridLine"] = value.ToString(); }
        }
    }
}
