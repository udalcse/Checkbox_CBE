using System;
using Checkbox.Security.Principal;
using Checkbox.Users;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base page class for pages that allow viewing or editing users
    /// </summary>
    public abstract class EditUserPage : SecuredPage
    {
        private CheckboxPrincipal _userToEdit;
        private bool? _isReadOnly;

        /// <summary>
        /// 
        /// </summary>
        public CheckboxPrincipal UserToEdit { 
            get
            {
                if (_userToEdit == null)
                {
                    if (UserGuid.HasValue)
                    {
                        _userToEdit = UserManager.GetUserByGuid(UserGuid.Value);
                    }
                    else if (!string.IsNullOrEmpty(ExternalUserId))
                    {
                        _userToEdit = UserManager.GetUserPrincipal(ExternalUserId.Replace("_BACKSLASH_", @"\"));
                    }
                    else
                    {
                        throw new Exception("Checkbox user guid or external user id not specified.");
                    }

                    if (_userToEdit == null)
                    {
                        throw new Exception("Unable to load specified user.");
                    }
                }

                return _userToEdit;
            }
        }
        /// <summary>
        /// Guid identifying user.  This value will be null for external (network) users.
        /// </summary>
        [QueryParameter("u")]
        public Guid? UserGuid { get; set; }

        /// <summary>
        /// ID of external user.  This value will be null or empty for users that
        /// exist in the Checkbox user store.
        /// </summary>
        [QueryParameter("e")]
        public string ExternalUserId { get; set; }

        /// <summary>
        /// Get whether current user is an external user
        /// </summary>
        public bool IsExternalUser { get { return UserManager.EXTERNAL_USER_AUTHENTICATION_TYPE.Equals(UserToEdit.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Get whether current user is an external user
        /// </summary>
        public bool IsCheckboxNetworkUser { get { return UserManager.NETWORK_USER_AUTHENTICATION_TYPE.Equals(UserToEdit.Identity.AuthenticationType, StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Require View User Group permission to access this page.  Further checks will be made to ensure logged-in user
        /// has access to view/edit this user.
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Group.View"; } }

        /// <summary>
        /// Return a value indicating if user can be edited.
        /// </summary>
        public bool IsUserReadOnly
        {
            get
            {
                if (!_isReadOnly.HasValue)
                {
                    _isReadOnly = !UserManager.CanCurrentPrincipalEditUser(UserToEdit.Identity.Name);
                }

                return _isReadOnly.Value;
            }
        }

        /// <summary>
        /// Return a value indicating if user can be edited.
        /// </summary>
        public bool CanViewUser
        {
            get
            {
                return UserManager.CanCurrentPrincipalEditUser(UserToEdit.Identity.Name)
                       || UserManager.CanCurrentPrincipalViewUser(UserToEdit.Identity.Name);
            }
        }

        /// <summary>
        /// Cause user to be reloaded
        /// </summary>
        public void ReloadUser()
        {
            //Set user to null, which will cause reload on next access
            _userToEdit = null;
        }

        /// <summary>
        /// Authorize current page.
        /// </summary>
        /// <returns></returns>
        protected override bool AuthorizePage()
        {
            //If user can't be viewed, then auth fail
            if (!CanViewUser)
            {
                return false;
            }

            return base.AuthorizePage();
        }
    }
}
