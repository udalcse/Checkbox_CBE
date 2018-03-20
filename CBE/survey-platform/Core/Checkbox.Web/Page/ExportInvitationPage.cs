using System;
using System.Collections.Generic;
using System.IO;
using Checkbox.Analytics.Export;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Prezza.Framework.Caching;
using Prezza.Framework.Security;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for pages that export survey results.
    /// </summary>
    public abstract class ExportInvitationPage : ResponseTemplatePage
    {
        /// <summary>
        /// Get/set min date for completed responses
        /// </summary>
        protected DateTime? MinCompletedDate { get; set; }

        /// <summary>
        /// Get/set max date for completed responses
        /// </summary>
        protected DateTime? MaxCompletedDate { get; set; }

        /// <summary>
        /// Get/set language code for export
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle { get { return WebTextManager.GetText("/pageText/reporting.aspx/exportInvitations"); } }

        /// <summary>
        /// Get path to temp folder for exports
        /// </summary>
        protected virtual string TempFolderPath
        {
            get { return Server.MapPath(ApplicationManager.ApplicationRoot + "/Temp/"); }
        }

        /// <summary>
        /// Get/set current export file path
        /// </summary>
        protected virtual string ExportFilePath
        {
            get { return GetSessionValue("ExportFilePath", false, string.Empty); }
            set { Session["ExportFilePath"] = value; }
        }

        /// <summary>
        /// Validate temp file path for writing
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateTempFilePath()
        {
            return UploadItemManager.ValidateDownloadDirectory(TempFolderPath);
        }

        /// <summary>
        /// Instead of generating a key, read it from the session
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return GetSessionValue("ExportResultsProgressKey", false, string.Empty);
        }

        /// <summary>
        /// Get the controllable entity for authorization
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity() { return ResponseTemplate; }

        /// <summary>
        /// Get the required permission for exporting results
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Form.Administer"; } }

        /// <summary>
        /// Require form edit permission to view this page
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Administer"; } }

        /// <summary>
        /// Get timeout redirect URL
        /// </summary>
      //  protected override string TimeoutRedirectUrl { get { return ApplicationManager.ApplicationRoot + "/Analytics/Reporting.aspx"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //
            if (Master is BaseMasterPage)
            {
                ((BaseMasterPage)Master).SetTitle(
                    WebTextManager.GetText("/pageText/forms/surveys/invitations/export.aspx/title")
                    + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64));
            }

            var languages = new List<string>(ResponseTemplate.LanguageSettings.SupportedLanguages);

            LanguageCode = !languages.Contains(WebTextManager.GetUserLanguage())
                ? ResponseTemplate.LanguageSettings.DefaultLanguage
                : WebTextManager.GetUserLanguage();

            //Clear any temp files older than 30 minutes
            if (!Page.IsPostBack)
            {
                CleanUpTempFiles();
            }
        }

        /// <summary>
        /// Clean up temp file path
        /// </summary>
        protected virtual void CleanUpTempFiles()
        {
            //Delete temp export files
            FileUtilities.DeleteFilesOlderThanTimeSpan(
                TempFolderPath,
                new TimeSpan(0, 30, 0),
                false,
                "csv",
                "zip");

            //Delete temp folders for file downloads.  Allow older files for cases
            // of large export files.
            FileUtilities.DeleteFilesInFolderOlderThanTimeSpan(
                TempFolderPath,
                new TimeSpan(0, 60, 0),
                "UploadedSurveyFiles",
                false);
        }

        /// <summary>
        /// Get/set export options
        /// </summary>
        protected ExportOptions ExportOptions
        {
            get { return GetSessionValue<ExportOptions>("ExportOptions", false, null); }
            set { Session["ExportOptions"] = value; }
        }


        /// <summary>
        /// The number of columns common to all CSV subset exported
        /// </summary>
        protected static int FixedColumnCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Get the name of the file to put in the attachment response header
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAttachmentFileName()
        {
            return FileUtilities.FixFileName(ResponseTemplate.Name, ".", string.Empty) + "_csvExport.csv";
        }

        /// <summary>
        /// Get the number of export files required when splitting results across
        /// files.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetSplitExportFileCount(int exportFieldCount)
        {
            if (exportFieldCount > SurveyDataExporter.MAX_COLUMN_COUNT_PER_FILE)
            {
                //The number of files required to export all data if there are no fixed columns in sub sets
                //The number of files required to export all data if there is one or more fixed columns in each sub set

                double baseFileCount = Math.Ceiling((double)exportFieldCount / SurveyDataExporter.MAX_COLUMN_COUNT_PER_FILE);
                int adjustedColumnCount = exportFieldCount + (FixedColumnCount * (int)(baseFileCount - 1));
                double adjustedFileCount = Math.Ceiling((double)adjustedColumnCount / SurveyDataExporter.MAX_COLUMN_COUNT_PER_FILE);

                //ensure that the last file exported contains data and not just the fixed columns
                if (adjustedColumnCount % SurveyDataExporter.MAX_COLUMN_COUNT_PER_FILE == FixedColumnCount * (adjustedFileCount - 1))
                    adjustedFileCount--;

                return (int)adjustedFileCount;
            }

            //Only 1 file necessary
            return 1;
        }

        /// <summary>
        /// Return name of archive path for split export .zip file
        /// </summary>
        /// <returns></returns>
        protected virtual string GetSplitZipArchivePath(string sessionId)
        {
            return string.Format(
                        "{0}_{1}_{2}_ConsolidatedExport.zip",
                        TempFolderPath,
                        ApplicationManager.ApplicationDataContext,
                        sessionId);
        }

        /// <summary>
        /// Split export files and zip.  Returns path to archive.
        /// </summary>
        protected virtual void SplitExportAndZip(int responseTemplateId, string languageCode, ExportOptions options, string progressKey, string archivePath)
        {
            //Recalculate number of files in case seleted options have changed
            //Create an exporter
            SurveyDataExporter exporter = "CSV".Equals(ExportOptions.ExportMode, StringComparison.InvariantCultureIgnoreCase)
                ? new CsvDataExporter()
                : new SpssCompatibleCsvDataExporter();

            exporter.Initialize(responseTemplateId, languageCode, options, progressKey);

            int fileCount = GetSplitExportFileCount(exporter.ListAllFieldNames().Count);

            var outputFiles = new List<string>();

            //Export files
            for (int i = 1; i <= fileCount; i++)
            {
                ExportOptions.FileSet = i;

                //Generate file path
                string exportFileName = string.Format(
                        "{0}_{1}_{2}_{3}_{4}",
                        TempFolderPath,
                        ApplicationManager.ApplicationDataContext,
                        DateTime.Now.Ticks,
                        i,
                        GetAttachmentFileName());

                //Add file to list
                outputFiles.Add(exportFileName);

                //Write file
                ExportManager.WriteExportToFile(exporter, exportFileName, ProgressKey);
            }

            //Now generate zip
            FileUtilities.CompressFiles(outputFiles.ToArray(), archivePath);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void PrepareResponse()
        {
            Response.Expires = -1;
            Response.BufferOutput = ApplicationManager.AppSettings.BufferResponseExport;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + GetAttachmentFileName());
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        protected void DownloadZipCommon(string fileName)
        {
            Response.Expires = -1;
            Response.BufferOutput = ApplicationManager.AppSettings.BufferResponseExport;
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.ContentType = "application/octet-stream";

        }

        /// <summary>
        /// Read the content of a zip file located in memory and write it to the response.
        /// </summary>
        protected void DownloadZipFromMemory(byte[] zip, string fileName)
        {
            //Set file size
            Response.AddHeader("Content-Length", zip.Length.ToString());

            DownloadZipCommon(fileName);

            Response.BinaryWrite(zip);
            Response.Flush();
            Response.End();

        }

        /// <summary>
        /// Read the content of a zip file located on disk and write it to the response.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        protected void DownloadZipFromDisk(string filePath, string fileName)
        {
            DownloadZipCommon(fileName);

            if (ApplicationManager.AppSettings.WebFarm)
            {
                var cacheManager = CacheFactory.GetCacheManager();
                var exportBytes = cacheManager[ProgressKey] as byte[] ?? new byte[] { };

                Response.AddHeader("Content-Length", exportBytes.Length.ToString());

                Response.BinaryWrite(exportBytes);
            }
            else
            {

                //Set file size
                var info = new FileInfo(filePath);

                Response.AddHeader("Content-Length", info.Length.ToString());


                Response.TransmitFile(filePath);
            }

            Response.Flush();
            Response.End();
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
