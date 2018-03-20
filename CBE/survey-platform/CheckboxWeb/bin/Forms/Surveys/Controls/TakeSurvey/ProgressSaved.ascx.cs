using System;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Wcf.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProgressSaved : Checkbox.Web.Forms.TakeSurvey.ProgressSavedBase
    {
        protected override Button SendResumeEmailBtn
        {
            get { return _sendResumeEmailBtn; }
        }

        protected override HyperLink ReturnLink
        {
            get { return _returnLink; }
        }

        protected override TextBox EmailAddressField
        {
            get { return _emailAddressField; }
        }

        protected override Panel ProgressSavedEmailSentPanel
        {
            get { return _progressSavedEmailSentPanel; }
        }

        protected override Panel ProgressSavedEmailErrorPanel
        {
            get { return _progressSavedEmailErrorPanel; }
        }

        protected override Panel EmailProgressLinkPanel
        {
            get { return _emailProgressLinkPanel; }
        }

        protected override Literal ProgressSavedTxt
        {
            get { return _progressSavedTxt; }
        }

        protected override Literal ToEmailLbl
        {
            get { return _toEmailLbl; }
        }

        protected override Literal EmailErrorLabel
        {
            get { return _emailErrorLabel; }
        }

        protected override Literal EmailSentLabel
        {
            get { return _emailSentLabel; }
        }

        protected override string GetSurveyText(string textCode)
        {
            return SurveyEditorServiceImplementation.GetSurveyText(textCode, ResponseTemplateId, SurveyLanguage, "en-US");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseSessionGuid"></param>
        /// <param name="surveyLanguage"></param>
        /// <param name="enableEmail"></param>
        /// <param name="surveyTitle"></param>
        /// <param name="emailFromAddress"></param>
        /// <param name="responseTemplateId"></param>
        public override void Initialize(Guid? responseSessionGuid, string surveyLanguage, bool enableEmail, string surveyTitle, string emailFromAddress, int responseTemplateId)
        {
            base.Initialize(responseSessionGuid, surveyLanguage, enableEmail, surveyTitle, emailFromAddress, responseTemplateId);

            _linkTxt.Text = string.Format("{0}/Survey.aspx?iid={1}", ApplicationManager.ApplicationPath, responseSessionGuid);

            _toResumeLbl.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_TORESUME", responseTemplateId, surveyLanguage, "en-US");

            _returnLink.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_CLICKHERE", responseTemplateId, surveyLanguage, "en-US");

            _toContinueLbl.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_TORETURN", responseTemplateId, surveyLanguage, "en-US");

            _returnLink.NavigateUrl = _linkTxt.Text;

            _emailAddressLbl.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_EMAILADDRESS", responseTemplateId, surveyLanguage, "en-US");

            //prefill user email
            var currentPrincipal = UserManager.GetCurrentPrincipal();

            if (currentPrincipal != null)
                _emailAddressField.Text = currentPrincipal.Email;
            
        }
    }
}