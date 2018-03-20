using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web.UI;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Install
{
    public partial class TestSmtp : CheckboxServerProtectedPage
    {
        protected override void OnInit(EventArgs e)
        {
            if (ApplicationManager.AppSettings.InstallSuccess)
                throw new Exception("The page is unavailable.");

            base.OnInit(e);
            ((Dialog)Master).OkClick += Master_OkClick;
            ((Dialog)Master).CancelClick += Master_CancelClick;

            ((Dialog)Master).SetTitle(WebTextManager.GetText("/pageText/settings/testSmtp.aspx/title"));
        }

        //
        void Master_CancelClick(object sender, EventArgs e)
        {
            ((Dialog)Master).CloseDialog(null);
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

            ((Dialog)Master).CloseDialog(args);
        }

    }
}
