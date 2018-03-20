namespace Checkbox.Security.Providers
{
    /// <summary>
    /// Interface definition of a Checkbox role provider to allow role providers defined
    /// in web namespace to be used elsewhere.
    /// </summary>
    public interface ICheckboxRoleProvider
    {
        /// <summary>
        /// Add the specified users to the specified roles
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        void AddUsersToRoles(string[] userNames, string[] roleNames);

        /// <summary>
        /// Remove the specified users from the specified roles
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="roleNames"></param>
        void RemoveUsersFromRoles(string[] userNames, string[] roleNames);

        /// <summary>
        /// List users in a given role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        string[] GetUsersInRole(string roleName);

        /// <summary>
        /// List all system roles
        /// </summary>
        /// <returns></returns>
        string[] GetAllRoles();

        /// <summary>
        /// List roles for a given user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string[] GetRolesForUser(string userName);
    }
}
