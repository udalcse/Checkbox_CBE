using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Web;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class OptionsQuickEntry : Checkbox.Web.Common.UserControlBase
    {
        ///<summary>
        /// Get/set whether to include points
        ///</summary>
        public virtual bool ShowPointsColumn { get; set; }

        /// <summary>
        /// Determine if columns should be categorized
        /// </summary>
        private bool AreColumnsCategorized { get; set; }


        //Get/set options
        private List<ListOptionData> Options
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_OptionsInQuickEntry"] == null)
                {
                    HttpContext.Current.Session[ID + "_OptionsInQuickEntry"] = new List<ListOptionData>();
                }

                return (List<ListOptionData>)HttpContext.Current.Session[ID + "_OptionsInQuickEntry"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_OptionsInQuickEntry"] = value;
            }
        }


        //Get/set option texts
        private Dictionary<int, string> OptionTexts
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_OptionTextsInQuickEntry"] == null)
                {
                    HttpContext.Current.Session[ID + "_OptionTextsInQuickEntry"] = new Dictionary<int, string>();
                }

                return (Dictionary<int, string>)HttpContext.Current.Session[ID + "_OptionTextsInQuickEntry"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_OptionTextsInQuickEntry"] = value;
            }
        }

        private bool AllowMultiDefaultSelect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OptionLinesCount
        {
            get { return _inputTextBox.Text.Split(new [] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).Length ; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                if (AreColumnsCategorized)
                {
                    _formatLabel.Text =
                        WebTextManager.GetText(ShowPointsColumn
                                                   ? "/controlText/quickEntry/entryFieldCategoryPoints"
                                                   : "/controlText/quickEntry/entryFieldCategoryNoPoints");
                }
                else
                {
                    _formatLabel.Text =
                        WebTextManager.GetText(ShowPointsColumn
                                                   ? "/controlText/quickEntry/entryFieldPoints"
                                                   : "/controlText/quickEntry/entryFieldNoPoints");
                }
            }
        }


        /// <summary>
        /// Initialize the control.
        /// </summary>
        public void Initialize(List<ListOptionData> listOptions, Dictionary<int, string> optionTexts, bool allowMultiDefaultSelect, bool isPagePostBack, bool enableScoring, bool areOptionsCategorized)
        {
            ShowPointsColumn = enableScoring;
            AllowMultiDefaultSelect = allowMultiDefaultSelect;
            AreColumnsCategorized = areOptionsCategorized;

            if (!isPagePostBack)
            {
                Options = listOptions;
                OptionTexts = optionTexts;
            }

            DataBindText();
        }


        /// <summary>
        /// Bind data
        /// </summary>
        public void DataBindText()
        {
            _inputTextBox.Text = String.Empty;
            foreach (ListOptionData listOptionData in Options)
            {
                if (!listOptionData.IsOther && !listOptionData.IsNoneOfAbove)
                    AddOption(listOptionData, OptionTexts.ContainsKey(listOptionData.OptionID) ? OptionTexts[listOptionData.OptionID] : string.Empty);
            }

            // Delete the last NewLine element
            if (!String.IsNullOrEmpty(_inputTextBox.Text))
                _inputTextBox.Text = _inputTextBox.Text.Remove(_inputTextBox.Text.Length - Environment.NewLine.Length);
        }

        /// <summary>
        /// Add option to the TextBox
        /// </summary>
        /// <param name="listOptionData"></param>
        /// <param name="text"></param>
        private void AddOption(ListOptionData listOptionData, String text)
        {
            _inputTextBox.Text += text + ",";
            _inputTextBox.Text += listOptionData.IsDefault ? "Yes," : "No,";
            _inputTextBox.Text += listOptionData.Alias;

            if (AreColumnsCategorized)
            {
                _inputTextBox.Text += "," + listOptionData.Category;
            }

            if (ShowPointsColumn)
            {
                _inputTextBox.Text += "," + listOptionData.Points.ToString("r");
            }

            _inputTextBox.Text += Environment.NewLine;
        }



        /// <summary>
        /// Defines if the string can be converted to double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsDouble(String str)
        {
            try
            {
                if (!String.IsNullOrEmpty(str))
                    Convert.ToDouble(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }



        /// <summary>
        /// Defines if the string can be converted to the ListOptionData
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        private bool ValidateOption(String option)
        {
            String[] columns = option.Split(new char[] { ',' });

            if (ShowPointsColumn)
            {
                if (AreColumnsCategorized)
                    return columns.Length < 5 || IsDouble(columns[4]);
                else
                    return columns.Length < 4 || IsDouble(columns[3]);
            }

            return true;
        }



        /// <summary>
        /// Defines if the text can be converted to the list of ListOptionData
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            _inputTextBox.Text = _inputTextBox.Text.Trim();

            //delete the CRLF at the end, if it exists.
            if (_inputTextBox.Text.Length >= Environment.NewLine.Length &&
                _inputTextBox.Text.Substring(_inputTextBox.Text.Length - Environment.NewLine.Length) == Environment.NewLine)
                _inputTextBox.Text = _inputTextBox.Text.Remove(_inputTextBox.Text.Length - Environment.NewLine.Length);

            String[] lines = _inputTextBox.Text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool result = true;
            foreach (string line in lines)
            {
                result &= ValidateOption(line);
            }


            //show error
            _errorPanel.Visible = !result;
            _errorLable.Visible = !result;
            return result;
        }


        /// <summary>
        /// Update option and option text
        /// </summary>
        /// <param name="option"></param>
        /// <param name="optionText"></param>
        /// <param name="newPosition"></param>
        private void UpdateOptionAndOptionText(ListOptionData option, String optionText, int newPosition)
        {
            String[] columns = optionText.Split(new char[] { ',' });
                        
            string text = columns[0].Trim();
            string isDefault = "0";
            string alias = string.Empty;
                        
            //There are possible situations when ',' sign could be used in the text body. Let's try to catch it
            for (int j = 1; j < columns.Count(); j++) 
            {
                string value = columns[j].ToLower().Trim();
                if (value != "0" && value != "1" &&
                    value != "false" && value != "true" &&
                    value != "no" && value != "yes")
                {
                    text += ',' + columns[j];
                }
                else
                {
                    isDefault = value;
                    for (var k = j + 1; k < (ShowPointsColumn ? columns.Count() - 1: columns.Count()); k++)
                    {
                        alias += columns[k];
                    }
                    break;
                }
            }

            OptionTexts[option.OptionID] = text;
            option.Position = newPosition;
            option.Alias = alias;

            if (columns.Length >= 2 && isDefault == "yes")
            {
                if (!AllowMultiDefaultSelect)
                    SetOptionDefaultToFalse();
                option.IsDefault = true;
            }
            else
            {
                option.IsDefault = false;
            }

            option.Category = AreColumnsCategorized && columns.Length >= 4 ? columns[3].Trim() : String.Empty;

            //Initialize points
            option.Points = 0;

            if (ShowPointsColumn)
            {
                option.Points = ToDouble(columns.Last().Trim());
                /*
                if (AreColumnsCategorized)
                {
                    option.Points = columns.Length >= 5 ? ToDouble(columns[4].Trim()) : 0;
                }
                else
                {
                    option.Points = columns.Length >= 4 ? ToDouble(columns[3].Trim()) : 0;
                }*/
            }
        }


        /// <summary>
        /// Set property "Default" for all Options to false
        /// </summary>
        private void SetOptionDefaultToFalse()
        {
            foreach (ListOptionData listOptionData in Options)
            {
                listOptionData.IsDefault = false;
            }
        }



        /// <summary>
        /// Update data in the textDecorator
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(SelectItemTextDecorator textDecorator)
        {
            String[] lines = _inputTextBox.Text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            int position = 0;

            //Update OptionData
            while (i < lines.Length && position < Options.Count)
            {
                if (lines[i][0] != ',') // if text isn't empty
                {
                    ListOptionData option = Options[position];
                    if (!option.IsOther && !option.IsNoneOfAbove)
                    {
                        if( !textDecorator.Data.OptionsList.ListOptions.Contains(option))
                        {
                            textDecorator.Data.AddOption(option.Alias, option.Category, option.IsDefault,
                                                        option.Position,
                                                        option.Points, option.IsOther, option.IsNoneOfAbove, option.ContentID);
                        }
                        UpdateOptionAndOptionText(option, lines[i], position + 1);
                        textDecorator.Data.UpdateOption(option.OptionID, option.Alias, option.Category, option.IsDefault,
                                                        option.Position,
                                                        option.Points, option.IsOther, option.IsNoneOfAbove, option.ContentID);
                        textDecorator.SetOptionText(option.Position, OptionTexts[option.OptionID]);
                    }
                    else
                    {
                        i--;
                    }
                    position++;

                }
                i++;
            }

            if (i == lines.Length)
            {
                //delete unnecessary options
                while (position < Options.Count)
                {
                    if (!Options[position].IsOther && !Options[position].IsNoneOfAbove)
                    {
                        textDecorator.Data.RemoveOption(Options[position].OptionID);
                        OptionTexts.Remove(Options[position].OptionID);
                        Options.RemoveAt(position);
                        position--;
                    }
                    position++;
                }
            }
            else
            {
                //add new Options
                for (int j = i; j < lines.Length; j++)
                {
                    if (lines[j][0] != ',') // if text isn't empty
                    {
                        String[] columns = lines[j].Split(new char[] { ',' });


                        string category = AreColumnsCategorized && columns.Length >= 4 ? columns[3].Trim() : String.Empty;

                        //Initialize points
                        double points = 0;

                        if (ShowPointsColumn)
                        {
                            if (AreColumnsCategorized)
                            {
                                points = columns.Length >= 5 ? ToDouble(columns[4].Trim()) : 0;
                            }
                            else
                            {
                                points = columns.Length >= 4 ? ToDouble(columns[3].Trim()) : 0;
                            }
                        }

                        ListOptionData newOption = textDecorator.Data.AddOption(
                            columns.Length >= 3 ? columns[2].Trim() : String.Empty,
                            category,
                            columns.Length >= 2 ? columns[1].ToLower().Trim() == "yes" : false,
                            position + 1,
                            points,
                            false,
                            false,
                            null
                            );

                        if (columns.Length >= 2 && columns[1].ToLower().Trim() == "yes")
                        {
                            if (!AllowMultiDefaultSelect)
                                SetOptionDefaultToFalse();
                            newOption.IsDefault = true;
                        }
                        else
                        {
                            newOption.IsDefault = false;
                        }
                        textDecorator.SetOptionText(position + 1, columns[0].Trim());
                        Options.Add(newOption);
                        OptionTexts.Add(newOption.OptionID, columns[0].Trim());
                        position++;

                    }
                }
            }

        }


        /// <summary>
        /// Convert string in right format to double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private double ToDouble(String str)
        {
            if (String.IsNullOrEmpty(str))
                return 0;
            return Convert.ToDouble(str);
        }
    }
}