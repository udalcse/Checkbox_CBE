using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Analytics.Items.Configuration
{
    /// <summary>
    /// Configuration data object for custom rating scale.  This object 
    /// handles persistence & retrieval of data specific to the custom object, including
    /// the parameters that control whether to include standard deviation, etc.
    /// </summary>
    [Serializable]
    public class StatisticsItemData : AnalysisItemData
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "StatisticsTableItemData"; }
        }

        /// <summary>
        /// Get sproc name to load item
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetStatisticsTable"; }
        }


        /// <summary>
        /// Get/set option calculation point
        /// </summary>
        public StatisticsItemReportingOption ReportOption { get; set; }

        /// <summary>
        /// Load item data from the specified data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(System.Data.DataRow data)
        {
            base.LoadBaseObjectData(data);

            string reportOptionAsString = DbUtility.GetValueFromDataRow(data, "ReportOption", string.Empty);

            if (!String.IsNullOrEmpty(reportOptionAsString))
            {
                try
                {
                    ReportOption =
                        (StatisticsItemReportingOption)
                        Enum.Parse(typeof (StatisticsItemReportingOption), reportOptionAsString);
                }
                catch
                {
                    ReportOption = StatisticsItemReportingOption.All;
                }
            }            
        }

        public override void SetDefaults(Template template)
        {
            ReportOption = StatisticsItemReportingOption.All;
        }

        /// <summary>
        /// Create an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertStatisticsTable");
            
            commandWrapper.AddInParameter("ItemID", DbType.Int32, ID);
            commandWrapper.AddInParameter("ReportOption", DbType.String, ReportOption);
            commandWrapper.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(commandWrapper, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update an instance of the item data in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper commandWrapper = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateStatisticsTable");

            commandWrapper.AddInParameter("ItemID", DbType.Int32, ID);
            commandWrapper.AddInParameter("ReportOption", DbType.String, ReportOption);
            commandWrapper.AddInParameter("UseAliases", DbType.Boolean, UseAliases);

            db.ExecuteNonQuery(commandWrapper, t);

            //Call the base class method to persist other data
            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new StatisticsItem();
        }

        #region IXMLSerializable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);
            
            writer.WriteElementString("ReportOption", ReportOption.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var reportOption = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ReportOption")) ??
                               StatisticsItemReportingOption.All.ToString();

            ReportOption =
                (StatisticsItemReportingOption) Enum.Parse(typeof (StatisticsItemReportingOption), reportOption);
        }

        #endregion
    }
}
