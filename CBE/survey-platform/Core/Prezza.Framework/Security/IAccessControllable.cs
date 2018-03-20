//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Security
{
    /// <summary>
    /// Defines the interface that Access Controllable resources must support.
    /// </summary>
    public interface IAccessControllable
    {
        /// <summary>
        /// The default <see cref="Policy"/> for this resource
        /// </summary>
        Policy DefaultPolicy { get; }

        /// <summary>
        /// The <see cref="AccessControlList"/> for this resource
        /// </summary>
        IAccessControlList ACL { get; }

        /// <summary>
        /// Factory method creates <see cref="Policy"/> of Type appropriate to IAccessControllable implementation
        /// </summary>
        /// <param name="permissions">the permissions for the <see cref="Policy"/></param>
        /// <returns></returns>
        Policy CreatePolicy(string[] permissions);

        /// <summary>
        /// Gets a list of supported permissions
        /// </summary>
        string[] SupportedPermissions { get; }

        /// <summary>
        /// Get a list of supported permission masks
        /// </summary>
        string[] SupportedPermissionMasks { get; }

        /// <summary>
        /// Get the <see cref="SecurityEditor"/> for the IAccessControllable entity.
        /// </summary>
        /// <returns></returns>
        SecurityEditor GetEditor();

        /// <summary>
        /// Get controllable entity name
        /// </summary>
        string Name { get; }
    }
}