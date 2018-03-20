using System;
using System.Data;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Logic;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Callback delegate for getting an item's text.
    /// <yo/summary>
    /// <param name="itemId">Id of item to get text for.</param>
    /// <param name="languageCode">Language code for text to return.</param>
    /// <returns>Text of an item.</returns>
    /// <remarks>For user convenience, it is nice to display the item text with the item's position within a survey.  Since a
    /// <see cref="FilterData"/> object doesn't (and shouldn't) have knowledge of the survey containing the item, this is a
    /// useful way to get the item's position text in a way that conforms to the consumer's (often a UI element) desire.</remarks>
    public delegate string ItemFilterTextPrefixCallback(int itemId, string languageCode);

    /// <summary>
    /// Meta data and factory for item-based filters, which compare answers to survey items against a
    /// specified value, which may be open-ended text or the ID of an item option.
    /// </summary>
    [Serializable]
    public class ItemFilterData : FilterData
    {
        private ItemFilterTextPrefixCallback _itemTextCallback;

        /// <summary>
        /// Item filter
        /// </summary>
        public override string ObjectTypeName { get { return "ItemFilter"; } }

        /// <summary>
        /// Get name of load data sproc
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_Filter_GetItem"; } }

        /// <summary>
        /// Create a configuration data set
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new PersistedDomainObjectDataSet(ObjectTypeName, "ItemFilterData", "FilterID");
        }

        /// <summary>
        /// Get/set id of item to use for comparison.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Register a callback method for getting item text
        /// </summary>
        /// <param name="callback">Text callback method with signature matching the <see cref="ItemFilterTextPrefixCallback"/> delegate signature.</param>
        public void RegisterTextCallback(ItemFilterTextPrefixCallback callback)
        {
            _itemTextCallback = callback;
        }

        /// <summary>
        /// Clear the current text callback
        /// </summary>
        public void UnRegisterTextCallback()
        {
            _itemTextCallback = null;
        }

        /// <summary>
        /// Instantiate the filter object associated with this filter meta data.
        /// </summary>
        /// <returns>Instance of a <see cref="Filter"/> object configured with this object's meta data.</returns>
        /// <remarks>The exact type of <see cref="Filter"/> object may vary depending on the Operator for the filter.  Possible types are:
        /// <see cref="AnsweredFilter"/>, <see cref="EqualFilter"/>, <see cref="NotAnsweredFilter"/>, <see cref="NotEqualFilter"/>, or <see cref="ItemFilter"/>.</remarks>
        protected override Filter CreateFilterObject()
        {
            if (Operator == LogicalOperator.Answered)
            {
                return new AnsweredFilter();
            }

            if (Operator == LogicalOperator.Equal)
            {
                return new EqualFilter();
            }

            if (Operator == LogicalOperator.NotAnswered)
            {
                return new NotAnsweredFilter();
            }

            if (Operator == LogicalOperator.NotEqual)
            {
                return new NotEqualFilter();
            }

            if (Operator == LogicalOperator.Contains)
            {
                return new ContainsFilter();
            }

            if (Operator == LogicalOperator.LessThan)
            {
                return new LessFilter();
            }

            if (Operator == LogicalOperator.LessThanEqual)
            {
                return new LessEqualFilter();
            }

            if (Operator == LogicalOperator.GreaterThan)
            {
                return new GreaterFilter();
            }

            if (Operator == LogicalOperator.GreaterThanEqual)
            {
                return new GreaterEqualFilter();
            }

            if (Operator == LogicalOperator.DoesNotContain)
            {
                return new DoesntContainFilter();
            }


            return new ItemFilter();
        }

        ///// <summary>
        ///// Get the name of the <see cref="DataTable"/> in the filter data configuration <see cref="DataSet"/> containing the configuration information
        ///// for this type of filter.
        ///// </summary>
        //public override string DataTableName { get { return "ItemFilterData"; } }

        ///// <summary>
        ///// Get the name of the <see cref="DataColumn"/> in the filter data configuration <see cref="DataTable"/> containing the identities of 
        ///// this type of filter.
        ///// </summary>
        //public override string IdentityColumnName { get { return "FilterID"; } }

        /// <summary>
        /// Load the filter data from the specified <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"><see cref="DataRow"/> containing filter configuration.</param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            ItemId = DbUtility.GetValueFromDataRow(data, "ItemID", -1);
        }

        /// <summary>
        /// Update the configuration of the item filter.
        /// </summary>
        /// <param name="t">Transaction to participate in for the update.</param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_UpdateItem");

            command.AddInParameter("FilterID", DbType.Int32, ID);
            command.AddInParameter("ItemID", DbType.Int32, ItemId);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Insert the configuration of the item filter into the database.
        /// </summary>
        /// <param name="t">Transaction to participate in for the insertion.</param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_CreateItem");

            command.AddInParameter("FilterID", DbType.Int32, ID);
            command.AddInParameter("ItemID", DbType.Int32, ItemId);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Delete the configuration of the item filter.
        /// </summary>
        /// <param name="t">Transaction to participate in for the deletion.</param>
        public override void Delete(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_DeleteItem");

            command.AddInParameter("FilterID", DbType.Int32, ID);

            db.ExecuteNonQuery(command, t);

            base.Delete(t);
        }

        /// <summary>
        /// Get the left operand text for the filter. 
        /// </summary>
        /// <param name="languageCode">Language code to use for the text.</param>
        /// <returns>Localized text for filter left operand.</returns>
        /// <remarks>The operand text is the concatenation of the text prefix returned by the text prefix callback method and the 
        /// localized text of the item associated with this filter data.</remarks>
        protected override string GetFilterLeftOperandText(string languageCode)
        {
            string prefix = string.Empty;

            if (_itemTextCallback != null)
            {
                prefix = _itemTextCallback(ItemId, languageCode);
            }

            return string.Format(
                "{0}{1}",
                prefix,
                Utilities.StripHtml(SurveyMetaDataProxy.GetItemText(ItemId, languageCode, false, true), 128));

        }

        /// <summary>
        /// Get the text of the filter's right operand.
        /// </summary>
        /// <param name="languageCode">Language code for the filter operand.</param>
        /// <returns>Operand text.</returns>
        /// <remarks>The text returned will either be open-ended text or the text of the option associated with the right operand for the filter.</remarks>
        protected override string GetFilterRightOperandText(string languageCode)
        {
            if (Value == null)
            {
                return string.Empty;
            }

            int? valueAsInt = Utilities.AsInt(Value.ToString());

            //Now see if the value is an option of some sort
            if (valueAsInt.HasValue)
            {
                string optionString = Utilities.StripHtml(SurveyMetaDataProxy.GetOptionText(ItemId, valueAsInt.Value, languageCode, false, true), null);

                if (Utilities.IsNullOrEmpty(optionString))
                {
                    optionString = Value.ToString();
                }

                return optionString;
            }

            return Value.ToString();
        }
    }
}
