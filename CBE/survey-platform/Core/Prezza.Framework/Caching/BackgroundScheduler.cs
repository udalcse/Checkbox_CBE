//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Threading;

using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Scheduler for background cache scavenging.
	/// </summary>
	internal class BackgroundScheduler : ICacheScavenger
	{
		private ProducerConsumerQueue inputQueue = new ProducerConsumerQueue();
		private Thread inputQueueThread;
		private ExpirationTask expirer;
		private ScavengerTask scavenger;
		private bool isActive = false;
		private bool running = false;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="expirer">Task for expiration.</param>
		/// <param name="scavenger">Cache scavenger task.</param>
		public BackgroundScheduler(ExpirationTask expirer, ScavengerTask scavenger)
		{
			this.expirer = expirer;
			this.scavenger = scavenger;

			ThreadStart queueReader = new ThreadStart(QueueReader);
			inputQueueThread = new Thread(queueReader);
			inputQueueThread.IsBackground = true;
		}
		#region ICacheScavenger Members

		/// <summary>
		/// Start servicing the queue.
		/// </summary>
		public void Start()
		{
			running = true;
			inputQueueThread.Start();
		}

		/// <summary>
		/// Stop servicing the queue.
		/// </summary>
		public void Stop()
		{
			running = false;
			inputQueueThread.Interrupt();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="notUsed"></param>
		public void ExpirationTimeoutExpired(object notUsed)
		{
			inputQueue.Enqueue(new ExpirationTimeoutExpiredMsg(this));
		}

		/// <summary>
		/// Indicate that the scheduler is active.
		/// </summary>
		internal bool IsActive
		{
			get{return isActive;}
		}

		/// <summary>
		/// Enqueue a scavenging task.
		/// </summary>
		public void StartScavenging()
		{
			inputQueue.Enqueue(new StartScavengingMsg(this));			
		}

		/// <summary>
		/// Have the scavenger start scavenging.
		/// </summary>
		public void DoStartScavenging()
		{
			scavenger.DoScavenging();
		}


		/// <summary>
		/// Handle expiration timeouts
		/// </summary>
		internal void DoExpirationTimeoutExpired()
		{
			expirer.DoExpirations();
		}

		/// <summary>
		/// Read the queue and run tasks
		/// </summary>
		private void QueueReader()
		{
			isActive = true;

			while(running)
			{
				IQueueMessage msg = inputQueue.DeQueue() as IQueueMessage;

				try
				{
					if(msg == null)
					{
						continue;
					}

					msg.Run();
				}
				catch(ThreadInterruptedException)
				{
					//Do nothing so we just stop reading the queue
					//Logger.Write("Background scheduler Queue Reader thread interrupted.", "Info", 1);
				}
				catch(Exception ex)
				{
					//CachingServiceInternalFailureEvent.Fire("An error occurred in the background producer/consumer queue processing.", ex);
					//Logger.Write("An error occurred while processing the background producer/consumer queue.", "Error", 5);
					ExceptionPolicy.HandleException(ex, "FrameworkCache");
				}
			}

			isActive = false;
		}
		#endregion
	}
}
