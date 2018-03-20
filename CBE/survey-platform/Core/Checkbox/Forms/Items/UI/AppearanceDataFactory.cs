using System;
using System.Reflection;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.UI
{
    internal class AppearanceDataFactory
    {
        public AppearanceData CreateAppearanceData(string typeName)
        {
            return CreateObject(GetType(typeName));
        }

        private static AppearanceData CreateObject(Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsAppearanceData(type);

            ConstructorInfo constructor = type.GetConstructor(new Type[] { });

            if (constructor == null)
            {
                throw new Exception("ItemData does not have a constructor: " + type.FullName);
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

            return (AppearanceData)createdObject;
        }

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

        private static void ValidateTypeIsAppearanceData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(AppearanceData).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between ItemData type [" + typeof(AppearanceData).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
