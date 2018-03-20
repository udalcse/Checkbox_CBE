using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Select item that allows multiple selections
    /// </summary>
    [Serializable]
    public class SelectMany : ScoredItem
    {
        /// <summary>
        /// Get the minimum number of items that have to be selected
        /// </summary>
        public int? MinToSelect { get; private set; }

        /// <summary>
        /// Get the maximum number of items that have to be selected
        /// </summary>
        public int? MaxToSelect { get; private set; }

        /// <summary>
        /// Get/set if 'none of above' option is 
        /// </summary>
        public bool AllowNoneOfAbove { get; private set; }

        /// <summary>
        /// Override required to return true if min to select > 0
        /// </summary>
        public override bool Required
        {
            get { return MinToSelect > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(SelectManyData));
            SelectManyData config = (SelectManyData)configuration;

            base.Configure(configuration, languageCode, templateId);

            AllowNoneOfAbove = config.AllowNoneOfAbove;
            MinToSelect = config.MinToSelect;
            MaxToSelect = (config.MaxToSelect.HasValue && 
                (!MinToSelect.HasValue || config.MaxToSelect >= MinToSelect)) ? config.MaxToSelect : null;
        }

        /// <summary>
        /// Validate selected answers
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            SelectManyValidator validator = new SelectManyValidator();

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
            NameValueCollection values =  base.GetMetaDataValuesForSerialization();

            values["minToSelect"] = MinToSelect.HasValue ? MinToSelect.ToString() : null;
            values["maxToSelect"] = MaxToSelect.HasValue ? MaxToSelect.ToString() : null;

            return values;
        }

        /// <summary>
        /// Override select logic to handle case where default selected checkbox is unselected
        /// by the user and no other options are checked.  This is causes default selected values
        /// to be reapplied.  This isn't an issue for other select items since it is not possible
        /// to not have a selected value in those cases.
        /// </summary>
        /// <param name="otherText"></param>
        /// <param name="optionIDs"></param>
        /// <param name="forceAdding"> Add selected option even if it doesn't exists in options list </param>
        public override void Select(string otherText, IEnumerable<int> optionIDs, bool forceAdding = false)
        {
            //Call base to store selected answers
            base.Select(otherText, optionIDs);

            //If no options selected, record an answer for the item, but with no option id as a 
            // placeholder
            if (!optionIDs.Any())
            {
                AddPlaceHolderAnswer();
            }
            else
            {
                RemovePlaceHolderAnswer();
            }

            //Re-sync. the options with the answer data
            SynchronizeSelectedOptions();
        }

       
    }
}