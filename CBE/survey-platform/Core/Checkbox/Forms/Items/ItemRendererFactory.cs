using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Checkbox.Common;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Factory for creating item renderers
    /// </summary>
    public static class ItemRendererFactory
    {
        /// <summary>
        /// Container class for storing renderer type information.
        /// </summary>
        public class RendererTypeInfo
        {
            /// <summary>
            /// Get/set appearance code
            /// </summary>
            public string AppearanceCode { get; set; }

            /// <summary>
            /// Class name for renderer
            /// </summary>
            public string ClassName { get; set; }

            /// <summary>
            /// Assembly name for renderer
            /// </summary>
            public string AssemblyName { get; set; }
        }


        private static Dictionary<string, RendererTypeInfo> _rendererTypeInfo;

        private static readonly object _lockObject = new object();

        /// <summary>
        /// Get renderer info
        /// </summary>
        private static Dictionary<string, RendererTypeInfo> RendererInfo
        {
            get
            {
                if (_rendererTypeInfo == null)
                {
                    lock (_lockObject)
                    {
                        //Recheck for null, in case dictionary was populated between check
                        // and lock.
                        if (_rendererTypeInfo == null)
                        {
                            //Create dictionary
                            _rendererTypeInfo = new Dictionary<string, RendererTypeInfo>(StringComparer.InvariantCultureIgnoreCase);

                            //Load info
                            LoadRendererInfo(_rendererTypeInfo);
                        }
                    }
                }

                return _rendererTypeInfo;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="appearanceCode"></param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public static IItemRenderer Create(string appearanceCode)
        {
            if (RendererInfo.ContainsKey(appearanceCode))
            {
                RendererTypeInfo rendererInfo = RendererInfo[appearanceCode];
            
                Type type = GetType(rendererInfo.ClassName + ", " + rendererInfo.AssemblyName);

                IItemRenderer renderer = CreateObject(type);
                return renderer;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IItemRenderer CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsItemRenderer(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("ItemRenderer does not have a constructor: " + type.FullName);
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

            return (IItemRenderer)createdObject;
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
        private static void ValidateTypeIsItemRenderer(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(IItemRenderer).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between ItemRenderer type [" + typeof(IItemRenderer).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// Populate renderer dictionary
        /// </summary>
        /// <param name="typeInfo"></param>
        private static void LoadRendererInfo(Dictionary<string, RendererTypeInfo> typeInfo)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemRenderers_List");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string appearanceCode = DbUtility.GetValueFromDataReader(reader, "AppearanceCode", string.Empty);
                        string className = DbUtility.GetValueFromDataReader(reader, "RendererTypeName", string.Empty);
                        string assemblyName = DbUtility.GetValueFromDataReader(reader, "RendererAssemblyName", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(appearanceCode)
                            && Utilities.IsNotNullOrEmpty(className)
                            && Utilities.IsNotNullOrEmpty(assemblyName))
                        {
                            typeInfo[appearanceCode] = new RendererTypeInfo
                            {
                                AppearanceCode = appearanceCode,
                                ClassName = className,
                                AssemblyName = assemblyName
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
    }
}

