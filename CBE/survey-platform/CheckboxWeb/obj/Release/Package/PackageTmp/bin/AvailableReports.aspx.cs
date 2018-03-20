using System.Text;
using Checkbox.Common;
using Checkbox.Users;

namespace CheckboxWeb
{
    public partial class AvailableReports : Checkbox.Web.Page.BasePage
    {
        /// <summary>
        /// Gets the "u=.." part of the survey URL if it is needed
        /// </summary>
        protected string UserIDSection
        {
            get
            {
                var sb = new StringBuilder();
                //Add user guid, if a user is logged in
                if (UserManager.GetCurrentPrincipal() != null)
                {
                    string guidString = UserManager.GetCurrentPrincipal().UserGuid.ToString();

                    if (Utilities.IsNotNullOrEmpty(guidString))
                    {
                        sb.Append("&u=");
                        sb.Append(guidString.Replace("-", string.Empty));
                    }
                }
                return sb.ToString();

            }
        }
    }
}
