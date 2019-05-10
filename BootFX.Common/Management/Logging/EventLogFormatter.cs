// BootFX - Application framework for .NET applications
// 
// File: EventLogFormatter.cs
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
	/// <summary>
	/// Formatter for event logs.
	/// </summary>
	public class EventLogFormatter : ILogFormatter
	{
		public EventLogFormatter()
		{
		}

		public string Format(ILog log, BootFX.Common.Management.LogLevel level, LogArgs args)
		{
			if(args.HasException)
				return string.Format("{0}\r\n\r\nException:\r\n{1}", args.Message, args.Exception);
			else
				return args.Message + "\r\n\r\n(No exception)";
		}
	}
}
