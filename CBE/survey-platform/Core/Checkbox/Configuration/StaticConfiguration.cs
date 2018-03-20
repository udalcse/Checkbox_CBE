using System.Diagnostics;

namespace Checkbox.Configuration
{
    /// <summary>
    /// Stores unchangable settings
    /// </summary>
    public static class StaticConfiguration
    {
        /// <summary>
        /// Restricts using of any providers except native CheckboxMembershipProviders
        /// </summary>
        public static bool DisableForeighMembershipProviders
        {
            //toggle to allow/restrict AD users
            get
            {
                return true; 
            }
        }
    }
}
