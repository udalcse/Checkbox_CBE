using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Validation;
using Checkbox.Security.Principal;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Users;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Panels;

namespace CheckboxWeb.Users.EmailLists
{
    public partial class Add : SecuredPage
    {
        private string _exitRedirectDestination;
        private List<string> _invalidAddresses;
        private const char DELIMITER = ',';

        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                AddressesToLoad = new List<string>();
                _invalidAddresses = new List<string>();
            }
            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _addEmailListWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/users/emailLists/add.aspx/wizardStepTitle/{0}", step.ID));
            }

            Master.SetTitle(WebTextManager.GetText("/pageText/users/emailLists/add.aspx/title"));
            _exitRedirectDestination = ResolveUrl("~/Users/EmailLists/Addresses.aspx");

            //Validators initialization
            _emailListNameRequired.ErrorMessage =
                WebTextManager.GetText("/pageText/users/emailLists/add.aspx/listNameRequired");
            _listNameLength.ErrorMessage =
                WebTextManager.GetText("/pageText/users/emaillists/add.aspx/emailListNameLength");
            _emailListExistValidator.ErrorMessage =
                WebTextManager.GetText("/pageText/users/emailLists/add.aspx/listNameExists");
            _descriptionLength.ErrorMessage = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/descriptionLength");
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

        #region Properties

        /// <summary>
        /// Get the page required role permission
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "EmailList.Edit"; }
        }

        #endregion

        #region Wizard event handlers

        /// <summary>
        /// Handles the next button click of the wizard
        /// - validates inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddEmailListWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            string cleanName = _emailListName.Text.Trim();
            string cleanDescription = _emailListDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            if (String.Compare(_addEmailListWizard.WizardSteps[e.CurrentStepIndex].ID, "ListNameStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                //Set up the review screen
                _emailListNameReview.Text = cleanName;
                _emailListDescriptionReview.Text = cleanDescription;
            }
            _fileUploadError.Visible = false;
            _textEntryError.Visible = false;
            //Uploaded data validation
            if (String.Compare(_addEmailListWizard.WizardSteps[e.CurrentStepIndex].ID, "AddAddresesStep", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AddressesToLoad.Clear();
                InvalidAddresses.Clear();

                if (_userSourceList.SelectedValue == "Upload")
                {
                    if (string.IsNullOrEmpty(_uploadedFilePathTxt.Text))
                    {
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
                        return;
                    }

                    //Import the data and store it
                    UploadTextEntry();
                }

                ConfigureReviewPanel();
            }
        }

        /// <summary>
        /// Handles the finish button click of the wizard
        /// - Creates the new email list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddEmailListWizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            EmailListPanel newPanel;

            try
            {
                newPanel = EmailListManager.CreateEmailList(_emailListName.Text, _emailListDescription.Text, UserManager.GetCurrentPrincipal());
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                _completionTitle.Text = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/errorTitle");
                _createEmailListError.Text = String.Format(WebTextManager.GetText("/pageText/users/emailLists/add.aspx/errorMessage"), err.Message);
                _createEmailListError.Visible = true;
                _exitRedirectDestination = ResolveUrl("~/Users/EmailLists/Manage.aspx");
                _closeButton.Text = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/closeButtonError");
                return;
            }

            if (newPanel != null)
            {
                Session["CurrentPanel"] = newPanel;
                if (AddressesToLoad.Count > 0)
                {
                    foreach (var adress in AddressesToLoad)
                    {
                        newPanel.AddPanelist(adress);
                    }
                    newPanel.Save(HttpContext.Current.User as CheckboxPrincipal);
                }
            }
            else
            {
                _completionTitle.Text = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/errorTitle");
                _createEmailListError.Text = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/errorMessageGeneric");
                _createEmailListError.Visible = true;
                _exitRedirectDestination = ResolveUrl("~/Users/EmailLists/Manage.aspx");
                _closeButton.Text = WebTextManager.GetText("/pageText/users/emailLists/add.aspx/closeButtonError");
            }
        }

        /// <summary>
        /// Handles the cancel button click of the wizard
        /// - returns the user to the email list manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddEmailListWizard_CancelButtonClick(object sender, EventArgs e)
        {
            if (Request["reloadOnCancel"]=="true")
            {
                //If at least one emailList was added, reload the emailList list.
                var panel = Session["CurrentPanel"] as EmailListPanel;
                var args = new Dictionary<string, string>
                               {
                                   {"page", "addEmailList"},
                                   {"newEmailListId", panel.ID.ToString()}
                               };

                Master.CloseDialog("reloadEmailListList", args);
            }
            else
            {
                Master.CloseDialog(null);
            }
        }

        #endregion

        #region Control event handlers

        protected void _editEmailListButton_OnClick(object sender, EventArgs e)
        {
            var panel = Session["CurrentPanel"] as EmailListPanel;
            var args = new Dictionary<string, string>
                           {
                               {"page", "addEmailList"},
                               {"newEmailListId", panel.ID.ToString()}
                           };

            Master.CloseDialog("onDialogClosed", args);
        }

        protected void _close_OnClick(object sender, EventArgs e)
        {
            //add null args to prevent js error
            var args = new Dictionary<string, string>
                           {
                               {"page", "addEmailList"},
                           };

            Master.CloseDialog("reloadEmailListList", args);

        }

        /// <summary>
        /// Check for an existing emailList name
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void _emailListExistValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string cleanName = _emailListName.Text.Trim();
            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
            }
            args.IsValid = !EmailListManager.IsDuplicateName(null, cleanName);
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
