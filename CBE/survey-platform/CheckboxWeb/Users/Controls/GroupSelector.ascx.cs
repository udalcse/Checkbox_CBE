using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Principal;

using Checkbox.Users;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Pagination;

using Checkbox.Common;

namespace CheckboxWeb.Users.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GroupSelector : Checkbox.Web.Common.UserControlBase
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IPrincipal UserToEdit { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Int32> SelectedGroupIDs
        {
            get 
            {
                return 
                    _selectedGroupsTxt.Text.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(selectedValue => Utilities.AsInt(selectedValue, -1))
                    .Where(valueAsInt => valueAsInt > 0).ToList();
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jQuery.dualListBox-1.3.js",
                ResolveUrl("~/Resources/jQuery.dualListBox-1.3.js"));

            //prevent loosing selected items
            if (IsPostBack)
            {
                var selectedIds = SelectedGroupIDs;

                if (selectedIds.Count > 0 && _selectedGroups.Items.Count == 0)
                {
                    var currentUser = HttpContext.Current.User as CheckboxPrincipal;
                    var context = new PaginationContext
                    {
                        SortField = "GroupName",
                        SortAscending = true,
                        PermissionJoin = PermissionJoin.Any
                    };
                    context.Permissions.Add("Group.Edit");

                    _selectedGroups.DataTextField = "Name";
                    _selectedGroups.DataValueField = "ID";

                    _selectedGroups.DataSource = (from g in GroupManager.GetAccessibleGroups(currentUser, context) where selectedIds.Contains(g.ID.Value) select g);
                    _selectedGroups.DataBind();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userToEdit"></param>
        /// <param name="isReadOnly"></param>
        public void Initialize(IPrincipal userToEdit, bool isReadOnly)
        {
            UserToEdit = userToEdit;
            IsReadOnly = isReadOnly;

            _groupSelectorPanel.Visible = true;
            _noGroupsLabel.Visible = false;

            _selectedGroups.Enabled = !IsReadOnly;
            _availableGroupList.Enabled = !IsReadOnly;
            _moveLeftButton.Enabled = !IsReadOnly;
            _moveRightButton.Enabled = !IsReadOnly;

            //Bind selected
            BindSelectedGroupsList();

            //Bind available
            BindAvailableGroupsList();

            //If there are no groups, show a message to that effect
            if (_availableGroupList.Items.Count == 0 && _selectedGroups.Items.Count == 0)
            {
                _groupSelectorPanel.Visible = false;
                _noGroupsLabel.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindAvailableGroupsList()
        {
            //Bind the available group list
            _availableGroupList.DataTextField = "Name";
            _availableGroupList.DataValueField = "ID";
            _availableGroupList.DataSource = AvailableGroups;
            _availableGroupList.DataBind();
        }

        /// <summary>
        /// Per-request cache
        /// </summary>
        List<Group> _AvailableGroups;
        /// <summary>
        /// Available groups
        /// </summary>
        private List<Group> AvailableGroups
        {
            get
            {
                if (_AvailableGroups != null)
                    return _AvailableGroups;
                var currentUser = HttpContext.Current.User as CheckboxPrincipal;
                var context = new PaginationContext
                {
                    SortField = "GroupName",
                    SortAscending = true,
                    PermissionJoin = PermissionJoin.Any
                };

                context.Permissions.Add("Group.Edit");
                _AvailableGroups = GroupManager.GetAccessibleGroups(currentUser, context).Where(g => g.ID != 1).ToList();  //Don't bind to everyone group
                return _AvailableGroups;
            }
        }
        /// <summary>
        /// Groups count
        /// </summary>
        public int AvailableGroupsCount
        {
            get
            {
                return AvailableGroups.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindSelectedGroupsList()
        {
            //Now bind the selected group list
            if (UserToEdit != null)
            {
                _selectedGroups.DataValueField = "ID";
                _selectedGroups.DataTextField = "Name";
                _selectedGroups.DataSource = GroupManager.GetGroupMemberships(UserToEdit.Identity.Name);
                _selectedGroups.DataBind();
                /*
                _actualSelectedGroups.DataValueField = "ID";
                _actualSelectedGroups.DataTextField = "Name";
                _actualSelectedGroups.DataSource = GroupManager.GetGroupMemberships(UserToEdit.Identity.Name);
                _actualSelectedGroups.DataBind();*/
            }
        }

        /// <summary>
        /// Handles the databound event of the selected groups list
        /// - Removes selected groups from the available group list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectedGroupList_DataBound(object sender, EventArgs e)
        {
            if (UserToEdit != null)
            {
                //For every item in this list, remove it from the groups list
                foreach (ListItem item in _selectedGroups.Items)
                {
                    if (_availableGroupList.Items.Contains(item))
                    {
                        _availableGroupList.Items.Remove(item);
                    }
                }
            }
        }

         /// <summary>
        /// Handles the databound event of the available groups list
        /// - If there is no user being edited and there is only one available group, preselect it
        /// - trim the group name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AvailableGroupList_DataBound(object sender, EventArgs e)
        {
            var itemsToRemove = new List<string>();

            //trim names
            foreach (ListItem item in _availableGroupList.Items)
            {
                if (_selectedGroups.Items.FindByValue(item.Value) != null)
                {
                    itemsToRemove.Add(item.Value);
                }
                else
                {
                    item.Text = Utilities.TruncateText(item.Text, 50);
                }
            }

            //Remove items that are already in the selected list
            foreach (var itemToRemove in itemsToRemove)
            {
                var item = _availableGroupList.Items.FindByValue(itemToRemove);

                if (item != null)
                {
                    _availableGroupList.Items.Remove(item);
                }
            }

             if (UserToEdit == null)
            {
                //If the editor is not a system administrator, select the only available group (if one) by default
                var currentUser = HttpContext.Current.User as CheckboxPrincipal;

                if (!currentUser.IsInRole("System Administrator"))
                {
                    if (_availableGroupList.Items.Count == 1)
                    {
                        var itemToAdd = new ListItem(_availableGroupList.Items[0].Text, _availableGroupList.Items[0].Value);

                        _selectedGroups.Items.Add(itemToAdd);
                        _availableGroupList.Items.RemoveAt(0);
                    }
                }
            }
        }
    }
}