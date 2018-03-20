using System;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Simple container for passing report wizard options.
    /// </summary>
    [Serializable]
    public class AnalysisWizardOptions
    {
        //report settings
        public string ItemPostion { get; set; }
        public string GraphType { get; set; }
        public int MaxOptions { get; set; }
        public bool UseAliases { get; set; }
        public bool DisplayStatistics { get; set; }
        public bool DisplayAnswers { get; set; }
        public bool IsSinglePageReport { get; set; }
        public bool IncludeIncompleteResponses { get; set; }
        public bool IncludeTestResponses { get; set; }

        //item settings
        public string SingleLineTextGraphType { get; set; }
        public string MultiLineTextGraphType { get; set; }
        public string SliderGraphType { get; set; }
        public string HiddenItemGraphType { get; set; }
        public string MatrixGraphType { get; set; }
        public string RadioButtonGraphType { get; set; }
        public string DropDownListGraphType { get; set; }
        public string CheckboxGraphType { get; set; }
        public string RatingScaleGraphType { get; set; }
        public string RankOrderGraphType { get; set; }
        public string NetPromoterScoreGraphType { get; set; }
    }
}
