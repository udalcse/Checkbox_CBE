namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class MatrixNumericSlider : SliderBase
    {
        /// <summary>
        /// Get current value
        /// </summary>
        public string CurrentValue
        {
            get { return Request["numericSlider_" + ClientID]; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string AnsweredValue
        {
            get
            {
                return Model.Answers.Length > 0 ? Model.Answers[0].AnswerText : DefaultValue.ToString();
            }
        }
    }
}