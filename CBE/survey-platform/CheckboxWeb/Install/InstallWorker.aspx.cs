using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using Checkbox.Common;
using Checkbox.Configuration.Install;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;
using Page = System.Web.UI.Page;

namespace CheckboxWeb.Install
{
    /// <summary>
    /// Worker page for running through steps of Checkbox web installer.  Has no UI, but uses progress provider
    /// to track operation progress.
    /// </summary>
    public partial class InstallWorker : CheckboxServerProtectedPage
    {
        /// <summary>
        /// Get mode for worker
        /// </summary>
        private string Mode
        {
            get { return Request.QueryString["m"]; }
        }

        /// <summary>
        /// Get progress key for tracking progress
        /// </summary>
        private string ProgressKey
        {
            get { return Session["InstallProgressKey"] as string; }
        }

        /// <summary>
        /// Install survey samples
        /// </summary>
        private bool InstallSurveySamples
        {
            get { return Request.QueryString["i"].Contains("v"); }
        }

        /// <summary>
        /// Install library samples
        /// </summary>
        private bool InstallLibrarySamples
        {
            get { return Request.QueryString["i"].Contains("l"); }
        }

        /// <summary>
        /// Install style samples
        /// </summary>
        private bool InstallStyleSamples
        {
            get { return Request.QueryString["i"].Contains("s"); }
        }

        /// <summary>
        /// Perform work of running install.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Set pending message
            ProgressProvider.SetProgress(
                ProgressKey,
                "Preparing to install Checkbox&reg; 6",
                string.Empty,
                ProgressStatus.Pending,
                0,
                100);

            try
            {
                var appInstaller = Mode.Equals("d", StringComparison.InvariantCultureIgnoreCase)
                    ? (ApplicationInstaller)Session["_appInstaller"]
                    :  new ApplicationInstaller(Server.MapPath("~"), false, "sqlserver");

                if (appInstaller == null)
                {
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        string.Empty,
                        "Unable to get appInstaller from Session.",
                        ProgressStatus.Error,
                        0,
                        100);

                    WriteResult(new { success = false, errorMessage = "Unable to get appInstaller from Session." });

                    return;
                }

                if (Utilities.IsNullOrEmpty(Mode))
                {
                    ProgressProvider.SetProgress(
                       ProgressKey,
                       string.Empty,
                        "Installation mode not specified.",
                       ProgressStatus.Error,
                       0,
                       100);

                    WriteResult(new { success = false, errorMessage = "Installation mode not specified." });

                    return;
                }

                if (Mode.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                {
                    lock (appInstaller)
                    {
                        if(!DoInstall(appInstaller))
                            return;
                    }
                }

                if (Mode.Equals("s", StringComparison.CurrentCultureIgnoreCase))
                {
                    //Samples
                    if (InstallSurveySamples
                        || InstallLibrarySamples
                        || InstallStyleSamples)
                    {
                        if (!InstallSamples(appInstaller))
                        {
                            WriteResult(new {success = false, errorMessage = "Install samples failed."});
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    string.Empty,
                    "An error occurred during installation: " + ex.Message,
                    ProgressStatus.Error,
                    0,
                    1);

                WriteResult(new { success = false, error = ex.Message });

                return;
            }
            
            ProgressProvider.SetProgress(
                ProgressKey,
                string.Empty,
                "Database installation and web.config updated successfully.",
                ProgressStatus.Completed,
                100,
                100);

            WriteResult(new { success = true });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appInstaller"></param>
        private bool DoInstall(ApplicationInstaller appInstaller)
        {
            if (appInstaller.InstallSuccess)
                return true;

            //Database
            if (!InstallDatabase(appInstaller))
            {
                WriteResult(new { success = false, errorMessage = "Database setup failed." });
                return false;
            }

            //Email Database
            if (!InstallEmailDatabase(appInstaller))
            {
                WriteResult(new { success = false, errorMessage = "Messaging Service Database setup failed." });
                return false;
            }

            //Webconfig
            if (!UpdateWebConfig(appInstaller))
            {
                WriteResult(new { success = false, errorMessage = "Update web.config failed." });
                return false;
            }

            //Set success flag since things are good at this point
            //Webconfig
            if (!UpdateInstallSuccessFlag(appInstaller))
            {
                WriteResult(new { success = false, errorMessage = "Update install success failed." });
                return false;
            }

            return true;
        }

        /// <summary>
        /// Install Checkbox database
        /// </summary>
        private bool InstallEmailDatabase(ApplicationInstaller appInstaller)
        {
            //Database
            ProgressProvider.SetProgress(
                ProgressKey,
                "Running Messaging Service database installation scripts.",
                string.Empty,
                ProgressStatus.Running,
                0,
                100);

            appInstaller.ProgressKey = ProgressKey;

            if (appInstaller.InstallEMailDatabase)
            {
                string dbStatus;

                //load scripts
                appInstaller.LoadInstallEmailFilesAndScripts("install");

                if (!appInstaller.DropEmailDatabaseData(out dbStatus) || !appInstaller.RunEMailDatabaseScripts(out dbStatus))
                {
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        string.Empty,
                        dbStatus,
                        ProgressStatus.Error,
                        0,
                        1);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Install Checkbox database
        /// </summary>
        private bool InstallDatabase(ApplicationInstaller appInstaller)
        {
            //Database
            ProgressProvider.SetProgress(
                ProgressKey,
                "Running database installation scripts.",
                string.Empty,
                ProgressStatus.Running,
                0,
                100);

            appInstaller.ProgressKey = ProgressKey;

            if (appInstaller.InstallDatabase)
            {
                string dbStatus;

                if (!appInstaller.SetupDatabase(out dbStatus))
                {
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        string.Empty,
                        dbStatus,
                        ProgressStatus.Error,
                        0,
                        1);

                    return false;
                }
            }
            else
            {
                string dbStatus;

                if (!appInstaller.SaveSettings(out dbStatus))
                {
                    ProgressProvider.SetProgress(
                        ProgressKey,
                        string.Empty,
                        dbStatus,
                        ProgressStatus.Error,
                        0,
                        1);

                    return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Update web.config
        /// </summary>
        private bool UpdateWebConfig(ApplicationInstaller appInstaller)
        {
            ProgressProvider.SetProgress(
                    ProgressKey,
                    "Updating web.config.",
                    string.Empty,
                    ProgressStatus.Running,
                    90,
                    100);

            //URL Settings
            string configStatus;
            if (!appInstaller.UpdateWebConfig(out configStatus))
            {
                ProgressProvider.SetProgress(
                       ProgressKey,
                       string.Empty,
                       configStatus,
                       ProgressStatus.Error,
                       0,
                       1);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Update web.config
        /// </summary>
        private bool UpdateInstallSuccessFlag(ApplicationInstaller appInstaller)
        {
            ProgressProvider.SetProgress(
                    ProgressKey,
                    "Setting success flag.",
                    string.Empty,
                    ProgressStatus.Running,
                    90,
                    100);

            //URL Settings
            string configStatus;
            if (!appInstaller.MarkInstallSuccess(out configStatus))
            {
                ProgressProvider.SetProgress(
                       ProgressKey,
                       string.Empty,
                       configStatus,
                       ProgressStatus.Error,
                       0,
                       1);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Install samples
        /// </summary>
        /// <param name="appInstaller"></param>
        private bool InstallSamples(ApplicationInstaller appInstaller)
        {
            ProgressProvider.SetProgress(
                   ProgressKey,
                   "Installing samples.",
                   string.Empty,
                   ProgressStatus.Running,
                   95,
                   100);

            string sampleStatus;

            if (InstallStyleSamples &&
                !InstallStyles(out sampleStatus))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    string.Empty,
                    "An error while installing sample styles: " + sampleStatus,
                    ProgressStatus.Error,
                    0,
                    1);

                return false;
            }


            if (InstallSurveySamples
                && !InstallSurveys(out sampleStatus))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    string.Empty,
                    "An error while installing sample surveys: " + sampleStatus,
                    ProgressStatus.Error,
                    0,
                    1);

                return false;
            }
            
            if (InstallLibrarySamples
                && !InstallLibraries(out sampleStatus))
            {
                ProgressProvider.SetProgress(
                   ProgressKey,
                   string.Empty,
                   "An error while installing sample libaries: " + sampleStatus,
                   ProgressStatus.Error,
                   0,
                   1);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private bool InstallStyles(out string status)
        {
            var workingDirectory = Server.MapPath("~/install/defaulttemplates/styles");
            var styleList = Directory.GetFiles(workingDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var styleTemplate in styleList)
            {
                var xmlDoc = new XmlDocument();

                xmlDoc.Load(styleTemplate);

                try
                {
                    //Check the name
                    var nameNode = xmlDoc.SelectSingleNode("/CssDocument/TemplateName");

                    if (nameNode != null)
                    {
                        var name = nameNode.InnerText;

                        if (!StyleTemplateManager.IsStyleNameInUse(name))
                        {
                            var template = StyleTemplateManager.CreateStyleTemplate(xmlDoc, UserManager.AuthenticateUser("admin", "admin"));
                            template.IsEditable = true;
                            template.IsPublic = true;

                            StyleTemplateManager.SaveTemplate(template, UserManager.AuthenticateUser("admin", "admin"));

                            if (template.Name.Equals("Default", StringComparison.InvariantCultureIgnoreCase))
                            {
                                ApplicationManager.AppSettings.DefaultStyleTemplate = template.TemplateID.Value;
                            }
                        }
                    }
                }
                catch
                {
                    //Ignore individual style errors so that an attempt is made to load each style
                }
            }

            status = String.Empty;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private bool InstallSurveys(out string status)
        {
            var workingDirectory = Server.MapPath("~/install/defaulttemplates/surveys");

            LoadSurveyInFolder(workingDirectory, -1);

            var directories = new List<string>(Directory.GetDirectories(workingDirectory));

            foreach (var directory in directories)
            {
                var folderId = CreateFolder(directory);

                if (folderId > -1)
                {
                    LoadSurveyInFolder(directory, folderId);
                }
            }

            status = String.Empty;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private bool InstallLibraries(out string status)
        {
            
            string workingDirectory = Server.MapPath("~/install/defaulttemplates/libraries");

            var libraries = Directory.GetFiles(workingDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (string library in libraries)
            {
                LibraryTemplate template = null;

                try
                {
                    var data = new DataSet();

                    data.ReadXml(library, XmlReadMode.ReadSchema);
                    var libraryName = GetLibraryName(data);

                    if (Utilities.IsNotNullOrEmpty(libraryName))
                    {
                        //creating a library automatically saves it.
                        template = LibraryTemplateManager.CreateLibraryTemplate("Imported Library", UserManager.AuthenticateUser("admin", "admin"));

                        //TODO: Import Libraries
                        //template.Import(data);

                        if (!LibraryTemplateManager.LibraryTemplateExists(libraryName, null))
                        {
                            TextManager.SetText(template.NameTextID, WebTextManager.GetUserLanguage(), libraryName);
                            TextManager.SetText(template.DescriptionTextID, WebTextManager.GetUserLanguage(), GetLibraryDescription(data));

                            SetDefaultPolicy(template);
                        }
                    }
                }
                catch
                {
                }
            }

            status = String.Empty;
            return true;
        }

        /// <summary>
        /// Creates a folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static int CreateFolder(string folderPath)
        {
            string folderName = new DirectoryInfo(folderPath).Name ?? string.Empty;

            if (Utilities.IsNotNullOrEmpty(folderName))
            {
                var folder = new FormFolder { Name = folderName };

                if (!FolderManager.FolderExists(folder.ID, folder.Name, UserManager.AuthenticateUser("admin", "admin")))
                {
                    folder.Save(UserManager.AuthenticateUser("admin", "admin"));
                    if (folder.ID != null)
                    {
                        return folder.ID.Value;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Loads all survey from a specified directory on the file system into a corresponding directory in Checkbox.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="folderId"></param>
        private static void LoadSurveyInFolder(string folderPath, int folderId)
        {
            string[] surveyXmlList = Directory.GetFiles(folderPath, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var surveyXml in surveyXmlList)
            {
                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(surveyXml);

                    ResponseTemplate template = ResponseTemplateManager.CreateResponseTemplate(UserManager.AuthenticateUser("admin", "admin"));
                    template.Import(xmlDoc.DocumentElement);

                    if (!ResponseTemplateManager.ResponseTemplateExists(template.Name))
                    {
                        template.Save();

                        FormFolder folder = folderId == -1 
                            ? FolderManager.GetRoot() 
                            : FolderManager.GetFolder(folderId);

                        folder.Add(template);

                        folder.Save(UserManager.AuthenticateUser("admin", "admin"));
                    }
                }
                catch
                {
                    //Ignore individual errors so that an attempt is made to load each survey
                }
            }
        }

        /// <summary>
        /// Sets the default policy for an imported <see cref="LibraryTemplate"/>.
        /// </summary>
        /// <param name="template">The <see cref="LibraryTemplate"/> to configure.</param>
        private static void SetDefaultPolicy(IAccessControllable template)
        {
            string[] defaultPolicyPermissions = { "Library.View", "Library.Edit" };
            SecurityEditor editor = template.GetEditor();

            if (editor != null)
            {
                editor.Initialize(UserManager.AuthenticateUser("admin", "admin"));
                editor.SetDefaultPolicy(editor.ControllableResource.CreatePolicy(defaultPolicyPermissions));
            }
        }

        /// <summary>
        /// Retrieves a libraries' name
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetLibraryName(DataSet data)
        {
            string libraryName = string.Empty;

            //Get the library name from the template texts table
            //This is a bit of a hack, but a library template doesn't have an unlocalized name member like a response
            //template does.  A library's name is stored only as localized text in the text table
            if (data != null && data.Tables.Contains("TemplateTexts"))
            {
                DataRow[] nameRows = data.Tables["TemplateTexts"].Select("TextIDSuffix = 'name'", null, DataViewRowState.CurrentRows);

                if (nameRows.Length > 0 && nameRows[0]["TextValue"] != DBNull.Value)
                {
                    libraryName = (string)nameRows[0]["TextValue"];
                }
            }

            return libraryName ?? string.Empty;
        }

        /// <summary>
        /// Retrieves a libraries' description
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetLibraryDescription(DataSet data)
        {
            string libraryDescription = string.Empty;
            //Get the library name from the template texts table
            //This is a bit of a hack, but a library template doesn't have an unlocalized name member like a response
            //template does.  A library's name is stored only as localized text in the text table
            if (data != null && data.Tables.Contains("TemplateTexts"))
            {
                DataRow[] descriptionRows = data.Tables["TemplateTexts"].Select("TextIDSuffix = 'description'", null, DataViewRowState.CurrentRows);

                if (descriptionRows.Length > 0 && descriptionRows[0]["TextValue"] != DBNull.Value)
                {
                    libraryDescription = (string)descriptionRows[0]["TextValue"];
                }
            }

            return libraryDescription ?? string.Empty;
        }


        /// <summary>
        /// Return result as JSON
        /// </summary>
        /// <param name="result"></param>
        protected void WriteResult(object result)
        {
            var serializer = new JavaScriptSerializer();

            Response.ContentType = "application/json";
            Response.Write(serializer.Serialize(result));
        }
    }
}