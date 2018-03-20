using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Renderer class for single line text inputs
    /// </summary>
    public partial class Matrix_SingleLineText : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Model == null || Page == null || !ApplicationManager.AppSettings.UseDatePicker)
            {
                return;
            }
            var parent = Parent as MatrixChildrensItemRenderer; 

            var answerFormat = GetAnswerFormat();
            
            // if it is mobile device set multiloine mode for text box 
           // _textBox.TextMode = this.IsMobileSurvey ? TextBoxMode.MultiLine : TextBoxMode.SingleLine;

            if (answerFormat == AnswerFormat.Date || answerFormat == AnswerFormat.Date_ROTW || answerFormat == AnswerFormat.Date_USA)
            {
                //_textBox.Text = DateTime.UtcNow.Date.ToString("dd-MM-yyyy");
                _textBox.TextMode = TextBoxMode.SingleLine;
                if (answerFormat == AnswerFormat.Date)
                {
                    DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower().Replace("yyyy", "yy");
                }

                if (answerFormat == AnswerFormat.Date_ROTW)
                {
                    DateFormat = "d/m/yy";
                }

                if (answerFormat == AnswerFormat.Date_USA)
                {
                    DateFormat = "m/d/yy";
                }

                _textBox.Attributes.Add("matrixdatetimepicker", "true");

                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "DatePicker_" + _textBox.ClientID,
                    "$(document).ready(function () {       " +
                    "   $('#" + _textBox.ClientID + "[matrixdatetimepicker=true]').datepicker({  " +
                    "      showOn: 'both', " +
                    "      buttonImageOnly: true, " +
                    "      buttonImage: '" + ResolveUrl("~/Resources/CalendarPopup.png") + "'," +
                    "      buttonText: 'Calendar', " +
                    "      dateFormat: '" + DateFormat + "'," +
                    "      changeMonth: true, " +
                    "      changeYear: true " +
                    "   }); " +
                    "});",
                    true);
            }
        }

        protected string DateFormat { get; set; }

        /// <summary>
        /// Get answer format from model
        /// </summary>
        /// <returns></returns>
        private AnswerFormat GetAnswerFormat()
        {
            AnswerFormat answerFormat = AnswerFormat.None;

            if (Utilities.IsNotNullOrEmpty(Model.Metadata["AnswerFormat"]))
            {
                try
                {
                    answerFormat = (AnswerFormat)Enum.Parse(typeof(AnswerFormat), Model.Metadata["AnswerFormat"]);
                }
                catch
                {
                }
            }

            return answerFormat;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCellClassName()
        {
            var parent = Parent as MatrixChildrensItemRenderer;

            if (parent == null)
            {
                return string.Empty;
            }

            string borderClass;
            if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
                borderClass = "BorderRight";
            else
                if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
                borderClass = "BorderTop";
            else
                borderClass = "Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                    ? "BorderBoth"
                    : String.Empty;

            string alignClass = parent.ChildType == MatrixChildType.RowText && Utilities.IsNotNullOrEmpty(parent.RowTextPosition)
                                     ? "rowTextPosition" + parent.RowTextPosition
                                     : String.Empty;
            var separator = Utilities.IsNotNullOrEmpty(borderClass) && Utilities.IsNotNullOrEmpty(alignClass)
                                ? " "
                                : String.Empty;
            return borderClass + separator + alignClass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCellWidthStyle()
        {
            var parent = Parent as MatrixChildrensItemRenderer;

            //Otherwise use column width
            if (parent == null || !parent.ColumnWidth.HasValue)
            {
                return string.Empty;
            }

            return "width: " + parent.ColumnWidth + "px;";
        }

        /// <summary>
        /// Bind control with the model
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _textBox.Text = Model.Answers.Length > 0
                                ? Model.Answers[0].AnswerText
                                : (Model.Metadata["DefaultText"] ?? String.Empty);

            bool widthSet = false;
           
            //Check for explicit width
            if (Appearance != null)
            {
                var width = Utilities.AsInt(Appearance["Width"]);

                if (width.HasValue)
                {
                    _textBox.Width = Unit.Pixel(width.Value);
                    widthSet = true;
                }
            }

            //set the default text
            _textBox.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? String.Empty;

            var parent = Parent as MatrixChildrensItemRenderer;

            //Otherwise use column width
            if (widthSet || parent == null || !parent.ColumnWidth.HasValue || parent.ColumnWidth.Value < 20)
            {
                return;
            }

            _textBox.Width = Unit.Pixel(parent.ColumnWidth.Value - 10);

        }


        /// <summary>
        /// Update answered text
        /// </summary>
        protected override void InlineUpdateModel()
        {
            string text = Request[_textBox.UniqueID] ?? string.Empty;
            UpsertTextAnswer(text.Trim());
        }
    }
}