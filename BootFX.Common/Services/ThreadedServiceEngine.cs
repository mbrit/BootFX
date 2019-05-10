// BootFX - Application framework for .NET applications
// 
// File: ThreadedServiceEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Threading;

namespace BootFX.Common.Services
{
	/// <summary>
	/// Summary description for <see cref="ThreadedServiceEngine"/>.
	/// </summary>
	public abstract class ThreadedServiceEngine : ServiceEngine
	{
		/// <summary>
		/// Private field to support <see cref="WorkStartedAt"/> property.
		/// </summary>
		private DateTime _workStartedAt = DateTime.MinValue;
		private object _workStatedAtLock = new object();
		
		private object _threadLock = new object();

		/// <summary>
		/// Raised when the thread is about to be disposed.
		/// </summary>
		public event EventHandler ThreadDisposing;

		/// <summary>
		/// Raised when the thread has been disposed.
		/// </summary>
		public event EventHandler ThreadDisposed;
		
		/// <summary>
		/// Private field to support <c>Thread</c> property.
		/// </summary>
		private Thread _thread;
		
		/// <summary>
		/// Private field to support <c>TickPeriod</c> property.
		/// </summary>
		private TimeSpan _tickPeriod;
		
		/// <summary>
		/// Private field to support <c>TickEvent</c> property.
		/// </summary>
		private ManualResetEvent _tickEvent = new ManualResetEvent(false);
		
		/// <summary>
		/// Private field to support <c>IsCancelled</c> property.
		/// </summary>
		private bool _isCancelled = false;
		
		/// <summary>
		/// Creates a new instance of <see cref="ThreadedServiceEngine"/> with a default timeout of thirty seconds.
		/// </summary>
		protected ThreadedServiceEngine() : this(new TimeSpan(0, 0, 30))
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="ThreadedServiceEngine"/>.
		/// </summary>
		protected ThreadedServiceEngine(TimeSpan tickPeriod)
		{
			_tickPeriod = tickPeriod;
		}

		/// <summary>
		/// Ensures that the thread is initialized.
		/// </summary>
		public void EnsureThreadInitialized()
		{
			lock(_threadLock)
			{
				if(_thread == null || !(_thread.IsAlive))
					this.InitializeThread();
			}
		}

		/// <summary>
		/// Initializes the thread.
		/// </summary>
		private void InitializeThread()
		{
			lock(_threadLock)
			{
				if(_thread != null && _thread.IsAlive)
					throw new InvalidOperationException("Thread is already running.");

				// create...
				_thread = new Thread(new ThreadStart(ThreadEntryPoint));
				_thread.IsBackground = true;

                // create a name - we add the hash code because VS .NET can get
                // confused when switching between threads with the same name...  also
                // this saves us from getting confused...
                //_thread.Name = string.Format("{0} Thread ({1})", this, this.GetHashCode());
                _thread.Name = this.Name;

				// start...
				_thread.Start();
			}
		}

		protected override void DoDispose(DisposeReason reason)
		{
			base.DoDispose (reason);

			// thread...
			DisposeThread();
		}

        /// <summary>
		/// Disposes the thread.
		/// </summary>
        // mbr - 2009-01-08 - added wait...
        private void DisposeThread()
        {
            this.DisposeThread(new TimeSpan(0, 0, 30));
        }

		/// <summary>
		/// Disposes the thread.
		/// </summary>
        // mbr - 2009-01-08 - added removal of timeout...
		private void DisposeThread(TimeSpan waitBeforeAbort)
		{
			lock(_threadLock)
			{
				if(_thread == null)
					return;

				// stop...
				if(this.Log.IsInfoEnabled)
					this.Log.Info("Disposing thread...");

				// check...
				if(_thread == Thread.CurrentThread)
					throw new InvalidOperationException("Cannot dispose thread from itself as this would cause a deadlock.");

				try
				{
					// disposing...
					this.OnThreadDisposing();

					// set...
					_isCancelled = true;

					// tick...
					this.Tick();

					// wait...
					if(!(_thread.Join(waitBeforeAbort)))
						_thread.Abort();
				}
				finally
				{
					// reset...
					_thread = null;

					// log...
					if(this.Log.IsInfoEnabled)
						this.Log.Info("Thread disposed.");

					// raise...
					this.OnThreadDisposed();
				}
			}
		}

		/// <summary>
		/// Raises the <c>ThreadDisposed</c> event.
		/// </summary>
		private void OnThreadDisposed()
		{
			OnThreadDisposed(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ThreadDisposed</c> event.
		/// </summary>
		protected virtual void OnThreadDisposed(EventArgs e)
		{
			// raise...
			if(ThreadDisposed != null)
				ThreadDisposed(this, e);
		}
		
		/// <summary>
		/// Raises the <c>ThreadDisposing</c> event.
		/// </summary>
		private void OnThreadDisposing()
		{
			OnThreadDisposing(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ThreadDisposing</c> event.
		/// </summary>
		protected virtual void OnThreadDisposing(EventArgs e)
		{
			// raise...
			if(ThreadDisposing != null)
				ThreadDisposing(this, e);
		}

		/// <summary>
		/// Does work in the thread.
		/// </summary>
		protected abstract bool DoWork();

		/// <summary>
		/// Returns true if the thread is doing work.
		/// </summary>
		// mbr - 14-04-2006 - added.		
		public ThreadedServiceEngineState GetState()
		{
			lock(_workStatedAtLock)
			{
				if(this._workStartedAt == DateTime.MinValue)
					return new ThreadedServiceEngineState(false, new TimeSpan(0,0,0));
				else
					return new ThreadedServiceEngineState(true, DateTime.UtcNow - _workStartedAt);
			}
		}

		/// <summary>
		/// Entry point for the thread.
		/// </summary>
		private void ThreadEntryPoint()
		{
			try
			{
				while(true)
				{
					try
					{
						// cancel?
						if(this.IsCancelled)
							break;

						// mbr - 12-11-2005 - if do work returns false, tear down the thread...
						// mbr - 14-04-2006 - added notion of the thread running for a period of time.				
						lock(_workStatedAtLock)
							_workStartedAt = DateTime.UtcNow;
						try
						{
							if(!(DoWork()))
								break;
						}
						finally
						{
							lock(_workStatedAtLock)
								_workStartedAt = DateTime.MinValue;
						}

						// cancel?
						if(this.IsCancelled)
							break;

						// wait...
						this.TickEvent.Reset();
						this.TickEvent.WaitOne(this.TickPeriod, false);
					}
					catch(ThreadAbortException)
					{
						if(this.Log.IsInfoEnabled)
							this.Log.Info("Thread was aborted.");
						return;
					}
					catch(Exception ex)
					{
						// critical!
						this.ProcessingErrorOccurred(ex, null);

						// wait...  
						this.TickEvent.Reset();

                        // mbr - 2014-12-04 - stopped this -- made no sense...
                        //this.TickEvent.WaitOne(new TimeSpan(0, 1, 0), false);
					}
				}
			}
			finally
			{
			}
		}

		/// <summary>
		/// Gets the thread.
		/// </summary>
		private Thread Thread
		{
			get
			{
				// returns the value...
				return _thread;
			}
		}

		/// <summary>
		/// Gets the iscancelled.
		/// </summary>
		protected bool IsCancelled
		{
			get
			{
				// returns the value...
				return _isCancelled;
			}
		}

		/// <summary>
		/// Gets the tickevent.
		/// </summary>
		private ManualResetEvent TickEvent
		{
			get
			{
				// returns the value...
				return _tickEvent;
			}
		}

		/// <summary>
		/// Gets the tickperiod.
		/// </summary>
		internal TimeSpan TickPeriod
		{
			get
			{
				// returns the value...
				return _tickPeriod;
			}
		}

		/// <summary>
		/// Ticks the service.
		/// </summary>
		protected void Tick()
		{
			// mbr - 12-11-2005 - ensure that we have a thread...
			this.EnsureThreadInitialized();

			// start...
			if(TickEvent == null)
				throw new InvalidOperationException("TickEvent is null.");
			this.TickEvent.Set();
		}

		/// <summary>
		/// Returns true if the thread is running.
		/// </summary>
		internal bool IsThreadAlive
		{
			get
			{
				if(this.Thread != null)
					return this.Thread.IsAlive;
				else
					return false;
			}
		}

        /// <summary>
        /// Aborts the thread.
        /// </summary>
        public void Abort()
        {
            this.DisposeThread(TimeSpan.MinValue);
        }
	}
}
