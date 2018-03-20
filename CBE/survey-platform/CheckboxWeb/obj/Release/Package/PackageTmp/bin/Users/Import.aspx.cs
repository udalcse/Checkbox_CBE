using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Checkbox.Configuration;
using Checkbox.Forms.Validation;
using Checkbox.Web.Page;
using Checkbox.Web;
using System.Text;
using Checkbox.Common;
using System.IO;
using Checkbox.Security.Principal;
using Checkbox.Management;
using Prezza.Framework.Security;
using Checkbox.Users;
using Checkbox.Management.Licensing.Limits;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Import : SecuredPage
    {
        private List<string> _rowErrors;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            _roleSelector.Initialize(null, false);
            _groupSelector.Initialize(null, false);

            base.OnPageInit();

            if (!Page.IsPostBack && !Page.IsCallback)
            {
                UsersToLoad = new List<List<string>>();
            }

            //Set up the localized text for wizard navigation elements (can't use inline code in the wizard tag)
            foreach (WizardStep step in _importUserWizard.WizardSteps)
            {
                step.Title = WebTextManager.GetText(String.Format("/pageText/users/import.aspx/wizardStepTitle/{0}", step.ID));
            }

            Master.SetTitle(WebTextManager.GetText("/pageText/users/import.aspx/title"));

            //Set up the required field instructions and validation
            _layoutInstructions.Text = WebTextManager.GetText("/pageText/users/import.aspx/layoutInstructions");
            _fieldSelectionError.Text = WebTextManager.GetText("/pageText/users/import.aspx/userNameEmailError");

            Master.HideDialogButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Store values required for worker
            SurveyEditorLimit = GetSurveyEditorLimit();
            StartingSurveyEditorCount = ActiveLicense.SurveyEditorLimit != null
                                            ? ActiveLicense.SurveyEditorLimit.UsersInRolePermissionLimit.LongCount()
                                            : (long?)null;
            if (!Page.IsPostBack)
            {
                Session.Remove("TempGroupCache");
            }

            SelectedUserRoles = Page.IsPostBack ? _roleSelector.SelectedRoles : new List<string>();

            SelectedUserFields = Page.IsPostBack ? _fileColumnSelector.SelectedUserFields : new List<string>();

            SelectedUserGroups = Page.IsPostBack ? _groupSelector.SelectedGroupIDs : new List<int>();


            _fieldOrderExample.Text = String.Empty;

            for (int i = 0; i < SelectedUserFields.Count; i++)
            {
                if (i > 0)
                {
                    _fieldOrderExample.Text += ", ";
                }

                _fieldOrderExample.Text += SelectedUserFields[i];
            }

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        #region Review screen methods

        /// <summary>
        /// Set up the user review panel
        /// </summary>
        private void ConfigureUserReviewPanel()
        {
            //Set user counts
            _fileReviewSuccess.Text = String.Format(WebTextManager.GetText("/pageText/users/import.aspx/validRows"), UsersToLoad.Count.ToString());
            _fileReviewError.Text = String.Format(WebTextManager.GetText("/pageText/users/import.aspx/invalidRows"), RowErrors.Count.ToString());

            //Write errors
            _fileReviewError.Visible = RowErrors.Count > 0;

            _invalidUserList.Items.Clear();

            _invalidUserList.DataSource = RowErrors;
            _invalidUserList.DataBind();


            //Write success
            _validUserList.Visible = UsersToLoad.Count > 0;

            _fileReviewSuccess.Visible = UsersToLoad.Count > 0;

            _validUserList.Items.Clear();

            _validUserFieldList.DataSource = _fileColumnSelector.SelectedUserFields;
            _validUserFieldList.DataBind();
            _validUserList.DataSource = UsersToLoad;
            _validUserList.DataBind();
            
            _validUserPanel.Width = Math.Max(650, 200 * _fileColumnSelector.SelectedUserFields.Count + 50);
        }

        /// <summary>
        /// Sets up the final review screen
        /// </summary>
        private void ConfigureReviewPanel()
        {
            //User count
            _userCountReview.Text = String.Format(WebTextManager.GetText("/pageText/users/import.aspx/reviewUserCount"), UsersToLoad.Count);
            _importWaitWarningPanel.Visible = UsersToLoad.Count > 250;


            _rolesReviewList.DataSource = _roleSelector.SelectedRoles;
            _rolesReviewList.DataBind();

            _groupsReviewList.DataSource = _groupSelector.SelectedGroupIDs;
            _groupsReviewList.DataBind();

        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
            //Only return url if we are on finish step so that progress doesn't start until then.
            if (_importUserWizard.ActiveStep != null && _importUserWizard.ActiveStep.StepType == WizardStepType.Finish)
            {
                return ResolveUrl("~/Users/ImportWorker.aspx");
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_ImportUsers";
        }

        /// <summary>
        /// Get worker url params
        /// </summary>
        /// <returns></returns>
        protected override object GetWorkerUrlData()
        {
            return new { updateExisting = _updateUsers.Checked };
        }

        #region Wizard event handlers

        /// <summary>
        /// Handles the next button click of the wizard
        /// - Validates input at each step, saves intermediate data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImportUserWizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            _fieldSelectionError.Visible = false;
            _fileUploadError.Visible = false;
            _textEntryError.Visible = false;

            if (!Page.IsValid)
            {
                e.Cancel = true;
                return;
            }

            bool isValid = true;
            string step = _importUserWizard.WizardSteps[e.CurrentStepIndex].ID.ToLower();

            switch (step)
            {
                case "layoutstep":
                    isValid = LayoutStepValidator();
                    if (isValid && _fileColumnSelector.ShouldSaveFieldsConfiguration)
                        UserImportConfigManager.SaveUserImportConfigs(_fileColumnSelector.SelectedUserFields);
                    break;

                case "uploadstep":
                    isValid = UploadStepValidator();

                    if (isValid)
                    {
                        if (_userSourceList.SelectedValue == "Upload")
                            UploadUserFile();
                        else
                            UploadTextEntry();

                        ConfigureUserReviewPanel();
                    }
                    break;

                case "rolestep":
                    isValid = RoleStepValidator();
                    break;

                case "groupstep":
                    isValid = GroupStepValidator();

                    if (isValid)
                        ConfigureReviewPanel();

                    break;

                default:
                    break;
            }

            if (!isValid)
                e.Cancel = true;
        }

        /// <summary>
        /// Handles the cancel event of the wizard;
        /// - redirects back to the user manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ImportUserWizard_CancelButtonClick(object sender, EventArgs e)
        {

            if (_fileColumnSelector.ShouldSaveFieldsConfiguration)
            {
                //Save selected CSV file mapping for user import
                UserImportConfigManager.SaveUserImportConfigs(SelectedUserFields);
            }

            //Clear out temporary session values
            Session["ImportUsersSelectedUserFields"] = null;
            if (Request["reloadOnCancel"] == "true")
            {
                var args = new Dictionary<string, string> { { "page", "importUsers" } };

                Master.CloseDialog("reloadUserList", args);
            }
            else
                Master.CloseDialog(null);
        }

        #endregion

        /// <summary>
        /// Determines if it is alright to move to the next step. If it is not a specific error message is displayed.
        /// </summary>
        /// <returns><see langword="True"/> if it is alright to move to the next step. Otherwise <see langword="false"/> is returned.</returns>
        private bool LayoutStepValidator()
        {
            bool isValid = true;

            if (!(_fileColumnSelector.SelectedUserFields.Contains(WebTextManager.GetText("/pageText/users/import.aspx/username"))
                || _fileColumnSelector.SelectedUserFields.Contains(WebTextManager.GetText("/pageText/users/import.aspx/email"))))
            {
                _fieldSelectionError.Visible = true;
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Determines if it is alright to move to the next step. If it is not a specific error message is displayed.
        /// </summary>
        /// <returns><see langword="True"/> if it is alright to move to the next step. Otherwise <see langword="false"/> is returned.</returns>
        private bool UploadStepValidator()
        {
            bool isValid = true;

            if (_userSourceList.SelectedValue == "Upload")
            {
                if (string.IsNullOrEmpty(_uploadedFilePathTxt.Text))
                {
                    _fileUploadError.Text = WebTextManager.GetText("/pageText/users/import.aspx/fileUploadMissingFile");
                    _fileUploadError.Visible = true;
                    isValid = false;
                }
                else
                {
                    //Validate file name
                    if (!Utilities.ValidateCsvFileName(_uploadedFileNameTxt.Text))
                    {
                        _fileUploadError.Text = WebTextManager.GetText("/pageText/users/import.aspx/fileUploadInvalidFileType");
                        _fileUploadError.Visible = true;
                        isValid = false;
                    }
                }
            }
            else
            {
                if (String.IsNullOrEmpty(_importTxt.Text.Trim()))
                {
                    _textEntryError.Text = WebTextManager.GetText("/pageText/users/import.aspx/textEntryError");
                    _textEntryError.Visible = true;
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Determines if it is alright to move to the next step. If it is not a specific error message is displayed.
        /// </summary>
        /// <returns><see langword="True"/> if it is alright to move to the next step. Otherwise <see langword="false"/> is returned.</returns>
        private bool RoleStepValidator()
        {
            bool isValid = true;

            if (_roleSelector.SelectedRoles.Count == 0)
            {
                _roleRequiredError.Visible = true;
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Determines if it is alright to move to the next step. If it is not a specific error message is displayed.
        /// </summary>
        /// <returns><see langword="True"/> if it is alright to move to the next step. Otherwise <see langword="false"/> is returned.</returns>
        private bool GroupStepValidator()
        {
            bool isValid = true;

            var currentUser = HttpContext.Current.User as CheckboxPrincipal;

            if (!currentUser.IsInRole("System Administrator") && !ApplicationManager.UseSimpleSecurity)
            {
                if (_groupSelector.SelectedGroupIDs.Count <= 0
                    && !AuthorizationFactory.GetAuthorizationProvider().Authorize(currentUser, GroupManager.GetEveryoneGroup(), "Group.ManageUsers"))
                {
                    _groupRequiredError.Visible = true;
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearSelectedUserFields()
        {
            _fileColumnSelector.ClearSelectedUserFields();
        }

        #region Control event handlers

        /// <summary>
        /// Handles the selected index changed event of the import source radio button list
        /// - Toggles the display of the import dialog panels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_userSourceList.SelectedValue == "Upload")
            {
                _fileUploadPanel.Visible = true;
                _textEntryPanel.Visible = false;
                _descriptionLabel.Text = WebTextManager.GetText("/pageText/users/import.aspx/csvText/explanation");
            }
            else
            {
                _fileUploadPanel.Visible = false;
                _textEntryPanel.Visible = true;
                _descriptionLabel.Text = WebTextManager.GetText("/pageText/users/import.aspx/textEntry/explanation");
            }
        }

        /// <summary>
        /// Hansles the item created event of the group review list
        /// - Sets the group name based on the selected group id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GroupsReviewList_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem && ((ListViewDataItem)e.Item).DataItem != null)
            {
                ((Label)e.Item.FindControl("_groupReviewLabel")).Text = GroupManager.GetGroup(Convert.ToInt32(((ListViewDataItem)e.Item).DataItem)).Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RestartButton_OnClick(object sender, EventArgs e)
        {
            ClearSelectedUserFields();
            Response.Redirect(ResolveUrl("~/Users/Import.aspx?reloadOnCancel=true"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _closeWizardButton_Click(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string> { { "page", "importUsers" } };

            Master.CloseDialog("reloadUserList", args);
        }

        #endregion

        #region File upload methods

        /// <summary>
        /// Upload the users.
        /// </summary>
        private void UploadUserFile()
        {
            //Clear values
            RowErrors.Clear();
            UsersToLoad.Clear();

            //Get file data
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return;
            }

            //Attempt to read & parse file as xml
            var ms = new MemoryStream(fileBytes);
            var sr = new StreamReader(ms, Encoding.GetEncoding(_uploadFileEncoding.SelectedValue));

            try
            {
                ReadStream(sr, true);
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// Load the users.
        /// </summary>
        private void UploadTextEntry()
        {
            //Clear values
            RowErrors.Clear();
            UsersToLoad.Clear();

            //Create streams for reading/writing data
            var ms = new MemoryStream();
            var sr = new StreamReader(ms, Encoding.GetEncoding(_uploadFileEncoding.SelectedValue));
            var sw = new StreamWriter(ms);

            //Write loaded text to stream
            sw.Write(_importTxt.Text);

            //Flush stream and read
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);


            try
            {
                //Now parse out values
                ReadStream(sr, false);
            }
            finally
            {
                sw.Close();
                sr.Close();
            }
        }

        /// <summary>
        /// Read data from the stream
        /// </summary>
        /// <param name="reader"></param>
        private void ReadStream(TextReader reader, bool isUploadedFile)
        {
            //Create and initialize a CSV reader class
            var utils = new CsvUtilities();
            utils.Initialize(reader, false, ',', '"', '"', '#', true, false);

            //Find the index of the user name field
            int userNameIndex = _fileColumnSelector.SelectedUserFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/username"));
            int emailIndex = _fileColumnSelector.SelectedUserFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/email"));
            int passwordIndex = _fileColumnSelector.SelectedUserFields.IndexOf(WebTextManager.GetText("/pageText/users/import.aspx/password"));

            userNameIndex = userNameIndex >= 0
                ? userNameIndex
                : emailIndex;

            int lineCount = 0;

            Encoding encoding = null;
            int secondValue = 0;

            if (isUploadedFile)
            {
                encoding = Encoding.GetEncoding(_uploadFileEncoding.SelectedValue);
            }

            while (utils.ReadNextRecord())
            {
                lineCount++;
                ValidateRecord(utils.CurrentRecord, utils.CurrentRawData, lineCount, userNameIndex, emailIndex, passwordIndex);

                if (isUploadedFile)
                {
                    secondValue += encoding.GetBytes(utils.CurrentRawData).Length;

                    if (!Response.IsClientConnected)
                    {
                        //Cancel button was clicked or the browser was closed, so stop processing
                        break;
                    }
                }
            }
        }

        EmailValidator _EmailValidator;
        /// <summary>
        /// Validator
        /// </summary>
        EmailValidator EmailValidator
        {
            get
            {
                if (_EmailValidator == null)
                    _EmailValidator = new EmailValidator();
                return _EmailValidator;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordValues"></param>
        /// <param name="lineText"></param>
        /// <param name="lineNumber"></param>
        /// <param name="userNameIndex"></param>
        /// <param name="emailIndex"></param>
        /// <param name="passwordIndex"></param>
        private void ValidateRecord(List<string> recordValues, string lineText, int lineNumber, int userNameIndex, int emailIndex, int passwordIndex)
        {
            if (recordValues.Count == 0)
            {
                return;

            }
            //Check for correct number of records or null records
            if (recordValues.Count != _fileColumnSelector.SelectedUserFields.Count
                || recordValues.Where(val => val == null).Count() != 0)
            {
                RowErrors.Add(string.Format("{0} {1}: \"{2}\" {3} {4} {5} {6}",
                                                    WebTextManager.GetText("/pageText/users/import.aspx/row"),
                                                    lineNumber,
                                                    Utilities.TruncateText(lineText, 32),
                                                    WebTextManager.GetText("/pageText/users/import.aspx/has"),
                                                    recordValues.Count,
                                                    WebTextManager.GetText("/pageText/users/import.aspx/columnsExpecting"),
                                                    _fileColumnSelector.SelectedUserFields.Count));
                return;
            }

            //Validate user name
            string userName = recordValues[userNameIndex];
            string email = string.Empty;
            string password = string.Empty;

            if (emailIndex >= 0)
                email = recordValues[emailIndex];

            if (passwordIndex >= 0)
                password = recordValues[passwordIndex];

            //Make sure name isn't empty
            if (Utilities.IsNullOrEmpty(userName))
            {
                RowErrors.Add(string.Format("{0} {1} {2}",
                    WebTextManager.GetText("/pageText/users/import.aspx/row"),
                    lineNumber,
                    WebTextManager.GetText("/pageText/users/import.aspx/emptyUserName")));

                return;
            }

            //Check for restricted letters
            const string userNameRegex = @"^[\w\s@.'-]+$";
            const string AD_userNameRegex = @"^[\w\s@.'-\\]+$";

            Regex regex = new Regex(StaticConfiguration.DisableForeighMembershipProviders ? userNameRegex : AD_userNameRegex);
            if (!regex.IsMatch(userName) || userName.Length > 255 || userName.Length < 1)
            {
                RowErrors.Add(string.Format("{0} {1} {2}",
                    WebTextManager.GetText("/pageText/users/import.aspx/row"),
                    lineNumber,
                    WebTextManager.GetText("/pageText/users/import.aspx/usernameLength")));

                return;
            }

            //Check that user name is a valid e-mail
            if (userName.Contains('@'))
            {
                if (!EmailValidator.Validate(userName))
                {
                    RowErrors.Add(string.Format("{0} {1} {2}",
                            WebTextManager.GetText("/pageText/users/import.aspx/row"),
                            lineNumber,
                            WebTextManager.GetText("/pageText/users/properties.aspx/userNameEmailRulesViolated")));
                    return;
                }
            }

            //Make sure email is valid
            if (Utilities.IsNotNullOrEmpty(email) && ApplicationManager.AppSettings.EnableEmailAddressValidation)
            {
                if (!EmailValidator.Validate(email))
                {
                    RowErrors.Add(string.Format("{0} {1} {2}",
                                                WebTextManager.GetText("/pageText/users/import.aspx/row"),
                                                lineNumber,
                                                WebTextManager.GetText("/pageText/users/import.aspx/invalidEmail")));

                    return;
                }
            }

            //Make sure password is valid in case of EnforcePasswordLimitsGlobally set to "true".
            if (ApplicationManager.AppSettings.EnforcePasswordLimitsGlobally)
            {
                PasswordValidator passwordValidator = new PasswordValidator();

                if (!passwordValidator.Validate(password))
                {
                    RowErrors.Add(string.Format("{0} {1} {2}",
                            WebTextManager.GetText("/pageText/users/import.aspx/row"),
                            lineNumber,
                            passwordValidator.GetMessage(WebTextManager.GetUserLanguage())));

                    return;
                }
            }

            //Check to see if another user with the same name has already been added.
            if (UsersToLoad.Find(propList => propList.Count > userNameIndex && userName.Equals(propList[userNameIndex], StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                RowErrors.Add(string.Format("{0} {1} {2}",
                   WebTextManager.GetText("/pageText/users/import.aspx/row"),
                   lineNumber,
                   WebTextManager.GetText("/pageText/users/import.aspx/duplicateUserName")));

                return;
            }

            //Check for funky values in user name
            if (!UserManager.ValidateUniqueIdentifierFormat(userName))
            {
                RowErrors.Add(string.Format(
                        "Row {0}: {1} -- {2}",
                        lineNumber,
                        recordValues[userNameIndex],
                        WebTextManager.GetText("/pageText/users/import.aspx/invalidCharactersFound")));
                return;
            }

            //If we get here, name checks out
            UsersToLoad.Add(recordValues);
        }

        #endregion

        #region User creation methods


        /// <summary>
        /// Get the number of allowed survey editors
        /// </summary>
        private long? GetSurveyEditorLimit()
        {
            SurveyEditorLimit limit = ActiveLicense.SurveyEditorLimit;

            if (limit == null)
                return null;

            return limit.RuntimeLimitValue;
        }



        #endregion

        #region Properties



        /// <summary>
        /// Get list of import row errors
        /// </summary>
        private List<string> RowErrors
        {
            get { return _rowErrors ?? (_rowErrors = new List<string>()); }
        }


        /// <summary>
        /// Get list of import row errors
        /// </summary>
        protected List<List<string>> UsersToLoad
        {
            get { return Session["ImportUsersUsersToLoad"] as List<List<string>> ?? new List<List<string>>(); }
            set { Session["ImportUsersUsersToLoad"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> SelectedUserFields
        {
            get { return Session["SelectedUserFields"] as List<string> ?? new List<string>(); }
            set { Session["SelectedUserFields"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<string> SelectedUserRoles
        {
            get { return Session["SelectedUserRoles"] as List<string> ?? new List<string>(); }
            set { Session["SelectedUserRoles"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<int> SelectedUserGroups
        {
            get { return Session["SelectedUserGroups"] as List<int> ?? new List<int>(); }
            set { Session["SelectedUserGroups"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected long? SurveyEditorLimit
        {
            get { return Session["SurveyEditorLimit"] as long?; }
            set { Session["SurveyEditorLimit"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected long? StartingSurveyEditorCount
        {
            get { return Session["StartingSurveyEditorCount"] as long?; }
            set { Session["StartingSurveyEditorCount"] = value; }
        }


        /// <summary>
        /// Get the required role permissions for the page
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Group.ManageUsers"; }
        }

        #endregion
    }
}
