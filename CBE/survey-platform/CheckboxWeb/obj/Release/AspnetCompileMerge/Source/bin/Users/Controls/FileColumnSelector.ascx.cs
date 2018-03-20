using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon.DynamoDBv2;
using Checkbox.Users;
using Checkbox.Security;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Users.Controls
{
    public partial class FileColumnSelector : Checkbox.Web.Common.UserControlBase
    {

        public bool ShouldSaveFieldsConfiguration => _saveFieldsConfiguration.Checked;

        public Button ClearCurrentConfiguration => _clearFieldsStructure;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                ClearSelectedUserFields();
            }
          
            BindProfileFieldList();
        }

        protected override void OnLoad(EventArgs e)
        {

            BindRemoveFieldsConfigurationBtn();
            BindSaveFieldsConfigurationCkbx();

        }

        /// <summary>
        /// Binds the save fields configuration CKBX.
        /// </summary>
        private void BindSaveFieldsConfigurationCkbx()
        {
            _saveFieldsConfiguration.Enabled = _selectedColumnList.Items.Count != 0;
        }

        /// <summary>
        /// Binds the remove fields configuration BTN.
        /// </summary>
        private void BindRemoveFieldsConfigurationBtn()
        {
            bool hasConfigs = UserManager.GetUserImportConfigs().Any();

            //if link is disabled add disabled class otherwise remove this class
            _clearFieldsStructure.Enabled = hasConfigs;
        }

        private void BindProfileFieldList()
        {
            _availableColumnList.Items.Clear();
            _selectedColumnList.Items.Clear();

            List<string> fieldNames = ProfileManager.ListPropertyNames();

            //Add domain, user name and password
            if (ApplicationManager.AppSettings.NTAuthentication)
            {
                fieldNames.Insert(0, WebTextManager.GetText("/pageText/users/import.aspx/domain"));
            }
            fieldNames.Insert(0, WebTextManager.GetText("/pageText/users/import.aspx/email"));
            fieldNames.Insert(0, WebTextManager.GetText("/pageText/users/import.aspx/password"));
            fieldNames.Insert(0, WebTextManager.GetText("/pageText/users/import.aspx/username"));

            //Add saved columns to SelectedUserFields on start
            if(!IsPostBack)
                SelectedUserFields.AddRange(UserManager.GetUserImportConfigs().Except(SelectedUserFields));

            //Add unselected fields
            foreach (string fieldName in fieldNames)
            {
                if (!SelectedUserFields.Contains(fieldName.ToString()))
                {
                    _availableColumnList.Items.Add(new ListItem(fieldName, fieldName));
                }
            }

            //Add selected fields
            foreach (string fieldName in SelectedUserFields)
            {
                _selectedColumnList.Items.Add(new ListItem(fieldName, fieldName));
            }
        }

        public void ClearSelectedUserFields()
        {
            SelectedUserFields = null;
        }

        #region Control event handlers

        public void ClearCurrentConfiguration_Click(object sender, EventArgs e)
        {
            SelectedUserFields.Clear();
            UserImportConfigManager.RemoveUserImportConfigs();
            BindProfileFieldList();

            _clearFieldsStructure.Enabled = false;
            _saveFieldsConfiguration.Enabled = false;
            _saveFieldsConfiguration.Checked = false;
        }

        /// <summary>
        /// Handles the click event of the move right button
        /// - Moves fields from the available list to the selected list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MoveRightButton_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in _availableColumnList.Items)
            {
                if (item.Selected
                    && !SelectedUserFields.Contains(item.Value))
                {
                    SelectedUserFields.Add(item.Value);
                }
            }

            BindProfileFieldList();

            BindSaveFieldsConfigurationCkbx();
        }

        /// <summary>
        /// Handles the click event of the move left button
        /// - Moves fields from the selected list to the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MoveLeftButton_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in _selectedColumnList.Items)
            {
                if (item.Selected
                    && SelectedUserFields.Contains(item.Value))
                {
                    SelectedUserFields.Remove(item.Value);
                }
            }

            BindProfileFieldList();

            BindSaveFieldsConfigurationCkbx();
        }

        /// <summary>
        /// Handles the click event of the move up button
        /// - Moves the selected fields up in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MoveFieldUpBtn_Click(object sender, ImageClickEventArgs e)
        {
            List<string> selectedFields = SelectedUserFields;

            foreach (ListItem listItem in _selectedColumnList.Items)
            {
                if (listItem.Selected)
                {
                    if (selectedFields.Contains(listItem.Value)
                        && selectedFields.IndexOf(listItem.Value) > 0)
                    {
                        selectedFields.Reverse(
                            selectedFields.IndexOf(listItem.Value) - 1,
                            2);
                    }
                }
            }

            SelectedUserFields = selectedFields;

            BindProfileFieldList();
        }

        /// <summary>
        /// Handles the click event of the move down button
        /// - Moves the selected fields down in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MoveFieldDownBtn_Click(object sender, ImageClickEventArgs e)
        {
            List<string> selectedFields = SelectedUserFields;

            foreach (ListItem listItem in _selectedColumnList.Items)
            {
                if (listItem.Selected)
                {
                    if (selectedFields.Contains(listItem.Value)
                        && selectedFields.IndexOf(listItem.Value) >= 0
                        && selectedFields.IndexOf(listItem.Value) < selectedFields.Count - 1)
                    {
                        selectedFields.Reverse(
                            selectedFields.IndexOf(listItem.Value),
                            2);
                    }
                }
            }

            SelectedUserFields = selectedFields;

            BindProfileFieldList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set currently selected user profile properties
        /// </summary>
        public List<string> SelectedUserFields
        {
            get
            {
                if (Session["ImportUsersSelectedUserFields"] == null)
                {
                    Session["ImportUsersSelectedUserFields"] = new List<string>();
                }
                return Session["ImportUsersSelectedUserFields"] as List<string>;
            }
            set { Session["ImportUsersSelectedUserFields"] = value; }

        }

        #endregion


    }
}