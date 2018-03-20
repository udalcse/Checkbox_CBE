using Checkbox.Common;

namespace Checkbox.Analytics.Items.UI
{
    public class HeatMapSummaryAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get heat map summary table appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_HEAT_MAP_SUMMARY"; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable numerical labels
        /// </summary>
        /// <value>
        ///   <c>true</c> if [numerical labels]; otherwise, <c>false</c>.
        /// </value>
        public int LableFontSize
        {
            get { return Utilities.AsInt(this["LableFontSize"], 0); }
            set { this["LableFontSize"] = value.ToString(); }
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

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable grid line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [grid line]; otherwise, <c>false</c>.
        /// </value>
        public bool RespondentLabels
        {
            get { return Utilities.AsBool(this["RespondentLabels"], true); }
            set { this["RespondentLabels"] = value.ToString(); }
        }

        public HeatMapSummaryAppearanceData()
        {
            this.GridLine = true;
            this.LableFontSize = 14;
            this.FontFamily = "Times New Roman";
            this.RespondentLabels = true;
        }

    }
}
