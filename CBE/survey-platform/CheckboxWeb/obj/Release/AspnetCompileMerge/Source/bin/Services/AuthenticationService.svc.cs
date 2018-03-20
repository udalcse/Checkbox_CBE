using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AuthenticationService : IAuthenticationService
    {
        #region Static Helper

        public const string NoUserMessage = "NoUser";

        /// <summary>
        /// Get current princpal by first inspecting ecrypted ticket and then falling back to current
        /// user context.
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        public static CheckboxPrincipal GetCurrentPrincipal(string encryptedTicket)
        {
            var user = UserManager.GetPrincipalByTicket(encryptedTicket);
            if (user != null)
                return user;

            //Fallback to current user
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User as CheckboxPrincipal;
            }

            return null;
        }


        #endregion

        #region IAuthenticationService Members

        /// <summary>
        /// Check if current user logged-in
        /// </summary>
        /// <returns></returns>
        public ServiceOperationResult<bool> IsLoggedIn()
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = HttpContext.Current.User.Identity.IsAuthenticated
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> Login(string userName, string password)
        {
            try
            {
                //Attempt to log the user in
                var user = UserManager.AuthenticateUser(userName, password);

                if (user == null)
                {
                    return new ServiceOperationResult<string>
                    {
                        CallSuccess = false,
                        ResultData = null,
                        FailureMessage = "Login failed for user [" + userName + "]"
                    };
                }

                //Create auth ticket
                string encryptedTicket;
                var ticket = UserManager.GenerateAuthenticationTicket(user, out encryptedTicket);

                //Store in cookie
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    Expires = ticket.Expiration,
                    HttpOnly = true,
                    Secure = HttpContext.Current.Request.IsSecureConnection
                };

                //Add cookie to response
                HttpContext.Current.Response.Cookies.Add(authCookie);

                //Otherwise, return encrypted authentication ticket.  Encrypted ticket is required for operations
                // against services.
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = encryptedTicket
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ServiceOperationResult<object> Logout()
        {
            try
            {
                FormsAuthentication.SignOut();

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> ValidateUser(string userName, string password)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = UserManager.AuthenticateUser(userName, password) != null
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion
    }
}