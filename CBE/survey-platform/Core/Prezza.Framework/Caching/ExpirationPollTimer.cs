//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Threading;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Timer for determining when to poll the cache for expired items.
	/// </summary>
	internal class ExpirationPollTimer
	{
		/// <summary>
		/// System timer object.
		/// </summary>
		private Timer pollTimer = null;

		/// <summary>
		/// Start the polling timer.
		/// </summary>
		/// <param name="callbackMethod">Method to call on time out.</param>
		/// <param name="policyCycleInMilliseconds">Period of the timer.</param>
		public void StartPolling(TimerCallback callbackMethod, int policyCycleInMilliseconds)
		{
			if(callbackMethod == null)
			{
				throw new ArgumentException("callbackMethod");
			}

			if(policyCycleInMilliseconds <= 0)
			{
				throw new ArgumentException("Policy cycle must be > 0 milliseconds.", "policyCycleInMilliseconds");
			}

			pollTimer = new Timer(callbackMethod, null, policyCycleInMilliseconds, policyCycleInMilliseconds);
		}

		/// <summary>
		/// Stop the polling timer.
		/// </summary>
		public void StopPolling()
		{
			if(pollTimer == null)
			{
				throw new InvalidOperationException("Poll timer is not running.");
			}

			pollTimer.Dispose();
			pollTimer = null;			
		}
	}
}
