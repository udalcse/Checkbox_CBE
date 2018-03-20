using System;
using System.Reflection;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    internal class ActionDataFactory
    {
		static Type[] _actionTypes = new Type[] { 
			typeof(IncludeExcludeActionData), 
			typeof(BranchPageActionData)
		};

		/// <summary>
		/// Get the <see cref="System.Type"/> from a type name.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static Type GetActionType(string typeName)
		{
			foreach (Type type in _actionTypes)
				if (type.Name == typeName)
					return type;

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static ActionData CreateActionDataObject(string typeName)
		{
			Type type = GetActionType(typeName);

			if (type == null)
				return null;

			return Activator.CreateInstance(type, true) as ActionData;
		}

        internal ActionData CreateActionData(string typeName, params object[] constructorArgs)
        {
            return CreateObject(constructorArgs, GetType(typeName));
        }

        private static ActionData CreateObject(object[] args, Type type)
        {
            //Validate arguments
            ArgumentValidation.CheckForNullReference(type, "type");

            ValidateTypeIsActionData(type);

            var cargs = new Type[args.Length];
            for (int x = 0; x < args.Length; x++)
            {
                cargs[x] = args[x].GetType();
            }

            ConstructorInfo constructor = type.GetConstructor(cargs);

            if (constructor == null)
            {
                throw new Exception("Action does not have a constructor: " + type.FullName);
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

            return (ActionData)createdObject;
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

        private static void ValidateTypeIsActionData(Type type)
        {
            ArgumentValidation.CheckForNullReference(type, "type");

            if (!typeof(ActionData).IsAssignableFrom(type))
            {
                throw new Exception("Type mismatch between Action type [" + typeof(ActionData).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
            }
        }
    }
}
