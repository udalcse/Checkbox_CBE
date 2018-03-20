//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Configuration;
using Prezza.Framework.Security.Principal;

namespace Prezza.Framework.Security
{
	/// <summary>
	///  Defines the interface that authorization providers must support.
	/// </summary>
	public interface IAuthorizationProvider : IConfigurationProvider
	{
		/// <summary>
		/// Authorize the specified principal against the specified
		/// context.
		/// </summary>
		/// <param name="principal">the <see cref="Prezza.Framework.Security.Principal.ExtendedPrincipal"/> to authorize.</param>
		/// <param name="context">Context to authorize the principal against.</param>
		/// <returns>True if authorization is given, false otherwise.</returns>
		bool Authorize(ExtendedPrincipal principal, string context);

		/// <summary>
		/// Authorize the specified principal to access a given resource against the specified context.
		/// </summary>
		/// <param name="principal">the <see cref="Prezza.Framework.Security.Principal.ExtendedPrincipal"/> to authorize.</param>
		/// <param name="resource">the <see cref="IAccessControllable"/> resource to authorize against</param>
		/// <param name="context">Context to authorize the principal against.</param>
		/// <returns>True if authorization is given, false otherwise.</returns>
		bool Authorize(ExtendedPrincipal principal, IAccessControllable resource, string context);

        /// <summary>
        /// Get a string identifying an auth failure type
        /// </summary>
        /// <returns></returns>
        string GetAuthorizationErrorType();
	}
}
