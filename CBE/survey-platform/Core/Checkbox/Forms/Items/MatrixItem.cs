using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Validation;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Matrix row type
    /// </summary>
    public enum RowType
    {
        /// <summary>
        /// Contains inputs and a categoroy text
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Subheading -- Contains descriptive text
        /// </summary>
        Subheading = 2,

        /// <summary>
        /// Similar to normal, except the respondent can enter the category text via a textbox
        /// </summary>
        Other = 3
    }

    /// <summary>
    /// Matrix item.  Contains questions in a grid-like structure
    /// </summary>
    [Serializable]
    public class MatrixItem : TabularItem
    {
        private Dictionary<int, MatrixRowInfo> _rowInfo;
        private Dictionary<int, MatrixColumnInfo> _columnInfo;

        /// <summary>
        /// Dictionary of row info objects
        /// </summary>
        private Dictionary<int, MatrixRowInfo> RowInfo
        {
            get { return _rowInfo ?? (_rowInfo = new Dictionary<int, MatrixRowInfo>()); }
        }

        /// <summary>
        /// Dictionary of column info objects
        /// </summary>
        private Dictionary<int, MatrixColumnInfo> ColumnInfo
        {
            get { return _columnInfo ?? (_columnInfo = new Dictionary<int, MatrixColumnInfo>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private Dictionary<int, ItemData> GetColumnPrototypesData(MatrixItemData config)
        {
            List<int> ids = new List<int>();

            for (int columnNumber = 1; columnNumber <= config.ColumnCount; columnNumber++)
            {
                ids.Add(config.GetColumnPrototypeId(columnNumber));
            }

            return ItemConfigurationManager.ListDataForItems(ids);
        }

        /// <summary>
        /// Initialize the matrix item with its configuration data
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(MatrixItemData));
            var config = (MatrixItemData)configuration;

            //Store information about columns and row types
            PrimaryColumnIndex = config.PrimaryKeyColumnIndex;

            ColumnInfo.Clear();
            RowInfo.Clear();

            var itemsData = GetColumnPrototypesData(config);

            //Store column information
            for (int columnNumber = 1; columnNumber <= config.ColumnCount; columnNumber++)
            {
                int protId = config.GetColumnPrototypeId(columnNumber);
                bool isRequired = itemsData.ContainsKey(protId)
                    ? GetColumnRequired(protId, itemsData[protId])
                    : GetColumnRequired(protId);

                var info = new MatrixColumnInfo
                {
                    ColumnNumber = columnNumber,
                    Width = config.GetColumnWidth(columnNumber),
                    ValidationErrors = new List<string>(),
                    PrototypeItemId = protId,
                    LanguageCode = languageCode,
                    TemplateID = templateId,
                    TypeName = config.GetColumnTypeName(columnNumber),
                    RequireAnswers = isRequired,
                    RequireUniqueAnswers = config.GetColumnUniqueness(columnNumber)
                };

                //convert dropdown items into pdf
                if (ExportMode == ExportMode.Pdf && info.TypeName == "DropdownList")
                    info.TypeName = "RadioButtons";

                ColumnInfo[columnNumber] = info;
            }

            var matrixItemTextDecorator =
                config.CreateTextDecorator(languageCode) as MatrixItemTextDecorator;

            //Store row information
            for (int rowNumber = 1; rowNumber <= config.RowCount; rowNumber++)
            {
                var rowInfo = new MatrixRowInfo { RowNumber = rowNumber };

                if (config.IsRowSubheading(rowNumber))
                {
                    rowInfo.RowType = RowType.Subheading;
                }
                else if (config.IsRowOther(rowNumber))
                {
                    rowInfo.RowType = RowType.Other;
                }
                else
                {
                    rowInfo.RowType = RowType.Normal;
                }

                rowInfo.Alias = config.GetRowAlias(rowNumber);

                rowInfo.Text = Utilities.AdvancedHtmlDecode(matrixItemTextDecorator.GetRowText(rowNumber));

                RowInfo[rowNumber] = rowInfo;
            }

            //Call base method, which will populate internal items dictionaries
            base.Configure(configuration, languageCode, templateId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnPrototypeId"></param>
        /// <returns></returns>
        protected bool GetColumnRequired(int columnPrototypeId)
        {
            return GetColumnRequired(columnPrototypeId, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnPrototypeId"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        protected bool GetColumnRequired(int columnPrototypeId, ItemData itemData)
        {
            if (columnPrototypeId <= 0)
            {
                return false;
            }

            var lightweightItemMetaData = SurveyMetaDataProxy.GetItemData(columnPrototypeId, false, itemData);

            return lightweightItemMetaData != null && lightweightItemMetaData.RequiresAnswer;
        }

        /// <summary>
        /// List column numbers
        /// </summary>
        /// <returns></returns>
        public List<MatrixColumnInfo> ListColumns()
        {
            var columnInfo = new List<MatrixColumnInfo>();

            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                columnInfo.Add(GetColumnInfo(columnNumber));
            }

            return columnInfo;
        }

        /// <summary>
        /// List row numbers
        /// </summary>
        /// <returns></returns>
        public List<MatrixRowInfo> ListRows()
        {
            var rowInfo = new List<MatrixRowInfo>();

            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                rowInfo.Add(GetRowInfo(rowNumber));
            }

            return rowInfo;
        }

        /// <summary>
        /// Get information for a column.  Returns an empty object with columnNumber of -1 for non-existant
        /// columns
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public MatrixColumnInfo GetColumnInfo(int columnNumber)
        {
            return _columnInfo.ContainsKey(columnNumber)
                ? ColumnInfo[columnNumber]
                : new MatrixColumnInfo { ColumnNumber = -1 };
        }

        /// <summary>
        /// Get a row info object for a row
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public MatrixRowInfo GetRowInfo(int rowNumber)
        {
            return _rowInfo.ContainsKey(rowNumber)
                ? RowInfo[rowNumber]
                : new MatrixRowInfo { RowNumber = -1 };
        }

        /// <summary>
        /// Get the primary column index
        /// </summary>
        public Int32 PrimaryColumnIndex { get; private set; }

        /// <summary>
        /// Create child items for the matrix
        /// </summary>
        /// <param name="config"></param>
        /// <param name="languageCode"></param>
        protected override void CreateChildItems(TabularItemData config, string languageCode)
        {
            List<int> itemIds = new List<int>();

            //Instantiate child items
            for (int i = 1; i <= RowCount; i++)
            {
                for (int j = 1; j <= ColumnCount; j++)
                {
                    int? id = config.GetItemIdAt(i, j);

                    if (j != PrimaryColumnIndex)
                        id = GetColumnInfo(j).PrototypeItemId;

                    if (id.HasValue && !itemIds.Contains(id.Value))
                        itemIds.Add(id.Value);
                }
            }

            var allItemsData = ItemConfigurationManager.ListDataForItems(itemIds);

            //Instantiate child items
            for (int i = 1; i <= RowCount; i++)
            {
                for (int j = 1; j <= ColumnCount; j++)
                {
                    int? itemId = config.GetItemIdAt(i, j);

                    if (itemId.HasValue)
                    {
                        //Now either create the item based on a prototype or load data from 
                        // database
                        ItemData itemData;

                        if (allItemsData.ContainsKey(itemId.Value))
                            itemData = allItemsData[itemId.Value];
                        else
                            itemData = (j == PrimaryColumnIndex)
                            ? ItemConfigurationManager.GetConfigurationData(itemId.Value)
                            : GetColumnInfo(j).PrototypeItemData;

                        if (itemData != null)
                        {
                            //Create item, configure, set id, and add to child item dictionary
                            Item item = itemData.CreateItem(languageCode, TemplateID);
                            item.Configure(itemData, languageCode, TemplateID);
                            item.Parent = this;
                            item.ID = itemId.Value;

                            SetChildItemInDictionary(new Coordinate(j, i), item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get whether the matrix item is required by checking each column
        /// </summary>
        public override bool Required
        {
            get
            { return ColumnInfo.Values.Any(info => info.RequireAnswers); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnItemExcluded()
        {
            base.OnItemExcluded();

            //add an empty answer for a numeric slider to avoid default value calculation 
            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
                {
                    var slider = GetItemAt(rowNumber, columnNumber) as Slider;

                    if (slider != null && slider.AnswerData != null)
                    {
                        slider.AnswerData.SetEmptyAnswerForItem(slider.ID);
                    }
                }
            }
        }

        /// <summary>
        /// Set the repsonse for child items as well
        /// </summary>
        protected override void OnResponseSet()
        {
            base.OnResponseSet();

            ////Set the response for the column templates so that piping can be done
            //foreach (Item item in _columnTemplates.Values)
            //{
            //    if (item != null && item is ResponseItem)
            //    {
            //        ((ResponseItem)item).Response = Response;
            //    }
            //}
        }


        /// <summary>
        /// Run any associated rules
        /// </summary>
        public override void RunRules()
        {
            base.RunRules();

            // run rules for each Item even if the parent is excluded, so that recorded answers are cleaned out
            for (int x = 1; x <= RowCount; x++)
            {
                var item = (ResponseItem)GetItemAt(x, PrimaryColumnIndex);
                item.RunRules();

                //set exclude status for all times in the row
                for (int y = PrimaryColumnIndex + 1; y <= ColumnCount; y++) // TablularItem indexes begin at 1
                {
                    var i = (ResponseItem)GetItemAt(x, y);
                    if (i != null)
                        i.Excluded = item.Excluded;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        protected override void OnChildItemExcluded(Item child)
        {
            base.OnChildItemExcluded(child);

            Coordinate coord = GetChildItemCoordinate(child.ID);

            for (int x = 1; x <= ColumnCount; x++) // TablularItem indexes begin at 1
            {
                if (x != PrimaryColumnIndex) // ignore the PK
                {
                    Item sibling = GetItemAt(coord.Y, x);

                    if (sibling != null)
                    {
                        if (!sibling.Excluded)
                        {
                            //Setting excluded also deletes answers for most items
                            sibling.Excluded = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        protected override void OnChildItemIncluded(Item child)
        {
            base.OnChildItemExcluded(child);

            Coordinate coord = GetChildItemCoordinate(child.ID);

            for (int x = 1; x <= ColumnCount; x++) // TablularItem indexes begin at 1
            {
                if (x != PrimaryColumnIndex) // ignore the PK
                {
                    Item sibling = GetItemAt(coord.Y, x);

                    if (sibling != null)
                    {
                        if (sibling.Excluded)
                        {
                            //Setting excluded also deletes answers for most items
                            sibling.Excluded = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate answers to matrix question
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            //Run any item rules to ensure that items are properly marked as excluded
            RunRules();

            var validator = new MatrixItemValidator();

            if (validator.Validate(this))
            {
                //Clear previous errors
                for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
                {
                    if (ColumnInfo.ContainsKey(columnNumber))
                    {
                        ColumnInfo[columnNumber].ValidationErrors.Clear();
                    }
                }
                return true;
            }

            //Get column validation errors
            Dictionary<int, List<string>> validationErrors = validator.ColumnValidationErrors;

            //Set matrix-wide error
            ValidationErrors.Add(validator.GetMessage(LanguageCode));

            //Now assign validation errors to columns
            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                if (ColumnInfo.ContainsKey(columnNumber))
                {
                    ColumnInfo[columnNumber].ValidationErrors.Clear();

                    if (validationErrors.ContainsKey(columnNumber))
                    {
                        ColumnInfo[columnNumber].ValidationErrors = new List<string>(validationErrors[columnNumber]);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the text for a row.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected virtual string GetRowText(int row)
        {
            Item item = GetItemAt(row, PrimaryColumnIndex);

            if (item != null && item is Message)
            {
                return ((Message)item).Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the alias for a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected virtual string GetRowAlias(int row)
        {
            Item item = GetItemAt(row, PrimaryColumnIndex);

            if (item != null)
            {
                return item.Alias;
            }

            return string.Empty;
        }

        /// <summary>
        /// Determine if the row is excluded or not.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        internal bool IsRowExcluded(int rowNumber)
        {
            return GetItemAt(rowNumber, PrimaryColumnIndex).Excluded;
        }

        /// <summary>
        /// Build up transfer object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is ItemProxyObject)
            {
                var additionalData = new MatrixAdditionalData { LabelColumnIndex = PrimaryColumnIndex };

                //Populate list of columns
                var dtoColumnList = new List<MatrixItemColumn>();

                for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
                {
                    if (ColumnInfo.ContainsKey(columnNumber))
                    {
                        dtoColumnList.Add(ColumnInfo[columnNumber].GetDataTransferObject());
                    }
                }

                additionalData.Columns = dtoColumnList.ToArray();

                var dtoRowList = new List<MatrixItemRow>();
                int currentRowIndex = 0;

                //Populate list of rows
                for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
                {
                    if (RowInfo.ContainsKey(rowNumber) && !IsRowExcluded(rowNumber))
                    {
                        MatrixItemRow row = RowInfo[rowNumber].GetDataTransferObject();
                        row.RowNumber = currentRowIndex + 1;  //Set actual row number because some rows can be excluded.
                        currentRowIndex++;
                        dtoRowList.Add(row);
                    }
                }

                additionalData.Rows = dtoRowList.ToArray();

                //Populate item list
                additionalData.ChildItems = new IItemProxyObject[dtoRowList.Count][];

                currentRowIndex = 0;

                for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
                {
                    if (IsRowExcluded(rowNumber))
                        continue;

                    var rowItems = new List<IItemProxyObject>();

                    for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
                    {
                        Item childItem = GetItemAt(rowNumber, columnNumber);

                        if (childItem != null)
                        {
                            if (ExportMode == ExportMode.Pdf && childItem.ItemTypeName == "DropdownList")
                                (childItem as Select1).ConvertToRadioButtons();

                            IItemProxyObject childItemDto = childItem.GetDataTransferObject();

                            if (childItemDto != null)
                            {
                                rowItems.Add(childItemDto);
                            }
                        }

                        if (childItem is LabelledItem)
                        {
                            dtoColumnList[columnNumber - 1].HeaderText = (childItem as LabelledItem).Text;
                            if (childItem is SelectItem)
                            {
                                var item = childItem as SelectItem;
                                var options = dtoColumnList[columnNumber - 1].OptionTexts;
                                for(int j=0; j<options.Length; j++)
                                {
                                    if (j < item.Options.Count)
                                        options[j] = item.Options[j].Text;
                                }
                            }
                        }
                    }

                    additionalData.ChildItems[currentRowIndex] = rowItems.ToArray();
                    currentRowIndex++;
                }

                ((ItemProxyObject)itemDto).AdditionalData = additionalData;
            }
        }

        /// <summary>
        /// Write matrix-specific data
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXmlMetaData(XmlWriter writer)
        {
            base.WriteXmlMetaData(writer);

            writer.WriteStartElement("matrixMetaData");
            writer.WriteElementString("pkColumnIndex", PrimaryColumnIndex.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write row instance data
        /// </summary>
        /// <param name="row"></param>
        /// <param name="writer"></param>
        protected override void WriteRowInstanceData(int row, XmlWriter writer)
        {
            writer.WriteStartElement("row");
            writer.WriteAttributeString("rowNumber", row.ToString());

            //Add row type, text and alias
            MatrixRowInfo rowInfo = GetRowInfo(row);

            writer.WriteAttributeString("rowType", rowInfo.RowType.ToString());

            writer.WriteElementString("text", Utilities.IsNotNullOrEmpty(rowInfo.Text) ? rowInfo.Text : string.Empty);
            writer.WriteElementString("alias", Utilities.IsNotNullOrEmpty(rowInfo.Alias) ? rowInfo.Alias : string.Empty);

            writer.WriteStartElement("columns");

            for (int i = 1; i <= ColumnCount; i++)
            {
                WriteColumnInstanceData(row, i, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write instance data for the column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="writer"></param>
        protected override void WriteColumnInstanceData(int row, int column, XmlWriter writer)
        {
            MatrixColumnInfo columnInfo = GetColumnInfo(column);

            writer.WriteStartElement("column");
            writer.WriteAttributeString("columnNumber", column.ToString());
            writer.WriteAttributeString("uniqueValues", columnInfo.RequireUniqueAnswers ? "true" : "false");
            writer.WriteAttributeString("isPkColumn", column == PrimaryColumnIndex ? "true" : "false");
            writer.WriteElementString("text", Utilities.IsNotNullOrEmpty(columnInfo.Text) ? columnInfo.Text : string.Empty);
            writer.WriteElementString("alias", Utilities.IsNotNullOrEmpty(columnInfo.Alias) ? columnInfo.Alias : string.Empty);

            Item item = GetItemAt(row, column);

            if (item != null)
            {
                item.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Update object state of child items based on answer data
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            base.UpdateFromDataTransferObject(dto);
            MatrixAdditionalData additionalData = dto.AdditionalData as MatrixAdditionalData;

            if (additionalData == null)
                throw new Exception("AdditionalData must be the type of MatrixAdditionalData");

            int currentRowIndex = 0;
            for (int row = 1; row <= RowCount; row++)
            {
                if (IsRowExcluded(row))
                    continue;

                for (int column = 1; column <= ColumnCount; column++)
                {
                    if (additionalData.ChildItems[currentRowIndex].Length > column - 1)
                        //old matrices may contain 1 child per row in case of sub-header rows
                        GetItemAt(row, column)
                            .UpdateFromDataTransferObject(additionalData.ChildItems[currentRowIndex][column - 1]);
                }

                currentRowIndex++;
            }
            
           
        }


        /// <summary>
                /// Initializes default values for children
                /// </summary>
            internal override
            void InitializeDefaults()
        {
            foreach (var i in Items)
            {
                i.InitializeDefaults();
            }
        }


    }
}
