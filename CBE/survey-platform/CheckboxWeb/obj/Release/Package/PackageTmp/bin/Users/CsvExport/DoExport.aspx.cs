using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Checkbox.Analytics.Export;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Progress;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Analytics;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;
using ProgressProvider = Checkbox.Progress.DatabaseProvider.ProgressProvider;

namespace CheckboxWeb.Users.CsvExport
{
    public partial class DoExport : ReportExportPageBase
    {
        private bool _useS3;
        private string _appContextName;

        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            ExportFilePath = string.Format(
               @"{0}\{1}_{2}_{3}",
               TempFolderPath,
               ApplicationManager.ApplicationDataContext.Replace(":", string.Empty),
               DateTime.Now.Ticks,
               "UserProfilesList.csv");

            try
            {
                if (UserList == null 
                    || Utilities.IsNullOrEmpty(ExportFilePath)
                    || Utilities.IsNullOrEmpty(ProgressKey))
                {
                    throw new Exception("Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                if ((UserList == null || UserList == Guid.Empty) && !AuthorizationProvider.Authorize(UserManager.GetCurrentPrincipal(), ReportTemplate, "Analysis.Run"))
                {
                    throw new Exception("The provided user context does not have the necessary authorization for the requested operation.");                    
                }

                _useS3 = UploadItemManager.UseS3ForTempFiles;
                _appContextName = ApplicationManager.ApplicationDataContext;

                List<string> bulkItems = null;
                var items = HttpContext.Current.Session[UserList.ToString()];
                var context = HttpContext.Current;
                var lang = WebTextManager.GetUserLanguage();

                if (items != null)
                {
                     bulkItems = items as List<string>;
                }

                if (!UploadItemManager.UseS3ForTempFiles)
                {
                    ThreadPool.QueueUserWorkItem(o => ExportWorker(null, context, lang, bulkItems));
                }
                else
                {
                    ExportWorker(Response, context, WebTextManager.GetUserLanguage(), bulkItems);
                }
            }
            catch (Exception ex)
            {
                SetErrorProgress(ex);

                WriteResult(new { success = false, error = ex.Message });
            }
        }

        private void ExportWorker(object response, HttpContext context, string lang, List<string> itemsToExport = null)
        {
            HttpContext.Current = context;

            try
            {
                string downloadUrl = string.Empty;

                if (itemsToExport != null)
                {
                    GenerateCSVile(itemsToExport, lang);
                }

                var progress = ProgressProvider.GetProgress(ProgressKey, _appContextName);
                if (progress.Status == ProgressStatus.Error)
                {
                    throw new Exception(progress.ErrorMessage);
                }

                //Depending on app settings, write file to S3, leave it on disk
                // or put in http context cache.
                if (_useS3)
                {
                    //Set progress to 99% so caller can finally
                    // mark progress complete.
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        new ProgressData
                        {
                            CurrentItem = 99,
                            Status = ProgressStatus.Pending,
                            Message = TextManager.GetText("/controlText/exportManager/uploadingToS3"),
                            TotalItemCount = 100
                        },
                        _appContextName
                    );

                    WriteFileToS3(ExportFilePath, string.Empty);

                    //Replace export path
                    string fileName = ExportFilePath.Replace(TempFolderPath, string.Empty);

                    //Remove first char path separator -- TODO: Figure out why this is not necessary
                    // when using multi-part .zip
                    if (fileName.Length > 0)
                    {
                        fileName = fileName.Remove(0, 1);
                    }

                    downloadUrl = UploadItemManager.GetS3TempFileDownloadLink(UploadItemManager.SanitizeFileName(fileName, "."));
                }
                else if (ApplicationManager.AppSettings.WebFarm)
                {
                    WriteFileToCache(ExportFilePath);
                }

                if (_useS3 && response != null)
                    WriteResult(new { success = true, downloadUrl }, response as HttpResponse);

                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/doExport.aspx/complete"),
                        TotalItemCount = 100,
                    },
                    _appContextName
                );
            }
            catch (Exception ex)
            {
                SetErrorProgress(ex);

                if (_useS3 && response != null)
                    WriteResult(new { success = false, error = ex.Message }, response as HttpResponse);
            }
        }

        /// <summary>
        /// Write file data to progress cache
        /// </summary>
        private void WriteFileToCache(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);

                var cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(ProgressKey, fileBytes);
            }
        }

        /// <summary>
        /// Write file to S3, modify ACL, and return link to file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="namePrefix"></param>
        /// <returns></returns>
        protected string WriteFileToS3(string filePath, string namePrefix)
        {
            //Ensure file exists
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            //Get file info to get file name
            var fileInfo = new FileInfo(filePath);

            string fileName = Utilities.IsNullOrEmpty(namePrefix)
                ? fileInfo.Name
                : string.Format("{0}.{1}", namePrefix, fileInfo.Name);

            //Store on S3 and return direct access url
            return UploadItemManager.SaveTempFileToS3(
                UploadItemManager.SanitizeFileName(fileName, "."),
                filePath);
        }

        private void SetErrorProgress(Exception ex)
        {
            ExceptionPolicy.HandleException(ex, "UIProcess");

            //Set progress to errr
            ProgressProvider.SetProgress(
                ProgressKey,
                new ProgressData
                {
                    CurrentItem = 100,
                    Status = ProgressStatus.Error,
                    Message = "An error occurred while exporting results data.",
                    ErrorMessage = ex.Message,
                    TotalItemCount = 100
                },
                _appContextName
            );
        }

        private void GenerateCSVile(List<string> itemsToExport, string lang)
        {
            var builder = new StringBuilder();
            List<string> userFieldNames = new List<string>();

            userFieldNames.Add(TextManager.GetText("/pageText/forms/surveys/responses/export.aspx/email"));
            userFieldNames.AddRange(ProfileManager.ListPropertyNames());

            //if value contains , we need to add double quotes to correct parse it 
            AddDoubleQuotes(userFieldNames);

            builder.Append(string.Join(",", userFieldNames));
            builder.Append(Environment.NewLine);

            var columnData = new List<string>();
            foreach (var user in itemsToExport)
            {
                columnData = CsvDataExporter.GetUserData(userFieldNames, user);

                //if value contains , we need to add double quotes to correct parse it 
                AddDoubleQuotes(columnData);

                builder.Append(string.Join(",", columnData));
                builder.Append(Environment.NewLine);
            }

            File.WriteAllText(ExportFilePath, builder.ToString());
        }

        private void AddDoubleQuotes(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains(","))
                    list[i] = list[i].AddDoubleQuotes();
                }}
        }
}