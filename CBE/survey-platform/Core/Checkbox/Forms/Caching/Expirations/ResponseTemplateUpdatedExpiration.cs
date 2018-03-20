using System;

using Prezza.Framework.Caching;

namespace Checkbox.Forms.Caching.Expirations
{
    /// <summary>
    /// Expiration handler for expiring response items when their response template changes
    /// </summary>
    [Serializable()]
    public class ResponseTemplateUpdatedExpiration : ICacheItemExpiration
    {
        private bool _expired = false;

        /// <summary>
        /// Constructur, which binds the rt saved event
        /// </summary>
        /// <param name="rt"></param>
        public ResponseTemplateUpdatedExpiration(ResponseTemplate rt)
        {
            if (rt != null)
            {
                //Bind the new handler
                rt.Updated += new TemplateUpdated(rt_Updated);
            }
        }

        /// <summary>
        /// Handle template saved by expiring cache item
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        void rt_Updated(Template source, EventArgs e)
        {
            //Set the expired flag
            _expired = true;

            //Unbind the event
            source.Updated -= new TemplateUpdated(rt_Updated);
        }


        #region ICacheItemExpiration Members

        /// <summary>
        /// Get whether the item has expired
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            return _expired;
        }

        /// <summary>
        /// Allow cache to notify that owning item has been updated
        /// </summary>
        public void Notify()
        {
        }

        /// <summary>
        /// Initialize with the owning item.  Does nothing since the expiration is
        /// dependent on the response template the cacheitem was initialized with
        /// </summary>
        /// <param name="owningCacheItem"></param>
        public void Initialize(CacheItem owningCacheItem)
        {
        }

        #endregion
    }
}
