// BootFX - Application framework for .NET applications
// 
// File: QueuingServiceEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Threading;
using System.Collections;

namespace BootFX.Common.Services
{
	/// <summary>
	/// Defines an engine that manages a thread-safe queue of items.
	/// </summary>
	public abstract class QueuingServiceEngine : ThreadedServiceEngine
	{
		/// <summary>
		/// Private field to support <c>ExpireThreadAt </c> property.
		/// </summary>
		private DateTime _expireThreadAt ;
		
		/// <summary>
		/// Private field to support <c>IsDisposing</c> property.
		/// </summary>
		private bool _isDisposing = false;
		
		/// <summary>
		/// Private field to support <c>Queue</c> property.
		/// </summary>
		private Queue _queue = new Queue();
		
		/// <summary>
		/// Private field to support <c>Lock</c> property.
		/// </summary>
		private ReaderWriterLock _lock = new ReaderWriterLock();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected QueuingServiceEngine() : base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected QueuingServiceEngine(TimeSpan tickPeriod) : base(tickPeriod)
		{
		}

		/// <summary>
		/// Queues a work item.
		/// </summary>
		/// <param name="item"></param>
		protected void Enqueue(object item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			// mbr - 07-11-2005 - stop?
			if(this.IsDisposing)
				throw new InvalidOperationException("The queue is being disposed.");

			// lock...
			Lock.AcquireWriterLock(-1);
			try
			{
				// add...
				this.Queue.Enqueue(item);
			}
			finally
			{
				Lock.ReleaseLock();
			}

			// tick now...
			this.Tick();
		}

		/// <summary>
		/// Gets the lock.
		/// </summary>
		private ReaderWriterLock Lock
		{
			get
			{
				// returns the value...
				return _lock;
			}
		}

		/// <summary>
		/// Gets the queue.
		/// </summary>
		private Queue Queue
		{
			get
			{
				// returns the value...
				return _queue;
			}
		}

		protected sealed override bool DoWork()
		{
			// loop through everything until we've dequeued the lot...
			int numDone = 0;
			while(!(this.IsCancelled))
			{
				// get a work item...
				object item = this.Dequeue();
			
				// anything to do?
				if(item != null)
				{
					numDone++;

					// try...
                    try
                    {
                        this.Status = ServiceEngineStatus.Active;
                        this.ProcessItem(item);
                    }
                    catch (Exception ex)
                    {
                        this.ItemFailed(item, ex);
                    }
                    finally
                    {
                        this.Status = ServiceEngineStatus.Idle;
                    }
				}
				else
					break;
			}

			// did we have any work?
			if(numDone == 0)
			{
				// are we expired?
				if(DateTime.UtcNow >= this.ExpireThreadAt)
				{
					// great - how do I tear down my own thread?
					return false;
				}
			}
			else
				_expireThreadAt = DateTime.UtcNow.AddMinutes(3);

			// ok...
			return true;
		}

		/// <summary>
		/// Gets the expirethreadat .
		/// </summary>
		private DateTime ExpireThreadAt 
		{
			get
			{
				// returns the value...
				return _expireThreadAt ;
			}
		}

		/// <summary>
		/// Called when an item needs to be processed.
		/// </summary>
		/// <param name="item"></param>
		protected abstract void ProcessItem(object item);

		/// <summary>
		/// Called when an item failed.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="ex"></param>
		protected virtual void ItemFailed(object item, Exception ex)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			if(ex == null)
				throw new ArgumentNullException("ex");

			// log...
			string message = string.Format("Failed when processing an item of type '{0}'.", item.GetType());
			if(this.Log.IsErrorEnabled)
				this.Log.Error(message, ex);
			
			// throw...
			throw new InvalidOperationException(message, ex);
		}

		/// <summary>
		/// Dequeues the given work item.
		/// </summary>
		/// <returns></returns>
		private object Dequeue()
		{
			Lock.AcquireWriterLock(-1);
			try
			{
				if(this.Queue.Count > 0)
					return this.Queue.Dequeue();
				else
					return null;
			}
			finally
			{
				Lock.ReleaseLock();
			}
		}

		protected override void OnDisposing(EventArgs e)
		{
			base.OnDisposing (e);

			// set (to stop more items being queued)...
			_isDisposing = true;

			// wait around...
			const int ticksPerSecond = 4;
			int totalTicks = ticksPerSecond * 30;
			for(int index = 0; index < totalTicks; index++)
			{
				// work?
				if(!(this.HasWork) || !(this.IsThreadAlive))
					break;

				// log...
				if(this.Log.IsInfoEnabled)
					this.Log.Info(string.Format("'{0}' has work to do - opportunity {1} of {2}...", this, index, totalTicks));

				// give it a last chance...
				this.Tick();

				// sleep...
				Thread.Sleep(1000 / ticksPerSecond);
			}
		}

		/// <summary>
		/// Gets the isdisposing.
		/// </summary>
		private bool IsDisposing
		{
			get
			{
				// returns the value...
				return _isDisposing;
			}
		}

		/// <summary>
		/// Gets the number of items in the queue.
		/// </summary>
		public int Count
		{
			get
			{
				Lock.AcquireReaderLock(-1);
				try
				{
					return this.Queue.Count;
				}
				finally
				{
					Lock.ReleaseLock();
				}
			}
		}

		private bool HasWork
		{
			get
			{
				Lock.AcquireReaderLock(-1);
				try
				{
					if(this.Queue.Count > 0)
						return true;
					else
						return false;
				}
				finally
				{
					Lock.ReleaseLock();
				}
			}
		}

        public object[] PeekQueue()
        {
			Lock.AcquireReaderLock(-1);
            try
            {
                return this.Queue.ToArray();
            }
            finally
            {
                Lock.ReleaseLock();
            }
        }
	}
}
