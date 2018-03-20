using System;
using System.Data;
using System.Xml;
using Checkbox.Forms.Items;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class AverageScoreItemData : AnalysisItemData
    {
        /// <summary>
        /// Get the data table name
        /// </summary>
        public override string ItemDataTableName { get { return "AverageScoreItemData"; } }

        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetAverageScore"; } }

        /// <summary>
        /// Aggregate the scores of the items when calculating the average or display individually
        /// </summary>
        public virtual AverageScoreCalculation AverageScoreCalculation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected virtual AverageScoreCalculation DefaultAverageScoreCalculationValue
        {
            get { return AverageScoreCalculation.Aggregate; }
        }

        /// <summary>
        /// Create the average score item data
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertAverageScore");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("ScoreOption", DbType.String, AverageScoreCalculation.ToString());
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update the average score item data
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateAverageScore");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("ScoreOption", DbType.String, AverageScoreCalculation.ToString());
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Load from datarow
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            if (data["ScoreOption"] != DBNull.Value)
            {
                AverageScoreCalculation = (AverageScoreCalculation)Enum.Parse(typeof(AverageScoreCalculation), (string)data["ScoreOption"]);
            }
            else
            {
                AverageScoreCalculation = DefaultAverageScoreCalculationValue;
            }
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new AverageScoreItem();
        }

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);
         
            writer.WriteElementString("AverageScoreCalculation", AverageScoreCalculation.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
 	        base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var calculation = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AverageScoreCalculation")) ?? DefaultAverageScoreCalculationValue.ToString();

            AverageScoreCalculation =
                (AverageScoreCalculation) Enum.Parse(typeof (AverageScoreCalculation), calculation);
        }

        #endregion

    }
}
