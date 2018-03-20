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
	///	This class tests if a data item was expired using a absolute time 
	///	schema.
	/// </summary>
	[Serializable]
	[ComVisible(false)]

	public class AbsoluteTime : ICacheItemExpiration, ISerializable
	{
		private DateTime absoluteExpirationTime;
        /// <summary>
        /// Absolute time of expiration
        /// </summary>
        public DateTime AbsoluteExpirationTime
        {
            get
            {
                return absoluteExpirationTime;
            }
        }

		/// <summary>
		///	Create a new instance of the class.
		/// </summary>
		public AbsoluteTime()
		{
		}

		/// <summary>
		///	Create an instance of the class with a time value as input and 
		///	convert it to UTC.
		/// </summary>
		/// <param name="absoluteTime">
		///	The time to be checked for expiration
		/// </param>
		public AbsoluteTime(DateTime absoluteTime)
		{
			if (absoluteTime > DateTime.Now)
			{
				// Convert to UTC in order to compensate for time zones	
				this.absoluteExpirationTime = absoluteTime.ToUniversalTime();
			}
			else
			{
				throw new ArgumentOutOfRangeException("absoluteTime", "Specified time [" + absoluteTime.ToString() + "] was out of range.");
			}
		}

		/// <summary>
		/// Creates an instance based on a time interval starting from now.
		/// </summary>
		/// <param name="timeFromNow">Time interval</param>
		public AbsoluteTime(TimeSpan timeFromNow) : this(DateTime.Now + timeFromNow)
		{
		}

		/// <summary>
		///	This method performs the deserialziaton of members of the current 
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
		protected AbsoluteTime(SerializationInfo info, StreamingContext context)
		{
			this.absoluteExpirationTime = Convert.ToDateTime(
				info.GetValue("absoluteExpiration", typeof(DateTime)),
				DateTimeFormatInfo.CurrentInfo);
		}

		/// <summary>
		///	Specifies if item has expired or not.
		/// </summary>
		/// <remarks>
		///	bool isExpired = ICacheItemExpiration.HasExpired();
		/// </remarks>
		/// <returns>
		///	"True", if the data item has expired or "false", if the data item 
		///	has not expired
		/// </returns>
		public bool HasExpired() //ICacheItemExpiration
		{
			// Convert to UTC in order to compensate for time zones		
			DateTime nowDateTime = DateTime.Now.ToUniversalTime();

			// Check expiration
			return nowDateTime.Ticks >= this.absoluteExpirationTime.Ticks;
		}

		/// <summary>
		///	Called to notify this object that the CacheItem owning this expiration was just touched by a user action
		/// </summary>
		public void Notify()
		{
		}

		/// <summary>
		/// Called to give this object an opportunity to initialize itself from data inside a CacheItem
		/// </summary>
		/// <param name="owningCacheItem">CacheItem provided to read initialization information from. Will never be null.</param>
		public void Initialize(CacheItem owningCacheItem)
		{
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
			info.AddValue("absoluteExpiration", this.absoluteExpirationTime);
		}
	}
}
