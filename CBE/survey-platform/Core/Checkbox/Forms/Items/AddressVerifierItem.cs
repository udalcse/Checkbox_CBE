//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;

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
    public class AddressVerifierItem : TextItem
    {
        // for use when format is numeric.

        /// <summary>
        /// Get/set the region where address should be selected from
        /// </summary>
        public string Region { get; private set; }
        /// <summary>
        /// Get/set the serach type
        /// </summary>
        public string SearchType { get; private set; }
        /// <summary>
        /// Get/set the rule
        /// </summary>
        public string Rule { get; private set; }
        /// <summary>
        /// Get/set the rural
        /// </summary>
        public string Rural { get; private set; }

        /// <summary>
        /// Configure the item with the supplied configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            ArgumentValidation.CheckExpectedType(configuration, typeof(AddressVerifierItemData));
            var config = (AddressVerifierItemData)configuration;
            
            Region = config.Region;
            SearchType = config.SearchType;
            Rule = config.Rule;
            Rural = config.Rural;

        }

        /// <summary>
        /// Get the typed answer for this item
        /// </summary>
        /// <returns></returns>
        public override string GetAnswer()
        {
            //If no response or answer data, return the default value
            if (HasAnswer)
            {
                string answer = base.GetAnswer();
                if (answer.Contains("~"))
                    return answer.Substring(0, answer.IndexOf('~'));
                return answer;
            }

            return DefaultText;
        }

        /// <summary>
        /// Returns full answer
        /// </summary>
        /// <returns></returns>
        public override string GetRawAnswer()
        {
            return base.GetAnswer();
        }

        /// <summary>
        /// First, use base validation
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var validator = new AddressVerifierValidator();

            if (!validator.Validate(this))
            {
                ValidationErrors.Add(validator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get meta data values for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();

            values["Region"] = Region;
            values["SearchType"] = SearchType;
            values["Rule"] = Rule;
            values["Rural"] = Rural;

            return values;
        }
    }
}
