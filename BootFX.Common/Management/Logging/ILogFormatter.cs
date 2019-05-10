// BootFX - Application framework for .NET applications
// 
// File: ILogFormatter.cs
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
	/// Defines an interface for log formatters.
	/// </summary>
	public interface ILogFormatter
	{
		/// <summary>
		/// Formats the given message.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="buf"></param>
		/// <returns></returns>
		string Format(ILog log, LogLevel level, LogArgs args);
	}
}
