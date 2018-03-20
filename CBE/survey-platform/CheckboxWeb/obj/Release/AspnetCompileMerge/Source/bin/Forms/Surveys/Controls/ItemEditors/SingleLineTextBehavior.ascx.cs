using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Management;
using Checkbox.Web;


namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Single line text behavior editor control
    /// </summary>
    public partial class SingleLineTextBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public bool PageIsPostback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int TemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected EditMode EditMode { get; set; }


        /// <summary>
        /// Get a boolean indicating if the answer format is a date type.
        /// </summary>
        public bool GetIsFormatDate(string formatString)
        {
            return
                formatString.Equals(AnswerFormat.Date.ToString(), StringComparison.InvariantCultureIgnoreCase)
                || formatString.Equals(AnswerFormat.Date_ROTW.ToString(), StringComparison.InvariantCultureIgnoreCase)
                || formatString.Equals(AnswerFormat.Date_USA.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///  Get a boolean indicating if the answer format is a numeric type.
        /// </summary>
        public bool GetIsFormatNumeric(string formatString)
        {
            return
                formatString.Equals(AnswerFormat.Decimal.ToString(), StringComparison.InvariantCultureIgnoreCase)
                || formatString.Equals(AnswerFormat.Integer.ToString(), StringComparison.InvariantCultureIgnoreCase)
                || formatString.Equals(AnswerFormat.Money.ToString(), StringComparison.InvariantCultureIgnoreCase)
                || formatString.Equals(AnswerFormat.Numeric.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get the culture for a date from the format string.
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public CultureInfo GetDateCulture(string formatString)
        {
            if (formatString.Equals(AnswerFormat.Date_ROTW.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return CultureInfo.GetCultureInfo("en-GB");
            }

            return CultureInfo.GetCultureInfo("en-US");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            _pipeSelector.ID = ID + "_" + _pipeSelector.ID;

            base.OnInit(e);
        }

        /// <summary>
        /// Update control visibility on load.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));

        }

        /// <summary>
        /// Initialize control with data to edit.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        /// <param name="isOnlyNumericFormat"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(TextItemDecorator textItemDecorator, bool isOnlyNumericFormat, bool isPagePostBack, string currentLanguage, int templateId, int? pagePosition, EditMode editMode)
        {
            if (!(textItemDecorator.Data is SingleLineTextItemData))
            {
                return;
            }

            var slTextItemData = (SingleLineTextItemData) textItemDecorator.Data;

            if (isOnlyNumericFormat) //Clear textFormatList if it's needed
            {
                for (int i = _textFormatList.Items.Count - 1; i >= 0; i--)
                {
                    if (!GetIsFormatNumeric(_textFormatList.Items[i].Value))
                        _textFormatList.Items.RemoveAt(i);
                }
            }

            //Set initial states for items
            _textFormatList.SelectedValue = _textFormatList.Items.FindByValue(textItemDecorator.Data.Format.ToString()) != null
                ? textItemDecorator.Data.Format.ToString()
                : "None";

            bool isNumeric = GetIsFormatNumeric(_textFormatList.SelectedValue);

            //Required
            _requiredChk.Checked = textItemDecorator.Data.IsRequired;

            //Max length
            _maxLengthTxt.Text = textItemDecorator.Data.MaxLength.HasValue
                ? textItemDecorator.Data.MaxLength.ToString()
                : string.Empty;

            _aliasText.Text = textItemDecorator.Data.Alias;
            
            //Default value
            _defaultTextTxt.Text = textItemDecorator.DefaultText;
            _minValueNumberTxt.Text = slTextItemData.MinValue.HasValue ? slTextItemData.MinValue.ToString() : string.Empty;
            _maxValueNumberTxt.Text = slTextItemData.MaxValue.HasValue ? slTextItemData.MaxValue.ToString() : string.Empty;

            //If default value has a piped value, perform no conversions
            var isPipe = _defaultTextTxt.Text.Contains(ApplicationManager.AppSettings.PipePrefix);

                if (IsDate)
                {
                    CultureInfo ci = GetDateCulture(_textFormatList.SelectedValue);

                    //Attempt to convert default date from ticks to date time.  Ignore if default value has pipe

                    if (!isPipe)
                    {
                        var defaultDate = Utilities.GetDate(_defaultTextTxt.Text, ci);

                        _defaultTextTxt.Text = defaultDate.HasValue
                                                   ? defaultDate.Value.ToString("d", ci)
                                                   : string.Empty;
                    }


                    _minValueNumberTxt.Text = !slTextItemData.MinValue.HasValue
                                                  ? string.Empty
                                                  : new DateTime(Convert.ToInt64(Math.Round((slTextItemData.MinValue.Value)))).ToString("d", ci);

                    _maxValueNumberTxt.Text = !slTextItemData.MaxValue.HasValue
                                                  ? ""
                                                  : new DateTime(Convert.ToInt64(Math.Round(slTextItemData.MaxValue.Value))).ToString("d", ci);
                }

                if (isNumeric)
                {
                    _minValueNumberTxt.Text = ((SingleLineTextItemData) textItemDecorator.Data).MinValue.ToString();
                    _maxValueNumberTxt.Text = ((SingleLineTextItemData) textItemDecorator.Data).MaxValue.ToString();
                }
            

            PageIsPostback = isPagePostBack;
            LanguageCode = currentLanguage;
            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;


            //Initialize pipeSelector
            switch (EditMode)
            {
                case EditMode.Survey:
                    _pipeSelector.Initialize(TemplateId, PagePosition, currentLanguage, _defaultTextTxt.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelector.Initialize(null, null, currentLanguage, _defaultTextTxt.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelector.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsDate
        {
            get
            {
                return GetIsFormatDate(_textFormatList.SelectedValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DatePickerLocale
        {
            get
            {
                CultureInfo culture = GetDateCulture(_textFormatList.SelectedValue);
                return culture == null ? "en-US" : culture.Name.Substring(0, culture.Name.IndexOf("-"));
            }
        }

        /// <summary>
        /// Validate data
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //Turn off validation errors
            _defaultValueError.Visible = false;
            _minValueError.Visible = false;
            _maxValueError.Visible = false;
            _maxLengthError.Visible = false;

            var answerFormat = (AnswerFormat)Enum.Parse(typeof(AnswerFormat), _textFormatList.SelectedValue);

            //Check length of default values
            bool lengthValidationRequired = (
                answerFormat == AnswerFormat.None
                || answerFormat == AnswerFormat.Alpha
                || answerFormat == AnswerFormat.AlphaNumeric
                || answerFormat == AnswerFormat.Email
                || answerFormat == AnswerFormat.Uppercase
                || answerFormat == AnswerFormat.Lowercase
                || answerFormat == AnswerFormat.URL);

            var defaultValue = _defaultTextTxt.Text;
            var minValue = _minValueNumberTxt.Text;
            var maxValue = _maxValueNumberTxt.Text;

            //Check if is piped value, this causes bypass of most validation
            var isPipedValue = defaultValue.Contains(ApplicationManager.AppSettings.PipePrefix);

            if (lengthValidationRequired)
            {
                //Validate max length value
                var maxLengthTxt = _maxLengthTxt.Text;
                var maxLengthInt = Utilities.AsInt(maxLengthTxt);

                if(maxLengthTxt.Length > 0 && !maxLengthInt.HasValue)
                {
                    _maxLengthError.Text = WebTextManager.GetText("/controlText/singleLineTextEditor/maxLengthValueError");
                    _maxLengthError.Visible = true;
                    
                    return false;
                }

                //Validate length of default text
                if(maxLengthInt.HasValue && defaultValue.Length > maxLengthInt.Value && !isPipedValue)
                {
                    _defaultValueError.Text = WebTextManager.GetText("/controlText/singleLineTextEditor/defaultValueLengthError");
                    _defaultValueError.Visible = true;
                    
                    return false;
                }
            }

            //Otherwise, get busy with validation
            var minMaxValidationRequired = EnableMinMax(answerFormat);

            //Todo: Handle custom formats
            List<Validator<string>> validatorList = TextAnswerValidator.GetFormatValidatorList(answerFormat, string.Empty);

            return
                ValidateValue(validatorList, defaultValue, _defaultValueError)
                && (!minMaxValidationRequired || ValidateValue(validatorList, minValue, _minValueError))
                && (!minMaxValidationRequired || ValidateValue(validatorList, maxValue, _maxValueError));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerFormat"></param>
        /// <returns></returns>
        private bool EnableMinMax(AnswerFormat answerFormat)
        {
            //Otherwise, get busy with validation
            return 
                answerFormat == AnswerFormat.Date
                || answerFormat == AnswerFormat.Date_ROTW
                || answerFormat == AnswerFormat.Date_USA
                || answerFormat == AnswerFormat.Decimal
                || answerFormat == AnswerFormat.Integer
                || answerFormat == AnswerFormat.Money
                || answerFormat == AnswerFormat.Numeric;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="validators"></param>
        /// <param name="theValue"></param>
        /// <param name="errorLbl"></param>
        private bool ValidateValue(List<Validator<string>> validators, string theValue, Label errorLbl)
        {
            string validationError = string.Empty;

            //No validators = valid
            if(validators.Count == 0 || string.IsNullOrEmpty(theValue) || theValue.Contains(ApplicationManager.AppSettings.PipePrefix))
            {
                return true;
            }

            foreach (var validator in validators)
            {
                if(validator.Validate(theValue))
                {
                    return true;
                }

                validationError = validator.GetMessage(WebTextManager.GetUserLanguage());
            }

            //If we get here, not valid
            errorLbl.Text = validationError;
            errorLbl.Visible = true;

            return false;
        }

        /// <summary>
        /// Update data with user inputs.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        public void UpdateData(TextItemDecorator textItemDecorator)
        {
            if (!(textItemDecorator.Data is SingleLineTextItemData))
            {
                return;
            }

            textItemDecorator.Data.Alias = _aliasText.Text;

            //Set format and text, which are used in all formats
            textItemDecorator.Data.Format = (AnswerFormat)Enum.Parse(typeof(AnswerFormat), _textFormatList.SelectedValue);
            textItemDecorator.DefaultText = _defaultTextTxt.Text;
            textItemDecorator.Data.IsRequired = _requiredChk.Checked;
            textItemDecorator.Data.MaxLength = Utilities.AsInt(_maxLengthTxt.Text);

            bool isNumeric = GetIsFormatNumeric(_textFormatList.SelectedValue);

            if (IsDate)
            {
                CultureInfo culture = GetDateCulture(_textFormatList.SelectedValue);

                //Set length/max/min values
                textItemDecorator.Data.MaxLength = null;

                ((SingleLineTextItemData)textItemDecorator.Data).MinValue =
                    string.IsNullOrEmpty(_minValueNumberTxt.Text) ? null :
                    (double?)(DateTime.Parse(_minValueNumberTxt.Text, culture).Ticks);

                ((SingleLineTextItemData)textItemDecorator.Data).MaxValue = 
                    string.IsNullOrEmpty(_maxValueNumberTxt.Text) ? null :
                    (double?)(DateTime.Parse(_maxValueNumberTxt.Text, culture).Ticks);
            }

            if (isNumeric)
            {
                //Set length/max/min values
                textItemDecorator.Data.MaxLength = null;

                ((SingleLineTextItemData)textItemDecorator.Data).MinValue = Utilities.AsDouble(_minValueNumberTxt.Text);
                ((SingleLineTextItemData)textItemDecorator.Data).MaxValue = Utilities.AsDouble(_maxValueNumberTxt.Text);
            }
        }
    }
}
