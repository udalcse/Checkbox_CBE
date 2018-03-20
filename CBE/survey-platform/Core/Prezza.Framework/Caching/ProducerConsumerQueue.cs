//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;
using System.Threading;

namespace Prezza.Framework.Caching
{
	/// <summary>
	/// Thread safe queuing, used by cache scavenging processes.
	/// </summary>
	internal class ProducerConsumerQueue
	{
		/// <summary>
		/// Lock object.
		/// </summary>
		private object lockObject = new Object();

		/// <summary>
		/// Queue of objects.
		/// </summary>
		private Queue queue = new Queue();

		/// <summary>
		/// Number of queued items.
		/// </summary>
		public int Count
		{
			get { return queue.Count; }
		}
        
		/// <summary>
		/// Get the next item in the queue.
		/// </summary>
		/// <returns>Next object in queue.</returns>
		public object DeQueue()
		{
			lock(lockObject)
			{
				while(queue.Count == 0)
				{
					if(WaitUntilInterrupted())
					{
						return null;
					}
				}

				return queue.Dequeue();
			}
		}

		/// <summary>
		/// Add an object to the queue.
		/// </summary>
		/// <param name="o">Object to add to the queue.</param>
		public void Enqueue(object o)
		{
			lock(lockObject)
			{
				queue.Enqueue(o);
				Monitor.Pulse(lockObject);
			}
		}

		/// <summary>
		/// Wait for the lockObject to become unlocked OR for a ThreadInterruptException to occur.
		/// </summary>
		/// <returns></returns>
		private bool WaitUntilInterrupted()
		{
			try
			{
				Monitor.Wait(lockObject);
			}
			catch(ThreadInterruptedException)
			{
				return true;
			}

			return false;
		}
	}
}
