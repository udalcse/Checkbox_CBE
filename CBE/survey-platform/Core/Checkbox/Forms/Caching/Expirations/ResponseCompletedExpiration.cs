using System;

using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Caching.Expirations
{
    /// <summary>
    /// Expiration handler for response items
    /// </summary>
    [Serializable()]
    public class ResponseCompletedExpiration : ICacheItemExpiration
    {
        #region ICacheItemExpiration Members

        private bool _expired = false;

        /// <summary>
        /// Get whether the item has expired
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            return _expired;
        }

        /// <summary>
        /// Provide means for cache to notify that the owning cache item has been touched.
        /// </summary>
        public void Notify()
        {
        }

        /// <summary>
        /// Initialize the expiration. In this case, hook the response completed event
        /// </summary>
        /// <param name="owningCacheItem"></param>
        public void Initialize(CacheItem owningCacheItem)
        {
            try
            {
                if (!(owningCacheItem.Value is Response))
                {
                    throw new System.Exception("ResponseCompletedExpiration only valid for Response objects.");
                }

                if (owningCacheItem.Value != null)
                {
                    ((Response)owningCacheItem.Value).ResponseCompleted += new Response.ResponseCompletedHandler(ResponseCompletedExpiration_ResponseCompleted);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }
        }

        /// <summary>
        /// Handle response completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResponseCompletedExpiration_ResponseCompleted(object sender, ResponseStateEventArgs e)
        {
            _expired = true;

            ((Response)sender).ResponseCompleted -= new Response.ResponseCompletedHandler(ResponseCompletedExpiration_ResponseCompleted);
        }

        #endregion
    }
}
