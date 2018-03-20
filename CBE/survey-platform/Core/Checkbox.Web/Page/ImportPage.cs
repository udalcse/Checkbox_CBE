using System;
using System.IO;
using Checkbox.Forms;
using Checkbox.Management;
using Prezza.Framework.Caching;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base page to handle some of the mechanics of handling import operations with or without AJAX.
    /// </summary>
    public class ImportPage : SecuredPage
    {
        /// <summary>
        /// Get/set temp path
        /// </summary>
        public string TempPath
        {
            get
            {
                if (string.IsNullOrEmpty(Request["tf"]))
                {
                    return string.Empty;
                }

                return Server.UrlDecode(Request["tf"]);
            }
        }

        /// <summary>
        /// Store import data to a temp location and redirect for AJAX-enabled
        /// importing
        /// </summary>
        /// <returns></returns>
        protected virtual void StoreDataAndRedirect(byte[] data, string importWorkerUrl)
        {
            string tempPath = StoreData(data);

            //Add query string to url
            if (importWorkerUrl.Contains("?"))
            {
                Response.Redirect(importWorkerUrl + "&tf=" + Server.UrlEncode(tempPath));
            }
            else
            {
                Response.Redirect(importWorkerUrl + "?tf=" + Server.UrlEncode(tempPath));
            }
        }

        /// <summary>
        /// Store data and return name of temp location.  Significance of name will depend on whether data is stored in
        /// file system or S3 bucket.  The latter is a requirement for proper web farm operation.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string StoreData(byte[] data)
        {
            string tempName;

            //Depending on config settings, write data to s3, http context cache, or 
            // temp folder.
            if (UploadItemManager.UseS3ForTempFiles)
            {
                tempName = UploadItemManager.SanitizeFileName(string.Format("{0}_{1}_Import.tmp", DateTime.Now.Ticks, Session.SessionID), ".");
                string s3Name = UploadItemManager.GetS3FileName(1, tempName);

                UploadItemManager.SaveTempFileToS3(s3Name, data);
            }
            else if (ApplicationManager.AppSettings.WebFarm)
            {
                tempName = ProgressKey;

                var cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(ProgressKey, data);
            }
            else
            {
                tempName = string.Format(
                    "{0}/{1}_{2}_Import.tmp",
                    Server.MapPath("~/Temp"),
                    DateTime.Now.Ticks,
                    Session.SessionID);

                using (FileStream fs = File.OpenWrite(tempName))
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
            }

            return tempName;
        }
    }
}
