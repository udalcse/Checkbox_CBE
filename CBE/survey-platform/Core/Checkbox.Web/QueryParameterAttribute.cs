using System;
using System.Web;
using Checkbox.Common;

namespace Checkbox.Web
{
    /// <summary>
    ///     A <see cref="WebParameterAttribute" /> that's specifically bound to the
    ///     a parameter in the query string (Request.QueryString collection)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class QueryParameterAttribute : WebParameterAttribute
    {
        /// <summary>
        ///     Retrieves an item from the QueryString by key
        /// </summary>
        protected override object GetValue(string paramName, HttpContext context)
        {
            var value = context.Server.HtmlEncode(HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(context.Request.Url.Query))[paramName]);

            if (Utilities.IsNullOrEmpty(value))
            {
                if (IsRequired)
                {
                    throw new Exception("Query parameter [" + paramName + "] is required for this page.");
                }

                return DefaultValue;
            }

            return value;
        }

        #region Constructors

        /// <summary>
        ///     Creates a new QueryParameterAttribute to load a field from an identically-named
        ///     parameter in the QueryString collection, if it exists.
        ///     The parameter has no default value, and is not required
        /// </summary>
        public QueryParameterAttribute()
        {
        }

        /// <summary>
        ///     Creates a new QueryParameterAttribute to load a field from the given parameter
        ///     in the QueryString collection, if it exists.
        ///     The parameter has no default value, and is not required
        /// </summary>
        /// <param name="paramName">The key of a parameter in the QueryString collections</param>
        public QueryParameterAttribute(string paramName) : base(paramName)
        {
        }

        /// <summary>
        ///     Creates a new QueryParameterAttribute to load a field from the given parameter
        ///     in the QueryString collection, if it exists.
        ///     If the paramter is not found the default value is used.
        /// </summary>
        /// <param name="paramName">The key of a parameter in the QueryString collections</param>
        /// <param name="defaultValue">The default value to use if the paramter can not be found</param>
        public QueryParameterAttribute(string paramName, string defaultValue)
            : base(paramName, defaultValue)
        {
        }

        #endregion
    }
}