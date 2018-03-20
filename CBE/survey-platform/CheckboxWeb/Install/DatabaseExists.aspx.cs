using System;
using Checkbox.Configuration.Install;
using Checkbox.Web.Page;

namespace CheckboxWeb.Install
{
    public partial class DatabaseExists : CheckboxServerProtectedPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RegisterClientScriptInclude("DialogHandler", ResolveUrl("~/Resources/DialogHandler.js"));
            
            var appInstaller = (ApplicationInstaller)Session["_appInstaller"];

            if (appInstaller != null
                && "upgrade".Equals(appInstaller.InstallType, StringComparison.InvariantCultureIgnoreCase))
            {
                var itemToRemove = _dbExistsOptions.Items.FindByValue("UseDB");

                if (itemToRemove != null)
                {
                    _dbExistsOptions.Items.Remove(itemToRemove);
                }
            }
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            var arg = "{dbOption:'" + _dbExistsOptions.SelectedValue + "', dbPW:'" + Session["dbPW"] + "'}";
            ClientScript.RegisterStartupScript(GetType(), "CloseDialog", "window.parent.jQuery.modal.close(" + arg + ");", true);
            //String script = "<script>closeWindow(" + arg + ");</script>";
            //ClientScript.RegisterStartupScript(this.GetType(), "CloseDialog", script);

            //UseDB
        }
    }
}
