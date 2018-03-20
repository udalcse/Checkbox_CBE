using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance information for template items, including form items, report items, etc.
    /// </summary>
    [Serializable]
    public abstract class AppearanceData : PersistedDomainObject
    {
        private Dictionary<string, AppearanceProperty> _properties;

		/// <summary>
		/// For appearance items, the actual concrete data lives in the parent table itself, which is
		/// backwards, but due to a legacy table so by default, data table is the same as the parent
		/// table.
		/// </summary>
		public override string DataTableName { get { return "AppearanceData"; } }

		/// <summary>
		/// 
		/// </summary>
		public override string IdentityColumnName { get { return "AppearanceId"; } }

        /// <summary>
        /// Get type name for data.
        /// </summary>
        public override string ObjectTypeName { get { return "AppearanceData"; } }

        /// <summary>
        /// Gets the string code for this AppearanceData
        /// <remarks>
        /// This would be the code used to map UI controls to the appearance, for example RADIO_BUTTONS
        /// </remarks>
        /// </summary>
        public abstract string AppearanceCode { get;}

        /// <summary>
        /// Used to override appearance code, such as when importing matrix children.
        /// </summary>
        private string AppearanceCodeOverride { get; set; }

        /// <summary>
        /// Get appearance data
        /// </summary>
        protected override string LoadSprocName { get { return "ckbx_sp_AppearanceData_Get"; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected AppearanceData()
        {
            Properties["ItemPosition"] = CreateProperty("ItemPosition", typeof(string).ToString(), "Left");
			Properties["Layout"] = CreateProperty("Layout", typeof(Layout).ToString(), Layout.Horizontal.ToString());
        }

        /// <summary>
        /// Access value of appearance properties as string.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks>To get properties cast to other times, use GetPropertyValue method.</remarks>
        public string this[string propertyName]
        {
            get { return GetPropertyValue<string>(propertyName); }
            set { SetPropertyValue(propertyName, value); }
        }

        /// <summary>
        /// Access properties dictionary
        /// </summary>
        public Dictionary<string, AppearanceProperty> Properties
        {
            get {
                return _properties ??
                       (_properties =
                        new Dictionary<string, AppearanceProperty>(StringComparer.InvariantCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Specify override for appearance data.  This value will be used instead of default appearance code
        /// when creating new instnaces of appearance in the database.  It is not used when updating existing items.
        /// </summary>
        /// <param name="newAppearanceCode"></param>
        public void OverrideAppearanceCode(string newAppearanceCode)
        {
            AppearanceCodeOverride = newAppearanceCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetDefaults() { }

        /// <summary>
        /// Create an appearance property object 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="typeNameAsString"></param>
        /// <param name="valueAsString"></param>
        /// <returns></returns>
        protected virtual AppearanceProperty CreateProperty(string propertyName, string typeNameAsString, string valueAsString)
        {
            return new AppearanceProperty
            {
                Name = propertyName,
                ValueTypeString = typeNameAsString,
                ValueAsString = valueAsString
            };
        }

        /// <summary>
        /// Get a typed value for a property. Returns default(T) in case of no value
        /// or conversion error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual T GetPropertyValue<T>(string propertyName)
        {
            return GetPropertyValue(propertyName, default(T));
        }

        /// <summary>
        /// Get a typed value for a property, returning specified default value
        /// if value is not present or conversion fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="?"></param>
        /// <returns></returns>
        public T GetPropertyValue<T>(string propertyName, T defaultValue)
        {
            try
            {
                if (Properties.ContainsKey(propertyName))
                {
                    return (T)Convert.ChangeType(Properties[propertyName].ValueAsString, typeof(T));
                }
            }
            catch { }


            return defaultValue;
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public virtual void SetPropertyValue(string propertyName, object value)
        {
            if (Properties.ContainsKey(propertyName))
            {
                Properties[propertyName].ValueAsString = value != null ? value.ToString() : string.Empty;
            }
            else
            {
                if(value != null)
                {
                    Properties[propertyName] = new AppearanceProperty
                    {
                        Name = propertyName,
                        ValueAsString = value.ToString(),
                        ValueTypeString = value.GetType().ToString()
                    };
                }
            }
        }


        /// <summary>
        /// Create configuration data
        /// </summary>
        /// <returns></returns>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return new AppearanceDataSet(ObjectTypeName);
        }

        /// <summary>
        /// Load property data from the data row
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadAdditionalData(PersistedDomainObjectDataSet data)
        {
            base.LoadAdditionalData(data);

            LoadProperties((AppearanceDataSet)data);
        }

        /// <summary>
        /// Load appearance properties from the specified data set.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void LoadProperties(AppearanceDataSet data)
        {
            //Clear properties
            //Properties.Clear(); -- don't clear properties because we may initial setting will be lost

            DataRow[] appearanceRows = data.GetPropertyRows();

            foreach (DataRow propertyRow in appearanceRows)
            {
                string propertyName = DbUtility.GetValueFromDataRow(propertyRow, "PropertyName", string.Empty);
                string propertyValue = DbUtility.GetValueFromDataRow(propertyRow, "ValueAsString", string.Empty);
                string propertyValueTypeString = DbUtility.GetValueFromDataRow(propertyRow, "ValueType", string.Empty);

                //Do nothing if data is incomplete
                if (string.IsNullOrEmpty(propertyName)
                    || string.IsNullOrEmpty(propertyValueTypeString))
                {
                    continue;
                }

                //Otherwise load a property and add to collection
                Properties[propertyName] = CreateProperty(propertyName, propertyValueTypeString, propertyValue);
            }
        }

        /// <summary>
        /// Save appearance data associated with an item
        /// </summary>
        /// <param name="itemId"></param>
        public virtual void Save(int? itemId)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        Save(transaction, itemId);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="itemId">Id of an item.  If no itemId is passed then the AppearanceData is saved but it will not be related to an item.</param>
        public virtual void Save(IDbTransaction transaction, int? itemId)
        {
            Save(transaction);

            if (itemId.HasValue)
            {
                //Create the link between the item id and the appearance code
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemAppearances_Add");
                command.AddInParameter("AppearanceID", DbType.Int32, ID);
                command.AddInParameter("ItemID", DbType.Int32, itemId);

                db.ExecuteNonQuery(command, transaction);
            }
        }


        /// <summary>
        /// Create an entry in item appearance for this item
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Create(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_Create");
            command.AddInParameter("AppearanceCode", DbType.String, AppearanceCodeOverride ?? AppearanceCode);
            command.AddInParameter("LastModified", DbType.DateTime, DateTime.Now);
            command.AddOutParameter("AppearanceID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, transaction);

            object id = command.GetParameterValue("AppearanceID");

            if (id == null || id == DBNull.Value)
            {
                throw new Exception("Unable to save appearance data.");
            }

            ID = (int)id;

            //Now save properties
            SaveProperties(transaction);
        }

        /// <summary>
        /// Update the appearance data
        /// </summary>
        /// <param name="transaction"></param>
        protected override void Update(IDbTransaction transaction)
        {
            if (ID <= 0)
            {
                throw new Exception("DataID must be set to Update appearance.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_Update");
            command.AddInParameter("AppearanceID", DbType.Int32, ID);
            command.AddInParameter("LastModified", DbType.DateTime, DateTime.Now);

            //Save properties
            SaveProperties(transaction);

            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Save property data
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void SaveProperties(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            //Remove existing appearance data
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_DeleteProperties");
            command.AddInParameter("AppearanceId", DbType.Int32, ID);
            db.ExecuteNonQuery(command, transaction);

            //Add properties
            foreach (string propertyName in Properties.Keys)
            {
                AppearanceProperty property = Properties[propertyName];

                command = db.GetStoredProcCommandWrapper("ckbx_sp_Appearance_InsertProperty");
                command.AddInParameter("AppearanceId", DbType.Int32, ID);
                command.AddInParameter("Name", DbType.String, propertyName);
                command.AddInParameter("ValueAsString", DbType.String, property.ValueAsString);
                command.AddInParameter("ValueType", DbType.String, property.ValueTypeString);

                db.ExecuteNonQuery(command, transaction);
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
        protected override void WriteConfigurationToXml(XmlWriter writer, WriteExternalDataCallback externalDataCallback = null)
		{
			writer.WriteStartElement("Properties");

			foreach (KeyValuePair<string, AppearanceProperty> pair in Properties)
			{
				AppearanceProperty property = pair.Value;

				writer.WriteStartElement("Property");
				writer.WriteAttributeString("Name", pair.Key);
				writer.WriteAttributeString("Value", property.ValueAsString);
				writer.WriteAttributeString("Type", property.ValueTypeString);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public override void Import(XmlNode xmlNode, ReadExternalDataCallback callback = null, object creator = null)
		{
		    var propertyNodes = xmlNode.SelectNodes("Properties/Property");

            foreach(XmlNode propertyNode in propertyNodes)
            {
                SetPropertyValue(XmlUtility.GetAttributeText(propertyNode, "Name"),
                                 XmlUtility.GetAttributeText(propertyNode, "Value"));
            }

            if (callback != null)
            {
                callback(this, xmlNode, creator);
            }
		}

        /// <summary>
        /// Delete the appearance data
        /// </summary>
        /// <param name="t"></param>
        public override void Delete(IDbTransaction t) { }

        /// <summary>
        /// Copy the appearance data to a new appearance data.  Returns null unless specifically overridden.
        /// </summary>
        /// <returns></returns>
        public virtual AppearanceData Copy()
        {
            return null;
        }

        /// <summary>
        /// Get appearance properties as a simple name value collection suitable for sending over WCF service.
        /// </summary>
        /// <returns></returns>
        public SimpleNameValueCollection GetPropertiesAsNameValueCollection()
        {
            var propertyNvCollection = new SimpleNameValueCollection();

            foreach (var appearanceProperty in Properties)
            {
                propertyNvCollection[appearanceProperty.Key] = appearanceProperty.Value.ValueAsString;
            }

            return propertyNvCollection;
        }
    }
}
