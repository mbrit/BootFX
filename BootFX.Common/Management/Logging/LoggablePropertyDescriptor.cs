// BootFX - Application framework for .NET applications
// 
// File: LoggablePropertyDescriptor.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace BootFX.Common.Management
{
	/// <summary>
	///	 Extension of <see cref="LoggablePropertyDescriptor"/> that implements <see cref="ILoggable"/>.
	/// </summary>
	public abstract class LoggablePropertyDescriptor : PropertyDescriptor, ILoggable
	{
		/// <summary>
		/// Private field to support <c>Logs</c> property.
		/// </summary>
		private ILogSet _logs = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attributes"></param>
		protected LoggablePropertyDescriptor(string propertyName, Attribute[] attributes) : base(propertyName, attributes)
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
	}
}
