using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Management;
using Checkbox.Security.Principal;
using System.Web;
using CheckboxWeb.Controls.Wizard.WizardControls;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Launch survey wizard
    /// </summary>
    public partial class Launch : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Form.Administer"; } }


        /// <summary>
        /// Override to bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            
            //Bind events
            _wizard.NextButtonClick += Wizard_NextButtonClick;
            _wizard.FinishButtonClick += Wizard_FinishButtonClick;
            _wizard.CancelButtonClick += Wizard_CancelButtonClick;
            _closeWizardButton.Click += Wizard_CancelButtonClick;

            _share.Initialize(ResponseTemplate);

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/launch.aspx/launchSurveyWizard") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64);

            //Hide Master dialog buttons
            Master.HideDialogButtons();
        }

        /// <summary>
        /// On load
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Initialize and load grant permission list if step index == 1
            _securitySelector.Initialize(ResponseTemplate, UserManager.GetCurrentPrincipal(), Page.IsPostBack, _wizard.ActiveStepIndex == 1);

            if (!Page.IsPostBack)
            {
                _behaviorOptions.Initialize(ResponseTemplate.BehaviorSettings);
                _responseLimits.Initialize(ResponseTemplate.BehaviorSettings);
                _timeLimits.Initialize(ResponseTemplate.BehaviorSettings);
            }

           
        }       

        /// <summary>
        /// Cancel wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wizard_CancelButtonClick(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>
                           {
                               {"op", "refresh"},
                           };

            Master.CloseDialog(args);
        }


        /// <summary>
        /// Handle clicking wizard next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (_wizard.WizardSteps[e.NextStepIndex].ID == "_stepConfirm")
            {
                e.Cancel = !_timeLimits.Validate();
                UpdateSummary();
            }

        }


        /// <summary>
        /// Handle clicking wizard finish button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            //Update, apply security
            UpdateSettings(ResponseTemplate.BehaviorSettings);

            ResponseTemplateManager.InitializeResponseTemplateAclAndPolicy(
                ResponseTemplate,
                UserManager.GetCurrentPrincipal());

            //Save
            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplate.Save();
            if (ResponseTemplate.ID != null) ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);
        }

        /// <summary>
        /// Update summary page
        /// </summary>
        private void UpdateSummary()
        {
            //Use dummy settings objects so we don't work on a live template
            var settings = new SurveyBehaviorSettings();

            UpdateSettings(settings);
            
            _summary.Initialize(settings);
        }

        /// <summary>
        /// Update settings with control values
        /// </summary>
        /// <param name="settingsToUpdate"></param>
        private void UpdateSettings(SurveyBehaviorSettings settingsToUpdate)
        {
            _securitySelector.UpdateSettings(settingsToUpdate);
            _behaviorOptions.UpdateSettings(settingsToUpdate);
            _responseLimits.UpdateSettings(settingsToUpdate);
            _timeLimits.UpdateSettings(settingsToUpdate);

            _timeLimits.Initialize(settingsToUpdate);

            //Set is active flag
            settingsToUpdate.IsActive = true;
        }
    }
}
