using System.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private const string CURRENT_EDITING_CONDITION_SOURCE = "CurrentEditingConditionValue";

        /// <summary>
        /// 
        /// </summary>
        public static string CurrentEditingConditionSource
        {
            set { HttpContext.Current.Session[CURRENT_EDITING_CONDITION_SOURCE] = value; }
            get { return HttpContext.Current.Session[CURRENT_EDITING_CONDITION_SOURCE] as string; }
        }
    }
}
