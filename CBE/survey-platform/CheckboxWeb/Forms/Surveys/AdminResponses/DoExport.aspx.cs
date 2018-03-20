using System;
using System.Collections.Generic;
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
using Checkbox.Web;
using Checkbox.Web.Analytics;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;
using ProgressProvider = Checkbox.Progress.DatabaseProvider.ProgressProvider;

namespace CheckboxWeb.Forms.Surveys.AdminResponses
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

            try
            {
                if (BulkPDF == null 
                    || Utilities.IsNullOrEmpty(ExportFilePath)
                    || Utilities.IsNullOrEmpty(ProgressKey)
                    || ExportSettings == null)
                {
                    throw new Exception("Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                if ((BulkPDF == null || BulkPDF == Guid.Empty) && !AuthorizationProvider.Authorize(UserManager.GetCurrentPrincipal(), ReportTemplate, "Analysis.Run"))
                {
                    throw new Exception("The provided user context does not have the necessary authorization for the requested operation.");                    
                }

                _useS3 = UploadItemManager.UseS3ForTempFiles;
                _appContextName = ApplicationManager.ApplicationDataContext;

                Dictionary<Guid, string> bulkItems = null;
                var items = HttpContext.Current.Session[BulkPDF.ToString()];
                var context = HttpContext.Current;
                var lang = WebTextManager.GetUserLanguage();

                if (items != null)
                {
                     bulkItems = items as Dictionary<Guid, string>;
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

        private void ExportWorker(object response, HttpContext context, string lang, Dictionary<Guid, string> itemsToExport = null)
        {
            HttpContext.Current = context;

            try
            {
                string downloadUrl = string.Empty;

                if (itemsToExport != null)
                {
                    var outputFiles = GeneratePDFFile(itemsToExport, lang);

                    FileUtilities.CompressFiles(outputFiles.ToArray(), ExportFilePath);
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

        private List<string> GeneratePDFFile(Dictionary<Guid, string> itemsToExport, string lang)
        {
            var index = 0;
            var outputFiles = new List<string>(); 

            foreach (var item in itemsToExport)
            {
                var responseTemplate = ResponseTemplateManager.GetResponseTemplateFromResponseGUID(item.Key);

                ExportSettings.Name = item.Value;
                ExportSettings.SurveyTemplateGUID = responseTemplate.GUID.ToString();
                ExportSettings.PrintClientPdf = true;
                ExportSettings.CustomResponseGUID = item.Key;

                var result = PdfExportManager.ExportSurvey(0, ExportSettings);

                string exportFileName = string.Format(
                      "{0}{1}_{2}{3}",
                      TempFolderPath,
                      item.Value,
                      index,
                      ".pdf");

                WriteTemporaryFile(exportFileName, result.Data);

               // PdfExportManager.AddBorders(exportFileName);


                outputFiles.Add(exportFileName);
                index++;

                ProgressProvider.SetProgress(ProgressKey,
                  new ProgressData {
                      CurrentItem = index,
                      Status = ProgressStatus.Pending,
                      Message = TextManager.GetText("/controlText/analysisData/pdfExport/pending"),
                      TotalItemCount = itemsToExport.Count
                  },
                  _appContextName);
            }

            return outputFiles;
        }
    }
}