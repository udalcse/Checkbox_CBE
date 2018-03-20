//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Checkbox.Forms.Validation;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Users;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Abstract base of an item that can present a text item
    /// </summary>
    [Serializable]
    public abstract class TextItem : LabelledItem
    {
        private string _defaultText;

        /// <summary>
        /// Get is Html editor for this item
        /// </summary>
        public bool IsHtmlFormattedData { get; protected set; }

        /// <summary>
        /// Get the default text for this item
        /// </summary>
        public string DefaultText
        {
            get { return GetPipedText("DefaultText", _defaultText);}
            set { _defaultText = value; }
        }

        /// <summary>
        /// Get the answer format for this item
        /// </summary>
        public AnswerFormat Format { get; protected set; }

        /// <summary>
        /// Get the unique identifier for a custom format
        /// </summary>
        public string CustomFormatId { get; private set; }

        /// <summary>
        /// Get the maximum length of an answer to this item.
        /// </summary>
        public int? MaxLength { get; protected set; }

        /// <summary>
        /// Get the minimum length of an answer to this item.
        /// </summary>
        public int? MinLength { get; protected set; }

        /// <summary>
        /// Configure this item with the supplied configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(TextItemData));
            var config = (TextItemData)configuration;

            _defaultText = GetText(config.DefaultTextID);

            IsHtmlFormattedData = config.IsHtmlFormattedData;
            Format = config.Format;
            CustomFormatId = config.CustomFormatId;
            MaxLength = config.MaxLength;
            MinLength = config.MinLength;
        }

        #region IAnswerable Members

        /// <summary>
        /// Get the typed answer for this item
        /// </summary>
        /// <returns></returns>
        public override string GetAnswer()
        {
            //If no response or answer data, return the default value
            if (HasAnswer)
            {
                return base.GetAnswer();
            }
            
            return DefaultText;
        }

        #endregion

        /// <summary>
        /// Return a boolean indicating if answers are valid.  If answers are not valid, appropriate
        /// base class validation errors will be set
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var validator = new TextAnswerValidator();

            if (!validator.Validate(this))
            {
                ValidationErrors.Add(validator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get meta data values
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["isHtmlFormattedData"] = IsHtmlFormattedData.ToString();
            values["answerFormat"] = Format.ToString();
            values["maxLength"] = MaxLength.HasValue ? MaxLength.ToString() : null;
            values["minLength"] = MinLength.HasValue ? MinLength.ToString() : null;
            values["defaultText"] = DefaultText;

            var connectedFieldName = ProfilePropertiesUpdater.GetConnectedProfileFieldName(ID);

            if (!string.IsNullOrWhiteSpace(connectedFieldName))
                values["ConnectedCustomFieldKey"] = connectedFieldName;

            return values;
        }
    }
}
