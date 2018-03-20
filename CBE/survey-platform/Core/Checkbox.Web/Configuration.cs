using System;
using System.Web;
using Checkbox.Configuration.Install;

namespace Checkbox.Web
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
	public static class Configuration
	{
        /// <summary>
        /// Discovers the application root directory for this application domain
        /// </summary>
        /// <returns>the paty of the application root directory</returns>
        public static string DiscoverApplicationRoot()
        {
            // try to get the appRootDir
            string appRoot = HttpContext.Current.Request.ServerVariables["appl_physical_path"];

            if(HttpContext.Current.Cache["prezza_appRootDirectory"] == null)
            {
                HttpContext.Current.Cache.Insert("prezza_appRootDirectory", appRoot, null, DateTime.MaxValue, TimeSpan.Zero);
            }
            else if(appRoot != (string)HttpContext.Current.Cache["prezza_appRootDirectory"])
            {
                // the root dir has changed, reset it
                HttpContext.Current.Cache.Remove("prezza_appRootDirectory");
                HttpContext.Current.Cache.Insert("prezza_appRootDirectory", appRoot, null, DateTime.MaxValue, TimeSpan.Zero);
            }

            if(HttpContext.Current.Cache["prezza_appRootDirectory"] == null)
                throw new ApplicationException("Application Root Directory was not found");

            return HttpContext.Current.Cache["prezza_appRootDirectory"].ToString();
        }

	    /// <summary>
        /// Indicates if a given module is installed
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static bool IsModuleInstalled(string moduleName)
        {
	        ApplicationInstaller appInstaller = (ApplicationInstaller)HttpContext.Current.Session["AppInstaller"];
            if (appInstaller == null)
            {
                appInstaller = new ApplicationInstaller(HttpContext.Current.Server.MapPath("~"));
                HttpContext.Current.Session["AppInstaller"] = appInstaller;
            }

            if (appInstaller.GetInstalledModuleInfo(moduleName) != null)
            {
                return true;
            }

	        return false;
        }
	}
}
