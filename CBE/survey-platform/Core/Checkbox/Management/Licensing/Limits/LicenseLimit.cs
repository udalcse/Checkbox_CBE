using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Checkbox.LicenseLibrary;

//using Xheo.Licensing;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Abstract base class for license limits
    /// </summary>
    public abstract class LicenseLimit : ILicenseLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public static readonly Dictionary<string, Type> LimitTypesByName = new Dictionary<string,Type>();
        public static readonly Type[] LimitTypes;

        /// <summary>
        /// 
        /// </summary>
		static LicenseLimit()
		{
		    Type baseType = typeof(LicenseLimit);
			Type[] types = baseType.Assembly.GetTypes();
            
            LimitTypes = types.Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract).ToArray();

            foreach (Type t in LimitTypes)
            {
                try
                {
                    ILicenseLimit limit = Activator.CreateInstance(t, null) as ILicenseLimit;
                    if (!string.IsNullOrEmpty(limit.LimitName))
                    {
                        LimitTypesByName.Add(limit.LimitName, t);
                    }
                }
                catch (Exception ex)
                {
                }                
            }
		}

        /// <summary>
        /// Get the limit type name
        /// </summary>
        public virtual string LimitTypeName { get { return GetType().Name; } }

		/// <summary>
		/// Get Limit type by name
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static Type GetLimit(string typeName)
		{
            if (LimitTypesByName.ContainsKey(typeName))
                return LimitTypesByName[typeName];
            else
                return (from t in LimitTypes where t.Name.Equals(typeName) select t).FirstOrDefault();
		}

        /// <summary>
		/// 
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static LicenseLimit Create(string typeName, string limitValue)
		{
			Type type = GetLimit(typeName);

			if (type == null)
				return null;

			return Activator.CreateInstance(type, limitValue) as LicenseLimit;
		}

        /// <summary>
        /// Initialize the limit.
        /// </summary>
        /// <param name="license">License to initialize the limit with.</param>
        public virtual void Initialize(CheckboxLicenseData license)
        {
        }
        
        /// <summary>
        /// Get the limit name
        /// </summary>
        public virtual string LimitName { get { return GetType().Name; } }

		/// <summary>
		/// Get the limit value as string
		/// </summary>
        public virtual string LimitValue { get { return null; } }

        /// <summary>
        /// Validate the limit and set a string description of the error.
        /// </summary>
        /// <param name="messageTextId">TextID of the error message to display.</param>
        /// <returns>Boolean indicating if the limit is valid.</returns>
        public virtual LimitValidationResult Validate(out string messageTextId)
        {
            messageTextId = "";
            return LimitValidationResult.LimitNotReached;
        }
    }
}
