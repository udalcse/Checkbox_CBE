using System;
using System.Web;
using System.Xml;
using System.IO;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Recieves a file and stores it to the session
    /// </summary>
    public class UFrameFileUploadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState  
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(System.Web.HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Params["controlID"]))
            {
                context.Response.Write("Control ID not set.");
                return;
            }
            if (context == null || context.Request == null || context.Request.Files == null || context.Request.Files.Count == 0)
            {
                context.Response.Write("File not found.");
                return;
            }

            if (context.Session == null)
            {
                context.Response.Write("Session not found.");
                return;
            }                
            try
            {
                context.Session["file_" + context.Request.Params["controlID"]] = context.Request.Files[0];

                //we have to read the file till the end 
                byte[] buffer = new byte[context.Request.Files[0].ContentLength];
                context.Request.Files[0].InputStream.Read(buffer, 0, context.Request.Files[0].ContentLength);
                
                //and pass it as an additional key, otherwise it will be closed 
                context.Session["buffer_" + context.Request.Params["controlID"]] = buffer;

                context.Response.Write("OK");
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
