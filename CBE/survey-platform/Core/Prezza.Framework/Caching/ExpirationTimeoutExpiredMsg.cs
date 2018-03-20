//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Simple queue message to handling expiration timeouts.
	/// </summary>
	internal class ExpirationTimeoutExpiredMsg : IQueueMessage
	{
		private BackgroundScheduler callback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="callback">Callback object</param>
		public ExpirationTimeoutExpiredMsg(BackgroundScheduler callback)
		{
			this.callback = callback;
		}
		#region IQueueMessage Members

		/// <summary>
		/// Run expired process.
		/// </summary>
		public void Run()
		{
			callback.DoExpirationTimeoutExpired();
		}

		#endregion
	}
}
