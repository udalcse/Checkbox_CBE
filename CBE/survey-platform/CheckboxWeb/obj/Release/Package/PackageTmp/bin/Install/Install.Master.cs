using System;
using System.Web.UI;
using System.Text;
using Checkbox.Management;

namespace CheckboxWeb.Install
{
    public partial class InstallMaster : MasterPage
    {
        public void HideChrome()
        {
            _headerPlace.Visible = false;
            //_footerPlace.Visible = false;
        }

        public void HideHeader()
        {
            _headerPlace.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ApplicationManager.AppSettings.TypeKitGuid != null)
                TypeKit.Text =
                    "<script type=\"text/javascript\" src=\"//use.typekit.net/" + ApplicationManager.AppSettings.TypeKitGuid + 
                    ".js\"></script><script type=\"text/javascript\">try{Typekit.load();}catch(e){}</script>";
        }
    }
}
