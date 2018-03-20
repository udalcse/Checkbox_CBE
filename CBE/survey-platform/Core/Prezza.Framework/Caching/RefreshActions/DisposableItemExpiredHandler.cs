//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Text;

using Prezza.Framework.Caching;

namespace Prezza.Framework.Caching.RefreshActions
{
    /// <summary>
    /// Create handler for when the disposable item expires from the cache.  Sets item to null and
    /// calls dispose.
    /// </summary>
    [Serializable()]
    public class DisposableItemExpiredHandler : ICacheItemRefreshAction
    {
        #region ICacheItemRefreshAction Members

        /// <summary>
        /// Handle cache expiration
        /// </summary>
        /// <param name="removedKey"></param>
        /// <param name="expiredValue"></param>
        /// <param name="removalReason"></param>
        public void Refresh(string removedKey, object expiredValue, CacheItemRemovedReason removalReason)
        {
            if (expiredValue is IDisposable && expiredValue != null)
            {
                ((IDisposable)expiredValue).Dispose();
                expiredValue = null;
            }
        }

        #endregion
    }
}
