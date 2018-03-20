using System;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class MatrixNumericSlider : SliderBase
    {
        /// <summary>
        /// Get current value
        /// </summary>
        public string CurrentValue
        {
            get { return Request[_currentValue.UniqueID]; }
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _currentValue.Text = AnsweredValue;
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