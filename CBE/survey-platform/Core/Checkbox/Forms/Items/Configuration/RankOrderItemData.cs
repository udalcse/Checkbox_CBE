using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Container for configuration information associated with RankOrder items.
    /// </summary>
    [Serializable]
    public class RankOrderItemData : SelectItemData
    {
        /// <summary>
        /// Data table name for slider item configuration data
        /// </summary>
        public override string ItemDataTableName
        {
            get { return "RankOrderItemData"; }
        }

        /// <summary>
        /// Load procedure name
        /// </summary>
        protected override string LoadSprocName
        {
            get { return "ckbx_sp_ItemData_GetRankOrder"; }
        }

        /// <summary>
        /// Get/Set Type of the item
        /// </summary>
        public RankOrderType RankOrderType { get; set; }

        /// <summary>
        /// Get/Set type of rank order option
        /// </summary>
        public RankOrderOptionType RankOrderOptionType { get; set; }

        /// <summary>
        /// N, top N items will be selected
        /// </summary>
        public int? N { get; set; }

        /// <summary>
        /// Rank order doesn't support scoring
        /// </summary>
        public override bool ItemIsIScored
        {
            get { return false; }
        }

        /// <summary>
        /// Load configuration data for this item from the supplied <see cref="DataRow"/>.
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            RankOrderType  =
                (RankOrderType)
                Enum.Parse(typeof(RankOrderType), DbUtility.GetValueFromDataRow(data, "RankOrderType", RankOrderType.Numeric.ToString()));

            RankOrderOptionType =
                (RankOrderOptionType)
                Enum.Parse(typeof (RankOrderOptionType),
                           DbUtility.GetValueFromDataRow(data, "RankOrderOptionType",
                                                         RankOrderOptionType.Text.ToString()));

            N = DbUtility.GetValueFromDataRow<int?>(data, "N", null);
            Randomize = DbUtility.GetValueFromDataRow(data, "Randomize", false);
            IsRequired = DbUtility.GetValueFromDataRow(data, "IsRequired", false);
        }

        /// <summary>
        /// Create an instance of a slider item based on this configuration
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new RankOrder();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetDefaults(Template template)
        {
            RankOrderType = RankOrderType.SelectableDragnDrop;
            RankOrderOptionType = RankOrderOptionType.Text;
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            base.Create(transaction);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertRankOrder");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("RankOrderType", DbType.String, RankOrderType.ToString());
            command.AddInParameter("RankOrderOptionType", DbType.String, RankOrderOptionType.ToString());
            command.AddInParameter("N", DbType.Int32, N);
            command.AddInParameter("Randomize", DbType.Boolean, Randomize);
            command.AddInParameter("IsRequired", DbType.Boolean, IsRequired);

            db.ExecuteNonQuery(command, transaction);

            UpdateLists(transaction);
        }

        /// <summary>
        /// Update an instance of a RankOrder configuration in the persistent store
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            if (ID <= 0)
            {
                throw new ApplicationException("No DataID specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_UpdateRankOrder");

            command.AddInParameter("ItemID", DbType.Int32, ID);
            command.AddInParameter("TextID", DbType.String, TextID);
            command.AddInParameter("SubTextID", DbType.String, SubTextID);
            command.AddInParameter("RankOrderType", DbType.String, RankOrderType.ToString());
            command.AddInParameter("RankOrderOptionType", DbType.String, RankOrderOptionType.ToString());
            command.AddInParameter("N", DbType.Int32, N);
            command.AddInParameter("Randomize", DbType.Boolean, Randomize);
            command.AddInParameter("IsRequired", DbType.Int32, IsRequired);

            db.ExecuteNonQuery(command, t);

            UpdateLists(t);
        }

        /// <summary>
        /// Ensure a max of one option is selected by default
        /// </summary>
        public override ReadOnlyCollection<ListOptionData> Options
        {
            get
            {
                bool selected = false;
                ReadOnlyCollection<ListOptionData> options = base.Options;

                foreach (ListOptionData option in options)
                {
                    if (option.IsDefault)
                    {
                        if (selected)
                        {
                            option.IsDefault = false;
                        }

                        selected = true;
                    }
                }

                return options;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

            writer.WriteElementString("RankOrderType", RankOrderType.ToString());
            writer.WriteElementString("RankOrderOptionType", RankOrderOptionType.ToString());
            writer.WriteElementString("N", N.ToString());
            writer.WriteElementString("Randomize", Randomize.ToString());
            writer.WriteElementString("IsRequired", IsRequired.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        protected override void ReadItemTypeSpecificXml(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
        {
            base.ReadItemTypeSpecificXml(xmlNode, callback, creator);

            var enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("RankOrderType"));

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = RankOrderType.Numeric.ToString();
            }

            RankOrderType =
                (RankOrderType)
                Enum.Parse(typeof(RankOrderType), enumString);

            enumString = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("RankOrderOptionType"));
            if (String.IsNullOrEmpty(enumString))
                enumString = RankOrderOptionType.Text.ToString();

            RankOrderOptionType = (RankOrderOptionType) Enum.Parse(typeof (RankOrderOptionType), enumString);

            N = XmlUtility.GetNodeInt(xmlNode.SelectSingleNode("N"));
            Randomize = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("Randomize"));
            IsRequired = XmlUtility.GetNodeBool(xmlNode.SelectSingleNode("IsRequired"));

            Save();
        }


        /// <summary>
        /// Ruturns the clone of the object
        /// </summary>
        /// <returns></returns>
        public new object Clone()
        {
            var result = base.Clone() as RankOrderItemData;

            result.Randomize = Randomize;
            result.N = N;
            result.RankOrderType = RankOrderType;
            result.RankOrderOptionType = RankOrderOptionType;

            return result;
        }
    }

    /// <summary>
    /// Rank Order Type that describes the behavior of the question item
    /// </summary>
    [Serializable]
    public enum RankOrderType
    {
        SelectableDragnDrop = 1,
        Numeric,
        TopN,
        DragnDroppable
    }

    /// <summary>
    /// Rank Order Option Type
    /// </summary>
    [Serializable]
    public enum RankOrderOptionType
    {
        Text = 1,
        Image
    }
}
