using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Validation;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Input item that allows selection of row(s) in matrix item.
    /// </summary>
    [Serializable]
    public class RowSelect : ScoredItem
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
        /// Determine if this row selector allows multiple selection or not.
        /// </summary>
        public bool AllowMultipleSelection { get; set; }

        /// <summary>
        /// Configure the item with the supplied configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(RowSelectData));
            RowSelectData config = (RowSelectData)configuration;
            MinToSelect = config.MinToSelect;
            MaxToSelect = config.MaxToSelect;
            AllowMultipleSelection = config.AllowMultipleSelection;

            base.Configure(configuration, languageCode, templateId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaOptions"></param>
        protected override void CreateOptions(ReadOnlyCollection<ListOptionData> metaOptions)
        {
            base.CreateOptions(metaOptions);

            if (!AllowMultipleSelection)
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
        /// Synchronize options, but verify that only one is selected
        /// </summary>
        /// <returns></returns>
        protected override void SynchronizeSelectedOptions()
        {
            base.SynchronizeSelectedOptions();

            if (!AllowMultipleSelection)
                EnsureSingleOptionSelected();
        }

        /// <summary>
        /// Get instance data for serialization
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            var values = base.GetInstanceDataValuesForSerialization();

            values["allowMultipleSelection"] = AllowMultipleSelection.ToString();

            return values;
        }

        /// <summary>
        /// Returns true. All validation actions will be performed in <see cref="MatrixItemValidator" />
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            return true;
        }

        /// <summary>
        /// Override to show "X" on selected row
        /// </summary>
        /// <param name="option"></param>
        /// <param name="writer"></param>
        protected override void WriteOptionAnswer(ListOption option, XmlWriter writer)
        {
            writer.WriteStartElement("answer");
            writer.WriteAttributeString("optionId", option.ID.ToString());

            writer.WriteCData("X");

            writer.WriteEndElement();
        }
    }
}