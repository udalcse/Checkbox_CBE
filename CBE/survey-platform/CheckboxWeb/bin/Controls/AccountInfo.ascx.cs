using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Users;

namespace CheckboxWeb.Controls
{
    public partial class AccountInfo : Checkbox.Web.Common.UserControlBase
    {
        public bool EditInfoEnabled { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           
            var currentUser = HttpContext.Current.User;
            
            _loggedInName.Text = string.IsNullOrWhiteSpace(currentUser.Identity.Name)
                                     ? "Guest"
                                     : currentUser.Identity.Name;

            if (ApplicationManager.AppSettings.EnableMultiDatabase &&
                (currentUser.IsInRole("System Administrator")
                || currentUser.IsInRole("Survey Editor")
                || currentUser.IsInRole("Survey Administrator")
                || currentUser.IsInRole("Report Administrator")))
            {
                _supportPlace.Visible = true;
            }

            if (ApplicationManager.AppSettings.DisplayAvailableSurveyList)
            {
                _availableSurveysPlace.Visible = true;
            }

            if (ApplicationManager.AppSettings.DisplayAvailableReportList)
            {
                _availableReportsPlace.Visible = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _editInfoPlace.Visible = EditInfoEnabled;
        }

        
        /// <summary>
        /// Checks if a user is logged in 
        /// </summary>
        protected bool LoggedIn
        {
            get
            {
                return HttpContext.Current.User != null &&
                    !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name);
            }
        }
    }
}