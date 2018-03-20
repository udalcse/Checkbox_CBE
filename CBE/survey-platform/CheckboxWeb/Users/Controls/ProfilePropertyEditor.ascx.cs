using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;
using System.Web;
using Checkbox.Security;
using Checkbox.Users;

namespace CheckboxWeb.Users.Controls
{
    public partial class ProfilePropertyEditor : Checkbox.Web.Common.UserControlBase
    {
        private Dictionary<string, Control> _propertyFields;
        private Dictionary<string, Control> _multiLineFields;

        //Get/set whether control is read-only or not
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IPrincipal UserToEdit { get; private set; }

        private Dictionary<string, Control> _matrixTables;

        private Dictionary<string, Control> _radioFields;

        private List<ProfileProperty> properties;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToEdit"></param>
        /// <param name="isReadOnly"></param>
        public void Initialize(IPrincipal userToEdit, bool isReadOnly)
        {
            UserToEdit = userToEdit;
            IsReadOnly = isReadOnly;

            _propertyFields = new Dictionary<string, Control>();

            properties = ProfileManager.GetProfileProperties(Context.User.Identity.Name)
                    .Where(item => !item.Name.Equals("Email", StringComparison.InvariantCultureIgnoreCase) && !item.IsHidden)
                    .ToList();

            List<string> propertyNames = properties.Where(prop => prop.FieldType != CustomFieldType.Matrix && prop.FieldType != CustomFieldType.RadioButton && prop.FieldType != CustomFieldType.MultiLine)
                .Select(item => item.Name).ToList();
            List<string> matrixPropertyNames = properties.Where(prop => prop.FieldType == CustomFieldType.Matrix).Select(item => item.Name).ToList();
            List<string> radioPropertyNames = properties.Where(prop => prop.FieldType == CustomFieldType.RadioButton).Select(item => item.Name).ToList();
            List<string> multyLineList = properties.Where(prop => prop.FieldType == CustomFieldType.MultiLine)
                .Select(item => item.Name)
                .ToList();
            _noPropertiesPanel.Visible = propertyNames.Count == 0;

            _profileList.DataSource = propertyNames;
            _profileList.DataBind();

            _multiLineFields = new Dictionary<string, Control>();
            _multiLineList.DataSource = multyLineList;
            _multiLineList.DataBind();

            _radioFields = new Dictionary<string, Control>();
            _profileRadioList.DataSource = radioPropertyNames;
            _profileRadioList.DataBind();

            _matrixTables = new Dictionary<string, Control>();
            _profileMatrixList.DataSource = matrixPropertyNames;
            _profileMatrixList.DataBind();
        }

        public void SaveProfileProperties()
        {

            var allProfileFields = this.ProfileProperties.Concat(this.MultiLineFields)
                .ToDictionary(x => x.Key, x => x.Value);

            if (allProfileFields.Keys.Count > 0)
            {
                ProfileManager.StoreProfile(UserToEdit.Identity.Name, allProfileFields);
            }
            if (this.MatrixTables.Any())
            {
                this.StoreMatrices();
            }
            if (this.RadioFields.Any())
            {
                this.StoreRadioButtons();
            }
        }

        public void StoreRadioButtons(Guid? userGuid = null)
        {
            foreach (var item in _radioFields)
            {
                var radioButtonControl = (RadioButtonList)_radioFields[item.Key].Controls[0];
                var selectedItem = radioButtonControl.SelectedItem;
                if (selectedItem == null)
                    continue;

                RadioButtonField radioButtonField = new RadioButtonField();
                radioButtonField.Name = item.Key;
                radioButtonField.Options = new List<RadioButtonFieldOption>();
                foreach (ListItem listItem in radioButtonControl.Items)
                {
                    RadioButtonFieldOption option = new RadioButtonFieldOption();
                    option.Name = listItem.Value;
                    option.IsSelected = listItem.Selected;
                    radioButtonField.Options.Add(option);
                }

                if (userGuid == null)
                    userGuid = UserManager.GetUserPrincipal(UserToEdit.Identity.Name).UserGuid;

                ProfileManager.AddRadioButtonField(radioButtonField, userGuid);
            }
        }

        public void StoreMatrices()
        {
            foreach (var item in this.MatrixTables)
            {
                var gridControl = (GridView)_matrixTables[item.Key].Controls[0].Controls[0];
                var matrixControl = (MatrixGrid)_matrixTables[item.Key].Controls[0];
                var matrixCellValues =
                       Request.Form.AllKeys.Where(key => key.Contains("_matrixField_" + item.Key + "$") || key.Contains(item.Key + "_RowHeader") || key.Contains(item.Key + "_ColumnHeader")).ToList();

                matrixCellValues.RemoveAll(cell => cell.Contains("rowsCount") || cell.Contains("columnsCount"));
                var userId = UserManager.GetUserPrincipal(UserToEdit.Identity.Name).UserGuid;
                var matrix = ProfileManager.GetMatrixField(item.Key, userId);

                var tableRowsCountKey = Request.Form.AllKeys.FirstOrDefault(key => key.Contains("_rowsCountTxt" + item.Key));
                var tableColumnCountKey = Request.Form.AllKeys.FirstOrDefault(key => key.Contains("_columnsCountTxt" + item.Key));

                int columns = int.Parse(Request.Form[tableColumnCountKey]);
                var rows = int.Parse(Request.Form[tableRowsCountKey]);

                matrix.RemoveDataCells();

                if (matrix.HasFixedStructure)
                {
                    var requestValues = matrixCellValues.Select(key => Request.Form[key]).ToList();
                    matrix.RemoveDataCells();
                    matrix.CreateDataCells(rows, columns, requestValues);
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
                        var rowHeaderToInsert = requestValues.Count / columns - 1;
                        var emptyRows = new List<string>();

                        for (int i = 0; i < rowHeaderToInsert; i++)
                            emptyRows.Add(null);

                        if (rowHeaderToInsert >= 1)
                            matrix.CreateCustomRowHeaders(emptyRows);
                    }

                    matrix.CreateDataCells(rows, columns, matrixCellValues.Select(key => Request.Form[key]).ToList());
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

                ProfileManager.AddCustomMatrixCells(matrix, userId);

                matrixControl.Initialize(matrix);
                matrixControl.BindMatrix();
            }
        }


        #region Properties


        public Dictionary<string, string> MatrixTables
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

        public Dictionary<string, string> RadioFields
        {
            get
            {
                var output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _radioFields.Keys)
                {
                    if (_radioFields.ContainsKey(profilePropertyName))
                    {
                        Panel propertyField = _radioFields[profilePropertyName] as Panel;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, string.Empty);
                        }
                    }
                }

                return output;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ProfileProperties
        {
            get
            {
                var output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _propertyFields.Keys)
                {
                    if (_propertyFields.ContainsKey(profilePropertyName))
                    {
                        var propertyField = _propertyFields[profilePropertyName] as TextBox;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, propertyField.Text.Trim());
                        }
                    }
                }

                return output;
            }
        }

        public Dictionary<string, string> MultiLineFields
        {
            get
            {
                var output = new Dictionary<string, string>();

                foreach (string profilePropertyName in _multiLineFields.Keys)
                {
                    if (_multiLineFields.ContainsKey(profilePropertyName))
                    {
                        var propertyField = _multiLineFields[profilePropertyName] as TextBox;
                        if (propertyField != null)
                        {
                            output.Add(profilePropertyName, propertyField.Text.Trim());
                        }
                    }
                }

                return output;
            }
        }

        #endregion


        #region Event handlers

        /// <summary>
        /// Handles the item data bound event of the profile repeater; sets the control ID
        /// of the fields and stores the keys in a dictionary so that the data can be loaded 
        /// for an existing user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var valueField = e.Item.FindControl("_propertyValue");
            var isEmailField = properties.Where(p => p.FieldType == CustomFieldType.Email).Any(p => p.Name == e.Item.DataItem.ToString());

            if (valueField != null
                && valueField is TextBox)
            {
                ((TextBox)valueField).Enabled = !IsReadOnly;

                if (isEmailField)
                {
                    ((TextBox)valueField).Attributes["type"] = "email";
                    ((TextBox)valueField).Attributes["name"] = "email";
                }

                _propertyFields.Add(e.Item.DataItem.ToString(), e.Item.FindControl("_propertyValue"));
            }
        }

        protected void ProfileMatrixList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _matrixTables.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_matrixGridPanel"));
        }

        protected void ProfileRadioList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _radioFields.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_radioButtonPanel"));
        }

        protected void MultiLineList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            _multiLineFields.Add(((ListViewDataItem)e.Item).DataItem.ToString(), e.Item.FindControl("_multiLineGridPanel"));
        }

        /// <summary>
        /// Handles the data bound event; loads user data into the profile list after the 
        /// list has been assembled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileList_DataBound(object sender, EventArgs e)
        {
            // if we edit user we take properties with values otherwise only list of properties to populate them during creating new user
            var properties = UserToEdit != null ? ProfileManager.GetProfileProperties(UserToEdit.Identity.Name)
                .Where(p => p.FieldType != CustomFieldType.Matrix && p.FieldType != CustomFieldType.RadioButton && p.FieldType != CustomFieldType.MultiLine).ToList()
                : ProfileManager.GetPropertiesList().Where(p => p.FieldType != CustomFieldType.Matrix && p.FieldType != CustomFieldType.RadioButton && p.FieldType != CustomFieldType.MultiLine).ToList();

            MapProfileFields(properties);
        }

        protected void MultiLineList_DataBound(object sender, EventArgs e)
        {
            var properties = UserToEdit != null ? ProfileManager.GetProfileProperties(UserToEdit.Identity.Name)
                    .Where(p => p.FieldType == CustomFieldType.MultiLine).ToList()
                : ProfileManager.GetPropertiesList().Where(p => p.FieldType == CustomFieldType.MultiLine).ToList();

            MapMultiLine(properties);
        }

        protected void ProfileRadioList_DataBound(object sender, EventArgs e)
        {
            var profileProperties = ProfileManager.GetProfileProperties(Context.User.Identity.Name).Where(item => item.FieldType == CustomFieldType.RadioButton);
            foreach (var item in profileProperties)
            {
                Panel panel = _radioFields[item.Name] as Panel;
                if (panel != null)
                {
                    Guid userGuid;
                    if (UserToEdit == null)
                    {
                        userGuid = UserManager.GetUserPrincipal(HttpContext.Current.User.Identity.Name).UserGuid;
                    }
                    else
                    {
                        userGuid = UserManager.GetUserPrincipal(UserToEdit.Identity.Name).UserGuid;
                    }
                    var radioButton = ProfileManager.GetRadioButtonField(item.Name, userGuid);
                    RadioButtonList radioButtonList = new RadioButtonList();
                    foreach (var option in radioButton.Options)
                    {
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
            var profileProperties = ProfileManager.GetProfileProperties(Context.User.Identity.Name).Where(item => item.FieldType == CustomFieldType.Matrix);

            foreach (var item in profileProperties)
            {
                //GridView propertyField = _propertyFields[item.Name] as GridView;
                Panel panel = _matrixTables[item.Name] as Panel;
                if (panel != null)
                {
                    Guid userGuid;
                    if (UserToEdit == null)
                    {
                        userGuid = UserManager.GetUserPrincipal(HttpContext.Current.User.Identity.Name).UserGuid;
                    }
                    else
                    {
                        userGuid = UserManager.GetUserPrincipal(UserToEdit.Identity.Name).UserGuid;
                    }
                    var matrix = ProfileManager.GetMatrixField(item.Name, userGuid);

                    MatrixGrid matrixTable = (MatrixGrid)((Page)HttpContext.Current.Handler).LoadControl(
                            "~/Users/Controls/MatrixGrid.ascx");

                    matrixTable.Initialize(matrix);
                    panel.Controls.Add(matrixTable);
                }
            }
        }

        private void MapProfileFields(List<ProfileProperty> profileFields)
        {
            foreach (var item in profileFields)
            {
                var propertyField = _propertyFields[item.Name] as TextBox;
                if (propertyField != null)
                {
                    propertyField.Text = item.Value;
                }
            }
        }

        private void MapMultiLine(List<ProfileProperty> multiLineList)
        {
            foreach (var item in multiLineList)
            {
                var propertyField = _multiLineFields[item.Name] as TextBox;
                if (propertyField != null)
                {
                    propertyField.TextMode = TextBoxMode.MultiLine;
                    //propertyField.CssClass += " multiLineProperty";

                    propertyField.Text = item.Value;
                }
            }
        }

        #endregion
    }
}