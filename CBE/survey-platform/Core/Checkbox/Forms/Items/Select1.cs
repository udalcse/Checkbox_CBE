//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Input item that allows selection of one of a number of options.
    /// </summary>
    [Serializable]
    public class Select1 : ScoredItem
    {
        /// <summary>
        /// Create options, but verify that only one is selected
        /// </summary>
        /// <returns></returns>
        protected override void CreateOptions(ReadOnlyCollection<ListOptionData> metaOptions)
        {
            base.CreateOptions(metaOptions);

            EnsureSingleOptionSelected();
        }

        /// <summary>
        /// Synchronize options, but verify that only one is selected
        /// </summary>
        /// <returns></returns>
        protected override void SynchronizeSelectedOptions()
        {
            base.SynchronizeSelectedOptions();

            EnsureSingleOptionSelected();
        }

        /// <summary>
        /// Ensure that only one option is selected
        /// </summary>
        protected virtual void EnsureSingleOptionSelected()
        {
            bool optionSelected = false;

            foreach (ListOption option in OptionsDictionary.Values)
            {
                //De select option if another option is selected
                option.IsSelected = option.IsSelected && !optionSelected;

                //Set flag if option is selected
                optionSelected = optionSelected || option.IsSelected;
            }
        }

        /// <summary>
        /// Select the answer option
        /// </summary>
        /// <param name="otherText"></param>
        /// <param name="optionIDs"></param>
        /// <param name="forceAdding"> Add selected option even if it doesn't exists in options list </param>
        public override void Select(string otherText, IEnumerable<int> optionIDs, bool forceAdding = false)
        {
            if (optionIDs.Count() > 1)
            {
                throw new Exception("Too many options were selected.");
            }

            base.Select(otherText, optionIDs, forceAdding);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConvertToRadioButtons()
        {
            ItemTypeName = "RadioButtons";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            if (ExportMode == ExportMode.Pdf && ItemTypeName == "DropdownList")
                ConvertToRadioButtons();
        }

        protected override void CheckForEmptyAnswer(SurveyResponseItemAnswer[] postedAnswers)
        {
            //dropdown first option "Select:" should be resolved as empty
            if (ItemTypeName == "DropdownList" && postedAnswers.Length == 1 && postedAnswers[0].OptionId == 0)
                AnswerData.SetEmptyAnswerForItem(ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override double GetPossibleMaxScore()
        {
            return Options.Any() ? Options.Max(o => o.Points) : 0;
        }
    }
}