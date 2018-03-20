using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Security;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Data for an item designed to update user profile properties
    /// </summary>
    [Serializable]
    public class ProfileUpdaterItemData : ResponseItemData
    {
        private Dictionary<int, int> _itemIdMap;
        private Dictionary<string, ProfileUpdaterProperty> _properties;

        /// <summary>
        /// Private container class
        /// </summary>
        [Serializable]
        public class ProfileUpdaterProperty : IEquatable<ProfileUpdaterProperty>
        {
            /// <summary>
            /// 
            /// </summary>
            public int SourceItemId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ProviderName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sourceItemID"></param>
            /// <param name="providerName"></param>
            /// <param name="propertyName"></param>
            public ProfileUpdaterProperty(int sourceItemID, string providerName, string propertyName)
            {
                SourceItemId = sourceItemID;
                ProviderName = providerName;
                PropertyName = propertyName;
            }

            /// <summary>
            /// Compare updater properties
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(ProfileUpdaterProperty other)
            {
                return
                    other.SourceItemId == SourceItemId
                    && other.ProviderName.Equals(ProviderName, StringComparison.InvariantCultureIgnoreCase)
                    && other.PropertyName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// List of profile properties
        /// </summary>
        public Dictionary<string, ProfileUpdaterProperty> Properties
        {
            get { return _properties ?? (_properties = new Dictionary<string, ProfileUpdaterProperty>()); }
        }

        /// <summary>
        /// Get name of profile updating data table name
        /// </summary>
        public const string UpdatedPropertiesTableName = "PUProperties";

        /// <summary>
        /// No item data table for pu props exists in 4.x.  The only table is the property mapping table, so return a table name, though
        /// this table is not used anywhere.
        /// </summary>
        public override string ItemDataTableName { get { return "ProfileUpdaterItemData"; } }

        /// <summary>
        /// Get name of procedure to use to load data
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_ItemData_GetProfileUpdater"; } }

        /// <summary>
        /// Generate a key to use to lookup properties in props dictionary
        /// </summary>
        /// <param name="sourceItemId"></param>
        /// <param name="providerName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string GeneratePropertyKey(int sourceItemId, string providerName, string propertyName)
        {
            return string.Format("{0}_{1}_{2}", sourceItemId, providerName, propertyName);
        }

        /// <summary>
        /// Add a property to be updated
        /// </summary>
        /// <param name="sourceItemId"></param>
        /// <param name="providerName"></param>
        /// <param name="propertyName"></param>
        public void AddProperty(Int32 sourceItemId, string providerName, string propertyName)
        {
            if (sourceItemId < 0 || Utilities.IsNullOrEmpty(providerName) || Utilities.IsNullOrEmpty(propertyName))
            {
                return;
            }

            Properties[GeneratePropertyKey(sourceItemId, providerName, propertyName)] = new ProfileUpdaterProperty(sourceItemId, providerName, propertyName);
        }

        /// <summary>
        /// Remove a property to be updated
        /// </summary>
        /// <param name="sourceItemId"></param>
        /// <param name="providerName"></param>
        /// <param name="propertyName"></param>
        public void RemoveProperty(Int32 sourceItemId, string providerName, string propertyName)
        {
            if (sourceItemId < 0 || Utilities.IsNullOrEmpty(providerName) || Utilities.IsNullOrEmpty(propertyName))
            {
                return;
            }

            string key = GeneratePropertyKey(sourceItemId, providerName, propertyName);

            if (Properties.ContainsKey(key))
            {
                Properties.Remove(key);
            }
        }

        /// <summary>
        /// Get configuration data set for this item
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new ItemDataSet(ObjectTypeName, ItemDataTableName, "ItemId", UpdatedPropertiesTableName);
        }

        /// <summary>
        /// Load additional data from the profile updater data set
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            if (data.Tables.Contains(ItemDataTableName))
            {
                DataRow[] propRows = data.Tables[ItemDataTableName].Select();

                foreach (DataRow propRow in propRows)
                {
                    AddProperty(
                        DbUtility.GetValueFromDataRow(propRow, "SourceItemId", -1),
                        DbUtility.GetValueFromDataRow(propRow, "ProviderName", string.Empty),
                        DbUtility.GetValueFromDataRow(propRow, "PropertyName", string.Empty));
                }
            }
        }

        /// <summary>
        /// Create the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            base.Create(t);

            UpSert(t);
        }

        /// <summary>
        /// Update the item in the database
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            base.Update(t);

            UpSert(t);
        }

        /// <summary>
        /// Upsert data
        /// </summary>
        /// <param name="t"></param>
        private void UpSert(IDbTransaction t)
        {
            if (ID <= 0)
            {
                throw new Exception("Unable to create item.  DataID must be greater than zero.");
            }

            //Remove mappings
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = GetPropertyDeleteCommand(db);
            db.ExecuteNonQuery(command, t);
            
            //Insert mappings
            foreach (ProfileUpdaterProperty property in Properties.Values)
            {
                command = GetPropertyInsertCommand(db);
                command.AddInParameter("SourceItemID", DbType.Int32, property.SourceItemId);
                command.AddInParameter("ProviderName", DbType.String, property.ProviderName);
                command.AddInParameter("PropertyName", DbType.String, property.PropertyName);

                db.ExecuteNonQuery(command, t);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteItemTypeSpecificXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
        {
            base.WriteItemTypeSpecificXml(writer, externalDataCallback);

			writer.WriteStartElement("Properties");
			writer.WriteAttributeString("Count", Properties.Count.ToString());

			foreach (ProfileUpdaterProperty property in Properties.Values)
			{
				writer.WriteStartElement("Property");
				writer.WriteElementString("ItemId", property.SourceItemId.ToString());
				writer.WriteElementString("ProviderName", property.ProviderName);
				writer.WriteElementString("PropertyName", property.PropertyName);
				writer.WriteEndElement();
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

            var propertiesNodes = xmlNode.SelectNodes("Properties/Property");

            foreach (XmlNode propertyNode in propertiesNodes)
            {
                int itemId = XmlUtility.GetNodeInt(propertyNode.SelectSingleNode("ItemId")) ?? -1;
                string provider = XmlUtility.GetNodeText(propertyNode.SelectSingleNode("ProviderName"));
                string name = XmlUtility.GetNodeText(propertyNode.SelectSingleNode("PropertyName"));

                var props = ProfileManager.ListPropertyNames();
                if (!props.Contains(name))
                    ProfileManager.AddProfileField(name, true, false);

                AddProperty(itemId, provider, name);
                ResolveIdDependencies(_itemIdMap);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadItemImportData(ItemImportReader reader)
        {
            base.ReadItemImportData(reader);

            if (reader != null)
                _itemIdMap = reader.ItemIdMap;
        }

        /// <summary>
        /// Get a command to insert a property
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DBCommandWrapper GetPropertyInsertCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_InsertPUProp");
            command.AddInParameter("ItemID", DbType.Int32, ID);
            return command;
        }

        /// <summary>
        /// Get a command to delete a property
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private DBCommandWrapper GetPropertyDeleteCommand(Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_DeletePUProps");
            command.AddInParameter("ItemID", DbType.Int32, ID);

            return command;
        }

        /// <summary>
        /// Create the item
        /// </summary>
        /// <returns></returns>
        protected override Item CreateItem()
        {
            return new ProfileUpdater();
        }

        /// <summary>
        /// Returns false since this item depends on other survey items, so it does not make sense to have it
        /// be individually exportable
        /// </summary>
        public override bool IsExportable
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        public override void ResolveIdDependencies(Dictionary<int, int> itemIdMap)
        {
            base.ResolveIdDependencies(itemIdMap);

            if (itemIdMap != null)
            {
                var lostProperties = new List<string>();

                foreach (var property in Properties)
                {
                    if (itemIdMap.ContainsKey(property.Value.SourceItemId))
                        property.Value.SourceItemId = itemIdMap[property.Value.SourceItemId];
                    else
                        lostProperties.Add(property.Key);
                }

                foreach (var property in lostProperties)
                {
                    Properties.Remove(property);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemMetadata itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (_properties == null)
            {
                return;
            }

            var propertyCsvList = string.Join(", ", _properties.Values.Select(prop => prop.PropertyName));

            if (itemDto is ItemMetaData)
            {
                var textDataList = ((ItemMetaData)itemDto).TextData;

                foreach (var textData in textDataList)
                {
                    textData.TextValues["NavText"] = Utilities.TruncateText(propertyCsvList, 50);
                }
            }
        }
    }
}
