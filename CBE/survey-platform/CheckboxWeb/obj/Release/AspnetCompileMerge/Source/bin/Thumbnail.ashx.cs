using System.Web;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public class Thumbnail : IHttpHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpg";
            context.Response.WriteFile(context.Server.MapPath("~/App_Themes/CheckboxTheme/images/form16.gif"));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable { get { return false; } }
    }
}
