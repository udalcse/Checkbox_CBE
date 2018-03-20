using Checkbox.Common;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// Represents appearence configuration settings for color director matrix chart
    /// </summary>
    /// <seealso cref="Checkbox.Analytics.Items.UI.AnalysisItemAppearanceData" />
    public class GradientColorDirectorSkillsMatrixAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Gets the string code for this AppearanceData
        /// <remarks>
        /// This would be the code used to map UI controls to the appearance, for example RADIO_BUTTONS
        /// </remarks>
        /// </summary>
        public override string AppearanceCode { get; } = "ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX";

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientColorDirectorSkillsMatrixAppearanceData"/> class.
        /// </summary>
        public GradientColorDirectorSkillsMatrixAppearanceData()
        {
            this.GridLine = false;
            this.TitleFontSize = 12;
            this.FontFamily = "Times New Roman";
            this.DirectorAverages = true;
            this.ItemColumnHeader = "Skill Areas";
            this.AveragesColumnHeader = "Board Average";
            this.RatingDetailsHeader = "Directors (Self Ratings)";
            this.SummaryHeader = "Total Expertise Average by Director";
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable numerical labels
        /// </summary>
        /// <value>
        ///   <c>true</c> if [numerical labels]; otherwise, <c>false</c>.
        /// </value>
        public bool DirectorAverages
        {
            get { return Utilities.AsBool(this["DirectorAverages"], true); }
            set { this["DirectorAverages"] = value.ToString(); }
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
        public string ItemColumnHeader
        {
            get { return this["ItemColumnHeader"]; }
            set { this["ItemColumnHeader"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable grid line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [grid line]; otherwise, <c>false</c>.
        /// </value>
        public string AveragesColumnHeader
        {
            get { return this["AveragesColumnHeader"]; }
            set { this["AveragesColumnHeader"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable or disable grid line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [grid line]; otherwise, <c>false</c>.
        /// </value>
        public string RatingDetailsHeader
        {
            get { return this["RatingDetailsHeader"]; }
            set { this["RatingDetailsHeader"] = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether enable or disable grid line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [grid line]; otherwise, <c>false</c>.
        /// </value>
        public string SummaryHeader
        {
            get { return this["SummaryHeader"]; }
            set { this["SummaryHeader"] = value; }
        }
    }
}
