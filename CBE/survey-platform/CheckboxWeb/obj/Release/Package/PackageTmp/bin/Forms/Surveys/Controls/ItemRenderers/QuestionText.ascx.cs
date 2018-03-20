using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class QuestionText : UserControlSurveyItemRendererBase
    {
        public string AssociatedControlId { get; set; }

        /// <summary>
        /// Set input associated with question for 508 compliance purposes
        /// </summary>
        /// <param name="inputId"></param>
        public void SetAssociatedInputId(string inputId)
        {
            AssociatedControlId = inputId;
        }
    }
}