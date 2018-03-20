//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Common;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item that allows for 
    /// </summary>
    [Serializable]
    public class SingleLineTextBoxItem : TextItem
    {
        /// <summary>
        /// Get the minimum numeric (if any) value enterable
        /// </summary>
        public double? MinNumericValue { get; private set; }

        /// <summary>
        /// Get the maximum numeric (if any) value enterable
        /// </summary>
        public double? MaxNumericValue { get; private set; }

        /// <summary>
        /// Get/set min value as a date time object.
        /// </summary>
        public DateTime? MinDateValue { get; private set; }

        /// <summary>
        /// Get/set max value as a date.
        /// </summary>
        public DateTime? MaxDateValue { get; private set; }

        /// <summary>
        /// Autocomplete list Id
        /// </summary>
        public int? AutocompleteListId { get; private set; }

        /// <summary>
        /// Autocomplete remote source url
        /// </summary>
        public string AutocompleteRemote { get; private set; }

        /// <summary>
        /// Get/set default value as date.
        /// </summary>
        public DateTime? GetAnswerValueAsDate()
        {         
            //Try to get default value to see if it is a date.  This will be a string so try to parse it
            // in format of answer.
            var cultures = new List<CultureInfo>();

            if (Format == AnswerFormat.Date)
            {
                cultures.Add(CultureInfo.CurrentCulture);
                cultures.Add(Utilities.GetUsCulture());
                cultures.Add(Utilities.GetRotwCulture());
            }

            if (Format == AnswerFormat.Date_USA)
                cultures.Add(Utilities.GetUsCulture());

            if (Format == AnswerFormat.Date_ROTW)
                cultures.Add(Utilities.GetRotwCulture());

            return Utilities.GetDate(
            GetAnswer(),
            cultures.ToArray());
        }

        /// <summary>
        /// Configure the item with the supplied configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(SingleLineTextItemData));
            var config = (SingleLineTextItemData)configuration;
            
            MinNumericValue = config.MinValue;
            MaxNumericValue = config.MaxValue;

            AutocompleteListId = config.AutocompleteListId;
            AutocompleteRemote = config.AutocompleteRemote;

            if (Format == AnswerFormat.Date
             || Format == AnswerFormat.Date_ROTW
             || Format == AnswerFormat.Date_USA)
            {
                //For dates, min/max numeric values are tick counts.
                if (MaxNumericValue.HasValue)
                {
                    MaxDateValue = new DateTime(Convert.ToInt64(MaxNumericValue));
                }

                if(MinNumericValue.HasValue)
                {
                    MinDateValue = new DateTime(Convert.ToInt64(MinNumericValue));
                }
            }
        }

        /// <summary>
        /// First, use base validation
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var validator = new SingleLineTextAnswerValidator();

            if (!validator.Validate(this))
            {
                ValidationErrors.Add(validator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values = base.GetInstanceDataValuesForSerialization();

            var valueAsDate = GetAnswerValueAsDate();
            values["AnswerValueAsDate"] = valueAsDate.HasValue ? valueAsDate.Value.ToString("u") : string.Empty;

            return values;
        }

        /// <summary>
        /// Get meta data values for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["answerFormat"] = Format.ToString();
            values["minNumericValue"] = MinNumericValue.HasValue ? MinNumericValue.ToString() : null;
            values["maxNumericValue"] = MaxNumericValue.HasValue ? MaxNumericValue.ToString() : null;
            values["MinDateValue"] = MinDateValue.HasValue ? MinDateValue.Value.ToString("u") : string.Empty;
            values["MaxDataValue"] = MaxDateValue.HasValue ? MaxDateValue.Value.ToString("u") : string.Empty;
            values["defaultText"] = Utilities.IsHtmlFormattedText(DefaultText) ? Utilities.StripHtml(DefaultText) : DefaultText;
            values["AutocompleteListId"] = AutocompleteListId.HasValue ? AutocompleteListId.ToString() : null;
            values["AutocompleteRemote"] = AutocompleteRemote;

            return values;
        }

        /// <summary>
        /// Write answer(s) to xml
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        protected override void WriteAnswers(XmlWriter writer, bool isText)
        {
            if (HasAnswer)
            {
                writer.WriteStartElement("answer");
                string rawAnswer = GetRawAnswer();
                writer.WriteCData(isText ? rawAnswer : Utilities.AdvancedHtmlEncode(rawAnswer));
                writer.WriteEndElement();
            }
        }

    }
}
