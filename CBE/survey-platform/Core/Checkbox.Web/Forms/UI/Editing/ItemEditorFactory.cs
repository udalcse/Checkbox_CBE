using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Creates the ItemEditor based on the type and initializes it with the <see cref="ItemData"/>
    /// </summary>
    public static class ItemEditorFactory
    {
        /// <summary>
        /// Simple container for item editor type information
        /// </summary>
        private class ItemEditorInfo
        {
            public string ItemTypeName { get; set; }
            public string EditorClassName { get; set; }
            public string EditorAssembly { get; set; }
        }

        //Simple cache for item editor info
        private static Dictionary<string, ItemEditorInfo> _infoCache;


        /// <summary>
        /// Initialize internal information
        /// </summary>
        static ItemEditorFactory()
        {
            lock (typeof(ItemEditorFactory))
            {
                _infoCache = new Dictionary<string, ItemEditorInfo>(StringComparer.InvariantCultureIgnoreCase);

                LoadInfoCache();
            }
        }

        /// <summary>
        /// Load item typoe info cache
        /// </summary>
        private static void LoadInfoCache()
        {
            _infoCache.Clear();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemEditors_List");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int itemTypeId = DbUtility.GetValueFromDataReader(reader, "ItemTypeId", -1);
                        string itemName = DbUtility.GetValueFromDataReader(reader, "ItemName", string.Empty);
                        string editorClass = DbUtility.GetValueFromDataReader(reader, "EditorTypeName", string.Empty);
                        string editorAssembly = DbUtility.GetValueFromDataReader(reader, "EditorTypeAssembly", string.Empty);

                        if (itemTypeId > 0
                            && Utilities.IsNotNullOrEmpty(itemName)
                            && Utilities.IsNotNullOrEmpty(editorClass)
                            && Utilities.IsNotNullOrEmpty(editorAssembly))
                        {
                            _infoCache[itemName] = new ItemEditorInfo
                            {
                                ItemTypeName = itemName,
                                EditorClassName = editorClass,
                                EditorAssembly = editorAssembly
                            };
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Create an item editor for the specified item type name
        /// </summary>
        /// <param name="itemTypeName"></param>
        /// <returns></returns>
        public static IItemEditor CreateEditor(string itemTypeName)
        {
            if (Utilities.IsNullOrEmpty(itemTypeName)
                || !_infoCache.ContainsKey(itemTypeName))
            {
                return null;
            }

            ItemEditorInfo info = _infoCache[itemTypeName];

            Type editorType = GetType(string.Format("{0},{1}", info.EditorClassName, info.EditorAssembly));

            if (editorType != null)
            {
                return CreateObject(editorType);
            }

            return null;
        }


        ///// <summary>
        ///// Create an item editor
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="appearance"></param>
        ///// <param name="languageCode"></param>
        ///// <param name="pagePosition"></param>
        ///// <param name="currentPrincipal"></param>
        ///// <returns></returns>
        //public static ItemEditor Create(ItemData data, AppearanceData appearance, string languageCode, int? pagePosition, ExtendedPrincipal currentPrincipal)
        //{
        //    // use the appearance code to lookup the associated item editor
        //    DataRow[] result = LookupTable.Select("ItemTypeID = " + data.ItemTypeID);

        //    if (result.Length > 0)
        //    {
        //        string editorType = (string)result[0]["EditorTypeName"] + "," + (string)result[0]["EditorTypeAssembly"];
        //        Type type = GetType(editorType);
        //        ItemEditor editor = CreateObject(type);
        //        editor.Initialize(data, appearance, languageCode, pagePosition, currentPrincipal);
        //        return editor;
        //    }
            
        //    throw new Exception("Unable to locate editor for type.");
        //}

        ///// <summary>
        ///// Create an instance of an item editor
        ///// </summary>
        ///// <param name="data"></param>
        ///// <param name="appearance"></param>
        ///// <param name="decorator"></param>
        ///// <param name="pagePosition"></param>
        ///// <param name="currentPrincipal"></param>
        ///// <returns></returns>
        //public static ItemEditor Create(ItemData data, AppearanceData appearance, ItemTextDecorator decorator, int? pagePosition, ExtendedPrincipal currentPrincipal)
        //{
        //    // use the appearance code to lookup the associated item editor
        //    DataRow[] result = LookupTable.Select("ItemTypeID = " + data.ItemTypeID);

        //    if (result.Length > 0)
        //    {
        //        string editorType = (string)result[0]["EditorTypeName"] + "," + (string)result[0]["EditorTypeAssembly"];
        //        Type type = GetType(editorType);
        //        ItemEditor editor = CreateObject(type);
        //        editor.Initialize(data, appearance, decorator, pagePosition, currentPrincipal);
        //        return editor;
        //    }
            
            
        //    throw new Exception("Unable to locate editor for type.");
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IItemEditor CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsItemEditor(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("ItemEditor does not have a constructor: " + type.FullName);
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

            return (IItemEditor)createdObject;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="type"></param>
        private static void ValidateTypeIsItemEditor(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(IItemEditor).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between ItemData type [" + typeof(IItemEditor).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
