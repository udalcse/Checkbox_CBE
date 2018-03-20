//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Provider for getting and setting security tokens for user sessions in the product.
	/// </summary>
	public interface ISessionTokenProvider : IConfigurationProvider
	{
        /// <summary>
        /// Get the current session token.
        /// </summary>
        /// <returns></returns>
		IToken GetSessionToken();

        /// <summary>
        /// Set the token for the current session.
        /// </summary>
        /// <param name="token"></param>
        void SetSessionToken(IToken token);

        /// <summary>
        /// Clear the session token.
        /// </summary>
        void ClearSessionToken();
	}
}
