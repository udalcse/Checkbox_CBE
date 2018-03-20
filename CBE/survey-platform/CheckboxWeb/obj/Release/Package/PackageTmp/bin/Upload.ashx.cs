using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Checkbox.Forms;
using Checkbox.Management;
using Prezza.Framework.Caching;

namespace CheckboxWeb
{
    public class Upload : IHttpHandler, IReadOnlySessionState
    {
        /// <summary>
        /// 
        /// </summary>
        public class FilesStatus
        {
            /// <summary>
            /// URL to use to show thumbnail of file
            /// </summary>
            public string thumbnail_url { get; set; }

            /// <summary>
            /// Display name of the file.
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// Temp file name.  Depending on storage mode, may have multiple meanings:
            ///   1) S3 - Name of file in context-specific S3 bucket.
            ///   2) Web Farm w/out S3 - Cache key that can be used to access file data in default cache manager.
            ///   3) Temp File - Name of file within session-specific folder. e.g. c:\checkbox\temp\[SESSION_ID]\file
            /// </summary>
            public string TempName { get; set; }

            /// <summary>
            /// URL to access file
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// Size of flie
            /// </summary>
            public int size { get; set; }

            /// <summary>
            /// Type of file
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// URL to access to delete file
            /// </summary>
            public string delete_url { get; set; }

            /// <summary>
            /// HTTP Verb for delete operation
            /// </summary>
            public string delete_type { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string error { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string progress { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        private readonly JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        
        /// <summary>
        /// 
        /// </summary>
        private string IngestPath { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var r = context.Response;
            IngestPath = context.Server.MapPath("~/Temp");

            r.AddHeader("Pragma", "no-cache");
            r.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    ServeFile(context);
                    break;

                case "POST":
                    if ("Delete".Equals(context.Request["op"], StringComparison.InvariantCultureIgnoreCase))
                    {
                        DeleteFile(context);
                    }
                    else
                    {
                        UploadFile(context);
                    }
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void DeleteFile(HttpContext context)
        {
            var filePath = string.Format("{0}/{1}/{2}", IngestPath, context.Session.SessionID, context.Request["f"]);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }


            WriteJsonIframeSafe(context, statuses);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="context"></param>
        /// <param name="statuses"></param>
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (UploadItemManager.UseS3ForTempFiles || ApplicationManager.AppSettings.WebFarm)
            {
                throw new HttpRequestValidationException(
                    "Partial file upload not supported when storing temp files in S3 or when running in WebFarm mode.");
            }

            if (context.Request.Files.Count != 1)
            {
                throw new HttpRequestValidationException(
                    "Attempt to upload chunked file containing more than one fragment per request");
            }
        

            var inputStream = context.Request.Files[0].InputStream;

            var fileBytes = new byte[inputStream.Length];
            inputStream.Read(fileBytes, 0, (int) inputStream.Length);

            var tempFileName = StoreData(context, fileBytes);

            statuses.Add(
                new FilesStatus
                    {
                        thumbnail_url   = string.Empty,
                        url = ApplicationManager.ApplicationPath + "/Upload.ashx?f=" + context.Server.HtmlEncode(tempFileName) + "&n=" + context.Server.HtmlEncode(fileName),
                        TempName = tempFileName,
                        name = fileName,
                        size = (int)inputStream.Length,
                        type = "image/png",
                        delete_url = ApplicationManager.ApplicationPath + "/Upload.ashx?op=delete&f=" + tempFileName,
                        delete_type = "POST",
                        progress = "1.0"
                    });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statuses"></param>
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            var inputStream = context.Request.Files[0].InputStream;

            var fileBytes = new byte[inputStream.Length];
            inputStream.Read(fileBytes, 0, (int)inputStream.Length);

            var tempFileName = StoreData(context, fileBytes);

            statuses.Add(
                new FilesStatus
                {
                    thumbnail_url = string.Empty,
                    url = ApplicationManager.ApplicationPath + "/Upload.ashx?f=" + context.Server.HtmlEncode(tempFileName) + "&n=" + context.Server.HtmlEncode(context.Request.Files[0].FileName),
                    TempName = tempFileName,
                    name = context.Request.Files[0].FileName,
                    size = (int)inputStream.Length,
                    type = "image/png",
                    delete_url = ApplicationManager.ApplicationPath + "/Upload.ashx?op=delete&f=" + tempFileName,
                    delete_type = "POST",
                    progress = "1.0"
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statuses"></param>
        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                context.Response.ContentType = context.Request["HTTP_ACCEPT"].Contains("application/json")
                                                   ? "application/json"
                                                   : "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = jsSerializer.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void ServeFile(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request["f"]))
            {
                ListCurrentFiles(context);
            }
            else
            {

                DeliverFile(context);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void DeliverFile(HttpContext context)
        {
            var fileBytes = GetDataForFile(context, context.Request["f"]);

            if(fileBytes.Length > 0)
            {
                context.Response.ContentType = "application/octet-stream";
                context.Response.BinaryWrite(fileBytes);
                context.Response.AddHeader("Content-Disposition", "attachment, filename=\"" + context.Request["n"] + "\"");
                return;
            }

            context.Response.StatusCode = 404;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromUrl(string url)
        {
            byte[] buffer = null;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();

            Stream stream = myResp.GetResponseStream();
            if (stream != null)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes(5000000);
                    br.Close();
                }
            }
            myResp.Close();

            return buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] GetDataForFile(HttpContext context, string fileName)
        {
            var fileBytes = new byte[] { };

            //Depending on mode, get file
            if (UploadItemManager.UseS3ForTempFiles)
            {
                fileBytes = UploadItemManager.GetTempFileFromS3(1, fileName);
            }
            else if (ApplicationManager.AppSettings.WebFarm)
            {
                var data = CacheFactory.GetCacheManager()[fileName] as byte[];

                if (data != null)
                {
                    fileBytes = data;
                }
            }
            else
            {
                var filePath = string.Format("{0}/{1}/{2}", context.Server.MapPath("~/Temp"), context.Session.SessionID, fileName);
                if (File.Exists(filePath))
                {
                    using(var fs = File.OpenRead(filePath))
                    {
                        var buffer = new byte[fs.Length];

                        fs.Read(buffer, 0, (int)fs.Length);

                        fs.Close();

                        fileBytes = buffer;
                    }
                }
            }

            return fileBytes;
        }

        /// <summary>
        /// NOT USED
        /// </summary>
        /// <param name="context"></param>
        private void ListCurrentFiles(HttpContext context)
        {
            var names = new string[] {}; // Directory.GetFiles(@"C:\temp\ingest", "*", SearchOption.TopDirectoryOnly);

            context.Response.AddHeader("Content-Disposition", "inline, filename=\"files.json\"");
            //var jsonObj = jsSerializer.Serialize(names.Select(name => new FileInfo(name)).Select(f => new FilesStatus
            //                                                                                    {
            //                                                                                        ThumbnailUrl = "Thumbnail.ashx?f=" + f.Name, url = "Upload.ashx?f=" + f.Name, name = f.Name, size = (int) f.Length, type = "image/png", delete_url = "Upload.ashx?f=" + f.Name, delete_type = "DELETE"
            //                                                                                    }).ToArray());
            //context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }


        /// <summary>
        /// Store data and return name of temp location.  Significance of name will depend on whether data is stored in
        /// file system or S3 bucket.  The latter is a requirement for proper web farm operation.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string StoreData(HttpContext context, byte[] data)
        {
            string tempName;

            //Depending on config settings, write data to s3, http context cache, or 
            // temp folder.
            if (UploadItemManager.UseS3ForTempFiles)
            {
                tempName = UploadItemManager.SanitizeFileName(string.Format("{0}_{1}_Import.tmp", DateTime.Now.Ticks, context.Session.SessionID), ".");
                var s3Name = UploadItemManager.GetS3FileName(1, tempName);

                UploadItemManager.SaveTempFileToS3(s3Name, data);
            }
            else if (ApplicationManager.AppSettings.WebFarm)
            {
                tempName = context.Request["key"];

                var cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(tempName, data);
            }
            else
            {
                tempName = StoreDataOnDisk(context, data, false);
            }

            return tempName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string StoreDataOnDisk(HttpContext context, byte[] data, bool append)
        {
            var sessionId = context.Session.SessionID;
            var sessionPath = string.Format("{0}/{1}", IngestPath, sessionId);

            //Clean up temp folders
            CleanupTempFolders();

            //Make sure folder exists
            if (!Directory.Exists(sessionPath))
            {
                Directory.CreateDirectory(sessionPath);
            }

            var tempFileName = string.Format("{0}_Import.tmp", DateTime.Now.Ticks);
            var fullTempPath = string.Format("{0}/{1}", sessionPath, tempFileName);

            //Write data
            using (FileStream fs = File.Open(fullTempPath, append ? FileMode.Append : FileMode.Create, FileAccess.Write))
            {
                try
                {
                    fs.Write(data, 0, data.Length);
                }
                finally
                {
                    fs.Close();
                }
            }

            return tempFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void CleanupTempFolders()
        {
            if(!Directory.Exists(IngestPath))
            {
                return;
            }

            try
            {
                foreach (var directoryName in Directory.GetDirectories(IngestPath))
                {
                    if (Directory.GetCreationTime(directoryName) < DateTime.Now.AddDays(-2))
                    {
                        Directory.Delete(directoryName, true);
                    }
                }
            }
            catch
            {
                //Suppress errors as these should not be fatal.
            }
        }
    }
}
