using System;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Web
{
    /// <summary>
    /// Memento class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Memento : WebParameterAttribute
    {
        #region Constructors
		/// <summary>
		/// Creates a new WebParameterAttribute to load a field from an identically-named
		/// parameter in the Session specific to a page collection, if it exists.
		/// The parameter has no default value, and is not required
		/// </summary>
		public Memento() {
		}

		/// <summary>
		/// Creates a new QueryParameterAttribute to load a field from the given parameter
		/// in the QueryString collection, if it exists.
		/// The parameter has no default value, and is not required
		/// </summary>
		/// <param name="paramName">The key of a parameter in the QueryString collections</param>
		public Memento(string paramName) : base(paramName) {
		}

        /// <summary>
        /// Creates a new QueryParameterAttribute to load a field from the given parameter
        /// in the QueryString collection, if it exists.
        /// If the paramter is not found the default value is used.
        /// </summary>
        /// <param name="paramName">The key of a parameter in the QueryString collections</param>
        /// <param name="defaultValue">The default value to use if the paramter can not be found</param>
        public Memento(string paramName, string defaultValue)
            : base(paramName, defaultValue)
        {
        }

	    #endregion

        /// <summary>
        /// Persist any mementos
        /// </summary>
        /// <param name="target"></param>
        /// <param name="context"></param>
        public static void PersistMementos(object target, HttpContext context)
        {
            System.Type type = target.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            MemberInfo[] members = new MemberInfo[fields.Length + properties.Length];
            fields.CopyTo(members, 0);
            properties.CopyTo(members, fields.Length);

            for (int f = 0; f < members.Length; f++)
            {
                PersistMementoValue(members[f], target, context);
            }
        }

        /// <summary>
        /// Persist any mementos
        /// </summary>
        /// <param name="target"></param>
        /// <param name="context"></param>
        public static void ClearMementos(object target, HttpContext context)
        {
            System.Type type = target.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            MemberInfo[] members = new MemberInfo[fields.Length + properties.Length];
            fields.CopyTo(members, 0);
            properties.CopyTo(members, fields.Length);

            for (int f = 0; f < members.Length; f++)
            {
                ClearMementoValue(members[f], target, context);
            }
        }

        /// <summary>
        /// Persist memento value
        /// </summary>
        /// <param name="member"></param>
        /// <param name="target"></param>
        /// <param name="context"></param>
        public static void PersistMementoValue(MemberInfo member, object target, HttpContext context)
        {
            try
            {
                Memento[] attribs = (Memento[])member.GetCustomAttributes(typeof(Memento), true);

                //Only one memento is supported per member/field
                if (attribs.Length > 0)
                {
                    Memento memento = attribs[0];

                    object memberValue = GetMemberValue(member, target, context);

                    memento.PersistValue((memento.ParameterName != null) ? memento.ParameterName : member.Name, memberValue, context);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clear memento value
        /// </summary>
        /// <param name="member"></param>
        /// <param name="target"></param>
        /// <param name="context"></param>
        public static void ClearMementoValue(MemberInfo member, object target, HttpContext context)
        {
            try
            {
                Memento[] attribs = (Memento[])member.GetCustomAttributes(typeof(Memento), true);

                //Only one memento is supported per member/field
                if (attribs.Length > 0)
                {
                    Memento memento = attribs[0];

                    memento.ClearValue(memento.ParameterName ?? member.Name, context);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Get the storage key specific to this page and parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetKey(string paramName, HttpContext context)
        {
            return context.Request.Url.AbsolutePath + "_Memento_" + paramName;
        }

        /// <summary>
        /// Get the parameter value
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override object GetValue(string paramName, HttpContext context)
        {
            object value = context.Session[GetKey(paramName, context)];

            if (value == null && DefaultValue != null)
            {
                return DefaultValue;
            }
            
            return value;
        }

        /// <summary>
        /// Clear memento value
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="context"></param>
        protected void ClearValue(string paramName, HttpContext context)
        {
            context.Session.Remove(GetKey(paramName, context));
        }

        /// <summary>
        /// Persist the object value
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        protected override void PersistValue(string paramName, object value, HttpContext context)
        {
            context.Session[GetKey(paramName, context)] = value;
        }
    }
}
