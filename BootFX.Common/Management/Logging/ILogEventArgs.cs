// BootFX - Application framework for .NET applications
// 
// File: ILogEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Management
{
	public delegate void ILogEventHandler(object sender, ILogEventArgs e);

	/// <summary>
	/// Summary description for ILogEventArgs.
	/// </summary>
	public class ILogEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Log"/> property.
		/// </summary>
		private ILog _log;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="log"></param>
		public ILogEventArgs(ILog log)
		{
			if(log == null)
				throw new ArgumentNullException("log");
			_log = log;
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		public ILog Log
		{
			get
			{
				return _log;
			}
		}
	}
}
