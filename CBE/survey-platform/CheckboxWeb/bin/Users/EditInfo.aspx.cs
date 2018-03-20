using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Users;
using Checkbox.Management;
using Checkbox.Security;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Common;
using Checkbox.Security.Principal;
using System.Web;
using Checkbox.Forms.Validation;
using CheckboxWeb.Services;
using CheckboxWeb.Users.Controls;
using Prezza.Framework.Caching;

namespace CheckboxWeb.Users
{
    public partial class EditInfo : SecuredPage, IStatusPage
    {
        private Dictionary<string, Control> _propertyFields;
        private Dictionary<string, Control> _multiLinePropertyFields;
        private Dictionary<string, Control> _radioPropertyFields;
        private Dictionary<string, Control> _matrixTables;

        private List<ProfileProperty> properties;

        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Do nothing of self-edit not enabled
            if (!ApplicationManager.AppSettings.AllowEditSelf)
            {
                Response.Redirect(UserDefaultRedirectUrl, false);
                return;
            }

            _propertyFields = new Dictionary<string, Control>();
            _multiLinePropertyFields = new Dictionary<string, Control>();
            _radioPropertyFields = new Dictionary<string, Control>();
            _matrixTables = new Dictionary<string, Control>();

            Master.SetTitle(WebTextManager.GetText("/pageText/users/editInfo.aspx/title") + " - " + Context.User.Identity.Name);

            _passwordPanel.Visible = true;

            if (UserManager.EXTERNAL_USER_AUTHENTICATION_TYPE.Equals(Context.User.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase) ||
                UserManager.NETWORK_USER_AUTHENTICATION_TYPE.Equals(Context.User.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase))
                _passwordPanel.Visible = false;

            _profileChangeButton.OnClientClick = "return validateMatrices()";
            //Bind the profile property repeater
            BindProfileProperties();
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            RegisterClientScriptInclude("jQueryValidate", ResolveUrl("~/Resources/jquery.validate.min.js"));
        }

        /// <summary>
        /// Binds the profile property repeater to only those profile properties that are self-editable
        /// </summary>
        private void BindProfileProperties()
        {
            //Bind the email address separately
            _email.Text = UserManager.GetUserEmail(Context.User.Identity.Name);
            var userId = AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid;
            properties =
                ProfileManager.GetProfileProperties(Context.User.Identity.Name, userId : userId)
                    .Where(
                        item =>
                            !item.Name.Equals("Email", StringComparison.InvariantCultureIgnoreCase) && !item.IsHidden)
                    .ToList();


            //Bind the remaining custom properties
            var singleLineFields =
                properties.Where(prop => prop.FieldType == CustomFieldType.SingleLine || prop.FieldType == CustomFieldType.Email).Select(item => item.Name);

            var multiLineFields =
                properties.Where(prop => prop.FieldType == CustomFieldType.MultiLine).Select(item => item.Name);

            var radioFields =
                properties.Where(prop => prop.FieldType == CustomFieldType.RadioButton).Select(item => item.Name);

            var matrixFields =
              properties.Where(prop => prop.FieldType == CustomFieldType.Matrix).Select(item => item.Name);

            _profileList.DataSource = singleLineFields;
            _profileMultiLineList.DataSource = multiLineFields;
            _profileRadioList.DataSource = radioFields;
            _profileMatrixList.DataSource = matrixFields;

            _profileList.DataBind();
            _profileMultiLineList.DataBind();
            _profileRadioList.DataBind();
            _profileMatrixList.DataBind();
        }


        private Dictionary<string, string> AllAccountProperties
        {
            get
            {
                return ProfileProperties.Concat(MultiLineProfileProperties).Concat(RadioProfileProperties).Concat(MatrixTables)
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private Dictionary<string, string> ProfileProperties
        {
            get
            {
                Dictionary<string, string> output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _propertyFields.Keys)
                {
                    if (_propertyFields.ContainsKey(profilePropertyName))
                    {
                        TextBox propertyField = _propertyFields[profilePropertyName] as TextBox;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, propertyField.Text.Trim());
                        }
                    }
                }

                return output;
            }
        }


        private Dictionary<string, string> MultiLineProfileProperties
        {
            get
            {
                Dictionary<string, string> output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _multiLinePropertyFields.Keys)
                {
                    if (_multiLinePropertyFields.ContainsKey(profilePropertyName))
                    {
                        TextBox propertyField = _multiLinePropertyFields[profilePropertyName] as TextBox;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, propertyField.Text.Trim());
                        }
                    }
                }

                return output;
            }
        }

        private Dictionary<string, string> RadioProfileProperties
        {
            get
            {
                Dictionary<string, string> output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _radioPropertyFields.Keys)
                {
                    if (_radioPropertyFields.ContainsKey(profilePropertyName))
                    {
                        Panel propertyField = _radioPropertyFields[profilePropertyName] as Panel;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, string.Empty);
                        }
                    }
                }

                return output;
            }
        }


        private Dictionary<string, string> MatrixTables
        {
            get
            {
                Dictionary<string, string> output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _matrixTables.Keys)
                {
                    if (_matrixTables.ContainsKey(profilePropertyName))
                    {
                        Panel propertyField = _matrixTables[profilePropertyName] as Panel;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, string.Empty);
                        }
                    }
                }

                return output;
            }
        }


        #region Event handlers

        protected void ChangePasswordButtton_Click(object sender, EventArgs e)
        {
            //First validate that the current password is correct
            if (UserManager.AuthenticateUser(Context.User.Identity.Name, _password.Text) == null)
            {
                ShowStatusMessage(WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordFail"), StatusMessageType.Error);
            }
            else
            {
                var status = string.Empty;
                try
                {
                    if (ApplicationManager.AppSettings.EnforcePasswordLimitsGlobally)
                    {
                        var passwordValidator = new PasswordValidator();

                        if (!passwordValidator.Validate(_newPassword.Text.Trim()))
                        {
                            ShowStatusMessage(String.Format(passwordValidator.GetMessage("en-US"), status), StatusMessageType.Error);
                            return;
                        }
                    }

                    if (UserManager.UpdateUser(Context.User.Identity.Name, null, null, _newPassword.Text.Trim(), null, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status) != null)
                    {
                        ShowStatusMessage(WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordSuccess"), StatusMessageType.Success);
                    }
                    else
                    {
                        ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordFailError"), status) , StatusMessageType.Error);
                    }
                }
                catch (Exception err)
                {
                    ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/editInfo.aspx/passwordFailError"), err.Message), StatusMessageType.Error);
                }
            }
        }

        protected void EditProfileButtton_Click(object sender, EventArgs e)
        {
            try
            {
                if (AllAccountProperties != null && AllAccountProperties.Any())
                {
                    if (this.MatrixTables.Any())
                        StoreMatrices();

                    if (this.RadioProfileProperties.Any())
                        StoreRadioButtons();

                    if (!ValidateEmailFields())
                        return;

                    ProfileManager.StoreProfile(Context.User.Identity.Name, AllAccountProperties);

                    //Also update the user principal so that the email address is stored as well
                    if (Utilities.IsNotNullOrEmpty(_email.Text.Trim()))
                    {
                        //First validate the email format
                        _emailFormatInvalidLabel.Visible = false;

                        if (!ValidateEmail(_email.Text.Trim(), true))
                        {
                            _emailFormatInvalidLabel.Visible = true;
                            return;
                        }

                        string status = string.Empty;
                        if (UserManager.UpdateUser(Context.User.Identity.Name, null, null, null, _email.Text.Trim(),
                            ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status) == null)
                        {
                            ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/editInfo.aspx/profileFailError"), status), StatusMessageType.Error);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/editInfo.aspx/profileFailError"), err.Message), StatusMessageType.Error);
                return;
            }

            ShowStatusMessage(WebTextManager.GetText("/pageText/users/editInfo.aspx/profileSuccess"), StatusMessageType.Success);
        }

        public void StoreRadioButtons()
        {
            foreach(var item in _radioPropertyFields)
            {
                var radioButtonControl = (RadioButtonList)_radioPropertyFields[item.Key].Controls[0];
                var selectedItem = radioButtonControl.SelectedItem;

                RadioButtonField radioButtonField = new RadioButtonField();
                radioButtonField.Name = item.Key;
                radioButtonField.Options = new List<RadioButtonFieldOption>();
                foreach(ListItem listItem in radioButtonControl.Items)
                {
                    RadioButtonFieldOption option = new RadioButtonFieldOption();
                    option.Name = listItem.Value;
                    option.IsSelected = listItem.Selected;
                    radioButtonField.Options.Add(option);
                }
                var selectedOption = radioButtonField.Options.FirstOrDefault(o => o.IsSelected);
                if (selectedOption != null)
                {
                    ProfileManager.UpdateRadioFieldSelectedOption(selectedOption.Name,
                        radioButtonField.Name, AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid);
                }
            }
        }

        public void StoreMatrices()
        {
           
            foreach (var item in this.MatrixTables)
            {
                var matrixControl = (MatrixGrid) _matrixTables[item.Key].Controls[0];
                var matrixCellValues =
                    Request.Form.AllKeys.Where(key => key.Contains("_matrixField_" + item.Key + "$") || key.Contains(item.Key + "_RowHeader") || key.Contains(item.Key + "_ColumnHeader")).ToList();

                matrixCellValues.RemoveAll(cell => cell.Contains("rowsCount") || cell.Contains("columnsCount"));
                var userId = AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid;
                var matrix = ProfileManager.GetMatrixField(item.Key, userId);
              

                var tableRowsCountKey = Request.Form.AllKeys.FirstOrDefault(key => key.EndsWith("_rowsCountTxt" + item.Key));
                var tableColumnCountKey = Request.Form.AllKeys.FirstOrDefault(key => key.EndsWith("_columnsCountTxt" + item.Key));

                int columns = int.Parse(Request.Form[tableColumnCountKey]);
                var rows = int.Parse(Request.Form[tableRowsCountKey]);

                matrix.RemoveDataCells();

                if (matrix.HasFixedStructure)
                {
                    var requestValues = matrixCellValues.Select(key => Request.Form[key]).ToList();
                    matrix.RemoveDataCells();
                    matrix.CreateDataCells(rows,columns, requestValues);
                }

                //if user is able to add custom rows 
                else if (!matrix.IsRowsFixed)
                {
                    var requestValues = matrixCellValues.Select(key => Request.Form[key]).ToList();

                    matrix.RemoveCustomStructure();

                    if (matrix.HasRowHeaders)
                    {
                        var rowsHeaders = matrixCellValues.Where(key => key.Contains(item.Key + "_RowHeader")).ToList();
                        var rowsHeaderValues = rowsHeaders.Select(key => Request.Form[key]).ToList();
                        matrixCellValues = matrixCellValues.Except(rowsHeaders).ToList();

                        matrix.CreateCustomRowHeaders(rowsHeaderValues);
                    }
                    else
                    {
                        var rowHeaderToInsert = requestValues.Count/columns - 1;
                        var emptyRows = new List<string>();

                        for (int i = 0; i < rowHeaderToInsert; i++)
                            emptyRows.Add(null);

                        if (rowHeaderToInsert >= 1)
                            matrix.CreateCustomRowHeaders(emptyRows);
                    }

                    
                    matrix.CreateDataCells(rows,columns, matrixCellValues.Select(key => Request.Form[key]).ToList());
                }

                //if user is able to add custom columns 
                else if (!matrix.IsColumnsFixed)
                {
                    var requestValues = matrixCellValues.Select(key => Request.Form[key]).ToList();
                    matrix.RemoveCustomStructure();

                    if (matrix.HasHeaders)
                    {
                        var columnHeaders =
                            matrixCellValues.Where(key => key.Contains(item.Key + "_ColumnHeader")).ToList();
                        var columnHeaderValues = columnHeaders.Select(key => Request.Form[key]).ToList();
                        matrixCellValues = matrixCellValues.Except(columnHeaders).ToList();

                        matrix.CreateCustomColumnHeaders(columnHeaderValues);
                    }
                    else
                    {
                        var headerToInsert = requestValues.Count / rows - 1;
                        var emptyHeaders = new List<string>();

                        for (int i = 0; i < headerToInsert; i++)
                            emptyHeaders.Add(null);

                        if (headerToInsert >= 1)
                            matrix.CreateCustomColumnHeaders(emptyHeaders);
                    }

                  
                    matrix.CreateDataCells(rows, columns, matrixCellValues.Select(key => Request.Form[key]).ToList());
                }

                ProfileManager.AddCustomMatrixCells(matrix, userId );

                matrixControl.Initialize(matrix);
                matrixControl.BindMatrix();
            }
        }

        /// <summary>
        /// Handles the item data bound event of the profile repeater; sets the control ID
        /// of the fields and stores the keys in a dictionary so that the data can be loaded 
        /// for an existing user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var emailFields = properties.Where(p => p.FieldType == CustomFieldType.Email).ToList();
            var controlToAdd = e.Item.FindControl("_propertyValue") as TextBox;

            if (controlToAdd != null && emailFields.Any(email => email.Name == ((ListViewDataItem)e.Item).DataItem.ToString()))
            {
                controlToAdd.Attributes["type"] = "email";
                controlToAdd.Attributes["name"] = "email";
            }

            _propertyFields.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_propertyValue"));
        }

        protected void ProfileMultiLIneList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _multiLinePropertyFields.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_multiLinePropertyValue"));
        }

        protected void ProfileRadioList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _radioPropertyFields.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_radioButtonPanel"));
        }

        protected void ProfileMatrixList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _matrixTables.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_matrixGridPanel"));
        }

        /// <summary>
        /// Handles the data bound event; loads user data into the profile list after the 
        /// list has been assembled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileList_DataBound(object sender, EventArgs e)
        {
            var profileProperties =
                properties
                    .Where(
                        item =>
                            item.FieldType == CustomFieldType.SingleLine || item.FieldType == CustomFieldType.Email);
            ;

            foreach (var item in profileProperties)
            {
                TextBox propertyField = _propertyFields[item.Name] as TextBox;

                if (propertyField != null)
                    propertyField.Text = item.Value;
            }
        }


        protected void ProfileMultiLIneList_DataBound(object sender, EventArgs e)
        {
            var profileProperties = properties.Where(item => item.FieldType == CustomFieldType.MultiLine);

            foreach (var item in profileProperties)
            {
                TextBox propertyField = _multiLinePropertyFields[item.Name] as TextBox;

                if (propertyField != null)
                {
                    propertyField.Text = item.Value;
                    propertyField.TextMode = TextBoxMode.MultiLine;
                }
            }
        }

        protected void ProfileRadioList_DataBound(object sender, EventArgs e)
        {
            var profileProperties = properties.Where(item => item.FieldType == CustomFieldType.RadioButton);

            foreach (var item in profileProperties)
            {
                Panel panel = _radioPropertyFields[item.Name] as Panel;
                if(panel != null)
                {
                    RadioButtonField radioButton = ProfileManager.GetRadioButtonField(item.Name, AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid);
                    RadioButtonList radioButtonList = new RadioButtonList();
                    foreach (var option in radioButton.Options) {
                        var radioButtonOption = new ListItem(option.Name);
                        radioButtonOption.Selected = option.IsSelected;
                        radioButtonList.Items.Add(radioButtonOption);
                    }
                    panel.Controls.Add(radioButtonList);
                }
            }
        }

        protected void ProfileMatrixList_DataBound(object sender, EventArgs e)
        {
            var profileProperties = properties.Where(item => item.FieldType == CustomFieldType.Matrix);

            foreach (var item in profileProperties)
            {
                //GridView propertyField = _propertyFields[item.Name] as GridView;
                Panel panel = _matrixTables[item.Name] as Panel;
                if (panel != null)
                {
                    var matrix = ProfileManager.GetMatrixField(item.Name, AuthenticationService.GetCurrentPrincipal(string.Empty).UserGuid);

                    MatrixGrid matrixTable = (MatrixGrid)((Page)HttpContext.Current.Handler).LoadControl(
                            "~/Users/Controls/MatrixGrid.ascx");

                    matrixTable.Initialize(matrix);
                    panel.Controls.Add(matrixTable);
                }
            }
        }

        private bool ValidateEmail(string email, bool isRequired)
        {
            EmailValidator validator = new EmailValidator();
            if (isRequired)
            {
                return validator.Validate(email);
            }

            return validator.ValidateOptional(email);
        }

        private bool ValidateEmailFields()
        {
            var allEmailFields = properties.Where(p => p.FieldType == CustomFieldType.Email);
            var updatedEmailFields = ProfileProperties.Where(p => allEmailFields.Any(email => email.Name == p.Key)).ToList();
            var allEmailsCorrect = true;

            foreach (var email in updatedEmailFields)
            {
                var emailErrorControl = _propertyFields[email.Key].Parent.FindControl("_emailInvalidLabel");
                if (emailErrorControl != null)
                    emailErrorControl.Visible = false;

                if (!ValidateEmail(email.Value, false) && emailErrorControl != null)
                {
                    emailErrorControl.Visible = true;
                    allEmailsCorrect = false;
                }
            }

            return allEmailsCorrect;
        }


        #endregion

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
        }

        public void WireUndoControl(Control sourceControl)
        {
            throw new NotImplementedException();
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _statusControl.Message = message;
            _statusControl.MessageType = messageType;
            //_statusControl.ActionText = actionText;
            //_statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        #endregion
    }
}
