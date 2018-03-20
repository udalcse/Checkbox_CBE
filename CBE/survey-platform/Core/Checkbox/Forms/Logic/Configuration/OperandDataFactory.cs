using System;
using System.Reflection;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Factory for creating operand data objects.
    /// </summary>
    public class OperandDataFactory
    {
		static Type[] _operandTypes = new Type[] { 
			typeof(ItemOperandData), 
			typeof(OptionOperandData), 
			typeof(ProfileOperandData), 
			typeof(ResponseOperandData), 
			typeof(StringOperandData),
            typeof(CategorizedMatrixItemOperandData)
		};

		/// <summary>
        /// Get the <see cref="System.Type"/> from a type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
		private static Type GetOperandType(string typeName)
		{
			foreach( Type type in _operandTypes)
				if (type.Name == typeName)
					return type;

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static OperandData CreateOperandDataObject(string typeName)
		{
			Type type = GetOperandType(typeName);

			if (type == null)
				return null;

			return Activator.CreateInstance(type, true) as OperandData;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal OperandData CreateOperandData(string typeName, params object[] args)
        {
            Type t = GetType(typeName);
            return CreateObject(t, args);
        }

        /// <summary>
        /// Create an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static OperandData CreateObject(Type type, object[] args)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsOperandData(type);

            Type[] constructorArgTypes = new Type[args.Length];

            for (int i = 0; i < constructorArgTypes.Length; i++)
            {
                constructorArgTypes[i] = args[i].GetType();
            }

            ConstructorInfo constructor = type.GetConstructor(constructorArgTypes);

            if (constructor == null)
            {
                throw new Exception("Operand does not have a constructor: " + type.FullName);
            }

            object createdObject;

            try
            {
                createdObject = constructor.Invoke(args);
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

            return (OperandData)createdObject;
        }

        /// <summary>
        /// Get the <see cref="System.Type"/> from a type name.
        /// </summary>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        private static Type GetType(string fullTypeName)
        {
            ArgumentValidation.CheckForEmptyString(fullTypeName, "typeName");
            try
            {
                //Get the type
                // Throw error on failure = true
                // Ignore case = false
                return Type.GetType(fullTypeName, true, false);

            }
            catch (TypeLoadException ex)
            {
                throw new Exception("A type-loading error occurred.  Type was: " + fullTypeName, ex);
            }
        }

        private static void ValidateTypeIsOperandData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(OperandData).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between OperandData type [" + typeof(OperandData).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
