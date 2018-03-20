using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    [ServiceContract]
    public interface ISecurityManagementService
    {
        #region Acl Operations

        /// <summary>
        /// List entries on an access list.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<AclEntry[]>> GetAclEntries(string authTicket, SecuredResourceType resourceType, int resourceId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue);

        /// <summary>
        /// List entries that could possibly be added to an access list.  This list includes all users current user can view that are not already
        /// on the access list.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="provider"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionToGrant"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<AclEntry[]>> GetAvailableEntries(string authTicket, string provider, SecuredResourceType resourceType, int resourceId, string permissionToGrant, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue);

        /// <summary>
        /// List entries that could possibly be added to an access list.  This list includes all users current user can view that are not already
        /// on the access list.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionToCheck"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<AclEntry[]>> GetCurrentEntries(string authTicket, SecuredResourceType resourceType, int resourceId, string permissionToCheck, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue);

        /// <summary>
        /// List entries on an access list.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="permissionToGrant"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddEntryToAcl(string authTicket, SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier, string permissionToGrant);

        /// <summary>
        /// List entries on an access list.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> RemoveEntryFromAcl(string authTicket, SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier);

        #endregion

        #region Permissions

        /// <summary>
        /// Get masked list of permissions for policy
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PermissionEntry[]> GetPolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId);

        /// <summary>
        /// Get masked list of permissions for policy
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="resourceType"></param>
        /// <param name="permissionMasks"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdatePolicyMaskedPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId, string[] permissionMasks);

        /// <summary>
        /// Get masked list of permissions for policy
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="resourceType"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> UpdatePolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId, string[] permissions);

        /// <summary>
        /// Get masked list of permissions for policy
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PermissionMaskEntry[]> GetMaskedPolicyPermissions(string authTicket, SecuredResourceType resourceType, int resourceId, int policyId);

        #endregion 
    }
}
