using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Forms.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ProgressSavedBase : Common.UserControlBase
    {
        #region Abstract Members

        protected abstract Button SendResumeEmailBtn { get; }
        protected abstract HyperLink ReturnLink { get; }
        protected abstract TextBox EmailAddressField { get; }

        protected abstract Panel ProgressSavedEmailSentPanel { get; }
        protected abstract Panel ProgressSavedEmailErrorPanel { get; }
        protected abstract Panel EmailProgressLinkPanel { get; }

        protected abstract Literal ProgressSavedTxt { get; }
        protected abstract Literal ToEmailLbl { get; }
        protected abstract Literal EmailErrorLabel { get; }
        protected abstract Literal EmailSentLabel { get; }

        protected abstract string GetSurveyText(string textCode);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected string SurveyTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string SurveyLanguage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int ResponseTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string MessageFromEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Guid? ResponseSessionGuid { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseSessionGuid"></param>
        /// <param name="surveyLanguage"></param>
        /// <param name="enableEmail"></param>
        /// <param name="surveyTitle"></param>
        /// <param name="emailFromAddress"></param>
        /// <param name="responseTemplateId"></param>
        public virtual void Initialize(Guid? responseSessionGuid, string surveyLanguage, bool enableEmail, string surveyTitle, string emailFromAddress, int responseTemplateId)
        {
            SurveyTitle = surveyTitle;
            SurveyLanguage = surveyLanguage;
            MessageFromEmail = emailFromAddress;
            ResponseSessionGuid = responseSessionGuid;
            ResponseTemplateId = responseTemplateId;

            ProgressSavedTxt.Text = GetSurveyText("SAVEPROGRESS_PROGRESSSAVED");

            if (!ApplicationManager.AppSettings.EmailSurveyResumeUrlEnabled)
                EmailProgressLinkPanel.Visible = false;
            else
            {
                MessageFromEmail = ApplicationManager.AppSettings.SystemEmailAddress;

                ToEmailLbl.Text = GetSurveyText("SAVEPROGRESS_TOEMAIL");
                SendResumeEmailBtn.Text = GetSurveyText("SAVEPROGRESS_SENDEMAIL");

                if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
                    SendResumeEmailBtn.OnClientClick = "return false;";

                SendResumeEmailBtn.Attributes["data-action"] = "resume-by-email";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendResumeEmail()
        {
            try
            {
                var subject = TextManager.GetText("/controlText/responseView/emailSubject", SurveyLanguage);
                var body = TextManager.GetText("/controlText/responseView/emailBody", SurveyLanguage);
                var sendTo = Request[EmailAddressField.UniqueID];

                subject = !string.IsNullOrEmpty(subject) ? subject.Replace("[SURVEY_NAME]", SurveyTitle) : SurveyTitle;

                if (!string.IsNullOrEmpty(body))
                {
                    body = body.Replace("[SURVEY_NAME]", SurveyTitle);

                    if (body.Contains("[SURVEY_LINK]"))
                        body = body.Replace("[SURVEY_LINK]", ReturnLink.NavigateUrl);
                    else
                        body += "\r\n\r\n" + ReturnLink.NavigateUrl;
                }
                else
                    body = ReturnLink.NavigateUrl;

                //Send the basic message
                var message = new EmailMessage
                {
                    From = MessageFromEmail ?? ApplicationManager.AppSettings.DefaultEmailFromName,
                    To = sendTo,
                    Subject = subject,
                    Body = body,
                    Format = MailFormat.Text
                };

                //send message to each individual in the to: field
                string[] addresses = message.To.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> badEmails = new List<string>();
                bool error = false;

                foreach (string address in addresses)
                {
                    message.To = address.Trim();

                    var emailValidator = new EmailValidator();
                    if (emailValidator.Validate(message.To))
                    {
                        try
                        {
                            EmailGateway.Send(message);
                        }
                        catch (Exception ex)
                        {
                            error = true;
                            ExceptionPolicy.HandleException(ex, "BusinessPublic");
                        }
                    }
                    else
                        badEmails.Add(address);
                }

                if (addresses.Length < 1)
                {
                    ProgressSavedEmailSentPanel.Visible = false;
                    ProgressSavedEmailErrorPanel.Visible = true;
                    EmailErrorLabel.Text = TextManager.GetText("/controlText/responseView/emptyEmail", SurveyLanguage);
                }
                else if (error)
                {
                    ProgressSavedEmailSentPanel.Visible = false;
                    ProgressSavedEmailErrorPanel.Visible = true;
                    EmailErrorLabel.Text = TextManager.GetText("/controlText/responseView/error", SurveyLanguage);
                }
                else if (badEmails.Any())
                {
                    ProgressSavedEmailSentPanel.Visible = false;
                    ProgressSavedEmailErrorPanel.Visible = true;

                    string errorText = TextManager.GetText("/controlText/responseView/emailValidation", SurveyLanguage) + "<br/>";
                    foreach (var email in badEmails)
                    {
                        errorText += Utilities.StripHtmlAndEncode(email) + "<br/>";
                    }

                    EmailErrorLabel.Text = errorText;
                }
                else
                {
                    ProgressSavedEmailSentPanel.Visible = true;
                    ProgressSavedEmailErrorPanel.Visible = false;
                    EmailSentLabel.Text = TextManager.GetText("/controlText/responseView/success", SurveyLanguage);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
            }
        }
    }
}
