using System;
using System.Web.UI.WebControls;
using Checkbox.Web;
using CheckboxWeb.Controls.Button;

namespace CheckboxWeb.Controls.Wizard.WizardControls
{
    public partial class WizardButtons : WizardTemplateControl
    {
        /// <summary>
        /// Get/set whether to use localization or not for texts. Necessary for cases where wizard is used
        /// when DB is not setup.
        /// </summary>
        public bool NonLocalized { get; set; }

        /// <summary>
        /// Get "Next" button
        /// </summary>
        public CheckboxButton NextButton
        {
            get { return _nextButton; }
        }

        /// <summary>
        /// Enable propery of next button
        /// </summary>
        public bool NextButtonEnabled
        {
            get { return _nextButton.Enabled; }
            set { _nextButton.Enabled = value; }
        }

        /// <summary>
        /// Determine if cancel button should close a dialog or not.
        /// </summary>
        public bool CloseDialogOnCancel
        {
            get { return _cancelButton.OnClientClick.Contains("closeWindow()"); }
            set { _cancelButton.OnClientClick = value ? "closeWindow();return true;" : String.Empty; }
        }

        /// <summary>
        /// Validation group of next button
        /// </summary>
        public String NextButtonValidationGroup
        {
            get { return NextButton.ValidationGroup; }
            set { NextButton.ValidationGroup = value; }
        }

        /// <summary>
        /// Get "Previous" button
        /// </summary>
        public CheckboxButton PreviousButton
        {
            get { return _previousButton; }
        }

        /// <summary>
        /// Get "Cancel" button
        /// </summary>
        public CheckboxButton CancelButton
        {
            get { return _cancelButton; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SetButtonState();
        }

        /// <summary>
        /// Localize button texts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (NonLocalized)
            {
                _cancelButton.Text = "CANCEL";
                _previousButton.Text = "PREVIOUS";
                _nextButton.Text = "NEXT";

            }
            else
            {
                _cancelButton.Text = WebTextManager.GetText("/controlText/wizard/cancel");
                _previousButton.Text = WebTextManager.GetText("/controlText/wizard/previous");
                _nextButton.Text = WebTextManager.GetText("/controlText/wizard/next");
            }
        }

        protected override void AssociatedWizardControl_ActiveStepChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }

        protected void SetButtonState()
        {
            //Cancel button
            _cancelButton.Visible = AssociatedWizardControl.DisplayCancelButton;

            //Previous button
            Int32 activeStepIndex = AssociatedWizardControl.ActiveStepIndex;
            _previousButton.Visible = activeStepIndex != 0 && AssociatedWizardControl.WizardSteps[activeStepIndex - 1].AllowReturn;

            //Next button
            if (AssociatedWizardControl.ActiveStep.StepType == WizardStepType.Finish)
            {
                _nextButton.Text = NonLocalized ? "FINISH" : WebTextManager.GetText("/controlText/wizard/finish");
                _nextButton.CommandName = "MoveComplete";
            }
            else
            {
                _nextButton.Text = NonLocalized ? "NEXT" : WebTextManager.GetText("/controlText/wizard/next");
                _nextButton.CommandName = "MoveNext";
            }
        }
    }
}