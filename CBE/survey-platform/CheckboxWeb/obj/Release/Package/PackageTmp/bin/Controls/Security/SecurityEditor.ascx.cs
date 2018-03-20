using System.Web.UI;
using Checkbox.Security.Principal;
using Prezza.Framework.Security;

namespace CheckboxWeb.Controls.Security
{
    public partial class SecurityEditor : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get a reference to the acl editor which can be used to set properties declaratively
        /// on the editor using dash syntax. e.g. AclEditor-ShowInstructions=false
        /// </summary>
        public AccessListEditor AclEditor { get { return _aclEditor; } }

        /// <summary>
        /// Initialize the control with the IAccessControllable resource
        /// to edit and the user performing the edits.
        /// </summary>
        /// <param name="accessControllableResource"></param>
        /// <param name="userPrincipal"></param>
        public void Initialize(CheckboxPrincipal userPrincipal, IAccessControllable accessControllableResource)
        {
            
        }
    }
}