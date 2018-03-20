using System;
using System.Collections.Generic;
using System.Globalization;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class SingleLineText : UserControlSurveyItemRendererBase
    {
        protected string[] AutocompleteData { set; get; }
        protected string AutocompleteRemote { get; set; }

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
        protected void SetInputProperties()
        {
            string inputType = "text";

            switch (GetAnswerFormat())
            {
                //Date Formats
                case AnswerFormat.Date:
                case AnswerFormat.Date_ROTW:
                case AnswerFormat.Date_USA:
                    inputType = "date";
                    break;

                //Masked formats
                case AnswerFormat.Phone:
                    inputType = "tel";
                    break;

                //Masked formats
                case AnswerFormat.URL:
                    inputType = "url";
                    break;

                //Money formats
                case AnswerFormat.Money:
                //Numeric formats
                case AnswerFormat.SSN:
                case AnswerFormat.Integer:
                case AnswerFormat.Decimal:
                case AnswerFormat.Numeric:
                    inputType = "number";
                    break;

                //do nothing for postal zip codes as there are different alpha-numeric formats
                case AnswerFormat.Postal:
                    break;

                //Text formats with JS input limitation in addition to server
                // side validation.
                case AnswerFormat.Alpha:
                case AnswerFormat.AlphaNumeric:
                case AnswerFormat.Uppercase:
                case AnswerFormat.Lowercase:
                    SetMaxLength();
                    break;

                //Other formats have only server-side validation & only need
                // answer values set and length restricted.
                default:
                    SetMaxLength();
                    break;
            }

            _textInput.Attributes["type"] = inputType;
        }

        /// <summary>
        /// Set accepted value for inputs, based on format
        /// </summary>
        protected void SetInputValue()
        {
            switch (GetAnswerFormat())
            {
                //Date Formats
                case AnswerFormat.Date:
                case AnswerFormat.Date_ROTW:
                case AnswerFormat.Date_USA:
                    SetDateValues();
                    break;

                //Numeric formats
                case AnswerFormat.Integer:
                case AnswerFormat.Decimal:
                case AnswerFormat.Numeric:
                    SetNumericValues();
                    break;

                default:
                    if (Model.Answers.Length > 0)
                    {
                        _textInput.Value = Model.Answers[0].AnswerText;
                    }
                    else
                    {
                        _textInput.Value = Model.Metadata["DefaultText"] ?? string.Empty;
                    }
                    break;
            }
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

            SetInputValue();

          //  SetAutocomplete();

            //set the default text
            _textInput.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                _textInput.Attributes.Add("binded-field", "true");
        }

        protected void SetAutocomplete()
        {
            var autocompleteListId = Utilities.AsInt(Model.Metadata["AutocompleteListId"]);
            if (autocompleteListId.HasValue)
            {
                AutocompleteData = AutocompleteListManager.ListItems(autocompleteListId.Value);
                _textInput.Attributes["autocomplete"] = "off";
            }
            else if (ApplicationManager.AppSettings.AllowAutocompleteRemoteSource)
            {
                AutocompleteRemote = Model.Metadata["AutocompleteRemote"];
                _textInput.Attributes["autocomplete"] = "off";
            }
        }

        /// <summary>
        /// Set min/max and answer date values
        /// </summary> 
        protected void SetDateValues()
        {
            DateTime? answerValueDate = Utilities.GetDate(Model.InstanceData["AnswerValueAsDate"], CultureInfo.InvariantCulture);
            
            //The HTML5 date input specification [1] refers to the RFC3339 specification [2], which specifies a full-date format equal to: yyyy-mm-dd.
            //1. http://dev.w3.org/html5/markup/input.date.html
            //2. http://tools.ietf.org/html/rfc3339
            
            const string dateFormat = "yyyy-MM-dd";

            if (answerValueDate.HasValue)
            {
                _textInput.Value = answerValueDate.Value.ToString(dateFormat, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Set max length of text
        /// </summary>
        protected void SetMaxLength()
        {
            int? maxLength = Utilities.AsInt(Model.Metadata["MaxLength"]);

            if (maxLength.HasValue)
            {
                _textInput.Attributes["maxlength"] = maxLength.Value.ToString();
            }
        }


        /// <summary>
        /// Update model
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            var answerFormat = GetAnswerFormat();
            string answer = (Request[_textInput.UniqueID] ?? _textInput.Value).Trim();

            bool isDate = answerFormat == AnswerFormat.Date ||
                          answerFormat == AnswerFormat.Date_ROTW ||
                          answerFormat == AnswerFormat.Date_USA;

            DateTime date;
            if (isDate && DateTime.TryParse(answer, out date))
            {
                string format;
                if (answerFormat == AnswerFormat.Date_USA)
                    format = "MM/dd/yyyy";
                else if (answerFormat == AnswerFormat.Date_ROTW)
                    format = "dd/MM/yyyy";
                else
                    format = "d";

                answer = date.ToString(format, CultureInfo.InvariantCulture);
            }

            UpsertTextAnswer(answer);

            UpdateConnectedProfileData(Request[_textInput.UniqueID]);
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

            _textInput.Value = Model.Answers.Length > 0
                ? Model.Answers[0].AnswerText
                : Model.InstanceData["DefaultText"];
        }



    }
}