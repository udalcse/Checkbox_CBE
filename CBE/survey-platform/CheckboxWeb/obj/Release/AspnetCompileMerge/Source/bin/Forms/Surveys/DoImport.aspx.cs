using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class DoImport : SecuredPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        /// <summary>
        /// Destination, if any, for imported survey.
        /// </summary>
        [QueryParameter("f")]
        public int? FolderId { get; set; }

        /// <summary>
        /// Path to uploaded file.
        /// </summary>
        [QueryParameter("p")]
        public string UploadedFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("n")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("s")]
        public int? StyleTemplateId { get; set; }

        [QueryParameter("mid")]
        public int? MobileTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DecodedName
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return Name;
                }

                return Server.UrlDecode(Name);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_ImportSurvey";
        }

        /// <summary>
        /// Get the current folder
        /// </summary>
        /// <returns></returns>
        private FormFolder CurrentFolder
        {
            get
            {
                if (!FolderId.HasValue || FolderId <= 0)
                {
                    return FolderManager.GetRoot();
                }

                var folder = new FormFolder();

                try
                {
                    folder.Load(FolderId.Value);
                    return folder;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Set progress message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currentItem"></param>
        protected void SetProgress(string message, int currentItem)
        {
            //Otherwise, we are go for import
            ProgressProvider.SetProgress(
                ProgressKey,
                message,
                string.Empty,
                ProgressStatus.Running,
                currentItem,
                100);
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            try
            {
                var cacheManager = Prezza.Framework.Caching.CacheFactory.GetCacheManager();

                //Ensure name set
                if (Utilities.IsNullOrEmpty(DecodedName))
                {
                    throw new Exception("Name for new survey not specified.");
                }

                //Check progress key
                if (Utilities.IsNullOrEmpty(ProgressKey))
                {
                    throw new Exception("No progress key specified for import survey process.");
                }

                //Check temp file
                if (Utilities.IsNullOrEmpty(UploadedFilePath))
                {
                    throw new Exception("No temp file specified for import survey process.");
                }

                //Otherwise, we are go for import
                SetProgress(WebTextManager.GetText("/pageText/doSurveyImport.aspx/loadingTempData"), 0);

                var templateXml = GetTemplateXml(UploadedFilePath);

                if (templateXml == null)
                {
                    const string errorMsg = "No temp file specified for import survey process.";
                    ExceptionPolicy.HandleException(new Exception(errorMsg), "BusinessPublic");
                }

                var version = templateXml.DocumentElement.GetAttribute("Version");
                if (Utilities.IsNullOrEmpty(version) || version[0] == '4')
                {
                    throw new Exception("The provided xml is from an old version of the application (" + version +
                        "), and cannot be imported to the " + Checkbox.Configuration.Install.ApplicationInstaller.ApplicationAssemblyVersion);
                }

                cacheManager.Add("UploadProgressKey", ProgressKey);

                var rt = ResponseTemplateManager.ImportResponseTemplate(templateXml, DecodedName, UserManager.GetCurrentPrincipal(), CurrentFolder, StyleTemplateId, MobileTemplateId, ProgressKey, WebTextManager.GetUserLanguage());

                string finishedText = WebTextManager.GetText("/pageText/doSurveyImport.aspx/finished");

                if (finishedText.Contains("{0}"))
                {
                    finishedText = string.Format(finishedText, Server.HtmlEncode(rt.Name));
                }

                var exportMessage = cacheManager.GetData(ProgressKey);

                //Set progress
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                            AdditionalData = rt.ID.ToString(),
                            Message = GetFinishText(exportMessage, finishedText),
                            Status = exportMessage != null ? ProgressStatus.Error : ProgressStatus.Completed,
                            CurrentItem = 100,
                            TotalItemCount = 100
                        }
                    );


                WriteResult(new { success = true });
            }
            catch (Exception ex)
            {
                //Set progress status
                ProgressProvider.SetProgress(ProgressKey, "An error occurred while importing the survey.", ex.Message, ProgressStatus.Error, 100, 100);
                WriteResult(new { success = false, error = ex.Message });
                throw;
            }
        }

        private string GetFinishText(object exportMessage, string finishedText)
        {
            if (exportMessage != null)
            {
                return "Couldn't insert following profile fields: " + string.Join(",", (List<string>)exportMessage);
            }

            return finishedText;
        }


        /// <summary>
        /// Get data set used to load template
        /// </summary>
        /// <returns></returns>
        private XmlDocument GetTemplateXml(string filePath)
        {
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, filePath);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return null;
            }

            //Attempt to read & parse file as xml
            try
            {
                var ms = new MemoryStream(fileBytes);
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(ms);


                return xmlDocument;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}