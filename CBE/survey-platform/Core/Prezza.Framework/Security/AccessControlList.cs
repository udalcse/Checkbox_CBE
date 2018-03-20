//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// A collection of IAccessControlEntry objects associated with IAccessControllable resources.
    /// </summary>
    [Serializable]
    public class AccessControlList : AbstractAccessControlList
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AccessControlList()
        {
        }

        /// <summary>
        /// Construct acl with id
        /// </summary>
        /// <param name="aclId"></param>
        public AccessControlList(int aclId)
            : base(aclId)
        {
        }
    }
}
