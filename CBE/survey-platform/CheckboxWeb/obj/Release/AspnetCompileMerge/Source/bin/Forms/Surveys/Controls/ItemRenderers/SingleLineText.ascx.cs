using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.UI.Controls;
using Newtonsoft.Json;
using Checkbox.Users;
using Checkbox.Security.Principal;
using System.Web;
using Checkbox.Security;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Renderer class for single line text inputs
    /// </summary>
    public partial class SingleLineText : SingleLineControlBase
    {
        protected string DateFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override List<UserControlItemRendererBase> ChildUserControls
        {
            get
            {
                var childControls = base.ChildUserControls;
                childControls.Add(_questionText);
                return childControls;
            }
        }

        /// <summary>
        /// Initialize child user controls to set repeat columns and other appearance properties
        /// </summary>
        protected override void InlineInitialize()
        {
            //Item and label position
            SetLabelPosition();
            SetItemPosition();
            SetInputWidth();
        }


        /// <summary>
        /// Bind controls to survey item.
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            //Input properties, such as showing/hiding proper input as well
            // as width and any restrictions based on answer format
            SetInputProperties();

            var textBox = _textInput.Visible ? _textInput :
                _numericInput.Visible ? _numericInput :
                _dateInput.Visible ? _dateInput :
                _maskedInput;

            if (Model.Answers.Length > 0)
                textBox.Text = Model.Answers[0].AnswerText;
            else
                textBox.Text = Model.Metadata["DefaultText"] ?? string.Empty;

            //set the default text
            textBox.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? string.Empty;

            InitAutocomplete(_textInput);

            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]) )
                _textInput.Attributes.Add("binded-field", "true");
        }

        /// <summary>
        /// Updates Profile Properties if the Profile Property connected the the item
        /// It means that the whole question is connected to Profile Property value
        /// And no matter which answer will be chosen by survey taker 
        /// it will be saved to this profile property
        /// </summary>
        /// <param name="inputText"></param>
        private void UpdateConnectedProfileData(string inputText)
        {
            if (string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                return;

            var name = PropertyBindingManager.GetCurrentUserName();

            if (!string.IsNullOrWhiteSpace(name))
            {
                ProfilePropertiesUpdater propertiesUpdater = new ProfilePropertiesUpdater();
                propertiesUpdater.UpdateUserProfileData(inputText,
                    Model.Metadata["ConnectedCustomFieldKey"], name);
            }
        }

        /// <summary>
        /// Update model
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            //Set text as answer
            //Get answer from whatever input is visible
            string text = string.Empty;
            
            if (_textInput.Visible)
                text = Request[_textInput.UniqueID] ?? _textInput.Text;
            else if (_numericInput.Visible)
                text = Request[_numericInput.UniqueID] ?? _numericInput.Text;
            else if (_dateInput.Visible)
                text = Request[_dateInput.UniqueID] ?? _dateInput.Text;
            else if (_maskedInput.Visible)
                text = Request[_maskedInput.UniqueID] ?? _maskedInput.Text;

            UpsertTextAnswer(text.Trim());

            UpdateConnectedProfileData(Request[_textInput.UniqueID]);

        }

        /// <summary>
        /// Get answer format from model
        /// </summary>
        /// <returns></returns>
        protected AnswerFormat GetAnswerFormat()
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
        /// Set accepted value for inputs, based on format
        /// </summary>
        protected virtual void SetInputProperties()
        {
            //By default, text input is visible and others are hidden
            _textInput.Visible = true;
            _dateInput.Visible = false;
            _numericInput.Visible = false;
            _maskedInput.Visible = false;

            AnswerFormat answerFormat = GetAnswerFormat();

            switch (answerFormat)
            {
                //Date Formats
                case AnswerFormat.Date:
                    _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                    _textInput.Visible = !_dateInput.Visible;
                    DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLower().Replace("yyyy", "yy");
                    SetDateValues();
                    break;

                case AnswerFormat.Date_ROTW:
                    _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                    _textInput.Visible = !_dateInput.Visible;
                    DateFormat = "d/m/yy";
                    SetDateValues();
                    break;

                case AnswerFormat.Date_USA:
                    _dateInput.Visible = ApplicationManager.AppSettings.UseDatePicker;
                    _textInput.Visible = !_dateInput.Visible;
                    DateFormat = "m/d/yy";
                    SetDateValues();
                    break;

                //Masked formats
                case AnswerFormat.Phone:
                    _textInput.Visible = false;
                    _maskedInput.Visible = true;
                    //_maskedInput.Mask = "(###) ### - ####";
                    //_maskedInput.PromptChar = "#";
                    break;

                case AnswerFormat.SSN:
                    _textInput.Visible = false;
                    _maskedInput.Visible = true;
                    //_maskedInput.Mask = "###-##-####";
                    //_maskedInput.PromptChar = "#";
                    break;

                //Numeric formats
                case AnswerFormat.Integer:
                    _textInput.Visible = false;
                    _numericInput.Visible = true;
                    //_numericInput.NumberFormat.DecimalDigits = 0;
                    SetNumericValues();
                    break;

                case AnswerFormat.Decimal:
                case AnswerFormat.Numeric:
                    _textInput.Visible = false;
                    _numericInput.Visible = true;
                    SetNumericValues();
                    break;

                //Text formats with JS input limitation in addition to server
                // side validation.
                case AnswerFormat.Alpha:
                case AnswerFormat.AlphaNumeric:
                case AnswerFormat.Lowercase:
                case AnswerFormat.Uppercase:
                    SetMaxLength();
                    break;

                //Other formats have only server-side validation & only need
                // answer values set and length restricted.
                default:
                    SetMaxLength();
                    break;
            }
        }

        /// <summary>
        /// Set associated control id for 508 input
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_maskedInput.Visible)
            {
                _questionText.SetAssociatedInputId(_maskedInput.ClientID);
            }

            if (_dateInput.Visible)
            {
                _questionText.SetAssociatedInputId(_dateInput.ClientID);
            }

            if (_textInput.Visible)
            {
                _questionText.SetAssociatedInputId(_textInput.ClientID);
            }
        }

        /// <summary>
        /// Set min/max and answer date values
        /// </summary> 
        protected virtual void SetDateValues()
        {
            DateTime? minDate = Utilities.GetDate(Model.Metadata["MinDateValue"]);
            DateTime? maxDate = Utilities.GetDate(Model.Metadata["MaxDateValue"]);

            DateTime? answerValueDate = Utilities.GetDate(Model.InstanceData["AnswerValueAsDate"], CultureInfo.InvariantCulture);

            //if (minDate.HasValue)
            //{
            //    _datePicker.MinDate = minDate.Value;
            //}

            //if (maxDate.HasValue)
            //{
            //    _datePicker.MaxDate = maxDate.Value;
            //}

            if (answerValueDate.HasValue)
            {
                AnswerFormat answerFormat = GetAnswerFormat();

                string formatString = "d";

                if (answerFormat == AnswerFormat.Date_USA)
                {
                    formatString = "MM/dd/yyyy";
                }

                if (answerFormat == AnswerFormat.Date_ROTW)
                {
                    formatString = "dd/MM/yyyy";
                }

                if (ApplicationManager.AppSettings.UseDatePicker)
                {
                    _dateInput.Text = answerValueDate.Value.ToUniversalTime().ToString(formatString, CultureInfo.InvariantCulture);
                }
                else
                {
                    _textInput.Text = answerValueDate.Value.ToUniversalTime().ToString(formatString, CultureInfo.InvariantCulture);
                }
            }
        }


        /// <summary>
        /// Set min/max and answer values for numeric values
        /// </summary>
        protected void SetNumericValues()
        {
            //double? minNumericValue = Utilities.AsDouble(Model.Metadata["MinNumericValue"]);
            //double? maxNumericValue = Utilities.AsDouble(Model.Metadata["MaxNumericValue"]);

            //if (minNumericValue.HasValue)
            //{
            //_numericInput.MinValue = minNumericValue.Value;
            //}

            //if (maxNumericValue.HasValue)
            //{
            //_numericInput.MaxValue = maxNumericValue.Value;
            //}

            _numericInput.Text = Model.Answers.Length > 0
                ? Model.Answers[0].AnswerText
                : Model.InstanceData["DefaultText"];
        }

        /// <summary>
        /// Set max length of text
        /// </summary>
        protected virtual void SetMaxLength()
        {
            int? maxLength = Utilities.AsInt(Model.Metadata["MaxLength"]);

            if (maxLength.HasValue)
            {
                _textInput.MaxLength = maxLength.Value;
            }
        }

        /// <summary>
        /// Set width of text input
        /// </summary>
        protected void SetInputWidth()
        {
            if (Utilities.IsNotNullOrEmpty(Appearance["Width"]))
            {
                _textInput.Width = Unit.Pixel(int.Parse(Appearance["Width"]));
            }
        }


        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected void SetLabelPosition()
        {
            //When label is set to bottom, we need to move controls from the top panel
            // to the bottom panel.  Otherwise, position changes are managed by setting
            // CSS class.
            if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                //Move text controls to bottom
                _bottomAndOrRightPanel.Controls.Add(_textContainer);

                //Move input to top
                _topAndOrLeftPanel.Controls.Add(_inputPanel);
            }

            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

            if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
            }
        }
    }
}