using System;
using System.IO;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DoDownloadFiles : ExportResultsPage
    {
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
                    throw new Exception(
                        "Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                string progressKey = GenerateProgressKey();

                string downloadUrl = ExportFilePath;

                //Store files in temp folder
                SaveFiles(ResponseTemplate.ID.Value, ExportFilePath, progressKey, LanguageCode);

                //Zip files
                ProgressProvider.SetProgress(
                    progressKey,
                    TextManager.GetText("/controlText/uploadItemManager/zippingFiles", LanguageCode),
                    string.Empty,
                    ProgressStatus.Running,
                    80,
                    100);

                string exportPath = FileUtilities.CompressFolder(
                    ExportFilePath,
                    GetAttachmentFileName());

                //Zip files
                ProgressProvider.SetProgress(
                    progressKey,
                    TextManager.GetText("/controlText/uploadItemManager/zippingFiles", LanguageCode),
                    string.Empty,
                    ProgressStatus.Completed,
                    100,
                    100);

                //Store data in S3, leave on disk, or in cache for 
                // retrival.
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

                    //Store file in S3
                    WriteFileToS3(exportPath,
                                  string.Format("{0}.{1}", ApplicationManager.ApplicationDataContext, Session.SessionID));

                    string fileName = string.Format(
                        "{0}.{1}.{2}",
                        ApplicationManager.ApplicationDataContext,
                        Session.SessionID,
                        GetAttachmentFileName());

                    downloadUrl = UploadItemManager.GetS3TempFileDownloadLink(fileName);
                }
                else if (ApplicationManager.AppSettings.WebFarm)
                {
                    WriteFileToCache(exportPath);
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
                        TotalItemCount = 100
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
                        Message = "An error occurred while generating download.",
                        ErrorMessage = ex.Message,
                        TotalItemCount = 100
                    }
                );


                WriteResult(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="exportFile"></param>
        /// <param name="progressKey"></param>
        /// <param name="languageCode"></param>
        protected void SaveFiles(int responseTemplateId, string exportFile, string progressKey, string languageCode)
        {
            UploadItemManager.SaveFilesToDisk(responseTemplateId, exportFile, progressKey, languageCode);
        }

        /// <summary>
        /// Get name of file for download
        /// </summary>
        /// <returns></returns>
        protected override string GetAttachmentFileName()
        {
            return FileUtilities.FixFileName(ResponseTemplate.Name, ".", string.Empty) + "_UploadedFiles.zip";
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
    }
}