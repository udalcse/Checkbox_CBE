using System;
using System.Data;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Globalization.Text;

using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Decorator for select items
    /// </summary>
    [Serializable]
    public class SelectItemTextDecorator : LabelledItemTextDecorator
    {
        private string _otherText;
        private string _noneOfAboveText;
        bool _otherTextModified;
        bool _noneOfAboveTextModified;

        private readonly Dictionary<int, string> _optionTexts;

        /// <summary>
        /// Get/set whether texts have been batch loaded for the item
        /// </summary>
        protected bool TextsLoaded { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public SelectItemTextDecorator(SelectItemData data, string language)
            : base(data, language)
        {
            _otherText = string.Empty;
            _noneOfAboveText = string.Empty;
            _otherTextModified = false;
            _noneOfAboveTextModified = false;
            _optionTexts = new Dictionary<int, string>();
            TextsLoaded = false;
        }

        /// <summary>
        /// Get properly casted item data
        /// </summary>
        new public SelectItemData Data
        {
            get
            {
                return (SelectItemData)base.Data;
            }
        }

        /// <summary>
        /// Set localized texts for this item
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            base.SetLocalizedTexts();

            if (Data.OtherTextID != string.Empty)
            {
                SetText(Data.OtherTextID, OtherText);
            }

            if (Data.NoneOfAboveTextID != string.Empty)
            {
                SetText(Data.NoneOfAboveTextID, NoneOfAboveText);
            }

            foreach (ListOptionData option in Data.Options)
            {
                if (option.TextID != string.Empty)
                {
                    if (_optionTexts.ContainsKey(option.Position))
                    {
                        SetText(option.TextID, _optionTexts[option.Position]);
                    }
                }
            }
        }

        /// <summary>
        /// Copy localized text to another item
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            base.CopyLocalizedText(data);

            ArgumentValidation.CheckExpectedType(data, typeof(SelectItemData));

            if (Data.OtherTextID != string.Empty)
            {
                Dictionary<string, string> otherTexts = GetAllTexts(Data.OtherTextID);
                if (otherTexts != null)
                {
                    foreach (string key in otherTexts.Keys)
                    {
                        SetText(((SelectItemData)data).OtherTextID, otherTexts[key], key);
                    }
                }
            }

            //Set the list options
            for (int i = 0; i < Data.Options.Count; i++)
            {
                ListOptionData option = ((SelectItemData)data).Options[i];

                if (option.TextID != string.Empty)
                {
                    if (((SelectItemData)data).Options.Count > i)
                    {
                        string copyFromTextID = Data.Options[i].TextID;

                        Dictionary<string, string> optionTexts = GetAllTexts(copyFromTextID);

                        if (optionTexts != null)
                        {
                            foreach (string key in optionTexts.Keys)
                            {
                                SetText(option.TextID, optionTexts[key], key);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get/set other text for the item
        /// </summary>
        public string OtherText
        {
            get
            {
                if (Data.OtherTextID != string.Empty && !_otherTextModified)
                {
                    return GetText(Data.OtherTextID);
                }

                return _otherText;
            }

            set
            {
                _otherText = value;
                _otherTextModified = true;
            }
        }

        /// <summary>
        /// Get/set none of above text for the item
        /// </summary>
        public string NoneOfAboveText
        {
            get
            {
                if (Data.NoneOfAboveTextID != string.Empty && !_noneOfAboveTextModified)
                {
                    return GetText(Data.NoneOfAboveTextID);
                }

                return _noneOfAboveText;
            }

            set
            {
                _noneOfAboveText = value;
                _noneOfAboveTextModified = true;
            }
        }

        /// <summary>
        /// Set the text of the option with the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void SetOptionText(int position, string text)
        {
            _optionTexts[position] = text;
        }

        /// <summary>
        /// Get the text of the option with the 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public string GetOptionText(int position)
        {
            //Get the modified text, if any
            if (_optionTexts.ContainsKey(position))
            {
                return _optionTexts[position];
            }

            foreach (ListOptionData option in Data.Options)
            {
                if (option.Position == position && option.TextID != string.Empty)
                {
                    if (option.IsOther)
                    {
                        //Attempt to get the other text
                        if (Utilities.IsNotNullOrEmpty(OtherText))
                            return OtherText;

                        if (Utilities.IsNotNullOrEmpty(option.Alias))
                            return option.Alias;
                    }

                    if (option.IsNoneOfAbove)
                    {
                        //Attempt to get the other text
                        if (Utilities.IsNotNullOrEmpty(NoneOfAboveText))
                            return NoneOfAboveText;

                        if (Utilities.IsNotNullOrEmpty(option.Alias))
                            return option.Alias;
                    }

                    return GetText(option.TextID);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Load option texts
        /// </summary>
        protected override void EnsureTextsLoaded()
        {
            //Do nothing for unsaved item
            if (!Data.ID.HasValue || TextsLoaded)
            {
                TextsLoaded = true;
                return;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_GetText");
            command.AddInParameter("ItemId", DbType.Int32, Data.ID.Value);
            command.AddInParameter("LanguageCode", DbType.String, Language);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Text
                    if (!TextModified)
                    {
                        string text = reader.Read()
                                   ? DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty)
                                   : string.Empty;

                        if (string.IsNullOrEmpty(text))
                        {
                            text = GetText(Data.TextID);
                        }

                        Text = text;
                    }

                    //Description
                    if (!SubTextModified)
                    {
                        string subText = reader.NextResult() && reader.Read()
                                   ? DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty)
                                   : string.Empty;

                        if (string.IsNullOrEmpty(subText))
                        {
                            subText = GetText(Data.SubTextID);
                        }

                        SubText = subText;
                    }

                    //Options
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            int position = DbUtility.GetValueFromDataReader(reader, "Position", -1);
                            string text = DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty);
                            string textId = DbUtility.GetValueFromDataReader(reader, "TextId", string.Empty);

                            if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(textId))
                            {
                                text = GetText(textId);
                            }

                            if (position > 0 && !_optionTexts.ContainsKey(position))
                            {
                                SetOptionText(position, text);
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            TextsLoaded = true;
        }
    }
}
