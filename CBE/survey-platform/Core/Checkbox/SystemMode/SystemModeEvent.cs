using Checkbox.Users;

namespace Checkbox.SystemMode
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemModeEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemModeEventType EventType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string User {
            get { return UserManager.GetCurrentPrincipal().Identity.Name?? "FailToGetName"; }
        }
    }
}