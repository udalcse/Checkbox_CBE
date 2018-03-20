using System;
using System.Data;
using System.Reflection;
using System.Collections;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.Data;
using Prezza.Framework.Common;

namespace Checkbox.Panels
{
    internal class PanelFactory
    {
        private static readonly Hashtable _panelTypes;

        /// <summary>
        /// Initialize the "cache"
        /// </summary>
        static PanelFactory()
        {
            lock(typeof(PanelFactory))
            {
                _panelTypes = new Hashtable();
            }
        }

        /// <summary>
        /// Create a panel with the specified type
        /// </summary>
        /// <param name="panelTypeID"></param>
        /// <returns></returns>
        public Panel CreatePanel(Int32 panelTypeID)
        {
            PanelTypeInfo typeInfo = GetPanelTypeInfo(panelTypeID);

            if (typeInfo != null)
            {
                return CreatePanel(typeInfo);
            }
            
            return null;
        }

        /// <summary>
        /// Get panel type info
        /// </summary>
        /// <param name="panelTypeID"></param>
        internal static PanelTypeInfo GetPanelTypeInfo(Int32 panelTypeID)
        {

            if (_panelTypes.ContainsKey(panelTypeID))
            {
                return (PanelTypeInfo)_panelTypes[panelTypeID];
            }
            
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetTypeInfo");
            command.AddInParameter("PanelTypeID", DbType.Int32, panelTypeID);

            string typeClassName = string.Empty;
            string typeAssembly = string.Empty;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        if (reader["TypeName"] != DBNull.Value)
                        {
                            typeClassName = (string)reader["TypeName"];
                        }

                        if (reader["TypeAssembly"] != DBNull.Value)
                        {
                            typeAssembly = (string)reader["TypeAssembly"];
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }



            if (typeClassName.Trim() != string.Empty && typeAssembly.Trim() != string.Empty)
            {
                PanelTypeInfo typeInfo = new PanelTypeInfo {ClassTypeName = typeClassName, ClassTypeAssembly = typeAssembly, TypeID = panelTypeID};

                lock (_panelTypes.SyncRoot)
                {
                    _panelTypes[panelTypeID] = typeInfo;
                }

                return typeInfo;
            }
            
            return null;
        }

       
        /// <summary>
        /// Create an item data of the specified type.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private static Panel CreatePanel(PanelTypeInfo typeInfo)
        {
            return CreateObject(GetType(typeInfo.ClassTypeName + "," + typeInfo.ClassTypeAssembly));
        }

        /// <summary>
        /// Create an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Panel CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsItemData(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("Panel does not have a constructor: " + type.FullName);
            }

            object createdObject;

            try
            {
                createdObject = constructor.Invoke(null);
            }
            catch (MethodAccessException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (TargetParameterCountException ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return (Panel)createdObject;
        }

        /// <summary>
        /// Get the <see cref="System.Type"/> from a type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static Type GetType(string typeName)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            try
            {
                //Get the type
                // Throw error on failure = true
                // Ignore case = false
                return Type.GetType(typeName, true, false);

            }
            catch (TypeLoadException ex)
            {
                throw new Exception("A type-loading error occurred.  Type was: " + typeName, ex);
            }
        }

        /// <summary>
        /// Validate that the specified <see cref="System.Type"/> is a valid <see cref="ItemData"/> type.
        /// </summary>
        /// <param name="type"></param>
        private static void ValidateTypeIsItemData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(Panel).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between Panel type [" + typeof(Panel).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// Container for type information
        /// </summary>
        internal class PanelTypeInfo
        {
            public Int32 TypeID;
            public string ClassTypeName;
            public string ClassTypeAssembly;
        }
    }
}
