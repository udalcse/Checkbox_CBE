using System;
using System.IO;
using Checkbox.Analytics.Export;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    /// <summary>
    /// Ajax-Enabled export page
    /// </summary>
    public partial class DoExport : ExportResultsPage
    {
        [QueryParameter]
        public bool ZipParts { get; set; }

        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            try
            {
                //Check for data
                if (ResponseTemplate == null
                    || Utilities.IsNullOrEmpty(LanguageCode)
                    || Utilities.IsNullOrEmpty(ProgressKey)
                    || ExportOptions == null)
                {
                    throw new Exception("Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                string progressKey = GenerateProgressKey();
                string splitZipArchivePath = GetSplitZipArchivePath(Session.SessionID);

                string downloadUrl = string.Empty;

                //Figure out what to do
                if (ZipParts)
                {
                    SplitExportAndZip(
                        ResponseTemplate.ID.Value,
                        LanguageCode,
                        ExportOptions,
                        progressKey,
                        splitZipArchivePath);
                }
                else
                {
                    if ("SPSS_NATIVE".Equals(ExportOptions.ExportMode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ExportManager.WriteNativeSpssExportToFile(
                            ResponseTemplate.ID.Value,
                            ExportOptions,
                            LanguageCode,
                            progressKey,
                            ExportFilePath);
                    }
                    else
                    {
                        ExportManager.WriteCommonExportToFile(
                            ResponseTemplate.ID.Value,
                            ExportOptions,
                            LanguageCode,
                            progressKey,
                            ExportFilePath);
                    }
                }

                var progress = ProgressProvider.GetProgress(ProgressKey);
                if(progress.Status == ProgressStatus.Error)
                {
                    throw new Exception(progress.ErrorMessage);
                }

                string filePath = ZipParts
                        ? splitZipArchivePath
                        : ExportFilePath;

                //Depending on app settings, write file to S3, leave it on disk
                // or put in http context cache.
                if (UploadItemManager.UseS3ForTempFiles)
                {

                    //Set progress to 99% so caller can finally
                    // mark progress complete.
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        new ProgressData
                        {
                            CurrentItem = 99,
                            Status = ProgressStatus.Pending,
                            Message = TextManager.GetText("/controlText/exportManager/uploadingToS3", LanguageCode),
                            TotalItemCount = 100
                        }
                    );

                    WriteFileToS3(filePath, string.Empty);

                    //Replace export path
                    string fileName = filePath.Replace(TempFolderPath, string.Empty);

                    //Remove first char path separator -- TODO: Figure out why this is not necessary
                    // when using multi-part .zip
                    if (!ZipParts)
                    {
                        if (fileName.Length > 0)
                        {
                            fileName = fileName.Remove(0, 1);
                        }
                    }

                    downloadUrl = UploadItemManager.GetS3TempFileDownloadLink(UploadItemManager.SanitizeFileName(fileName, "."));
                }
                else if(ApplicationManager.AppSettings.WebFarm)
                {
                    WriteFileToCache(filePath);
                }

                WriteResult(new { success = true, downloadUrl });

                //Set progress to 100%
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/doExport.aspx/complete", LanguageCode),
                        TotalItemCount = 100,
                        //AdditionalData = downloadUrl
                    }
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
                    }
                );


                WriteResult(new {success = false, error = ex.Message});
            }
        }
       
        /// <summary>
        /// Write file data to progress cache
        /// </summary>
        private void WriteFileToCache(string filePath)
        {
            using(var fileStream = File.OpenRead(filePath))
            {
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, fileBytes.Length);

                var cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(ProgressKey, fileBytes);
            }
        }
    }
}