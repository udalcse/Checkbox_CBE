using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.UI;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class TestSmtp : SettingsPage
    {
        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += Master_OkClick;
            Master.CancelClick += Master_CancelClick;

            Master.SetTitle(WebTextManager.GetText("/pageText/settings/testSmtp.aspx/title"));
        }

        //
        void Master_CancelClick(object sender, EventArgs e)
        {
            Master.CloseDialog(null);
        }

        //
        void Master_OkClick(object sender, EventArgs e)
        {
            Session["Email_TestPageSaved"] = true;
            ApplicationManager.AppSettings.SmtpServer = _TestSMTP.Host;
            ApplicationManager.AppSettings.SmtpPort = _TestSMTP.Port;
            ApplicationManager.AppSettings.EnableSmtpSsl = _TestSMTP.UseSsl;
            ApplicationManager.AppSettings.EnableSmtpAuthentication = _TestSMTP.UseSmtpAuthentication;
            ApplicationManager.AppSettings.SmtpUserName = _TestSMTP.UserName.Replace("\\\\", "\\");
            ApplicationManager.AppSettings.SmtpPassword = _TestSMTP.Password;

            var args = new Dictionary<string, string>();

            args.Add("result", "ok");
            args.Add("dialog", "Email.aspx");

            Master.CloseDialog(args);
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            if (!Page.IsPostBack)
            {
                _TestSMTP.DefaultTitle = WebTextManager.GetText("/pageText/settings/testSmtp.aspx/defaultSubject");
                _TestSMTP.DefaultBody = WebTextManager.GetText("/pageText/settings/testSmtp.aspx/defaultBody");
            }
        }
    }
}
