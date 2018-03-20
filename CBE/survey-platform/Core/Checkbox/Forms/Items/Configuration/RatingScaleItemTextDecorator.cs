using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for localizing rating scale item texts
    /// </summary>
    [Serializable]
    public class RatingScaleItemTextDecorator : SelectItemTextDecorator
    {
        private string _startText;
        private string _midText;
        private string _endText;
        private string _notApplicableText;

        private bool _startTextModified;
        private bool _midTextModified;
        private bool _endTextModified;
        private bool _notApplicableTextModified;

        /// <summary>
        /// Constructor
        /// </summary>
        public RatingScaleItemTextDecorator(RatingScaleItemData itemData, string language)
            : base(itemData, language)
        {
            _startText = string.Empty;
            _midText = string.Empty;
            _endText = string.Empty;
            _notApplicableText = string.Empty;

            _startTextModified = false;
            _midTextModified = false;
            _endTextModified = false;
            _notApplicableTextModified = false;
        }

        /// <summary>
        /// Get a properly cast instance of item data
        /// </summary>
        new public RatingScaleItemData Data
        {
            get { return (RatingScaleItemData)base.Data; }
        }

        /// <summary>
        /// Set localized texts
        /// </summary>
        protected override void SetLocalizedTexts()
        {
            if (_startTextModified)
            {
                SetText(Data.StartTextID, _startText);
            }

            if (_midTextModified)
            {
                SetText(Data.MidTextID, _midText);
            }

            if (_endTextModified)
            {
                SetText(Data.EndTextID, _endText);
            }

            if (_notApplicableTextModified)
            {
                SetText(Data.NotApplicableTextID, _notApplicableText);
            }

            PopulateOptionTexts();
          
            base.SetLocalizedTexts();
        }

        /// <summary>
        /// Populate rating scale options with text values
        /// </summary>
        public void PopulateOptionTexts()
        {
            //Set the option texts, which will be the same as the points for
            // a scale.
            ReadOnlyCollection<ListOptionData> options = Data.Options;

            foreach (ListOptionData option in options)
            {
                if (option.IsOther)
                {
                    SetOptionText(option.Position, GetText(Data.NotApplicableTextID));
                }
                else
                {
                    SetOptionText(option.Position, option.Points.ToString());
                }
            }

        }

        /// <summary>
        /// Get text in all languages for the specified text id. If textID is null, return null.
        /// </summary>
        /// <param name="textID"></param>
        /// <returns></returns>
        public override Dictionary<string, string> GetAllTexts(string textID)
        {
            return String.IsNullOrEmpty(textID)? null : base.GetAllTexts(textID);
        }

        /// <summary>
        /// Copy localized texts
        /// </summary>
        /// <param name="data"></param>
        protected override void CopyLocalizedText(ItemData data)
        {
            //Copy Question & SubText
            base.CopyLocalizedText(data);
            
            //Start Text
            if (Data.StartTextID != string.Empty)
            {
                Dictionary<string, string> textIDTexts = GetAllTexts(((RatingScaleItemData)data).StartTextID);

                if (textIDTexts != null)
                {
                    foreach (string key in textIDTexts.Keys)
                    {
                        SetText(Data.StartTextID, textIDTexts[key], key);
                    }
                }
            }

            //Copy mid text
            if (Data.MidTextID != string.Empty)
            {
                Dictionary<string, string> textIDTexts = GetAllTexts(((RatingScaleItemData)data).MidTextID);

                if (textIDTexts != null)
                {
                    foreach (string key in textIDTexts.Keys)
                    {
                        SetText(Data.MidTextID, textIDTexts[key], key);
                    }
                }
            }

            //Copy End Text
            if (Data.EndTextID != string.Empty)
            {
                Dictionary<string, string> textIDTexts = GetAllTexts(((RatingScaleItemData)data).EndTextID);

                if (textIDTexts != null)
                {
                    foreach (string key in textIDTexts.Keys)
                    {
                        SetText(Data.EndTextID, textIDTexts[key], key);
                    }
                }
            }            
            
            //Copy N/A Text
            if (Data.EndTextID != string.Empty)
            {
                Dictionary<string, string> textIDTexts = GetAllTexts(((RatingScaleItemData)data).NotApplicableTextID);

                if (textIDTexts != null)
                {
                    foreach (string key in textIDTexts.Keys)
                    {
                        SetText(Data.NotApplicableTextID, textIDTexts[key], key);
                    }
                }
            }
        }

        /// <summary>
        /// Get/set the scale start textfs
        /// </summary>
        public string StartText
        {
            get
            {
                if (Data.StartTextID != string.Empty && !_startTextModified)
                {
                    return GetText(Data.StartTextID);
                }
                
                return _startText;
            }
            set
            {
                _startText = value;
                _startTextModified = true;
            }
        }

        /// <summary>
        /// Get/set the scale mid text
        /// </summary>
        public string MidText
        {
            get
            {
                if (Data.MidTextID != string.Empty && !_midTextModified)
                {
                    return GetText(Data.MidTextID);
                }
                
                return _midText;
            }
            set
            {
                _midText = value;
                _midTextModified = true;
            }

        }

        /// <summary>
        /// Get/set the scale end text
        /// </summary>
        public string EndText
        {
            get
            {
                if (Data.EndTextID != string.Empty && !_endTextModified)
                {
                    return GetText(Data.EndTextID);
                }
                
                return _endText;
            }
            set
            {
                _endText = value;
                _endTextModified = true;
            }
        }        
        
        /// <summary>
        /// Get/set the scale end text
        /// </summary>
        public string NotApplicableText
        {
            get
            {
                if (Data.NotApplicableTextID != string.Empty && !_notApplicableTextModified)
                {
                    return GetText(Data.NotApplicableTextID);
                }
                
                return _notApplicableText;
            }
            set
            {
                _notApplicableText = value;
                _notApplicableTextModified = true;
            }
        }
    }
}
