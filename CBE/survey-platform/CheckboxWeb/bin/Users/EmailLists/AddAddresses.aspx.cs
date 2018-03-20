using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Web;
using System.Text;
using Checkbox.Common;
using System.IO;
using Checkbox.Forms.Validation;

namespace CheckboxWeb.Users.EmailLists
{
    public partial class AddAddresses : EmailListEditorPage
    {
        private List<string> _invalidAddresses;
        private const char DELIMITER = ',';

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                AddressesToLoad = new List<string>();
            }

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _addAddressesWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/users/emailLists/addAddresses.aspx/wizardStepTitle/{0}", step.ID));
            }             

            Master.SetTitle(String.Format(WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/title"), EmailList.Name));
            Master.HideDialogButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (_userSourceList.SelectedValue == "Upload")
            {
                _textPanel.Visible = false;
                _filePanel.Visible = true;
            }
            else
            {
                _textPanel.Visible = true;
                _filePanel.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
            //Only return url if we are on finish step so that progress doesn't start until then.
            if (_addAddressesWizard.ActiveStep != null && _addAddressesWizard.ActiveStep.StepType == WizardStepType.Finish)
            {
                return ResolveUrl("~/Users/EmailLists/ImportWorker.aspx");
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object GetWorkerUrlData()
        {
            return new {p = EmailList.ID.Value};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_AddAddresses";
        }

        #region Control event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RestartButton_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Users/EmailLists/AddAddresses.aspx") + "?p=" + EmailList.ID.ToString());
        }


        #endregion

        #region Wizard event handlers

        /// <summary>
        /// Handles the next button click of the wizard
        /// - Validates input at each step, saves intermediate data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddAddressesWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            _fileUploadError.Visible = false;
            _textEntryError.Visible = false;
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }
            
            //Uploaded data validation
            if (String.Compare(_addAddressesWizard.WizardSteps[e.CurrentStepIndex].ID, "DataSourceStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AddressesToLoad.Clear();
                InvalidAddresses.Clear();

                if (_userSourceList.SelectedValue == "Upload")
                {
                    if(string.IsNullOrEmpty(_uploadedFilePathTxt.Text))
                    {
                        _fileUploadError.Text = WebTextManager.GetText("/pageText/users/import.aspx/fileUploadMissingFile");
                        _fileUploadError.Visible = true;
                        e.Cancel = true;
                        return;
                    }
                    
                    if (!Utilities.ValidateCsvFileName(_uploadedFileNameTxt.Text))
                    {
                        _fileUploadError.Text = WebTextManager.GetText("/pageText/users/import.aspx/fileUploadInvalidFileType");
                        _fileUploadError.Visible = true;
                        e.Cancel = true;
                        return;
                    }

                    //Now upload the data and store it
                    ProcessFile();
                }
                else
                {
                    if (String.IsNullOrEmpty(_importTxt.Text.Trim()))
                    {
                        _textEntryError.Text = WebTextManager.GetText("/pageText/users/import.aspx/textEntryError");
                        _textEntryError.Visible = true;
                        e.Cancel = true;
                        return;
                    }

                    //Import the data and store it
                    UploadTextEntry();
                }

                ConfigureReviewPanel();
            }
        }

        /// <summary>
        /// Handles the cancel event of the wizard;
        /// - redirects back to the address manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddAddressesWizard_CancelButtonClick(object sender, EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Users/EmailLists/Addresses.aspx") + "?p=" + EmailList.ID.ToString());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get list of invalid email addresses
        /// </summary>
         private List<string> InvalidAddresses
        {
            get { return _invalidAddresses ?? (_invalidAddresses = new List<string>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> AddressesToLoad
        {
            get { return GetSessionValue("EmailListAddressesToLoad", new List<string>()); }
            set { Session["EmailListAddressesToLoad"] = value; }
        }

        #endregion

        #region File upload methods

        /// <summary>
        /// 
        /// </summary>
        private void ProcessFile()
        {
            //Get file data
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return;
            }

            //Attempt to read & parse file as xml
            var ms = new MemoryStream(fileBytes);
            var sr = new StreamReader(ms, Encoding.GetEncoding(_uploadFileEncoding.SelectedValue));


            var fileText = FileUtilities.ReadTextStream(sr);
            Dictionary<string, string> addresses = ParseUploadedAddresses(fileText);

            var validator = new EmailValidator();
            foreach (string address in addresses.Keys)
            {
                if (validator.Validate(address))
                {
                    AddressesToLoad.Add(address);
                }
                else
                {
                    InvalidAddresses.Add(address);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UploadTextEntry()
        {
            var panelists = new List<string>(_importTxt.Text.Split(new[] { Environment.NewLine },
                                                                               StringSplitOptions.RemoveEmptyEntries));

            var validator = new EmailValidator();
            foreach (string address in panelists)
            {
                if (validator.Validate(address))
                {
                    AddressesToLoad.Add(address);
                }
                else
                {
                    InvalidAddresses.Add(address);
                }
            }
        }

        /// <summary>
        /// Given a list of new line or comma delimited email addresses builds a dictionary of
        /// unique addresses.
        /// </summary>
        /// <param name="rows">The rows of imported text to process.</param>
        /// <returns>A dictionary of unique email addresses.<a</returns>
        private Dictionary<string, string> ParseUploadedAddresses(IEnumerable<string> rows)
        {
            //A dictionary is used to enforce uniqueness.
            //On large datasets there is a noticeable performance gain.
            var addresses = new Dictionary<string, string>();

            foreach (string emailAddress in rows)
            {
                //check for multiple addresses on one line
                var record = new List<string>(emailAddress.Split(DELIMITER));

                if (record.Count > 1)
                {
                    foreach (string address in record)
                    {
                        if (!addresses.ContainsKey(address.ToLower().Trim()))
                        {
                            addresses.Add(address.ToLower().Trim(), null);
                        }
                    }
                }
                else
                {
                    if (!addresses.ContainsKey(emailAddress.ToLower().Trim()))
                    {
                        addresses.Add(emailAddress.ToLower().Trim(), null);
                    }
                }
            }

            return addresses;
        }

        #endregion

        /// <summary>
        /// Set up the user review panel
        /// </summary>
        private void ConfigureReviewPanel()
        {
            //Set user counts
            _fileReviewSuccess.Text = String.Format(WebTextManager.GetText("/pageText/users/import.aspx/validRows"), AddressesToLoad.Count.ToString());
            _fileReviewError.Text = String.Format(WebTextManager.GetText("/pageText/users/import.aspx/invalidRows"), InvalidAddresses.Count.ToString());

            //Write errors
            _invalidUserList.Visible = InvalidAddresses.Count > 0;
            _fileReviewError.Visible = InvalidAddresses.Count > 0;

            _invalidUserList.Items.Clear();

            if (InvalidAddresses.Count > 0)
            {
                _invalidUserList.DataSource = InvalidAddresses;
                _invalidUserList.DataBind();
            }

            //Write success
            _validUserList.Visible = AddressesToLoad.Count > 0;

            _fileReviewSuccess.Visible = AddressesToLoad.Count > 0;

            _validUserList.Items.Clear();

            if (AddressesToLoad.Count > 0)
            {
                _validUserList.DataSource = AddressesToLoad;
                _validUserList.DataBind();
            }
        }

    }
}
