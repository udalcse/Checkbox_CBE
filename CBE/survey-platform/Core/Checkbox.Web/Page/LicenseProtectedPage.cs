using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Management.Licensing;
using Checkbox.Management.Licensing.Limits;
using Prezza.Framework.Data;

//using Xheo.Licensing;

using Prezza.Framework.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for all pages protected by the licensing component.
    /// </summary>
    [LicenseProvider(typeof(CheckboxLicenseProvider))]
    public class LicenseProtectedPage : BasePage
    {
        private readonly LicenseValidator _licenseValidator;

        private const string TRIAL_MESSAGE_SERVER = "<div id=\"trialBanner\" style=\"border-bottom:2px solid #EE852B;background:white url('/App_Themes/CheckboxTheme/Images/pattern-stripes-ltgrey.png') repeat 0 0;padding: 4px;text-align:center;font-weight:bold;color:#444;\">Checkbox&reg; Survey Trial Evaluation</div>";
        private const string TRIAL_MESSAGE_WEB = "<div id=\"trialBanner\" style=\"border-bottom:2px solid #EE852B;background:white url('/App_Themes/CheckboxTheme/Images/pattern-stripes-ltgrey.png') repeat 0 0;padding: 4px;text-align:center;font-weight:bold;color:#444;\">Checkbox&reg; Survey Trial Evaluation {0} {1}</div>";

        private const string TRIAL_CHAT_SCRIPT =
          "<script type='text/javascript'> window.olark||(function(c){var f=window,d=document,l=f.location.protocol==\"https:\"?\"https:\":\"http:\",z=c.name,r=\"load\";(function(){f[z]=function(){ (a.s=a.s||[]).push(arguments)};var a=f[z]._={},q=c.methods.length;while(q--){(function(n){f[z][n]=function(){f[z](\"call\",n,arguments)}})(c.methods[q] )}a.v=0;a.l=c.loader;a.i=arguments.callee;a.f=setTimeout(function(){if(a.f){(new Image).src=l+\"//\"+a.l.replace(\".js\",\".png\")+\"&\"+escape(f.location.href) }a.f=null},20000);a.p={0:+new Date};a.P=function(u){a.p[u]=new Date-a.p[0]};function s(){a.P(r);f[z](r)}f.addEventListener?f.addEventListener(r,s,false ):f.attachEvent(\"on\"+r,s);(function(){function p(){return[\"<head></head><\",i,' onload=\"var d=',g,\";d.getElementsByTagName('head')[0].\",j,\"(d.\",h, \"('script')).\",k,\"='\",l,\"//\",a.l,\"'\\\"></\",i,\">\"].join(\"\")}var i=\"body\",m=d[i];if(!m){return setTimeout(arguments.callee,100)}a.P(1);var j=\"appendChild\", h=\"createElement\",k=\"src\",n=d[h](\"div\"),v=n[j](d[h](z)),b=d[h](\"iframe\"),g=\"document\",e=\"domain\",o;n.style.display=\"none\";m.insertBefore(n,m.firstChild ).id=z;b.frameBorder=\"0\";b.id=z+\"-loader\";if(/MSIE\\s+6/.test(navigator.userAgent)){b.src=\"javascript:false\"}b.allowTransparency=\"true\";v[j](b);try{ b.contentWindow[g].open()}catch(w){c[e]=d[e];o=\"javascript:var d=\"+g+\".open();d.domain='\"+d.domain+\"';\";b[k]=o+\"void(0);\"}try{var t=b.contentWindow[g]; t.write(p());t.close()}catch(x){b[k]=o+'d.write(\"'+p().replace(/\"/g,'\\\\\"')+'\");d.close();'}a.P(2)})()})()})({loader:(function(m){ return\"static.olark.com/jsclient/loader0.js?ts=\"+(m?m[1]:(+new Date))})(document.cookie.match(/olarkld=(\\d+)/)), name:\"olark\",methods:[\"configure\",\"extend\",\"declare\",\"identify\"]}); olark.identify('1291-773-10-9695');</script>";

        private bool _writeTrialMessageInOnRender;

        /// <summary>
        /// Retrieve and Validate the license.
        /// </summary>
        public LicenseProtectedPage()
        {
            //Build activation message

            //bool licenseValid;
            CheckboxLicense license = null;

            try
            {
                license = LicenseManager.Validate(typeof(LicenseProtectedPage), this) as CheckboxLicense;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                //Store the error message
                HttpContext.Current.Items["LicenseErrorMessage"] = ex.Message;
                HttpContext.Current.Server.Transfer(ResolveUrl("~/ErrorPages/LicenseError.aspx"));
                return;
            }

            //Construct a license validator, which also attempts to get a valid license
            _licenseValidator = new LicenseValidator(license);

            //If the license is null or is not valid
            if (!_licenseValidator.IsLicenseValid)
            {
                //Store the error message
                HttpContext.Current.Items["LicenseErrorMessage"] = _licenseValidator.LicenseError;
                HttpContext.Current.Server.Transfer(ResolveUrl("~/ErrorPages/LicenseError.aspx"));
            }
        }

        /// <summary>
        /// Validate license limits
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ValidateLimits(out string message)
        {
            //Only validate one at a time
            var limits = GetLimits();

            foreach (var limit in limits)
            {
                var result = limit.Validate(out message);

                if (result == LimitValidationResult.LimitExceeded || result == LimitValidationResult.UnableToEvaluate)
                {
                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Get license limits, currently only handle numeric cases
        /// </summary>
        /// <returns></returns>
        protected virtual List<LicenseLimit> GetLimits()
        {
            return new List<LicenseLimit>();
        }

        /// <summary>
        /// Get a value from a license
        /// </summary>
        /// <param name="valueName">Name of value to get.</param>
        /// <returns></returns>
        protected string GetLicenseValue(string valueName)
        {
            return _licenseValidator.GetLicenseValue(valueName);
        }

        ///<summary>
        /// Gets the expiration message for the trial banner
        /// </summary>
        protected string GetExpirationMessage()
        {
            try
            {
                var endDate = DateTime.MinValue;

                var db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                var command = db.GetStoredProcCommandWrapper("ckbx_GetApplicationContextDetails");
                command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);

                using (var reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            endDate = DbUtility.GetValueFromDataReader(reader, "enddate", DateTime.MinValue);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return endDate != DateTime.MinValue
                           ? string.Format("Your trial will expire in {0} days on {1}.", endDate.Subtract(DateTime.Now).Days, endDate.ToShortDateString())
                           : string.Empty;
            }
            catch
            {
                return null;
            }
        }

        ///<summary>
        /// Gets the upgrade message
        /// If the contextguid is valid, will return the actual message with a link, if not will return a blank string.
        /// </summary>
        /// <returns></returns>
        protected string GetUpgradeMessage()
        {
            try
            {
                string contextGuid = null;
                var db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                var command = db.GetStoredProcCommandWrapper("ckbx_GetContextGuid");
                command.AddInParameter("contextname", DbType.String, Request.Headers["Host"]);

                using (var reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            contextGuid = DbUtility.GetValueFromDataReader(reader, "ContextGuid", string.Empty);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                return !string.IsNullOrEmpty(contextGuid)
                           ? string.Format("<a href=\"http://www.checkbox.com/upgrade/{0}\">Upgrade now!</a>",
                                           contextGuid)
                           : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the active license
        /// </summary>
        public CheckboxLicense ActiveLicense
        {
            get { return _licenseValidator.ActiveLicense; }
        }

        /// <summary>
        /// Override OnPreRender to attach a license or other error message to the top of the application body
        /// if necessary or to redirect to an error page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string message;

            //If there is no license, write an error
            if (!_licenseValidator.IsLicenseValid)
            {
                HttpContext.Current.Server.Transfer(ApplicationManager.ApplicationRoot + "/ErrorPages/LicenseError.aspx");
                return;
            }

            //If the trial is expired
            if (_licenseValidator.IsLicenseExpired)
            {
                HttpContext.Current.Server.Transfer(ApplicationManager.ApplicationRoot + "/ErrorPages/TrialExpired.aspx");
                return;
            }

            if (!ValidateLimits(out message))
            {
                HttpContext.Current.Response.Redirect(ApplicationManager.ApplicationRoot + "/ErrorPages/LimitError.aspx?msg=" + HttpContext.Current.Server.UrlEncode(message), false);
                return;
            }

            if (_licenseValidator.IsTrial)
            {
                //Set a flag indicating that the trial message should be written in OnRender because it couldn't be inserted as a dynamic body control.
                _writeTrialMessageInOnRender = !AddMessageControlToPageBody(TRIAL_MESSAGE_SERVER);
                return;
            }

            if (ApplicationManager.AppSettings.EnableMultiDatabase && ApplicationManager.IsDataContextTrial)
            {
                _writeTrialMessageInOnRender = !AddMessageControlToPageBody(string.Format(TRIAL_MESSAGE_WEB, GetExpirationMessage(), GetUpgradeMessage()));
            }
             
        }

        /// <summary>
        /// Add a message literal control to the page body.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Returns a boolean indicating whether control could be added or not.</returns>
        /// <remarks>The literal control is used to try to avoid splashing html above the html tag, which can cause issues with
        /// proper rendering in standards compliant browser.</remarks>
        private bool AddMessageControlToPageBody(string message)
        {
            //Skip message for dialogs
            if (Page != null && (Page.Master == null || ("ASP.dialog_master".Equals(Page.Master.ToString(), StringComparison.InvariantCultureIgnoreCase))))
            {
                return true;
            }

            if (Page != null && Page.Form != null)
            {
                var messageControl = Page.Form.FindControl("_trialMessagePlace");
                //If control not found, check for page's 0th control, which is likely the master page
                if (messageControl == null && Page.Controls.Count > 0)
                {
                    messageControl = Page.Form.Controls[0].FindControl("_trialMessagePlace");
                }

                //Add message to body
                if (messageControl != null)
                {
                    messageControl.Controls.AddAt(0, new LiteralControl(message));
                    messageControl.Visible = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a trial or activation message, if necessary.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (_writeTrialMessageInOnRender)
            {
                writer.Write(TRIAL_MESSAGE_SERVER);
            }

            base.Render(writer);
        }

        ///<summary>
        ///
        ///</summary>
        ///<returns></returns>
        protected virtual string SystemAdministratorBanner()
        {
            return string.Empty;
        }
    }
}