using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for simple authentication service to validate users and create forms authentication tickets.
    /// </summary>
    [ServiceContract]
    public interface IAuthenticationService
    {
        /// <summary>
        /// Get whether the caller is currently logged-in.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> IsLoggedIn();

        /// <summary>
        /// Log a user in and associated forms auth cookie with user. 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<string> Login(string userName, string password);

        /// <summary>
        /// Log current user out.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> Logout();

        /// <summary>
        /// Validate a user's credentials without logging the user
        /// in or creating an associated forms authentication cookie.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> ValidateUser(string userName, string password);
    }
}
