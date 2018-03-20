using System;
using System.IO;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Invitations.Export;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    /// <summary>
    /// Ajax-Enabled export page
    /// </summary>
    public partial class DoExport : ExportInvitationPage
    {
        [QueryParameter("i")]
        public int? InvitationId { get; set; }


        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            try
            {
                //Check for data
                if (Utilities.IsNullOrEmpty(LanguageCode)
                    || Utilities.IsNullOrEmpty(ProgressKey))
                {
                    throw new Exception("Unable to do export:  A required parameter, such as export options, file path, etc. was not set.");
                }

                string progressKey = GenerateProgressKey();

                string downloadUrl = string.Empty;

                bool writeToFile = UploadItemManager.ValidateDownloadDirectory(TempFolderPath);

                if (writeToFile)
                {
                    //Store export file path
                    string filePath = ExportFilePath = string.Format(
                            @"{0}\{1}_{2}_{3}_{4}",
                            TempFolderPath,
                            ApplicationManager.ApplicationDataContext,
                            DateTime.Now.Ticks,
                            "UploadedSurveyFiles",
                            ResponseTemplate.ID);

                    //Figure out what to do
                    ExportManager.WriteCommonExportToFile(
                        InvitationId.Value,
                        LanguageCode,
                        progressKey,
                        ExportFilePath);

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
                        if (fileName.Length > 0)
                        {
                            fileName = fileName.Remove(0, 1);
                        }

                        downloadUrl = UploadItemManager.GetS3TempFileDownloadLink(UploadItemManager.SanitizeFileName(fileName, "."));
                    }
                    else if (ApplicationManager.AppSettings.WebFarm)
                    {
                        WriteFileToCache(filePath);
                    }
                }
                else
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (TextWriter textWriter = new StreamWriter(memoryStream))
                        {

                            ExportManager.WriteExportToTextWriter(
                                textWriter,
                                InvitationId.Value,
                                LanguageCode,
                                ProgressKey);

                            memoryStream.Position = 0;

                            using(StreamReader streamReader = new StreamReader(memoryStream))
                            {
                                WriteStringToCache(streamReader);

                                Session["InvitationExportStringType"] = true;
                            }
                        }
                    }
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

        /// <summary>
        /// Write file data to progress cache
        /// </summary>
        private void WriteStringToCache(StreamReader reader)
        {
            var cacheManager = CacheFactory.GetCacheManager();
            cacheManager.Add(ProgressKey, reader.ReadToEnd());
        }
    }
}