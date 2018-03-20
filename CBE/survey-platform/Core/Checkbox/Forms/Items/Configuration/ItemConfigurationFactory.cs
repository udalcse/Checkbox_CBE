//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Reflection;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Factory for creating item configuration objects.
    /// </summary>
    internal class ItemConfigurationFactory
    {
        /// <summary>
        /// Create an item data of the specified type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ItemData CreateItemData(string typeName)
        {
            return CreateObject(GetType(typeName));
        }

        /// <summary>
        /// Create an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ItemData CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");
            
            ValidateTypeIsItemData(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("ItemData does not have a constructor: " + type.FullName);
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
            catch (Exception ex)
            {
                throw ex;
            }

            return (ItemData)createdObject;
        }

        /// <summary>
        /// Get the <see cref="System.Type"/> from a type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private Type GetType(string typeName)
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
        private void ValidateTypeIsItemData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(ItemData).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between ItemData type [" + typeof(ItemData).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
