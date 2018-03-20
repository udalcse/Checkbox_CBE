using System;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using CheckboxWeb.Controls.Navigation;
using Prezza.Framework.Security.Principal;

namespace CheckboxWeb.Controls
{
    public partial class Header : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public NavMenu NavMenu { get { return _navMenu; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var currentUser = HttpContext.Current.User;

            if (String.IsNullOrEmpty(currentUser.Identity.Name))
            {
                if(!ApplicationManager.AppSettings.ShowNavWhenNotAuthenticated)
                {
                    _navMenu.Visible = false;
                }
                HideSearch();
            }
            _infoPlace.EditInfoEnabled = (ApplicationManager.AppSettings.AllowEditSelf && !String.IsNullOrEmpty(currentUser.Identity.Name));

            _settingsGear.AddControlToSecure(settings);
            _settingsGear.CurrentPrincipal = UserManager.GetCurrentPrincipal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateHeader();
        }

        /// <summary>
        /// Hide search box
        /// </summary>
        public void HideSearch()
        {
            _searchPlace.Visible = false;
        }

        /// <summary>
        /// Update header to reflect current app settings
        /// </summary>
        public void UpdateHeader()
        {
            //Set logo attributes 
            _siteLogo.Src = "~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png";
            _siteLogo.Attributes["height"] = "42";
            _siteLogo.Attributes["width"] = "195";
            _siteLogo.Visible = true;
            _textLogoPanel.Visible = false;

            //Online accounts must use the Checkbox logo
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
                return;

            //If the account is a server account, they can change there logo or use text.
            //If they use a logo, pull the logo that they have set or the default if one does not exist.
            if (ApplicationManager.AppSettings.HeaderTypeChosen == AppSettings.HeaderType.Logo)
            {
                if (!string.IsNullOrEmpty(ApplicationManager.AppSettings.HeaderLogo))
                    _siteLogo.Src = ApplicationManager.AppSettings.HeaderLogo;

                return;
            }

            //If the type is not logo, then display the text.
            _siteLogo.Visible = false;
            _textLogoPanel.Visible = true;

            _headerLbl.Text = WebTextManager.GetText("/siteText/headerText");

            _textLogoPanel.Style["color"] = ApplicationManager.AppSettings.HeaderTextColor;
            _textLogoPanel.Style["font-family"] = ApplicationManager.AppSettings.HeaderFont;

            int intHeaderFontSize;
            var headerFontSize = ApplicationManager.AppSettings.HeaderFontSize;
            if (int.TryParse(headerFontSize, out intHeaderFontSize))
            {
                _textLogoPanel.Style["font-size"] = string.Format("{0}px", intHeaderFontSize);
            }
            else
            {
                _textLogoPanel.Style["font-size"] = headerFontSize;
            }

            
        }
    }
}