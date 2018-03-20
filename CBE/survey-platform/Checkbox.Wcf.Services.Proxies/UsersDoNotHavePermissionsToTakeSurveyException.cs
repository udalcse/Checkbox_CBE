using System;
using System.Collections.Generic;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UsersDoNotHavePermissionsToTakeSurveyException : Exception
    {
        /// <summary>
        /// Exception to verify the existence of the user's email.
        /// </summary>
        public UsersDoNotHavePermissionsToTakeSurveyException(IEnumerable<string> uniqueIdentifiers)
            : base("Following users do not have sufficient permissions to take the survey: " + string.Join(", ", uniqueIdentifiers))
        {
        }
    }
}
