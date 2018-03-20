using System;
using System.Data;
using System.Collections.Generic;

using Checkbox.Common;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Factory for creating item formatters
    /// </summary>
    public static class ItemFormatterFactory
    {
        private static Dictionary<string, Type> _typeCache;

        /// <summary>
        /// Get the type cache
        /// </summary>
        private static Dictionary<string, Type> TypeCache
        {
            get
            {
                if (_typeCache == null)
                {
                    lock (typeof(ItemFormatterFactory))
                    {
                        _typeCache = new Dictionary<string, Type>();
                    }
                }

                return _typeCache;
            }
        }

        /// <summary>
        /// Get the type for a formatter based on the item type id and desired
        /// text format.
        /// </summary>
        /// <param name="itemTypeId">Type id of item to create a formatter for.</param>
        /// <param name="format">Desired format for output text.</param>
        /// <returns>Type information for item formatter.</returns>
        /// <remarks>The type information is cached, so any changes to the type
        /// information in the database will require an application restart
        /// to take effect.</remarks>
        private static Type GetFormatterType(int itemTypeId, string format)
        {
            string formatterKey = itemTypeId + "_" + format;

            return TypeCache.ContainsKey(formatterKey) ? TypeCache[formatterKey] : LookupFormatterType(itemTypeId, format);
        }

        /// <summary>
        /// Lookup an item formatter type in the database.
        /// </summary>
        /// <param name="itemTypeId">Type Id of item to format.</param>
        /// <param name="format">Desired format.</param>
        /// <returns>Type information.</returns>
        private static Type LookupFormatterType(int itemTypeId, string format)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemFormatter_Get");
            command.AddInParameter("ItemTypeId", DbType.Int32, itemTypeId);
            command.AddInParameter("Format", DbType.String, format);

            Type theType = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        string className = DbUtility.GetValueFromDataReader<string>(reader, "FormatterClassName", null);
                        string assembly = DbUtility.GetValueFromDataReader<string>(reader, "FormatterAssembly", null);

                        if (Utilities.IsNotNullOrEmpty(className) && Utilities.IsNotNullOrEmpty(assembly))
                        {
                            theType = Type.GetType(className + "," + assembly);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return theType;
        }

        /// <summary>
        /// Get an item formatter for the specified item type and format.
        /// </summary>
        /// <param name="itemTypeId">Type Id of item to format.</param>
        /// <param name="format">Desired format.</param>
        /// <returns><see cref="IItemFormatter"/> item formatter.</returns>
        /// <remarks>The type information is cached, so any changes to the type
        /// information in the database will require an application restart
        /// to take effect.</remarks>
        public static IItemFormatter GetItemFormatter(int itemTypeId, string format)
        {
            Type theType = GetFormatterType(itemTypeId, format);

            if (theType != null)
            {
                return Activator.CreateInstance(theType) as IItemFormatter;
            }
            
            return null;
        }
    }
}
