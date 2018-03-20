using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;

using Prezza.Framework.Data;

namespace Checkbox.Forms.PageLayout.Configuration
{
    /// <summary>
    /// Abstract base for page layout data
    /// </summary>
    [Serializable]
    public abstract class PageLayoutTemplateData : PersistedDomainObject
    {
        private Dictionary<int, string> _itemZoneMappings;
        private Dictionary<string, LayoutTemplateExtendedProperty> _extendedProperties;

        /// <summary>
        /// Get the type name for the layout template
        /// </summary>
        public abstract string TypeName { get; }

        /// <summary>
        /// Get type name of persisted domain object
        /// </summary>
        public override string ObjectTypeName { get { return "PageLayoutTemplate"; } }

        /// <summary>
        /// Add an extended property to the collection
        /// </summary>
        /// <param name="property"></param>
        protected void AddExtendedProperty(LayoutTemplateExtendedProperty property)
        {
            ExtendedProperties[property.Name] = property;
        }

        /// <summary>
        /// Clear the extended properties
        /// </summary>
        protected void ClearExtendedProperties()
        {
            ExtendedProperties.Clear();
        }

        /// <summary>
        /// Get a list of the extended properties.
        /// </summary>
        protected Dictionary<string, LayoutTemplateExtendedProperty> ExtendedProperties
        {
            get { return _extendedProperties ?? (_extendedProperties = new Dictionary<string, LayoutTemplateExtendedProperty>(StringComparer.InvariantCultureIgnoreCase)); }
        }

        /// <summary>
        /// Get dictionary of item to zone mappings
        /// </summary>
        protected Dictionary<int, string> ItemZoneMappings
        {
            get { return _itemZoneMappings ?? (_itemZoneMappings = new Dictionary<int, string>()); }
        }

        /// <summary>
        /// Get a list of extended properties supported by the template
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetExtendedPropertyNames()
        {
            return ExtendedProperties.Values.Select(prop => prop.Name).ToList();
        }

        /// <summary>
        /// Get the value of an extended property.  Returns null if the property could not be found
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual object GetPropertyValue(string propertyName)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                LayoutTemplateExtendedProperty prop = ExtendedProperties[propertyName];

                if (prop.Value != null && prop.Value.ToString().ToLower() == "true")
                {
                    return "Yes";
                }

                if (prop.Value != null && prop.Value.ToString().ToLower() == "false")
                {
                    return "No";
                }

                return prop.Value;
            }

            return null;
        }

        /// <summary>
        /// Set the value for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public virtual void SetPropertyValue(string propertyName, object value)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                ExtendedProperties[propertyName].Value = value;
            }
        }

        /// <summary>
        /// Get a list of possible values for a property.  An empty list signifies no constraint on the values
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual List<string> GetPropertyValueList(string propertyName)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                if (ExtendedProperties[propertyName].PossibleValues != null)
                {
                    return new List<string>(ExtendedProperties[propertyName].PossibleValues);
                }

                return new List<string>();
            }

            return new List<string>();
        }

        /// <summary>
        /// Get the maximum value for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual double? GetPropertyMaxValue(string propertyName)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                return ExtendedProperties[propertyName].MaxValue;
            }

            return 0;
        }

        /// <summary>
        /// Get the minimum value for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual double? GetPropertyMinValue(string propertyName)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                return ExtendedProperties[propertyName].MinValue;
            }

            return 0;
        }

        /// <summary>
        /// Get the type for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual LayoutTemplateExtendedPropertyType GetPropertyType(string propertyName)
        {
            if (ExtendedProperties.ContainsKey(propertyName))
            {
                return ExtendedProperties[propertyName].Type;
            }

            return LayoutTemplateExtendedPropertyType.String;
        }

        /// <summary>
        /// Get whether the template allows manual item placement
        /// </summary>
        public virtual bool AllowManualLayout
        {
            get { return true; }
        }


        /// <summary>
        /// Add an item to the layout, or move the item to the specified zone if it is already in the layout.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="zoneName"></param>
        public void AddItemToLayout(int itemId, string zoneName)
        {
            if (itemId > 0 && Utilities.IsNotNullOrEmpty(zoneName))
            {
                ItemZoneMappings[itemId] = zoneName;
            }
        }

        /// <summary>
        /// Remove an item from the page layout
        /// </summary>
        /// <param name="itemId"></param>
        public void RemoveItemFromLayout(int itemId)
        {
            if (ItemZoneMappings.ContainsKey(itemId))
            {
                ItemZoneMappings.Remove(itemId);
            }
        }

        /// <summary>
        /// Instantiate the template represented by this configuration
        /// </summary>
        /// <param name="languageCode">Language code for ml controls in the template.</param>
        /// <returns>Layout template</returns>
        public abstract object CreateTemplate(string languageCode);

        #region Load

        /// <summary>
        /// Get the zone for a particular item.  If no zone is found in the map, null is returned.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetItemZoneMappings()
        {
            return ItemZoneMappings;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete the layout template
        /// </summary>
        /// <param name="t"></param>
        public override void Delete(IDbTransaction t)
        {
            if (ID.HasValue)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_Delete");
                command.AddInParameter("LayoutTemplateID", DbType.Int32, ID);

                db.ExecuteNonQuery(command, t);
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Create the item
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_Insert");
            command.AddInParameter("LayoutTemplateTypeName", DbType.String, TypeName);
            command.AddOutParameter("LayoutTemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            object value = command.GetParameterValue("LayoutTemplateID");

            if (value == null || value == DBNull.Value)
            {
                throw new Exception("No ID was returned from ckbx_sp_LayoutTemplate_Insert");
            }

            ID = (int)value;
        }

        /// <summary>
        /// Update template, currently does nothing
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
        }

        /// <summary>
        /// Save the item/zone mappings
        /// </summary>
        /// <param name="t"></param>
        protected void SaveMappings(IDbTransaction t)
        {
            if (!ID.HasValue)
            {
                return;
            }

            Database db = DatabaseFactory.CreateDatabase();

            //First delete mappings
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_DeleteAllItemZones");
            command.AddInParameter("LayoutTemplateID", DbType.Int32, ID);
            db.ExecuteNonQuery(command, t);

            //Now re-add them
            foreach (int itemId in ItemZoneMappings.Keys)
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_LayoutTemplate_UpSertItemZone");
                command.AddInParameter("LayoutTemplateId", DbType.Int32, ID);
                command.AddInParameter("ItemId", DbType.Int32, itemId);
            }
        }

        #endregion
    }
}
