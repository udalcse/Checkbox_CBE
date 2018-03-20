using Checkbox.Configuration.Install;
using Checkbox.Users;
using Checkbox.Web.Page;
using Prezza.Framework.Security.Principal;
using System;

namespace CheckboxWeb.Install
{
    public partial class Maintenance : CheckboxServerProtectedPage
    {
        private ApplicationInstaller _appInstaller;

        protected ApplicationInstaller AppInstaller
        {
            get { return _appInstaller; }
        }

        /// <summary>
        /// Initializes the data grids
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _appInstaller = (ApplicationInstaller)Session["_appInstaller"] ??
                         new ApplicationInstaller(Server.MapPath("~"), false, string.Empty);

            _availablePatchesGrid.DataSource = _appInstaller.AvailablePatches;
            _installedPatchesGrid.DataSource = _appInstaller.InstalledPatches;

            _availablePatchesGrid.DataBind();
            _installedPatchesGrid.DataBind();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Also make sure the user is a system administrator
            ExtendedPrincipal princpal = UserManager.GetCurrentPrincipal();

            if (princpal != null && princpal.IsInRole("System Administrator")) return;
            Response.Redirect(ResolveUrl("~/Login.aspx") + "?ReturnUrl=Install/Maintenance.aspx", false);

            if (IsPostBack)
            {
                
            }
        }
    }
}