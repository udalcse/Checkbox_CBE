using System.Web;

using Prezza.Framework.Security;
using Prezza.Framework.Configuration;

namespace Checkbox.Web.Caching
{
	/// <summary>
	/// Summary description for AspNetSessionTokenProvider.
	/// </summary>
	public class AspNetSessionTokenProvider : ISessionTokenProvider
	{
	    #region ISessionTokenProvider

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IToken GetSessionToken()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["UserCacheToken"] != null)
                {
                    return (IToken)HttpContext.Current.Session["UserCacheToken"];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearSessionToken()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session["UserCacheToken"] = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public void SetSessionToken(IToken token)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session["UserCacheToken"] = token;
            }
        }
        #endregion

        #region IConfigurationProvider Members

	    /// <summary>
	    /// Name of the provider.
	    /// </summary>
	    public string ConfigurationName { get; set; }

	    /// <summary>
        /// Initialize the authentication provider with the supplied configuration object.
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ConfigurationBase config)
        {
        }

        #endregion
	}
}
