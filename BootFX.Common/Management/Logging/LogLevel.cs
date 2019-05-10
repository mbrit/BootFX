// BootFX - Application framework for .NET applications
// 
// File: LogLevel.cs
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
	/// Defines log levels.
	/// </summary>
	public enum LogLevel
	{
        Trace = -1,     // mbr - 2013-02-15 - needs to be below debug...
		Debug = 0,
		Info = 1,
		Warn = 2,
		Error = 3,
		Fatal = 4,

		// mbr - 06-02-2007 - audit...
		AuditPending = 5,
		AuditSuccess = 6,
		AuditFailed = 7,

		// mbr - 10-05-2007 - a message written on receipt of a 'phone home' message.		
		PhoneHome = 8
	}
}
