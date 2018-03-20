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
    public class FrequencyItemData : AnalysisItemData
    {
        /// <summary>
        /// Get/set the other option
        /// </summary>
        public OtherOption OtherOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override string ItemDataTableName { get { return "FrequencyItemData"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetFrequency"; } }

        /// <summary>
        /// 
        /// </summary>
        public int? PrimarySourceItemID { get; set; }

        /// <summary>
        /// Show statistics table below the chart or not
        /// </summary>
        public bool DisplayStatistics { get; set; }

        /// <summary>
        /// Show summary answers table below the chart or not
        /// </summary>
        public bool DisplayAnswers { get; set; }

        /// <summary>
        /// Load from datarow
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            if (data != null)
            {
                try
                {
                    if (data["OtherOption"] != DBNull.Value)
                    {
                        OtherOption = (OtherOption)Enum.Parse(typeof(OtherOption), Convert.ToString(data["OtherOption"]));
                        PrimarySourceItemID = data["PrimarySourceItemID"] is DBNull ? null : (Int32?)data["PrimarySourceItemID"];
                    }
                }
                catch
                {
                    OtherOption = OtherOption.Aggregate;
                }

                try
                {
                    DisplayStatistics = (bool)data["DisplayStatistics"];
                }
                catch
                {
                    DisplayStatistics = false;
                }

                try
                {
                    DisplayAnswers = (bool)data["DisplayAnswers"];
                }
                catch
                {
                    DisplayAnswers = false;
                }

            }
        }

        /// <summary>
        /// Loads additional data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(Common.PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);
            //after loading source item we must check that PrimarySourceItem is among them
            if (PrimarySourceItemID.HasValue && !SourceItemIds.Contains(PrimarySourceItemID.Value))
                PrimarySourceItemID = null;
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            if (ID <= 0)
            {
                throw new Exception("Unable to save analysis item.  DataID is <= 0.");
            }

            //Save item data
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertFrequency");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("OtherOption", DbType.String, OtherOption.ToString());
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            command.AddInParameter("PrimarySourceItemID", DbType.Int32, PrimarySourceItemID);
            command.AddInParameter("DisplayStatistics", DbType.Boolean, DisplayStatistics);
            command.AddInParameter("DisplayAnswers", DbType.Boolean, DisplayAnswers);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Update the item
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
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateFrequency");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("OtherOption", DbType.String, OtherOption.ToString());
            command.AddInParameter("UseAliases", DbType.Boolean, UseAliases);
            command.AddInParameter("PrimarySourceItemID", DbType.Int32, PrimarySourceItemID);
            command.AddInParameter("DisplayStatistics", DbType.Boolean, DisplayStatistics);
            command.AddInParameter("DisplayAnswers", DbType.Boolean, DisplayAnswers);

            db.ExecuteNonQuery(command, t);

            UpdateSourceItemTables(t);
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new FrequencyItem();
        }


        #region IXMLSerializable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(System.Xml.XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);
            writer.WriteElementString("OtherOption", OtherOption.ToString());
            writer.WriteElementString("DisplayStatistics", DisplayStatistics.ToString());
            writer.WriteElementString("DisplayAnswers", DisplayAnswers.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var otherOption = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("OtherOption")) ??
                              OtherOption.Aggregate.ToString();

            OtherOption = (OtherOption) Enum.Parse(typeof (OtherOption), otherOption);

            var ds = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("DisplayStatistics")) ??
                              "False";
            DisplayStatistics = bool.Parse(ds);

            var da = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("DisplayAnswers")) ??
                              "False";
            DisplayAnswers = bool.Parse(da);

        }

        #endregion


    }
}
