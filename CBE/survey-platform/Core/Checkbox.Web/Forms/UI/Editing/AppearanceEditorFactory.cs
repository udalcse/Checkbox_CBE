using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Checkbox.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Common;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Creates the 
    /// </summary>
    public static class AppearanceEditorFactory
    {
        /// <summary>
        /// Simple container for item editor type information
        /// </summary>
        private class AppearanceEditorInfo
        {
            public string AppearanceCode { get; set; }
            public string EditorClassName { get; set; }
            public string EditorAssembly { get; set; }
        }

        private static readonly Dictionary<string, AppearanceEditorInfo> _infoCache;

        /// <summary>
        /// Initialize internal collections
        /// </summary>
        static AppearanceEditorFactory()
        {
            lock (typeof(AppearanceEditorFactory))
            {
                _infoCache = new Dictionary<string, AppearanceEditorInfo>(StringComparer.InvariantCultureIgnoreCase);

                LoadInfoCache();
            }
        }

        /// <summary>
        /// Populate type info cache
        /// </summary>
        private static void LoadInfoCache()
        {
            _infoCache.Clear();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_AppearanceEditors_List");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string appearanceCode = DbUtility.GetValueFromDataReader(reader, "AppearanceCode", string.Empty);
                        string editorClass = DbUtility.GetValueFromDataReader(reader, "EditorTypeName", string.Empty);
                        string editorAssembly = DbUtility.GetValueFromDataReader(reader, "EditorTypeAssembly", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(appearanceCode)
                            && Utilities.IsNotNullOrEmpty(editorClass)
                            && Utilities.IsNotNullOrEmpty(editorAssembly))
                        {
                            _infoCache[appearanceCode] = new AppearanceEditorInfo
                            {
                                AppearanceCode = appearanceCode,
                                EditorAssembly = editorAssembly,
                                EditorClassName = editorClass
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
        /// Create an editor for configuring appearance data
        /// </summary>
        /// <param name="appearanceCode"></param>
        public static IAppearanceEditor CreateEditor(string appearanceCode)
        {
            if (Utilities.IsNullOrEmpty(appearanceCode)
                || !_infoCache.ContainsKey(appearanceCode))
            {
                return null;
            }

            AppearanceEditorInfo info = _infoCache[appearanceCode];

            Type editorType = GetType(string.Format("{0},{1}", info.EditorClassName, info.EditorAssembly));

            if (editorType != null)
            {
                return CreateObject(editorType);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IAppearanceEditor CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsAppearanceEditor(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("AppearanceEditor does not have a constructor: " + type.FullName);
            }

            object createdObject = null;

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
            catch (System.Exception ex)
            {
                throw ex;
            }

            return (IAppearanceEditor)createdObject;
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
        private static void ValidateTypeIsAppearanceEditor(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(IAppearanceEditor).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between ItemData type [" + typeof(IAppearanceEditor).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
