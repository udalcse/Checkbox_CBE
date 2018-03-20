using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Footer : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            _enableFooter.Checked = ApplicationManager.AppSettings.FooterEnabled;
            _enableFooter.Visible = !ApplicationManager.AppSettings.EnableMultiDatabase;

            _pipeSelector.Initialize(null, null, WebTextManager.GetUserLanguage(), _footerText.ClientID);

            Master.OkClick += Master_OkClick;
            base.OnInit(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ApplicationManager.AppSettings.FooterEnabled = _enableFooter.Checked;
            if (ApplicationManager.AppSettings.FooterEnabled)
            {
                var footerText = _footerText.Text;

                //add unsubscribe-link placeholder
                var regex = new Regex("<a\\s([^>]*\\s)?id=\"unsubscribeLink\"(.*?)>");
                footerText = regex.Replace(footerText, "<a id=\"unsubscribeLink\" href=\"" + InvitationManager.OPT_OUT_URL_PLACEHOLDER + "\">");

                ApplicationManager.AppSettings.FooterText = footerText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                _footerText.Text = ApplicationManager.AppSettings.FooterText;
            }
        }

        protected void Validate(object source, ServerValidateEventArgs args)
        {
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                List<Checkbox.Invitations.CompanyProfile.Property> properties;
         
                var error = InvitationEmailTextValidator.ValidateInvitationText(
                    args.Value, out properties, true, false);

                if (error == InvitationEmailTextValidator.ErrorType.FooterMissed)
                {
                    string param = string.Join(", ", properties.Select(p => ApplicationManager.AppSettings.PipePrefix + p).ToArray());
                    var errorText = WebTextManager.GetText("/pageText/settings/footer.aspx/footerTextAddressValidatorMessage");

                    _footerTextValidator.ErrorMessage = string.Format(errorText, param);

                    args.IsValid = false;                    
                } 
                else if (error == InvitationEmailTextValidator.ErrorType.OptOutLinkMissed)
                {
                    _footerTextValidator.ErrorMessage = WebTextManager.GetText("/pageText/settings/footer.aspx/footerTextOptOutValidatorMessage");

                    args.IsValid = false;
                }
            }
        }

    }   
}