// BootFX - Application framework for .NET applications
// 
// File: Loggable.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Defines a class that provides easy access to logging.
	/// </summary>
	/// <remarks>To access the default log for the class, use the <c>Log</c> property.</remarks>
	/// <remarks>
	/// You can define any number of logs to refine the consumer's ability to control what information is output.  To do this,
	/// use the <see cref="Logs"/> property passing in the (arbitrary) name of an activity.  Normally, you should wrap this
	/// in a separate property, and if you do that property should be decorated with the <see cref="LogActivityAttribute"/> attribute.  
	/// (This enables the idea of a notional tool that can go through and indicate where the logging points of your library are.)
	/// </remarks>
	/// <example>
	/// <code>
	/// public void Foo()
	/// {
	///		// output to the standard log...
	///		if(Log.IsInfoEnabled)
	///			Log.Info("Some magic happened.");
	///			
	///		// output to a custom log...
	///		if(BarLog.IsInfoEnabled)
	///			BarLog.Info("More magic happened.");
	///	}
	///	
	///	[LogActivity("Bar")]
	///	private ILog BarLog
	///	{
	///		get
	///		{
	///			return Logs["Bar"];
	///		}
	///	}
	/// </code>
	/// </example>
	[Serializable()]
	public abstract class Loggable : ILoggable
	{
		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		/// <remarks>This is not serialized.  The idea being is if this gets transmitted, when it's rehydated
		/// the demand load at the other end will configure it with the other end's logging configuration for
		/// this type.</remarks>
		[NonSerialized()]
		private ILogSet _logs = null;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected Loggable()
		{
		}

		ILog ILoggable.Log
		{
			get
			{
				return this.Log;
			}
		}

		/// <summary>
		/// Gets the default log.
		/// </summary>
		protected ILog Log
		{
			get
			{
				return this.Logs.DefaultLog;
			}
		}

		ILogSet ILoggable.Logs
		{
			get
			{
				return this.Logs;
			}
		}

		/// <summary>
		/// Gets the set of logs for other activities.
		/// </summary>
		protected ILogSet Logs
		{
			get
			{
				// mbr - 11-10-2005 - provided an ability to invalidate logs if the context changes...				
				if(_logs != null && _logs.ContextId != LogSet.CurrentContextId)
					_logs = null;
				if(_logs == null)
					_logs = new LogSet(this.GetType());
				return _logs;
			}
		}

        protected void RunOperation(Action d)
        {
            try
            {
                d();
            }
            catch (Exception ex)
            {
                if (this.Log.IsErrorEnabled)
                    this.Log.Error("The operation failed.", ex);
            }
        }

        protected async Task RunOperationAsync(Func<Task> d)
        {
            try
            {
                await d();
            }
            catch (Exception ex)
            {
                if (this.Log.IsErrorEnabled)
                    this.Log.Error("The operation failed.", ex);
            }
        }

        protected void RunOperationForEach<T>(IEnumerable<T> items, Action<T> d)
        {
            foreach (var item in items)
            {
                try
                {
                    d(item);
                }
                catch (Exception ex)
                {
                    if (this.Log.IsErrorEnabled)
                        this.Log.Error("The operation failed.", ex);
                }
            }
        }

        protected async Task RunOperationForEachAsync<T>(IEnumerable<T> items, Func<T, Task> d)
        {
            foreach (var item in items)
            {
                try
                {
                    await d(item);
                }
                catch (Exception ex)
                {
                    if (this.Log.IsErrorEnabled)
                        this.Log.Error("The operation failed.", ex);
                }
            }
        }

        protected T RunOperation<T>(Func<T> d)
        {
            try
            {
                return d();
            }
            catch (Exception ex)
            {
                if (this.Log.IsInfoEnabled)
                    this.Log.Error("The operation failed.", ex);
                return default(T);
            }
        }

        protected async Task<T> RunOperationAsync<T>(Func<Task<T>> d)
        {
            try
            {
                var result = await d();
                return result;
            }
            catch (Exception ex)
            {
                if (this.Log.IsInfoEnabled)
                    this.Log.Error("The operation failed.", ex);
                return default(T);
            }
        }
    }
}
