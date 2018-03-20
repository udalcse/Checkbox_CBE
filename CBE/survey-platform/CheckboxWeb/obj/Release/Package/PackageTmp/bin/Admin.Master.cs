using System;
using System.Reflection;
using System.Web.UI;
using Checkbox.Configuration.Install;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Admin : BaseMasterPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsEmbedded)
            {
                _mDomUpdateInclude.Visible = false;
            }

            CheckDatabaseVersion();

            if (ApplicationManager.AppSettings.TypeKitGuid != null)
                TypeKit.Text =
                    "<script type=\"text/javascript\" src=\"//use.typekit.net/" + ApplicationManager.AppSettings.TypeKitGuid +
                    ".js\"></script><script type=\"text/javascript\">try{Typekit.load();}catch(e){}</script>";

            _title.Text = WebTextManager.GetText("/siteText/siteName",
                                                 WebTextManager.GetUserLanguage(), "Checkbox&reg; 6 Survey");

        }

        private void CheckDatabaseVersion()
        {
            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                var appInstaller = ApplicationInstaller.CachedApplicationInstaller;
                if (appInstaller == null)
                {
                    appInstaller = new ApplicationInstaller(Server.MapPath("~"), false, string.Empty);
                    ApplicationInstaller.CachedApplicationInstaller = appInstaller;
                }

                var dbVersion = appInstaller.CurrentPatchVersion;

                var appVersionString = Assembly.Load("Checkbox").GetName().Version.ToString();
                var appVersion = new Version(appVersionString.Remove(appVersionString.LastIndexOf('.')));

                if (appVersion.CompareTo(dbVersion) != 0)
                {
                    _databaseVersionWarning.Message = WebTextManager.GetText("/siteText/databaseVersionWarning", WebTextManager.GetUserLanguage(),
                        "The database version is obsolete, please apply an appropriate patch.");
                    _databaseVersionWarning.MessageType = StatusMessageType.Warning;
                    _databaseVersionWarning.ShowStatus();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titleControl"></param>
        public override void SetTitleControl(Control titleControl)
        {
            //_titlePlace.Controls.Clear();
            //_titlePlace.Controls.Add(titleControl);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public override void HideTitle()
        //{
        //    _pageTitlePlace.Visible = false;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        public override void ShowError(string error, Exception ex)
        {
            _errorMessageLiteral.Text = error;
            _errorDescriptionLiteral.Text = ex != null ? ex.Message : string.Empty;

            _errorPlace.Visible = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void HideError()
        {
            _errorPlace.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideSearch()
        {
            _headerCtrl.HideSearch();
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ActiveMenuSection
        {
            get { return _headerCtrl.NavMenu.ActiveMenuItem; }
            set{_headerCtrl.NavMenu.ActiveMenuItem = value;}
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideHeader()
        {
            _headerCtrl.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void HideFooter()
        {
            _footer.Visible = false;
        }
    }

}
