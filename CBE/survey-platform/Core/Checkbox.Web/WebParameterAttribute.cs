using System;
using System.Web;
using System.ComponentModel;
using System.Reflection;
using Checkbox.Common;

namespace Checkbox.Web
{
	/// <summary>
	/// Marks a field or property as being bound to a specific parameter present in the
	/// <see cref="System.Web.HttpRequest"/>. This attribute is normally only
	/// applied to subclasses of <see cref="System.Web.UI.Page"/>
	/// </summary>
	/// <example>
	/// Here a simple page class marks field with the attribute, and then
	/// calls the static WebParameterAttribute.SetValues() method to
	/// automatically load the fields with value from Request.Form or Request.QueryString
	/// (depending on what was used to submit the form). Note that since
	/// parameter binding in this example is done both on first-request
	/// and on postback, this page must always be either linked to supplying
	/// data in the querystring, or cross-posted to with the data in the Form.
	/// <code><![CDATA[
	/// public class BoundParameterDemo : Checkbox.Web.Page.BasePage{
	///		[WebParameter()]
	///		protected string FirstName;
	///
	///		[WebParameter("Last_Name")]
	///		protected string LastName;
	///
	///		[WebParameter(IsRequired=true)]
	///		protected int CustomerID;
	///
	///		private void Page_Load(object sender, System.EventArgs e) {
	///			WebParameterAttribute.SetValues(this, Request);
	///		}
	///	}
	/// ]]>
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public class WebParameterAttribute : Attribute
	{
		#region Declarations

	    #endregion

		#region Constructors
		/// <summary>
		/// Creates a new WebParameterAttribute to load a field from an identically-named
		/// parameter in the Form/QueryString collection, if it exists.
		/// The parameter has no default value, and is not required
		/// </summary>
		public WebParameterAttribute()
		{
		}

		/// <summary>
		/// Creates a new WebParameterAttribute to load a field from the given parameter name
		/// The parameter has no default value, and is not required
		/// </summary>
		/// <param name="paramName">The key of a parameter in the Form or QueryString collections</param>
		public WebParameterAttribute(string paramName)
		{
			ParameterName		=paramName;
		}

        /// <summary>
        /// Creates a new WebParameterAttribute to load a field from the given parameter name
        /// </summary>
        /// <param name="paramName">The key of a parameter in the Form or QueryString collections</param>
        /// <param name="defaultValue">The default value to use if the paramter can not be found</param>
        public WebParameterAttribute(string paramName, string defaultValue)
        {
            ParameterName = paramName;
            DefaultValue = defaultValue;
        }
		#endregion

	    /// <summary>
	    /// The name (key) of the parameter being bound against in the Request
	    /// </summary>
	    public string ParameterName { get; set; }

	    /// <summary>
	    /// An optional default value to use if the parameter doesn't exist
	    /// in the current Request, or null to clear
	    /// </summary>
	    /// <remarks>Whilst this is a bit unneccesary for a field, its
	    /// handy for properties - can save all that <code>if(ViewState["x"]==null)</code>
	    /// stuff...</remarks>
	    public string DefaultValue { get; set; }

	    /// <summary>
	    /// Whether the absence of the parameter, along with the absence
	    /// of a default, causes an error, rather than the default
	    /// behaviour which is that the field will just be skipped.
	    /// The default is false.
	    /// </summary>
	    public bool IsRequired { get; set; }

	    /// <summary>
	    /// Whether the default value can be used if the value passed to
	    /// the page is invalid in some way (rejected by the type converter,
	    /// or causes an error on the field/property set).
	    /// The default is false.
	    /// </summary>
	    public bool IsDefaultUsedForInvalid { get; set; }

	    /// <summary>
		/// Retrieves an item either from the Query or POST collections, depending on the
		/// mode of the request, or performs custom retrieval in derived classes
		/// </summary>
		/// <remarks>Typically a subclass would overide this to bind a parameter
		/// specifically to - say - Request.Form, or even an item in the Cookies
		/// collection.
		/// The parameter name has to be passed since some attributes have
		/// no ParameterName, marking them as using the same name as
		/// the member (which is unknown to the attribute)</remarks>
		protected virtual object GetValue(string paramName, HttpContext context)
	    {
	        return context.Request.HttpMethod.ToLower()=="post" 
                ? context.Request.Form[paramName] 
                : context.Request.QueryString[paramName];
	    }

	    /// <summary>
        /// Persist an attribute value...if that is supported by a child class
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="context"></param>
        protected virtual void PersistValue(string paramName, object value, HttpContext context)
        {
        }
        
        /// <summary>
        /// Get the value of a member
        /// </summary>
        /// <param name="member"></param>
        /// <param name="target"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected static object GetMemberValue(MemberInfo member, object target, HttpContext context)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).GetValue(target);
            }
            
            if (member.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)member).GetValue(target, null);
            }
            
            return null;
        }

	    /// <summary>
		/// Sets public properties and fields on <c>target</c> that are marked with
		/// <see cref="WebParameterAttribute"/> to the corresponding values retrieved from
		/// <c>request</c>, or a default value as set on the attribute
		/// </summary>
		/// <param name="target">The object (typically a <see cref="System.Web.UI.Page"/>) being bound</param>
		/// <param name="context">The <see cref="System.Web.HttpContext"/> to load the data from.
		/// The attribute determines whether data is loaded from request.Form, request.QueryString
		/// or other parts of request</param>
		public static void SetValues(object target, HttpContext context)
		{
			Type type			        =target.GetType();
			FieldInfo[] fields			=type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] properties	=type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			MemberInfo[] members		=new MemberInfo[fields.Length + properties.Length];
			fields.CopyTo(members, 0);
			properties.CopyTo(members, fields.Length);

			for(int f=0; f<members.Length; f++)
				SetValue(members[f], target, context);
		}

		/// <summary>
		/// Examines a single <c>member</c> (a property or field) for <see cref="WebParameterAttribute"/>.
		/// If so marked then the member is set on <c>target</c> with the relevant value
		/// retrieved from <c>request</c>, or the default value provided in the attribute
		/// </summary>
		/// <param name="member">Member to examine.</param>
		/// <param name="target">The object (typically a <see cref="System.Web.UI.Page"/>) being bound</param>
		/// <param name="context">The <see cref="System.Web.HttpContext"/> to load the data from.
		/// The attribute determines whether data is loaded from request.Form, request.QueryString
		/// or other parts of request</param>
		public static void SetValue(MemberInfo member, object target, HttpContext context)
		{
			WebParameterAttribute[] attribs;
			WebParameterAttribute attrib = null;
			TypeConverter converter;
			object paramValue;
			string paramName;
			object typedValue;
			bool usingDefault;

			try
			{
				attribs	=(WebParameterAttribute[])member.GetCustomAttributes(typeof(WebParameterAttribute), true);
				if(attribs!=null && attribs.Length>0)
				{
					// indexed property not supported
					if (member.MemberType==MemberTypes.Property)
					{
						ParameterInfo[] ps	=((PropertyInfo)member).GetIndexParameters();
						if (ps.Length > 0)
							throw new NotSupportedException("Cannot apply WebParameterAttribute to indexed property");
					}

					// There should only be one WebParameterAttribute (it's a single-use attribute)
					attrib		= attribs[0];
					paramName	= attrib.ParameterName ?? member.Name;
					paramValue	= attrib.GetValue(paramName, context);

					// Handle default value assignment, if required
					usingDefault = false;
					if (paramValue == null || (paramValue is string && ((string)paramValue).Length == 0))
					{
						if (attrib.DefaultValue != null)
						{
							paramValue = attrib.DefaultValue;
							usingDefault = true;
						}
						else if (!attrib.IsRequired)
							return;	// Just skip the member
						else
							throw new ApplicationException(String.Format("Missing required parameter '{0}'", paramName));
					}

					// Now assign the loaded value onto the member, using the relevant type converter
					// Have to perform the assignment slightly differently for fields and properties
					converter = TypeDescriptor.GetConverter(GetMemberUnderlyingType(member));

                    if (converter == null)
                    {
                        throw new ApplicationException(String.Format("Could not convert from {0}", paramValue.GetType()));
                    }

                    Type memberType = GetMemberUnderlyingType(member);

					try
					{
                        //Try is assignable from to handle base-class cases
                        if (!converter.CanConvertFrom(paramValue.GetType()))
                        {
                            if (memberType.IsAssignableFrom(paramValue.GetType()))
                            {
                                SetMemberValue(member, target, paramValue);
                            }
                        }
                        else
                        {
                            typedValue = converter.ConvertFrom(paramValue);
                            SetMemberValue(member, target, typedValue);
                        }
					}
					catch
					{
						// We catch errors both from the type converter
						// and from any problems in setting the field/property
						// (eg property-set rules, security, readonly properties)

						// If we're not already using the default, but there
						// is one, and we're allowed to use it for invalid data, give
						// it a go, otherwise just propagate the error

						if (!usingDefault && attrib.IsDefaultUsedForInvalid)
						{
                            if (attrib.DefaultValue == null)
                            {
                                SetMemberValue(member, target, null);
                            }
                            else
                            {
                                if (!converter.CanConvertFrom(attrib.DefaultValue.GetType()))
                                {
                                    if (memberType.IsAssignableFrom(attrib.DefaultValue.GetType()))
                                    {
                                        SetMemberValue(member, target, attrib.DefaultValue);
                                    }
                                }
                                else
                                {
                                    typedValue = converter.ConvertFrom(attrib.DefaultValue);
                                    SetMemberValue(member, target, typedValue);
                                }
                            }
						}
						else
							throw;
					}
				}
			}
			catch(Exception err)
			{
			    var message = Utilities.AdvancedHtmlDecode(err.Message);
			    message = Utilities.RemoveScript(message);
			    message = Utilities.AdvancedHtmlEncode(message);

                if (attrib != null)
				    throw new ApplicationException(string.Format("Property/field \"{0}\" could not be set from request - ", attrib.ParameterName) + message, err);

                throw new ApplicationException(message, err);
            }
        }

		/// <summary>
		/// Sets <c>member</c> on <c>target</c> to <c>value</c>. <c>member</c>
		/// may be a field or a property
		/// </summary>
		private static void SetMemberValue(MemberInfo member, object target, object value)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					((FieldInfo)member).SetValue(target, value);
					break;

				case MemberTypes.Property:
					// We've already ensured this isn't an indexed property
					((PropertyInfo)member).SetValue(target, value, new Object[0]);
					break;
			}
		}

		/// <summary>
		/// Retrieves the <see cref="Type"/> that <c>member</c> represents
		/// (that is to say the type of the member on the object, rather
		/// than the subtype of MemberInfo). <c>member</c> may be a
		/// <see cref="FieldInfo"/> or a <see cref="PropertyInfo"/>
		/// </summary>
		private static Type GetMemberUnderlyingType(MemberInfo member)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;

				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;

				default:
					throw new ArgumentException("Expected a FieldInfo or PropertyInfo", "member");
			}
		}
	}
}
