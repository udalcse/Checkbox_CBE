using System;
using System.Data;
using System.Xml;
using Checkbox.Forms.Logic;
using Checkbox.Globalization.Text;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Item data for matrix sum total items
    /// </summary>
    [Serializable]
    public class MatrixSumTotalItemData : SingleLineTextItemData
    {
        private LogicalOperator _comparisonOperator;
        private double _totalValue;

        /// <summary>
        /// Load data for the matrix sum total item
        /// </summary>
        public MatrixSumTotalItemData()
        {
            _comparisonOperator = LogicalOperator.Equal;
            _totalValue = 0;
        }

        /// <summary>
        /// Get data table name
        /// </summary>
        public override string ItemDataTableName { get { return "MatrixSumTotalItemData"; } }

        /// <summary>
        /// Get load sproc name
        /// </summary>
        protected override string LoadSprocName
        {
            get
            {
                return "ckbx_sp_ItemData_GetMatrixSumTotal";
            }
        }

        /// <summary>
        /// Get/set the operator to use when 
        /// </summary>
        public virtual LogicalOperator ComparisonOperator
        {
            get { return _comparisonOperator; }
            set
            {
                if (value == LogicalOperator.Equal || value == LogicalOperator.GreaterThan || value == LogicalOperator.GreaterThanEqual
                    || value == LogicalOperator.LessThan || value == LogicalOperator.LessThanEqual)
                {
                    _comparisonOperator = value;
                }
                else
                {
                    throw new Exception("Operator type " + value + " is not supported for matrix sum total columns.");
                }
            }
        }

        /// <summary>
        /// Get/set the total value to use when comparing matrix column totals
        /// </summary>
        public virtual double TotalValue
        {
            get { return _totalValue; }
            set { _totalValue = value; }
        }

        /// <summary>
        /// Only numeric format is supported
        /// </summary>
        public override AnswerFormat Format
        {
            get
            {
                if (base.Format != AnswerFormat.Decimal && base.Format != AnswerFormat.Integer && base.Format != AnswerFormat.Numeric)
                {
                    return AnswerFormat.Numeric;
                }

                return base.Format;
            }
            set
            {
                if (value != AnswerFormat.Decimal && value != AnswerFormat.Integer && value != AnswerFormat.Numeric)
                {
                    base.Format = AnswerFormat.Numeric;
                }
                else
                {
                    base.Format = value;
                }
            }
        }

        /// <summary>
        /// Create an instance of the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertMatrixSumTotal");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Operator", DbType.String, ComparisonOperator.ToString());
            command.AddInParameter("SumValue", DbType.Double, TotalValue);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Update an instance of the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateMatrixSumTotal");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("Operator", DbType.String, ComparisonOperator.ToString());
            command.AddInParameter("SumValue", DbType.Double, TotalValue);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Load the item data from the data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            TotalValue = DbUtility.GetValueFromDataRow<double>(data, "SumValue", 0);

            string comparisonOperatorString = DbUtility.GetValueFromDataRow(data, "Operator", LogicalOperator.Equal.ToString());
            ComparisonOperator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), comparisonOperatorString);
        }

        /// <summary>
        /// Create a matrix sum total item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new MatrixSumTotalItem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="externalDataCallback"> </param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("TotalValue", TotalValue.ToString());
            writer.WriteElementString("ComparisonOperator", ComparisonOperator.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="callback"> </param>
        /// <param name="creator"> </param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var totalValue = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("TotalValue"));
            double dTotalValue;
            if (double.TryParse(totalValue, out dTotalValue))
                TotalValue = dTotalValue;

            var comparisonOperator = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ComparisonOperator"));
            LogicalOperator logicalOperator;
            if (Enum.TryParse(comparisonOperator, out logicalOperator))
                ComparisonOperator = logicalOperator;
        }
    }
}
