//===============================================================================
// Checkbox UI Controls
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Security;

namespace Checkbox.Web.Security
{
	/// <summary>
	/// Container class for information needed by the security editor control.
	/// </summary>
	[Serializable]
	public class SecurityEditorData
	{
	    /// <summary>
	    /// Get/set type of secured resource
	    /// </summary>
        public SecuredResourceType SecuredResourceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SecuredResourceName { get; set; }

        /// <summary>
        /// ID of secured resource
        /// </summary>
        public int SecuredResourceId { get; set; }

	    /// <summary>
	    /// Get/set the sitemap context
	    /// </summary>
	    public string Context { get; set; }

	    /// <summary>
	    /// Get/set permission required for access
	    /// </summary>
	    public string RequiredPermission { get; set; }

        /// <summary>
        /// Get/set permissions to grant on ACL
        /// </summary>
        public string AclPermissionsToGrant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SecuredResourceDefaultPolicyId { get; set; }
	}
}
