using System;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ResponseDetails : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/responseDetails.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.OkClick += new EventHandler(Master_OkClick);

            if (!Page.IsPostBack)
            {
                _displayIncompleteResponses.Checked = ApplicationManager.AppSettings.ResponseDetailsDisplayIncompleteResponses;
                _resultsPerPage.Text = ApplicationManager.AppSettings.ResponseDetailsResultsPerPage.ToString();

                _displayResponseDetails.Checked = ApplicationManager.AppSettings.ResponseDisplayDetails;
                _displayDetailedUserInfo.Checked = ApplicationManager.AppSettings.ResponseDisplayUserDetails;
                _displayQuestionNumbers.Checked = ApplicationManager.AppSettings.ResponseDisplayQuestionNumbers;
                _displayUnansweredQuestions.Checked = ApplicationManager.AppSettings.ResponseDisplayUnansweredQuestions;
                _displayRankOrderPoints.Checked = ApplicationManager.AppSettings.ResponseDisplayRankOrderPoints;
            }
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                ApplicationManager.AppSettings.ResponseDetailsDisplayIncompleteResponses = _displayIncompleteResponses.Checked;
                ApplicationManager.AppSettings.ResponseDetailsResultsPerPage = Int32.Parse(_resultsPerPage.Text.Trim());

                ApplicationManager.AppSettings.ResponseDisplayDetails = _displayResponseDetails.Checked;
                ApplicationManager.AppSettings.ResponseDisplayUserDetails = _displayDetailedUserInfo.Checked;
                ApplicationManager.AppSettings.ResponseDisplayQuestionNumbers = _displayQuestionNumbers.Checked;
                ApplicationManager.AppSettings.ResponseDisplayUnansweredQuestions = _displayUnansweredQuestions.Checked;
                ApplicationManager.AppSettings.ResponseDisplayRankOrderPoints = _displayRankOrderPoints.Checked;

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }

        private bool ValidateInputs()
        {
            _resultsPerPageRequiredValidator.Visible = false;
            _resultsPerPageFormatValidator.Visible = false;

            bool isValid = true;
            int value;

            if (Utilities.IsNullOrEmpty(_resultsPerPage.Text))
            {
                _resultsPerPageRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_resultsPerPage.Text.Trim(), out value))
            {
                _resultsPerPageFormatValidator.Visible = true;
                isValid = false;
            }

            return isValid;
        }
    }
}
