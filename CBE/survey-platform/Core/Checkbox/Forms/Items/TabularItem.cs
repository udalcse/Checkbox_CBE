using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Checkbox.Forms.Items.Configuration;

using Prezza.Framework.Common;


namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Represents a composite item that has a grid of child items
    /// </summary>
    [Serializable]
    public class TabularItem : LabelledItem, ICompositeItem, IScored
    {
        private Dictionary<Coordinate, Item> _childItems;

        #region Configure

        /// <summary>
        /// Configure the item with its configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(TabularItemData));
            TabularItemData config = (TabularItemData)configuration;

            base.Configure(configuration, languageCode, templateId);

            SetRowCount(config);
            SetColumnCount(config);
            CreateChildItemsDictionary();
            CreateChildItems(config, languageCode);
        }

        /// <summary>
        /// Set the row count
        /// </summary>
        /// <param name="config"></param>
        protected virtual void SetRowCount(TabularItemData config)
        {
            RowCount = config.RowCount;
        }

        /// <summary>
        /// Set the column count
        /// </summary>
        /// <param name="config"></param>
        protected virtual void SetColumnCount(TabularItemData config)
        {
            ColumnCount = config.ColumnCount;
        }

        /// <summary>
        /// Create the dictionary to hold child items
        /// </summary>
        protected virtual void CreateChildItemsDictionary()
        {
            _childItems = new Dictionary<Coordinate, Item>(new CoordinateComparer());
        }

        /// <summary>
        /// Create child items
        /// <remarks>
        /// Overriding this method requires you also override GeChildItemCoordinate(ItemData child) as it depends on the 
        /// particular Dictionary key encoding performed in this explicit implementation TabularItem.CreateChildItems
        /// </remarks>
        /// </summary>
        /// <param name="config"></param>
        /// <param name="languageCode"></param>
        protected virtual void CreateChildItems(TabularItemData config, string languageCode)
        {
            //Instantiate child items
            for (int i = 1; i <= RowCount; i++)
            {
                for (int j = 1; j <= ColumnCount; j++)
                {
                    int? itemId = config.GetItemIdAt(i, j);

                    if (itemId.HasValue)
                    {
                        ItemData data = ItemConfigurationManager.GetConfigurationData(itemId.Value);

                        if (data != null)
                        {
                            //Create item, configure, set id, and add to child item dictionary
                            Item item = data.CreateItem(languageCode, TemplateID);
                            item.Configure(data, languageCode, TemplateID);
                            item.Parent = this;
                            item.ID = itemId.Value;

                            SetChildItemInDictionary(new Coordinate(j, i), item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the coordinate for child item
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        protected virtual Coordinate GetChildItemCoordinate(int childId)
        {
            List<Coordinate> coordinates = new List<Coordinate>(from entry in _childItems where entry.Value.ID == childId select entry.Key);

            if (coordinates.Count > 0)
            {
                return coordinates[0];
            }
            return null;
        }

        /// <summary>
        /// Set the reference to the child item in the child items dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="childItem"></param>
        protected virtual void SetChildItemInDictionary(Coordinate key, Item childItem)
        {
            _childItems[key] = childItem;
            childItem.ItemExcluded += childItem_ItemExcluded;
            childItem.ItemIncluded += childItem_ItemIncluded;
        }

        /// <summary>
        /// Handle a child item being excluded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void childItem_ItemExcluded(object sender, EventArgs e)
        {
            OnChildItemExcluded((Item)sender);
        }

        /// <summary>
        /// Handle a child item being included
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void childItem_ItemIncluded(object sender, EventArgs e)
        {
            OnChildItemIncluded((Item)sender);
        }

        /// <summary>
        /// Overridable handler for a child being excluded
        /// </summary>
        /// <param name="child"></param>
        protected virtual void OnChildItemExcluded(Item child)
        {
            if (child is IAnswerable)
            {
                ((IAnswerable)child).DeleteAnswers();
            }
        }

        /// <summary>
        /// Overridable handler for a child being included
        /// </summary>
        /// <param name="child"></param>
        protected virtual void OnChildItemIncluded(Item child)
        {
        }

        /// <summary>
        /// Get the child item from the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual Item GetChildItemFromDictionary(Coordinate key)
        {
            if (_childItems.ContainsKey(key))
            {
                return _childItems[key];
            }

            return null;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Get/set the number of rows for this item
        /// </summary>
        public virtual int RowCount { get; set; }

        /// <summary>
        /// Get/set the number of columns for this item
        /// </summary>
        public virtual int ColumnCount { get; set; }

        #endregion

        #region Answers

        /// <summary>
        /// Get whether the item has been answered
        /// </summary>
        public override bool HasAnswer
        {
            get
            {
                foreach (Item item in Items)
                {
                    if (item != null && item is IAnswerable && ((IAnswerable)item).HasAnswer)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Delete the child answers
        /// </summary>
        public override void DeleteAnswers()
        {
            foreach (Item item in Items)
            {
                if (item is IAnswerable)
                {
                    ((IAnswerable)item).DeleteAnswers();
                }
            }
        }

        /// <summary>
        /// Validate answers
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            bool valid = true;

            foreach (Item item in Items)
            {
                if (item is ResponseItem)
                {
                    ((ResponseItem)item).Validate();

                    if (!((ResponseItem)item).Valid)
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        #endregion

        /// <summary>
        /// Get the item at the specified index
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public virtual Item GetItemAt(Int32 row, Int32 column)
        {
            return GetChildItemFromDictionary(new Coordinate(column, row));
        }

        ///<summary>
        ///</summary>
        ///<param name="id"></param>
        ///<returns></returns>
        public Item GetItemWithID(int id)
        {
            Item child = null;

            foreach (Item c in _childItems.Values)
            {
                if (c.ID == id)
                {
                    child = c;
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Handle setting response to set response for child items
        /// </summary>
        protected override void OnResponseSet()
        {
            //Set the response for child items
            foreach (Item item in Items)
            {
                if (item != null && item is ResponseItem)
                {
                    ((ResponseItem)item).Response = Response;

                    if (item.ID > 0 && !Response.ContainsItem(item.ID))
                    {
                        Response.AddItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Set the answer data for the child items
        /// </summary>
        protected override void OnAnswerDataSet()
        {
            base.OnAnswerDataSet();

            foreach (AnswerableItem item in Items.OfType<AnswerableItem>())
            {
                item.AnswerData = AnswerData;
            }
        }

        #region ICompositeItem Members

        /// <summary>
        /// Get the children of this item
        /// </summary>
        public virtual ReadOnlyCollection<Item> Items
        {
            get
            {
                List<Item> items = new List<Item>();

                if (_childItems != null)
                    items.AddRange(_childItems.Values);

                return new ReadOnlyCollection<Item>(items);
            }
        }

        #endregion

        /// <summary>
        /// Dispose of child items
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            foreach (Item item in Items)
            {
                item.Dispose();
            }
        }

        #region IScored Members

        /// <summary>
        /// Get the score for the item, as the sum of the scores of all child items
        /// </summary>
        /// <returns></returns>
        public double GetScore()
        {
            double score = 0;

            foreach (Item item in Items)
            {
                if (item is IScored)
                {
                    score += ((IScored)item).GetScore();
                }
            }

            return score;
        }

        public double GetPossibleMaxScore()
        {
            double score = 0;

            foreach (Item item in Items)
            {
                if (!item.Excluded && item is IScored)
                {
                    score += ((IScored)item).GetPossibleMaxScore();
                }
            }

            return score;
        }

        #endregion

        #region XML Serialization

        /// <summary>
        /// Write instance data to XML
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            base.WriteXmlInstanceData(writer, isText);

            WriteTableInstanceData(writer);
        }

        /// <summary>
        /// Write data for items in the table
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteTableInstanceData(XmlWriter writer)
        {
            writer.WriteStartElement("rows");

            for (int i = 1; i <= RowCount; i++)
            {
                WriteRowInstanceData(i, writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write row instance data
        /// </summary>
        /// <param name="row"></param>
        /// <param name="writer"></param>
        protected virtual void WriteRowInstanceData(int row, XmlWriter writer)
        {

            writer.WriteStartElement("row");
            writer.WriteAttributeString("rowNumber", row.ToString());

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
        protected virtual void WriteColumnInstanceData(int row, int column, XmlWriter writer)
        {
            writer.WriteStartElement("column");
            writer.WriteAttributeString("columnNumber", column.ToString());

            Item item = GetItemAt(row, column);

            if (item != null)
            {
                item.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        /*

            writer.WriteStartElement("childInstanceData");
            writer.WriteStartElement("rows");

            //Iterate rows and columns to write answers
            for (int row = 1; row <= RowCount; row++)
            {
                writer.WriteStartElement("row");
                writer.WriteAttributeString("rowNumber", row.ToString());

                writer.WriteStartElement("columns");

                for (int column = 1; column <= ColumnCount; column++)
                {
                    writer.WriteStartElement("column");
                    writer.WriteAttributeString("columnNumber", column.ToString());

                    Item item = GetItemAt(row, column);

                    if (item != null)
                    {
                        item.WriteXmlInstanceData(writer);
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }*/

        #endregion
    }
}
