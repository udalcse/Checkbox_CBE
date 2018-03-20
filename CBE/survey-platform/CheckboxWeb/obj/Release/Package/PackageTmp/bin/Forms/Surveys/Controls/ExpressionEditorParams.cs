using System;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Parameters governing behavior of expression editors.
    /// </summary>
    [Serializable]
    public class ExpressionEditorParams
    {
        public RuleDataService RuleDataService { get; set; }
        public string LanguageCode { get; set; }
        public int MaxSourceQuestionPagePosition { get; set; }
        public int ResponseTemplateId { get; set; }
        public int? DependentPageId { get; set; }
        public int? DependentItemId { get; set; }
        public RuleType RuleType { get; set; }
    }
}
