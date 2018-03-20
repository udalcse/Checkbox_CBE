//===============================================================================
// Checkbox UI Controls
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using Prezza.Framework.Security;
using Checkbox.Security;
using Checkbox.Web.UI.Controls;

namespace Checkbox.Web.Security.UI
{
    /// <summary>
    /// Event handler delegate for policy editor events.
    /// </summary>
    public delegate void PolicyEditorEventHandler(object sender, SecurityEditorPolicyControl.PolicyEditorEventArgs e);

    /// <summary>
    /// Control for rendering a policy and allowing selection of permission masks and permissions.
    /// </summary>
    public class SecurityEditorPolicyControl : Common.WebControlBase, INamingContainer
    {
        /// <summary>
        /// Fired when an item in the permissions list is checked.
        /// </summary>
        public event PolicyEditorEventHandler ItemChecked;

        /// <summary>
        /// Fired when an item in the permissions list is unchecked.
        /// </summary>
        public event PolicyEditorEventHandler ItemUnChecked;

        /// <summary>
        /// Fired when the control's 'Apply' button is checked.
        /// </summary>
        public event PolicyEditorEventHandler ApplyButtonClick;

        private ArrayList _permissibleEntities;
        private IAccessControllable _controllableEntity;

        private string _checkedCssClass;
        private string _unCheckedCssClass;
        private string _grayCssClass;

        private string _headerCssClass;

        private string _titleCssClass;
        private string _subTitleCssClass;

        private int _gridWidth;
        private int _gridHeight;
        private int _gridBorderWidth;

        private DataGrid _dg;

        private Panel _policyPanel;
        private Panel _scrollPanel;
        private Panel _bottomPanel;

        private MultiLanguageLabel _gridLabel;
        private MultiLanguageLabel _warningLabel;

        private MultiLanguageButton _applyButton;

        private string _warningCssClass;
        private string _buttonCssClass;

        private string _subTitle;

        /// <summary>
        /// Current edit mode of the control.
        /// </summary>
        public enum PolicyEditMode
        {
            /// <summary>
            /// Editing permission masks on a controllable item.
            /// </summary>
            Permission,

            /// <summary>
            /// Editing individual permissions on a controllable item.
            /// </summary>
            PermissionAdvanced,

            /// <summary>
            /// Editing default policy with permission masks on a controllable item.
            /// </summary>
            DefaultPolicy,

            /// <summary>
            /// Editing individual permissions on the default policy.
            /// </summary>
            DefaultPolicyAdvanced
        }

        /// <summary>
        /// State of permission checkbox
        /// </summary>
        private enum CheckState
        {
            Checked,
            UnChecked,
            GrayChecked,
            GrayUnchecked
        }

        /// <summary>
        /// Init handler to ensure that child controls are created in time for view state to be restored.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            base.OnInit(e);
        }

        /// <summary>
        /// Initialize the control.
        /// </summary>
        /// <param name="permissibleEntities">List of permissible entities the permission display should reflect.</param>
        /// <param name="controllableEntity">Entity the permissions are to be displayed for.</param>
        public void Initialize(IAccessPermissible[] permissibleEntities, IAccessControllable controllableEntity)
        {
            //Save edit mode before clearing viewstate, since that should be saved
            PolicyEditMode editMode = EditMode;

            ViewState.Clear();

            //Restore the edit mode
            EditMode = editMode;

            _controllableEntity = controllableEntity;

            _permissibleEntities = permissibleEntities != null ? new ArrayList(permissibleEntities) : new ArrayList();
        }

        /// <summary>
        /// Perform any data-bind time display processing and bind the data to the sub-controls.
        /// </summary>
        public void BindData()
        {
            EnsureChildControls();

            //Set post back, if necessary
            if (EditMode == PolicyEditMode.Permission || EditMode == PolicyEditMode.PermissionAdvanced)
                ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).AutoPostBack = true;
            else
                ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).AutoPostBack = false;

            //Set style information
            SetStyles();

            //Clear selected items when new items are selected
            SelectedItemsInternal.Clear();

            if (_controllableEntity != null)
            {
                //Get the data table to bind, depending on edit mode
                DataTable t;

                //Unless advanced mode, use permission masks
                if (EditMode == PolicyEditMode.DefaultPolicy || EditMode == PolicyEditMode.Permission)
                {
                    t = GetMaskedPermissionsDataTable();
                }
                else if (EditMode == PolicyEditMode.DefaultPolicyAdvanced || EditMode == PolicyEditMode.PermissionAdvanced)
                {
                    t = GetAllPermissionsDataTable();
                }
                else
                {
                    t = null;
                }

                _dg.DataSource = t;
                _dg.DataBind();
            }
        }

        /// <summary>
        /// Get the data table containing information to render the permissions mask table
        /// </summary>
        private DataTable GetMaskedPermissionsDataTable()
        {
            DataTable t = new DataTable();
            t.Columns.Add("PermissionName");
            t.Columns.Add("DisplayName");
            t.Columns.Add("CheckState");
            t.Columns.Add("NumWithPermission");
            t.Columns.Add("NumWithoutPermission");
            t.Columns.Add("NumNotAllowed");

            //Get the list of supported permission masks
            string[] supportedPermissionMasks = _controllableEntity.SupportedPermissionMasks;

            double permissibleEntitiesCount = 1;

            if (_permissibleEntities.Count > 0)
            {
                permissibleEntitiesCount = _permissibleEntities.Count;
            }

            if (supportedPermissionMasks != null)
            {
                for (int i = 0; i < supportedPermissionMasks.Length; i++)
                {
                    string permissionMask = supportedPermissionMasks[i];
                    object[] rowData = new object[6];
                    CheckState maskCheckState = CheckState.UnChecked;

                    int totalWithPermission = 0;
                    int totalUnableToHavePermission = 0;
                    int numPermissions = 0;

                    List<string> maskPermissions = AccessManager.GetPermissionMaskPermissions(permissionMask);
                    
                    foreach (string maskPermission in maskPermissions)
                    {
                        numPermissions++;

                        //If we are not in default policy mode, show information about current acl entries, which is determined by GetCheckState(...)
                        if (EditMode == PolicyEditMode.Permission)
                        {
                            int numWithPermission;
                            int numWithoutPermission;
                            int numUnableToHavePermission;

                            //Get check state information
                            GetCheckState(maskPermission, out numWithPermission, out numWithoutPermission, out numUnableToHavePermission);

                            totalWithPermission += numWithPermission;
                            totalUnableToHavePermission += numUnableToHavePermission;
                        }
                        else
                        {
                            //Get check state information
                            CheckState permissionCheckState = GetCheckState(maskPermission);

                            if (permissionCheckState == CheckState.Checked)
                            {
                                totalWithPermission++;
                            }
                        }
                    }
                    
                    //In mask mode, items are only checked if all permissions in the mask are present.  If only some are, leave it unchecked
                    if (totalWithPermission / permissibleEntitiesCount == numPermissions)
                    {
                        //If all have permission, check
                        if (totalWithPermission > 0)
                        {
                            maskCheckState = CheckState.Checked;
                        }
                    }
                    else
                    {
                        //If not all have permission, but some do, gray uncheck
                        if (totalWithPermission > 0)
                        {
                            maskCheckState = CheckState.GrayUnchecked;
                        }
                        else
                        {
                            //If none have permission, but some are unable, gray uncheck, otherwise normal uncheck
                            maskCheckState = totalUnableToHavePermission > 0 ? CheckState.GrayUnchecked : CheckState.UnChecked;
                        }
                    }

                    if (!NotAllowedCounts.Contains(permissionMask))
                    {
                        NotAllowedCounts.Add(permissionMask, totalUnableToHavePermission);
                    }
                    else
                    {
                        NotAllowedCounts[permissionMask] = totalUnableToHavePermission;
                    }

                    //Show warning if necessary
                    //if(totalUnableToHavePermission > 0)
                    //{
                    //	_warningLabel.Visible = true;
                    //}

                    //For each mask, get the name
                    rowData[0] = permissionMask;
                    rowData[1] = AccessManager.GetPermissionMaskDisplayName(permissionMask);
                    rowData[2] = maskCheckState;
                    rowData[3] = 0;
                    rowData[4] = 0;
                    rowData[5] = 0;

                    //Add selected item to internal collection
                    if (maskCheckState == CheckState.Checked || maskCheckState == CheckState.GrayChecked)
                    {
                        if (!SelectedItemsInternal.Contains(permissionMask))
                        {
                            SelectedItemsInternal.Add(permissionMask);
                        }
                    }

                    t.Rows.Add(rowData);
                }
            }

            return t;
        }

        /// <summary>
        /// Get a data table containing all permissions
        /// </summary>
        private DataTable GetAllPermissionsDataTable()
        {
            DataTable t = new DataTable();
            t.Columns.Add("PermissionName");
            t.Columns.Add("DisplayName");
            t.Columns.Add("CheckState");
            t.Columns.Add("NumWithPermission");
            t.Columns.Add("NumWithoutPermission");
            t.Columns.Add("NumNotAllowed");

            string[] supportedPermissions = _controllableEntity.SupportedPermissions;

            if (supportedPermissions != null)
            {
                for (int i = 0; i < supportedPermissions.Length; i++)
                {
                    string permission = supportedPermissions[i];
                    object[] rowData;
                    CheckState checkState;

                    if (EditMode == PolicyEditMode.PermissionAdvanced)
                    {
                        int numWithPermission;
                        int numWithoutPermission;
                        int numUnableToHavePermission;

                        //Get check state information
                        checkState = GetCheckState(permission, out numWithPermission, out numWithoutPermission, out numUnableToHavePermission);

                        //Build object array to use as row to add
                        rowData = new object[6];

                        rowData[0] = permission;
                        rowData[1] = permission;
                        rowData[2] = checkState;
                        rowData[3] = numWithPermission;
                        rowData[4] = numWithoutPermission;
                        rowData[5] = numUnableToHavePermission;

                        //Store the counts so they can be used on check-changed events to show/hide the warning
                        if (!NotAllowedCounts.Contains(permission))
                        {
                            NotAllowedCounts.Add(permission, numUnableToHavePermission);
                        }
                        else
                        {
                            NotAllowedCounts[permission] = numUnableToHavePermission;
                        }

                        if (numUnableToHavePermission > 0 && (checkState == CheckState.Checked || checkState == CheckState.GrayChecked))
                        {
                            _warningLabel.Visible = true;
                        }
                    }
                    else
                    {
                        //Get check state information
                        checkState = GetCheckState(permission);

                        //Build object array as row to add
                        rowData = new object[6];

                        rowData[0] = permission;
                        rowData[1] = permission;
                        rowData[2] = checkState;
                        rowData[3] = 0;
                        rowData[4] = 0;
                        rowData[5] = 0;
                    }

                    //Add selected item to internal collection
                    if (checkState == CheckState.Checked || checkState == CheckState.GrayChecked)
                    {
                        if (!SelectedItemsInternal.Contains(permission))
                        {
                            SelectedItemsInternal.Add(permission);
                        }
                    }

                    t.Rows.Add(rowData);
                }
            }

            return t;
        }

        /// <summary>
        /// Set control styles at bind time so that run-time modifications are taken into account.
        /// </summary>
        private void SetStyles()
        {
            ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).CheckedCssClass = CheckedCssClass;
            ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).GrayCssClass = GrayCssClass;
            ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).UnCheckedCssClass = UnCheckedCssClass;
            ((PermissionColumn)((TemplateColumn)_dg.Columns[0]).ItemTemplate).ReadOnly = ReadOnly;

            _dg.HeaderStyle.CssClass = HeaderCssClass;
            _gridLabel.CssClass = SubTitleCssClass;
            _warningLabel.CssClass = WarningCssClass;
            _applyButton.CssClass = ButtonCssClass;

            if (EditMode == PolicyEditMode.PermissionAdvanced)
            {
                _dg.Columns[1].ItemStyle.CssClass = CheckedCssClass;
                _dg.Columns[1].ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                _dg.Columns[2].ItemStyle.CssClass = CheckedCssClass;
                _dg.Columns[2].ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                _dg.Columns[3].ItemStyle.CssClass = WarningCssClass;
                _dg.Columns[3].ItemStyle.HorizontalAlign = HorizontalAlign.Center;

                if (ShowCounts)
                {
                    _dg.ShowHeader = true;
                    _dg.Columns[1].Visible = true;
                    _dg.Columns[2].Visible = true;
                    _dg.Columns[3].Visible = true;
                }
            }
            else
            {
                _dg.ShowHeader = false;
                _dg.Columns[1].Visible = false;
                _dg.Columns[2].Visible = false;
                _dg.Columns[3].Visible = false;
            }

            _scrollPanel.Height = Unit.Pixel(GridHeight);
            _scrollPanel.Width = Unit.Pixel(GridWidth);
            _bottomPanel.Width = Unit.Pixel(GridWidth);
            _dg.BorderWidth = Unit.Pixel(GridBorderWidth);

            _gridLabel.Visible = ShowSubTitle;

            _warningLabel.Visible = false;

            //By default, button is disabled, unless a change is detected
            _applyButton.Enabled = false;

            //Firefox seems to not change the button style when disabled
            _applyButton.Style.Add("color", "gray");

            if (ReadOnly || !ShowButton)
            {
                _applyButton.Visible = false;
            }
            else
            {
                _applyButton.Visible = true;
            }
        }

        /// <summary>
        /// Return a string
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        private CheckState GetCheckState(string permission)
        {
            Policy p = _controllableEntity.DefaultPolicy;

            if (p != null)
            {
                if (p.HasPermission(permission))
                {
                    return CheckState.Checked;
                }

                return CheckState.UnChecked;
            }

            return CheckState.UnChecked;
        }

        /// <summary>
        /// Get the check state for the permission and current controllable entity
        /// </summary>
        /// <param name="permission">Permission to get state of</param>
        /// <param name="numWithPermission">Number of selected entities with the permission.</param>
        /// <param name="numWithoutPermission">Number of selected entities without the permission.</param>
        /// <param name="numUnableToHavePermission">Number of selected entities not in roles that allow the permission.</param>
        /// <returns>Check state</returns>
        private CheckState GetCheckState(string permission, out int numWithPermission, out int numWithoutPermission, out int numUnableToHavePermission)
        {
            numWithPermission = 0;
            numWithoutPermission = 0;
            numUnableToHavePermission = 0;

            for (int i = 0; i < _permissibleEntities.Count; i++)
            {
                IAccessPermissible permissibleEntity = (IAccessPermissible)_permissibleEntities[i];

                IAccessControlList acl = _controllableEntity.ACL;

                if (acl != null)
                {
                    Policy p = acl.GetPolicy(permissibleEntity);

                    if (p != null)
                    {
                        if (p.HasPermission(permission))
                            numWithPermission++;
                        else
                            numWithoutPermission++;
                    }
                    else
                    {
                        numWithoutPermission++;
                    }
                }
                else
                {
                    numWithoutPermission++;
                }

                //TODO: Handle cases of different authentication types
                if (string.Compare(permissibleEntity.AclTypeIdentifier, "Prezza.Framework.Security.ExtendedPrincipal", true) == 0)
                {
                    if (!RoleManager.UserHasRoleWithPermission(permissibleEntity.AclEntryIdentifier, permission))
                    {
                        numUnableToHavePermission++;
                    }
                }
            }

            if (numWithPermission > 0 && numWithoutPermission > 0)
            {
                return CheckState.GrayChecked;
            }

            if (numWithPermission == 0 && numWithoutPermission > 0)
            {
                return CheckState.UnChecked;
            }

            if (numWithPermission > 0 && numWithoutPermission == 0)
            {
                return CheckState.Checked;
            }

            return CheckState.UnChecked;
        }

        /// <summary>
        /// Create child controls that make up this control
        /// </summary>
        protected override void CreateChildControls()
        {
            _policyPanel = new Panel();
            _scrollPanel = new Panel();
            _bottomPanel = new Panel();
            _dg = new DataGrid();
            _gridLabel = new MultiLanguageLabel();
            _warningLabel = new MultiLanguageLabel();
            _applyButton = new MultiLanguageButton();

            _scrollPanel.Controls.Add(_dg);
            _policyPanel.Controls.Add(_gridLabel);
            _policyPanel.Controls.Add(_scrollPanel);

            _bottomPanel.Controls.Add(_warningLabel);
            _bottomPanel.Controls.Add(new LiteralControl("<table width=\"100%\"><tr><td align=\"right\">"));
            _bottomPanel.Controls.Add(_applyButton);
            _bottomPanel.Controls.Add(new LiteralControl("</td></tr></table>"));

            _policyPanel.Controls.Add(_bottomPanel);

            Controls.Add(_policyPanel);

            _gridLabel.Text = "Permissions:";

            _scrollPanel.Style["overflow-y"] = "scroll";
            _scrollPanel.BorderStyle = BorderStyle.Inset;
            _scrollPanel.BorderWidth = Unit.Pixel(4);

            TemplateColumn tc1 = new TemplateColumn { HeaderText = "Permission", ItemTemplate = new PermissionColumn() };
            BoundColumn bc1 = new BoundColumn { HeaderText = "# Having", DataField = "NumWithPermission" };
            BoundColumn bc2 = new BoundColumn { HeaderText = "# Not Having", DataField = "NumWithoutPermission" };
            TemplateColumn tc2 = new TemplateColumn { HeaderText = "# Not Allowed", ItemTemplate = new NotAllowedColumn() };

            _dg.AutoGenerateColumns = false;
            _dg.ShowHeader = false;
            _dg.ShowFooter = false;
            _dg.Width = Unit.Percentage(100);

            _dg.Columns.Add(tc1);

            _dg.Columns.Add(bc1);
            _dg.Columns.Add(bc2);
            _dg.Columns.Add(tc2);

            _applyButton.Text = "Apply Policy";
            _applyButton.TextId = "/controlText/securityEditorPolicyControl/applyPolicy";
            _applyButton.Width = Unit.Pixel(100);

            _warningLabel.Text = "Please note that one or more of the selected entities are not members of a role that allow one or more of the selected permissions to be applied.  ";
            _warningLabel.Text += "For these entities, only permissions allowed by their roles will be applied.";
            _warningLabel.TextId = "/controlText/securityEditorPolicyControl/warningText";

            _dg.ItemCreated += dg_ItemCreated;
            _applyButton.Click += applyButton_Click;
        }

        #region Event Handlers

        /// <summary>
        /// Adds event handlers to checkbox controls when they are created in the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                foreach (TableCell cell in e.Item.Cells)
                {
                    foreach (WebControl control in cell.Controls)
                    {
                        if (control is CheckBox)
                        {
                            CheckBox box = (CheckBox)control;

                            box.Enabled = Enabled;

                            box.CheckedChanged += SecurityEditorPolicyControl_CheckedChanged;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for item checked/unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SecurityEditorPolicyControl_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = sender as CheckBox;

            if (c != null)
            {
                string permission = c.Attributes["PermissionName"].Trim();

                if (!string.IsNullOrEmpty(permission))
                {
                    if (c.Checked)
                    {
                        if (!SelectedItemsInternal.Contains(permission))
                        {
                            SelectedItemsInternal.Add(permission);
                        }

                        if (ItemChecked != null)
                        {
                            ItemChecked(this, new PolicyEditorEventArgs("ItemChecked", permission));
                        }
                    }
                    else
                    {
                        if (SelectedItemsInternal.Contains(permission))
                        {
                            SelectedItemsInternal.Remove(permission);
                        }

                        if (ItemUnChecked != null)
                        {
                            ItemUnChecked(this, new PolicyEditorEventArgs("ItemUnChecked", permission));
                        }
                    }
                }

                //Set the color of the text in the allowed box
                SetAllowedColor(c);

                //Now see if the warning text should be shown or hidden
                _warningLabel.Visible = false;

                foreach (object item in SelectedItemsInternal)
                {
                    if (NotAllowedCounts.Contains(item))
                    {
                        if ((int)NotAllowedCounts[item] > 0)
                        {
                            _warningLabel.Visible = true;
                        }
                    }
                }

                _applyButton.Enabled = true;
                _applyButton.Style.Remove("color");
            }
        }


        /// <summary>
        /// Set the color of the allowed column to reflect whether any role/permission conflicts may have been created.
        /// </summary>
        /// <param name="checkbox"></param>
        private static void SetAllowedColor(CheckBox checkbox)
        {
            if (checkbox != null)
            {
                //C.Parent = TD  containing checkbox
                //C.Parent.Parent = TR containing checkbox
                if (checkbox.Parent != null)
                {
                    TableRow row = checkbox.Parent.Parent as TableRow;

                    if (row != null)
                    {
                        if (row.Cells.Count >= 4)
                        {
                            TableCell cell = row.Cells[3];

                            foreach (WebControl control in cell.Controls)
                            {
                                if (control is Label)
                                {
                                    try
                                    {
                                        int number = int.Parse(((Label)control).Text);

                                        if (number == 0 && checkbox.Checked)
                                        {
                                            control.Style["color"] = "green";
                                        }
                                        else if (number > 0 && checkbox.Checked)
                                        {
                                            control.Style["color"] = "red";
                                        }
                                        else
                                        {
                                            control.Style["color"] = "black";
                                        }
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Event handler for clicking the apply button.  Fires an event to be handled by the controls creator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (ApplyButtonClick != null)
            {
                ApplyButtonClick(this, new PolicyEditorEventArgs("ApplyButtonClick", SelectedItems));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Edit mode for the policy editor.
        /// </summary>
        public PolicyEditMode EditMode
        {
            get
            {
                if (ViewState["EditMode"] == null)
                    ViewState["EditMode"] = PolicyEditMode.Permission;

                return (PolicyEditMode)ViewState["EditMode"];
            }
            set
            {
                ViewState["EditMode"] = value;
            }
        }

        /// <summary>
        /// Get/Set whether the apply-policy button is shown.
        /// </summary>
        public bool ShowButton { get; set; }

        /// <summary>
        /// Internal array of selected items.
        /// </summary>
        protected ArrayList SelectedItemsInternal
        {
            get
            {
                if (ViewState["SelectedItems"] == null)
                    ViewState["SelectedItems"] = new ArrayList();

                return (ArrayList)ViewState["SelectedItems"];
            }
            set
            {
                ViewState["SelectedItems"] = value;
            }
        }

        /// <summary>
        /// For each item, the count of selected items that can't have the respective permission.
        /// </summary>
        protected Hashtable NotAllowedCounts
        {
            get
            {
                if (ViewState["NotAllowedCounts"] == null)
                    ViewState["NotAllowedCounts"] = new Hashtable();

                return (Hashtable)ViewState["NotAllowedCounts"];
            }
            set
            {
                ViewState["NotAllowedCounts"] = value;
            }
        }


        /// <summary>
        /// Get the list of currently selected items.
        /// </summary>
        public string[] SelectedItems
        {
            get
            {
                if (EditMode == PolicyEditMode.DefaultPolicyAdvanced || EditMode == PolicyEditMode.PermissionAdvanced)
                {
                    ArrayList items = SelectedItemsInternal;
                    return (string[])items.ToArray(typeof(string));
                }

                ArrayList selectedMasks = SelectedItemsInternal;
                ArrayList selectedPermissions = new ArrayList();

                for (int i = 0; i < selectedMasks.Count; i++)
                {
                    List<string> maskPermissions = AccessManager.GetPermissionMaskPermissions(selectedMasks[i].ToString());

                    foreach (string maskPermission in maskPermissions)
                    {
                        if (!selectedPermissions.Contains(maskPermission))
                        {
                            selectedPermissions.Add(maskPermission);
                        }
                    }
                }

                return (string[])selectedPermissions.ToArray(typeof(string));
            }

        }

        /// <summary>
        /// Css class for checked items
        /// </summary>
        public string CheckedCssClass
        {
            get { return string.IsNullOrEmpty(_checkedCssClass) ? "PrezzaNormal" : _checkedCssClass; }
            set { _checkedCssClass = value; }
        }

        /// <summary>
        /// Css class for unchecked items.
        /// </summary>
        public string UnCheckedCssClass
        {
            get { return string.IsNullOrEmpty(_unCheckedCssClass) ? "PrezzaNormal" : _unCheckedCssClass; }
            set { _unCheckedCssClass = value; }
        }

        /// <summary>
        /// Css class for the grayed-out buttons.
        /// </summary>
        public string GrayCssClass
        {
            get { return string.IsNullOrEmpty(_grayCssClass) ? "PrezzaCondition" : _grayCssClass; }
            set { _grayCssClass = value; }
        }

        /// <summary>
        /// Set the width of the policy permission display grid.
        /// </summary>
        public int GridWidth
        {
            get { return _gridWidth <= 0 ? 300 : _gridWidth; }
            set { _gridWidth = value; }
        }

        /// <summary>
        /// Get/Set the width of the grid border.
        /// </summary>
        public int GridBorderWidth
        {
            get { return _gridBorderWidth < 0 ? 0 : _gridBorderWidth; }
            set { _gridBorderWidth = value; }
        }

        /// <summary>
        /// Get/set the height of the policy grid.
        /// </summary>
        public int GridHeight
        {
            get { return _gridHeight <= 0 ? 200 : _gridHeight; }
            set { _gridHeight = value; }
        }

        /// <summary>
        /// Get/Set the css class for the control title.
        /// </summary>
        public string TitleCssClass
        {
            get { return string.IsNullOrEmpty(_titleCssClass) ? "PrezzaTitle" : _titleCssClass; }
            set { _titleCssClass = value; }
        }

        /// <summary>
        /// Get/set the css class for the control sub headings.
        /// </summary>
        public string SubTitleCssClass
        {
            get { return string.IsNullOrEmpty(_subTitleCssClass) ? "PrezzaSubTitle" : _subTitleCssClass; }
            set { _subTitleCssClass = value; }
        }

        /// <summary>
        /// Get/set the css class for the control header.
        /// </summary>
        public string HeaderCssClass
        {
            get { return string.IsNullOrEmpty(_headerCssClass) ? "TableHeader" : _headerCssClass; }
            set { _headerCssClass = value; }
        }

        /// <summary>
        /// Get/Set whether the control shows the counts of items selected, unselected, etc.
        /// </summary>
        public bool ShowCounts { get; set; }

        /// <summary>
        /// Get/Set whether the control operates only in read-only mode.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Get/set the css class for warning text.
        /// </summary>
        public string WarningCssClass
        {
            get { return string.IsNullOrEmpty(_warningCssClass) ? "ErrorMessage" : _warningCssClass; }
            set { _warningCssClass = value; }
        }

        /// <summary>
        /// Css class for editor buttons.
        /// </summary>
        public string ButtonCssClass
        {
            get { return string.IsNullOrEmpty(_buttonCssClass) ? "PrezzaButton" : _buttonCssClass; }
            set { _buttonCssClass = value; }
        }

        /// <summary>
        /// Get/set the control sub heading.
        /// </summary>
        public string SubTitle
        {
            get { return string.IsNullOrEmpty(_subTitle) ? "Permissions:" : _subTitle; }
            set { _subTitle = value; }
        }

        /// <summary>
        /// Get/Set whether the control sub heading is shown.
        /// </summary>
        public bool ShowSubTitle { get; set; }

        #endregion

        #region EventArgs

        /// <summary>
        /// Container class for editor events.
        /// </summary>
        public class PolicyEditorEventArgs : EventArgs
        {
            private readonly List<string> _items;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="eventName">Event name.</param>
            /// <param name="item">Item associated with the event.</param>
            public PolicyEditorEventArgs(string eventName, string item)
            {
                EventName = eventName;
                _items = new List<string> { item };
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="eventName">Name of the events.</param>
            /// <param name="items">Items associated with the event.</param>
            public PolicyEditorEventArgs(string eventName, string[] items)
            {
                EventName = eventName;
                _items = items != null ? new List<string>(items) : new List<string>();
            }

            /// <summary>
            /// Get the name of the event.
            /// </summary>
            public string EventName { get; private set; }

            /// <summary>
            /// Get the _items associated with the event.
            /// </summary>
            public string[] Items
            {
                get { return _items.ToArray(); }
            }
        }
        #endregion

        #region Template

        /// <summary>
        /// Template for permissions
        /// </summary>
        private class PermissionColumn : ITemplate
        {
            public string CheckedCssClass { get; set; }
            public string UnCheckedCssClass { get; set; }
            public string GrayCssClass { get; set; }
            public bool ReadOnly { get; set; }
            public bool AutoPostBack { get; set; }

            #region ITemplate Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            public void InstantiateIn(Control container)
            {
                CheckBox b = new CheckBox();
                container.Controls.Add(b);

                b.DataBinding += b_DataBinding;
                b.AutoPostBack = true;
            }

            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void b_DataBinding(object sender, EventArgs e)
            {
                CheckBox b = (CheckBox)sender;

                DataGridItem item = (DataGridItem)b.NamingContainer;

                item.VerticalAlign = VerticalAlign.Top;

                string checkState = WebUtilities.GetPropertyFromDataGridItem("CheckState", item);

                item.Attributes.Add("CheckState", checkState);

                b.Text = WebUtilities.GetPropertyFromDataGridItem("DisplayName", item);
                b.Attributes["PermissionName"] = WebUtilities.GetPropertyFromDataGridItem("PermissionName", item);

                if (string.Compare(checkState, "Checked", true) == 0)
                {
                    b.CssClass = CheckedCssClass;
                    b.Checked = true;
                }
                else if (string.Compare(checkState, "GrayUnChecked", true) == 0)
                {
                    b.CssClass = GrayCssClass;
                    b.Checked = false;
                }
                else if (string.Compare(checkState, "GrayChecked", true) == 0)
                {
                    b.CssClass = GrayCssClass;
                    b.Checked = true;
                }
                else
                {
                    b.CssClass = UnCheckedCssClass;
                    b.Checked = false;
                }

                if (ReadOnly)
                {
                    b.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class NotAllowedColumn : ITemplate
        {
            #region ITemplate Members

            public void InstantiateIn(Control container)
            {
                Label l = new Label();

                l.DataBinding += l_DataBinding;

                container.Controls.Add(l);
            }

            #endregion

            /// <summary>
            /// CSS class for items
            /// </summary>
            public string ItemCssClass { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void l_DataBinding(object sender, EventArgs e)
            {
                Label l = (Label)sender;
                DataGridItem item = (DataGridItem)l.NamingContainer;

                if (!string.IsNullOrEmpty(ItemCssClass))
                {
                    l.CssClass = ItemCssClass;
                }

                string countString = WebUtilities.GetPropertyFromDataGridItem("NumNotAllowed", item);
                string checkState = WebUtilities.GetPropertyFromDataGridItem("CheckState", item);

                int count = !string.IsNullOrEmpty(countString) ? Convert.ToInt32(countString) : 0;

                if (count > 0 && (checkState == "Checked" || checkState == "GrayChecked"))
                {
                    l.Style["color"] = "red";
                }
                else if (count == 0 && (checkState == "Checked" || checkState == "GrayChecked"))
                {
                    l.Style["color"] = "green";
                }
                else
                {
                    l.Style["color"] = "black";
                }

                l.Text = count.ToString();
            }
        }
        #endregion
    }
}
