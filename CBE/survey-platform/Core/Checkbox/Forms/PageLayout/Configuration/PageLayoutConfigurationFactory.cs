//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Reflection;
using System.Configuration;

using Prezza.Framework.Common;

namespace Checkbox.Forms.PageLayout.Configuration
{
    /// <summary>
    /// Factory for creating page layout configuration objects.
    /// </summary>
    internal static class PageLayoutConfigurationFactory
    {
        /// <summary>
        /// Create an page layout data of the specified type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PageLayoutTemplateData CreatePageLayoutTemplateData(string typeName)
        {
            return CreateObject(GetType(typeName));
        }

        /// <summary>
        /// Create an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static PageLayoutTemplateData CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsPageLayoutTemplateData(type);

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
            

            return (PageLayoutTemplateData)createdObject;
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
        /// Validate that the specified <see cref="System.Type"/> is a valid <see cref="PageLayoutTemplateData"/> type.
        /// </summary>
        /// <param name="type"></param>
        private static void ValidateTypeIsPageLayoutTemplateData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(PageLayoutTemplateData).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between PageLayoutTemplateData type [" + typeof(PageLayoutTemplateData).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
