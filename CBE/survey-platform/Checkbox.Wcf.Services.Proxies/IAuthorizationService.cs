using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for authorizing access to secured resources
    /// </summary>
    [ServiceContract]
    public interface IAuthorizationService
    {
        /// <summary>
        /// Return a boolean indicating if the user is in a role with the specified permission
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> HasRolePermission(string userUniqueIdentifier, string permission);

        /// <summary>
        /// Return a boolean indicating if the specified user has the specified permission on the specified resource
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="securedResourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> IsAuthorized(string userUniqueIdentifier, SecuredResourceType securedResourceType, string resourceId, string permission);

        /// <summary>
        /// Return name value collection indicating whether specified permissions are met for the object.  Object "Names" are permissions and object "Values" are string "true" or "false" 
        /// value indicating whether permission is met.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="securedResourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionCsv"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SimpleNameValueCollection> BatchIsAuthorized(string userUniqueIdentifier, SecuredResourceType securedResourceType, string resourceId, string permissionCsv);
    }
}
