using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// SPSS export item
    /// </summary>
    [Serializable]
    public class SpssExportItem : ExportItem
    {
        private int _questionCount;
        private bool _includeOpenEnded;

        //Map of header (Q1, Q1_5, etc.) values to
        // actual text values.
        Dictionary<string, string> _textMappings;

        //Map of header values to item ids
        Dictionary<string, int> _itemMappings;

        private Dictionary<int, Dictionary<int, string>> _optionTextOverrides;

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            _includeOpenEnded = ((SPSSExportItemData)itemData).IncludeOpenEnded;
            _textMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            _itemMappings = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            UseAliases = false;
        }

        /// <summary>
        /// Get container for overriding option texts for export
        /// </summary>
        protected Dictionary<int, Dictionary<int, string>> OptionTextOverrides
        {
            get { return _optionTextOverrides ?? (_optionTextOverrides = new Dictionary<int, Dictionary<int, string>>()); }
        }

        /// <summary>
        /// Get the text of the option, first looking in text overrides table.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="optionID"></param>
        /// <returns></returns>
        public override string GetOptionText(int itemID, int optionID)
        {
            if (OptionTextOverrides.ContainsKey(itemID)
                && OptionTextOverrides[itemID].ContainsKey(optionID))
            {
                return OptionTextOverrides[itemID][optionID];
            }

            return base.GetOptionText(itemID, optionID);
        }

        /// <summary>
        /// Get/set option text.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"></param>
        /// <param name="optionText"></param>
        protected virtual void SetOptionText(int itemId, int optionId, string optionText)
        {
            if (!OptionTextOverrides.ContainsKey(itemId))
            {
                OptionTextOverrides[itemId] = new Dictionary<int, string>();
            }

            OptionTextOverrides[itemId][optionId] = optionText;
        }

        /// <summary>
        /// Get whether to merge select many items for export.  Always false
        /// </summary>
        protected override bool MergeSelectMany
        {
            get { return false; }
        }

        /// <summary>
        /// Add columns
        /// </summary>
        protected override void AddColumns(string progressKey, string languageCode, int startProgress, int endProgress, Guid? responseId, string userName)
        {
            _questionCount = 0;

            base.AddColumns(progressKey, languageCode, startProgress, endProgress, null, null);
        }

        /// <summary>
        /// Get whether to include open ended items in the export.
        /// </summary>
        protected override bool IncludeOpenEnded
        {
            get { return _includeOpenEnded; }
        }

        /// <summary>
        /// Get whether to include hidden items in SPSS exports.  Hidden items will
        /// be included if open-ended inputs are included.
        /// </summary>
        public override bool IncludeHidden
        {
            get { return _includeOpenEnded; }
        }

        public override bool ExportRankOrderPoints
        {
            get { return false; }
        }

        /// <summary>
        /// Determine the text for a column
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected override string DetermineColumnTextForItem(ItemData itemData, string prefix)
        {
            _questionCount++;

            string columnText = "Q" + _questionCount;

            //Use base method to get actual text
            _textMappings[columnText] = base.DetermineColumnTextForItem(itemData, prefix);
            _itemMappings[columnText] = itemData.ID.Value;

            return columnText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdOverride"></param>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected void AddColumnsForSelectItemOptions(int? itemIdOverride, SelectItemData itemData, string prefix)
        {
            int itemId = itemIdOverride.HasValue ? itemIdOverride.Value : itemData.ID.Value;

            ReadOnlyCollection<ListOptionData> itemOptions = itemData.Options;

            for (int i = 1; i <= itemOptions.Count; i++)
            {
                if (itemData is SelectManyData)
                {
                    string columnName = "Q" + _questionCount + "_" + i;

                    AddColumn(ColumnCount, columnName, itemId, itemOptions[i - 1].OptionID);

                    //Add text for the option
                    _textMappings[columnName] = GetItemText(itemId) + " -- " + GetOptionText(itemId, itemOptions[i - 1].OptionID);
                }

                //Store numeric option value, overwriting stored text value in the process
                SetOptionText(itemId, itemOptions[i - 1].OptionID, i.ToString());
            }
        }

        /// <summary>
        /// Add the options for a select item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected override void AddColumnsForSelectItemOptions(SelectItemData itemData, string prefix, SelectItemData prototype)
        {
            AddColumnsForSelectItemOptions(itemData.ID, prototype ?? itemData, prefix);
        }

        /// <summary>
        /// Get actual text for a column.
        /// </summary>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public string GetActualText(string columnHeader)
        {
            if (_textMappings.ContainsKey(columnHeader))
            {
                return _textMappings[columnHeader];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get id of item associated with column
        /// </summary>
        /// <param name="columnHeader"></param>
        /// <returns></returns>
        public int GetItemId(string columnHeader)
        {
            if (_itemMappings.ContainsKey(columnHeader))
            {
                return _itemMappings[columnHeader];
            }

            return -1;
        }

        /// <summary>
        /// Get the text of an answerf
        /// </summary>
        /// <param name="answerDataObject"></param>
        /// <returns></returns>
        protected override string GetAnswerText(ItemAnswer answerDataObject, int column)
        {
            if (answerDataObject.OptionId == null)
            {
                return base.GetAnswerText(answerDataObject, column);
            }

            return GetOptionText(answerDataObject.ItemId, answerDataObject.OptionId.Value);
        }

        /// <summary>
        /// Only add "upload item" columns when including open-ended results.
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <param name="textOverride"></param>
        protected override void AddColumnsForUploadItem(ItemData itemData, string prefix, string textOverride)
        {
            if (IncludeOpenEnded)
            {
                base.AddColumnsForUploadItem(itemData, prefix, textOverride);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected override void AddColumnsForRankOrder(ItemData itemData, string prefix)
        {
            if (!ItemsForPointsExport.Contains(itemData.ID.Value))
                ItemsForPointsExport.Add(itemData.ID.Value);

            string columnText = DetermineColumnTextForItem(itemData, prefix);
            RankOrderItemData roid = (RankOrderItemData)itemData;

            for (int i = 0; i < roid.Options.Count; i++)
            {
                var o = roid.Options[i];
                string optionText = GetOptionText(itemData.ID.Value, o.OptionID);
                optionText = Utilities.DecodeAndStripHtml(optionText);

                //Add the column
                AddColumn(ColumnCount, string.Format("{0}_{1}", columnText, i + 1), itemData.ID.Value, o.OptionID);
            }
        }


        /* /// <summary>
         /// Add columns for the item
         /// </summary>
         /// <param name="itemData"></param>
         /// <param name="prefix"></param>
         /// <param name="textOverride"></param>
         protected override void AddColumnsForItem(ItemData itemData, string prefix, string textOverride)
         {
             //Only add answer able items
             if (itemData.ItemIsIAnswerable)
             {
                 if (itemData is MatrixItemData)
                 {
                     AddColumnsForMatrixItem((MatrixItemData)itemData, prefix);
                 }
                 else if (itemData is ICompositeItemData)
                 {
                     AddColumnsForCompositeItem((ICompositeItemData)itemData, prefix);
                 }
                 else
                 {
                     if (itemData is SelectItemData)
                     {
                         AddColumnsForSelectItem((SelectItemData)itemData, prefix, textOverride);
                     }
                     else if ((itemData is TextItemData && IncludeOpenEnded) || (itemData is HiddenItemData && IncludeHidden))
                     {
                         //Add the item
                         string columnText;

                         if (textOverride != null && textOverride.Trim() != string.Empty)
                         {
                             columnText = textOverride;
                         }
                         else
                         {
                             columnText = DetermineColumnTextForItem(itemData, prefix);
                         }

                         AddColumn(ColumnCount + 1, columnText, itemData.ID.ToString());
                     }
                 }
             }
         } */
        /*
                /// <summary>
                /// Add columns for a select item
                /// </summary>
                /// <param name="itemData"></param>
                /// <param name="prefix"></param>
                /// <param name="textOverride"></param>
                protected override void AddColumnsForSelectItem(SelectItemData itemData, string prefix, string textOverride)
                {
                    string itemText;

                    if (textOverride == null || textOverride.Trim() == string.Empty)
                    {
                        itemText = DetermineColumnTextForItem(itemData, prefix);
                    }
                    else
                    {
                        itemText = textOverride;
                    }

                    //Add column for the select item
                    if (!(itemData is SelectManyData) || MergeSelectMany)
                    {
                        AddColumn(ColumnCount, itemText, itemData.ID.ToString());
                    }

                    AddColumnsForSelectItemOptions(itemData, itemText + "_");
                }
                */

        /// <summary>
        /// Add columns for a matrix item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected override void AddColumnsForMatrixItem(MatrixItemData itemData, string prefix)
        {
            string matrixText = base.DetermineColumnTextForItem(itemData, prefix);

            var decorator = (MatrixItemTextDecorator)itemData.CreateTextDecorator(LanguageCode);

            var columnPrototypes = new Dictionary<int, ItemData>();

            for (int i = 1; i <= itemData.ColumnCount; i++)
            {
                var columnProtypeId = itemData.GetColumnPrototypeId(i);

                if (columnProtypeId > 0)
                {
                    columnPrototypes[i] = ItemConfigurationManager.GetConfigurationData(columnProtypeId);
                }
            }

            //Loop through the rows
            for (int row = 1; row <= itemData.RowCount; row++)
            {
                //Avoid subheadings
                if (!itemData.IsRowSubheading(row))
                {
                    //Get the row text
                    string rowText = string.Empty;

                    if (UseAliases)
                    {
                        rowText = decorator.Data.GetRowAlias(row);
                    }

                    if (Utilities.IsNullOrEmpty(rowText))
                    {
                        rowText = decorator.GetRowText(row);
                    }

                    if (Utilities.IsNullOrEmpty(rowText))
                    {
                        rowText = "Row" + row;
                    }

                    //Get the columns
                    for (int column = 1; column <= itemData.ColumnCount; column++)
                    {
                        //Determine if there is an answerable item here
                        ItemData childItemData = itemData.GetItemAt(row, column);

                        if (column == itemData.PrimaryKeyColumnIndex)
                        {
                            if (childItemData != null && childItemData.ItemIsIAnswerable)
                            {
                                //If this is a matrix "other" item...add it
                                if (IncludeOpenEnded)
                                {
                                    _questionCount++;

                                    string questionText = "Q" + _questionCount;

                                    //Use base method to get actual text.  If multiple answer columns include column name in
                                    // text, otherwise consider rowtext to be sufficient.
                                    _textMappings[questionText] = matrixText + " -- " + rowText + " -- Other " + row;
                                    _itemMappings[questionText] = itemData.ID.Value;

                                    //Add column
                                    AddColumn(ColumnCount, questionText, itemData.ID.Value, null);
                                }
                            }
                        }
                        else
                        {
                            var columnPrototype = columnPrototypes.ContainsKey(column) ? columnPrototypes[column] : null;

                            //Get the column text
                            string columnText = string.Empty;

                            //If only one data column, don't bother with column text since
                            // rowtext will suffice.
                            if (itemData.ColumnCount == 2)
                            {
                                columnText = matrixText + " -- " + rowText;
                            }
                            else
                            {
                                if (UseAliases && columnPrototype != null)
                                {
                                    columnText = columnPrototype.Alias;
                                }

                                if (columnText == null || columnText.Trim() == string.Empty)
                                {
                                    columnText = decorator.GetColumnText(column);
                                }

                                if (columnText == null || columnText.Trim() == string.Empty)
                                {
                                    columnText = "Column" + column;
                                }

                                columnText = matrixText + " -- " + rowText + " -- " + columnText;
                            }

                            //Add column for the select item
                            if ((childItemData is TextItemData && IncludeOpenEnded))
                            {
                                //Add the column
                                _questionCount++;
                                AddColumn(ColumnCount, "Q" + _questionCount, childItemData.ID.Value, null);

                                _textMappings["Q" + _questionCount] = columnText;
                                _itemMappings["Q" + _questionCount] = childItemData.ID.Value;
                            }
                            else if (childItemData is SelectItemData)
                            {
                                _questionCount++;

                                if (!(childItemData is SelectManyData))
                                {
                                    //Add the column
                                    AddColumn(ColumnCount, "Q" + _questionCount, childItemData.ID.Value, null);

                                    _textMappings["Q" + _questionCount] = columnText;
                                    _itemMappings["Q" + _questionCount] = childItemData.ID.Value;
                                }

                                //Note, this is a change in 5.0.  Matrix select children in 5.x share options with each other
                                // and their column prototype, so they have no options.  To minimize refactoring effort, we'll
                                // simply pass the id of the child item along with the prototype so the prototype's options
                                // will be associated with the child.
                                AddColumnsForSelectItemOptions(childItemData.ID, (SelectItemData)columnPrototype, columnText + "_");
                            }
                        }
                    }
                }
            }
        }
    }
}
