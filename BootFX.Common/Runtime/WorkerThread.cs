// BootFX - Application framework for .NET applications
// 
// File: WorkerThread.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>WorkerThread</c>.
	/// </summary>
	public abstract class WorkerThread : Loggable, IDisposable
	{
		/// <summary>
		/// Raised when an error occurs.
		/// </summary>
		public event ThreadExceptionEventHandler UnhandledError;
		
		/// <summary>
		/// Raised when the thread starts.
		/// </summary>
		public event EventHandler ThreadStarted;

		/// <summary>
		/// Raised when the thread finishes.
		/// </summary>
		public event EventHandler ThreadFinished;
		
		/// <summary>
		/// Private field to support <c>TickTimeout</c> property.
		/// </summary>
		private TimeSpan _tickTimeout;
		
		/// <summary>
		/// Private field to support <c>Thread</c> property.
		/// </summary>
		private Thread _thread;
		private object _threadLock = new object();

		/// <summary>
		/// Private field to support <c>TickBlock</c> property.
		/// </summary>
		private ManualResetEvent _tickBlock = new ManualResetEvent(false);
		
		protected WorkerThread(TimeSpan tickTimeout)
		{
			_tickTimeout = tickTimeout;
		}

		~WorkerThread()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Disposes the object.
		/// </summary>
		/// <param name="explicitCall"></param>
		protected virtual void Dispose(bool explicitCall)
		{
			// sup...
			GC.SuppressFinalize(this);
		}

		private void DisposeThread()
		{
			// dispose it...
			lock(_threadLock)
			{
				if(_thread == Thread.CurrentThread)
					throw new InvalidOperationException("A thread cannot dispose itself.");

				// stop...
				_thread.Abort();
				_thread = null;
			}
		}

		/// <summary>
		/// Gets the tickblock.
		/// </summary>
		private ManualResetEvent TickBlock
		{
			get
			{
				// returns the value...
				return _tickBlock;
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
		/// Ticks the thread.
		/// </summary>
		protected void Tick()
		{
			this.TickBlock.Set();
		}

		public void EnsureThreadCreated()
		{
			lock(_threadLock)
			{
				if(_thread == null)
					this.SpinUp();
			}
		}

		/// <summary>
		/// Spins up the thread.
		/// </summary>
		protected void SpinUp()
		{
			lock(_threadLock)
			{
				if(_thread != null)
					throw new InvalidOperationException("Thread has already been started.");

				// create...
				_thread = new Thread(new ThreadStart(ThreadEntryPoint));
				_thread.IsBackground = true;
				_thread.Name = this.ThreadName;

				// run...
				_thread.Start();
			}
		}

		protected virtual string ThreadName
		{
			get
			{
				return string.Format("Thread Worker ({0})", this.GetHashCode());
			}
		}

		private void ThreadEntryPoint()
		{
			// start...
			this.OnThreadStarted();

			// run...
			try
			{
				while(true)
				{
					try
					{
						// do work... a false return quits the thread.
						if(!(DoWork()))
							return;

						// wait...
						this.TickBlock.Reset();
						this.TickBlock.WaitOne(this.TickTimeout, false);
					}
					catch(ThreadAbortException)
					{
						return;
					}
					catch(Exception ex)
					{
						// log...
						if(this.Log.IsErrorEnabled)
							this.Log.Error("An unhandled error occurred when processing thread..", ex);
						this.OnUnhandledError(new ThreadExceptionEventArgs(ex));
					}
				}
			}
			finally
			{
				this.OnThreadFinished();
			}
		}

		/// <summary>
		/// Gets the ticktimeout.
		/// </summary>
		private TimeSpan TickTimeout
		{
			get
			{
				// returns the value...
				return _tickTimeout;
			}
		}

		/// <summary>
		/// Runs work for the thread operation.
		/// </summary>
		protected abstract bool DoWork();

		/// <summary>
		/// Raises the <c>ThreadFinished</c> event.
		/// </summary>
		private void OnThreadFinished()
		{
			OnThreadFinished(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ThreadFinished</c> event.
		/// </summary>
		protected virtual void OnThreadFinished(EventArgs e)
		{
			// raise...
			if(ThreadFinished != null)
				ThreadFinished(this, e);
		}
		
		/// <summary>
		/// Raises the <c>ThreadStarted</c> event.
		/// </summary>
		private void OnThreadStarted()
		{
			OnThreadStarted(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>ThreadStarted</c> event.
		/// </summary>
		protected virtual void OnThreadStarted(EventArgs e)
		{
			// raise...
			if(ThreadStarted != null)
				ThreadStarted(this, e);
		}

		/// <summary>
		/// Raises the <c>Error</c> event.
		/// </summary>
		protected virtual void OnUnhandledError(ThreadExceptionEventArgs e)
		{
			// raise...
			if(UnhandledError != null)
				UnhandledError(this, e);
		}
	}
}
