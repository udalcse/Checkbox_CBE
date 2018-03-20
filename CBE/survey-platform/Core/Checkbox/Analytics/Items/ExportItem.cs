using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Checkbox.Analytics.Data;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Progress;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Users;
using Newtonsoft.Json;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Base class for export items
    /// </summary>
    [Serializable]
    public abstract class ExportItem : AnalysisItem
    {
        private struct ColumnData
        {
            public int ItemId;
            public int? OptionId;
            public bool IsBinded;
        }

        private List<string> _columnNames;
        private Dictionary<int, ColumnData> _columnToIdMapping;
        private List<int> _multiselectOtherOptions;
        private List<int> _naOptions;
        private List<int> _itemsForPointsExport;

        bool _columnsAdded;

        protected List<int> ItemsForPointsExport
        {
            get
            {
                return _itemsForPointsExport;
            }
        }

        /// <summary>
        /// Configure the export item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData itemData, string languageCode, int? templateId)
        {
            base.Configure(itemData, languageCode, templateId);

            _columnNames = new List<string>();
            _columnToIdMapping = new Dictionary<Int32, ColumnData>();
            _multiselectOtherOptions = new List<int>();
            _naOptions = new List<int>();
            _itemsForPointsExport = new List<int>();
        }

        /// <summary>
        /// Get whether select many items should have separate columns or be merged
        /// </summary>
        protected abstract bool MergeSelectMany { get; }

        /// <summary>
        /// Get whether open-end answers should be included
        /// </summary>
        protected abstract bool IncludeOpenEnded { get; }

        /// <summary>
        /// Get whether hidden item answers should be included
        /// </summary>
        public abstract bool IncludeHidden { get; }

        /// <summary>
        /// Get whether hidden item answers should be included
        /// </summary>
        public abstract bool ExportRankOrderPoints { get; }

        /// <summary>
        /// Consider templates valid to avoid freshness checks on source items.  This assumes export item data
        /// created by AnalysisTemplateManager which loads survey items and does not skip freshness checks.
        /// </summary>
        public override bool TemplatesValidated { get { return true; } }

        /// <summary>
        /// Clear columns
        /// </summary>
        public void ClearColumns()
        {
            _columnNames.Clear();
            _columnToIdMapping.Clear();
            _multiselectOtherOptions.Clear();
            _naOptions.Clear();

            _columnsAdded = false;
        }

        /// <summary>
        /// Determine the column names
        /// </summary>
        protected virtual void AddColumns(string progressKey, string languageCode, int startProgress, int endProgress, Guid? responseId, string userName)
        {
            ClearColumns();

            //Get the text for each item.  If a child item of a composite is included directly, contextual information from 
            // the parent will not be included.  For composite items, the contextual information will be included for the children
            AddColumnsForResponseTemplate(SourceResponseTemplateId, progressKey, languageCode, startProgress, endProgress, responseId, userName);
        }

        /// <summary>
        /// Add columns for the specified response template
        /// </summary>
        /// <param name="responseTemplateID">The response template identifier.</param>
        /// <param name="progressKey">The progress key.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="startProgress">The start progress.</param>
        /// <param name="endProgress">The end progress.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="userName">Name of the user.</param>
        protected virtual void AddColumnsForResponseTemplate(Int32 responseTemplateID, string progressKey, string languageCode, int startProgress, int endProgress, Guid? responseId, string userName)
        {
            ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(responseTemplateID);

            if (rt != null)
            {
                rt.LoadPages();

                var rtItemCount = rt.ListTemplateItemIds().Length;

                bool trackingProgress = Utilities.IsNotNullOrEmpty(progressKey) && Utilities.IsNotNullOrEmpty(languageCode);

                string trackingText = trackingProgress
                    ? TextManager.GetText("/controlText/exportManager/loadingItems", languageCode)
                    : string.Empty;

                int itemCount = 1;

                for (int pageNumber = 0; pageNumber < rt.PageCount; pageNumber++)
                {
                    var page = rt.GetPageAtPosition(pageNumber + 1);

                    var pageItemIds = page.ListItemIds();

                    foreach (var itemId in pageItemIds)
                    {
                        if (SourceItemIds.Contains(itemId))
                        {
                            AddColumnsForItem(ItemConfigurationManager.GetConfigurationData(itemId, true), string.Empty, string.Empty, null, responseId, userName);

                            if (trackingProgress)
                            {
                                string curText = trackingText.Contains("{0}") && trackingText.Contains("{1}")
                                    ? string.Format(trackingText, itemCount, rtItemCount)
                                    : trackingText;

                                int curItem = startProgress + (int)(((double)itemCount / rtItemCount) * (endProgress - startProgress));

                                ProgressProvider.SetProgress(
                                    progressKey,
                                    curText,
                                    string.Empty,
                                    ProgressStatus.Running,
                                    curItem,
                                    100);
                            }

                            itemCount++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add columns for the item
        /// </summary>
        /// <param name="itemData">The item data.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="textOverride">The text override.</param>
        /// <param name="prototype">The prototype.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="userName">Name of the user.</param>
        protected virtual void AddColumnsForItem(ItemData itemData, string prefix, string textOverride, ItemData prototype, Guid? responseId = null, string userName = null)
        {
            //Only add answer able items
            if (itemData.ItemIsIAnswerable)
            {
                if (itemData is MatrixItemData)
                {
                    if (PropertyBindingManager.IsBinded(itemData.ID.Value))
                    {
                        var itemText = GetItemText(itemData.ID.Value);

                        //add binded matrix id to the mapping list to generate columns and responses later during writing csv 
                        AddColumn(ColumnCount, itemText, itemData.ID.Value, null, true);
                    }
                    else
                    {
                        AddColumnsForMatrixItem((MatrixItemData)itemData, prefix);
                    }
                }
                else if (itemData is ICompositeItemData)
                {
                    AddColumnsForCompositeItem((ICompositeItemData)itemData, prefix);
                }
                else if (itemData is UploadItemData)
                {
                    AddColumnsForUploadItem(itemData, prefix, textOverride);
                }
                else if (itemData is AddressVerifierItemData)
                {
                    AddColumnsForAddressVerifierItem(itemData, prefix, textOverride);
                }
                else if (itemData is RankOrderItemData)
                {
                    AddColumnsForRankOrder(itemData, prefix);
                }
                else
                {
                    if (itemData is SelectItemData)
                    {
                        AddColumnsForSelectItem((SelectItemData)itemData, prefix, textOverride, (SelectItemData)prototype);
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

                        AddColumn(ColumnCount, columnText, itemData.ID.Value, null);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected virtual void AddColumnsForRankOrder(ItemData itemData, string prefix)
        {
            if (!_itemsForPointsExport.Contains(itemData.ID.Value))
                _itemsForPointsExport.Add(itemData.ID.Value);

            string columnText = DetermineColumnTextForItem(itemData, prefix);
            RankOrderItemData roid = (RankOrderItemData)itemData;

            for (int i=0; i < roid.Options.Count; i++)
            {
                var o = roid.Options[i];
                string optionText = GetOptionText(itemData.ID.Value, o.OptionID);
                optionText = Utilities.DecodeAndStripHtml(optionText);

                //Add the column
                AddColumn(ColumnCount, string.Format("{0}:{1}", optionText, columnText), itemData.ID.Value, o.OptionID);
            }
        }

        /// <summary>
        /// Add columns for a matrix item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected virtual void AddColumnsForMatrixItem(MatrixItemData itemData, string prefix)
        {
            string matrixText = DetermineColumnTextForItem(itemData, prefix);

            var decorator = (MatrixItemTextDecorator)itemData.CreateTextDecorator(LanguageCode);

            var columnPrototypes = new Dictionary<int, ItemData>();

            

            for (int i = 1; i <= itemData.ColumnCount; i++)
            {
                var columnProtypeId = itemData.GetColumnPrototypeId(i);

                if (columnProtypeId > 0)
                {
                    columnPrototypes[i] = ItemConfigurationManager.GetConfigurationData(columnProtypeId, true);
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
                                    AddColumnsForItem(childItemData, string.Empty, matrixText + "_" + rowText + "_Other", null);
                                }
                            }
                        }
                        else
                        {
                            var columnPrototype = columnPrototypes.ContainsKey(column) ? columnPrototypes[column] : null;

                            //Get the column text
                            string columnText = string.Empty;

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

                            columnText = matrixText + "_" + rowText + "_" + columnText;

                            //Add column for the select item
                            if ((childItemData is TextItemData && IncludeOpenEnded))
                            {
                                //Add the column
                                AddColumnsForItem(childItemData, string.Empty, columnText, null);
                            }
                            else if (childItemData is SelectItemData)
                            {
                                if (childItemData is SelectManyData)
                                {
                                    if (MergeSelectMany)
                                    {
                                        AddColumnsForItem(childItemData, string.Empty, columnText, columnPrototype);
                                    }
                                    else
                                    {
                                        AddColumnsForSelectItemOptions((SelectItemData)childItemData, columnText + "_", (SelectItemData)columnPrototype);
                                    }
                                }
                                else
                                {
                                    AddColumnsForItem(childItemData, string.Empty, columnText, columnPrototype);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add columns for children of composites
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected virtual void AddColumnsForCompositeItem(ICompositeItemData itemData, string prefix)
        {
            string itemText = DetermineColumnTextForItem((ItemData)itemData, prefix);

            foreach (var childItemId in itemData.GetChildItemDataIDs())
            {
                var childItemData = ItemConfigurationManager.GetConfigurationData(childItemId, true);
                AddColumnsForItem(childItemData, itemText, string.Empty, null);
            }
        }

        /// <summary>
        /// Add columns for a select item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <param name="textOverride"></param>
        protected virtual void AddColumnsForSelectItem(SelectItemData itemData, string prefix, string textOverride, SelectItemData prototype)
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
                AddColumn(ColumnCount, itemText, itemData.ID.Value, null);
            }

            AddColumnsForSelectItemOptions(itemData, itemText + "_", prototype);
        }

        /// <summary>
        /// This method performs two tasks. The first is to add columns for individual options
        /// in a select many item and the second is to store the ids of "other" fields for select
        /// many and rating scale options for later use. (The "Other' option in a rating scale represents
        /// the n/a option).
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        protected virtual void AddColumnsForSelectItemOptions(SelectItemData itemData, string prefix, SelectItemData prototype)
        {
            bool isSelectMany = (itemData is SelectManyData);
            bool isRatingScale = (itemData is RatingScaleItemData);

            //If the item is not a rating scale or
            //if it is a checkbox box with merged responses, then there's nothing to do.
            if (!isRatingScale && (isSelectMany && MergeSelectMany))
            {
                return;
            }

            System.Collections.ObjectModel.ReadOnlyCollection<ListOptionData> options = prototype == null ? itemData.Options : prototype.Options;
            //Get the option texts and add a column for each
            foreach (ListOptionData option in options)
            {
                //Flag other options for multi select so we know to export the answer text instead
                // of a 1 or 0 when not merging selectmany answers
                if (isSelectMany)
                {
                    string optionText = GetOptionText(itemData.ID.Value, option.OptionID);
                    string columnText = prefix + Utilities.DecodeAndStripHtml(optionText);
                    AddColumn(ColumnCount, columnText, itemData.ID.Value, option.OptionID);
                }

                if (option.IsOther)
                {
                    if (isSelectMany)
                    {
                        _multiselectOtherOptions.Add(option.OptionID);
                    }

                    //Store rating scale option id
                    if (isRatingScale)
                    {
                        _naOptions.Add(option.OptionID);
                    }
                }
            }
        }

        /// <summary>
        /// Determine the column text for an upload item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <param name="textOverride"></param>
        protected virtual void AddColumnsForUploadItem(ItemData itemData, string prefix, string textOverride)
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

            AddColumn(ColumnCount, itemText, itemData.ID.Value, null);
        }

        enum AddressVerifierProperties
        {
            Address = 0,
            Latitude,
            Longitude,
            Count
        }

        /// <summary>
        /// Determine the column texts for an address verifier item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <param name="textOverride"></param>
        protected virtual void AddColumnsForAddressVerifierItem(ItemData itemData, string prefix, string textOverride)
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

            for (int i = 0; i < (int)AddressVerifierProperties.Count; i++)
            {
                AddColumn(ColumnCount, itemText + " (" + ((AddressVerifierProperties)i) + ")", itemData.ID.Value, null);
            }
        }

        /// <summary>
        /// Determine the column text for an item
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected virtual string DetermineColumnTextForItem(ItemData itemData, string prefix)
        {
            return prefix + GetItemText(itemData.ID.Value);
        }

        /// <summary>
        /// Add a column to the column collection
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="columnName"></param>
        /// <param name="itemId"></param>
        /// <param name="optionId"></param>
        protected virtual void AddColumn(Int32 columnNumber, string columnName, int itemId, int? optionId, bool isBinded = false)
        {
            _columnNames.Add(columnName);
            _columnToIdMapping[columnNumber] = new ColumnData { ItemId = itemId, OptionId = optionId, IsBinded = isBinded};
        }

        /// <summary>
        /// Get the number of columns for the item
        /// </summary>
        public virtual Int32 ColumnCount
        {
            get { return _columnNames.Count; }
        }

        /// <summary>
        /// Get the list of column names for the export item.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetColumnNames(string progressKey, string languageCode, int startProgress, int endProgress, Guid? responseId = null , string userName = null)
        {
            EnsureColumnsAdded(progressKey, languageCode, startProgress, endProgress, responseId, userName);

            return new List<string>(_columnNames);
        }

        /// <summary>
        /// Ensure export columns have been added
        /// </summary>
        private void EnsureColumnsAdded(string progressKey, string languageCode, int startProgress, int endProgress, Guid? responseId, string userName)
        {
            if (!_columnsAdded)
            {
                AddColumns(progressKey, languageCode, startProgress, endProgress, responseId, userName);
                _columnsAdded = true;
            }
        }

        /// <summary>
        /// Get the answer row for the specified response id
        /// </summary>
        /// <param name="includeTestResponses"></param>
        /// <param name="rt"></param>
        /// <param name="responseID"></param>
        /// <param name="stripHtmlTags"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <returns></returns>
        public virtual List<string> GetRowAnswers(long responseID, bool stripHtmlTags, bool includeIncompleteResponses, bool includeTestResponses, ResponseTemplate rt)
        {
            int columnCount = _columnNames.Count;

            var rowAnswers = new List<string>(_columnNames.Count);

            for (int column = 0; column < columnCount; column++)
            {
                int itemId = _columnToIdMapping[column].ItemId;

                RankOrderItemData rankOrderData;
                SliderItemData sliderItemData;

                if (!ExportRankOrderPoints &&
                    (rankOrderData = ItemConfigurationManager.GetConfigurationData(itemId, true) as RankOrderItemData) != null)
                {
                    var answers = GetRowAnswersForRankOrder(responseID, includeIncompleteResponses, includeTestResponses, rankOrderData);
                    rowAnswers.AddRange(answers);
                    column += answers.Count - 1;
                }
                else if ((sliderItemData = ItemConfigurationManager.GetConfigurationData(itemId, true) as SliderItemData) != null)
                {
                    //if this is a matrix slider, we should use column prototype data to get slider value type
                    var protId = ItemConfigurationManager.GetPrototypeItemID(itemId);
                    SliderItemData protItemData = null;
                    if (protId.HasValue)
                        protItemData = ItemConfigurationManager.GetConfigurationData(protId.Value) as SliderItemData;

                    if ((protItemData != null && protItemData.ValueType == SliderValueType.NumberRange)
                        || (protItemData == null && sliderItemData.ValueType == SliderValueType.NumberRange))
                        rowAnswers.Add(GetRowAnswersForNumericSlider(responseID, includeIncompleteResponses, includeTestResponses, sliderItemData, rt));
                    else
                        rowAnswers.Add(GetAnswerColumnValue(responseID, column, stripHtmlTags, includeIncompleteResponses, includeTestResponses));
                }
                else
                    rowAnswers.Add(GetAnswerColumnValue(responseID, column, stripHtmlTags, includeIncompleteResponses, includeTestResponses));
            }

            return rowAnswers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseID"></param>
        /// <returns></returns>
        private ResponseTemplate GetResponseTemplateByResponseId(long responseID)
        {
            var surveyId = ResponseManager.GetSurveyIdFromResponseId(responseID);

            return surveyId.HasValue 
                ? ResponseTemplateManager.GetResponseTemplate(surveyId.Value) 
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="includeTestResponses"></param>
        /// <param name="data"> </param>
        /// <returns></returns>
        private string GetRowAnswersForNumericSlider(long responseId, bool includeIncompleteResponses, bool includeTestResponses, SliderItemData data, ResponseTemplate rt)
        {
            var answerData = GetAnalysisData(includeIncompleteResponses, includeTestResponses);
            var answer = answerData.ListItemResponseAnswers(responseId, data.ID.Value).FirstOrDefault();
            var answerText = string.Empty;

            if (answer != null)
                answerText = answer.AnswerText;
            //if answer is missed, but the page is reached, add default values
            else if (IsPageReached(responseId, data.ID.Value, rt, answerData))
            {
                if (data.DefaultValue.HasValue)
                    answerText = data.DefaultValue.ToString();
                else
                {
                    int? average = null;
                    if (data.MinValue.HasValue && data.MaxValue.HasValue)
                        average = (data.MaxValue - data.MinValue)/2;

                    var value = average ?? (data.MinValue ?? data.MaxValue);

                    if (value.HasValue)
                        answerText = value.ToString();
                }
            }

            return answerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsPageReached(long responseId, int itemId, ResponseTemplate rt, AnalysisAnswerData answerData)
        {
            var responseData = ResponseManager.GetResponseData(responseId);

            if (responseData == null)
                return false;

            if (rt == null)
                rt = ResponseTemplateManager.GetResponseTemplateFromResponseGUID(responseData.Guid);
            
            var itemPagePos = rt.GetPagePositionForItem(itemId);
            var lastPage = rt.GetPage(responseData.LastPageViewed);

            if (!itemPagePos.HasValue || lastPage == null)
                return false;

            if (itemPagePos.Value <= lastPage.Position)
                return true;

            //check next page after slider, has it been reached ?
            var nextPage = rt.GetPageAtPosition(itemPagePos.Value + 1);

            foreach (var id in nextPage.ListItemIds())
            {
                if (answerData.ListItemResponseAnswers(responseId, id).Any())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseID"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="data"> </param>
        /// <returns></returns>
        private List<string> GetRowAnswersForRankOrder(Int64 responseID, bool includeIncompleteResponses, bool includeTestResponses, RankOrderItemData data)
        {           
            Dictionary<ListOptionData, double> options = new Dictionary<ListOptionData, double>();
            foreach (var option in data.Options)
            {
                ItemAnswer optionAnswer = GetAnalysisData(includeIncompleteResponses, includeTestResponses).GetOptionAnswer(responseID, data.ID.Value, option.OptionID);
                if (optionAnswer != null && _itemsForPointsExport.Contains(data.ID.Value) && optionAnswer.Points.HasValue)
                    options.Add(option, optionAnswer.Points.Value);
            }

            var optionsRanked = options.OrderByDescending(o => o.Value).Select(o => o.Key.OptionID).ToList();

            var result = new List<string>();
            foreach (var option in data.Options)
            {
                if (optionsRanked.Contains(option.OptionID))
                    result.Add((optionsRanked.IndexOf(option.OptionID) + 1).ToString());
                else
                    result.Add(string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Get the value for a particular column
        /// </summary>
        /// <param name="responseID"></param>
        /// <param name="column"></param>
        /// <param name="stripHtmlTags"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <returns></returns>
        public virtual string GetAnswerColumnValue(Int64 responseID, Int32 column, bool stripHtmlTags, bool includeIncompleteResponses, bool includeTestResponses)
        {
            if (!_columnToIdMapping.ContainsKey(column))
            {
                return string.Empty;
            }

            ColumnData columnData = _columnToIdMapping[column];

            //Get the answer
            if (columnData.OptionId.HasValue)
            {
                // We can't use the AppSetting here in the hosted environment due to it's lack of AppSetting cache
                // Without the cache the AppSetting check must call into the database for every column, which is very expensive
                ItemAnswer optionAnswer = GetAnalysisData(includeIncompleteResponses, includeTestResponses).GetOptionAnswer(responseID, columnData.ItemId, columnData.OptionId.Value);

                //handle the case when the option is of type other for a multiselect return the other text
                // rather than the typical 1 or 0 indicator.
                if (_multiselectOtherOptions.Contains(columnData.OptionId.Value))
                {
                    if (optionAnswer != null)
                    {
                        if (IncludeOpenEnded)
                        {
                            return GetAnswerText(optionAnswer, column);
                        }

                        return "1";
                    }

                    if (IncludeOpenEnded)
                    {
                        return string.Empty;
                    }
                    return "0";
                }

                if (optionAnswer != null)
                {
                    //Rank Order results
                    if (_itemsForPointsExport.Contains(columnData.ItemId) && optionAnswer.Points.HasValue)
                        return optionAnswer.Points.Value.ToString();
                    return "1";
                }

                return "0";
            }


            //Otherwise, this is an item

            string answer = string.Empty;

            if (PropertyBindingManager.IsBinded(columnData.ItemId))
            {
                var item = ItemConfigurationManager.GetItemTypeInfo(columnData.ItemId);
                if (item.TypeName.Equals("RadioButtons"))
                {
                    var response = ResponseManager.GetResponseData(responseID);
                    var radioField = PropertyBindingManager.GetResponseStateField<RadioButtonField>(columnData.ItemId,
                       response.Guid, response.UserIdentifier);

                    var selectedOption = radioField?.Options.FirstOrDefault(option => option.IsSelected);

                    if (selectedOption != null)
                    {
                        answer = !string.IsNullOrWhiteSpace(selectedOption.Alias)
                            ? selectedOption.Alias
                            : selectedOption.Name;
                    }
                }
                else if (item.TypeName.Equals("Matrix"))
                {
                    var response = ResponseManager.GetResponseData(responseID);
                    var matrixField = PropertyBindingManager.GetResponseStateField<MatrixField>(columnData.ItemId,
                        response.Guid,
                        response.UserIdentifier);
                    if (matrixField != null)
                        answer = matrixField.ConvertMatrixToCsvState();
                }
            }

            List<ItemAnswer> itemAnswers = GetAnalysisData(includeIncompleteResponses, includeTestResponses).ListItemResponseAnswers(responseID, columnData.ItemId);

            for (int i = 0; i < itemAnswers.Count; i++)
            {
                if (i > 0)
                {
                    answer += ", ";
                }

                answer += GetAnswerText(itemAnswers[i], column);
            }

            if (stripHtmlTags)
            {
                answer = Utilities.AdvancedHtmlDecode(answer);
                answer = Utilities.StripMSWordTags(answer);
                answer = Utilities.EncodeTagsInHtmlContent(answer);
                answer = Utilities.StripHtmlTags(answer);
                answer = answer.Replace("\r\n", " ").Replace("·", string.Empty);
                answer = Regex.Replace(answer, @"\s+", " ").Trim();
            }

            return answer;
        }

        /// <summary>
        /// Get the answer text from the given report answer object
        /// </summary>
        /// <param name="answerDataObject"></param>
        protected virtual string GetAnswerText(ItemAnswer answerDataObject, int column)
        {
            //If answer is for an "option", get the option text
            if (answerDataObject.OptionId.HasValue)
            {
                //Check for n/a option
                if (_naOptions.Contains(answerDataObject.OptionId.Value))
                {
                    return "n/a";
                }

                //Get other text if necessary
                if (answerDataObject.IsOther)
                {
                    return answerDataObject.AnswerText ?? string.Empty;
                }

                //Otherwise go with the option text
                string optionText = GetOptionText(answerDataObject.ItemId, answerDataObject.OptionId.Value);
                optionText = Utilities.DecodeAndStripHtml(optionText);

                if (string.IsNullOrWhiteSpace(optionText))
                {
                    return "Option " + answerDataObject.OptionId;
                }

                return optionText;
            }
            if (ItemConfigurationManager.GetItemTypeName(answerDataObject.ItemId).Equals("AddressVerifier"))
            {
                    if (!string.IsNullOrEmpty(answerDataObject.AnswerText))
                    {
                        string[] elements = answerDataObject.AnswerText.Split('~');
                        for (int i = 0; i < (int)AddressVerifierProperties.Count && i < elements.Length; i++)
                        {
                            if (this._columnNames[column].Contains("(" + ((AddressVerifierProperties)i).ToString() + ")"))
                                return elements[i];
                        }
                    }
            } 
            else if (ItemConfigurationManager.GetItemTypeName(answerDataObject.ItemId).Equals("FileUpload"))
            {
                if (!string.IsNullOrEmpty(answerDataObject.AnswerText))
                {
                    var answerText = string.Format("{0}_{1}_{2}", answerDataObject.ResponseId,
                        answerDataObject.AnswerId, answerDataObject.AnswerText);

                    return answerText;
                }
            }

            //Otherwise return the answer text
            return answerDataObject.AnswerText;
        }
    }
}
