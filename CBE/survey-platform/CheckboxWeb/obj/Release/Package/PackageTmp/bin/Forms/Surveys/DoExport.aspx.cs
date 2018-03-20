using System;
using System.IO;
using System.Threading;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Progress;
using Checkbox.Users;
using Checkbox.Web.Forms;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;
using ProgressProvider = Checkbox.Progress.DatabaseProvider.ProgressProvider;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class DoExport : ResponseTemplateExportPageBase
    {
        private bool _useS3;
        private string _appContextName;

        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            try
            {
                //Check for data
                if (ResponseTemplateId == 0
                    || Utilities.IsNullOrEmpty(ExportFilePath)
                    || Utilities.IsNullOrEmpty(ProgressKey)
                    || ExportSettings == null)
                {
                    throw new Exception("Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                //Check user rights
                //Uncomment to enable user role validations 
                //if (!AuthorizationProvider.Authorize(UserManager.GetCurrentPrincipal(), ResponseTemplate, "Form.Edit"))
                //{
                //    throw new Exception("The provided user context does not have the necessary authorization for the requested operation.");                    
                //}

                _useS3 = UploadItemManager.UseS3ForTempFiles;
                _appContextName = ApplicationManager.ApplicationDataContext;

                //make export in a single thread for better progress retrieving
                if (!UploadItemManager.UseS3ForTempFiles)
                    ThreadPool.QueueUserWorkItem(ExportWorker, null);
                else // if S3 file uploading feature is enabled, do in the same thread to keep the current Http Response
                    ExportWorker(Response);
            }
            catch (Exception ex)
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

                WriteResult(new { success = false, error = ex.Message });
            }
        }

        private void ExportWorker(object response)
        {
            try
            {
                string downloadUrl = string.Empty;

                var result = PdfExportManager.ExportSurvey(ResponseTemplateId, ExportSettings);
                WriteTemporaryFile(ExportFilePath, result.Data);

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

                //Set progress to 100%
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/doExport.aspx/complete"),
                        TotalItemCount = 100,
                        AdditionalData = $"/Temp/{ExportFileName}"
                    },
                    _appContextName
                );
            }
            catch (Exception ex)
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
    }
}