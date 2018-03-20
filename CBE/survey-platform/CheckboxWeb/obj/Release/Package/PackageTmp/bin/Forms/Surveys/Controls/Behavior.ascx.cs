using System.Web.UI;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class Behavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Override onload to enable/disable options
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            _resumeOptionsPanel.Enabled = AllowResume;
            _resumeLinkFromEmail.Enabled = AllowResume && EnableSendResumeEmail;
            _resumeLinkFromEmailLbl.Enabled = AllowResume && EnableSendResumeEmail;
        }

        /// <summary>
        /// Determine if responses are anonymize
        /// </summary>
        public bool AreResponsesAnonymize
        {
            get { return _anonymizeResponses.Checked; }
            set { _anonymizeResponses.Checked = value; }
        }

        /// <summary>
        /// Get/set whether enable back button is enabled.
        /// </summary>
        public bool EnableBackButton
        {
            get { return _enableBackButton.Checked; }
            set { _enableBackButton.Checked = value; }
        }

        /// <summary>
        /// Get/set whether response editing is enabled.
        /// </summary>
        public bool EnableEdit
        {
            get { return _enableEdit.Checked; }
            set { _enableEdit.Checked = value; }
        }

        /// <summary>
        /// Get/set whether resuming incomplete responses is enabled
        /// </summary>
        public bool AllowResume
        {
            get { return _allowResume.Checked; }
            set { _allowResume.Checked = value; }
        }

        /// <summary>
        /// Get/set whether to show save and exit button
        /// </summary>
        public bool ShowSaveAndExitButton
        {
            get { return _showSaveAndExitButton.Checked; }
            set { _showSaveAndExitButton.Checked = value; }
        }

        /// <summary>
        /// Get/set whether to show send resume link via email.
        /// </summary>
        public bool EnableSendResumeEmail
        {
            get { return _showEmailResumeLinkDialog.Checked; }
            set { _showEmailResumeLinkDialog.Checked = value; }
        }

        /// <summary>
        /// Get/set email address to use as from address when sending
        /// links.
        /// </summary>
        public string ResumeLinkEmailFromAddress
        {
            get { return _resumeLinkFromEmail.Text.Trim(); }
            set { _resumeLinkFromEmail.Text = value.Trim(); }
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void Initialize(SurveyBehaviorSettings behaviorSettings)
        {
            AreResponsesAnonymize = behaviorSettings.AnonymizeResponses;
            EnableBackButton = !behaviorSettings.DisableBackButton;
            EnableEdit = behaviorSettings.AllowEdit;
            AllowResume = behaviorSettings.AllowContinue;
            ShowSaveAndExitButton = behaviorSettings.ShowSaveAndQuit;
            EnableSendResumeEmail = behaviorSettings.EnableSendResumeEmail;
            ResumeLinkEmailFromAddress = behaviorSettings.ResumeEmailFromAddress ?? string.Empty;
        }

        /// <summary>
        /// Update survey with selected options
        /// </summary>
        /// <param name="behaviorSettings"></param>
        public void UpdateSettings(SurveyBehaviorSettings behaviorSettings)
        {
            behaviorSettings.AnonymizeResponses = AreResponsesAnonymize;
            behaviorSettings.DisableBackButton = !EnableBackButton;
            behaviorSettings.AllowEdit = EnableEdit;
            behaviorSettings.AllowContinue = AllowResume;
            behaviorSettings.ShowSaveAndQuit = ShowSaveAndExitButton;
            behaviorSettings.EnableSendResumeEmail = EnableSendResumeEmail;
            behaviorSettings.ResumeEmailFromAddress = ResumeLinkEmailFromAddress ?? string.Empty;
        }
    }
}