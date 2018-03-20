using System;
using System.Data;
using System.Text;

using Checkbox.Common;
using Checkbox.Forms.Logic;
using Checkbox.Globalization;
using Checkbox.Globalization.Text;

using Prezza.Framework.Data;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// Metadata container for report filter objects.  This container also acts as a factory for the <see cref="Filter"/> objects themselves, which
    /// contain the implementation of the filter logic itself.
    /// </summary>
    [Serializable]
    public abstract class FilterData : PersistedDomainObject, IEquatable<FilterData>
    {
        /// <summary>
        /// Get the filter type name.
        /// </summary>
        /// <remarks>Checkbox supports three filter types:
        ///   Item          --  Filter based on answers to a question.
        ///   Profile       --  Filter based on profile attributes of the respondent.
        ///   Response      --  Filter based on properties of the response.
        /// </remarks>
        public string FilterTypeName { get; private set; }

        /// <summary>
        /// Get/set <see cref="LogicalOperator"/> used by the filter for comparing values.
        /// </summary>
        public virtual LogicalOperator Operator { get; set; }

        /// <summary>
        /// Get/set value to use for comparison
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// Initialize the filter data object with the specified ID.
        /// </summary>
        /// <param name="filterId">ID of the filter.</param>
        /// <remarks>This method is used by the FilterFactory class to initialize instances of FilterData objects for EXISTING filters.</remarks>
        public void Initialize(int filterId)
        {
            ID = filterId;
        }

        /// <summary>
        /// Initialize the filter data object with the specified type name.
        /// </summary>
        /// <param name="filterTypeName">ID of the filter.</param>
        /// <remarks>This method is used by the FilterFactory class to initialize instances of FilterData objects for NEW filters.</remarks>
        public void Initialize(string filterTypeName)
        {
            FilterTypeName = filterTypeName;
        }
        /// <summary>
        /// Factory method to create an instance of a <see cref="Filter"/> object to do the work of evaluating filters.
        /// </summary>
        /// <param name="languageCode">Language code for the Filter to use when it's ToString() method is called.</param>
        /// <returns><see cref="Filter"/> object configured with this data object.</returns>
        public virtual Filter CreateFilter(string languageCode)
        {
            Filter f = CreateFilterObject();

            if (f != null)
            {
                f.Configure(this, languageCode);
            }

            return f;
        }

        /// <summary>
        /// Overridable method to create the actual instance of the <see cref="Filter"/> object this data class
        /// is associated with.
        /// </summary>
        /// <returns>Instance of a <see cref="Filter"/> object.</returns>
        protected abstract Filter CreateFilterObject();

        /// <summary>
        /// Get the left operand text for the filter.  If operand has no value
        /// an empty string should be returned.
        /// </summary>
        /// <param name="languageCode">Language code to use for localizing the operand text.</param>
        /// <returns>Text representation of the filter's left operand.</returns>
        protected abstract string GetFilterLeftOperandText(string languageCode);

        /// <summary>
        /// Get text for filter right operand.  If operand has no value, an empty
        /// string should be returned.
        /// </summary>
        /// <param name="languageCode">Language code to use for localizing the operand text.</param>
        /// <returns>Text representation of the filter's right operand.</returns>
        /// <remarks>Unless overridden, this method will return the string returned by the ToString() method of the 
        /// <see cref="Value"/> property.</remarks>
        protected virtual string GetFilterRightOperandText(string languageCode)
        {
            if (Value != null)
            {
                var dateTime = Utilities.GetDate(Value.ToString());
                if (dateTime.HasValue)
                    return GlobalizationManager.FormatTheDate(dateTime.Value);

                return Value.ToString();
            }

            return string.Empty;
        }


        /// <summary>
        /// Get a string representation of the filter configuration.
        /// </summary>
        /// <param name="languageCode">Language code to use for localizing the operator and operand text.</param>
        /// <returns>Text representation of the filter configuration.</returns>
        public virtual string ToString(string languageCode)
        {
            var sb = new StringBuilder();

            sb.Append(GetFilterLeftOperandText(languageCode));
            sb.Append(" ");
            sb.Append(TextManager.GetText("/enum/logicalOperator/" + Operator, languageCode, Operator.ToString(), TextManager.ApplicationLanguages));
            sb.Append(" ");
            sb.Append(GetFilterRightOperandText(languageCode));

            return sb.ToString();
        }

        /// <summary>
        /// Load filter configuration information from the specified <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"><see cref="DataRow"/> containing configuration information for the filter.</param>
        protected override void  LoadBaseObjectData(DataRow data)
        {
            FilterTypeName = DbUtility.GetValueFromDataRow(data, "FilterTypeName", string.Empty);
            string operatorString = DbUtility.GetValueFromDataRow<string>(data, "Operator", null);

            if (operatorString != null)
            {
                Operator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), (string)data["Operator"]);
            }

            LastModified = DbUtility.GetValueFromDataRow<DateTime?>(data, "ModifiedDate", null);
            Value = DbUtility.GetValueFromDataRow<object>(data, "Value", null);
        }

        /// <summary>
        /// Insert an entry for the filter in the base filter data table.
        /// </summary>
        /// <param name="t">Transaction context to participate in for the insertion.</param>
        protected override void Create(IDbTransaction t)
        {
            LastModified = DateTime.Now;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_Insert");
            command.AddInParameter("Operator", DbType.String, Operator.ToString());
            command.AddInParameter("Value", DbType.String, Value);
            command.AddInParameter("FilterTypeName", DbType.String, FilterTypeName);
            command.AddInParameter("ModifiedDate", DbType.DateTime, LastModified);
            command.AddOutParameter("FilterID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            object id = command.GetParameterValue("FilterID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save filter.");
            }

            ID = (Int32)id;
        }

        /// <summary>
        /// Update the entry for the filter in the base filter data table.
        /// </summary>
        /// <param name="t">Transaction context to participate in for the update.</param>
        protected override void Update(IDbTransaction t)
        {
            LastModified = DateTime.Now;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_Update");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);
            command.AddInParameter("Operator", DbType.String, Operator.ToString());
            command.AddInParameter("ModifiedDate", DbType.DateTime, LastModified);

            if (Value != null)
            {
                command.AddInParameter("Value", DbType.String, Value.ToString());
            }
            else
            {
                command.AddInParameter("Value", DbType.String, null);
            }

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Delete an entry for the filter in the base filter data table.
        /// </summary>
        /// <param name="t">Transaction context to participate in for the deletion.</param>
        public override void Delete(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Filter_Delete");
            command.AddInParameter("FilterID", DbType.Int32, ID.Value);

            db.ExecuteNonQuery(command, t);
        }


        #region IEquatable<FilterData> Members

        /// <summary>
        /// Compare one <see cref="FilterData"/> object with another <see cref="FilterData"/> object.
        /// </summary>
        /// <param name="other">Other <see cref="FilterData"/> object to compare to.</param>
        /// <returns>Boolean indicating if the objects are equal.</returns>
        /// <remarks>Comparison is performed by comparing the IDs of the <see cref="FilterData"/> objects.</remarks>
        public bool Equals(FilterData other)
        {
            return ID == other.ID;
        }

        #endregion
    }
}
