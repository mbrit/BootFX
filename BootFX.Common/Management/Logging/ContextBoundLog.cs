// BootFX - Application framework for .NET applications
// 
// File: ContextBoundLog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Describes an object that holds logs bound to the current thread.
	/// </summary>
	internal class ContextBoundLog
	{
		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		private IList _logs = new ArrayList();

		/// <summary>
		/// Constructor.
		/// </summary>
		internal ContextBoundLog()
		{
		}

		/// <summary>
		/// Adds to an internal list.
		/// </summary>
		/// <param name="log"></param>
		internal void Bind(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// add...
			if(this.Logs.Contains(log))
				throw new InvalidOperationException(string.Format("Context already contains '{0}'.", log));
			this.Logs.Add(log);
		}

		/// <summary>
		/// Removes an the internal list.
		/// </summary>
		/// <param name="log"></param>
		internal bool Unbind(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			
			// check...
			if(!(this.Logs.Contains(log)))
				throw new InvalidOperationException(string.Format("'{0}' is not bound to the context.", log));
			this.Logs.Remove(log);

			// return...
			if(this.Logs.Count == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the current log.
		/// </summary>
		internal ILog CurrentLog
		{
			get
			{
				if(Logs == null)
					throw new InvalidOperationException("Logs is null.");
				if(this.Logs.Count > 0)
					return (ILog)this.Logs[this.Logs.Count - 1];
				else
					throw new InvalidOperationException("No logs are available.");
			}
		}

		/// <summary>
		/// Gets the logs.
		/// </summary>
		private IList Logs
		{
			get
			{
				// returns the value...
				return _logs;
			}
		}
	}
}
