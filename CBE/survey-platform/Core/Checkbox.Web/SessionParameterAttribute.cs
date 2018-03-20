using System;

namespace Checkbox.Web
{
	/// <summary>
	/// Summary description for SessionParameterAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public sealed class SessionParameterAttribute : WebParameterAttribute
	{
		#region Constructors
		/// <summary>
		/// Creates a new SessionParameterAttribute to load a field from an identically-named
		/// parameter in the Session, if it exists.
		/// The parameter has no default value, and is not required
		/// </summary>
		public SessionParameterAttribute() 
		{
		}

		/// <summary>
		/// Creates a new SessionParameterAttribute to load a field from the given parameter
		/// in the Session, if it exists.
		/// The parameter has no default value, and is not required
		/// </summary>
		/// <param name="paramName">The key of a parameter in the QueryString collections</param>
		public SessionParameterAttribute(string paramName) : base(paramName) 
		{
		}
		#endregion
		
		/// <summary>
		/// Retrieves an item from the SessionParameterAttribute by key
		/// </summary>
		protected override object GetValue(string paramName, System.Web.HttpContext context)
		{
			return context.Session[paramName];
		}
	}
}
