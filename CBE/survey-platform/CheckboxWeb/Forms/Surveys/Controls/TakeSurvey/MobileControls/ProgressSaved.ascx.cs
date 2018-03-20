using System;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Wcf.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls
{
    public partial class ProgressSaved : Checkbox.Web.Forms.TakeSurvey.ProgressSavedBase
    {
        protected override Button SendResumeEmailBtn
        {
            get { return _sendResumeEmailBtnMobile; }
        }

        protected override HyperLink ReturnLink
        {
            get { return _returnLinkMobile; }
        }

        protected override TextBox EmailAddressField
        {
            get { return _emailAddressFieldMobile; }
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

            _toResumeLbl.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_MOBILERESUME", responseTemplateId, surveyLanguage, "en-US");

            _returnLinkMobile.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_CLICKHERE", responseTemplateId, surveyLanguage, "en-US")
                 + SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS_TORETURN", responseTemplateId, surveyLanguage, "en-US");
            _returnLinkMobile.Attributes["data-role"] = "button";

            _returnLinkMobile.NavigateUrl = string.Format("{0}/Survey.aspx?iid={1}", ApplicationManager.ApplicationPath, responseSessionGuid);

            _sendResumeEmailBtnMobile.Attributes["data-role"] = "button";
        }
    }
}