//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Prezza.Framework.Caching.Expirations
{
    /// <summary>
    ///	This provider tests if a item was expired using a time slice schema.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class SlidingTime : ICacheItemExpiration, ISerializable
    {
        private DateTime _timeLastUsed;
        private readonly TimeSpan _itemSlidingExpiration;

        /// <summary>
        ///	Create an instance of this class with the timespan for expiration.
        /// </summary>
        /// <param name="slidingExpiration">
        ///	Expiration time span
        /// </param>
        public SlidingTime(TimeSpan slidingExpiration)
        {
            // Check that expiration is a valid numeric value
            if (!(slidingExpiration.TotalSeconds >= 1))
            {
                throw new ArgumentOutOfRangeException("slidingExpiration", "Total seconds value [" + slidingExpiration.TotalSeconds + "] is invalid.");
            }

            _itemSlidingExpiration = slidingExpiration;
        }

        /// <devdoc>
        /// This constructor is for testing purposes only. Never, ever call it in a real program
        /// </devdoc>
        internal SlidingTime(TimeSpan slidingExpiration, DateTime originalTimeStamp)
            : this(slidingExpiration)
        {
            _timeLastUsed = originalTimeStamp;
        }

        /// <summary>
        ///	This method performs the deserialziaton of members of the current 
        ///	class.
        /// </summary>
        /// <param name="info">
        /// A SerializationInfo object which is deserialized by the formatter 
        ///	and then passed to current constructor
        /// </param>
        /// <param name="context">
        ///	A StreamingContext that describes the source of the serialized 
        ///	stream from where the Serialization object is retrieved
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SlidingTime(SerializationInfo info, StreamingContext context)
        {
            _timeLastUsed = Convert.ToDateTime(info.GetValue("lastUsed", typeof(DateTime)),
                DateTimeFormatInfo.CurrentInfo);
            _itemSlidingExpiration = (TimeSpan)info.GetValue("slidingExpiration", typeof(TimeSpan));
        }

        /// <summary>
        /// Returns sliding time window that must be exceeded for expiration to occur
        /// </summary>
        public TimeSpan ItemSlidingExpiration
        {
            get { return _itemSlidingExpiration; }
        }

        /// <summary>
        /// Returns time that this object was last touched
        /// </summary>
        public DateTime TimeLastUsed
        {
            get { return _timeLastUsed; }
        }

        /// <summary>
        ///	This method performs the serialziaton of members of the current 
        ///	class.
        /// </summary>
        /// <param name="info">
        ///	A SerializationInfo object which is deserialized by the formatter 
        ///	and then passed to current constructor
        /// </param>
        /// <param name="context">
        ///	A StreamingContext that describes the source of the serialized 
        ///	stream from where the Serialization object is retrieved
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("slidingExpiration", _itemSlidingExpiration);
            info.AddValue("lastUsed", _timeLastUsed);
        }

        /// <summary>
        ///	Specifies if item has expired or not.
        /// </summary>
        /// <returns>Returns true if the item has expired otherwise false.</returns>
        public bool HasExpired()
        {
            bool expired = CheckSlidingExpiration(DateTime.Now,
                _timeLastUsed,
                _itemSlidingExpiration);
            return expired;
        }

        /// <summary>
        ///	Notifies that the item was recently used.
        /// </summary>
        public void Notify()
        {
            _timeLastUsed = DateTime.Now;
        }

        /// <summary>
        /// Used to set the initial value of TimeLastUsed. This method is invoked during the reinstantiation of
        /// an instance from a persistent store. 
        /// </summary>
        /// <param name="owningCacheItem">CacheItem to which this expiration belongs.</param>
        public void Initialize(CacheItem owningCacheItem)
        {
            _timeLastUsed = owningCacheItem.LastAccessedTime;
        }

        /// <summary>
        ///	Check whether the sliding time has expired.
        /// </summary>
        /// <param name="nowDateTime">Current time </param>
        /// <param name="lastUsed">The last time when the item has been used</param>
        /// <param name="slidingExpiration">The span of sliding expiration</param>
        /// <returns>True if the item was expired, otherwise false</returns>
        private static bool CheckSlidingExpiration(DateTime nowDateTime,
            DateTime lastUsed,
            TimeSpan slidingExpiration)
        {
            /*******************************************************
             * Commenting out the use of ToUniversalTime() as this is 
             * a performance bottleneck.  Since we're not using string
             * representations of dates anymore, shouldn't need this call.
             * *****************************************************/
            //// Convert to UTC in order to compensate for time zones
            //DateTime tmpNowDateTime = nowDateTime.ToUniversalTime();
            //// Convert to UTC in order to compensate for time zones
            //DateTime tmpLastUsed = lastUsed.ToUniversalTime();

            //long expirationTicks = tmpLastUsed.Ticks + slidingExpiration.Ticks;

            //bool expired = (tmpNowDateTime.Ticks >= expirationTicks) ? true : false;

            long expirationTicks = lastUsed.Ticks + slidingExpiration.Ticks;
            return (nowDateTime.Ticks >= expirationTicks) ? true : false;
        }
    }
}
