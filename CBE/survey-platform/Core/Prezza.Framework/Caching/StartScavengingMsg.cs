//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Message to start cache scavenging.
	/// </summary>
	internal class StartScavengingMsg : IQueueMessage
	{
		/// <summary>
		/// Background schdeduler callback.
		/// </summary>
		private BackgroundScheduler callback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="callback">Background scheduler callback.</param>
		public StartScavengingMsg(BackgroundScheduler callback)
		{
			this.callback = callback;
		}
		#region IQueueMessage Members

		/// <summary>
		/// Run the scavenging.
		/// </summary>
		public void Run()
		{
			callback.DoStartScavenging();
		}

		#endregion
	}
}
