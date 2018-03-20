using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Checkbox.Forms.Items;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Data for analysis item to display answer detail information in reports
    /// </summary>
    [Serializable]
    public class DetailsItemData : AnalysisItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "DetailsItemData"; } }

        /// <summary>
        /// Get sproc name to load item
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetDetailsItem"; } }

        /// <summary>
        /// Get/set whether to group answers or not
        /// </summary>
        public bool GroupAnswers { get; set; }

        /// <summary>
        /// Get/set whether to link to response details
        /// </summary>
        public bool LinkToResponseDetails { get; set; }

        /// <summary>
        /// Load item data from the specified data row
        /// </summary>
        /// <param name="data"></param>
        protected override void  LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            GroupAnswers = DbUtility.GetValueFromDataRow(data, "GroupAnswers", false);
            LinkToResponseDetails = DbUtility.GetValueFromDataRow(data, "LinkToResponseDetails", false);
        }

        /// <summary>
        /// Create an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertDetailsItem");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("GroupAnswers", DbType.Boolean, GroupAnswers);
            command.AddInParameter("LinkToResponseDetails", DbType.Boolean, LinkToResponseDetails);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update an instance of the details item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateDetailsItem");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("GroupAnswers", DbType.Boolean, GroupAnswers);
            command.AddInParameter("LinkToResponseDetails", DbType.Boolean, LinkToResponseDetails);
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(command, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new DetailsItem();
        }

        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);
            writer.WriteElementString("GroupAnswers", GroupAnswers.ToString());
            writer.WriteElementString("LinkToResponseDetails", LinkToResponseDetails.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            GroupAnswers = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("GroupAnswers"));
            LinkToResponseDetails =  XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("LinkToResponseDetails"));
        }

        #endregion

    }
}

