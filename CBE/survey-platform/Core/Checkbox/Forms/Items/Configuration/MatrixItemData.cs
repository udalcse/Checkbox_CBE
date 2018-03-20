//===============================================================================
// Checkbox Application Source Code
// Copyright В© Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Checkbox.Forms.Data;
using Checkbox.Security;
using Checkbox.Users;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Class to represent matrix item data
    /// </summary>
    [Serializable]
    public class MatrixItemData : TabularItemData, ICloneable, ICompositeXmlSerializable
    {
        /// <summary>
        /// Enum used to indicate reason a child is invalid
        /// </summary>
        private enum InvalidChildReason
        {
            NoReason,
            NoRowHeader,
            RowHeaderWrongType,
            NoRowHeaderAppearance,
            RowHeaderAppearanceWrongType,
            NoChild,
            ChildWrongType,
            AliasIsNotUpToDate
        }

        /// <summary>
        /// Simple container for matrix column properties
        /// </summary>
        [Serializable]
        private class ColumnProperties 
        {
            public int PrototypeItemId { get; set; }
            public string ItemTypeName { get; set; }
            public bool RequiresUniqueAnswers { get; set; }
            public int? Width { get; set; }
        }

        /// <summary>
        /// Simple container for matrix row properties
        /// </summary>
        [Serializable]
        private class RowProperties
        {
            public bool IsOther { get; set; }
            public bool IsRowSubheading { get; set; }
            public string Alias { get; set; }
        }

        /// <summary>
        /// Simple container for properties of items in the matrix
        /// </summary>
        [Serializable]
        private class ChildItemProperties
        {
            public int ItemId { get; set; }
            public string ItemTypeName { get; set; }
        }

        //List of column configuration properties.
        private readonly List<ColumnProperties> _columnProperties;

        //List of row configuration properties
        private readonly List<RowProperties> _rowProperties;

        //List of items, including headers and such, in the matrix
        private readonly Dictionary<TableCoordinate, ChildItemProperties> _matrixChildren;

        //List that contains the items being removed during the row or column removal.
        //That's necessary for correct removal of all expressions that depends on these items
        private List<int> _deletedChildrenItems;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public MatrixItemData()
        {
            PrimaryKeyColumnIndex = 1;

            _rowProperties = new List<RowProperties>();
            _deletedChildrenItems = new List<int>();
            _columnProperties = new List<ColumnProperties>
                                    {
                                        new ColumnProperties
                                            {
                                                ItemTypeName = "PrimaryKey",
                                                PrototypeItemId = -1,
                                                RequiresUniqueAnswers = false,
                                                Width = null
                                            }
                                    };

            //Add column for primary key

            _matrixChildren = new Dictionary<TableCoordinate, ChildItemProperties>(new TableCoordinateComparer());
        }

        /// <summary>
        /// Get name of data table containing matrix data
        /// </summary>
        public override string ItemDataTableName { get { return "MatrixItemData"; } }

        /// <summary>
        /// Get load item sproc
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetMatrix"; } }

        /// <summary>
        /// Get/set the index of the column that represents the primary keys (questions) for the matrix.
        /// </summary>
        public int PrimaryKeyColumnIndex { get; set; }

        /// <summary>
        /// Get the number of rows in the matrix
        /// </summary>
        public override Int32 RowCount { get { return _rowProperties.Count; } }

        /// <summary>
        /// Get the number of columns in the matrix.
        /// </summary>
        public override Int32 ColumnCount { get { return _columnProperties.Count; } }

        /// <summary>
        /// Gets or sets the bindined property identifier.
        /// </summary>
        /// <value>
        /// The bindined property identifier.
        /// </value>
        public int? BindinedPropertyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the binded field.
        /// </summary>
        /// <value>
        /// The name of the binded field.
        /// </value>
        public string BindedFieldName { get; set; }

        /// <summary>
        /// Throw an exception if a column position is invalid.
        /// </summary>
        /// <param name="columnPosition"></param>
        private void ValidateColumnPosition(int columnPosition)
        {
            if (columnPosition < 1 || columnPosition > _columnProperties.Count)
            {
                throw new Exception("Column position [" + columnPosition + "] is not valid for this item [" + ID + "]");
            }
        }

        /// <summary>
        /// Throw an exception if a row position is invalid.
        /// </summary>
        /// <param name="rowPosition"></param>
        private void ValidateRowPosition(int rowPosition)
        {
            if (rowPosition < 1 || rowPosition > _rowProperties.Count)
            {
                throw new Exception("Row position [" + rowPosition + "] is not valid for this item [" + ID + "]");
            }
        }

        /// <summary>
        /// Get the ID of a prototype for a column.
        /// </summary>
        /// <param name="position">1-based Column position</param>
        /// <returns>Id of the prototype.</returns>
        public Int32 GetColumnPrototypeId(Int32 position)
        {
            ValidateColumnPosition(position);

            return _columnProperties[position - 1].PrototypeItemId;
        }

        /// <summary>
        /// Get type name of a column.
        /// </summary>
        public string GetColumnTypeName(Int32 position)
        {
            ValidateColumnPosition(position);

            return _columnProperties[position - 1].ItemTypeName;
        }

        /// <summary>
        /// Get the id of the item at the given position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public override int? GetItemIdAt(Int32 row, Int32 column)
        {
            ValidateRowPosition(row);
            ValidateColumnPosition(column);

            var coordinate = new TableCoordinate(row, column);

            if (_matrixChildren.ContainsKey(coordinate))
            {
                return _matrixChildren[coordinate].ItemId;
            }

            return null;
        }

        /// <summary>
        /// Get the coordinate of a child item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public override TableCoordinate GetItemCoordinate(int itemId)
        {
            var coordinates = new List<TableCoordinate>(from entry in _matrixChildren where entry.Value.ItemId == itemId select entry.Key);

            if (coordinates.Count > 0)
            {
                return coordinates[0];
            }

            return null;
        }


        /// <summary>
        /// Get column position
        /// </summary>
        /// <param name="prototypeItemId"></param>
        /// <returns></returns>
        public int? GetColumnPosition(int prototypeItemId)
        {
            for (int i = 0; i < _columnProperties.Count; i++)
            {
                if (_columnProperties[i].PrototypeItemId == prototypeItemId)
                    return i + 1;
            }
            return null;
        }

        /// <summary>
        /// Determine if a row is a subheading or not
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool IsRowSubheading(int row)
        {
            ValidateRowPosition(row);

            return _rowProperties[row - 1].IsRowSubheading;
        }

        /// <summary>
        /// Determine if a row is an "other" row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool IsRowOther(int row)
        {
            ValidateRowPosition(row);

            return _rowProperties[row - 1].IsOther;
        }

        /// <summary>
        /// Get the alias for a row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetRowAlias(int row)
        {
            ValidateRowPosition(row);

            return _rowProperties[row - 1].Alias;
        }

        /// <summary>
        /// Set the alias for a row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="alias"></param>
        public void SetRowAlias(int row, string alias)
        {
            ValidateRowPosition(row);

            _rowProperties[row - 1].Alias = alias;
        }


        /// <summary>
        /// Clears the matrix structure.
        /// </summary>
        public void ClearMatrixStructure()
        {
            _columnProperties.Clear();
            _rowProperties.Clear();
        }

        #region Load/Save

        /// <summary>
        /// Create a new instance of matrix item configuration in the data store.
        /// </summary>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            //Save the base item data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMatrix");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, base.TextID);
            command.AddInParameter("SubTextID", DbType.String, base.SubTextID);
            command.AddInParameter("IsRequired", DbType.Boolean, base.IsRequired);
            command.AddInParameter("PKIndex", DbType.Int32, PrimaryKeyColumnIndex);

            db.ExecuteNonQuery(command, t);

            //Now create any necessary items
            SaveItems(t);
        }

        /// <summary>
        /// Update an instance of matrix item configuration in the data store.
        /// </summary>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            //Save the base item data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMatrix");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, base.TextID);
            command.AddInParameter("SubTextID", DbType.String, base.SubTextID);
            command.AddInParameter("IsRequired", DbType.Boolean, base.IsRequired);
            command.AddInParameter("PKIndex", DbType.Int32, PrimaryKeyColumnIndex);

            db.ExecuteNonQuery(command, t);

            //Now update any items
            SaveItems(t);
            
           
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyItemMetaData)
            {
                ((SurveyItemMetaData) itemDto).ChildItemIds = GetChildItemDataIDs().ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            //Columns
            WriteColumnData(writer);

            //Rows
            WriteRowData(writer, externalDataCallback);

            //Should be no need to write items as they will be created when the matrix is imported and saved
            // UPDATE NWC 2011-11-21: We still need to write the child item ids as they will need to be 
            //                        mapped to new child item ids during the import process so that any
            //                        conditions based on matrix items can be properly updated.

            
            writer.WriteStartElement("ChildItems");
            writer.WriteAttributeString("Count", _matrixChildren.Count.ToString());

            foreach (KeyValuePair<TableCoordinate, ChildItemProperties> pair in _matrixChildren.OrderBy(kvp => kvp.Key.Row).OrderBy(kvp => kvp.Key.Column))
            {
                ItemData item = ItemConfigurationManager.GetConfigurationData(pair.Value.ItemId);

                if (item == null)
                    continue;

                writer.WriteStartElement("Item");
                writer.WriteAttributeString("Id", pair.Value.ItemId.ToString());
                writer.WriteAttributeString("TypeId", item.ItemTypeID.ToString());
                writer.WriteAttributeString("Row", pair.Key.Row.ToString());
                writer.WriteAttributeString("Column", pair.Key.Column.ToString());

                writer.WriteEndElement(); // Item
            }

            writer.WriteEndElement(); // Items


            if (BindinedPropertyId.HasValue && this.ID.HasValue)
            {
                var bindedFieldName = ProfileManager.GetConnectedProfileFieldName(this.ID.Value);

                writer.WriteElementString("BindedPropertyName", bindedFieldName);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteRowData(XmlWriter writer, WriteExternalDataCallback externalDataCallback)
        {
            writer.WriteStartElement("Rows");
            writer.WriteAttributeString("Count", _rowProperties.Count.ToString());

            for (int rowIndex = 0; rowIndex < _rowProperties.Count; rowIndex++)
            {
                writer.WriteStartElement("Row");

                RowProperties props = _rowProperties[rowIndex];

                writer.WriteAttributeString("Index", (rowIndex + 1).ToString());
                writer.WriteAttributeString("IsSubheading", props.IsRowSubheading.ToString());
                writer.WriteAttributeString("IsOther", props.IsOther.ToString());
                writer.WriteAttributeString("Alias", props.Alias);

                //Write row texts by loading row message item
                var rowItemData = GetItemAt(rowIndex + 1, PrimaryKeyColumnIndex) as LocalizableResponseItemData;

                if (rowItemData != null)
                {
                    writer.WriteAttributeString("RowItemId", rowItemData.ID.ToString());

                    rowItemData.WriteItemTexts(writer);

                    writer.WriteStartElement("RowCondition");
                    writer.WriteAttributeString("Id", rowItemData.ID.ToString());

                    if (externalDataCallback != null)
                        externalDataCallback(rowItemData, writer);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteColumnData(XmlWriter writer)
        {
            int columnCount = 0;

            for (int columnIndex = 0; columnIndex < _columnProperties.Count; columnIndex++)
            {
                if (columnIndex + 1 == PrimaryKeyColumnIndex)
                    continue;

                columnCount++;
            }

            writer.WriteStartElement("Columns");
            writer.WriteAttributeString("Count", columnCount.ToString());

            for (int columnIndex = 0; columnIndex < _columnProperties.Count; columnIndex++)
            {
                //Rows are not inserted in column data table for PK index column
                // ColumnNumber = ColumnIndex + 1 since index is 0-based and number is 1-based
                if (columnIndex + 1 == PrimaryKeyColumnIndex)
                    continue;

                writer.WriteStartElement("Column");

                ColumnProperties props = _columnProperties[columnIndex];

                writer.WriteAttributeString("Index", (columnIndex + 1).ToString());
                writer.WriteAttributeString("Type", props.ItemTypeName);
                writer.WriteAttributeString("UniqueAnswers", props.RequiresUniqueAnswers.ToString());
                writer.WriteAttributeString("Width", props.Width.HasValue ? props.Width.Value.ToString() : "-1");

                ItemData prototypeItem = ItemConfigurationManager.GetConfigurationData(props.PrototypeItemId);

                prototypeItem.WriteXml(writer);

                writer.WriteEndElement(); //Column
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            //Columns
            ReadColumnData(xmlNode, creator);

            //Rows
            ReadRowData(xmlNode, callback, creator);


            BindedFieldName = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("BindedPropertyName"));
            
            // bind matrix field if it has binding id value 
            if (!string.IsNullOrWhiteSpace(BindedFieldName) && ID.HasValue)
            {
                var property = ProfileManager.GetPropertiesList().FirstOrDefault(item => item.Name.Equals(BindedFieldName));
                if (property != null)
                {
                    PropertyBindingManager.AddItemMapping(ID.Value, property.FieldId, Users.CustomFieldType.Matrix);
                }
            }
            

            //Save the item to create row message items and then read row texts
            Save();

            //
            ReadRowTexts(xmlNode);

            //Should be no need to import children as they will be created on the fly when the matrix is saved.

            /*
            reader.EnsureElement("Items", true, true);
            int childCount = reader.ReadAttributeInt(0);

            for (int itemIndex = 0; itemIndex < childCount; itemIndex++)
            {
                reader.EnsureElement("Item", true, true);

                int oldId = reader.ReadAttributeInt(0);
                int typeId = reader.ReadAttributeInt(1);
                int row = reader.ReadAttributeInt(2);
                int column = reader.ReadAttributeInt(3);

                ChildItemProperties properties = _matrixChildren[new TableCoordinate(row, column)];

                ItemData item = ItemConfigurationManager.GetConfigurationData(properties.ItemId);

                if (item.ItemTypeID != typeId)
                    throw new Exception("Type mismatch error while importing the matrix.");

                item.ReadXml(reader);

                item.Save();

                AppearanceData appearanceData = AppearanceDataManager.GetAppearanceDataForItem(item.ID.Value);

                //Ensure that AppearanceCode starts with "MATRIX_" or "CATEGORIZED". if not - add this prefix.
                if (!appearanceData.AppearanceCode.StartsWith("MATRIX_") && !appearanceData.AppearanceCode.StartsWith("CATEGORIZED"))
                {
                    AppearanceDataManager.UpdateAppearanceCode(appearanceData.ID.Value, GetChildDefaultAppearanceCode(item.ItemTypeName));
                    appearanceData.Save(item.ID.Value);
                }

            }// item */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        private void ReadRowTexts(XmlNode xmlNode)
        {
            var rowNodes = xmlNode.SelectNodes("Rows/Row");

            foreach (XmlNode rowNode in rowNodes)
            {
                var rowNumber = XmlUtility.GetAttributeInt(rowNode, "Index");

                if (rowNumber <= 0)
                {
                    continue;
                }
                
                var rowItemData = GetItemAt(rowNumber, PrimaryKeyColumnIndex) as LocalizableResponseItemData;

                if (rowItemData == null)
                {
                    continue;
                }
                
                rowItemData.ReadItemTexts(rowNode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        private void ReadColumnData(XmlNode xmlNode, object creator)
        {
            var columnNodes = xmlNode.SelectNodes("Columns/Column");

            foreach(XmlNode columnNode in columnNodes)
            {
                //Rows are not inserted in column data table for PK index column
                // ColumnNumber = ColumnIndex + 1 since index is 0-based and number is 1-based
                //if (columnIndex + 1 == PrimaryKeyColumnIndex)
                //    continue;

                var columnIndex = XmlUtility.GetAttributeInt(columnNode, "Index");
                var columType = XmlUtility.GetAttributeText(columnNode, "Type");
                var uniqueAnswers = XmlUtility.GetAttributeBool(columnNode, "UniqueAnswers");
                var width = XmlUtility.GetAttributeInt(columnNode, "Width");

                ItemData prototypeItem = ItemConfigurationManager.CreateConfigurationData(columType);

                var prototypeItemNode = columnNode.SelectSingleNode("Item");

                if(prototypeItemNode == null)
                {
                    throw new Exception("Matrix column has no prototype");
                }

                prototypeItem.Import(prototypeItemNode, UpdateChildAppearanceCode, creator);
                prototypeItem.Save();

                AddColumn(prototypeItem.ID.Value, columType, uniqueAnswers, width > 0 ? (int?)width : null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childAppearancePdo"></param>
        /// <param name="xmlNode"></param>
        private void UpdateChildAppearanceCode(PersistedDomainObject childAppearancePdo, XmlNode xmlNode, object creator)
        {
            var childAppearanceData = childAppearancePdo as AppearanceData;

            if (childAppearanceData != null && childAppearanceData.AppearanceCode.IndexOf("MATRIX", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                childAppearanceData.OverrideAppearanceCode("MATRIX_" + childAppearanceData.AppearanceCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        private void ReadRowData(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            var rowNodes = xmlNode.SelectNodes("Rows/Row");

            int rowIndex = 0;
            foreach(XmlNode rowNode in rowNodes)
            {
                var rowAlias = XmlUtility.GetAttributeText(rowNode, "Alias");
                var isSubheading = XmlUtility.GetAttributeBool(rowNode, "IsSubheading");
                var isOther = XmlUtility.GetAttributeBool(rowNode, "IsOther");

                var rowType = RowType.Normal;

                if(isSubheading)
                {
                    rowType = RowType.Subheading;
                }

                if(isOther)
                {
                    rowType = RowType.Other;
                }

                AddRow(rowType.ToString(), rowAlias);

                //get row condition node
                var conditionNode = rowNode.SelectSingleNode("RowCondition");
                if (conditionNode != null)
                {
                    //get row item
                    var rowItemData = GetItemAt(rowIndex + 1, PrimaryKeyColumnIndex) as LocalizableResponseItemData;
                    //if the item exists -- apply condition to it.
                    if (rowItemData != null && callback != null)
                        callback(rowItemData, conditionNode, creator);
                }
                rowIndex++;
            }
        }


        /// <summary>
        /// Save column information
        /// </summary>
        /// <param name="t"></param>
        protected void SaveColumnData(IDbTransaction t)
        {
            //Update the mappings
            //We'll simplify by removing the column data then re-adding it.
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper removeColumnsCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_RemoveColumnsFromMatrix");
            removeColumnsCommand.AddInParameter("MatrixID", DbType.Int32, ID);
            db.ExecuteNonQuery(removeColumnsCommand, t);

            for (int columnIndex = 0; columnIndex < _columnProperties.Count; columnIndex++)
            {
                //Rows are not inserted in column data table for PK index column
                // ColumnNumber = ColumnIndex + 1 since index is 0-based and number is 1-based
                if (columnIndex + 1 == PrimaryKeyColumnIndex)
                {
                    continue;
                }

                ColumnProperties props = _columnProperties[columnIndex];

                DBCommandWrapper insertCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMColType");
                insertCommand.AddInParameter("MatrixID", DbType.Int32, ID);
                insertCommand.AddInParameter("Column", DbType.Int32, columnIndex + 1);
                insertCommand.AddInParameter("PrototypeID", DbType.Int32, props.PrototypeItemId);
                insertCommand.AddInParameter("UniqueAnswers", DbType.Boolean, props.RequiresUniqueAnswers);
                insertCommand.AddInParameter("Width", DbType.Int32, props.Width);

                db.ExecuteNonQuery(insertCommand, t);
            }
        }

        /// <summary>
        /// Persist the row data to the database
        /// </summary>
        /// <param name="t"></param>
        protected void SaveRowData(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            //1) Delete row information
            DBCommandWrapper deleteCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeleteMRows");
            deleteCommand.AddInParameter("MatrixID", DbType.Int32, ID);
            db.ExecuteNonQuery(deleteCommand, t);

            for (int rowIndex = 0; rowIndex < _rowProperties.Count; rowIndex++)
            {
                RowProperties props = _rowProperties[rowIndex];
                DBCommandWrapper insertCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMRow");

                insertCommand.AddInParameter("MatrixID", DbType.Int32, ID.Value);
                insertCommand.AddInParameter("Row", DbType.Int32, rowIndex + 1);
                insertCommand.AddInParameter("IsSubheading", DbType.Boolean, props.IsRowSubheading);
                insertCommand.AddInParameter("IsOther", DbType.Boolean, props.IsOther);

                db.ExecuteNonQuery(insertCommand, t);
            }
        }

        /// <summary>
        /// Perform any necessary item updates
        /// </summary>
        /// <param name="t"></param>
        protected void SaveItems(IDbTransaction t)
        {
            //Save the column data
            SaveColumnData(t);

            //Save the row data
            SaveRowData(t);

            //Loop through rows and ensure child items are proper type
            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                VerifyAndUpdateRowItems(rowNumber);
            }

            //Now update matrix item position table
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper deleteCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeleteMItems");
            deleteCommand.AddInParameter("MatrixId", DbType.Int32, ID);
            db.ExecuteNonQuery(deleteCommand, t);

            //Now insert data for each item in the matrix
            foreach (TableCoordinate coordinate in _matrixChildren.Keys)
            {
                DBCommandWrapper insertCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMItem");
                insertCommand.AddInParameter("MatrixId", DbType.Int32, ID);
                insertCommand.AddInParameter("Row", DbType.Int32, coordinate.Row);
                insertCommand.AddInParameter("Column", DbType.Int32, coordinate.Column);
                insertCommand.AddInParameter("ItemId", DbType.Int32, _matrixChildren[coordinate].ItemId);

                db.ExecuteNonQuery(insertCommand, t);
            }

            //No additional step is required to "save" the actual item data objects, since a 5.0 change is that Matrix children
            // only have abstract entries and are created iteratively by calling CreateItem(...) on the column
            // prototype item data.  This should drastically reduce overhead (memory & performance) of matrix items
            // with many rows/columns or options.
        }

        /// <summary>
        /// Unbind expressions from deleted items
        /// </summary>
        public void DeleteDependentExpressions()
        {
            if (_deletedChildrenItems.Count == 0)
                return;

            RuleDataService rds = null;
            
            if (ParentTemplateId.HasValue)
            {
                try
                {
                    rds = ResponseTemplate.CreateRuleDataService(ParentTemplateId.Value);
                }
                catch (Exception) //suppress the exception
                {
                }
            }
            else
            {
                try
                {
                    if (ID.HasValue)
                    {
                        rds = ResponseTemplate.CreateRuleDataService(ResponseTemplateManager.GetResponseTemplateIDByItemID(ID.Value));
                    }
                }
                catch (Exception) //suppress the exception
                {
                }

            }

            if (rds == null)
                return;

            
            foreach (var i in _deletedChildrenItems)
            {
                rds.DeleteSubscriberExpressions(i);
            }

            rds.SaveRuleData();

            _deletedChildrenItems.Clear();
        }

        /// <summary>
        /// Create data container for matrix item
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new MatrixDataSet(ObjectTypeName, "MatrixItemData");
        }

        /// <summary>
        /// Load additional data for the matrix
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            var mds = data as MatrixDataSet;

            if (mds == null)
            {
                if (ID.HasValue)
                {
                    mds = GetConfigurationDataSet(ID.Value) as MatrixDataSet;
                    LoadBaseObjectData(mds.DomainObjectDataRow);
                    base.LoadAdditionalData(mds);
                }
                if (mds == null)
                    return;
            }

            PopulateColumnProperties(mds);
            PopulateRowProperties(mds);
            PopulateItemDataDictionary(mds);
        }

        /// <summary>
        /// Populate properties for matrix columns
        /// </summary>
        /// <param name="data"></param>
        protected virtual void PopulateColumnProperties(MatrixDataSet data)
        {
            _columnProperties.Clear();

            DataRow[] columnRows = data.GetColumnData();

            foreach (DataRow columnRow in columnRows)
            {
                int columnNumber = DbUtility.GetValueFromDataRow(columnRow, "Column", -1);
                int prototypeId = DbUtility.GetValueFromDataRow(columnRow, "ColumnPrototypeId", -1);
                string typeName = DbUtility.GetValueFromDataRow(columnRow, "ItemTypeName", string.Empty);
                bool requiresUnique = DbUtility.GetValueFromDataRow(columnRow, "UniqueAnswers", false);
                var width = DbUtility.GetValueFromDataRow<int?>(columnRow, "Width", null);

                if (columnNumber > 0
                    && prototypeId > 0
                    && Utilities.IsNotNullOrEmpty(typeName))
                {
                    _columnProperties.Add(new ColumnProperties
                    {
                        ItemTypeName = typeName,
                        Width = width,
                        PrototypeItemId = prototypeId,
                        RequiresUniqueAnswers = requiresUnique
                    });
                }
            }

            //Add column for primary key
            _columnProperties.Insert(
                PrimaryKeyColumnIndex - 1,
                new ColumnProperties
                {
                    ItemTypeName = "PrimaryKey",
                    PrototypeItemId = -1,
                    RequiresUniqueAnswers = false,
                    Width = null
                });
        }

        /// <summary>
        /// Populate properties for matrix rows
        /// </summary>
        /// <param name="data"></param>
        protected virtual void PopulateRowProperties(MatrixDataSet data)
        {
            _rowProperties.Clear();

            DataRow[] rowDataRows = data.GetRowData();

            foreach (DataRow rowDataRow in rowDataRows)
            {
                int rowNumber = DbUtility.GetValueFromDataRow(rowDataRow, "Row", -1);
                string alias = DbUtility.GetValueFromDataRow(rowDataRow, "Alias", string.Empty);
                bool isSubheading = DbUtility.GetValueFromDataRow(rowDataRow, "IsSubheading", false);
                bool isOther = DbUtility.GetValueFromDataRow(rowDataRow, "IsOther", false);

                if (rowNumber > 0)
                {
                    _rowProperties.Add(new RowProperties
                    {
                        Alias = alias,
                        IsOther = isOther,
                        IsRowSubheading = isSubheading
                    });
                }
            }
        }

        /// <summary>
        /// Populate the item data dictionary
        /// </summary>
        protected virtual void PopulateItemDataDictionary(MatrixDataSet data)
        {
            _matrixChildren.Clear();

            DataRow[] positionRows = data.GetChildPositions();

            foreach (DataRow positionRow in positionRows)
            {
                int row = DbUtility.GetValueFromDataRow(positionRow, "Row", -1);
                int column = DbUtility.GetValueFromDataRow(positionRow, "Column", -1);
                int itemId = DbUtility.GetValueFromDataRow(positionRow, "ItemId", -1);
                string itemTypeName = DbUtility.GetValueFromDataRow(positionRow, "ItemTypeName", string.Empty);

                //Add data to child items dictionary
                _matrixChildren[new TableCoordinate(row, column)] = new ChildItemProperties
                {
                    ItemId = itemId,
                    ItemTypeName = itemTypeName
                };
            }
        }

        /// <summary>
        /// Load the matrix propertie from the supplied <see cref="DataRow" />.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", false);
            PrimaryKeyColumnIndex = DbUtility.GetValueFromDataRow(data, "PKIndex", 0);
            BindinedPropertyId = DbUtility.GetValueFromDataRow<int?>(data, "BindedPropertyId", null);
        }


        #endregion

        #region Child Item Sync.

        /// <summary>
        /// Create/update/do nothing with row child as necessary
        /// </summary>
        /// <param name="rowNumber"></param>
        private void VerifyAndUpdateRowItems(int rowNumber)
        {
            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                VerifyAndUpdateChildItem(rowNumber, columnNumber);
            }
        }


        /// <summary>
        /// Create/update/do nothing with column child as necessary
        /// </summary>
        /// <param name="columnNumber"></param>
        private void VerifyAndUpdateColumnItems(int columnNumber)
        {
            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                VerifyAndUpdateChildItem(rowNumber, columnNumber);
            }
        }

        /// <summary>
        /// Ensure child item is of specific type and replace if necessary.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        private void VerifyAndUpdateChildItem(int rowNumber, int columnNumber)
        {
            InvalidChildReason reason;

            //If child can't be verified, figure out what to do
            if (!VerifyChildItem(columnNumber, rowNumber, out reason))
            {
                var itemCoordinate = new TableCoordinate(rowNumber, columnNumber);

                //If child is wrong type, delete existing child & create a new one
                if (reason == InvalidChildReason.ChildWrongType
                    || reason == InvalidChildReason.RowHeaderWrongType)
                {
                    DeleteChildItemData(itemCoordinate);
                }

                //Now create new item.  For row headers, full items are created
                if (reason == InvalidChildReason.RowHeaderWrongType
                    || reason == InvalidChildReason.NoRowHeader
                    || reason == InvalidChildReason.NoChild
                    || reason == InvalidChildReason.ChildWrongType)
                {
                    ChildItemProperties itemProps = CreateChildItemData(itemCoordinate);

                    if (itemProps != null)
                    {
                        _matrixChildren[itemCoordinate] = itemProps;
                    }
                }

                //Handle case where item appearance is wrong type
                if (reason == InvalidChildReason.NoRowHeaderAppearance
                    || reason == InvalidChildReason.RowHeaderAppearanceWrongType)
                {
                    //Create a new appearance
                    if (!_matrixChildren.ContainsKey(itemCoordinate))
                    {
                        return;
                    }
                    ChildItemProperties itemProps = _matrixChildren[itemCoordinate];

                    if (itemProps == null)
                    {
                        return;
                    }
                    //Create and save appearance, which is required to render the item.
                    int? childItemTypeId = ItemConfigurationManager.GetTypeIdFromName(itemProps.ItemTypeName);

                    if (!childItemTypeId.HasValue)
                    {
                        return;
                    }
                    string childTypeDefaultAppearanceCode = AppearanceDataManager.GetDefaultAppearanceCodeForType(childItemTypeId.Value);

                    if (Utilities.IsNullOrEmpty(childTypeDefaultAppearanceCode))
                    {
                        return;
                    }

                    ////Get matrix-specific version of item appearance for everything except message items.
                    //if (!childTypeDefaultAppearanceCode.Equals("MESSAGE", StringComparison.InvariantCultureIgnoreCase))
                    //{
                    childTypeDefaultAppearanceCode = "MATRIX_" + childTypeDefaultAppearanceCode;
                    //}

                    AppearanceData childAppearanceData = AppearanceDataManager.GetAppearanceDataForCode(childTypeDefaultAppearanceCode);

                    if (childAppearanceData != null)
                    {
                        childAppearanceData.Save(itemProps.ItemId);
                    }
                }
                
                //Update row alias if necessary.
                if (reason == InvalidChildReason.AliasIsNotUpToDate)
                {
                    ChildItemProperties itemProps = _matrixChildren[itemCoordinate];
                    RowProperties rowProps = _rowProperties[rowNumber - 1];
                    ItemData itemData = ItemConfigurationManager.GetConfigurationData(itemProps.ItemId);
                    itemData.Alias = rowProps.Alias;                    
                    itemData.Save();
                }
            }
        }







        /// <summary>
        /// Create child data for the matrix
        /// </summary>
        /// <param name="itemCoordinate"></param>
        /// <returns></returns>
        private ChildItemProperties CreateChildItemData(TableCoordinate itemCoordinate)
        {
            if (itemCoordinate.Column == PrimaryKeyColumnIndex)
            {
                return CreateRowHeaderItemData(itemCoordinate);
            }

            return CreateAbstractChildItemData(itemCoordinate);
        }

        /// <summary>
        /// Create row header data
        /// </summary>
        /// <param name="itemCoordinate"></param>
        /// <returns></returns>
        private ChildItemProperties CreateRowHeaderItemData(TableCoordinate itemCoordinate)
        {
            if (itemCoordinate.Row < 1 || itemCoordinate.Row > _rowProperties.Count)
            {
                return null;
            }

            string rowItemType = _rowProperties[itemCoordinate.Row - 1].IsOther
                            ? "SingleLineText"
                            : "Message";

            ItemData rowItemData = ItemConfigurationManager.CreateConfigurationData(rowItemType);

            if (rowItemData != null)
            {
                rowItemData.Alias = _rowProperties[itemCoordinate.Row - 1].Alias;
                rowItemData.Save();

                if (rowItemData.ID.HasValue
                    && rowItemData.ID.Value > 0)
                {
                    //Create and save appearance, which is required to render the item.
                    AppearanceData itemAppearanceData = AppearanceDataManager.GetDefaultAppearanceDataForType(rowItemData.ItemTypeID);
                    itemAppearanceData =
                        AppearanceDataManager.GetAppearanceDataForCode("MATRIX_" + itemAppearanceData.AppearanceCode);

                    //Save appearance and associate with item)
                    if (itemAppearanceData != null)
                    {
                        itemAppearanceData.Save(rowItemData.ID.Value);
                    }

                    return new ChildItemProperties
                    {
                        ItemTypeName = rowItemType,
                        ItemId = rowItemData.ID.Value
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Create data for non-row-header child
        /// </summary>
        /// <param name="itemCoordinate"></param>
        /// <returns></returns>
        private ChildItemProperties CreateAbstractChildItemData(TableCoordinate itemCoordinate)
        {
            if (itemCoordinate.Column > 0 && itemCoordinate.Column <= ColumnCount)
            {
                string itemTypeName = _columnProperties[itemCoordinate.Column - 1].ItemTypeName;

                //Insert an item
                int newItemId = ItemConfigurationManager.InsertAbstractItem(
                    itemTypeName,
                    string.Empty,
                    true,
                    CreatedBy,
                    null);

                //If success, add to items collection
                if (newItemId > 0)
                {
                    return new ChildItemProperties
                    {
                        ItemTypeName = itemTypeName,
                        ItemId = newItemId
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Item coordinate
        /// </summary>
        /// <param name="itemCoordinate"></param>
        private void DeleteChildItemData(TableCoordinate itemCoordinate)
        {
            if (itemCoordinate.Column == PrimaryKeyColumnIndex)
            {
                DeleteRowHeaderItemData(itemCoordinate);
            }
            else
            {
                DeleteAbstractItemData(itemCoordinate);
            }
        }

        /// <summary>
        /// Delete item data a row header item.
        /// </summary>
        private void DeleteRowHeaderItemData(TableCoordinate itemCoordinate)
        {
            if (_matrixChildren.ContainsKey(itemCoordinate))
            {
                ItemData headerItemData = ItemConfigurationManager.GetConfigurationData(_matrixChildren[itemCoordinate].ItemId);

                if (headerItemData != null)
                {
                    headerItemData.Delete();
                }
            }
        }

        /// <summary>
        /// Delete data for a child that is not a row header.
        /// </summary>
        private void DeleteAbstractItemData(TableCoordinate itemCoordinate)
        {
            if (_matrixChildren.ContainsKey(itemCoordinate))
            {
                ItemConfigurationManager.DeleteAbstractItem(
                    _matrixChildren[itemCoordinate].ItemId,
                    null);
            }
        }

        /// <summary>
        /// Verify a matrix child item
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="rowNumber"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        private bool VerifyChildItem(int columnNumber, int rowNumber, out InvalidChildReason reason)
        {
            var itemCoordinate = new TableCoordinate(rowNumber, columnNumber);

            //If item is pk item, make sure it is correct type
            if (columnNumber == PrimaryKeyColumnIndex)
            {
                if (!_matrixChildren.ContainsKey(itemCoordinate))
                {
                    reason = InvalidChildReason.NoRowHeader;
                    return false;
                }

                if (rowNumber > 0 && rowNumber <= _rowProperties.Count)
                {
                    RowProperties rowProps = _rowProperties[rowNumber - 1];
                    ChildItemProperties childProps = _matrixChildren[itemCoordinate];

                    //If row is other and child is not a text input, child needs to be updated.
                    if (rowProps.IsOther
                        && !childProps.ItemTypeName.Equals("SingleLineText", StringComparison.InvariantCultureIgnoreCase)
                        && !childProps.ItemTypeName.Equals("MultiLineText", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reason = InvalidChildReason.RowHeaderWrongType;
                        return false;
                    }

                    //Similarly, if child is a text input and row is not an "other" row, child needs to be updated.
                    //If row is other and child is not a text input, child needs to be updated.
                    if (!rowProps.IsOther
                        && (childProps.ItemTypeName.Equals("SingleLineText", StringComparison.InvariantCultureIgnoreCase)
                            || childProps.ItemTypeName.Equals("MultiLineText", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        reason = InvalidChildReason.RowHeaderWrongType;
                        return false;
                    }

                    //Ensure that the alias for the row is up to date.
                    ItemData itemData = ItemConfigurationManager.GetConfigurationData(childProps.ItemId);
                    if (itemData.Alias != rowProps.Alias)
                    {
                        reason = InvalidChildReason.AliasIsNotUpToDate;
                        return false;
                    }

                    //Ensure item has an appearance
                    AppearanceData rowHeaderAppearanceData = AppearanceDataManager.GetAppearanceDataForItem(childProps.ItemId);

                    if (rowHeaderAppearanceData == null)
                    {
                        reason = InvalidChildReason.NoRowHeaderAppearance;
                        return false;
                    }


                    //Ensure appearance is proper type
                    string defaultAppearanceCodeForType = GetChildDefaultAppearanceCode(childProps.ItemTypeName);

                    //Items in matrix have an appearance code that is the concatenation of "MATRIX_" and an item types default appearance.
                    if (Utilities.IsNotNullOrEmpty(defaultAppearanceCodeForType)
                        && !rowHeaderAppearanceData.AppearanceCode.Equals(defaultAppearanceCodeForType, StringComparison.InvariantCultureIgnoreCase))
                    {
                        reason = InvalidChildReason.RowHeaderWrongType;
                        return false;
                    }
                }
            }
            else
            {
                if (!_matrixChildren.ContainsKey(itemCoordinate))
                {
                    reason = InvalidChildReason.NoChild;
                    return false;
                }

                //For normal items, compare child item type to column prototype type
                if (columnNumber > 0 && columnNumber <= _columnProperties.Count)
                {
                    if (!_columnProperties[columnNumber - 1].ItemTypeName.Equals(
                        _matrixChildren[itemCoordinate].ItemTypeName,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        reason = InvalidChildReason.ChildWrongType;
                        return false;
                    }
                }
            }

            //No problems
            reason = InvalidChildReason.NoReason;
            return true;
        }

        /// <summary>
        /// Get default appearance code for items with same type as child.
        /// </summary>
        /// <param name="childItemTypeName"></param>
        /// <returns></returns>
        private static string GetChildDefaultAppearanceCode(string childItemTypeName)
        {
            int? childTypeId = ItemConfigurationManager.GetTypeIdFromName(childItemTypeName);

            if (childTypeId.HasValue)
            {
                string defaultAppearanceCodeForType = AppearanceDataManager.GetDefaultAppearanceCodeForType(childTypeId.Value);

                //Items in matrix have an appearance code that is the concatenation of "MATRIX_" and an item types default appearance.
                if (Utilities.IsNotNullOrEmpty(defaultAppearanceCodeForType))
                {
                    return "MATRIX_" + defaultAppearanceCodeForType;
                }
            }

            return string.Empty;
        }

        #endregion


        #region Row Maintenance

        /// <summary>
        /// Add a row to the matrix.  Row will be added as the last row of the matrix
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="rowType"></param>
        public void AddRow(string rowType, string alias)
        {
            if (Utilities.IsNullOrEmpty(rowType))
            {
                throw new Exception("Type of row to add must be specified.");
            }

            _rowProperties.Add(new RowProperties
            {
                Alias = alias,
                IsOther = rowType.Equals("Other", StringComparison.InvariantCultureIgnoreCase),
                IsRowSubheading = rowType.Equals("Subheading", StringComparison.InvariantCultureIgnoreCase)
            });

            VerifyAndUpdateRowItems(RowCount);
        }

        /// <summary>
        /// Update an existing row
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="rowNumber"></param>
        /// <param name="rowType"></param>
        public void UpdateRow(int rowNumber, string alias, string rowType)
        {
            if (rowNumber < 0
                || rowNumber > RowCount)
            {
                return;
            }

            //Insert entry for row properties
            _rowProperties[rowNumber - 1] = new RowProperties
            {
                Alias = alias,
                IsOther = rowType.Equals("Other", StringComparison.InvariantCultureIgnoreCase),
                IsRowSubheading = rowType.Equals("Subheading", StringComparison.InvariantCultureIgnoreCase)
            };

            //Ensure child items have proper types, exist, etc.
            VerifyAndUpdateRowItems(rowNumber);
        }

        /// <summary>
        /// Remove a row at the specified location
        /// </summary>
        /// <param name="rowNumber"></param>
        public void RemoveRow(int rowNumber)
        {
            ValidateRowPosition(rowNumber);

            //Step 1: Remove row data
            _rowProperties.RemoveAt(rowNumber - 1);

            //Step 2: Remove children
            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                var itemCoordinate = new TableCoordinate(rowNumber, columnNumber);

                if (_matrixChildren.ContainsKey(itemCoordinate))
                {
                    _deletedChildrenItems.Add(_matrixChildren[itemCoordinate].ItemId);
                    _matrixChildren.Remove(itemCoordinate);
                }
            }

            //Step 3: Update row positions for child items in rows following this row
            //        Loop w/RowCount + 1 because we need to account
            //        for the fact that a row was removed above.
            for (int row = rowNumber + 1; row <= RowCount + 1; row++)
            {
                //Update child item info
                for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
                {
                    var itemCoordinate = new TableCoordinate(row, columnNumber);

                    if (_matrixChildren.ContainsKey(itemCoordinate))
                    {
                        //Move item "up" a row.
                        _matrixChildren[new TableCoordinate(row - 1, columnNumber)] = _matrixChildren[itemCoordinate];
                        _matrixChildren.Remove(itemCoordinate);
                    }
                }
            }
        }


        /// <summary>
        /// Move a row
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="newPosition"></param>
        public void MoveRow(Int32 currentPosition, Int32 newPosition)
        {
            if (currentPosition == newPosition)
            {
                return;
            }

            //Whem moving rows, child items in rows between and including old position and new position need to have positions
            // updated.
            ValidateRowPosition(currentPosition);
            ValidateRowPosition(newPosition);


            //1. Move row property list element and account for fact that row position is rowIndex in list + 1
            Utilities.MoveListElement(_rowProperties, currentPosition - 1, newPosition - 1);

            //2. Store children of the row for eventual move to new row
            var rowChildren = new List<ChildItemProperties>();

            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                var childCoordinate = new TableCoordinate(currentPosition, columnNumber);

                if (_matrixChildren.ContainsKey(childCoordinate))
                {
                    rowChildren.Add(_matrixChildren[childCoordinate]);
                }
            }

            //3. Move children of rows between new row and old row.  Movement will either be up or down one row
            //   depending on whether new row position > old row position.
            MoveRowChildren(currentPosition, newPosition);


            //4. Now place child items from row to move at new row
            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                if (rowChildren.Count >= columnNumber)
                {
                    _matrixChildren[new TableCoordinate(newPosition, columnNumber)] = rowChildren[columnNumber - 1];
                }
            }
        }

        /// <summary>
        /// Move matrix row children to new row.
        /// </summary>
        /// <param name="currentRowNumber"></param>
        /// <param name="newRowNumber"></param>
        private void MoveRowChildren(int currentRowNumber, int newRowNumber)
        {
            //If moving "down" (to a higher index)
            if (newRowNumber > currentRowNumber)
            {
                for (int rowNumber = currentRowNumber; rowNumber < newRowNumber; rowNumber++)
                {
                    //Move rows between old position and new position "up" by decrementing row numbers
                    //Update properties dictionary
                    MoveRowChildrenOneRow(rowNumber, true);
                }
            }

            //If moving "up" (to a lower index)
            if (newRowNumber < currentRowNumber)
            {
                for (int rowNumber = currentRowNumber; rowNumber > newRowNumber; rowNumber--)
                {
                    MoveRowChildrenOneRow(rowNumber, false);
                }
            }
        }

        /// <summary>
        /// Move children of the specified row up or down one row.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="moveUp"></param>
        private void MoveRowChildrenOneRow(int rowNumber, bool moveUp)
        {
            //Also track change for items in the row
            for (int columnNumber = 1; columnNumber <= ColumnCount; columnNumber++)
            {
                //Move up means that row numbers should be decremented
                TableCoordinate itemCoordinate = moveUp
                ? new TableCoordinate(rowNumber + 1, columnNumber)       //n + 1 = item on next row
                : new TableCoordinate(rowNumber - 1, columnNumber);      //n - 1 = item on previous row

                if (_matrixChildren.ContainsKey(itemCoordinate))
                {
                    _matrixChildren[new TableCoordinate(rowNumber, columnNumber)] = _matrixChildren[itemCoordinate];
                }
            }
        }

        #endregion

        #region Column Maintenance

        /// <summary>
        /// Add a column to the matrix using the specified item as the prototype.  Column is added as last (rightmost) column.
        /// </summary>
        /// <param name="prototypeItemId"></param>
        /// <param name="itemTypeName"></param>
        /// <param name="requiresUniqueAnswers"></param>
        /// <param name="width"></param>
        public void AddColumn(int prototypeItemId, string itemTypeName, bool requiresUniqueAnswers, int? width)
        {
            //Validate inputs
            if (prototypeItemId <= 0)
            {
                throw new Exception("Prototype item id must be specified for column.");
            }

            if (Utilities.IsNullOrEmpty(itemTypeName))
            {
                throw new Exception("Type name of column prototype must be specified when adding column.");
            }

            _columnProperties.Add(new ColumnProperties
            {
                ItemTypeName = itemTypeName,
                PrototypeItemId = prototypeItemId,
                RequiresUniqueAnswers = requiresUniqueAnswers,
                Width = width
            });

            //Ensure child items have proper types, exist, etc.
            VerifyAndUpdateColumnItems(ColumnCount);
        }

        /// <summary>
        /// Update a column's information
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="prototypeItemId"></param>
        /// <param name="itemTypeName"></param>
        /// <param name="requiresUniqueAnswers"></param>
        /// <param name="width"></param>
        public void UpdateColumn(int columnNumber, int prototypeItemId, string itemTypeName, bool requiresUniqueAnswers, int? width)
        {
            if (columnNumber < 1
                || columnNumber > ColumnCount)
            {
                return;
            }

            //Insert entry for column properties
            _columnProperties[columnNumber - 1] = new ColumnProperties
            {
                ItemTypeName = itemTypeName,
                PrototypeItemId = prototypeItemId,
                RequiresUniqueAnswers = requiresUniqueAnswers,
                Width = width
            };

            //Ensure child items have proper types, exist, etc.
            VerifyAndUpdateColumnItems(columnNumber);
        }

        /// <summary>
        /// Remove the column with the specified position
        /// </summary>
        /// <param name="columnNumber"></param>
        public void RemoveColumn(int columnNumber)
        {
            ValidateColumnPosition(columnNumber);

            //Step 1: Remove row data
            _columnProperties.RemoveAt(columnNumber - 1);
            UpdatePrimaryKeyColumnIndex();

            //Step 2: Remove children
            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                var itemCoordinate = new TableCoordinate(rowNumber, columnNumber);

                if (_matrixChildren.ContainsKey(itemCoordinate))
                {
                    _deletedChildrenItems.Add(_matrixChildren[itemCoordinate].ItemId);
                    _matrixChildren.Remove(itemCoordinate);
                }
            }

            //Step 3: Update column positions for child items in rows following this column
            //        Loop w/ColumnCount + 1 because we need to account
            //        for the fact that a column was removed in the above code.
            for (int column = columnNumber + 1; column <= ColumnCount + 1; column++)
            {
                //Update child item info
                for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
                {
                    var itemCoordinate = new TableCoordinate(rowNumber, column);

                    if (_matrixChildren.ContainsKey(itemCoordinate))
                    {
                        //Move item "left" a column
                        _matrixChildren[new TableCoordinate(rowNumber, column - 1)] = _matrixChildren[itemCoordinate];
                        _matrixChildren.Remove(itemCoordinate);
                    }
                }
            }
        }


        /// <summary>
        /// Update primaryKeyColumnIndex if it was changed
        /// </summary>
        private void UpdatePrimaryKeyColumnIndex()
        {
            if (PrimaryKeyColumnIndex > _columnProperties.Count || !String.Equals(_columnProperties[PrimaryKeyColumnIndex - 1].ItemTypeName, "PrimaryKey", StringComparison.InvariantCultureIgnoreCase))
            {
                for (int i = 0; i < _columnProperties.Count; i++)
                {
                    ColumnProperties columnProperty = _columnProperties[i];
                    if (String.Equals(columnProperty.ItemTypeName, "PrimaryKey", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PrimaryKeyColumnIndex = i + 1;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Set order of rows as function of current positions.  For example, in a 5 row matrix, passing order of
        /// 1,2,3,5,4 would have effect of swapping last two row positions.
        /// </summary>
        /// <param name="rows"></param>
        public void SetRowOrder(List<int> rows)
        {
            //Populate list of current row positions. We'll update this list so we can
            // easily tell where to move row
            var currentRowList = new List<int>();

            for (int i = 1; i <= RowCount; i++)
            {
                currentRowList.Add(i);
            }


            //Now handle moving rows
            for (int i = 1; i <= rows.Count; i++)
            {
                var rowToMove = rows[i - 1];

                //Locate current position of this row, which may have changed due to movement of 
                // other rows.
                var currentRowPosition = currentRowList.IndexOf(rowToMove) + 1;


                //Since items in rows array are in order, new row position is i
                MoveRow(currentRowPosition, i);

                //Update current row list.  Account for fact that row positions are 1-based, but
                // indexes for list elements are 0-based.
                Utilities.MoveListElement(currentRowList, currentRowPosition - 1, i - 1);
            }
        }

        /// <summary>
        /// Move a column
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="newPosition"></param>
        public void MoveColumn(Int32 currentPosition, Int32 newPosition)
        {
            if (currentPosition == newPosition)
            {
                return;
            }

            //Whem moving rows, child items in rows between and including old position and new position need to have positions
            // updated.
            ValidateColumnPosition(currentPosition);
            ValidateColumnPosition(newPosition);

            //1. Move row property list element and account for fact that row position is rowIndex in list + 1
            Utilities.MoveListElement(_columnProperties, currentPosition - 1, newPosition - 1);
            UpdatePrimaryKeyColumnIndex();

            //2. Store children of the column for eventual move to new column
            var columnChildren = new List<ChildItemProperties>();

            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                var childCoordinate = new TableCoordinate(rowNumber, currentPosition);

                if (_matrixChildren.ContainsKey(childCoordinate))
                {
                    columnChildren.Add(_matrixChildren[childCoordinate]);
                }
            }

            //3. Move children of rows between new row and old row.  Movement will either be up or down one row
            //   depending on whether new row position > old row position.
            MoveColumnChildren(currentPosition, newPosition);

            ////4. Now place child items from row to move at new row
            //for (int rowNumber = 1; rowNumber <= ColumnCount; rowNumber++)
            //{
            //    if (columnChildren.Count >= rowNumber)
            //    {
            //        _matrixChildren[new TableCoordinate(rowNumber, currentPosition)] = columnChildren[rowNumber - 1];
            //    }
            //}
        }

        /// <summary>
        /// Move matrix column children to another column
        /// </summary>
        /// <param name="currentColumnNumber"></param>
        /// <param name="newColumnNumber"></param>
        private void MoveColumnChildren(int currentColumnNumber, int newColumnNumber)
        {
            //If moving "down" (to a higher index)
            if (newColumnNumber > currentColumnNumber)
            {
                for (int columnNumber = currentColumnNumber; columnNumber < newColumnNumber; columnNumber++)
                {
                    //Move columns between old position and new position "left" by decrementing column numbers
                    //Update properties dictionary
                    MoveColumnChildrenOneRow(columnNumber, true);
                }
            }

            //If moving "up" (to a lower index)
            if (newColumnNumber < currentColumnNumber)
            {
                for (int columnNumber = currentColumnNumber; columnNumber > newColumnNumber; columnNumber--)
                {
                    MoveColumnChildrenOneRow(columnNumber, false);
                }
            }
        }

        /// <summary>
        /// Move children of the specified column left or right one column.
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <param name="moveLeft"></param>
        private void MoveColumnChildrenOneRow(int columnNumber, bool moveLeft)
        {
            for (int rowNumber = 1; rowNumber <= RowCount; rowNumber++)
            {
                //Move up means that row numbers should be decremented
                TableCoordinate itemCoordinate = moveLeft
                ? new TableCoordinate(rowNumber, columnNumber + 1)       //n + 1 = item on next row
                : new TableCoordinate(rowNumber, columnNumber - 1);      //n - 1 = item on previous row

                if (_matrixChildren.ContainsKey(itemCoordinate))
                {
                    ChildItemProperties temp = _matrixChildren[new TableCoordinate(rowNumber, columnNumber)];
                    _matrixChildren[new TableCoordinate(rowNumber, columnNumber)] = _matrixChildren[itemCoordinate];
                    _matrixChildren[itemCoordinate] = temp;
                }
            }
        }


        /// <summary>
        /// Set whether a particular column should be unique
        /// </summary>
        /// <param name="column"></param>
        /// <param name="unique"></param>
        public void SetColumnUniqueness(Int32 column, bool unique)
        {
            ValidateColumnPosition(column);

            _columnProperties[column - 1].RequiresUniqueAnswers = unique;
        }
        /// <summary>
        /// Get whether a particular column requires unique answers
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool GetColumnUniqueness(Int32 column)
        {
            ValidateColumnPosition(column);

            return _columnProperties[column - 1].RequiresUniqueAnswers;
        }

        /// <summary>
        /// Set the width of the column, in pixels
        /// </summary>
        /// <param name="column"></param>
        /// <param name="width"></param>
        public void SetColumnWidth(Int32 column, int? width)
        {
            ValidateColumnPosition(column);

            _columnProperties[column - 1].Width = width;
        }
        /// <summary>
        /// Get the width of the column, in pixels
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public int? GetColumnWidth(Int32 column)
        {
            ValidateColumnPosition(column);

            return _columnProperties[column - 1].Width;
        }

        #endregion


        #region Create Item

        /// <summary>
        /// Create an instance of a matrix item based on this item type
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new MatrixItem();
        }

        #endregion

        /// <summary>
        /// Get whether this represents an answerable item
        /// </summary>
        public override bool ItemIsIAnswerable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Create a text decorator for an item
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override ItemTextDecorator CreateTextDecorator(string languageCode)
        {
            return new MatrixItemTextDecorator(this, languageCode);
        }


        /// <summary>
        /// ICompositeItemData implementation of get child item data ids
        /// </summary>
        /// <returns></returns>
        public override ReadOnlyCollection<int> GetChildItemDataIDs()
        {
            List<Int32> childItemIDs = _matrixChildren.Values.Select(itemProp => itemProp.ItemId).ToList();

            return new ReadOnlyCollection<int>(childItemIDs);
        }


        /// <summary>
        /// Returns the clone of the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var theClone = (MatrixItemData)base.Copy();

            theClone._columnProperties.Clear();
            foreach (ColumnProperties columnPropertiese in _columnProperties)
            {
                ColumnProperties newProp = new ColumnProperties
                                               {
                                                   ItemTypeName = columnPropertiese.ItemTypeName,
                                                   PrototypeItemId = columnPropertiese.PrototypeItemId,
                                                   RequiresUniqueAnswers = columnPropertiese.RequiresUniqueAnswers,
                                                   Width = columnPropertiese.Width
                                               };
                theClone._columnProperties.Add(newProp);
            }

            foreach (KeyValuePair<TableCoordinate, ChildItemProperties> childItemPropertiese in _matrixChildren)
            {
                ChildItemProperties newProp = new ChildItemProperties
                                                  {
                                                      ItemId = childItemPropertiese.Value.ItemId,
                                                      ItemTypeName = childItemPropertiese.Value.ItemTypeName
                                                  };
                theClone._matrixChildren.Add(childItemPropertiese.Key, newProp);
            }

            foreach (RowProperties rowPropertiese in _rowProperties)
            {
                RowProperties newProp = new RowProperties
                                            {
                                                Alias = rowPropertiese.Alias,
                                                IsOther = rowPropertiese.IsOther,
                                                IsRowSubheading = rowPropertiese.IsRowSubheading
                                            };
                theClone._rowProperties.Add(newProp);
            }

            if (CreatedDate.HasValue)
                theClone.CreatedDate = new DateTime(CreatedDate.Value.Ticks);
            if (LastModified.HasValue)
                theClone.LastModified = new DateTime(LastModified.Value.Ticks);

            theClone.IsRequired = IsRequired;
            theClone.Alias = Alias;
            theClone.ID = ID;
            theClone.IsActive = IsActive;
            theClone.ItemTypeID = ItemTypeID;
            theClone.ItemTypeName = ItemTypeName;
            theClone.PrimaryKeyColumnIndex = PrimaryKeyColumnIndex;
            return theClone;
        }

        internal string GetRowColumnAlias(int row, int col)
        {
            int protoID = GetColumnPrototypeId(col);
            ItemData data = ItemConfigurationManager.GetConfigurationData(protoID);

            string rowAlias = GetRowAlias(row);
            if (string.IsNullOrEmpty(rowAlias))
                rowAlias = "row" + row.ToString();

            string colAlias = data == null ? ("col" + col.ToString()) : data.Alias;

            return rowAlias + "__" + colAlias;
        }


        /// <summary>
        /// Get an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        internal LightweightItemMetaData GetLightweightItem(int itemId)
        {
            //Otherwise, load lightweight item data
            LightweightItemMetaData itemMetaData = SurveyMetaDataProxy.GetItemData(itemId, false);

            return itemMetaData;
        }

        internal ItemData GetItemAt(int row, int col)
        {
            int? itemId = GetItemIdAt(row, col);

            if (!itemId.HasValue)
                return null;

            return ItemConfigurationManager.GetConfigurationData(itemId.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="rules"></param>
        /// <param name="itemTable"></param>
        /// <param name="options"></param>
        /// <param name="itemConditions"></param>
        public void Load(XmlNode xmlNode, RuleDataService rules, Dictionary<int, ItemData> itemTable, Dictionary<int, ListOptionData> options, List<RuleData> itemConditions, object creator)
        {
            //Load common, columns, and rows
            base.Import(xmlNode, null, creator);
            
            //TODO: Load Row Conditions
            /*
            
            int childCount = reader.ReadAttributeInt(0);

            for (int itemIndex = 0; itemIndex < childCount; itemIndex++)
            {
                reader.EnsureElement("Item", true, true);

                int oldId = reader.ReadAttributeInt(0);
                int typeId = reader.ReadAttributeInt(1);
                int row = reader.ReadAttributeInt(2);
                int column = reader.ReadAttributeInt(3);

                ChildItemProperties properties = _matrixChildren[new TableCoordinate(row, column)];

                ItemData item = ItemConfigurationManager.GetConfigurationData(properties.ItemId);

                if (item.ItemTypeID != typeId)
                    throw new Exception("Type mismatch error while importing the matrix.");

                item.ReadXml(reader);

                item.Save();

                itemTable.Add(oldId, item);

                AppearanceData appearanceData = AppearanceDataManager.GetAppearanceDataForItem(item.ID.Value);

                //Ensure that AppearanceCode starts with "MATRIX_" or "CATEGORIZED". if not - add this prefix.
                if (!appearanceData.AppearanceCode.StartsWith("MATRIX_") && !appearanceData.AppearanceCode.StartsWith("CATEGORIZED"))
                {
                    AppearanceDataManager.UpdateAppearanceCode(appearanceData.ID.Value, GetChildDefaultAppearanceCode(item.ItemTypeName));
                    appearanceData.Save(item.ID.Value);
                }

                reader.EnsureElement("ItemCondition", true, true);

                if (!reader.IsEmptyElement)
                {
                    reader.MoveToNextElement(null);
                    RuleData itemCond = rules.GetConditionForItem(item.ID.Value, true, true);
                    itemCond.ReadXml(reader);
                    itemConditions.Add(itemCond);
                }

                //Store options for select item
                if (item is SelectItemData)
                {
                    foreach (var option in ((SelectItemData)item).Options)
                    {
                        options[option.OptionID] = option;
                    }
                }

            }// item

            */
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool ItemIsIScored
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="rules"></param>
        public void WriteXml(XmlWriter writer, RuleDataService rules)
        {
            base.WriteXml(writer);

            //TODO: Item Conditions

            /*
            WriteCommonXml(writer);
            WriteColumnData(writer);

            WriteRowData(writer);

            writer.WriteStartElement("Items");
            writer.WriteAttributeString("Count", _matrixChildren.Count.ToString());

            foreach (KeyValuePair<TableCoordinate, ChildItemProperties> pair in _matrixChildren)
            {
                ItemData item = ItemConfigurationManager.GetConfigurationData(pair.Value.ItemId);

                if (item == null)
                    continue;

                writer.WriteStartElement("Item");
                writer.WriteAttributeString("Id", pair.Value.ItemId.ToString());
                writer.WriteAttributeString("TypeId", item.ItemTypeID.ToString());
                writer.WriteAttributeString("Row", pair.Key.Row.ToString());
                writer.WriteAttributeString("Column", pair.Key.Column.ToString());

                item.WriteXml(writer);

                RuleData itemCondition = rules.GetConditionForItem(item.ID.Value, false, false);

                writer.WriteStartElement("ItemCondition");

                if (itemCondition != null)
                    itemCondition.WriteXml(writer);
                //else writer.WriteNull();

                writer.WriteEndElement(); // ItemCondition

                writer.WriteEndElement(); // Item
            }

            writer.WriteEndElement(); // Items
            */
        }
    }
}
